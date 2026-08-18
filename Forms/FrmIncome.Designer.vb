Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmIncome
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
        Friend WithEvents lblHeader As Label
        Friend WithEvents lbl1 As Label
        Friend WithEvents lbl2 As Label
        Friend WithEvents lbl3 As Label
        Friend WithEvents lbl4 As Label
        Friend WithEvents lbl5 As Label
        Friend WithEvents lbl6 As Label
        Friend WithEvents lbl7 As Label
        Friend WithEvents lbl8 As Label
        Friend WithEvents cboReceiveChannel As ComboBox
        Friend WithEvents lblChannel As Label
        Friend WithEvents txtReferenceNo As TextBox
        Friend WithEvents lblRefNo As Label
        Friend WithEvents dgvRecentTransactions As DataGridView
        Friend lblRecentHeader As Label
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
            lblHeader = New Label()
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
            btnClearReceipt = New Button()
            btnSave = New Button()
            btnCancel = New Button()
            btnImportExcel = New Button()
            cboReceiveChannel = New ComboBox()
            lblChannel = New Label()
            txtReferenceNo = New TextBox()
            lblRefNo = New Label()
            lblRecentHeader = New Label()
            lblReceiptCount = New Label()
            lblMaxAmount = New Label()
            lblAvgAmount = New Label()
            lblTotalMonthlyIncome = New Label()
            dgvRecentTransactions = New DataGridView()
            CType(dgvRecentTransactions, ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 15F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Padding = New Padding(0, 8, 0, 8)
            lblHeader.Size = New Size(1773, 70)
            lblHeader.TabIndex = 22
            lblHeader.Text = "💰 บันทึกรายรับเงินเข้าวัด"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblReceiptCount
            ' 
            lblReceiptCount.BackColor = Color.White
            lblReceiptCount.BorderStyle = BorderStyle.FixedSingle
            lblReceiptCount.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            lblReceiptCount.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
            lblReceiptCount.Location = New Point(40, 78)
            lblReceiptCount.Name = "lblReceiptCount"
            lblReceiptCount.Size = New Size(380, 40)
            lblReceiptCount.TabIndex = 31
            lblReceiptCount.Text = "0 ใบ"
            lblReceiptCount.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblMaxAmount
            ' 
            lblMaxAmount.BackColor = Color.White
            lblMaxAmount.BorderStyle = BorderStyle.FixedSingle
            lblMaxAmount.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            lblMaxAmount.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
            lblMaxAmount.Location = New Point(435, 78)
            lblMaxAmount.Name = "lblMaxAmount"
            lblMaxAmount.Size = New Size(380, 40)
            lblMaxAmount.TabIndex = 33
            lblMaxAmount.Text = "0.00"
            lblMaxAmount.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblAvgAmount
            ' 
            lblAvgAmount.BackColor = Color.White
            lblAvgAmount.BorderStyle = BorderStyle.FixedSingle
            lblAvgAmount.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            lblAvgAmount.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
            lblAvgAmount.Location = New Point(830, 78)
            lblAvgAmount.Name = "lblAvgAmount"
            lblAvgAmount.Size = New Size(380, 40)
            lblAvgAmount.TabIndex = 35
            lblAvgAmount.Text = "0.00"
            lblAvgAmount.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lblTotalMonthlyIncome
            ' 
            lblTotalMonthlyIncome.BackColor = Color.White
            lblTotalMonthlyIncome.BorderStyle = BorderStyle.FixedSingle
            lblTotalMonthlyIncome.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            lblTotalMonthlyIncome.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
            lblTotalMonthlyIncome.Location = New Point(1225, 78)
            lblTotalMonthlyIncome.Name = "lblTotalMonthlyIncome"
            lblTotalMonthlyIncome.Size = New Size(380, 40)
            lblTotalMonthlyIncome.TabIndex = 37
            lblTotalMonthlyIncome.Text = "0.00"
            lblTotalMonthlyIncome.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' lbl1
            ' 
            lbl1.Location = New Point(40, 130)
            lbl1.Name = "lbl1"
            lbl1.Size = New Size(190, 40)
            lbl1.TabIndex = 0
            lbl1.Text = "วันที่ทำรายการ:"
            lbl1.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl2
            ' 
            lbl2.Location = New Point(40, 192)
            lbl2.Name = "lbl2"
            lbl2.Size = New Size(190, 40)
            lbl2.TabIndex = 1
            lbl2.Text = "ประเภทรายรับ:"
            lbl2.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl3
            ' 
            lbl3.Location = New Point(40, 254)
            lbl3.Name = "lbl3"
            lbl3.Size = New Size(190, 40)
            lbl3.TabIndex = 2
            lbl3.Text = "กองทุน:"
            lbl3.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl4
            ' 
            lbl4.Location = New Point(40, 316)
            lbl4.Name = "lbl4"
            lbl4.Size = New Size(190, 40)
            lbl4.TabIndex = 3
            lbl4.Text = "บัญชีธนาคาร (ถ้ามี):"
            lbl4.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl5
            ' 
            lbl5.Location = New Point(40, 440)
            lbl5.Name = "lbl5"
            lbl5.Size = New Size(190, 40)
            lbl5.TabIndex = 4
            lbl5.Text = "รายละเอียดรายการ:"
            lbl5.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl6
            ' 
            lbl6.Location = New Point(40, 502)
            lbl6.Name = "lbl6"
            lbl6.Size = New Size(190, 40)
            lbl6.TabIndex = 5
            lbl6.Text = "จำนวนเงิน (บาท):"
            lbl6.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl7
            ' 
            lbl7.Location = New Point(40, 564)
            lbl7.Name = "lbl7"
            lbl7.Size = New Size(190, 40)
            lbl7.TabIndex = 6
            lbl7.Text = "หมายเหตุ:"
            lbl7.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lbl8
            ' 
            lbl8.Location = New Point(40, 676)
            lbl8.Name = "lbl8"
            lbl8.Size = New Size(190, 40)
            lbl8.TabIndex = 7
            lbl8.Text = "หลักฐาน/ใบเสร็จ:"
            lbl8.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpDate
            ' 
            dtpDate.Font = New Font("Tahoma", 10.5F)
            dtpDate.Location = New Point(240, 130)
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
            cboCategory.Location = New Point(240, 192)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(520, 33)
            cboCategory.TabIndex = 1
            ' 
            ' cboFund
            ' 
            cboFund.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            cboFund.AutoCompleteSource = AutoCompleteSource.CustomSource
            cboFund.Font = New Font("Tahoma", 10.5F)
            cboFund.Location = New Point(240, 254)
            cboFund.Name = "cboFund"
            cboFund.Size = New Size(520, 33)
            cboFund.TabIndex = 6
            ' 
            ' cboBank
            ' 
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboBank.Font = New Font("Tahoma", 10.5F)
            cboBank.Location = New Point(240, 316)
            cboBank.Name = "cboBank"
            cboBank.Size = New Size(520, 33)
            cboBank.TabIndex = 7
            ' 
            ' txtDescription
            ' 
            txtDescription.Font = New Font("Tahoma", 10.5F)
            txtDescription.Location = New Point(240, 440)
            txtDescription.Name = "txtDescription"
            txtDescription.Size = New Size(520, 33)
            txtDescription.TabIndex = 3
            ' 
            ' txtAmount
            ' 
            txtAmount.Font = New Font("Tahoma", 11.5F, FontStyle.Bold)
            txtAmount.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            txtAmount.Location = New Point(240, 502)
            txtAmount.Name = "txtAmount"
            txtAmount.Size = New Size(260, 35)
            txtAmount.TabIndex = 4
            txtAmount.TextAlign = HorizontalAlignment.Right
            ' 
            ' txtRemark
            ' 
            txtRemark.Font = New Font("Tahoma", 10.5F)
            txtRemark.Location = New Point(240, 564)
            txtRemark.Multiline = True
            txtRemark.Name = "txtRemark"
            txtRemark.ScrollBars = ScrollBars.Vertical
            txtRemark.Size = New Size(520, 100)
            txtRemark.TabIndex = 9
            ' 
            ' txtReceipt
            ' 
            txtReceipt.BackColor = Color.White
            txtReceipt.Font = New Font("Tahoma", 10.5F)
            txtReceipt.Location = New Point(240, 676)
            txtReceipt.Name = "txtReceipt"
            txtReceipt.ReadOnly = True
            txtReceipt.Size = New Size(300, 33)
            txtReceipt.TabIndex = 10
            ' 
            ' btnBrowseReceipt
            ' 
            btnBrowseReceipt.BackColor = Color.FromArgb(CByte(79), CByte(70), CByte(229))
            btnBrowseReceipt.Cursor = Cursors.Hand
            btnBrowseReceipt.FlatStyle = FlatStyle.Flat
            btnBrowseReceipt.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnBrowseReceipt.ForeColor = Color.White
            btnBrowseReceipt.Location = New Point(550, 676)
            btnBrowseReceipt.Name = "btnBrowseReceipt"
            btnBrowseReceipt.Size = New Size(160, 40)
            btnBrowseReceipt.TabIndex = 11
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
            btnPasteReceipt.Location = New Point(716, 676)
            btnPasteReceipt.Name = "btnPasteReceipt"
            btnPasteReceipt.Size = New Size(166, 40)
            btnPasteReceipt.TabIndex = 12
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
            btnClearReceipt.Location = New Point(888, 676)
            btnClearReceipt.Name = "btnClearReceipt"
            btnClearReceipt.Size = New Size(50, 40)
            btnClearReceipt.TabIndex = 13
            btnClearReceipt.Text = "🗑️"
            btnClearReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnSave
            ' 
            btnSave.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSave.Cursor = Cursors.Hand
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            btnSave.ForeColor = Color.White
            btnSave.Location = New Point(240, 744)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(240, 56)
            btnSave.TabIndex = 5
            btnSave.Text = "💾 บันทึกรายการ"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' btnCancel
            ' 
            btnCancel.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnCancel.Cursor = Cursors.Hand
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            btnCancel.ForeColor = Color.White
            btnCancel.Location = New Point(500, 744)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(180, 56)
            btnCancel.TabIndex = 14
            btnCancel.Text = "❌ เคลียร์"
            btnCancel.UseVisualStyleBackColor = False
            ' 
            ' btnImportExcel
            ' 
            btnImportExcel.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnImportExcel.Cursor = Cursors.Hand
            btnImportExcel.FlatStyle = FlatStyle.Flat
            btnImportExcel.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            btnImportExcel.ForeColor = Color.White
            btnImportExcel.Location = New Point(750, 744)
            btnImportExcel.Name = "btnImportExcel"
            btnImportExcel.Size = New Size(240, 56)
            btnImportExcel.TabIndex = 15
            btnImportExcel.Text = "📥 นำเข้าจาก Excel"
            btnImportExcel.UseVisualStyleBackColor = False
            ' 
            ' cboReceiveChannel
            ' 
            cboReceiveChannel.DropDownStyle = ComboBoxStyle.DropDownList
            cboReceiveChannel.Font = New Font("Tahoma", 10.5F)
            cboReceiveChannel.Items.AddRange(New Object() {"เงินสด", "โอนธนาคาร", "พร้อมเพย์", "e-Donation"})
            cboReceiveChannel.Location = New Point(240, 378)
            cboReceiveChannel.Name = "cboReceiveChannel"
            cboReceiveChannel.Size = New Size(280, 33)
            cboReceiveChannel.TabIndex = 2
            ' 
            ' lblChannel
            ' 
            lblChannel.Location = New Point(40, 378)
            lblChannel.Name = "lblChannel"
            lblChannel.Size = New Size(190, 40)
            lblChannel.TabIndex = 22
            lblChannel.Text = "ช่องทางรับเงิน:"
            lblChannel.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtReferenceNo
            ' 
            txtReferenceNo.Font = New Font("Tahoma", 10.5F)
            txtReferenceNo.Location = New Point(670, 378)
            txtReferenceNo.Name = "txtReferenceNo"
            txtReferenceNo.Size = New Size(90, 33)
            txtReferenceNo.TabIndex = 8
            ' 
            ' lblRefNo
            ' 
            lblRefNo.Location = New Point(530, 378)
            lblRefNo.Name = "lblRefNo"
            lblRefNo.Size = New Size(140, 40)
            lblRefNo.TabIndex = 24
            lblRefNo.Text = "เลขอ้างอิง (ถ้ามี):"
            lblRefNo.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblRecentHeader
            ' 
            lblRecentHeader.BackColor = Color.FromArgb(CByte(253), CByte(224), CByte(71))
            lblRecentHeader.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            lblRecentHeader.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblRecentHeader.Location = New Point(770, 130)
            lblRecentHeader.Name = "lblRecentHeader"
            lblRecentHeader.Size = New Size(768, 40)
            lblRecentHeader.TabIndex = 25
            lblRecentHeader.Text = "📋 รายการรายรับล่าสุด 10 รายการ"
            lblRecentHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' dgvRecentTransactions
            ' 
            dgvRecentTransactions.AllowUserToAddRows = False
            dgvRecentTransactions.AllowUserToDeleteRows = False
            dgvRecentTransactions.AllowUserToResizeRows = False
            dgvRecentTransactions.BackgroundColor = Color.White
            dgvRecentTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvRecentTransactions.Location = New Point(770, 170)
            dgvRecentTransactions.MultiSelect = False
            dgvRecentTransactions.Name = "dgvRecentTransactions"
            dgvRecentTransactions.ReadOnly = True
            dgvRecentTransactions.RowHeadersVisible = False
            dgvRecentTransactions.RowHeadersWidth = 62
            dgvRecentTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvRecentTransactions.Size = New Size(857, 690)
            dgvRecentTransactions.TabIndex = 26
            ' 
            ' FrmIncome
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1773, 900)
            Controls.Add(lblReceiptCount)
            Controls.Add(lblMaxAmount)
            Controls.Add(lblAvgAmount)
            Controls.Add(lblTotalMonthlyIncome)
            Controls.Add(lbl1)
            Controls.Add(lbl2)
            Controls.Add(lbl3)
            Controls.Add(lbl4)
            Controls.Add(lblChannel)
            Controls.Add(lbl5)
            Controls.Add(lblRefNo)
            Controls.Add(lbl6)
            Controls.Add(lbl7)
            Controls.Add(lbl8)
            Controls.Add(dtpDate)
            Controls.Add(cboCategory)
            Controls.Add(cboFund)
            Controls.Add(cboBank)
            Controls.Add(cboReceiveChannel)
            Controls.Add(txtDescription)
            Controls.Add(txtReferenceNo)
            Controls.Add(txtAmount)
            Controls.Add(txtRemark)
            Controls.Add(txtReceipt)
            Controls.Add(btnBrowseReceipt)
            Controls.Add(btnPasteReceipt)
            Controls.Add(btnClearReceipt)
            Controls.Add(btnSave)
            Controls.Add(btnCancel)
            Controls.Add(btnImportExcel)
            Controls.Add(lblRecentHeader)
            Controls.Add(dgvRecentTransactions)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1100, 900)
            Name = "FrmIncome"
            StartPosition = FormStartPosition.CenterScreen
            Text = "บันทึกรายรับ"
            WindowState = FormWindowState.Maximized
            CType(dgvRecentTransactions, ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub
    End Class
End Namespace
