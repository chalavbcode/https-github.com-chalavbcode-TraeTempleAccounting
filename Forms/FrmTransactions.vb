Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.Json
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb

Namespace TempleAccounting
    Partial Public Class FrmTransactions
        Inherits Form

        Private ReadOnly _searchFlow As New List(Of Control)()
        Private _isEditing As Boolean = False
        Private _editingTransactionId As Integer = 0

#Region "debug-point Z:debug-report"
        Private Const DebugSessionId As String = "transactions-grid-empty"
        Private Const DebugRunId As String = "post-fix"

        Private Shared Function FindUpwards(startDir As String, relativePath As String) As String
            Try
                Dim dir = startDir
                If String.IsNullOrWhiteSpace(dir) Then Return ""
                For i As Integer = 0 To 12
                    Dim candidate = Path.Combine(dir, relativePath)
                    If File.Exists(candidate) Then Return candidate
                    Dim parent = Directory.GetParent(dir)
                    If parent Is Nothing Then Exit For
                    dir = parent.FullName
                Next
            Catch
            End Try
            Return ""
        End Function

        Private Shared Function GetDebugServerUrl() As String
            Dim fallback = "http://127.0.0.1:7777/event"
            Try
                Dim envPath = FindUpwards(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(".dbg", DebugSessionId & ".env"))
                If String.IsNullOrWhiteSpace(envPath) OrElse Not File.Exists(envPath) Then Return fallback
                Dim lines = File.ReadAllLines(envPath, Encoding.UTF8)
                For Each line In lines
                    If line.StartsWith("DEBUG_SERVER_URL=", StringComparison.OrdinalIgnoreCase) Then
                        Dim url = line.Substring("DEBUG_SERVER_URL=".Length).Trim()
                        If url <> "" Then Return url
                    End If
                Next
            Catch
            End Try
            Return fallback
        End Function

        Private Shared Sub DebugReport(hypothesisId As String, location As String, msg As String, data As Dictionary(Of String, Object))
            Try
                Dim url = GetDebugServerUrl()
                Dim payload As New Dictionary(Of String, Object) From {
                    {"sessionId", DebugSessionId},
                    {"runId", DebugRunId},
                    {"hypothesisId", hypothesisId},
                    {"location", location},
                    {"msg", "[DEBUG] " & msg},
                    {"data", If(data, New Dictionary(Of String, Object)())},
                    {"ts", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}
                }

                Dim json = JsonSerializer.Serialize(payload)
                Dim bytes = Encoding.UTF8.GetBytes(json)
                Dim req = CType(WebRequest.Create(url), HttpWebRequest)
                req.Method = "POST"
                req.ContentType = "application/json"
                req.Timeout = 500
                Using s = req.GetRequestStream()
                    s.Write(bytes, 0, bytes.Length)
                End Using
                Using resp = CType(req.GetResponse(), HttpWebResponse)
                End Using
            Catch
            End Try
        End Sub
#End Region

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FrmTransactions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ' SetupRuntimeLayout() ' ลบออกเพื่อให้ใช้ค่าจาก Designer
            SetupToolTips()
#Region "debug-point A:form-load"
            DebugReport("A", "FrmTransactions_Load", "load-start", New Dictionary(Of String, Object) From {
                {"baseDir", AppDomain.CurrentDomain.BaseDirectory},
                {"dbPath", AppPaths.DatabaseFile},
                {"dbExists", File.Exists(AppPaths.DatabaseFile)}
            })
#End Region
            Db.EnsureSchema()
#Region "debug-point A:ensure-schema"
            DebugReport("A", "FrmTransactions_Load", "ensure-schema-ok", New Dictionary(Of String, Object) From {
                {"dbPath", AppPaths.DatabaseFile},
                {"dbExists", File.Exists(AppPaths.DatabaseFile)}
            })
