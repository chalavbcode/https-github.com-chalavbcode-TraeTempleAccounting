Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data
Imports System.Data.OleDb
Imports System.Globalization
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
                Me.KeyPreview = True
                HelpSystem.SetupHelp(Me, "FrmReports")
                Db.EnsureSchema()
                Using conn = Db.OpenConn()
                    Dim ft = Db.GetTable(conn, "SELECT 0 AS ID, '(ทุกกองทุน)' AS FundName FROM (SELECT COUNT(*) FROM Funds) C1 UNION ALL SELECT ID, FundName FROM Funds ORDER BY ID")
                    cboFund.DisplayMember = "FundName" : cboFund.ValueMember = "ID" : cboFund.DataSource = ft
                    Dim bt = Db.GetTable(conn, "SELECT 0 AS ID, '(ทุกบัญชี)' AS Disp FROM (SELECT COUNT(*) FROM BankAccounts) C1 UNION ALL SELECT ID, BankName & ' - ' & IIF(AccountNo IS NULL,'',AccountNo) AS Disp FROM BankAccounts ORDER BY ID")
                    cboBank.DisplayMember = "Disp" : cboBank.ValueMember = "ID" : cboBank.DataSource = bt
                End Using
                SetupFilterEnterNavigation()
                SetupToolTips()
                ' ปรับขนาดปุ่ม toolbar ให้เท่ากันทั้งแถว + ปรับฟอนต์ข้อความไทยให้เต็มปุ่ม
                UiFitter.UniformButtonGroup(btnPrintDetail, btnPrintSummary, btnSummaryIncome, btnSummaryExpense, btnShowChart, btnMonthly, btnLedger, btnPrint)
                UiFitter.AutoFitFormButtons(Me)
                btnLedger_Click(Nothing, EventArgs.Empty)
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub FrmReports_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmReports", Me)
            End If
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
            ttMain.SetToolTip(btnMonthly, "แสดงสรุปรายรับ-รายจ่าย แยกเป็นรายเดือน (ตารางตัวเลข)")
            ttMain.SetToolTip(btnShowChart, "แสดงกราฟแท่งสรุปรายรับ-รายจ่ายรายเดือน (รายรับ / รายจ่าย / คงเหลือสุทธิ)")
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
            ' Ensure we read the date only, and then add full day for toDate
            Dim fDate = dtpFrom.Value.Date
            Dim tDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)
            Return "FROM ((Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID) LEFT JOIN Funds f ON t.FundID=f.ID) LEFT JOIN BankAccounts b ON t.BankID=b.ID " &
                   BuildMonthlyFilter(fDate, tDate, params)
        End Function

        ''' <summary>
        ''' สร้าง WHERE clause สำหรับช่วงวันที่ที่ระบุ + filter กองทุน/ธนาคาร/ประเภท
        ''' (เหมือนเงื่อนไขของ BaseSql แต่ยอมรับช่วงวันที่เอง เพื่อใช้ต่อเดือนในกราฟ)
        ''' </summary>
        Private Function BuildMonthlyFilter(monthStart As Date, monthEnd As Date, ByRef params As List(Of Tuple(Of String, Object))) As String
            params = New List(Of Tuple(Of String, Object))()
            Dim fromDate = Db.NormalizeGregorianDate(monthStart)
            Dim toDate = Db.NormalizeGregorianDate(monthEnd)
            Dim sql = "WHERE DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) " &
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

        Private Sub ShowGridView()
            dgvReport.Visible = True
            chartMonthly.Visible = False
        End Sub

        Private Sub ShowChartView()
            dgvReport.Visible = False
            chartMonthly.Visible = True
        End Sub

        Private Sub btnLedger_Click(sender As Object, e As EventArgs) Handles btnLedger.Click, btnRefresh.Click
            Try
                ShowGridView()
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
            ShowGridView()
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
            ShowGridView()
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
                ' 1. แสดงตารางตัวเลข (มุมมองแบบตัวเลข) เสมอ
                ShowGridView()

                ' 2. Reuse ตารางสรุปรายเดือนกลาง (1 แถว/เดือน) — คอลัมน์ MonthName/TotalIncome/TotalExpense/NetBalance
                Dim dtMonthlySummary = GetMonthlySummary()
                If dtMonthlySummary.Rows.Count = 0 Then
                    MessageBox.Show("ไม่มีข้อมูลในช่วงวันที่ที่เลือก", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                dgvReport.DataSource = dtMonthlySummary

                ' 3. ฟอร์แมตคอลัมน์ตัวเลขให้อ่านง่าย
                For Each col As DataGridViewColumn In dgvReport.Columns
                    If col.Name = "TotalIncome" OrElse col.Name = "TotalExpense" OrElse col.Name = "NetBalance" Then
                        col.DefaultCellStyle.Format = "#,##0.00"
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    End If
                Next

                ' 4. สรุปยอดรวมช่วงเวลา — คำนวณจาก DataTable ที่ reuse อยู่แล้ว (ไม่ query ซ้ำ)
                Dim totI As Decimal = 0D
                Dim totE As Decimal = 0D
                For Each row As DataRow In dtMonthlySummary.Rows
                    totI += Convert.ToDecimal(row("TotalIncome"))
                    totE += Convert.ToDecimal(row("TotalExpense"))
                Next
                lblSummary.Text = $"สรุปรายเดือน: รายรับรวม {totI:n2} บาท  |  รายจ่ายรวม {totE:n2} บาท  |  คงเหลือ {totI - totE:n2} บาท"
            Catch ex As Exception
                MessageBox.Show("แสดงสรุปรายเดือนไม่ได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnShowChart_Click(sender As Object, e As EventArgs) Handles btnShowChart.Click
            Try
                ' ===== Step 5: Chart Properties — รีเซ็ต chart สะอาด =====
                ' Series ทั้ง 3 = SeriesChartType.Column (Clustered side-by-side)
                ' chartMonthly.DataSource = Nothing (ห้าม bind DataTable ตรง ๆ)
                ' IsValueShownAsLabel = True → 1 ตัวเลขสรุปต่อแท่ง (ตั้งใน CreateColumnSeries)
                ResetMonthlyChart()

                ' ===== Step 1: อ่านช่วงวันที่จากตัวกรอง =====
                Dim fromDate As DateTime = dtpFrom.Value.Date
                Dim toDate As DateTime = dtpTo.Value.Date

                ' ===== Step 2: สร้างรายการเดือนที่อยู่ในช่วง (loop ทีละเดือน) =====
                ' เช่น ม.ค. 2569 .. ก.ค. 2569 = 7 เดือน
                Dim months As New List(Of DateTime)()
                Dim cursor As DateTime = New DateTime(fromDate.Year, fromDate.Month, 1)
                Dim lastMonth As DateTime = New DateTime(toDate.Year, toDate.Month, 1)
                While cursor <= lastMonth
                    months.Add(cursor)
                    cursor = cursor.AddMonths(1)
                End While

                If months.Count = 0 Then
                    MessageBox.Show("ไม่มีช่วงเดือนให้แสดง", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' ===== Step 3+4: ต่อเดือน — คำนวณยอดรวมรายเดือน แล้ว plot 1 จุดต่อ Series =====
                Dim thaiCulture As New CultureInfo("th-TH")
                Dim anyPlotted As Boolean = False

                ' Running totals สำหรับแท่ง "รวม" ท้ายกราฟ (เคารพ filter อัตโนมัติจาก loop)
                Dim totalIncome As Decimal = 0D
                Dim totalExpense As Decimal = 0D

                ' แกน X ใช้ numeric index (กันป้ายซ้ำ/แท่งทับกัน) — ป้ายไทยตั้งผ่าน AxisLabel แยก
                Dim monthIndex As Double = 0

                Using conn = Db.OpenConn()
                    For Each m As DateTime In months
                        ' ขอบเขตวันที่ของเดือนนี้ (วันแรก 00:00:00 .. วันสุดท้าย 23:59:59)
                        Dim monthStart As DateTime = m
                        Dim monthEnd As DateTime = m.AddMonths(1).AddSeconds(-1)

                        ' SUM รายรับ/รายจ่าย เฉพาะเดือนนี้ (พร้อม filter กองทุน/ธนาคาร/ประเภท)
                        Dim ps As List(Of Tuple(Of String, Object)) = Nothing
                        Dim whereClause = BuildMonthlyFilter(monthStart, monthEnd, ps)
                        Dim sql = "SELECT SUM(IIF(t.TranType='Income', t.Amount, 0)) AS SumIncome, " &
                                  "SUM(IIF(t.TranType='Expense', t.Amount, 0)) AS SumExpense " &
                                  "FROM ((Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID) LEFT JOIN Funds f ON t.FundID=f.ID) LEFT JOIN BankAccounts b ON t.BankID=b.ID " &
                                  whereClause
                        Dim row As DataRow = Db.GetTable(conn, sql, ps.ToArray()).Rows(0)

                        ' ===== Step 3: รวมรายเดือน =====
                        Dim SumIncome As Decimal = Db.ToDecimalOrZero(row("SumIncome"))
                        Dim SumExpense As Decimal = Db.ToDecimalOrZero(row("SumExpense"))
                        Dim NetBalance As Decimal = SumIncome - SumExpense

                        ' สะสมยอดรวมสำหรับแท่ง "รวม"
                        totalIncome += SumIncome
                        totalExpense += SumExpense

                        ' ===== Step 4: plot 1 จุดต่อ Series ต่อเดือน =====
                        ' X = numeric index เพื่อแยกแท่งทุกเดือนเสมอ (เป็นไปไม่ได้ที่เดือนชนกัน)
                        ' ป้ายไทย ("ม.ค. ๒๕๖๙") ตั้งผ่าน AxisLabel
                        Dim monthLabel As String = MonthLabelThai(m.Year, m.Month, thaiCulture)

                        ' Log ตรวจสอบก่อน plot — ดูผลใน Immediate Window
                        System.Diagnostics.Debug.WriteLine(
                            String.Format("MONTH={0:yyyy-MM} | Label={1} | Income={2:N2} | Expense={3:N2} | Net={4:N2}",
                                          m, monthLabel, SumIncome, SumExpense, NetBalance))

                        ' แท่งเขียว = รายรับ
                        Dim idxIncome = chartMonthly.Series("รายรับ").Points.AddXY(monthIndex, SumIncome)
                        chartMonthly.Series("รายรับ").Points(idxIncome).AxisLabel = monthLabel

                        ' แท่งแดง = รายจ่าย
                        Dim idxExpense = chartMonthly.Series("รายจ่าย").Points.AddXY(monthIndex, SumExpense)
                        chartMonthly.Series("รายจ่าย").Points(idxExpense).AxisLabel = monthLabel

                        ' แท่งน้ำเงิน = คงเหลือสุทธิ
                        Dim idxNet = chartMonthly.Series("เงินคงเหลือสุทธิ").Points.AddXY(monthIndex, NetBalance)
                        chartMonthly.Series("เงินคงเหลือสุทธิ").Points(idxNet).AxisLabel = monthLabel

                        monthIndex += 1
                        anyPlotted = True
                    Next
                End Using

                If Not anyPlotted Then
                    MessageBox.Show("ไม่มีข้อมูลในช่วงวันที่ที่เลือก", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' ===== Step 5: เพิ่มแท่งกลุ่ม "รวม" ต่อจากเดือนสุดท้าย =====
                ' ค่า = ผลรวมทั้งช่วง (สะสมใน loop ตาม filter ที่เลือกอยู่แล้ว ไม่ query ซ้ำ)
                Dim totalNet As Decimal = totalIncome - totalExpense

                System.Diagnostics.Debug.WriteLine(
                    String.Format("TOTAL | Label={0} | Income={1:N2} | Expense={2:N2} | Net={3:N2}",
                                  "รวม", totalIncome, totalExpense, totalNet))

                ' แท่งเขียวเข้ม = รวมรายรับ
                Dim idxTotalIncome = chartMonthly.Series("รายรับ").Points.AddXY(monthIndex, totalIncome)
                chartMonthly.Series("รายรับ").Points(idxTotalIncome).AxisLabel = "รวม"
                chartMonthly.Series("รายรับ").Points(idxTotalIncome).Color = Color.ForestGreen

                ' แท่งแดงเข้ม = รวมรายจ่าย
                Dim idxTotalExpense = chartMonthly.Series("รายจ่าย").Points.AddXY(monthIndex, totalExpense)
                chartMonthly.Series("รายจ่าย").Points(idxTotalExpense).AxisLabel = "รวม"
                chartMonthly.Series("รายจ่าย").Points(idxTotalExpense).Color = Color.Firebrick

                ' แท่งน้ำเงินเข้ม = รวมคงเหลือสุทธิ
                Dim idxTotalNet = chartMonthly.Series("เงินคงเหลือสุทธิ").Points.AddXY(monthIndex, totalNet)
                chartMonthly.Series("เงินคงเหลือสุทธิ").Points(idxTotalNet).AxisLabel = "รวม"
                chartMonthly.Series("เงินคงเหลือสุทธิ").Points(idxTotalNet).Color = Color.RoyalBlue

                ' Bold ป้าย "รวม" ให้เห็นชัดว่าเป็นสรุปยอด
                chartMonthly.Series("รายรับ").Points(idxTotalIncome).Font =
                    New Font("Tahoma", 8.5F, FontStyle.Bold)
                chartMonthly.Series("รายจ่าย").Points(idxTotalExpense).Font =
                    New Font("Tahoma", 8.5F, FontStyle.Bold)
                chartMonthly.Series("เงินคงเหลือสุทธิ").Points(idxTotalNet).Font =
                    New Font("Tahoma", 8.5F, FontStyle.Bold)

                ' Render ใหม่ แล้วสลับไปมุมมองกราฟ
                chartMonthly.Refresh()
                ShowChartView()
                lblSummary.Text = "รายงานกราฟสรุปรายรับ-รายจ่ายรายเดือน (ช่วงเวลาที่เลือก)"
            Catch ex As Exception
                MessageBox.Show("แสดงกราฟรายเดือนไม่ได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' สร้างตารางสรุปรายเดือนรวม (1 แถวต่อเดือน) จาก SQL GROUP BY FORMAT 'yyyy-mm'
        ''' คอลัมน์: MonthName (ป้ายไทย), TotalIncome, TotalExpense, NetBalance
        ''' เป็นแหล่งข้อมูลกลางเดียวที่ทั้งตาราง dgvReport และกราฟ chartMonthly ใช้ร่วมกัน
        ''' </summary>
        Private Function GetMonthlySummary() As DataTable
            Dim ps As List(Of Tuple(Of String, Object)) = Nothing
            Dim fromWhere = BaseSql(ps)
            Dim monthKey = "FORMAT(t.TranDate, 'yyyy-mm')"
            Dim sql = "SELECT " & monthKey & " AS [MonthKey], " &
                      "SUM(IIF(t.TranType='Income', t.Amount, 0)) AS [TotalIncome], " &
                      "SUM(IIF(t.TranType='Expense', t.Amount, 0)) AS [TotalExpense] " &
                      fromWhere &
                      " GROUP BY " & monthKey &
                      " ORDER BY " & monthKey

            Using conn = Db.OpenConn()
                Dim raw = Db.GetTable(conn, sql, ps.ToArray())

                ' สร้าง DataTable ที่มีคอลัมน์ตามที่ตาราง/กราฟต้องการ
                Dim result As New DataTable()
                result.Columns.Add("MonthName", GetType(String))
                result.Columns.Add("TotalIncome", GetType(Decimal))
                result.Columns.Add("TotalExpense", GetType(Decimal))
                result.Columns.Add("NetBalance", GetType(Decimal))

                Dim thaiCulture As New CultureInfo("th-TH")
                For Each r As DataRow In raw.Rows
                    Dim income As Decimal = Db.ToDecimalOrZero(r("TotalIncome"))
                    Dim expense As Decimal = Db.ToDecimalOrZero(r("TotalExpense"))
                    Dim net As Decimal = income - expense
                    result.Rows.Add(FormatThaiMonthKey(Convert.ToString(r("MonthKey")), thaiCulture), income, expense, net)
                Next
                Return result
            End Using
        End Function

        ''' <summary>
        ''' แปลงคีย์ 'yyyy-mm' (ปีอาจเป็น พ.ศ. หรือ ค.ศ.) เป็นป้ายไทย เช่น ก.ค. ๒๕๖๙
        ''' </summary>
        Private Function FormatThaiMonthKey(key As String, thaiCulture As CultureInfo) As String
            If String.IsNullOrWhiteSpace(key) Then Return key
            Try
                Dim parts = key.Split("-"c)
                Dim year = Convert.ToInt32(parts(0))
                Dim month = Convert.ToInt32(parts(1))
                ' หากปีเกิน 2400 แปลว่าเป็นปี พ.ศ. ให้แปลงกลับเป็น ค.ศ. ก่อน (th-TH จะบวก 543 ให้อัตโนมัติ)
                If year > 2400 Then year -= 543
                Return MonthLabelThai(year, month, thaiCulture)
            Catch
                Return key
            End Try
        End Function

        ''' <summary>
        ''' สร้างป้ายกำกับเดือนแบบไทย เช่น ก.ค. ๒๕๖๙ (ปี พ.ศ.)
        ''' </summary>
        Private Function MonthLabelThai(gregorianYear As Integer, month As Integer, thaiCulture As CultureInfo) As String
            Try
                Dim d As New DateTime(gregorianYear, month, 1)
                ' th-TH culture แปลงปี ค.ศ. เป็น พ.ศ. อัตโนมัติ (2569 = 2026+543)
                Dim label = d.ToString("MMM yyyy", thaiCulture)
                ' แปลงตัวเลขอารบิกเป็นเลขไทย
                Return ConvertToThaiDigits(label)
            Catch
                Return String.Format("{0:D2}-{1}", month, gregorianYear + 543)
            End Try
        End Function

        ''' <summary>
        ''' แปลงตัวเลข 0-9 ในสตริงเป็นเลขไทย ๐-๙
        ''' </summary>
        Private Function ConvertToThaiDigits(text As String) As String
            If String.IsNullOrEmpty(text) Then Return text
            Dim thaiDigits As String = "๐๑๒๓๔๕๖๗๘๙"
            Dim sb As New System.Text.StringBuilder(text.Length)
            For Each c As Char In text
                If c >= "0"c AndAlso c <= "9"c Then
                    sb.Append(thaiDigits(Asc(c) - Asc("0"c)))
                Else
                    sb.Append(c)
                End If
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' รีเซ็ต ChartArea และ Series ทั้งหมดใหม่ทุกครั้งที่วาดกราฟ (destroy → recreate)
        ''' ป้องกัน property/Series/DataSource ตกค้างจาก runtime ก่อนหน้า
        ''' </summary>
        Private Sub ResetMonthlyChart()
            ' 1. ล้าง data binding เก่าทั้งหมด (กัน DataSource ตกค้าง)
            chartMonthly.DataSource = Nothing
            chartMonthly.Titles.Clear()
            chartMonthly.Annotations.Clear()
            ' 2. ลบ Series / ChartArea / Legends เก่าทั้งหมด
            chartMonthly.Series.Clear()
            chartMonthly.ChartAreas.Clear()
            chartMonthly.Legends.Clear()
            ' 3. ปิด default palette เพื่อให้สี Series ควบคุมเองได้
            chartMonthly.Palette = ChartColorPalette.None

            ' 4. สร้าง ChartArea ใหม่ — แกน X เป็น numeric index (0,1,2,..) ต่อเดือน ป้ายไทยตั้งผ่าน AxisLabel
            Dim area As New ChartArea("MonthlyArea") With {
                .BackColor = Color.White
            }
            ' Interval=1 → 1 จุดข้อมูลต่อ index ต่อเดือน (บังคับให้ทุกเดือนแสดง label ครบ)
            area.AxisX.Interval = 1
            area.AxisX.LabelStyle.Font = New Font("Tahoma", 8.5F)
            area.AxisX.LabelStyle.Angle = -45
            area.AxisX.MajorGrid.Enabled = False
            area.AxisX.Title = "เดือน / ปี (พ.ศ.)"

            area.AxisY.IsStartedFromZero = False
            area.AxisY.LabelStyle.Format = "฿#,##0"
            area.AxisY.LabelStyle.Font = New Font("Tahoma", 9F)
            area.AxisY.Title = "จำนวนเงิน (บาท)"
            area.AxisY.MajorGrid.Enabled = True
            area.AxisY.MajorGrid.LineColor = Color.LightGray

            ' === Zoom/Scroll แกน Y (แนวตั้ง): เปิด ScrollBar + mouse wheel zoom ===
            area.AxisY.ScaleView.Zoomable = True
            area.AxisY.ScrollBar.Enabled = True
            area.AxisY.ScrollBar.BackColor = Color.Gainsboro
            area.CursorY.IsUserSelectionEnabled = True
            area.CursorY.IsUserEnabled = True

            chartMonthly.ChartAreas.Add(area)

            ' 5. สร้าง 3 Clustered Column Series สี เขียว-แดง-น้ำเงิน
            CreateColumnSeries("รายรับ", Color.MediumSeaGreen)
            CreateColumnSeries("รายจ่าย", Color.IndianRed)
            CreateColumnSeries("เงินคงเหลือสุทธิ", Color.SteelBlue)

            ' 6. Legend ด้านบน
            Dim legend As New Legend("Legend1") With {
                .Docking = Docking.Top
            }
            chartMonthly.Legends.Add(legend)
        End Sub

        ''' <summary>
        ''' สร้าง Series แบบ Clustered Column (Side-by-Side) — baseline = Y=0 เสมอ
        ''' PointWidth = 0.7 ป้องกันแท่งกว้างเกินจนซ้อนทับกัน
        ''' </summary>
        Private Sub CreateColumnSeries(name As String, color As Color)
            Dim s As New Series(name) With {
                .ChartType = SeriesChartType.Column,
                .ChartArea = "MonthlyArea",
                .Color = color,
                .BorderWidth = 1,
                .IsValueShownAsLabel = True,
                .LabelFormat = "฿#,##0",
                .Font = New Font("Tahoma", 8.5F)
            }
            ' ควบคุมความกว้างแท่ง — ป้องกัน overlap / Waterfall look
            s("PointWidth") = "0.7"
            ' SmartLabel จัดตำแหน่งป้ายอัตโนมัติ — 1 ป้ายต่อแท่ง ห้ามซ้อน
            s.SmartLabelStyle.Enabled = True
            s.SmartLabelStyle.CalloutLineColor = color
            chartMonthly.Series.Add(s)
        End Sub

        ''' <summary>
        ''' เลื่อนล้อเมาส์บนกราฟ = ซูมเข้า/ออก แกน Y (แนวตั้ง)
        ''' ล้อขึ้น (Delta>0) = ซูมเข้า, ล้อลง = ซูมออก — ค่าถูก clamp ในช่วงข้อมูลจริง
        ''' </summary>
        Private Sub chartMonthly_MouseWheel(sender As Object, e As MouseEventArgs) Handles chartMonthly.MouseWheel
            Try
                If chartMonthly.ChartAreas.Count = 0 Then Return
                Dim axisY = chartMonthly.ChartAreas(0).AxisY

                ' ช่วงข้อมูลจริง = ค่า Y ทั้งหมดใน series (min..max พร้อม margin)
                Dim fullMin As Double = Double.MaxValue
                Dim fullMax As Double = Double.MinValue
                For Each s As Series In chartMonthly.Series
                    For Each p As DataPoint In s.Points
                        If p.YValues IsNot Nothing AndAlso p.YValues.Length > 0 Then
                            Dim v = p.YValues(0)
                            If v < fullMin Then fullMin = v
                            If v > fullMax Then fullMax = v
                        End If
                    Next
                Next
                If fullMin >= fullMax Then Return
                Dim margin = (fullMax - fullMin) * 0.05
                fullMin -= margin
                fullMax += margin
                Dim fullRange As Double = fullMax - fullMin
                If fullRange <= 0 Then Return

                ' ช่วงที่กำลังแสดงอยู่ (ก่อน zoom ค่า View เป็น NaN → ใช้ full range)
                Dim viewMin As Double = axisY.ScaleView.ViewMinimum
                Dim viewMax As Double = axisY.ScaleView.ViewMaximum
                If Double.IsNaN(viewMin) OrElse Double.IsNaN(viewMax) Then
                    viewMin = fullMin
                    viewMax = fullMax
                End If

                Dim zoomFactor As Double = If(e.Delta > 0, 0.9, 1.1)
                Dim range = viewMax - viewMin
                Dim newRange = range * zoomFactor

                ' ซูมออกเกินช่วงจริง → reset กลับเต็ม view
                If newRange >= fullRange Then
                    axisY.ScaleView.ZoomReset()
                Else
                    ' ซูมโดยคงกึ่งกลางปัจจุบันไว้ แล้ว clamp ไม่เกินช่วงข้อมูล
                    Dim center = viewMin + range / 2
                    Dim newMin = center - newRange / 2
                    Dim newMax = center + newRange / 2
                    If newMin < fullMin Then newMin = fullMin
                    If newMax > fullMax Then newMax = fullMax
                    If newMax - newMin < 0.001 Then Return
                    axisY.ScaleView.Zoom(newMin, newMax)
                End If
                chartMonthly.Invalidate()
            Catch
                ' ไม่ควรขัดจังหวะการใช้งาน — ละเว้น error ที่ไม่คาดคิด
            End Try
        End Sub

        ''' <summary>
        ''' เมื่อเมาส์เข้าสู่กราฟ ให้ focus ไปที่ chart เพื่อให้ MouseWheel ทำงานได้จริง
        ''' (WinForms ต้องให้ control ที่รับ wheel มี focus)
        ''' </summary>
        Private Sub chartMonthly_MouseEnter(sender As Object, e As EventArgs) Handles chartMonthly.MouseEnter
            chartMonthly.Focus()
        End Sub

        ''' <summary>
        ''' ดับเบิลคลิกบนกราฟ = reset zoom กลับเป็นมุมมองเต็มช่วง
        ''' </summary>
        Private Sub chartMonthly_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles chartMonthly.MouseDoubleClick
            Try
                If chartMonthly.ChartAreas.Count > 0 Then
                    chartMonthly.ChartAreas(0).AxisY.ScaleView.ZoomReset()
                    chartMonthly.ChartAreas(0).AxisX.ScaleView.ZoomReset()
                    chartMonthly.Invalidate()
                End If
            Catch
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
                ' 1. Read dates explicitly from UI controls
                Dim startDate As DateTime = dtpFrom.Value.Date
                Dim endDate As DateTime = dtpTo.Value.Date.AddDays(1).AddSeconds(-1) ' Include entire end day

                Dim fundID = If(cboFund.SelectedValue IsNot Nothing, CInt(cboFund.SelectedValue), 0)
                Dim bankID = If(cboBank.SelectedValue IsNot Nothing, CInt(cboBank.SelectedValue), 0)
                
                System.Diagnostics.Debug.WriteLine("[DEBUG FrmReports] btnPrintDetail_Click - startDate: " & startDate & ", endDate: " & endDate)
                
                ' 2. Pass explicit dates to the report
                IncomeExpenseReport.ShowPreview(startDate, endDate, Me, IncomeExpenseReport.ReportModes.Detailed, GetManualBalance(), If(fundID = 0, Nothing, fundID), If(bankID = 0, Nothing, bankID))
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnPrintSummary_Click(sender As Object, e As EventArgs) Handles btnPrintSummary.Click
            Try
                ' 1. Read dates explicitly from UI controls
                Dim startDate As DateTime = dtpFrom.Value.Date
                Dim endDate As DateTime = dtpTo.Value.Date.AddDays(1).AddSeconds(-1) ' Include entire end day

                Dim fundID = If(cboFund.SelectedValue IsNot Nothing, CInt(cboFund.SelectedValue), 0)
                Dim bankID = If(cboBank.SelectedValue IsNot Nothing, CInt(cboBank.SelectedValue), 0)
                
                System.Diagnostics.Debug.WriteLine("[DEBUG FrmReports] btnPrintSummary_Click - startDate: " & startDate & ", endDate: " & endDate)
                
                ' 2. Pass explicit dates to the report
                IncomeExpenseReport.ShowPreview(startDate, endDate, Me, IncomeExpenseReport.ReportModes.Summary, GetManualBalance(), If(fundID = 0, Nothing, fundID), If(bankID = 0, Nothing, bankID))
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub dgvReport_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvReport.CellContentClick


        End Sub
    End Class
End Namespace
