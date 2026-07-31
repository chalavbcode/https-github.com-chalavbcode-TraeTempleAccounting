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

        Friend WithEvents lblCategory As Label
        Friend WithEvents lblSearch As Label
        Friend WithEvents lblType As Label
        Friend WithEvents lblDate As Label
        Friend WithEvents cboCategory As ComboBox
        Friend WithEvents cboType As ComboBox
        Friend WithEvents txtSearch As TextBox
        Friend WithEvents btnSearch As Button
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents dgvTransactions As DataGridView
        Friend WithEvents btnAddInc As Button
        Friend WithEvents btnAddExp As Button
        Friend WithEvents btnAddTrans As Button
        Friend WithEvents btnEdit As Button
        Friend WithEvents btnDelete As Button
        Friend WithEvents btnRefresh As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents lblSummary As Label
        Friend WithEvents lblHeader As Label
        Friend WithEvents pFilter As Panel
        Friend WithEvents pActions As Panel
        Friend WithEvents ttMain As ToolTip

        <DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        <DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New Container()
            Me.ttMain = New ToolTip(Me.components)
            Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
            lblHeader = New Label()
            pFilter = New Panel()
            lblCategory = New Label()
            cboCategory = New ComboBox()
            lblType = New Label()
            cboType = New ComboBox()
            lblDate = New Label()
            dtpFrom = New DateTimePicker()
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
            lblHeader.Size = New Size(1648, 64)
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
            pFilter.Controls.Add(dtpTo)
            pFilter.Controls.Add(lblSearch)
            pFilter.Controls.Add(txtSearch)
            pFilter.Controls.Add(btnSearch)
            pFilter.Controls.Add(btnRefresh)
            pFilter.Dock = DockStyle.Top
            pFilter.Location = New Point(0, 64)
            pFilter.Name = "pFilter"
            pFilter.Padding = New Padding(16)
            pFilter.Size = New Size(1648, 130)
            pFilter.TabIndex = 3
            ' 
            ' lblCategory
            ' 
            lblCategory.Location = New Point(0, 0)
            lblCategory.Name = "lblCategory"
            lblCategory.Size = New Size(100, 23)
            lblCategory.TabIndex = 0
            ' 
            ' cboCategory
            ' 
            cboCategory.Location = New Point(0, 0)
            cboCategory.Name = "cboCategory"
            cboCategory.Size = New Size(121, 33)
            cboCategory.TabIndex = 1
            ' 
            ' lblType
            ' 
            lblType.Location = New Point(0, 0)
            lblType.Name = "lblType"
            lblType.Size = New Size(100, 23)
            lblType.TabIndex = 2
            ' 
            ' cboType
            ' 
            cboType.Items.AddRange(New Object() {"ทั้งหมด", "Income รายรับ", "Expense รายจ่าย", "Transfer โอนภายใน"})
            cboType.Location = New Point(0, 0)
            cboType.Name = "cboType"
            cboType.Size = New Size(121, 33)
            cboType.TabIndex = 3
            ' 
            ' lblDate
            ' 
            lblDate.Location = New Point(0, 0)
            lblDate.Name = "lblDate"
            lblDate.Size = New Size(100, 23)
            lblDate.TabIndex = 4
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Location = New Point(0, 0)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(200, 33)
            dtpFrom.TabIndex = 5
            ' 
            ' dtpTo
            ' 
            dtpTo.Location = New Point(0, 0)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(200, 33)
            dtpTo.TabIndex = 6
            ' 
            ' lblSearch
            ' 
            lblSearch.Location = New Point(0, 0)
            lblSearch.Name = "lblSearch"
            lblSearch.Size = New Size(100, 23)
            lblSearch.TabIndex = 7
            ' 
            ' txtSearch
            ' 
            txtSearch.Location = New Point(0, 0)
            txtSearch.Name = "txtSearch"
            txtSearch.Size = New Size(100, 33)
            txtSearch.TabIndex = 8
            ' 
            ' btnSearch
            ' 
            btnSearch.Location = New Point(0, 0)
            btnSearch.Name = "btnSearch"
            btnSearch.Size = New Size(75, 23)
            btnSearch.TabIndex = 9
            ' 
            ' btnRefresh
            ' 
            btnRefresh.Location = New Point(0, 0)
            btnRefresh.Name = "btnRefresh"
            btnRefresh.Size = New Size(75, 23)
            btnRefresh.TabIndex = 10
            ' 
            ' lblSummary
            ' 
            lblSummary.BackColor = Color.FromArgb(CByte(254), CByte(240), CByte(138))
            lblSummary.Dock = DockStyle.Top
            lblSummary.Font = New Font("Tahoma", 11F, FontStyle.Bold)
            lblSummary.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblSummary.Location = New Point(0, 194)
            lblSummary.Name = "lblSummary"
            lblSummary.Size = New Size(1648, 52)
            lblSummary.TabIndex = 2
            lblSummary.Text = "รายรับเดือนนี้: 0.00 บาท   |   รายจ่าย: 0.00 บาท   |   คงเหลือ: 0.00 บาท   |   โอนภายใน: 0.00 บาท"
            lblSummary.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' dgvTransactions
            ' 
            dgvTransactions.AllowUserToAddRows = False
            dgvTransactions.AllowUserToDeleteRows = False
            DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvTransactions.BackgroundColor = Color.White
            dgvTransactions.BorderStyle = BorderStyle.None
            dgvTransactions.ColumnHeadersHeight = 34
            dgvTransactions.Dock = DockStyle.Fill
            dgvTransactions.EditMode = DataGridViewEditMode.EditOnEnter
            dgvTransactions.Font = New Font("Tahoma", 10F)
            dgvTransactions.Location = New Point(0, 246)
            dgvTransactions.Name = "dgvTransactions"
            dgvTransactions.ReadOnly = True
            dgvTransactions.RowHeadersWidth = 62
            dgvTransactions.RowTemplate.Height = 34
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvTransactions.Size = New Size(1648, 397)
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
            pActions.Location = New Point(0, 643)
            pActions.Name = "pActions"
            pActions.Padding = New Padding(14)
            pActions.Size = New Size(1648, 80)
            pActions.TabIndex = 1
            ' 
            ' btnClose
            ' 
            btnClose.Location = New Point(0, 0)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(75, 23)
            btnClose.TabIndex = 0
            ' 
            ' btnDelete
            ' 
            btnDelete.Location = New Point(0, 0)
            btnDelete.Name = "btnDelete"
            btnDelete.Size = New Size(75, 23)
            btnDelete.TabIndex = 1
            ' 
            ' btnEdit
            ' 
            btnEdit.Location = New Point(0, 0)
            btnEdit.Name = "btnEdit"
            btnEdit.Size = New Size(75, 23)
            btnEdit.TabIndex = 2
            ' 
            ' btnAddTrans
            ' 
            btnAddTrans.Location = New Point(0, 0)
            btnAddTrans.Name = "btnAddTrans"
            btnAddTrans.Size = New Size(75, 23)
            btnAddTrans.TabIndex = 3
            ' 
            ' btnAddExp
            ' 
            btnAddExp.Location = New Point(0, 0)
            btnAddExp.Name = "btnAddExp"
            btnAddExp.Size = New Size(75, 23)
            btnAddExp.TabIndex = 4
            ' 
            ' btnAddInc
            ' 
            btnAddInc.Location = New Point(0, 0)
            btnAddInc.Name = "btnAddInc"
            btnAddInc.Size = New Size(75, 23)
            btnAddInc.TabIndex = 5
            ' 
            ' FrmTransactions
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1648, 723)
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
            Text = "รายการทั้งหมด"
            WindowState = FormWindowState.Maximized
            pFilter.ResumeLayout(False)
            pFilter.PerformLayout()
            CType(dgvTransactions, ISupportInitialize).EndInit()
            pActions.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
