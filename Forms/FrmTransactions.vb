Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb

Namespace TempleAccounting
    Partial Public Class FrmTransactions
        Inherits Form

        Private components As IContainer = Nothing
        Private ReadOnly _searchFlow As New List(Of Control)()
        Private _isEditing As Boolean = False
        Private _editingTransactionId As Integer = 0
        Friend WithEvents lblCategory As Label, lblSearch As Label, lblType As Label, lblDate As Label
        Friend WithEvents cboCategory As ComboBox, cboType As ComboBox
        Friend WithEvents txtSearch As TextBox, btnSearch As Button
        Friend WithEvents dtpFrom As DateTimePicker, dtpTo As DateTimePicker
        Friend WithEvents dgvTransactions As DataGridView
        Friend WithEvents btnAddInc As Button, btnAddExp As Button, btnAddTrans As Button
        Friend WithEvents btnEdit As Button, btnDelete As Button, btnRefresh As Button, btnClose As Button
        Friend WithEvents lblSummary As Label, lblHeader As Label

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.Text = "รายการทั้งหมด"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "📋 รายการรับ-จ่ายทั้งหมด"
            lblHeader.Font = New Font("Tahoma", 15.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 64
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            Dim pFilter As New Panel()
            pFilter.BackColor = Color.White
            pFilter.Dock = DockStyle.Top
            pFilter.Height = 130
            pFilter.Padding = New Padding(16)

            lblCategory = New Label() With {.Text = "ประเภท:", .Location = New Point(16, 12), .AutoSize = True}
            cboCategory = New ComboBox() With {.Location = New Point(100, 8), .Size = New Size(220, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}

            lblType = New Label() With {.Text = "ชนิด:", .Location = New Point(340, 12), .AutoSize = True}
            cboType = New ComboBox() With {.Location = New Point(400, 8), .Size = New Size(180, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            cboType.Items.AddRange({"ทั้งหมด", "Income รายรับ", "Expense รายจ่าย", "Transfer โอนภายใน"})
            cboType.SelectedIndex = 0

            lblDate = New Label() With {.Text = "ตั้งแต่วันที่:", .Location = New Point(600, 12), .AutoSize = True}
            dtpFrom = New DateTimePicker() With {.Location = New Point(690, 8), .Size = New Size(180, 40), .Font = New Font("Tahoma", 10.0!), .Value = New Date(Today.Year, Today.Month, 1)}
            dtpTo = New DateTimePicker() With {.Location = New Point(880, 8), .Size = New Size(180, 40), .Font = New Font("Tahoma", 10.0!), .Value = Today}

            lblSearch = New Label() With {.Text = "ค้นหา:", .Location = New Point(16, 58), .AutoSize = True}
            txtSearch = New TextBox() With {.Location = New Point(100, 54), .Size = New Size(480, 40), .Font = New Font("Tahoma", 10.0!)}

            btnSearch = New Button() With {.Text = "🔍 ค้นหา", .Location = New Point(600, 54), .Size = New Size(150, 44), .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnRefresh = New Button() With {.Text = "🔄 รีเฟรช", .Location = New Point(760, 54), .Size = New Size(130, 44), .BackColor = Color.FromArgb(5, 150, 105), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}

            pFilter.Controls.AddRange(New Control() {lblCategory, cboCategory, lblType, cboType, lblDate, dtpFrom, dtpTo, lblSearch, txtSearch, btnSearch, btnRefresh})

            lblSummary = New Label()
            lblSummary.Dock = DockStyle.Top
            lblSummary.Height = 52
            lblSummary.BackColor = Color.FromArgb(254, 240, 138)
            lblSummary.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(69, 26, 3)
            lblSummary.Text = "รายรับเดือนนี้: 0.00 บาท   |   รายจ่าย: 0.00 บาท   |   คงเหลือ: 0.00 บาท   |   โอนภายใน: 0.00 บาท"
            lblSummary.TextAlign = ContentAlignment.MiddleCenter

            dgvTransactions = New DataGridView()
            dgvTransactions.Dock = DockStyle.Fill
            dgvTransactions.BackgroundColor = Color.White
            dgvTransactions.AllowUserToAddRows = False
            dgvTransactions.AllowUserToDeleteRows = False
            dgvTransactions.ReadOnly = True
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvTransactions.BorderStyle = BorderStyle.None
            dgvTransactions.Font = New Font("Tahoma", 10.0!)
            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)
            dgvTransactions.RowTemplate.Height = 34
            dgvTransactions.EditMode = DataGridViewEditMode.EditOnEnter

            Dim pActions As New Panel()
            pActions.Dock = DockStyle.Bottom
            pActions.Height = 80
            pActions.BackColor = Color.FromArgb(245, 240, 220)
            pActions.Padding = New Padding(14, 14, 14, 14)

            btnAddInc = New Button() With {.Text = "➕ รายรับใหม่", .Dock = DockStyle.Left, .Size = New Size(160, 52), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnAddExp = New Button() With {.Text = "➕ รายจ่ายใหม่", .Dock = DockStyle.Left, .Size = New Size(160, 52), .BackColor = Color.FromArgb(190, 18, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnAddTrans = New Button() With {.Text = "🔁 โอนเงินใหม่", .Dock = DockStyle.Left, .Size = New Size(170, 52), .BackColor = Color.FromArgb(126, 34, 206), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnEdit = New Button() With {.Text = "📝 แก้ไข", .Dock = DockStyle.Left, .Size = New Size(120, 52), .BackColor = Color.FromArgb(245, 158, 11), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnDelete = New Button() With {.Text = "🗑️ ลบรายการ", .Dock = DockStyle.Left, .Size = New Size(150, 52), .BackColor = Color.FromArgb(153, 27, 27), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnClose = New Button() With {.Text = "ปิดหน้านี้", .Dock = DockStyle.Right, .Size = New Size(140, 52), .BackColor = Color.FromArgb(75, 85, 99), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}

            pActions.Controls.AddRange(New Control() {btnClose, btnDelete, btnEdit, btnAddTrans, btnAddExp, btnAddInc})

            ' Order: Actions Bottom, Summary, Filter, Header
            Me.Controls.Add(dgvTransactions)
            Me.Controls.Add(pActions)
            Me.Controls.Add(lblSummary)
            Me.Controls.Add(pFilter)
            Me.Controls.Add(lblHeader)
        End Sub

        Private Sub FrmTransactions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            LoadFilters()
            SetupSearchEnterNavigation()
            LoadData()
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
            End Using
        End Sub

        Private Sub LoadData()
            Dim keepSelectedId As Integer = If(_editingTransactionId > 0, _editingTransactionId, GetSelectedTransactionId())

            Using conn = Db.OpenConn()
                Dim sql = "SELECT t.ID, t.TranDate, t.TranType, t.CategoryID, IIF(c.CategoryName IS NULL,'',c.CategoryName) AS CategoryName, " &
                          "t.FundID, IIF(f.FundName IS NULL,'',f.FundName) AS FundName, " &
                          "t.BankID, IIF(b.BankName IS NULL,'',b.BankName & IIF(b.AccountNo IS NULL,'',' ' & b.AccountNo)) AS BankName, " &
                          "t.Detail, t.Amount, t.Note, t.CreateDate, t.ToFundID, IIF(f2.FundName IS NULL,'',f2.FundName) AS ToFundName, " &
                          "t.ToBankID, IIF(b2.BankName IS NULL,'',b2.BankName & IIF(b2.AccountNo IS NULL,'',' ' & b2.AccountNo)) AS ToBankName " &
                          "FROM ((((Transactions t " &
                          "LEFT JOIN Categories c ON t.CategoryID=c.ID) " &
                          "LEFT JOIN Funds f ON t.FundID=f.ID) " &
                          "LEFT JOIN BankAccounts b ON t.BankID=b.ID) " &
                          "LEFT JOIN Funds f2 ON t.ToFundID=f2.ID) " &
                          "LEFT JOIN BankAccounts b2 ON t.ToBankID=b2.ID " &
                          "WHERE t.TranDate BETWEEN @d1 AND @d2 "

                Dim ps As New List(Of Tuple(Of String, Object))
                ps.Add(New Tuple(Of String, Object)("@d1", dtpFrom.Value.Date))
                ps.Add(New Tuple(Of String, Object)("@d2", dtpTo.Value.Date.AddDays(1).AddSeconds(-1)))

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
                sql &= " ORDER BY t.TranDate DESC, t.ID DESC"

                Dim dt = Db.GetTable(conn, sql, ps.ToArray())
                dgvTransactions.DataSource = dt
                If dgvTransactions.Columns.Count > 0 Then
                    ConfigureGridColumns()
                End If
                dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

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

            RestoreSelectionById(keepSelectedId)
            If _isEditing Then
                ApplyEditModeToGrid()
            End If
        End Sub

        Private Sub ConfigureGridColumns()
            Dim headers As New Dictionary(Of String, String) From {
                {"ID", "ID"},
                {"TranDate", "TranDate"},
                {"TranType", "TranType"},
                {"CategoryID", "CategoryID"},
                {"CategoryName", "Category"},
                {"FundID", "FundID"},
                {"FundName", "Fund"},
                {"BankID", "BankID"},
                {"BankName", "Bank"},
                {"Detail", "Detail"},
                {"Amount", "Amount"},
                {"Note", "Note"},
                {"CreateDate", "CreateDate"},
                {"ToFundID", "ToFundID"},
                {"ToFundName", "ToFund"},
                {"ToBankID", "ToBankID"},
                {"ToBankName", "ToBank"}
            }

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
            For Each readOnlyName In New String() {"CategoryName", "FundName", "BankName", "ToFundName", "ToBankName"}
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
    End Class
End Namespace