#End Region
            LoadFilters()
            SetupSearchEnterNavigation()
            LoadData()
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSearch, "ค้นหารายการตามช่วงวันที่ ประเภท และคำค้นหาที่ระบุ")
            ttMain.SetToolTip(btnRefresh, "ล้างการค้นหาและดึงข้อมูลใหม่ทั้งหมด")
            ttMain.SetToolTip(btnAddInc, "เปิดหน้าจอสำหรับบันทึกรายรับใหม่")
            ttMain.SetToolTip(btnAddExp, "เปิดหน้าจอสำหรับบันทึกรายจ่ายใหม่")
            ttMain.SetToolTip(btnAddTrans, "เปิดหน้าจอสำหรับบันทึกการโอนเงินภายใน")
            ttMain.SetToolTip(btnEdit, "แก้ไขข้อมูลรายการที่เลือกในตาราง (กดซ้ำเพื่อบันทึก)")
            ttMain.SetToolTip(btnDelete, "ลบรายการที่เลือกออกจากฐานข้อมูล")
            ttMain.SetToolTip(btnClose, "ปิดหน้าจอรายการนี้และกลับไปหน้าหลัก")
        End Sub

        Private Sub SetupSearchEnterNavigation()
            If _searchFlow.Count > 0 Then Return
            _searchFlow.AddRange({dtpFrom, dtpTo, cboCategory, cboType, txtSearch, btnSearch})
            For Each ctrl In _searchFlow
                AddHandler ctrl.KeyDown, AddressOf HandleSearchEnterAdvance
            Next
        End Sub

        Private Sub MoveNextSearchFrom(current As Control)
            Dim idx = _searchFlow.IndexOf(current)
            If idx < 0 Then Return
            If idx = _searchFlow.Count - 1 Then
                btnSearch.PerformClick()
                Return
            End If

            Dim nextCtrl = _searchFlow(idx + 1)
            nextCtrl.Focus()
            Dim cb = TryCast(nextCtrl, ComboBox)
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then cb.DroppedDown = True
            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then tb.SelectAll()
        End Sub

        Private Sub HandleSearchEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextSearchFrom(DirectCast(sender, Control))
        End Sub

        Private Sub LoadFilters()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT ID, CategoryName FROM Categories ORDER BY CategoryName")
                Dim dr = dt.NewRow() : dr("ID") = 0 : dr("CategoryName") = "ทุกประเภท"
                dt.Rows.InsertAt(dr, 0)
                cboCategory.DisplayMember = "CategoryName"
                cboCategory.ValueMember = "ID"
                cboCategory.DataSource = dt
                cboCategory.SelectedIndex = 0

                Dim bounds = Db.GetTable(conn, "SELECT MIN(IIF(Year(TranDate)>2400, DateAdd('yyyy',-543,TranDate), TranDate)) AS MinTranDate, MAX(IIF(Year(TranDate)>2400, DateAdd('yyyy',-543,TranDate), TranDate)) AS MaxTranDate FROM Transactions")
                If bounds.Rows.Count > 0 Then
                    Dim minValue = bounds.Rows(0)("MinTranDate")
                    Dim maxValue = bounds.Rows(0)("MaxTranDate")

                    If minValue IsNot DBNull.Value Then
                        dtpFrom.Value = Db.NormalizeGregorianDate(Convert.ToDateTime(minValue))
                    End If

                    If maxValue IsNot DBNull.Value Then
                        dtpTo.Value = Db.NormalizeGregorianDate(Convert.ToDateTime(maxValue))
                    Else
                        dtpTo.Value = Db.NormalizeGregorianDate(Date.Today)
                    End If
                Else
                    dtpFrom.Value = New Date(Date.Today.Year, 1, 1)
                    dtpTo.Value = Db.NormalizeGregorianDate(Date.Today)
                End If
            End Using

            cboType.SelectedIndex = 0
            txtSearch.Clear()
        End Sub

        Private Sub LoadData()
            Dim keepSelectedId As Integer = If(_editingTransactionId > 0, _editingTransactionId, GetSelectedTransactionId())

            Try
