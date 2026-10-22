Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmReports
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents cboType As ComboBox
        Friend WithEvents cboFund As ComboBox
        Friend WithEvents cboBank As ComboBox
        Friend WithEvents lbl1 As Label
        Friend WithEvents lbl2 As Label
        Friend WithEvents lbl3 As Label
        Friend WithEvents lbl4 As Label
        Friend WithEvents lbl5 As Label
        Friend WithEvents lblBalance As Label
        Friend WithEvents txtBalance As TextBox
        Friend WithEvents btnCalcBalance As Button
        Friend WithEvents btnSummaryIncome As Button
        Friend WithEvents btnSummaryExpense As Button
        Friend WithEvents btnMonthly As Button
        Friend WithEvents btnShowChart As Button
        Friend WithEvents btnLedger As Button
        Friend WithEvents btnPrint As Button
        Friend WithEvents btnRefresh As Button
        Friend WithEvents btnPrintDetail As Button
        Friend WithEvents btnPrintSummary As Button
        Friend WithEvents btnSavePdf As Button
        Friend WithEvents btnPrintReport As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents dgvReport As DataGridView
        Friend WithEvents lblSummary As Label
        Friend WithEvents p As Panel
        Friend WithEvents pa As FlowLayoutPanel
        Friend WithEvents ttMain As ToolTip
        Friend WithEvents chartMonthly As Chart

        <DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <DebuggerStepThrough()>
        Private Sub InitializeComponent()
            components = New Container()
            Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
            ttMain = New ToolTip(components)
            p = New Panel()
            lbl1 = New Label()
            dtpFrom = New DateTimePicker()
            lbl2 = New Label()
            dtpTo = New DateTimePicker()
            lbl3 = New Label()
            cboType = New ComboBox()
            lbl4 = New Label()
            cboFund = New ComboBox()
            lbl5 = New Label()
            cboBank = New ComboBox()
            lblBalance = New Label()
            txtBalance = New TextBox()
            btnCalcBalance = New Button()
            btnRefresh = New Button()
            pa = New FlowLayoutPanel()
            btnPrintDetail = New Button()
            btnPrintSummary = New Button()
            btnSummaryIncome = New Button()
            btnSummaryExpense = New Button()
            btnShowChart = New Button()
            btnMonthly = New Button()
            btnLedger = New Button()
            btnSavePdf = New Button()
            btnPrintReport = New Button()
            btnPrint = New Button()
            btnClose = New Button()
            lblSummary = New Label()
            dgvReport = New DataGridView()
            chartMonthly = New Chart()
            p.SuspendLayout()
            pa.SuspendLayout()
            CType(dgvReport, ISupportInitialize).BeginInit()
            CType(chartMonthly, ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' p
            ' 
            p.BackColor = Color.White
            p.Controls.Add(lbl1)
            p.Controls.Add(dtpFrom)
            p.Controls.Add(lbl2)
            p.Controls.Add(dtpTo)
            p.Controls.Add(lbl3)
            p.Controls.Add(cboType)
            p.Controls.Add(lbl4)
            p.Controls.Add(cboFund)
            p.Controls.Add(lbl5)
            p.Controls.Add(cboBank)
            p.Controls.Add(lblBalance)
            p.Controls.Add(txtBalance)
            p.Controls.Add(btnCalcBalance)
            p.Controls.Add(btnRefresh)
            p.Controls.Add(btnClose)
            p.Dock = DockStyle.Top
            p.Location = New Point(0, 0)
            p.Name = "p"
            p.Padding = New Padding(12, 8, 12, 8)
            p.Size = New Size(1478, 90)
            p.TabIndex = 3
            ' 
            ' lbl1
            ' 
            lbl1.AutoSize = True
            lbl1.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lbl1.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl1.Location = New Point(15, 5)
            lbl1.Name = "lbl1"
            lbl1.Size = New Size(85, 22)
            lbl1.TabIndex = 0
            lbl1.Text = "จากวันที่:"
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 9.0F)
            dtpFrom.Location = New Point(138, 3)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(140, 29)
            dtpFrom.TabIndex = 1
            dtpFrom.Value = New Date(Date.Today.Year, Date.Today.Month, 1, 0, 0, 0, 0)
            ' 
            ' lbl2
            ' 
            lbl2.AutoSize = True
            lbl2.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lbl2.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl2.Location = New Point(349, 5)
            lbl2.Name = "lbl2"
            lbl2.Size = New Size(74, 22)
            lbl2.TabIndex = 2
            lbl2.Text = "ถึงวันที่:"
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 9.0F)
            dtpTo.Location = New Point(444, 3)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(140, 29)
            dtpTo.TabIndex = 3
            dtpTo.Value = New Date(Date.Today.Year, Date.Today.Month, DateTime.DaysInMonth(Date.Today.Year, Date.Today.Month), 0, 0, 0, 0)
            ' 
            ' lbl3
            ' 
            lbl3.AutoSize = True
            lbl3.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lbl3.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl3.Location = New Point(649, 5)
            lbl3.Name = "lbl3"
            lbl3.Size = New Size(79, 22)
            lbl3.TabIndex = 4
            lbl3.Text = "ประเภท:"
            ' 
            ' cboType
            ' 
            cboType.DropDownStyle = ComboBoxStyle.DropDownList
            cboType.Font = New Font("Tahoma", 9.0F)
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(775, 3)
            cboType.Name = "cboType"
            cboType.Size = New Size(120, 30)
            cboType.TabIndex = 5
            ' 
            ' lbl4
            ' 
            lbl4.AutoSize = True
            lbl4.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lbl4.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl4.Location = New Point(12, 48)
            lbl4.Name = "lbl4"
            lbl4.Size = New Size(75, 22)
            lbl4.TabIndex = 6
            lbl4.Text = "กองทุน:"
            ' 
            ' cboFund
            ' 
            cboFund.DropDownStyle = ComboBoxStyle.DropDownList
            cboFund.Font = New Font("Tahoma", 9.0F)
            cboFund.Location = New Point(93, 45)
            cboFund.Name = "cboFund"
            cboFund.Size = New Size(160, 30)
            cboFund.TabIndex = 7
            ' 
            ' lbl5
            ' 
            lbl5.AutoSize = True
            lbl5.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lbl5.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl5.Location = New Point(281, 48)
            lbl5.Name = "lbl5"
            lbl5.Size = New Size(81, 22)
            lbl5.TabIndex = 8
            lbl5.Text = "ธนาคาร:"
            ' 
            ' cboBank
            ' 
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboBank.Font = New Font("Tahoma", 9.0F)
            cboBank.Location = New Point(368, 45)
            cboBank.Name = "cboBank"
            cboBank.Size = New Size(160, 30)
            cboBank.TabIndex = 9
            ' 
            ' lblBalance
            ' 
            lblBalance.AutoSize = True
            lblBalance.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            lblBalance.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblBalance.Location = New Point(544, 47)
            lblBalance.Name = "lblBalance"
            lblBalance.Size = New Size(98, 22)
            lblBalance.TabIndex = 10
            lblBalance.Text = "ยอดยกมา:"
            ' 
            ' txtBalance
            ' 
            txtBalance.Font = New Font("Tahoma", 9.0F)
            txtBalance.Location = New Point(648, 45)
            txtBalance.Name = "txtBalance"
            txtBalance.Size = New Size(80, 29)
            txtBalance.TabIndex = 11
            txtBalance.Text = "0.00"
            txtBalance.TextAlign = HorizontalAlignment.Right
            ' 
            ' btnCalcBalance
            ' 
            btnCalcBalance.BackColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
            btnCalcBalance.FlatStyle = FlatStyle.Flat
            btnCalcBalance.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnCalcBalance.ForeColor = Color.White
            btnCalcBalance.Location = New Point(746, 45)
            btnCalcBalance.Name = "btnCalcBalance"
            btnCalcBalance.Size = New Size(79, 30)
            btnCalcBalance.TabIndex = 12
            btnCalcBalance.Text = ChrW(55358) & ChrW(56814)
            btnCalcBalance.UseVisualStyleBackColor = False
            ' 
            ' btnRefresh
            ' 
            btnRefresh.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnRefresh.FlatStyle = FlatStyle.Flat
            btnRefresh.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(831, 45)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(100, 30)
            btnRefresh.TabIndex = 13
            btnRefresh.Text = "🔍 ดูรายงาน"
            btnRefresh.UseVisualStyleBackColor = False
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 9.75F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(950, 45)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(100, 30)
            btnClose.TabIndex = 14
            btnClose.Text = "❌ ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' pa
            ' 
            pa.AutoSize = True
            pa.AutoSizeMode = AutoSizeMode.GrowAndShrink
            pa.BackColor = Color.FromArgb(CByte(245), CByte(240), CByte(220))
            pa.Controls.Add(btnPrintDetail)
            pa.Controls.Add(btnPrintSummary)
            pa.Controls.Add(btnSummaryIncome)
            pa.Controls.Add(btnSummaryExpense)
            pa.Controls.Add(btnShowChart)
            pa.Controls.Add(btnMonthly)
            pa.Controls.Add(btnLedger)
            pa.Controls.Add(btnSavePdf)
            pa.Controls.Add(btnPrintReport)
            pa.Controls.Add(btnPrint)
            pa.Dock = DockStyle.Top
            pa.Location = New Point(0, 90)
            pa.Name = "pa"
            pa.Padding = New Padding(8, 6, 8, 6)
            pa.Size = New Size(1478, 100)
            pa.TabIndex = 2
            ' 
            ' btnPrintDetail
            ' 
            btnPrintDetail.AutoSize = True
            btnPrintDetail.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnPrintDetail.BackColor = Color.FromArgb(CByte(185), CByte(28), CByte(28))
            btnPrintDetail.Cursor = Cursors.Hand
            btnPrintDetail.FlatAppearance.BorderSize = 0
            btnPrintDetail.FlatStyle = FlatStyle.Flat
            btnPrintDetail.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnPrintDetail.ForeColor = Color.White
            btnPrintDetail.Location = New Point(11, 8)
            btnPrintDetail.Margin = New Padding(3, 2, 3, 2)
            btnPrintDetail.Name = "btnPrintDetail"
            btnPrintDetail.Padding = New Padding(12, 4, 12, 4)
            btnPrintDetail.Size = New Size(197, 40)
            btnPrintDetail.TabIndex = 6
            btnPrintDetail.Text = "📜 รายงานละเอียด"
            btnPrintDetail.UseVisualStyleBackColor = False
            ' 
            ' btnPrintSummary
            ' 
            btnPrintSummary.AutoSize = True
            btnPrintSummary.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnPrintSummary.BackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnPrintSummary.Cursor = Cursors.Hand
            btnPrintSummary.FlatAppearance.BorderSize = 0
            btnPrintSummary.FlatStyle = FlatStyle.Flat
            btnPrintSummary.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnPrintSummary.ForeColor = Color.White
            btnPrintSummary.Location = New Point(214, 8)
            btnPrintSummary.Margin = New Padding(3, 2, 3, 2)
            btnPrintSummary.Name = "btnPrintSummary"
            btnPrintSummary.Padding = New Padding(12, 4, 12, 4)
            btnPrintSummary.Size = New Size(156, 40)
            btnPrintSummary.TabIndex = 5
            btnPrintSummary.Text = ChrW(55358) & ChrW(56830) & " รายงานย่อ"
            btnPrintSummary.UseVisualStyleBackColor = False
            ' 
            ' btnSummaryIncome
            ' 
            btnSummaryIncome.AutoSize = True
            btnSummaryIncome.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnSummaryIncome.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSummaryIncome.Cursor = Cursors.Hand
            btnSummaryIncome.FlatAppearance.BorderSize = 0
            btnSummaryIncome.FlatStyle = FlatStyle.Flat
            btnSummaryIncome.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnSummaryIncome.ForeColor = Color.White
            btnSummaryIncome.Location = New Point(376, 8)
            btnSummaryIncome.Margin = New Padding(3, 2, 3, 2)
            btnSummaryIncome.Name = "btnSummaryIncome"
            btnSummaryIncome.Padding = New Padding(12, 4, 12, 4)
            btnSummaryIncome.Size = New Size(157, 40)
            btnSummaryIncome.TabIndex = 4
            btnSummaryIncome.Text = "💵 สรุปรายรับ"
            btnSummaryIncome.UseVisualStyleBackColor = False
            ' 
            ' btnSummaryExpense
            ' 
            btnSummaryExpense.AutoSize = True
            btnSummaryExpense.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnSummaryExpense.BackColor = Color.FromArgb(CByte(190), CByte(18), CByte(60))
            btnSummaryExpense.Cursor = Cursors.Hand
            btnSummaryExpense.FlatAppearance.BorderSize = 0
            btnSummaryExpense.FlatStyle = FlatStyle.Flat
            btnSummaryExpense.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnSummaryExpense.ForeColor = Color.White
            btnSummaryExpense.Location = New Point(539, 8)
            btnSummaryExpense.Margin = New Padding(3, 2, 3, 2)
            btnSummaryExpense.Name = "btnSummaryExpense"
            btnSummaryExpense.Padding = New Padding(12, 4, 12, 4)
            btnSummaryExpense.Size = New Size(167, 40)
            btnSummaryExpense.TabIndex = 3
            btnSummaryExpense.Text = "💸 สรุปรายจ่าย"
            btnSummaryExpense.UseVisualStyleBackColor = False
            ' 
            ' btnShowChart
            ' 
            btnShowChart.AutoSize = True
            btnShowChart.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnShowChart.BackColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            btnShowChart.Cursor = Cursors.Hand
            btnShowChart.FlatAppearance.BorderSize = 0
            btnShowChart.FlatStyle = FlatStyle.Flat
            btnShowChart.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnShowChart.ForeColor = Color.White
            btnShowChart.Location = New Point(712, 8)
            btnShowChart.Margin = New Padding(3, 2, 3, 2)
            btnShowChart.Name = "btnShowChart"
            btnShowChart.Padding = New Padding(12, 4, 12, 4)
            btnShowChart.Size = New Size(221, 40)
            btnShowChart.TabIndex = 7
            btnShowChart.Text = "📊 กราฟสรุปรายเดือน"
            btnShowChart.UseVisualStyleBackColor = False
            ' 
            ' btnMonthly
            ' 
            btnMonthly.AutoSize = True
            btnMonthly.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnMonthly.BackColor = Color.FromArgb(CByte(126), CByte(34), CByte(206))
            btnMonthly.Cursor = Cursors.Hand
            btnMonthly.FlatAppearance.BorderSize = 0
            btnMonthly.FlatStyle = FlatStyle.Flat
            btnMonthly.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnMonthly.ForeColor = Color.White
            btnMonthly.Location = New Point(939, 8)
            btnMonthly.Margin = New Padding(3, 2, 3, 2)
            btnMonthly.Name = "btnMonthly"
            btnMonthly.Padding = New Padding(12, 4, 12, 4)
            btnMonthly.Size = New Size(143, 40)
            btnMonthly.TabIndex = 2
            btnMonthly.Text = "📈 รายเดือน"
            btnMonthly.UseVisualStyleBackColor = False
            ' 
            ' btnLedger
            ' 
            btnLedger.AutoSize = True
            btnLedger.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnLedger.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnLedger.Cursor = Cursors.Hand
            btnLedger.FlatAppearance.BorderSize = 0
            btnLedger.FlatStyle = FlatStyle.Flat
            btnLedger.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnLedger.ForeColor = Color.White
            btnLedger.Location = New Point(1088, 8)
            btnLedger.Margin = New Padding(3, 2, 3, 2)
            btnLedger.Name = "btnLedger"
            btnLedger.Padding = New Padding(12, 4, 12, 4)
            btnLedger.Size = New Size(158, 40)
            btnLedger.TabIndex = 1
            btnLedger.Text = "📒 สมุดรายวัน"
            btnLedger.UseVisualStyleBackColor = False
            ' 
            ' btnSavePdf
            ' 
            btnSavePdf.AutoSize = True
            btnSavePdf.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnSavePdf.BackColor = Color.FromArgb(CByte(202), CByte(37), CByte(90))
            btnSavePdf.Cursor = Cursors.Hand
            btnSavePdf.FlatAppearance.BorderSize = 0
            btnSavePdf.FlatStyle = FlatStyle.Flat
            btnSavePdf.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnSavePdf.ForeColor = Color.White
            btnSavePdf.Location = New Point(1252, 8)
            btnSavePdf.Margin = New Padding(3, 2, 3, 2)
            btnSavePdf.Name = "btnSavePdf"
            btnSavePdf.Padding = New Padding(12, 4, 12, 4)
            btnSavePdf.Size = New Size(159, 40)
            btnSavePdf.TabIndex = 8
            btnSavePdf.Text = "📄 บันทึก PDF"
            btnSavePdf.UseVisualStyleBackColor = False
            ' 
            ' btnPrintReport
            ' 
            btnPrintReport.AutoSize = True
            btnPrintReport.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnPrintReport.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnPrintReport.Cursor = Cursors.Hand
            btnPrintReport.FlatAppearance.BorderSize = 0
            btnPrintReport.FlatStyle = FlatStyle.Flat
            btnPrintReport.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnPrintReport.ForeColor = Color.White
            btnPrintReport.Location = New Point(11, 52)
            btnPrintReport.Margin = New Padding(3, 2, 3, 2)
            btnPrintReport.Name = "btnPrintReport"
            btnPrintReport.Padding = New Padding(12, 4, 12, 4)
            btnPrintReport.Size = New Size(160, 40)
            btnPrintReport.TabIndex = 9
            btnPrintReport.Text = "🖨️ เครื่องพิมพ์"
            btnPrintReport.UseVisualStyleBackColor = False
            ' 
            ' btnPrint
            ' 
            btnPrint.AutoSize = True
            btnPrint.AutoSizeMode = AutoSizeMode.GrowAndShrink
            btnPrint.BackColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            btnPrint.Cursor = Cursors.Hand
            btnPrint.FlatAppearance.BorderSize = 0
            btnPrint.FlatStyle = FlatStyle.Flat
            btnPrint.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
            btnPrint.ForeColor = Color.White
            btnPrint.Location = New Point(177, 52)
            btnPrint.Margin = New Padding(3, 2, 3, 2)
            btnPrint.Name = "btnPrint"
            btnPrint.Padding = New Padding(12, 4, 12, 4)
            btnPrint.Size = New Size(175, 40)
            btnPrint.TabIndex = 0
            btnPrint.Text = "📊 ส่งออกข้อมูล"
            btnPrint.UseVisualStyleBackColor = False
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(253), CByte(224), CByte(71))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 10.0F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 190)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1478, 45)
            lblSummary.TabIndex = 1
            lblSummary.Text = "รายรับรวม 0.00  |  รายจ่ายรวม 0.00  |  ส่วนเกิน 0.00"
            lblSummary.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' dgvReport
            ' 
            DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvReport.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvReport.BackgroundColor = Color.White
            dgvReport.BorderStyle = BorderStyle.None
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvReport.Dock = DockStyle.Fill
            dgvReport.Font = New Font("Tahoma", 9.0F)
            dgvReport.Location = New Point(0, 235)
            dgvReport.Name = "dgvReport"
            dgvReport.ReadOnly = True
            dgvReport.RowHeadersWidth = 50
            dgvReport.RowTemplate.Height = 28
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvReport.Size = New Size(1478, 765)
            dgvReport.TabIndex = 0
            ' 
            ' chartMonthly
            ' 
            chartMonthly.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            chartMonthly.Dock = DockStyle.Fill
            chartMonthly.Location = New Point(0, 235)
            chartMonthly.Name = "chartMonthly"
            chartMonthly.Size = New Size(1478, 765)
            chartMonthly.TabIndex = 2
            chartMonthly.Visible = False
            ' 
            ' FrmReports
            ' 
            AutoScaleDimensions = New SizeF(10.0F, 22.0F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1478, 1000)
            Controls.Add(chartMonthly)
            Controls.Add(dgvReport)
            Controls.Add(lblSummary)
            Controls.Add(pa)
            Controls.Add(p)
            Font = New Font("Tahoma", 9.0F)
            MinimumSize = New Size(1000, 800)
            Name = "FrmReports"
            StartPosition = FormStartPosition.CenterScreen
            Text = "รายงาน"
            WindowState = FormWindowState.Maximized
            p.ResumeLayout(False)
            p.PerformLayout()
            pa.ResumeLayout(False)
            pa.PerformLayout()
            CType(dgvReport, ISupportInitialize).EndInit()
            CType(chartMonthly, ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub
    End Class
End Namespace
