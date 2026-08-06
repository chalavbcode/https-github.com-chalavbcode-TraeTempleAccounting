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
    <DesignerCategory("Form")>
    Partial Public Class FrmReports
        Inherits Form

        Private ReadOnly _filterFlow As New List(Of Control)()

        Public Sub New()
            InitializeComponent()
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
                SetupToolTips()
                btnLedger_Click(Nothing, EventArgs.Empty)
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(txtBalance, "กรอกยอดยกมาเอง (หากลบให้ว่าง ระบบจะคำนวณให้อัตโนมัติ)")
            ttMain.SetToolTip(btnCalcBalance, "คลิกเพื่อคำนวณยอดยกมาจริงจากฐานข้อมูล (คำนวณจาก รายรับ - รายจ่าย สะสม)")
            ttMain.SetToolTip(btnRefresh, "ดึงข้อมูลรายงานใหม่ตามเงื่อนไขที่เลือก (Enter)")
            ttMain.SetToolTip(btnPrint, "ส่งออกข้อมูลที่แสดงในตารางเป็นไฟล์ Excel (CSV)")
            ttMain.SetToolTip(btnPrintDetail, "พิมพ์รายงานสรุปรายรับ-รายจ่าย แบบแสดงรายละเอียดทุกรายการ")
            ttMain.SetToolTip(btnPrintSummary, "พิมพ์รายงานสรุปรายรับ-รายจ่าย แบบย่อ (แยกตามประเภทรายการ)")
            ttMain.SetToolTip(btnLedger, "แสดงรายงานสมุดบัญชีรายวัน (เรียงตามวันที่)")
            ttMain.SetToolTip(btnSummaryIncome, "แสดงสรุปรายรับแยกตามประเภทรายการ")
            ttMain.SetToolTip(btnSummaryExpense, "แสดงสรุปรายจ่ายแยกตามประเภทรายการ")
            ttMain.SetToolTip(btnMonthly, "แสดงสรุปรายรับ-รายจ่าย แยกเป็นรายเดือน")
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
            ' Ensure we read the date only, and then add full day for toDate
            Dim fDate = dtpFrom.Value.Date
            Dim tDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)
            
            Dim fromDate = Db.NormalizeGregorianDate(fDate)
            Dim toDate = Db.NormalizeGregorianDate(tDate)
            
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

        Private Sub dtpFrom_ValueChanged(sender As Object, e As EventArgs) Handles dtpFrom.ValueChanged
            If Not Me.IsHandleCreated Then Return
            
            ' ป้องกันการถามซ้ำซ้อนตอน Load
            Static lastDate As Date = Date.MinValue
            If dtpFrom.Value.Date = lastDate Then Return
            lastDate = dtpFrom.Value.Date

            Dim result = MessageBox.Show($"คุณเปลี่ยนวันที่เริ่มต้นเป็น {dtpFrom.Value:dd/MM/yyyy} ต้องการกรอก 'ยอดยกมา' เองหรือไม่?{vbCrLf}{vbCrLf}(ถ้าเลือก 'ไม่ใช่' ระบบจะคำนวณจากฐานข้อมูลให้อัตโนมัติ)", "ยอดยกมา", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            
            If result = DialogResult.Yes Then
                txtBalance.Focus()
                txtBalance.SelectAll()
                
                ' ทำให้ช่องกรอกยอดยกมาเด่นขึ้น (Blink effect แบบง่าย)
                Dim originalColor = txtBalance.BackColor
                Dim blinkTimer As New Timer() With {.Interval = 300}
                Dim count = 0
                AddHandler blinkTimer.Tick, Sub()
                    count += 1
                    If count Mod 2 = 1 Then
                        txtBalance.BackColor = Color.Yellow
                    Else
                        txtBalance.BackColor = originalColor
                    End If
                    If count >= 6 Then
                        blinkTimer.Stop()
                        txtBalance.BackColor = originalColor
                        blinkTimer.Dispose()
                    End If
                End Sub
                blinkTimer.Start()
            End If
        End Sub

        Private Sub btnCalcBalance_Click(sender As Object, e As EventArgs) Handles btnCalcBalance.Click
            Try
                Dim fundID = If(cboFund.SelectedValue IsNot Nothing, CInt(cboFund.SelectedValue), 0)
                Dim bankID = If(cboBank.SelectedValue IsNot Nothing, CInt(cboBank.SelectedValue), 0)
                Dim startDate = dtpFrom.Value.Date

                Using conn = Db.OpenConn()
                    ' คำนวณแยกเพื่อความโปร่งใสตามคำขอผู้ใช้
                    Dim sqlBase = "FROM Transactions t WHERE DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) < " & Db.AccessDateLiteral(startDate)
                    Dim params As New List(Of Tuple(Of String, Object))()
                    
                    If fundID <> 0 Then
                        sqlBase &= " AND t.FundID = @f"
                        params.Add(New Tuple(Of String, Object)("@f", fundID))
                    End If
                    If bankID <> 0 Then
                        sqlBase &= " AND t.BankID = @b"
                        params.Add(New Tuple(Of String, Object)("@b", bankID))
                    End If

                    Dim sumInc = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) " & sqlBase & " AND t.TranType='Income'", params.ToArray()))
                    Dim sumExp = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) " & sqlBase & " AND t.TranType='Expense'", params.ToArray()))
                    Dim bal = sumInc - sumExp

                    Dim msg = $"📊 รายละเอียดการคำนวณยอดยกมา (ก่อนวันที่ {startDate:dd/MM/yyyy}):" & vbCrLf &
                              $"--------------------------------------------------" & vbCrLf &
                              $"รายรับสะสม: {sumInc:n2} บาท" & vbCrLf &
                              $"รายจ่ายสะสม: {sumExp:n2} บาท" & vbCrLf &
                              $"คงเหลือ (ยอดยกมา): {bal:n2} บาท" & vbCrLf & vbCrLf &
                              $"ต้องการนำค่านี้ไปใส่ในช่อง 'ยอดยกมา' หรือไม่?"

                    If MessageBox.Show(msg, "ยืนยันการคำนวณ", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                        txtBalance.Text = bal.ToString("n2")
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("คำนวณยอดยกมาไม่ได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

        Private Function GetManualBalance() As Decimal?
            Dim txt = txtBalance.Text.Trim()
            If String.IsNullOrEmpty(txt) Then Return Nothing

            Dim balance As Decimal
            If Decimal.TryParse(txt, balance) Then
                ' ถ้ากรอกเป็น 0.00 (ค่าเริ่มต้น) ให้ถือว่าเป็น Auto
                ' แต่ถ้าผู้ใช้จงใจแก้เป็นค่าอื่น หรือลบออกแล้วกรอก 0 ใหม่ ให้ใช้ค่านั้น
                ' เพื่อความง่าย: ถ้าไม่ว่างและแปลงเป็นตัวเลขได้ ให้ใช้ค่านั้นเลย
                Return balance
            End If
            Return Nothing
        End Function

        Private Sub btnPrintDetail_Click(sender As Object, e As EventArgs) Handles btnPrintDetail.Click
            Try
                ' 1. Read dates directly from UI controls at the exact moment of click
                ' Ensure toDate includes the full day up to 23:59:59
                Dim fDate As DateTime = dtpFrom.Value.Date
                Dim tDate As DateTime = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)

                ' 2. Normalize dates to Gregorian for database querying if needed
                Dim fromDate = Db.NormalizeGregorianDate(fDate)
                Dim toDate = Db.NormalizeGregorianDate(tDate)

                Dim fundID = If(cboFund.SelectedValue IsNot Nothing, CInt(cboFund.SelectedValue), 0)
                Dim bankID = If(cboBank.SelectedValue IsNot Nothing, CInt(cboBank.SelectedValue), 0)
                
                System.Diagnostics.Debug.WriteLine("[DEBUG FrmReports] btnPrintDetail_Click - dtpFrom: " & fDate & ", dtpTo: " & tDate)
                
                IncomeExpenseReport.ShowPreview(fromDate, toDate, Me, IncomeExpenseReport.ReportModes.Detailed, GetManualBalance(), If(fundID = 0, Nothing, fundID), If(bankID = 0, Nothing, bankID))
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnPrintSummary_Click(sender As Object, e As EventArgs) Handles btnPrintSummary.Click
            Try
                ' 1. Read dates directly from UI controls at the exact moment of click
                ' Ensure toDate includes the full day up to 23:59:59
                Dim fDate As DateTime = dtpFrom.Value.Date
                Dim tDate As DateTime = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)

                ' 2. Normalize dates to Gregorian for database querying if needed
                Dim fromDate = Db.NormalizeGregorianDate(fDate)
                Dim toDate = Db.NormalizeGregorianDate(tDate)

                Dim fundID = If(cboFund.SelectedValue IsNot Nothing, CInt(cboFund.SelectedValue), 0)
                Dim bankID = If(cboBank.SelectedValue IsNot Nothing, CInt(cboBank.SelectedValue), 0)
                
                System.Diagnostics.Debug.WriteLine("[DEBUG FrmReports] btnPrintSummary_Click - dtpFrom: " & fDate & ", dtpTo: " & tDate)
                
                IncomeExpenseReport.ShowPreview(fromDate, toDate, Me, IncomeExpenseReport.ReportModes.Summary, GetManualBalance(), If(fundID = 0, Nothing, fundID), If(bankID = 0, Nothing, bankID))
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub dgvReport_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvReport.CellContentClick


        End Sub
    End Class
End Namespace