#Region "debug-point B:loaddata-start"
                DebugReport("B", "LoadData", "loaddata-start", New Dictionary(Of String, Object) From {
                    {"dbPath", AppPaths.DatabaseFile},
                    {"dbExists", File.Exists(AppPaths.DatabaseFile)},
                    {"from", dtpFrom.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},
                    {"to", dtpTo.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},
                    {"fromYear", dtpFrom.Value.Year},
                    {"toYear", dtpTo.Value.Year},
                    {"categoryId", If(cboCategory.SelectedValue, 0)},
                    {"typeIndex", cboType.SelectedIndex},
                    {"search", txtSearch.Text}
                })
#End Region

                Using conn = Db.OpenConn()
                    Dim tranDateExpr = "IIF(Year(t.TranDate)>2400, DateAdd('yyyy',-543,t.TranDate), t.TranDate)"
                    Dim d1 = dtpFrom.Value.Date
                    Dim d2 = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)
                    Dim d1Literal = Db.AccessDateLiteral(d1)
                    Dim d2Literal = Db.AccessDateLiteral(d2)

#Region "debug-point B:db-stats"
                    Dim totalCount = Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions")
                    Dim minDb = Db.DbScalar(conn, "SELECT MIN(TranDate) FROM Transactions")
                    Dim maxDb = Db.DbScalar(conn, "SELECT MAX(TranDate) FROM Transactions")
                    Dim minYearDb = Db.DbScalar(conn, "SELECT MIN(Year(TranDate)) FROM Transactions")
                    Dim maxYearDb = Db.DbScalar(conn, "SELECT MAX(Year(TranDate)) FROM Transactions")
                    Dim rawBetweenCount = Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranDate BETWEEN " & d1Literal & " AND " & d2Literal)
                    Dim exprBetweenCount = Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions t WHERE " & tranDateExpr & " BETWEEN " & d1Literal & " AND " & d2Literal)
                    DebugReport("B", "LoadData", "db-stats", New Dictionary(Of String, Object) From {
                        {"totalCount", totalCount},
                        {"minDb", If(minDb, "")},
                        {"maxDb", If(maxDb, "")},
                        {"minYearDb", If(minYearDb, "")},
                        {"maxYearDb", If(maxYearDb, "")},
                        {"rawBetweenCount", rawBetweenCount},
                        {"exprBetweenCount", exprBetweenCount}
                    })
