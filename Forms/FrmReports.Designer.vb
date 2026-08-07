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
        Friend WithEvents dgvReport As DataGridView
        Friend WithEvents lblSummary As Label
        Friend WithEvents p As Panel
        Friend WithEvents pa As Panel
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
            pa = New Panel()
            btnPrint = New Button()
            btnLedger = New Button()
            btnShowChart = New Button()
            btnMonthly = New Button()
            btnSummaryExpense = New Button()
            btnSummaryIncome = New Button()
            btnPrintSummary = New Button()
            btnPrintDetail = New Button()
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
            p.Dock = DockStyle.Top
            p.Location = New Point(0, 0)
            p.Name = "p"
            p.Padding = New Padding(12, 8, 12, 8)
            p.Size = New Size(1200, 90)
            p.TabIndex = 3
            ' 
            ' lbl1
            ' 
            lbl1.AutoSize = True
            lbl1.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            lbl1.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl1.Location = New Point(15, 5)
            lbl1.Name = "lbl1"
            lbl1.Size = New Size(85, 22)
            lbl1.TabIndex = 0
            lbl1.Text = "จากวันที่:"
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 9F)
            dtpFrom.Location = New Point(138, 3)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(140, 29)
            dtpFrom.TabIndex = 1
            dtpFrom.Value = New Date(2026, 7, 1, 0, 0, 0, 0)
            ' 
            ' lbl2
            ' 
            lbl2.AutoSize = True
            lbl2.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            lbl2.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl2.Location = New Point(349, 5)
            lbl2.Name = "lbl2"
            lbl2.Size = New Size(74, 22)
            lbl2.TabIndex = 2
            lbl2.Text = "ถึงวันที่:"
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 9F)
            dtpTo.Location = New Point(444, 3)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(140, 29)
            dtpTo.TabIndex = 3
            dtpTo.Value = New Date(2026, 7, 31, 0, 0, 0, 0)
            ' 
            ' lbl3
            ' 
            lbl3.AutoSize = True
            lbl3.Font = New Font("Tahoma", 9F, FontStyle.Bold)
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
            cboType.Font = New Font("Tahoma", 9F)
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(775, 3)
            cboType.Name = "cboType"
            cboType.Size = New Size(120, 30)
            cboType.TabIndex = 5
            ' 
            ' lbl4
            ' 
            lbl4.AutoSize = True
            lbl4.Font = New Font("Tahoma", 9F, FontStyle.Bold)
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
            cboFund.Font = New Font("Tahoma", 9F)
            cboFund.Location = New Point(93, 45)
            cboFund.Name = "cboFund"
            cboFund.Size = New Size(160, 30)
            cboFund.TabIndex = 7
            ' 
            ' lbl5
            ' 
            lbl5.AutoSize = True
            lbl5.Font = New Font("Tahoma", 9F, FontStyle.Bold)
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
            cboBank.Font = New Font("Tahoma", 9F)
            cboBank.Location = New Point(368, 45)
            cboBank.Name = "cboBank"
            cboBank.Size = New Size(160, 30)
            cboBank.TabIndex = 9
            ' 
            ' lblBalance
            ' 
            lblBalance.AutoSize = True
            lblBalance.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            lblBalance.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblBalance.Location = New Point(544, 47)
            lblBalance.Name = "lblBalance"
            lblBalance.Size = New Size(98, 22)
            lblBalance.TabIndex = 10
            lblBalance.Text = "ยอดยกมา:"
            ' 
            ' txtBalance
            ' 
            txtBalance.Font = New Font("Tahoma", 9F)
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
            btnCalcBalance.Font = New Font("Tahoma", 9F, FontStyle.Bold)
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
            btnRefresh.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(831, 45)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(100, 30)
            btnRefresh.TabIndex = 13
            btnRefresh.Text = "🔍 ดูรายงาน"
            btnRefresh.UseVisualStyleBackColor = False
            ' 
            ' pa
            ' 
            pa.BackColor = Color.FromArgb(CByte(245), CByte(240), CByte(220))
            pa.Controls.Add(btnPrint)
            pa.Controls.Add(btnLedger)
            pa.Controls.Add(btnMonthly)
            pa.Controls.Add(btnShowChart)
            pa.Controls.Add(btnSummaryExpense)
            pa.Controls.Add(btnSummaryIncome)
            pa.Controls.Add(btnPrintSummary)
            pa.Controls.Add(btnPrintDetail)
            pa.Dock = DockStyle.Top
            pa.Location = New Point(0, 90)
            pa.Name = "pa"
            pa.Padding = New Padding(8, 6, 8, 6)
            pa.Size = New Size(1200, 50)
            pa.TabIndex = 2
            ' 
            ' btnPrint
            ' 
            btnPrint.BackColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            btnPrint.Cursor = Cursors.Hand
            btnPrint.Dock = DockStyle.Right
            btnPrint.FlatAppearance.BorderSize = 0
            btnPrint.FlatStyle = FlatStyle.Flat
            btnPrint.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnPrint.ForeColor = Color.White
            btnPrint.Location = New Point(1087, 6)
            btnPrint.Name = "btnPrint"
            btnPrint.Size = New Size(105, 38)
            btnPrint.TabIndex = 0
            btnPrint.Text = "📊 Excel"
            btnPrint.UseVisualStyleBackColor = False
            ' 
            ' btnLedger
            ' 
            btnLedger.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnLedger.Cursor = Cursors.Hand
            btnLedger.Dock = DockStyle.Left
            btnLedger.FlatAppearance.BorderSize = 0
            btnLedger.FlatStyle = FlatStyle.Flat
            btnLedger.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnLedger.ForeColor = Color.White
            btnLedger.Location = New Point(791, 6)
            btnLedger.Name = "btnLedger"
            btnLedger.Size = New Size(130, 38)
            btnLedger.TabIndex = 1
            btnLedger.Text = "📒 สมุดรายวัน"
            btnLedger.UseVisualStyleBackColor = False
            ' 
            ' btnMonthly
            ' 
            btnMonthly.BackColor = Color.FromArgb(CByte(126), CByte(34), CByte(206))
            btnMonthly.Cursor = Cursors.Hand
            btnMonthly.Dock = DockStyle.Left
            btnMonthly.FlatAppearance.BorderSize = 0
            btnMonthly.FlatStyle = FlatStyle.Flat
            btnMonthly.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnMonthly.ForeColor = Color.White
            btnMonthly.Location = New Point(646, 6)
            btnMonthly.Name = "btnMonthly"
            btnMonthly.Size = New Size(120, 38)
            btnMonthly.TabIndex = 2
            btnMonthly.Text = "📈 รายเดือน"
            btnMonthly.UseVisualStyleBackColor = False
            ' 
            ' btnShowChart
            ' 
            btnShowChart.BackColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            btnShowChart.Cursor = Cursors.Hand
            btnShowChart.Dock = DockStyle.Left
            btnShowChart.FlatAppearance.BorderSize = 0
            btnShowChart.FlatStyle = FlatStyle.Flat
            btnShowChart.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnShowChart.ForeColor = Color.White
            btnShowChart.Location = New Point(481, 6)
            btnShowChart.Name = "btnShowChart"
            btnShowChart.Size = New Size(165, 38)
            btnShowChart.TabIndex = 7
            btnShowChart.Text = "📊 กราฟสรุปรายเดือน"
            btnShowChart.UseVisualStyleBackColor = False
            ' 
            ' btnSummaryExpense
            ' 
            btnSummaryExpense.BackColor = Color.FromArgb(CByte(190), CByte(18), CByte(60))
            btnSummaryExpense.Cursor = Cursors.Hand
            btnSummaryExpense.Dock = DockStyle.Left
            btnSummaryExpense.FlatAppearance.BorderSize = 0
            btnSummaryExpense.FlatStyle = FlatStyle.Flat
            btnSummaryExpense.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnSummaryExpense.ForeColor = Color.White
            btnSummaryExpense.Location = New Point(484, 6)
            btnSummaryExpense.Name = "btnSummaryExpense"
            btnSummaryExpense.Size = New Size(158, 38)
            btnSummaryExpense.TabIndex = 3
            btnSummaryExpense.Text = "💸 สรุปรายจ่าย"
            btnSummaryExpense.UseVisualStyleBackColor = False
            ' 
            ' btnSummaryIncome
            ' 
            btnSummaryIncome.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSummaryIncome.Cursor = Cursors.Hand
            btnSummaryIncome.Dock = DockStyle.Left
            btnSummaryIncome.FlatAppearance.BorderSize = 0
            btnSummaryIncome.FlatStyle = FlatStyle.Flat
            btnSummaryIncome.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnSummaryIncome.ForeColor = Color.White
            btnSummaryIncome.Location = New Point(335, 6)
            btnSummaryIncome.Name = "btnSummaryIncome"
            btnSummaryIncome.Size = New Size(149, 38)
            btnSummaryIncome.TabIndex = 4
            btnSummaryIncome.Text = "💵 สรุปรายรับ"
            btnSummaryIncome.UseVisualStyleBackColor = False
            ' 
            ' btnPrintSummary
            ' 
            btnPrintSummary.BackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnPrintSummary.Cursor = Cursors.Hand
            btnPrintSummary.Dock = DockStyle.Left
            btnPrintSummary.FlatAppearance.BorderSize = 0
            btnPrintSummary.FlatStyle = FlatStyle.Flat
            btnPrintSummary.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnPrintSummary.ForeColor = Color.White
            btnPrintSummary.Location = New Point(192, 6)
            btnPrintSummary.Name = "btnPrintSummary"
            btnPrintSummary.Size = New Size(143, 38)
            btnPrintSummary.TabIndex = 5
            btnPrintSummary.Text = ChrW(55358) & ChrW(56830) & " รายงานย่อ"
            btnPrintSummary.UseVisualStyleBackColor = False
            ' 
            ' btnPrintDetail
            ' 
            btnPrintDetail.BackColor = Color.FromArgb(CByte(185), CByte(28), CByte(28))
            btnPrintDetail.Cursor = Cursors.Hand
            btnPrintDetail.Dock = DockStyle.Left
            btnPrintDetail.FlatAppearance.BorderSize = 0
            btnPrintDetail.FlatStyle = FlatStyle.Flat
            btnPrintDetail.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnPrintDetail.ForeColor = Color.White
            btnPrintDetail.Location = New Point(8, 6)
            btnPrintDetail.Name = "btnPrintDetail"
            btnPrintDetail.Size = New Size(184, 38)
            btnPrintDetail.TabIndex = 6
            btnPrintDetail.Text = "📜 รายงานละเอียด"
            btnPrintDetail.UseVisualStyleBackColor = False
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(253), CByte(224), CByte(71))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 140)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1200, 45)
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
            dgvReport.Font = New Font("Tahoma", 9F)
            dgvReport.Location = New Point(0, 185)
            dgvReport.Name = "dgvReport"
            dgvReport.ReadOnly = True
            dgvReport.RowHeadersWidth = 50
            dgvReport.RowTemplate.Height = 28
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvReport.Size = New Size(1200, 815)
            dgvReport.TabIndex = 0
            ' 
            ' chartMonthly
            ' 
            chartMonthly.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            chartMonthly.BorderSkin.SkinStyle = BorderSkinStyle.None
            chartMonthly.Dock = DockStyle.Fill
            chartMonthly.Location = New Point(0, 185)
            chartMonthly.Name = "chartMonthly"
            chartMonthly.Size = New Size(1200, 815)
            chartMonthly.TabIndex = 2
            chartMonthly.TabStop = True
            chartMonthly.Visible = False
            ' 
            ' FrmReports
            ' 
            AutoScaleDimensions = New SizeF(10F, 22F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1200, 1000)
            Controls.Add(chartMonthly)
            Controls.Add(dgvReport)
            Controls.Add(lblSummary)
            Controls.Add(pa)
            Controls.Add(p)
            Font = New Font("Tahoma", 9F)
            MinimumSize = New Size(1000, 800)
            Name = "FrmReports"
            StartPosition = FormStartPosition.CenterScreen
            Text = "รายงาน"
            WindowState = FormWindowState.Maximized
            p.ResumeLayout(False)
            p.PerformLayout()
            pa.ResumeLayout(False)
            CType(dgvReport, ISupportInitialize).EndInit()
            CType(chartMonthly, ISupportInitialize).EndInit()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
