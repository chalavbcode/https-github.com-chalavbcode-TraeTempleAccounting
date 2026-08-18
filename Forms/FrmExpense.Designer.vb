Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmExpense
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboCategory As ComboBox
        Friend WithEvents cboFund As ComboBox
        Friend WithEvents cboBank As ComboBox
        Friend WithEvents txtDescription As TextBox
        Friend WithEvents txtAmount As TextBox
        Friend WithEvents txtRemark As TextBox
        Friend WithEvents txtReceipt As TextBox
        Friend WithEvents btnBrowseReceipt As Button
        Friend WithEvents btnPasteReceipt As Button
        Friend WithEvents btnClearReceipt As Button
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnImportExcel As Button
        Friend WithEvents ttMain As ToolTip
        Friend WithEvents lbl1 As Label
        Friend WithEvents lbl2 As Label
        Friend WithEvents lbl3 As Label
        Friend WithEvents lbl4 As Label
        Friend WithEvents lbl5 As Label
        Friend WithEvents lbl6 As Label
        Friend WithEvents lbl7 As Label
        Friend WithEvents lbl8 As Label
        Friend WithEvents dgvRecentTransactions As DataGridView
        Friend WithEvents lblReceiptCount As Label
        Friend WithEvents lblMaxAmount As Label
        Friend WithEvents lblAvgAmount As Label
        Friend WithEvents lblTotalMonthlyIncome As Label

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
            ttMain = New ToolTip(components)
            lbl1 = New Label()
            lbl2 = New Label()
            lbl3 = New Label()
            lbl4 = New Label()
            lbl5 = New Label()
            lbl6 = New Label()
            lbl7 = New Label()
            lbl8 = New Label()
            dtpDate = New DateTimePicker()
            cboCategory = New ComboBox()
            cboFund = New ComboBox()
            cboBank = New ComboBox()
            txtDescription = New TextBox()
            txtAmount = New TextBox()
            txtRemark = New TextBox()
            txtReceipt = New TextBox()
            btnBrowseReceipt = New Button()
            btnPasteReceipt = New Button()
            lblReceiptCount = New Label()
            lblMaxAmount = New Label()
            lblAvgAmount = New Label()
            lblTotalMonthlyIncome = New Label()
            btnClearReceipt = New Button()
            btnSave = New Button()
            btnCancel = New Button()
            btnImportExcel = New Button()
            dgvRecentTransactions = New DataGridView()
            CType(dgvRecentTransactions, ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' lbl1
            ' 
            lbl1.Location = New Point(40, 0)
            lbl1.Name = "lbl1"
            lbl1.Size = New Size(190, 40)
            lbl1.TabIndex = 0
            lbl1.Text = "วันที่ทำรายการ:"
            lbl1.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl2
            ' 
            lbl2.Location = New Point(40, 62)
            lbl2.Name = "lbl2"
            lbl2.Size = New Size(190, 40)
            lbl2.TabIndex = 1
            lbl2.Text = "ประเภทรายจ่าย:"
            lbl2.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl3
            ' 
            lbl3.Location = New Point(40, 124)
            lbl3.Name = "lbl3"
            lbl3.Size = New Size(190, 40)
            lbl3.TabIndex = 2
            lbl3.Text = "กองทุน:"
            lbl3.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl4
            ' 
            lbl4.Location = New Point(40, 186)
            lbl4.Name = "lbl4"
            lbl4.Size = New Size(190, 40)
            lbl4.TabIndex = 3
            lbl4.Text = "บัญชีธนาคาร (ถ้ามี):"
            lbl4.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl5
            ' 
            lbl5.Location = New Point(40, 248)
            lbl5.Name = "lbl5"
            lbl5.Size = New Size(190, 40)
            lbl5.TabIndex = 4
            lbl5.Text = "รายละเอียดรายการ:"
            lbl5.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl6
            ' 
            lbl6.Location = New Point(40, 310)
            lbl6.Name = "lbl6"
            lbl6.Size = New Size(190, 40)
            lbl6.TabIndex = 5
            lbl6.Text = "จำนวนเงิน (บาท):"
            lbl6.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl7
            ' 
            lbl7.Location = New Point(40, 372)
            lbl7.Name = "lbl7"
            lbl7.Size = New Size(190, 40)
            lbl7.TabIndex = 6
            lbl7.Text = "หมายเหตุ:"
            lbl7.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl8
            ' 
            lbl8.Location = New Point(40, 484)
            lbl8.Name = "lbl8"
            lbl8.Size = New Size(190, 40)
            lbl8.TabIndex = 7
            lbl8.Text = "หลักฐาน/ใบเสร็จ:"
            lbl8.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpDate
            ' 
            dtpDate.Font = New Font("Tahoma", 10.5F)
            dtpDate.Location = New Point(240, 0)
            dtpDate.Name = "dtpDate"
            dtpDate.Size = New Size(520, 33)
            dtpDate.TabIndex = 0
            dtpDate.Value = New Date(2026, 8, 1, 0, 0, 0, 0)
            ' 
            ' cboCategory
            ' 
            cboCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            cboCategory.AutoCompleteSource = AutoCompleteSource.CustomSource
            cboCategory.Font = New Font("Tahoma", 10.5F)
            cboCategory.Location = New Point(240, 62)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(520, 33)
            cboCategory.TabIndex = 1
            ' 
            ' cboFund
            ' 
            cboFund.DropDownStyle = ComboBoxStyle.DropDownList
            cboFund.Font = New Font("Tahoma", 10.5F)
            cboFund.Location = New Point(240, 124)
            cboFund.Name = "cboFund"
            cboFund.Size = New Size(520, 33)
            cboFund.TabIndex = 5
            ' 
            ' cboBank
            ' 
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboBank.Font = New Font("Tahoma", 10.5F)
            cboBank.Location = New Point(240, 186)
            cboBank.Name = "cboBank"
            cboBank.Size = New Size(520, 33)
            cboBank.TabIndex = 6
            ' 
            ' txtDescription
            ' 
            txtDescription.Font = New Font("Tahoma", 10.5F)
            txtDescription.Location = New Point(240, 248)
            txtDescription.Name = "txtDescription"
            txtDescription.Size = New Size(520, 33)
            txtDescription.TabIndex = 2
            ' 
            ' txtAmount
            ' 
            txtAmount.Font = New Font("Tahoma", 11.5F, FontStyle.Bold)
            txtAmount.ForeColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            txtAmount.Location = New Point(240, 310)
            txtAmount.Name = "txtAmount"
            txtAmount.Size = New Size(260, 35)
            txtAmount.TabIndex = 3
            txtAmount.TextAlign = HorizontalAlignment.Right
            ' 
            ' txtRemark
            ' 
            txtRemark.Font = New Font("Tahoma", 10.5F)
            txtRemark.Location = New Point(240, 372)
            txtRemark.Multiline = True
            txtRemark.Name = "txtRemark"
            txtRemark.ScrollBars = ScrollBars.Vertical
            txtRemark.Size = New Size(520, 100)
            txtRemark.TabIndex = 7
            ' 
            ' txtReceipt
            ' 
            txtReceipt.BackColor = Color.White
            txtReceipt.Font = New Font("Tahoma", 10.5F)
            txtReceipt.Location = New Point(240, 484)
            txtReceipt.Name = "txtReceipt"
            txtReceipt.ReadOnly = True
            txtReceipt.Size = New Size(300, 33)
            txtReceipt.TabIndex = 8
            ' 
            ' btnBrowseReceipt
            ' 
            btnBrowseReceipt.BackColor = Color.FromArgb(CByte(79), CByte(70), CByte(229))
            btnBrowseReceipt.Cursor = Cursors.Hand
            btnBrowseReceipt.FlatStyle = FlatStyle.Flat
            btnBrowseReceipt.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnBrowseReceipt.ForeColor = Color.White
            btnBrowseReceipt.Location = New Point(550, 484)
            btnBrowseReceipt.Name = "btnBrowseReceipt"
            btnBrowseReceipt.Size = New Size(157, 40)
            btnBrowseReceipt.TabIndex = 9
            btnBrowseReceipt.Text = "📂 เลือกรูปภาพ"
            btnBrowseReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnPasteReceipt
            ' 
            btnPasteReceipt.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnPasteReceipt.Cursor = Cursors.Hand
            btnPasteReceipt.FlatStyle = FlatStyle.Flat
            btnPasteReceipt.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnPasteReceipt.ForeColor = Color.White
            btnPasteReceipt.Location = New Point(713, 485)
            btnPasteReceipt.Name = "btnPasteReceipt"
            btnPasteReceipt.Size = New Size(169, 40)
            btnPasteReceipt.TabIndex = 10
            btnPasteReceipt.Text = "📋 วางจาก LINE"
            btnPasteReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnClearReceipt
            ' 
            btnClearReceipt.BackColor = Color.FromArgb(CByte(220), CByte(38), CByte(38))
            btnClearReceipt.Cursor = Cursors.Hand
            btnClearReceipt.FlatStyle = FlatStyle.Flat
            btnClearReceipt.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnClearReceipt.ForeColor = Color.White
            btnClearReceipt.Location = New Point(888, 484)
            btnClearReceipt.Name = "btnClearReceipt"
            btnClearReceipt.Size = New Size(50, 40)
            btnClearReceipt.TabIndex = 11
            btnClearReceipt.Text = "🗑️"
            btnClearReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnSave
            ' 
            btnSave.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnSave.Cursor = Cursors.Hand
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            btnSave.ForeColor = Color.White
            btnSave.Location = New Point(240, 552)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(240, 56)
            btnSave.TabIndex = 4
            btnSave.Text = "💾 บันทึกรายการ"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' btnCancel
            ' 
            btnCancel.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnCancel.Cursor = Cursors.Hand
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            btnCancel.ForeColor = Color.White
            btnCancel.Location = New Point(500, 552)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(180, 56)
            btnCancel.TabIndex = 12
            btnCancel.Text = "❌ เคลียร์"
            btnCancel.UseVisualStyleBackColor = False
            ' 
            ' btnImportExcel
            ' 
            btnImportExcel.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnImportExcel.Cursor = Cursors.Hand
            btnImportExcel.FlatStyle = FlatStyle.Flat
            btnImportExcel.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            ' lblReceiptCount
            ' 
            lblReceiptCount.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblReceiptCount.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            lblReceiptCount.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblReceiptCount.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblReceiptCount.Location = New Point(643, 0)
            lblReceiptCount.Name = "lblReceiptCount"
            lblReceiptCount.Size = New Size(250, 28)
            lblReceiptCount.TabIndex = 31
            lblReceiptCount.Text = "จำนวน 0 ใบ"
            lblReceiptCount.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblMaxAmount
            ' 
            lblMaxAmount.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblMaxAmount.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            lblMaxAmount.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblMaxAmount.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblMaxAmount.Location = New Point(891, 0)
            lblMaxAmount.Name = "lblMaxAmount"
            lblMaxAmount.Size = New Size(250, 28)
            lblMaxAmount.TabIndex = 33
            lblMaxAmount.Text = "สูงสุด 0.00"
            lblMaxAmount.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblAvgAmount
            ' 
            lblAvgAmount.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblAvgAmount.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            lblAvgAmount.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblAvgAmount.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblAvgAmount.Location = New Point(1139, 0)
            lblAvgAmount.Name = "lblAvgAmount"
            lblAvgAmount.Size = New Size(250, 28)
            lblAvgAmount.TabIndex = 35
            lblAvgAmount.Text = "เฉลี่ย 0.00"
            lblAvgAmount.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblTotalMonthlyIncome
            ' 
            lblTotalMonthlyIncome.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblTotalMonthlyIncome.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            lblTotalMonthlyIncome.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblTotalMonthlyIncome.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblTotalMonthlyIncome.Location = New Point(1387, 0)
            lblTotalMonthlyIncome.Name = "lblTotalMonthlyIncome"
            lblTotalMonthlyIncome.Size = New Size(250, 28)
            lblTotalMonthlyIncome.TabIndex = 37
            lblTotalMonthlyIncome.Text = "รวมรายจ่าย 0.00"
            lblTotalMonthlyIncome.TextAlign = ContentAlignment.MiddleRight
            ' 
            btnImportExcel.ForeColor = Color.White
            btnImportExcel.Location = New Point(750, 552)
            btnImportExcel.Name = "btnImportExcel"
            btnImportExcel.Size = New Size(240, 56)
            btnImportExcel.TabIndex = 13
            btnImportExcel.Text = "📥 นำเข้าจาก Excel"
            btnImportExcel.UseVisualStyleBackColor = False
            ' 
            dgvRecentTransactions.Location = New Point(770, 28)
            ' 
            ' dgvRecentTransactions
            ' 
            dgvRecentTransactions.AllowUserToAddRows = False
            dgvRecentTransactions.AllowUserToDeleteRows = False
            dgvRecentTransactions.AllowUserToResizeRows = False
            dgvRecentTransactions.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            dgvRecentTransactions.BackgroundColor = Color.White
            dgvRecentTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvRecentTransactions.Location = New Point(770, 28)
            dgvRecentTransactions.MultiSelect = False
            dgvRecentTransactions.Name = "dgvRecentTransactions"
            dgvRecentTransactions.ReadOnly = True
            dgvRecentTransactions.RowHeadersVisible = False
            dgvRecentTransactions.RowHeadersWidth = 62
            dgvRecentTransactions.ScrollBars = ScrollBars.Both
            dgvRecentTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvRecentTransactions.Size = New Size(857, 686)
            dgvRecentTransactions.TabIndex = 26
            ' 
            ' FrmExpense
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1639, 900)
            Controls.Add(lbl1)
            Controls.Add(lbl2)
            Controls.Add(lbl3)
            Controls.Add(lbl4)
            Controls.Add(lbl5)
            Controls.Add(lbl6)
            Controls.Add(lbl7)
            Controls.Add(lbl8)
            Controls.Add(dtpDate)
            Controls.Add(cboCategory)
            Controls.Add(cboFund)
            Controls.Add(cboBank)
            Controls.Add(txtDescription)
            Controls.Add(txtAmount)
            Controls.Add(txtRemark)
            Controls.Add(txtReceipt)
            Controls.Add(btnBrowseReceipt)
            Controls.Add(btnPasteReceipt)
            Controls.Add(btnClearReceipt)
            Controls.Add(btnSave)
            Controls.Add(btnCancel)
            Controls.Add(btnImportExcel)
            Controls.Add(dgvRecentTransactions)
            Controls.Add(lblReceiptCount)
            Controls.Add(lblMaxAmount)
            Controls.Add(lblAvgAmount)
            Controls.Add(lblTotalMonthlyIncome)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1100, 900)
            Name = "FrmExpense"
            StartPosition = FormStartPosition.CenterScreen
            Text = "บันทึกรายจ่าย"
            WindowState = FormWindowState.Maximized
            CType(dgvRecentTransactions, ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub
    End Class
End Namespace
