Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb
Imports System.IO

Namespace TempleAccounting
    Partial Public Class FrmReports
        Inherits Form

        Private components As IContainer = Nothing
        Private ReadOnly _filterFlow As New List(Of Control)()
        Friend WithEvents lblHeader As Label
        Friend WithEvents dtpFrom As DateTimePicker, dtpTo As DateTimePicker
        Friend WithEvents cboType As ComboBox, cboFund As ComboBox, cboBank As ComboBox
        Friend WithEvents lbl1, lbl2, lbl3, lbl4, lbl5 As Label
        Friend WithEvents btnSummaryIncome, btnSummaryExpense, btnMonthly, btnLedger, btnPrint, btnRefresh, btnPrintDetail, btnPrintSummary As Button
        Friend WithEvents dgvReport As DataGridView
        Friend WithEvents lblSummary As Label

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.Text = "รายงาน"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "📊 ศูนย์รายงาน (วางแผน RDLC ในรุ่นถัดไป - ตอนนี้ Preview ก่อนพิมพ์ได้)"
            lblHeader.Font = New Font("Tahoma", 13.5!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(24, 83, 63)
            lblHeader.BackColor = Color.FromArgb(167, 243, 208)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 62
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            Dim p = New Panel With {.BackColor = Color.White, .Dock = DockStyle.Top, .Height = 120, .Padding = New Padding(16)}
            lbl1 = MakeLbl("จากวันที่:", New Point(16, 16))
            dtpFrom = New DateTimePicker With {.Location = New Point(130, 12), .Size = New Size(200, 40), .Font = New Font("Tahoma", 10.0!), .Value = New Date(Today.Year, Today.Month, 1)}
            lbl2 = MakeLbl("ถึงวันที่:", New Point(350, 16))
            dtpTo = New DateTimePicker With {.Location = New Point(460, 12), .Size = New Size(200, 40), .Font = New Font("Tahoma", 10.0!), .Value = Today}
            lbl3 = MakeLbl("ประเภท:", New Point(680, 16))
            cboType = New ComboBox With {.Location = New Point(760, 12), .Size = New Size(200, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            cboType.Items.AddRange({"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"}) : cboType.SelectedIndex = 0
            lbl4 = MakeLbl("กองทุน:", New Point(16, 58))
            cboFund = New ComboBox With {.Location = New Point(130, 54), .Size = New Size(260, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            lbl5 = MakeLbl("ธนาคาร:", New Point(410, 58))
            cboBank = New ComboBox With {.Location = New Point(510, 54), .Size = New Size(260, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}

            btnRefresh = New Button With {.Text = "🔍 ดูรายงาน", .Location = New Point(790, 54), .Size = New Size(170, 44), .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            p.Controls.AddRange(New Control() {lbl1, dtpFrom, lbl2, dtpTo, lbl3, cboType, lbl4, cboFund, lbl5, cboBank, btnRefresh})

            Dim pa = New Panel With {.Dock = DockStyle.Top, .Height = 76, .BackColor = Color.FromArgb(245, 240, 220), .Padding = New Padding(14, 14, 14, 14)}
            btnPrintDetail = New Button With {.Text = "📜 พิมพ์รายงานละเอียด", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(185, 28, 28), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnPrintSummary = New Button With {.Text = "🧾 พิมพ์รายงานย่อ", .Dock = DockStyle.Left, .Size = New Size(220, 48), .BackColor = Color.FromArgb(146, 64, 14), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnSummaryIncome = New Button With {.Text = "💵 สรุปรายรับแยกประเภท", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnSummaryExpense = New Button With {.Text = "💸 สรุปรายจ่ายแยกประเภท", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(190, 18, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnMonthly = New Button With {.Text = "📈 รายงานรายเดือน", .Dock = DockStyle.Left, .Size = New Size(220, 48), .BackColor = Color.FromArgb(126, 34, 206), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnLedger = New Button With {.Text = "📒 สมุดรายวัน (แสดงทั้งหมด)", .Dock = DockStyle.Left, .Size = New Size(260, 48), .BackColor = Color.FromArgb(180, 83, 9), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnPrint = New Button With {.Text = "🖨️ ส่งไป Excel", .Dock = DockStyle.Right, .Size = New Size(180, 48), .BackColor = Color.FromArgb(30, 64, 175), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            pa.Controls.AddRange(New Control() {btnPrint, btnLedger, btnMonthly, btnSummaryExpense, btnSummaryIncome, btnPrintSummary, btnPrintDetail})

            lblSummary = New Label With {.Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(253, 224, 71), .Font = New Font("Tahoma", 11.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(69, 26, 3), .TextAlign = ContentAlignment.MiddleCenter, .Text = "รายรับรวม 0.00  |  รายจ่ายรวม 0.00  |  ส่วนเกิน 0.00"}

            dgvReport = New DataGridView With {.Dock = DockStyle.Fill, .BackgroundColor = Color.White, .ReadOnly = True, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .BorderStyle = BorderStyle.None, .Font = New Font("Tahoma", 9.5!), .RowTemplate = New DataGridViewRow() With {.Height = 30}}
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)

            ' Order Bottom -> Top
            Me.Controls.Add(dgvReport)
            Me.Controls.Add(lblSummary)
            Me.Controls.Add(pa)
            Me.Controls.Add(p)
            Me.Controls.Add(lblHeader)
        End Sub

        Private Function MakeLbl(t As String, p As Point) As Label
            Return New Label With {.Text = t, .Location = p, .AutoSize = True, .ForeColor = Color.FromArgb(69, 26, 3)}
        End Function

        Private Sub FrmReports_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                Db.EnsureSchema()
                Using conn = Db.OpenConn()
                    Dim ft = Db.GetTable(conn, "SELECT 0 AS ID, '(ทุกกองทุน)' AS FundName FROM (SELECT COUNT(*) FROM Funds) C1 UNION ALL SELECT ID, FundName FROM Funds ORDER BY ID")
                    cboFund.DisplayMember = "FundName" : cboFund.ValueMember = "ID" : cboFund.DataSource = ft
                    Dim bt = Db.GetTable(conn, "SELECT 0 AS ID, '(ทุกบัญชี)' AS Disp FROM (SELECT COUNT(*) FROM BankAccounts) C1 UNION ALL SELECT ID, BankName & ' - ' & IIF(AccountNo IS NULL,'',AccountNo) AS Disp FROM BankAccounts ORDER BY ID")
                    cboBank.DisplayMember = "Disp" : cboBank.ValueMember = "ID" : cboBank.DataSource = bt
                End Using
                SetupFilterEnterNavigation()
                btnLedger_Click(Nothing, EventArgs.Empty)
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub SetupFilterEnterNavigation()
            If _filterFlow.Count > 0 Then Return
            _filterFlow.AddRange({dtpFrom, dtpTo, cboType, cboFund, cboBank, btnRefresh})
            For Each ctrl In _filterFlow
                AddHandler ctrl.KeyDown, AddressOf HandleFilterEnterAdvance
            Next
        End Sub

        Private Sub MoveNextFilterFrom(current As Control)
            Dim idx = _filterFlow.IndexOf(current)
            If idx < 0 Then Return
            If idx = _filterFlow.Count - 1 Then
                btnRefresh.PerformClick()
                Return
            End If

            Dim nextCtrl = _filterFlow(idx + 1)
            nextCtrl.Focus()
            Dim cb = TryCast(nextCtrl, ComboBox)
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then cb.DroppedDown = True
        End Sub

        Private Sub HandleFilterEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFilterFrom(DirectCast(sender, Control))
        End Sub

        Private Function BaseSql(ByRef params As List(Of Tuple(Of String, Object))) As String
            params = New List(Of Tuple(Of String, Object))()
            Dim fromDate = Db.NormalizeGregorianDate(dtpFrom.Value.Date)
            Dim toDate = Db.NormalizeGregorianDate(dtpTo.Value.Date)
            Dim sql = "FROM ((Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID) LEFT JOIN Funds f ON t.FundID=f.ID) LEFT JOIN BankAccounts b ON t.BankID=b.ID " &
                      "WHERE DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) " &
                      "BETWEEN " & Db.AccessDateLiteral(fromDate) & " AND " & Db.AccessDateLiteral(toDate)
            If cboFund.SelectedValue IsNot Nothing AndAlso CInt(cboFund.SelectedValue) <> 0 Then
                sql &= " AND t.FundID=@f"
                params.Add(New Tuple(Of String, Object)("@f", CInt(cboFund.SelectedValue)))
            End If
            If cboBank.SelectedValue IsNot Nothing AndAlso CInt(cboBank.SelectedValue) <> 0 Then
                sql &= " AND t.BankID=@b"
                params.Add(New Tuple(Of String, Object)("@b", CInt(cboBank.SelectedValue)))
            End If
            Select Case cboType.SelectedIndex
                Case 1 : sql &= " AND t.TranType='Income'"
                Case 2 : sql &= " AND t.TranType='Expense'"
                Case 3 : sql &= " AND t.TranType='Transfer'"
            End Select
            Return sql
        End Function

        Private Sub btnLedger_Click(sender As Object, e As EventArgs) Handles btnLedger.Click, btnRefresh.Click
            Try
                Dim ps As List(Of Tuple(Of String, Object)) = Nothing
                Dim fromWhere = BaseSql(ps)
                Dim sel = "SELECT DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) AS วันที่, IIF(t.TranType='Income','💰 รับ',IIF(t.TranType='Expense','💸 จ่าย','🔁 โอน')) AS ชนิด, " &
                           "IIF(c.CategoryName IS NULL,'-',c.CategoryName) AS ประเภท, IIF(f.FundName IS NULL,'-',f.FundName) AS กองทุน, IIF(b.ID IS NULL,'-',b.BankName & ' ' & IIF(b.AccountNo IS NULL,'',b.AccountNo)) AS ธนาคาร, t.Detail AS รายละเอียด, " &
                           "IIF(t.TranType='Income',t.Amount,0) AS รายรับ, IIF(t.TranType='Expense',t.Amount,0) AS รายจ่าย, IIF(t.TranType='Transfer',t.Amount,0) AS โอน, t.Note AS หมายเหตุ "
                Dim sq = sel & " " & fromWhere & " ORDER BY t.TranDate, t.ID"
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, sq, ps.ToArray())
                    dgvReport.DataSource = dt
                    Dim sumI = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(IIF(t.TranType='Income',t.Amount,0)) " & fromWhere, ps.ToArray()))
                    Dim sumE = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(IIF(t.TranType='Expense',t.Amount,0)) " & fromWhere, ps.ToArray()))
                    lblSummary.Text = $"รายรับรวม {sumI:n2} บาท  |  รายจ่ายรวม {sumE:n2} บาท  |  ส่วนเกิน/ขาด {sumI - sumE:n2} บาท"
                End Using
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub btnSummaryIncome_Click(sender As Object, e As EventArgs) Handles btnSummaryIncome.Click
            Dim ps As List(Of Tuple(Of String, Object)) = Nothing
            Dim fromWhere = BaseSql(ps)
            fromWhere = fromWhere.Replace("WHERE", "WHERE t.TranType='Income' AND ")
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) AS ประเภทรายรับ, SUM(t.Amount) AS จำนวนเงิน " & fromWhere & " GROUP BY IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) ORDER BY SUM(t.Amount) DESC", ps.ToArray())
                dgvReport.DataSource = dt
                Dim tot = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(t.Amount) " & fromWhere, ps.ToArray()))
                lblSummary.Text = $"รายรับรวมในช่วง = {tot:n2} บาท (แยกตามประเภท)"
            End Using
        End Sub
        Private Sub btnSummaryExpense_Click(sender As Object, e As EventArgs) Handles btnSummaryExpense.Click
            Dim ps As List(Of Tuple(Of String, Object)) = Nothing
            Dim fromWhere = BaseSql(ps)
            fromWhere = fromWhere.Replace("WHERE", "WHERE t.TranType='Expense' AND ")
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) AS ประเภทรายจ่าย, SUM(t.Amount) AS จำนวนเงิน " & fromWhere & " GROUP BY IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) ORDER BY SUM(t.Amount) DESC", ps.ToArray())
                dgvReport.DataSource = dt
                Dim tot = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(t.Amount) " & fromWhere, ps.ToArray()))
                lblSummary.Text = $"รายจ่ายรวมในช่วง = {tot:n2} บาท (แยกตามประเภท)"
            End Using
        End Sub
        Private Sub btnMonthly_Click(sender As Object, e As EventArgs) Handles btnMonthly.Click
            Try
                Dim ps As List(Of Tuple(Of String, Object)) = Nothing
                Dim fromWhere = BaseSql(ps)
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, "SELECT Format(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)), 'yyyy-MM') AS เดือน, " &
                               "SUM(IIF(t.TranType='Income',t.Amount,0)) AS รายรับ, " &
                               "SUM(IIF(t.TranType='Expense',t.Amount,0)) AS รายจ่าย, " &
                               "SUM(IIF(t.TranType='Income',t.Amount,0)) - SUM(IIF(t.TranType='Expense',t.Amount,0)) AS ส่วนเกิน " &
                               fromWhere & " GROUP BY Format(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)), 'yyyy-MM') " &
                               "ORDER BY Format(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)), 'yyyy-MM')", ps.ToArray())
                    dgvReport.DataSource = dt
                    lblSummary.Text = "รายงานสรุปรายเดือน (ช่วงเวลาที่เลือก)"
                End Using
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
            Try
                Dim fn = Path.Combine(AppPaths.ExportFolder, $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv")
                Using sw As New StreamWriter(fn, False, System.Text.Encoding.UTF8)
                    Dim headers As New List(Of String)
                    For Each col As DataGridViewColumn In dgvReport.Columns
                        headers.Add(col.HeaderText)
                    Next
                    sw.WriteLine(String.Join(",", headers.Select(Function(h) """" & h.Replace("""", """""") & """")))
                    For Each r As DataGridViewRow In dgvReport.Rows
                        If r.IsNewRow Then Continue For
                        Dim cells As New List(Of String)
                        For Each c As DataGridViewCell In r.Cells
                            Dim v = If(c.Value Is Nothing, "", c.Value.ToString())
                            cells.Add("""" & v.Replace("""", """""") & """")
                        Next
                        sw.WriteLine(String.Join(",", cells))
                    Next
                End Using
                Clipboard.SetText(fn)
                MessageBox.Show("ส่งออก CSV (เปิดใน Excel ได้เลย) ไว้ที่: " & fn & vbCrLf & "(Path ถูกคัดลอกไปยังคลิปบอร์ดแล้ว)", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("ส่งออกไม่ได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnPrintDetail_Click(sender As Object, e As EventArgs) Handles btnPrintDetail.Click
            Try
                IncomeExpenseReport.ShowPreview(Db.NormalizeGregorianDate(dtpFrom.Value.Date), Db.NormalizeGregorianDate(dtpTo.Value.Date), Me, IncomeExpenseReport.ReportModes.Detailed)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnPrintSummary_Click(sender As Object, e As EventArgs) Handles btnPrintSummary.Click
            Try
                IncomeExpenseReport.ShowPreview(Db.NormalizeGregorianDate(dtpFrom.Value.Date), Db.NormalizeGregorianDate(dtpTo.Value.Date), Me, IncomeExpenseReport.ReportModes.Summary)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
