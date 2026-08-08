Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmTransactions
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents lblHeader As Label
        Friend WithEvents pFilter As Panel
        Friend WithEvents lblCategory As Label
        Friend WithEvents cboCategory As ComboBox
        Friend WithEvents lblType As Label
        Friend WithEvents cboType As ComboBox
        Friend WithEvents lblDate As Label
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents lblTo As Label
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents lblSearch As Label
        Friend WithEvents txtSearch As TextBox
        Friend WithEvents btnSearch As Button
        Friend WithEvents btnRefresh As Button
        Friend WithEvents lblSummary As Label
        Friend WithEvents dgvTransactions As DataGridView
        Friend WithEvents pActions As Panel
        Friend WithEvents btnAddInc As Button
        Friend WithEvents btnAddExp As Button
        Friend WithEvents btnAddTrans As Button
        Friend WithEvents btnEdit As Button
        Friend WithEvents btnDelete As Button
        Friend WithEvents btnViewReceipt As Button
        Friend WithEvents btnPasteReceipt As Button
        Friend WithEvents btnBrowseReceipt As Button
        Friend WithEvents btnDeleteReceipt As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents ttMain As ToolTip

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
            pFilter = New Panel()
            btnRefresh = New Button()
            btnSearch = New Button()
            txtSearch = New TextBox()
            lblSearch = New Label()
            dtpTo = New DateTimePicker()
            lblTo = New Label()
            dtpFrom = New DateTimePicker()
            lblDate = New Label()
            cboType = New ComboBox()
            lblType = New Label()
            cboCategory = New ComboBox()
            lblCategory = New Label()
            lblSummary = New Label()
            dgvTransactions = New DataGridView()
            pActions = New Panel()
            btnDeleteReceipt = New Button()
            btnBrowseReceipt = New Button()
            btnPasteReceipt = New Button()
            btnClose = New Button()
            btnDelete = New Button()
            btnViewReceipt = New Button()
            btnEdit = New Button()
            btnAddTrans = New Button()
            btnAddExp = New Button()
            btnAddInc = New Button()
            pFilter.SuspendLayout()
            CType(dgvTransactions, ISupportInitialize).BeginInit()
            pActions.SuspendLayout()
            SuspendLayout()
            ' 
            ' pFilter
            ' 
            pFilter.BackColor = Color.White
            pFilter.Controls.Add(btnRefresh)
            pFilter.Controls.Add(btnSearch)
            pFilter.Controls.Add(txtSearch)
            pFilter.Controls.Add(lblSearch)
            pFilter.Controls.Add(dtpTo)
            pFilter.Controls.Add(lblTo)
            pFilter.Controls.Add(dtpFrom)
            pFilter.Controls.Add(lblDate)
            pFilter.Controls.Add(cboType)
            pFilter.Controls.Add(lblType)
            pFilter.Controls.Add(cboCategory)
            pFilter.Controls.Add(lblCategory)
            pFilter.Dock = DockStyle.Top
            pFilter.Location = New Point(0, 50)
            pFilter.Name = "pFilter"
            pFilter.Size = New Size(1250, 110)
            pFilter.TabIndex = 1
            ' 
            ' btnRefresh
            ' 
            btnRefresh.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnRefresh.FlatStyle = FlatStyle.Flat
            btnRefresh.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(1059, 21)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(120, 35)
            btnRefresh.TabIndex = 11
            btnRefresh.Text = "🔄 รีเฟรช"
            btnRefresh.UseVisualStyleBackColor = False
            ' 
            ' btnSearch
            ' 
            btnSearch.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSearch.FlatStyle = FlatStyle.Flat
            btnSearch.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSearch.ForeColor = Color.White
            btnSearch.Location = New Point(933, 20)
            btnSearch.Name = "btnSearch"
            btnSearch.Size = New Size(120, 35)
            btnSearch.TabIndex = 10
            btnSearch.Text = "🔍 ค้นหา"
            btnSearch.UseVisualStyleBackColor = False
            ' 
            ' txtSearch
            ' 
            txtSearch.Location = New Point(717, 22)
            txtSearch.Name = "txtSearch"
            txtSearch.Size = New Size(210, 33)
            txtSearch.TabIndex = 9
            ' 
            ' lblSearch
            ' 
            lblSearch.Location = New Point(641, 20)
            lblSearch.Name = "lblSearch"
            lblSearch.Size = New Size(70, 30)
            lblSearch.TabIndex = 8
            lblSearch.Text = "ค้นหา:"
            lblSearch.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpTo
            ' 
            dtpTo.Format = DateTimePickerFormat.Short
            dtpTo.Location = New Point(406, 65)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(160, 33)
            dtpTo.TabIndex = 7
            ' 
            ' lblTo
            ' 
            lblTo.Location = New Point(340, 65)
            lblTo.Name = "lblTo"
            lblTo.Size = New Size(40, 30)
            lblTo.TabIndex = 6
            lblTo.Text = "ถึง:"
            lblTo.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Format = DateTimePickerFormat.Short
            dtpFrom.Location = New Point(120, 65)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(160, 33)
            dtpFrom.TabIndex = 5
            ' 
            ' lblDate
            ' 
            lblDate.Location = New Point(20, 65)
            lblDate.Name = "lblDate"
            lblDate.Size = New Size(90, 30)
            lblDate.TabIndex = 4
            lblDate.Text = "ตั้งแต่:"
            lblDate.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboType
            ' 
            cboType.DropDownStyle = ComboBoxStyle.DropDownList
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(406, 23)
            cboType.Name = "cboType"
            cboType.Size = New Size(180, 33)
            cboType.TabIndex = 3
            ' 
            ' lblType
            ' 
            lblType.Location = New Point(340, 22)
            lblType.Name = "lblType"
            lblType.Size = New Size(60, 30)
            lblType.TabIndex = 2
            lblType.Text = "ชนิด:"
            lblType.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboCategory
            ' 
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList
            cboCategory.Location = New Point(120, 20)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(200, 33)
            cboCategory.TabIndex = 1
            ' 
            ' lblCategory
            ' 
            lblCategory.Location = New Point(20, 20)
            lblCategory.Name = "lblCategory"
            lblCategory.Size = New Size(90, 30)
            lblCategory.TabIndex = 0
            lblCategory.Text = "ประเภท:"
            lblCategory.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(254), CByte(240), CByte(138))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 0)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1250, 50)
            lblSummary.TabIndex = 0
            lblSummary.Text = "รายรับ: 0.00 บาท   |   รายจ่าย: 0.00 บาท   |   คงเหลือ: 0.00 บาท   |   โอน: 0.00 บาท"
            lblSummary.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' dgvTransactions
            ' 
            dgvTransactions.AllowUserToAddRows = False
            dgvTransactions.AllowUserToDeleteRows = False
            dgvTransactions.AllowUserToResizeColumns = True
            dgvTransactions.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgvTransactions.DefaultCellStyle.WrapMode = DataGridViewTriState.True
            DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            dgvTransactions.BackgroundColor = Color.White
            dgvTransactions.BorderStyle = BorderStyle.None
            dgvTransactions.ColumnHeadersHeight = 40
            dgvTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            dgvTransactions.Dock = DockStyle.Fill
            dgvTransactions.EditMode = DataGridViewEditMode.EditOnEnter
            dgvTransactions.Font = New Font("Tahoma", 10F)
            dgvTransactions.Location = New Point(0, 160)
            dgvTransactions.Name = "dgvTransactions"
            dgvTransactions.ReadOnly = True
            dgvTransactions.RowHeadersWidth = 50
            dgvTransactions.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
            dgvTransactions.RowTemplate.Height = 34
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvTransactions.Size = New Size(1250, 500)
            dgvTransactions.TabIndex = 3
            ' 
            ' pActions
            ' 
            pActions.BackColor = Color.FromArgb(CByte(245), CByte(240), CByte(220))
            pActions.Controls.Add(btnDeleteReceipt)
            pActions.Controls.Add(btnBrowseReceipt)
            pActions.Controls.Add(btnPasteReceipt)
            pActions.Controls.Add(btnClose)
            pActions.Controls.Add(btnDelete)
            pActions.Controls.Add(btnViewReceipt)
            pActions.Controls.Add(btnEdit)
            pActions.Controls.Add(btnAddTrans)
            pActions.Controls.Add(btnAddExp)
            pActions.Controls.Add(btnAddInc)
            pActions.Dock = DockStyle.Bottom
            pActions.Location = New Point(0, 660)
            pActions.Name = "pActions"
            pActions.Padding = New Padding(16, 12, 16, 12)
            pActions.Size = New Size(1250, 140)
            pActions.TabIndex = 2
            ' 
            ' btnDeleteReceipt
            ' 
            btnDeleteReceipt.BackColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            btnDeleteReceipt.Cursor = Cursors.Hand
            btnDeleteReceipt.FlatStyle = FlatStyle.Flat
            btnDeleteReceipt.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnDeleteReceipt.ForeColor = Color.White
            btnDeleteReceipt.Location = New Point(661, 75)
            btnDeleteReceipt.Name = "btnDeleteReceipt"
            btnDeleteReceipt.Size = New Size(185, 45)
            btnDeleteReceipt.TabIndex = 12
            btnDeleteReceipt.Text = "❌ ลบรูปใบเสร็จ"
            btnDeleteReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnBrowseReceipt
            ' 
            btnBrowseReceipt.BackColor = Color.FromArgb(CByte(79), CByte(70), CByte(229))
            btnBrowseReceipt.Cursor = Cursors.Hand
            btnBrowseReceipt.FlatStyle = FlatStyle.Flat
            btnBrowseReceipt.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnBrowseReceipt.ForeColor = Color.White
            btnBrowseReceipt.Location = New Point(447, 75)
            btnBrowseReceipt.Name = "btnBrowseReceipt"
            btnBrowseReceipt.Size = New Size(208, 45)
            btnBrowseReceipt.TabIndex = 11
            btnBrowseReceipt.Text = "📂 เลือกรูปย้อนหลัง"
            btnBrowseReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnPasteReceipt
            ' 
            btnPasteReceipt.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnPasteReceipt.Cursor = Cursors.Hand
            btnPasteReceipt.FlatStyle = FlatStyle.Flat
            btnPasteReceipt.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnPasteReceipt.ForeColor = Color.White
            btnPasteReceipt.Location = New Point(240, 75)
            btnPasteReceipt.Name = "btnPasteReceipt"
            btnPasteReceipt.Size = New Size(200, 45)
            btnPasteReceipt.TabIndex = 10
            btnPasteReceipt.Text = "📋 วางรูปย้อนหลัง"
            btnPasteReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.Cursor = Cursors.Hand
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1098, 18)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(120, 45)
            btnClose.TabIndex = 9
            btnClose.Text = "ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' btnDelete
            ' 
            btnDelete.BackColor = Color.FromArgb(CByte(220), CByte(38), CByte(38))
            btnDelete.Cursor = Cursors.Hand
            btnDelete.FlatStyle = FlatStyle.Flat
            btnDelete.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnDelete.ForeColor = Color.White
            btnDelete.Location = New Point(771, 18)
            btnDelete.Name = "btnDelete"
            btnDelete.Size = New Size(120, 45)
            btnDelete.TabIndex = 8
            btnDelete.Text = "🗑️ ลบ"
            btnDelete.UseVisualStyleBackColor = False
            ' 
            ' btnViewReceipt
            ' 
            btnViewReceipt.BackColor = Color.FromArgb(CByte(245), CByte(158), CByte(11))
            btnViewReceipt.Cursor = Cursors.Hand
            btnViewReceipt.FlatStyle = FlatStyle.Flat
            btnViewReceipt.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnViewReceipt.ForeColor = Color.White
            btnViewReceipt.Location = New Point(33, 75)
            btnViewReceipt.Name = "btnViewReceipt"
            btnViewReceipt.Size = New Size(200, 45)
            btnViewReceipt.TabIndex = 3
            btnViewReceipt.Text = "🔍 ดูใบเสร็จ"
            btnViewReceipt.UseVisualStyleBackColor = False
            ' 
            ' btnEdit
            ' 
            btnEdit.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnEdit.Cursor = Cursors.Hand
            btnEdit.FlatStyle = FlatStyle.Flat
            btnEdit.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnEdit.ForeColor = Color.White
            btnEdit.Location = New Point(614, 18)
            btnEdit.Name = "btnEdit"
            btnEdit.Size = New Size(150, 45)
            btnEdit.TabIndex = 7
            btnEdit.Text = "✏️ แก้ไข"
            btnEdit.UseVisualStyleBackColor = False
            ' 
            ' btnAddTrans
            ' 
            btnAddTrans.BackColor = Color.FromArgb(CByte(126), CByte(34), CByte(206))
            btnAddTrans.Cursor = Cursors.Hand
            btnAddTrans.FlatStyle = FlatStyle.Flat
            btnAddTrans.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnAddTrans.ForeColor = Color.White
            btnAddTrans.Location = New Point(447, 18)
            btnAddTrans.Name = "btnAddTrans"
            btnAddTrans.Size = New Size(160, 45)
            btnAddTrans.TabIndex = 4
            btnAddTrans.Text = "🔁 โอนเงิน"
            btnAddTrans.UseVisualStyleBackColor = False
            ' 
            ' btnAddExp
            ' 
            btnAddExp.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnAddExp.Cursor = Cursors.Hand
            btnAddExp.FlatStyle = FlatStyle.Flat
            btnAddExp.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnAddExp.ForeColor = Color.White
            btnAddExp.Location = New Point(240, 18)
            btnAddExp.Name = "btnAddExp"
            btnAddExp.Size = New Size(200, 45)
            btnAddExp.TabIndex = 5
            btnAddExp.Text = "💸 บันทึกรายจ่าย"
            btnAddExp.UseVisualStyleBackColor = False
            ' 
            ' btnAddInc
            ' 
            btnAddInc.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnAddInc.Cursor = Cursors.Hand
            btnAddInc.FlatStyle = FlatStyle.Flat
            btnAddInc.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnAddInc.ForeColor = Color.White
            btnAddInc.Location = New Point(33, 18)
            btnAddInc.Name = "btnAddInc"
            btnAddInc.Size = New Size(200, 45)
            btnAddInc.TabIndex = 6
            btnAddInc.Text = "💰 บันทึกรายรับ"
            btnAddInc.UseVisualStyleBackColor = False
            ' 
            ' FrmTransactions
            ' 
            AutoScaleDimensions = New SizeF(12F, 25F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1250, 800)
            Controls.Add(dgvTransactions)
            Controls.Add(pActions)
            Controls.Add(pFilter)
            Controls.Add(lblSummary)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1180, 760)
            Name = "FrmTransactions"
            StartPosition = FormStartPosition.CenterScreen
            Text = "รายการรับ-จ่ายทั้งหมด"
            WindowState = FormWindowState.Maximized
            pFilter.ResumeLayout(False)
            pFilter.PerformLayout()
            CType(dgvTransactions, ISupportInitialize).EndInit()
            pActions.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
