Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmReports
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents lblHeader As Label
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
        Friend WithEvents btnLedger As Button
        Friend WithEvents btnPrint As Button
        Friend WithEvents btnRefresh As Button
        Friend WithEvents btnPrintDetail As Button
        Friend WithEvents btnPrintSummary As Button
        Friend WithEvents dgvReport As DataGridView
        Friend WithEvents lblSummary As Label
        Friend WithEvents p As Panel
        Friend WithEvents pa As Panel

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
            Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
            lblHeader = New Label()
            p = New Panel()
            lbl1 = New Label()
            lbl2 = New Label()
            lbl3 = New Label()
            lbl4 = New Label()
            lbl5 = New Label()
            lblBalance = New Label()
            dtpFrom = New DateTimePicker()
            dtpTo = New DateTimePicker()
            cboType = New ComboBox()
            cboFund = New ComboBox()
            cboBank = New ComboBox()
            txtBalance = New TextBox()
            btnCalcBalance = New Button()
            btnRefresh = New Button()
            pa = New Panel()
            btnPrint = New Button()
            btnLedger = New Button()
            btnMonthly = New Button()
            btnSummaryExpense = New Button()
            btnSummaryIncome = New Button()
            btnPrintSummary = New Button()
            btnPrintDetail = New Button()
            lblSummary = New Label()
            dgvReport = New DataGridView()
            p.SuspendLayout()
            pa.SuspendLayout()
            CType(dgvReport, ISupportInitialize).BeginInit()
            SuspendLayout()
            
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(167), CByte(243), CByte(208))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 16.0F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(24), CByte(83), CByte(63))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1500, 70)
            lblHeader.TabIndex = 4
            lblHeader.Text = "📊 ศูนย์รายงาน"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            
            ' 
            ' p - Filter Panel
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
            p.Location = New Point(0, 70)
            p.Name = "p"
            p.Padding = New Padding(16, 12, 16, 12)
            p.Size = New Size(1500, 100)
            p.TabIndex = 3
            
            ' 
            ' lbl1
            ' 
            lbl1.AutoSize = True
            lbl1.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lbl1.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl1.Location = New Point(16, 12)
            lbl1.Name = "lbl1"
            lbl1.Size = New Size(76, 24)
            lbl1.TabIndex = 0
            lbl1.Text = "จากวันที่:"
            
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 11.0F)
            dtpFrom.Location = New Point(98, 8)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(180, 36)
            dtpFrom.TabIndex = 1
            dtpFrom.Value = New Date(2026, 7, 1, 0, 0, 0, 0)
            
            ' 
            ' lbl2
            ' 
            lbl2.AutoSize = True
            lbl2.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lbl2.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl2.Location = New Point(294, 12)
            lbl2.Name = "lbl2"
            lbl2.Size = New Size(70, 24)
            lbl2.TabIndex = 2
            lbl2.Text = "ถึงวันที่:"
            
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 11.0F)
            dtpTo.Location = New Point(370, 8)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(180, 36)
            dtpTo.TabIndex = 3
            dtpTo.Value = New Date(2026, 7, 31, 0, 0, 0, 0)
            
            ' 
            ' lbl3
            ' 
            lbl3.AutoSize = True
            lbl3.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lbl3.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl3.Location = New Point(566, 12)
            lbl3.Name = "lbl3"
            lbl3.Size = New Size(58, 24)
            lbl3.TabIndex = 4
            lbl3.Text = "ประเภท:"
            
            ' 
            ' cboType
            ' 
            cboType.DropDownStyle = ComboBoxStyle.DropDownList
            cboType.Font = New Font("Tahoma", 11.0F)
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(630, 8)
            cboType.Name = "cboType"
            cboType.Size = New Size(150, 36)
            cboType.TabIndex = 5
            
            ' 
            ' lbl4
            ' 
            lbl4.AutoSize = True
            lbl4.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lbl4.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl4.Location = New Point(16, 52)
            lbl4.Name = "lbl4"
            lbl4.Size = New Size(57, 24)
            lbl4.TabIndex = 6
            lbl4.Text = "กองทุน:"
            
            ' 
            ' cboFund
            ' 
            cboFund.DropDownStyle = ComboBoxStyle.DropDownList
            cboFund.Font = New Font("Tahoma", 11.0F)
            cboFund.Location = New Point(79, 48)
            cboFund.Name = "cboFund"
            cboFund.Size = New Size(200, 36)
            cboFund.TabIndex = 7
            
            ' 
            ' lbl5
            ' 
            lbl5.AutoSize = True
            lbl5.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lbl5.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lbl5.Location = New Point(295, 52)
            lbl5.Name = "lbl5"
            lbl5.Size = New Size(57, 24)
            lbl5.TabIndex = 8
            lbl5.Text = "ธนาคาร:"
            
            ' 
            ' cboBank
            ' 
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboBank.Font = New Font("Tahoma", 11.0F)
            cboBank.Location = New Point(358, 48)
            cboBank.Name = "cboBank"
            cboBank.Size = New Size(200, 36)
            cboBank.TabIndex = 9
            
            ' 
            ' lblBalance
            ' 
            lblBalance.AutoSize = True
            lblBalance.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            lblBalance.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblBalance.Location = New Point(574, 52)
            lblBalance.Name = "lblBalance"
            lblBalance.Size = New Size(74, 24)
            lblBalance.TabIndex = 10
            lblBalance.Text = "ยอดยกมา:"
            
            ' 
            ' txtBalance
            ' 
            txtBalance.Font = New Font("Tahoma", 11.0F)
            txtBalance.Location = New Point(654, 48)
            txtBalance.Name = "txtBalance"
            txtBalance.Size = New Size(100, 36)
            txtBalance.TabIndex = 11
            txtBalance.Text = "0.00"
            txtBalance.TextAlign = HorizontalAlignment.Right
            
            ' 
            ' btnCalcBalance
            ' 
            btnCalcBalance.BackColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
            btnCalcBalance.FlatStyle = FlatStyle.Flat
            btnCalcBalance.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnCalcBalance.ForeColor = Color.White
            btnCalcBalance.Location = New Point(760, 48)
            btnCalcBalance.Name = "btnCalcBalance"
            btnCalcBalance.Size = New Size(45, 36)
            btnCalcBalance.TabIndex = 12
            btnCalcBalance.Text = "🧮"
            btnCalcBalance.UseVisualStyleBackColor = False
            
            ' 
            ' btnRefresh
            ' 
            btnRefresh.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnRefresh.FlatStyle = FlatStyle.Flat
            btnRefresh.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(821, 48)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(130, 36)
            btnRefresh.TabIndex = 13
            btnRefresh.Text = "🔍 ดูรายงาน"
            btnRefresh.UseVisualStyleBackColor = False
            
            ' 
            ' pa - Button Panel
            ' 
            pa.BackColor = Color.FromArgb(CByte(245), CByte(240), CByte(220))
            pa.Controls.Add(btnPrint)
            pa.Controls.Add(btnLedger)
            pa.Controls.Add(btnMonthly)
            pa.Controls.Add(btnSummaryExpense)
            pa.Controls.Add(btnSummaryIncome)
            pa.Controls.Add(btnPrintSummary)
            pa.Controls.Add(btnPrintDetail)
            pa.Dock = DockStyle.Top
            pa.Location = New Point(0, 170)
            pa.Name = "pa"
            pa.Padding = New Padding(16, 10, 16, 10)
            pa.Size = New Size(1500, 80)
            pa.TabIndex = 2
            
            ' 
            ' btnPrint
            ' 
            btnPrint.BackColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            btnPrint.Dock = DockStyle.Right
            btnPrint.FlatStyle = FlatStyle.Flat
            btnPrint.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnPrint.ForeColor = Color.White
            btnPrint.Location = New Point(1334, 10)
            btnPrint.Name = "btnPrint"
            btnPrint.Size = New Size(150, 60)
            btnPrint.TabIndex = 0
            btnPrint.Text = "📊 ส่งไป Excel"
            btnPrint.UseVisualStyleBackColor = False
            
            ' 
            ' btnLedger
            ' 
            btnLedger.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnLedger.Dock = DockStyle.Left
            btnLedger.FlatStyle = FlatStyle.Flat
            btnLedger.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnLedger.ForeColor = Color.White
            btnLedger.Location = New Point(16, 10)
            btnLedger.Name = "btnLedger"
            btnLedger.Size = New Size(180, 60)
            btnLedger.TabIndex = 1
            btnLedger.Text = "📒 สมุดรายวัน"
            btnLedger.UseVisualStyleBackColor = False
            
            ' 
            ' btnMonthly
            ' 
            btnMonthly.BackColor = Color.FromArgb(CByte(126), CByte(34), CByte(206))
            btnMonthly.Dock = DockStyle.Left
            btnMonthly.FlatStyle = FlatStyle.Flat
            btnMonthly.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnMonthly.ForeColor = Color.White
            btnMonthly.Location = New Point(196, 10)
            btnMonthly.Name = "btnMonthly"
            btnMonthly.Size = New Size(160, 60)
            btnMonthly.TabIndex = 2
            btnMonthly.Text = "📈 รายงานรายเดือน"
            btnMonthly.UseVisualStyleBackColor = False
            
            ' 
            ' btnSummaryExpense
            ' 
            btnSummaryExpense.BackColor = Color.FromArgb(CByte(190), CByte(18), CByte(60))
            btnSummaryExpense.Dock = DockStyle.Left
            btnSummaryExpense.FlatStyle = FlatStyle.Flat
            btnSummaryExpense.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnSummaryExpense.ForeColor = Color.White
            btnSummaryExpense.Location = New Point(356, 10)
            btnSummaryExpense.Name = "btnSummaryExpense"
            btnSummaryExpense.Size = New Size(170, 60)
            btnSummaryExpense.TabIndex = 3
            btnSummaryExpense.Text = "💸 สรุปรายจ่าย"
            btnSummaryExpense.UseVisualStyleBackColor = False
            
            ' 
            ' btnSummaryIncome
            ' 
            btnSummaryIncome.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSummaryIncome.Dock = DockStyle.Left
            btnSummaryIncome.FlatStyle = FlatStyle.Flat
            btnSummaryIncome.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnSummaryIncome.ForeColor = Color.White
            btnSummaryIncome.Location = New Point(526, 10)
            btnSummaryIncome.Name = "btnSummaryIncome"
            btnSummaryIncome.Size = New Size(170, 60)
            btnSummaryIncome.TabIndex = 4
            btnSummaryIncome.Text = "💵 สรุปรายรับ"
            btnSummaryIncome.UseVisualStyleBackColor = False
            
            ' 
            ' btnPrintSummary
            ' 
            btnPrintSummary.BackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnPrintSummary.Dock = DockStyle.Left
            btnPrintSummary.FlatStyle = FlatStyle.Flat
            btnPrintSummary.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnPrintSummary.ForeColor = Color.White
            btnPrintSummary.Location = New Point(696, 10)
            btnPrintSummary.Name = "btnPrintSummary"
            btnPrintSummary.Size = New Size(160, 60)
            btnPrintSummary.TabIndex = 5
            btnPrintSummary.Text = "🧾 พิมพ์รายงานย่อ"
            btnPrintSummary.UseVisualStyleBackColor = False
            
            ' 
            ' btnPrintDetail
            ' 
            btnPrintDetail.BackColor = Color.FromArgb(CByte(185), CByte(28), CByte(28))
            btnPrintDetail.Dock = DockStyle.Left
            btnPrintDetail.FlatStyle = FlatStyle.Flat
            btnPrintDetail.Font = New Font("Tahoma", 11.0F, FontStyle.Bold)
            btnPrintDetail.ForeColor = Color.White
            btnPrintDetail.Location = New Point(856, 10)
            btnPrintDetail.Name = "btnPrintDetail"
            btnPrintDetail.Size = New Size(170, 60)
            btnPrintDetail.TabIndex = 6
            btnPrintDetail.Text = "📜 พิมพ์รายงานละเอียด"
            btnPrintDetail.UseVisualStyleBackColor = False
            
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(253), CByte(224), CByte(71))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 12.0F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 250)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1500, 60)
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
            dgvReport.Font = New Font("Tahoma", 10.0F)
            dgvReport.Location = New Point(0, 310)
            dgvReport.Name = "dgvReport"
            dgvReport.ReadOnly = True
            dgvReport.RowHeadersWidth = 62
            dgvReport.RowTemplate.Height = 32
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvReport.Size = New Size(1500, 490)
            dgvReport.TabIndex = 0
            
            ' 
            ' FrmReports
            ' 
            AutoScaleDimensions = New SizeF(7.0F, 18.0F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1500, 800)
            Controls.Add(dgvReport)
            Controls.Add(lblSummary)
            Controls.Add(pa)
            Controls.Add(p)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1280, 700)
            Name = "FrmReports"
            StartPosition = FormStartPosition.CenterScreen
            Text = "รายงาน"
            WindowState = FormWindowState.Maximized
            p.ResumeLayout(False)
            p.PerformLayout()
            pa.ResumeLayout(False)
            CType(dgvReport, ISupportInitialize).EndInit()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
