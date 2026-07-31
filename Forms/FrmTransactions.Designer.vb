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
        Friend WithEvents tblFilter As TableLayoutPanel
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
            Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
            ttMain = New ToolTip(components)
            lblHeader = New Label()
            pFilter = New Panel()
            tblFilter = New TableLayoutPanel()
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
            tblFilter.SuspendLayout()
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
            pFilter.Controls.Add(tblFilter)
            pFilter.Dock = DockStyle.Top
            pFilter.Location = New Point(0, 64)
            pFilter.Name = "pFilter"
            pFilter.Padding = New Padding(10)
            pFilter.Size = New Size(1400, 130)
            pFilter.TabIndex = 3
            ' 
            ' tblFilter
            ' 
            tblFilter.ColumnCount = 6
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 106F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 209F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 119F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 237F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 137F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tblFilter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 183F))
            tblFilter.Controls.Add(lblCategory, 0, 0)
            tblFilter.Controls.Add(cboCategory, 1, 0)
            tblFilter.Controls.Add(cboType, 3, 0)
            tblFilter.Controls.Add(lblDate, 0, 1)
            tblFilter.Controls.Add(dtpFrom, 1, 1)
            tblFilter.Controls.Add(lblTo, 2, 1)
            tblFilter.Controls.Add(dtpTo, 3, 1)
            tblFilter.Controls.Add(txtSearch, 4, 0)
            tblFilter.Controls.Add(lblType, 2, 0)
            tblFilter.Controls.Add(btnRefresh, 4, 1)
            tblFilter.Controls.Add(btnSearch, 6, 1)
            tblFilter.Controls.Add(lblSearch, 5, 1)
            tblFilter.Dock = DockStyle.Fill
            tblFilter.GrowStyle = TableLayoutPanelGrowStyle.AddColumns
            tblFilter.Location = New Point(10, 10)
            tblFilter.Name = "tblFilter"
            tblFilter.Padding = New Padding(5)
            tblFilter.RowCount = 2
            tblFilter.RowStyles.Add(New RowStyle(SizeType.Absolute, 50F))
            tblFilter.RowStyles.Add(New RowStyle(SizeType.Absolute, 50F))
            tblFilter.Size = New Size(1380, 110)
            tblFilter.TabIndex = 0
            ' 
            ' lblCategory
            ' 
            lblCategory.Location = New Point(8, 5)
            lblCategory.Name = "lblCategory"
            lblCategory.Size = New Size(87, 36)
            lblCategory.TabIndex = 0
            lblCategory.Text = "ประเภท:"
            lblCategory.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboCategory
            ' 
            cboCategory.Dock = DockStyle.Fill
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList
            cboCategory.Location = New Point(114, 8)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(203, 33)
            cboCategory.TabIndex = 1
            ' 
            ' lblType
            ' 
            lblType.Location = New Point(323, 5)
            lblType.Name = "lblType"
            lblType.Size = New Size(60, 36)
            lblType.TabIndex = 2
            lblType.Text = "ชนิด:"
            lblType.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboType
            ' 
            cboType.Dock = DockStyle.Fill
            cboType.DropDownStyle = ComboBoxStyle.DropDownList
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.Location = New Point(442, 8)
            cboType.Name = "cboType"
            cboType.Size = New Size(231, 33)
            cboType.TabIndex = 3
            ' 
            ' lblDate
            ' 
            lblDate.Location = New Point(8, 55)
            lblDate.Name = "lblDate"
            lblDate.Size = New Size(71, 36)
            lblDate.TabIndex = 4
            lblDate.Text = "ตั้งแต่:"
            lblDate.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Dock = DockStyle.Fill
            dtpFrom.Format = DateTimePickerFormat.Short
            dtpFrom.Location = New Point(114, 58)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(203, 33)
            dtpFrom.TabIndex = 5
            ' 
            ' lblTo
            ' 
            lblTo.Location = New Point(323, 55)
            lblTo.Name = "lblTo"
            lblTo.Size = New Size(50, 36)
            lblTo.TabIndex = 6
            lblTo.Text = "ถึง:"
            lblTo.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' dtpTo
            ' 
            dtpTo.Dock = DockStyle.Fill
            dtpTo.Format = DateTimePickerFormat.Short
            dtpTo.Location = New Point(442, 58)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(231, 33)
            dtpTo.TabIndex = 7
            ' 
            ' lblSearch
            ' 
            lblSearch.Location = New Point(816, 55)
            lblSearch.Name = "lblSearch"
            lblSearch.Size = New Size(100, 36)
            lblSearch.TabIndex = 8
            lblSearch.Text = "ค้นหา:"
            lblSearch.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtSearch
            ' 
            tblFilter.SetColumnSpan(txtSearch, 2)
            txtSearch.Dock = DockStyle.Fill
            txtSearch.Location = New Point(679, 8)
            txtSearch.Name = "txtSearch"
            txtSearch.Size = New Size(510, 33)
            txtSearch.TabIndex = 9
            ' 
            ' btnSearch
            ' 
            btnSearch.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSearch.Dock = DockStyle.Fill
            btnSearch.FlatStyle = FlatStyle.Flat
            btnSearch.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnSearch.ForeColor = Color.White
            btnSearch.Location = New Point(1195, 58)
            btnSearch.Name = "btnSearch"
            btnSearch.Size = New Size(177, 44)
            btnSearch.TabIndex = 10
            btnSearch.Text = "🔍 ค้นหา"
            btnSearch.UseVisualStyleBackColor = False
            ' 
            ' btnRefresh
            ' 
            btnRefresh.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnRefresh.Dock = DockStyle.Fill
            btnRefresh.FlatStyle = FlatStyle.Flat
            btnRefresh.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnRefresh.ForeColor = Color.White
            btnRefresh.Location = New Point(679, 58)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(131, 44)
            btnRefresh.TabIndex = 11
            btnRefresh.Text = "� รีเฟรช"
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
            DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle5
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvTransactions.BackgroundColor = Color.White
            dgvTransactions.BorderStyle = BorderStyle.None
            dgvTransactions.ColumnHeadersHeight = 34
            dgvTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            dgvTransactions.Dock = DockStyle.Fill
            dgvTransactions.EditMode = DataGridViewEditMode.EditOnEnter
            dgvTransactions.Font = New Font("Tahoma", 10F)
            dgvTransactions.Location = New Point(0, 242)
            dgvTransactions.Name = "dgvTransactions"
            dgvTransactions.ReadOnly = True
            dgvTransactions.RowHeadersWidth = 62
            dgvTransactions.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
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
            MinimumSize = New Size(1180, 760)
            Name = "FrmTransactions"
            StartPosition = FormStartPosition.CenterScreen
            Text = "รายการรับ-จ่ายทั้งหมด"
            WindowState = FormWindowState.Maximized
            pFilter.ResumeLayout(False)
            tblFilter.ResumeLayout(False)
            tblFilter.PerformLayout()
            CType(dgvTransactions, ISupportInitialize).EndInit()
            pActions.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