#End Region

                    Dim sql = "SELECT t.ID, t.TranDate, t.TranType, IIF(t.TranType='Income','รายรับ',IIF(t.TranType='Expense','รายจ่าย','โอนภายใน')) AS TranTypeDisplay, t.CategoryID, IIF(c.CategoryName IS NULL,'',c.CategoryName) AS CategoryName, " &
                              "t.FundID, IIF(f.FundName IS NULL,'',f.FundName) AS FundName, " &
                              "t.BankID, IIF(b.BankName IS NULL,'',b.BankName & IIF(b.AccountNo IS NULL,'',' ' & b.AccountNo)) AS BankName, " &
                              "t.Detail, t.Amount, t.Note, t.CreateDate, t.ToFundID, IIF(f2.FundName IS NULL,'',f2.FundName) AS ToFundName, " &
                              "t.ToBankID, IIF(b2.BankName IS NULL,'',b2.BankName & IIF(b2.AccountNo IS NULL,'',' ' & b2.AccountNo)) AS ToBankName, " &
                              "t.ReceiptPath " &
                              "FROM ((((Transactions t " &
                              "LEFT JOIN Categories c ON t.CategoryID=c.ID) " &
                              "LEFT JOIN Funds f ON t.FundID=f.ID) " &
                              "LEFT JOIN BankAccounts b ON t.BankID=b.ID) " &
                              "LEFT JOIN Funds f2 ON t.ToFundID=f2.ID) " &
                              "LEFT JOIN BankAccounts b2 ON t.ToBankID=b2.ID " &
                              "WHERE " & tranDateExpr & " BETWEEN " & d1Literal & " AND " & d2Literal & " "

                    Dim ps As New List(Of Tuple(Of String, Object))

                    If cboCategory.SelectedValue IsNot Nothing AndAlso CInt(cboCategory.SelectedValue) <> 0 Then
                        sql &= " AND t.CategoryID=@cat "
                        ps.Add(New Tuple(Of String, Object)("@cat", CInt(cboCategory.SelectedValue)))
                    End If
                    If cboType.SelectedIndex = 1 Then
                        sql &= " AND t.TranType='Income'"
                    ElseIf cboType.SelectedIndex = 2 Then
                        sql &= " AND t.TranType='Expense'"
                    ElseIf cboType.SelectedIndex = 3 Then
                        sql &= " AND t.TranType='Transfer'"
                    End If
                    If Not String.IsNullOrWhiteSpace(txtSearch.Text) Then
                        sql &= " AND (t.Detail LIKE @s OR t.Note LIKE @s OR t.TranType LIKE @s OR c.CategoryName LIKE @s OR f.FundName LIKE @s OR b.BankName LIKE @s OR f2.FundName LIKE @s OR b2.BankName LIKE @s) "
                        ps.Add(New Tuple(Of String, Object)("@s", "*" & txtSearch.Text.Trim() & "*"))
                    End If
                    sql &= " ORDER BY " & tranDateExpr & " DESC, t.ID DESC"

                    Dim dt = Db.GetTable(conn, sql, ps.ToArray())

#Region "debug-point C:loaddata-result"
                    DebugReport("C", "LoadData", "query-result", New Dictionary(Of String, Object) From {
                        {"rowCount", dt.Rows.Count},
                        {"colCount", dt.Columns.Count}
                    })
#End Region

                    dgvTransactions.DataSource = dt
                    If dgvTransactions.Columns.Count > 0 Then
                        ConfigureGridColumns()
                    End If

                    Dim sumInc As Decimal = 0D
                    Dim sumExp As Decimal = 0D
                    Dim sumTrf As Decimal = 0D
                    For Each row As DataRow In dt.Rows
                        Dim amount = Db.ToDecimalOrZero(row("Amount"))
                        Dim tranType = Convert.ToString(row("TranType"))
                        If tranType = "Income" Then
                            sumInc += amount
                        ElseIf tranType = "Expense" Then
                            sumExp += amount
                        ElseIf tranType = "Transfer" Then
                            sumTrf += amount
                        End If
                    Next
                    lblSummary.Text = $"รายรับ: {sumInc:n2} บาท  |  รายจ่าย: {sumExp:n2} บาท  |  คงเหลือ: {(sumInc - sumExp):n2} บาท  |  โอนภายใน: {sumTrf:n2} บาท"
                End Using
            Catch ex As Exception
#Region "debug-point D:loaddata-ex"
                DebugReport("D", "LoadData", "exception", New Dictionary(Of String, Object) From {
                    {"type", ex.GetType().FullName},
                    {"message", ex.Message}
                })
