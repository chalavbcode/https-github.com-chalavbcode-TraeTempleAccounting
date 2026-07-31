Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmTransactions
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
        Friend WithEvents btnClose As Button
        Friend WithEvents ttMain As ToolTip

        <DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        <DebuggerStepThrough()>
        Private Sub InitializeComponent()
            components = New Container()
            Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
            ttMain = New ToolTip(components)
            lblHeader = New Label()
            pFilter = New Panel()
            lblCategory = New Label()
            cboCategory = New ComboBox()
            lblType = New Label()
            cboType = New ComboBox()
            lblDate = New Label()
            dtpFrom = New DateTimePicker()
            lblTo = New Label()
            dtpTo = New DateTimePicker()
            lblSearch = New Label()
            txtSearch = New TextBox()
            btnSearch = New Button()
            btnRefresh = New Button()
            lblSummary = New Label()
            dgvTransactions = New DataGridView()
            pActions = New Panel()
            btnClose = New Button()
            btnDelete = New Button()
            btnEdit = New Button()
            btnAddTrans = New Button()
            btnAddExp = New Button()
            btnAddInc = New Button()
            pFilter.SuspendLayout()
            CType(dgvTransactions, ISupportInitialize).BeginInit()
            pActions.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 15F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1400, 64)
            lblHeader.TabIndex = 4
            lblHeader.Text = "📋 รายการรับ-จ่ายทั้งหมด"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' pFilter
            ' 
            pFilter.BackColor = Color.White
            pFilter.Controls.Add(lblCategory)
            pFilter.Controls.Add(cboCategory)
            pFilter.Controls.Add(lblType)
            pFilter.Controls.Add(cboType)
            pFilter.Controls.Add(lblDate)
            pFilter.Controls.Add(dtpFrom)
            pFilter.Controls.Add(lblTo)
            pFilter.Controls.Add(dtpTo)
            pFilter.Controls.Add(lblSearch)
            pFilter.Controls.Add(txtSearch)
            pFilter.Controls.Add(btnSearch)
            pFilter.Controls.Add(btnRefresh)
            pFilter.Dock = DockStyle.Top
            pFilter.Location = New Point(0, 64)
            pFilter.Name = "pFilter"
            pFilter.Padding = New Padding(16)
            pFilter.Size = New Size(1400, 130)
            pFilter.TabIndex = 3
            ' 
            ' lblCategory
            ' 
            lblCategory.Font = New Font("Tahoma", 10F)
            lblCategory.Location = New Point(16, 16)
            lblCategory.Name = "lblCategory"
            lblCategory.Size = New Size(80, 28)
            lblCategory.TabIndex = 0
            lblCategory.Text = "ประเภท:"
            lblCategory.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboCategory
            ' 
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList
            cboCategory.Font = New Font("Tahoma", 10F)
            cboCategory.Location = New Point(118, 12)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(200, 32)
            cboCategory.TabIndex = 1
            ' 
            ' lblType
            ' 
            lblType.Font = New Font("Tahoma", 10F)
            lblType.Location = New Point(386, 12)
            lblType.Name = "lblType"
            lblType.Size = New Size(60, 28)
            lblType.TabIndex = 2
            lblType.Text = "ชนิด:"
            lblType.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboType
            ' 
            cboType.DropDownStyle = ComboBoxStyle.DropDownList
            cboType.Font = New Font("Tahoma", 10F)
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(485, 12)
            cboType.Name = "cboType"
            cboType.Size = New Size(180, 32)
            cboType.TabIndex = 3
            ' 
            ' lblDate
            ' 
            lblDate.Font = New Font("Tahoma", 10F)
            lblDate.Location = New Point(16, 54)
            lblDate.Name = "lblDate"
            lblDate.Size = New Size(80, 28)
            lblDate.TabIndex = 4
            lblDate.Text = "ตั้งแต่:"
            lblDate.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 10F)
            dtpFrom.Format = DateTimePickerFormat.Short
            dtpFrom.Location = New Point(118, 56)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(160, 32)
            dtpFrom.TabIndex = 5
            ' 
            ' lblTo
            ' 
            lblTo.Font = New Font("Tahoma", 10F)
            lblTo.Location = New Point(294, 56)
            lblTo.Name = "lblTo"
            lblTo.Size = New Size(40, 28)
            lblTo.TabIndex = 6
            lblTo.Text = "ถึง:"
            lblTo.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 10F)
            dtpTo.Format = DateTimePickerFormat.Short
            dtpTo.Location = New Point(362, 55)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(160, 32)
            dtpTo.TabIndex = 7
            ' 
            ' lblSearch
            ' 
            lblSearch.Font = New Font("Tahoma", 10F)
            lblSearch.Location = New Point(528, 56)
            lblSearch.Name = "lblSearch"
            lblSearch.Size = New Size(60, 28)
            lblSearch.TabIndex = 8
            lblSearch.Text = "ค้นหา:"
            lblSearch.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtSearch
            ' 
            txtSearch.Font = New Font("Tahoma", 10F)
            txtSearch.Location = New Point(594, 58)
            txtSearch.Name = "txtSearch"
            txtSearch.Size = New Size(258, 32)
            txtSearch.TabIndex = 9
            ' 
            ' btnSearch
            ' 
            btnSearch.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSearch.Cursor = Cursors.Hand
            btnSearch.FlatStyle = FlatStyle.Flat
            btnSearch.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSearch.ForeColor = Color.White
            btnSearch.Location = New Point(858, 56)
            btnSearch.Name = "btnSearch"
            btnSearch.Size = New Size(100, 36)
            btnSearch.TabIndex = 10
            btnSearch.Text = "🔍 ค้นหา"
            btnSearch.UseVisualStyleBackColor = False
            ' 
            ' btnRefresh
            ' 
            btnRefresh.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnRefresh.Cursor = Cursors.Hand
            btnRefresh.FlatStyle = FlatStyle.Flat
            btnRefresh.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(964, 58)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(100, 36)
            btnRefresh.TabIndex = 11
            btnRefresh.Text = "🔄 รีเฟรช"
            btnRefresh.UseVisualStyleBackColor = False
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(254), CByte(240), CByte(138))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 194)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1400, 48)
            lblSummary.TabIndex = 2
            lblSummary.Text = "รายรับ: 0.00 บาท   |   รายจ่าย: 0.00 บาท   |   คงเหลือ: 0.00 บาท   |   โอน: 0.00 บาท"
            lblSummary.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' dgvTransactions
            ' 
            dgvTransactions.AllowUserToAddRows = False
            dgvTransactions.AllowUserToDeleteRows = False
            DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle2
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvTransactions.BackgroundColor = Color.White
            dgvTransactions.BorderStyle = BorderStyle.None
            dgvTransactions.ColumnHeadersHeight = 34
            dgvTransactions.Dock = DockStyle.Fill
            dgvTransactions.EditMode = DataGridViewEditMode.EditOnEnter
            dgvTransactions.Font = New Font("Tahoma", 10F)
            dgvTransactions.Location = New Point(0, 242)
            dgvTransactions.Name = "dgvTransactions"
            dgvTransactions.ReadOnly = True
            dgvTransactions.RowHeadersWidth = 62
            dgvTransactions.RowTemplate.Height = 34
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvTransactions.Size = New Size(1400, 478)
            dgvTransactions.TabIndex = 0
            ' 
            ' pActions
            ' 
            pActions.BackColor = Color.FromArgb(CByte(245), CByte(240), CByte(220))
            pActions.Controls.Add(btnClose)
            pActions.Controls.Add(btnDelete)
            pActions.Controls.Add(btnEdit)
            pActions.Controls.Add(btnAddTrans)
            pActions.Controls.Add(btnAddExp)
            pActions.Controls.Add(btnAddInc)
            pActions.Dock = DockStyle.Bottom
            pActions.Location = New Point(0, 720)
            pActions.Name = "pActions"
            pActions.Padding = New Padding(16, 12, 16, 12)
            pActions.Size = New Size(1400, 80)
            pActions.TabIndex = 1
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.Cursor = Cursors.Hand
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1376, 12)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(80, 50)
            btnClose.TabIndex = 0
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
            btnDelete.Location = New Point(1268, 12)
            btnDelete.Name = "btnDelete"
            btnDelete.Size = New Size(100, 50)
            btnDelete.TabIndex = 1
            btnDelete.Text = "🗑️ ลบ"
            btnDelete.UseVisualStyleBackColor = False
            ' 
            ' btnEdit
            ' 
            btnEdit.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnEdit.Cursor = Cursors.Hand
            btnEdit.FlatStyle = FlatStyle.Flat
            btnEdit.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnEdit.ForeColor = Color.White
            btnEdit.Location = New Point(1140, 12)
            btnEdit.Name = "btnEdit"
            btnEdit.Size = New Size(120, 50)
            btnEdit.TabIndex = 2
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
            btnAddTrans.Location = New Point(352, 12)
            btnAddTrans.Name = "btnAddTrans"
            btnAddTrans.Size = New Size(140, 50)
            btnAddTrans.TabIndex = 3
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
            btnAddExp.Location = New Point(184, 12)
            btnAddExp.Name = "btnAddExp"
            btnAddExp.Size = New Size(160, 50)
            btnAddExp.TabIndex = 4
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
            btnAddInc.Location = New Point(16, 12)
            btnAddInc.Name = "btnAddInc"
            btnAddInc.Size = New Size(160, 50)
            btnAddInc.TabIndex = 5
            btnAddInc.Text = "💰 บันทึกรายรับ"
            btnAddInc.UseVisualStyleBackColor = False
            ' 
            ' FrmTransactions
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1400, 800)
            Controls.Add(dgvTransactions)
            Controls.Add(pActions)
            Controls.Add(lblSummary)
            Controls.Add(pFilter)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            FormBorderStyle = FormBorderStyle.Sizable
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
