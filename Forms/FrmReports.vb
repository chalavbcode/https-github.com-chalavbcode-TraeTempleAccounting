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
                ' === Step 1: Reuse ตารางสรุปรายเดือนกลางเดียวกันกับตาราง dgvReport
                '     (1 แถว/เดือน: MonthName, TotalIncome, TotalExpense, NetBalance)
                '     ห้าม query raw transactions หรือ loop ข้อมูลดิบเด็ดขาด
                Dim dtMonthlySummary = GetMonthlySummary()
                If dtMonthlySummary.Rows.Count = 0 Then
                    MessageBox.Show("ไม่มีข้อมูลในช่วงวันที่ที่เลือก", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' === Step 2: รีเซ็ต Chart ให้สะอาด — สร้าง ChartArea + 3 Clustered Column Series + Legend ใหม่
                ResetMonthlyChart()

                ' === Step 3: Clear points เดิมทั้งหมดก่อนเติมข้อมูลใหม่
                chartMonthly.Series("รายรับ").Points.Clear()
                chartMonthly.Series("รายจ่าย").Points.Clear()
                chartMonthly.Series("เงินคงเหลือสุทธิ").Points.Clear()

                ' === Step 4: Loop เฉพาะแถวที่รวมกลุ่มแล้ว (1 จุดต่อ Series ต่อเดือน)
                For Each row As DataRow In dtMonthlySummary.Rows
                    Dim monthLabel As String = Convert.ToString(row("MonthName"))   ' เช่น "ม.ค. ๒๕๖๙"
                    Dim income As Decimal = Convert.ToDecimal(row("TotalIncome"))
                    Dim expense As Decimal = Convert.ToDecimal(row("TotalExpense"))
                    Dim balance As Decimal = Convert.ToDecimal(row("NetBalance"))

                    ' เพิ่ม 1 จุดต่อ Series ต่อเดือน: เขียว=รายรับ, แดง=รายจ่าย, น้ำเงิน=คงเหลือ
                    chartMonthly.Series("รายรับ").Points.AddXY(monthLabel, income)
                    chartMonthly.Series("รายจ่าย").Points.AddXY(monthLabel, expense)
                    chartMonthly.Series("เงินคงเหลือสุทธิ").Points.AddXY(monthLabel, balance)
                Next

                ' === Step 5: บังคับ render ใหม่ แล้วสลับไปมุมมองกราฟ ===
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

            ' 4. สร้าง ChartArea ใหม่ — แกน Y เริ่มที่ 0, แกน X Categorical (ป้ายสตริง)
            Dim area As New ChartArea("MonthlyArea") With {
                .BackColor = Color.White
            }
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