#End Region
                Throw
            End Try

            RestoreSelectionById(keepSelectedId)
            If _isEditing Then
                ApplyEditModeToGrid()
            End If
        End Sub

        Private Sub ConfigureGridColumns()
            Dim headers As New Dictionary(Of String, String) From {
                {"ID", "ID"},
                {"TranDate", "วันที่"},
                {"TranTypeDisplay", "ชนิด"},
                {"CategoryName", "ประเภท"},
                {"FundName", "กองทุน"},
                {"BankName", "ธนาคาร"},
                {"Detail", "รายละเอียด"},
                {"Amount", "จำนวนเงิน"},
                {"Note", "หมายเหตุ"},
                {"CreateDate", "วันที่บันทึก"},
                {"ToFundName", "ไปยังกองทุน"},
                {"ToBankName", "ไปยังธนาคาร"},
                {"ReceiptPath", "เอกสารแนบ"}
            }

            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            dgvTransactions.ScrollBars = ScrollBars.Both

            ' Hide technical columns but keep them for logic
            For Each colName In New String() {"TranType", "CategoryID", "FundID", "BankID", "ToFundID", "ToBankID", "ReceiptPath"}
                If dgvTransactions.Columns.Contains(colName) Then
                    dgvTransactions.Columns(colName).Visible = False
                End If
            Next

            For Each pair In headers
                If dgvTransactions.Columns.Contains(pair.Key) Then
                    dgvTransactions.Columns(pair.Key).HeaderText = pair.Value
                End If
            Next
            If dgvTransactions.Columns.Contains("TranDate") Then
                dgvTransactions.Columns("TranDate").DefaultCellStyle.Format = "dd/MM/yyyy"
            End If
            If dgvTransactions.Columns.Contains("CreateDate") Then
                dgvTransactions.Columns("CreateDate").DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss"
            End If

            If dgvTransactions.Columns.Contains("Amount") Then
                dgvTransactions.Columns("Amount").DefaultCellStyle.Format = "N2"
                dgvTransactions.Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If

            For Each readOnlyName In New String() {"CategoryName", "FundName", "BankName", "ToFundName", "ToBankName"}
                If dgvTransactions.Columns.Contains(readOnlyName) Then
                    dgvTransactions.Columns(readOnlyName).ReadOnly = True
                    dgvTransactions.Columns(readOnlyName).DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
                End If
            Next

            Dim widths As New Dictionary(Of String, Integer) From {
                {"ID", 70},
                {"TranDate", 95},
                {"TranType", 90},
                {"CategoryID", 90},
                {"CategoryName", 160},
                {"FundID", 90},
                {"FundName", 150},
                {"BankID", 90},
                {"BankName", 180},
                {"Detail", 240},
                {"Amount", 120},
                {"Note", 220},
                {"CreateDate", 150},
                {"ToFundID", 90},
                {"ToFundName", 150},
                {"ToBankID", 90},
                {"ToBankName", 180}
            }

            For Each kv In widths
                If dgvTransactions.Columns.Contains(kv.Key) Then
                    Dim col = dgvTransactions.Columns(kv.Key)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    col.Width = kv.Value
                    col.MinimumWidth = Math.Min(90, kv.Value)
                End If
            Next
        End Sub

        Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click, btnSearch.Click
            If sender Is btnRefresh AndAlso _isEditing Then
                ExitEditMode()
            End If
            LoadData()
        End Sub

        Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            If _isEditing Then
                MessageBox.Show("กรุณาบันทึกหรือยกเลิกการแก้ไขก่อนลบรายการ", "แจ้งเตือน") : Return
            End If
            If dgvTransactions.CurrentRow Is Nothing Then MessageBox.Show("กรุณาเลือกรายการที่จะลบ", "แจ้งเตือน") : Return
            Dim id = CInt(dgvTransactions.CurrentRow.Cells("ID").Value)
            If MessageBox.Show("คุณแน่ใจว่าจะลบรายการนี้ใช่หรือไม่?", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "DELETE FROM Transactions WHERE ID=@id", New Tuple(Of String, Object)("@id", id))
                MessageBox.Show("ลบรายการแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadData()
            End Using
        End Sub

        Private Sub btnAddInc_Click(sender As Object, e As EventArgs) Handles btnAddInc.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmIncome(), "💰 บันทึกรายรับ")
            End If
        End Sub

        Private Sub btnAddExp_Click(sender As Object, e As EventArgs) Handles btnAddExp.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmExpense(), "💸 บันทึกรายจ่าย")
            End If
        End Sub

        Private Sub btnAddTrans_Click(sender As Object, e As EventArgs) Handles btnAddTrans.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmTransfer(), "🔁 โอนเงินภายใน")
            End If
        End Sub

        Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
            If _isEditing Then
                SaveSelectedRowEdits()
                Return
            End If

            If dgvTransactions.CurrentRow Is Nothing Then
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการแก้ไขก่อน", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            _editingTransactionId = GetSelectedTransactionId()
            If _editingTransactionId <= 0 Then
                MessageBox.Show("ไม่พบรหัสรายการที่จะแก้ไข", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            _isEditing = True
            ApplyEditModeToGrid()
            MessageBox.Show("แก้ไขข้อมูลในแถวที่เลือกได้เลย แล้วกดปุ่ม 'บันทึกแก้ไข' เพื่อบันทึก", "โหมดแก้ไข", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.CloseActiveForm()
                f.ShowDashboard()
            End If
        End Sub

        Private Sub ApplyEditModeToGrid()
            dgvTransactions.ReadOnly = False
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.CellSelect
            btnEdit.Text = "💾 บันทึกแก้ไข"
            btnRefresh.Text = "↩ ยกเลิกแก้ไข"

            For Each col As DataGridViewColumn In dgvTransactions.Columns
                col.ReadOnly = False
            Next

            If dgvTransactions.Columns.Contains("ID") Then dgvTransactions.Columns("ID").ReadOnly = True
            If dgvTransactions.Columns.Contains("CreateDate") Then dgvTransactions.Columns("CreateDate").ReadOnly = True
            For Each readOnlyName In New String() {"TranTypeDisplay", "CategoryName", "FundName", "BankName", "ToFundName", "ToBankName"}
                If dgvTransactions.Columns.Contains(readOnlyName) Then dgvTransactions.Columns(readOnlyName).ReadOnly = True
            Next

            If dgvTransactions.CurrentRow IsNot Nothing AndAlso dgvTransactions.Columns.Contains("TranDate") Then
                dgvTransactions.CurrentCell = dgvTransactions.CurrentRow.Cells("TranDate")
                dgvTransactions.BeginEdit(True)
            End If
        End Sub

        Private Sub ExitEditMode()
            _isEditing = False
            _editingTransactionId = 0
            dgvTransactions.EndEdit()
            dgvTransactions.ReadOnly = True
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            btnEdit.Text = "📝 แก้ไข"
            btnRefresh.Text = "🔄 รีเฟรช"
        End Sub

        Private Function GetSelectedTransactionId() As Integer
            If dgvTransactions.CurrentRow Is Nothing OrElse Not dgvTransactions.Columns.Contains("ID") Then Return 0
            Return Db.ToIntOrZero(dgvTransactions.CurrentRow.Cells("ID").Value)
        End Function

        Private Sub RestoreSelectionById(id As Integer)
            If id <= 0 OrElse dgvTransactions.Rows.Count = 0 OrElse Not dgvTransactions.Columns.Contains("ID") Then Return
            For Each row As DataGridViewRow In dgvTransactions.Rows
                If Db.ToIntOrZero(row.Cells("ID").Value) = id Then
                    row.Selected = True
                    dgvTransactions.CurrentCell = row.Cells(Math.Min(1, row.Cells.Count - 1))
                    Exit For
                End If
            Next
        End Sub

        Private Function CellValueOrDbNull(row As DataGridViewRow, columnName As String) As Object
            If Not dgvTransactions.Columns.Contains(columnName) Then Return DBNull.Value
            Dim value = row.Cells(columnName).Value
            If value Is Nothing Then Return DBNull.Value
            Dim text = Convert.ToString(value).Trim()
            If text = "" Then Return DBNull.Value
            Return value
        End Function

        Private Function NullableIntFromCell(row As DataGridViewRow, columnName As String) As Object
            Dim value = CellValueOrDbNull(row, columnName)
            If value Is DBNull.Value Then Return DBNull.Value
            Dim parsed As Integer
            If Integer.TryParse(Convert.ToString(value), parsed) Then Return parsed
            Throw New ApplicationException(columnName & " ต้องเป็นตัวเลข")
        End Function

        Private Function DateFromCell(row As DataGridViewRow, columnName As String) As Date
            Dim value = CellValueOrDbNull(row, columnName)
            If value Is DBNull.Value Then Throw New ApplicationException(columnName & " ต้องมีค่า")
            Return Db.NormalizeGregorianDate(Convert.ToDateTime(value))
        End Function

        Private Function DecimalFromCell(row As DataGridViewRow, columnName As String) As Decimal
            Dim value = CellValueOrDbNull(row, columnName)
            Dim parsed As Decimal
            If value Is DBNull.Value OrElse Not Decimal.TryParse(Convert.ToString(value), parsed) Then
                Throw New ApplicationException(columnName & " ต้องเป็นตัวเลข")
            End If
            If parsed < 0D Then Throw New ApplicationException(columnName & " ต้องไม่ติดลบ")
            Return parsed
        End Function

        Private Function TextFromCell(row As DataGridViewRow, columnName As String) As String
            Dim value = CellValueOrDbNull(row, columnName)
            If value Is DBNull.Value Then Return ""
            Return Convert.ToString(value).Trim()
        End Function

        Private Function NullableIntToInteger(value As Object) As Integer?
            If value Is DBNull.Value Then Return Nothing
            Return Convert.ToInt32(value)
        End Function

        Private Sub ValidateTransactionByType(tranType As String,
                                              categoryId As Object,
                                              fundId As Object,
                                              bankId As Object,
                                              detail As String,
                                              amount As Decimal,
                                              toFundId As Object,
                                              toBankId As Object)
            If String.IsNullOrWhiteSpace(detail) Then
                Throw New ApplicationException("Detail ต้องมีค่า")
            End If
            If amount <= 0D Then
                Throw New ApplicationException("Amount ต้องมากกว่า 0")
            End If

            Dim fromFund = NullableIntToInteger(fundId)
            Dim fromBank = NullableIntToInteger(bankId)
            Dim toFund = NullableIntToInteger(toFundId)
            Dim toBank = NullableIntToInteger(toBankId)
            Dim category = NullableIntToInteger(categoryId)

            Select Case tranType
                Case "Income", "Expense"
                    If Not category.HasValue Then
                        Throw New ApplicationException("รายการรับ/จ่ายต้องมี CategoryID")
                    End If
                    If Not fromFund.HasValue Then
                        Throw New ApplicationException("รายการรับ/จ่ายต้องมี FundID")
                    End If
                    If toFund.HasValue OrElse toBank.HasValue Then
                        Throw New ApplicationException("รายการรับ/จ่ายทั่วไปไม่ควรมี ToFundID หรือ ToBankID")
                    End If

                Case "Transfer"
                    If category.HasValue Then
                        Throw New ApplicationException("รายการโอนไม่ควรมี CategoryID")
                    End If
                    If Not fromFund.HasValue AndAlso Not fromBank.HasValue Then
                        Throw New ApplicationException("รายการโอนต้องมีต้นทางอย่างน้อย FundID หรือ BankID")
                    End If
                    If Not toFund.HasValue AndAlso Not toBank.HasValue Then
                        Throw New ApplicationException("รายการโอนต้องมีปลายทางอย่างน้อย ToFundID หรือ ToBankID")
                    End If
                    If fromFund = toFund AndAlso fromBank = toBank Then
                        Throw New ApplicationException("ต้นทางและปลายทางของรายการโอนต้องแตกต่างกัน")
                    End If
            End Select
        End Sub

        Private Sub SaveSelectedRowEdits()
            If dgvTransactions.CurrentRow Is Nothing Then
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการบันทึก", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim row = dgvTransactions.CurrentRow
            Dim id = Db.ToIntOrZero(row.Cells("ID").Value)
            If id <= 0 Then
                MessageBox.Show("ไม่พบรหัสรายการที่จะแก้ไข", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Try
                dgvTransactions.EndEdit()

                Dim tranDate = DateFromCell(row, "TranDate")
                Dim tranType = TextFromCell(row, "TranType")
                If tranType <> "Income" AndAlso tranType <> "Expense" AndAlso tranType <> "Transfer" Then
                    Throw New ApplicationException("TranType ต้องเป็น Income, Expense หรือ Transfer")
                End If

                Dim categoryId = NullableIntFromCell(row, "CategoryID")
                Dim fundId = NullableIntFromCell(row, "FundID")
                Dim bankId = NullableIntFromCell(row, "BankID")
                Dim detail = TextFromCell(row, "Detail")
                Dim amount = DecimalFromCell(row, "Amount")
                Dim note = TextFromCell(row, "Note")
                Dim toFundId = NullableIntFromCell(row, "ToFundID")
                Dim toBankId = NullableIntFromCell(row, "ToBankID")

                ValidateTransactionByType(tranType, categoryId, fundId, bankId, detail, amount, toFundId, toBankId)

                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn,
"UPDATE Transactions SET TranDate=" & Db.AccessDateLiteral(tranDate) & ", TranType=@t, CategoryID=@c, FundID=@f, BankID=@b, [Detail]=@d, Amount=@a, [Note]=@n, ToFundID=@tf, ToBankID=@tb WHERE ID=@id",
New Tuple(Of String, Object)("@t", tranType),
New Tuple(Of String, Object)("@c", categoryId),
New Tuple(Of String, Object)("@f", fundId),
New Tuple(Of String, Object)("@b", bankId),
New Tuple(Of String, Object)("@d", detail),
New Tuple(Of String, Object)("@a", amount),
New Tuple(Of String, Object)("@n", note),
New Tuple(Of String, Object)("@tf", toFundId),
New Tuple(Of String, Object)("@tb", toBankId),
New Tuple(Of String, Object)("@id", id))
                End Using

                MessageBox.Show("บันทึกการแก้ไขเรียบร้อยแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                ExitEditMode()
                LoadData()
            Catch ex As Exception
                MessageBox.Show("บันทึกการแก้ไขไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub dgvTransactions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTransactions.CellContentClick

        End Sub

        Private Sub lblSearch_Click(sender As Object, e As EventArgs) Handles lblSearch.Click

        End Sub

        Private Sub btnViewReceipt_Click(sender As Object, e As EventArgs) Handles btnViewReceipt.Click
            ' 1. ตรวจสอบว่ามีการเลือกแถวใน DataGridView หรือไม่
            If dgvTransactions.CurrentRow Is Nothing Then
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการดูใบเสร็จ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 2. อ่านชื่อไฟล์จากคอลัมน์ ReceiptPath
            Dim fileName As String = Convert.ToString(dgvTransactions.CurrentRow.Cells("ReceiptPath").Value)

            If String.IsNullOrWhiteSpace(fileName) Then
                MessageBox.Show("รายการนี้ไม่มีรูปภาพใบเสร็จแนบไว้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' 3. หาตำแหน่งไฟล์จริงในโฟลเดอร์ Receipts
            Dim fullPath As String = IO.Path.Combine(AppPaths.ReceiptsDir, fileName)

            ' 4. ตรวจสอบไฟล์และสั่งเปิดดูรูปด้วยโปรแกรมมาตรฐานของ Windows
            If IO.File.Exists(fullPath) Then
                Process.Start(New ProcessStartInfo(fullPath) With {.UseShellExecute = True})
            Else
                MessageBox.Show($"ไม่พบไฟล์รูปภาพในระบบ: {fileName}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub
    End Class
End Namespace
