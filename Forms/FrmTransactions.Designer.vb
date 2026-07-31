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
            Me.components = New System.ComponentModel.Container()
            Me.ttMain = New System.Windows.Forms.ToolTip(Me.components)

            ' === Create all controls ===
            Me.lblHeader = New System.Windows.Forms.Label()
            Me.pFilter = New System.Windows.Forms.Panel()
            Me.lblCategory = New System.Windows.Forms.Label()
            Me.cboCategory = New System.Windows.Forms.ComboBox()
            Me.lblType = New System.Windows.Forms.Label()
            Me.cboType = New System.Windows.Forms.ComboBox()
            Me.lblDate = New System.Windows.Forms.Label()
            Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
            Me.lblTo = New System.Windows.Forms.Label()
            Me.dtpTo = New System.Windows.Forms.DateTimePicker()
            Me.lblSearch = New System.Windows.Forms.Label()
            Me.txtSearch = New System.Windows.Forms.TextBox()
            Me.btnSearch = New System.Windows.Forms.Button()
            Me.btnRefresh = New System.Windows.Forms.Button()
            Me.lblSummary = New System.Windows.Forms.Label()
            Me.dgvTransactions = New System.Windows.Forms.DataGridView()
            Me.pActions = New System.Windows.Forms.Panel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnAddTrans = New System.Windows.Forms.Button()
            Me.btnAddExp = New System.Windows.Forms.Button()
            Me.btnAddInc = New System.Windows.Forms.Button()

            ' === Suspend layouts ===
            Me.pFilter.SuspendLayout()
            CType(Me.dgvTransactions, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pActions.SuspendLayout()
            Me.SuspendLayout()

            ' 
            ' lblHeader
            ' 
            Me.lblHeader.BackColor = System.Drawing.Color.FromArgb(CType(253, Byte), CType(230, Byte), CType(138, Byte))
            Me.lblHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblHeader.Font = New System.Drawing.Font("Tahoma", 15.0!, System.Drawing.FontStyle.Bold)
            Me.lblHeader.ForeColor = System.Drawing.Color.FromArgb(CType(69, Byte), CType(26, Byte), CType(3, Byte))
            Me.lblHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New System.Drawing.Size(1400, 64)
            Me.lblHeader.TabIndex = 4
            Me.lblHeader.Text = "📋 รายการรับ-จ่ายทั้งหมด"
            Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter

            ' 
            ' pFilter
            ' 
            Me.pFilter.BackColor = System.Drawing.Color.White
            Me.pFilter.Controls.Add(Me.lblCategory)
            Me.pFilter.Controls.Add(Me.cboCategory)
            Me.pFilter.Controls.Add(Me.lblType)
            Me.pFilter.Controls.Add(Me.cboType)
            Me.pFilter.Controls.Add(Me.lblDate)
            Me.pFilter.Controls.Add(Me.dtpFrom)
            Me.pFilter.Controls.Add(Me.lblTo)
            Me.pFilter.Controls.Add(Me.dtpTo)
            Me.pFilter.Controls.Add(Me.lblSearch)
            Me.pFilter.Controls.Add(Me.txtSearch)
            Me.pFilter.Controls.Add(Me.btnSearch)
            Me.pFilter.Controls.Add(Me.btnRefresh)
            Me.pFilter.Dock = System.Windows.Forms.DockStyle.Top
            Me.pFilter.Location = New System.Drawing.Point(0, 64)
            Me.pFilter.Name = "pFilter"
            Me.pFilter.Padding = New System.Windows.Forms.Padding(16)
            Me.pFilter.Size = New System.Drawing.Size(1400, 130)
            Me.pFilter.TabIndex = 3

            ' 
            ' lblCategory
            ' 
            Me.lblCategory.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblCategory.Location = New System.Drawing.Point(16, 16)
            Me.lblCategory.Name = "lblCategory"
            Me.lblCategory.Size = New System.Drawing.Size(80, 28)
            Me.lblCategory.TabIndex = 0
            Me.lblCategory.Text = "ประเภท:"
            Me.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            ' 
            ' cboCategory
            ' 
            Me.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboCategory.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboCategory.Location = New System.Drawing.Point(118, 12)
            Me.cboCategory.Name = "cboCategory"
            Me.cboCategory.Size = New System.Drawing.Size(200, 32)
            Me.cboCategory.TabIndex = 1

            ' 
            ' lblType
            ' 
            Me.lblType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblType.Location = New System.Drawing.Point(386, 12)
            Me.lblType.Name = "lblType"
            Me.lblType.Size = New System.Drawing.Size(60, 28)
            Me.lblType.TabIndex = 2
            Me.lblType.Text = "ชนิด:"
            Me.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            ' 
            ' cboType
            ' 
            Me.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboType.Items.AddRange(New Object() {"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            Me.cboType.Location = New System.Drawing.Point(485, 12)
            Me.cboType.Name = "cboType"
            Me.cboType.Size = New System.Drawing.Size(180, 32)
            Me.cboType.TabIndex = 3

            ' 
            ' lblDate
            ' 
            Me.lblDate.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblDate.Location = New System.Drawing.Point(16, 54)
            Me.lblDate.Name = "lblDate"
            Me.lblDate.Size = New System.Drawing.Size(80, 28)
            Me.lblDate.TabIndex = 4
            Me.lblDate.Text = "ตั้งแต่:"
            Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            ' 
            ' dtpFrom
            ' 
            Me.dtpFrom.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpFrom.Location = New System.Drawing.Point(118, 56)
            Me.dtpFrom.Name = "dtpFrom"
            Me.dtpFrom.Size = New System.Drawing.Size(160, 32)
            Me.dtpFrom.TabIndex = 5

            ' 
            ' lblTo
            ' 
            Me.lblTo.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTo.Location = New System.Drawing.Point(294, 56)
            Me.lblTo.Name = "lblTo"
            Me.lblTo.Size = New System.Drawing.Size(40, 28)
            Me.lblTo.TabIndex = 6
            Me.lblTo.Text = "ถึง:"
            Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            ' 
            ' dtpTo
            ' 
            Me.dtpTo.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short
            Me.dtpTo.Location = New System.Drawing.Point(362, 55)
            Me.dtpTo.Name = "dtpTo"
            Me.dtpTo.Size = New System.Drawing.Size(160, 32)
            Me.dtpTo.TabIndex = 7

            ' 
            ' lblSearch
            ' 
            Me.lblSearch.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblSearch.Location = New System.Drawing.Point(528, 56)
            Me.lblSearch.Name = "lblSearch"
            Me.lblSearch.Size = New System.Drawing.Size(60, 28)
            Me.lblSearch.TabIndex = 8
            Me.lblSearch.Text = "ค้นหา:"
            Me.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight

            ' 
            ' txtSearch
            ' 
            Me.txtSearch.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtSearch.Location = New System.Drawing.Point(594, 58)
            Me.txtSearch.Name = "txtSearch"
            Me.txtSearch.Size = New System.Drawing.Size(258, 32)
            Me.txtSearch.TabIndex = 9

            ' 
            ' btnSearch
            ' 
            Me.btnSearch.BackColor = System.Drawing.Color.FromArgb(CType(37, Byte), CType(99, Byte), CType(235, Byte))
            Me.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSearch.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnSearch.ForeColor = System.Drawing.Color.White
            Me.btnSearch.Location = New System.Drawing.Point(858, 56)
            Me.btnSearch.Name = "btnSearch"
            Me.btnSearch.Size = New System.Drawing.Size(100, 36)
            Me.btnSearch.TabIndex = 10
            Me.btnSearch.Text = "🔍 ค้นหา"
            Me.btnSearch.UseVisualStyleBackColor = False

            ' 
            ' btnRefresh
            ' 
            Me.btnRefresh.BackColor = System.Drawing.Color.FromArgb(CType(5, Byte), CType(150, Byte), CType(105, Byte))
            Me.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnRefresh.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnRefresh.ForeColor = System.Drawing.Color.White
            Me.btnRefresh.Location = New System.Drawing.Point(964, 58)
            Me.btnRefresh.Name = "btnRefresh"
            Me.btnRefresh.Size = New System.Drawing.Size(100, 36)
            Me.btnRefresh.TabIndex = 11
            Me.btnRefresh.Text = "🔄 รีเฟรช"
            Me.btnRefresh.UseVisualStyleBackColor = False

            ' 
            ' lblSummary
            ' 
            Me.lblSummary.BackColor = System.Drawing.Color.FromArgb(CType(254, Byte), CType(240, Byte), CType(138, Byte))
            Me.lblSummary.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblSummary.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Bold)
            Me.lblSummary.ForeColor = System.Drawing.Color.FromArgb(CType(69, Byte), CType(26, Byte), CType(3, Byte))
            Me.lblSummary.Location = New System.Drawing.Point(0, 194)
            Me.lblSummary.Name = "lblSummary"
            Me.lblSummary.Size = New System.Drawing.Size(1400, 48)
            Me.lblSummary.TabIndex = 2
            Me.lblSummary.Text = "รายรับ: 0.00 บาท   |   รายจ่าย: 0.00 บาท   |   คงเหลือ: 0.00 บาท   |   โอน: 0.00 บาท"
            Me.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter

            ' 
            ' dgvTransactions
            ' 
            Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(255, Byte), CType(251, Byte), CType(235, Byte))
            Me.dgvTransactions.AllowUserToAddRows = False
            Me.dgvTransactions.AllowUserToDeleteRows = False
            Me.dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            Me.dgvTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvTransactions.BackgroundColor = System.Drawing.Color.White
            Me.dgvTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.dgvTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.dgvTransactions.ColumnHeadersHeight = 34
            Me.dgvTransactions.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvTransactions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
            Me.dgvTransactions.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.dgvTransactions.Location = New System.Drawing.Point(0, 242)
            Me.dgvTransactions.Name = "dgvTransactions"
            Me.dgvTransactions.ReadOnly = True
            Me.dgvTransactions.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
            Me.dgvTransactions.RowHeadersWidth = 62
            Me.dgvTransactions.RowTemplate.Height = 34
            Me.dgvTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvTransactions.Size = New System.Drawing.Size(1400, 478)
            Me.dgvTransactions.TabIndex = 0

            ' 
            ' pActions
            ' 
            Me.pActions.BackColor = System.Drawing.Color.FromArgb(CType(245, Byte), CType(240, Byte), CType(220, Byte))
            Me.pActions.Controls.Add(Me.btnClose)
            Me.pActions.Controls.Add(Me.btnDelete)
            Me.pActions.Controls.Add(Me.btnEdit)
            Me.pActions.Controls.Add(Me.btnAddTrans)
            Me.pActions.Controls.Add(Me.btnAddExp)
            Me.pActions.Controls.Add(Me.btnAddInc)
            Me.pActions.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pActions.Location = New System.Drawing.Point(0, 720)
            Me.pActions.Name = "pActions"
            Me.pActions.Padding = New System.Windows.Forms.Padding(16, 12, 16, 12)
            Me.pActions.Size = New System.Drawing.Size(1400, 80)
            Me.pActions.TabIndex = 1

            ' 
            ' btnClose
            ' 
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(75, Byte), CType(85, Byte), CType(99, Byte))
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(1376, 12)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(80, 50)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "ปิด"
            Me.btnClose.UseVisualStyleBackColor = False

            ' 
            ' btnDelete
            ' 
            Me.btnDelete.BackColor = System.Drawing.Color.FromArgb(CType(220, Byte), CType(38, Byte), CType(38, Byte))
            Me.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnDelete.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnDelete.ForeColor = System.Drawing.Color.White
            Me.btnDelete.Location = New System.Drawing.Point(1268, 12)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(100, 50)
            Me.btnDelete.TabIndex = 1
            Me.btnDelete.Text = "🗑️ ลบ"
            Me.btnDelete.UseVisualStyleBackColor = False

            ' 
            ' btnEdit
            ' 
            Me.btnEdit.BackColor = System.Drawing.Color.FromArgb(CType(37, Byte), CType(99, Byte), CType(235, Byte))
            Me.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnEdit.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnEdit.ForeColor = System.Drawing.Color.White
            Me.btnEdit.Location = New System.Drawing.Point(1140, 12)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(120, 50)
            Me.btnEdit.TabIndex = 2
            Me.btnEdit.Text = "✏️ แก้ไข"
            Me.btnEdit.UseVisualStyleBackColor = False

            ' 
            ' btnAddTrans
            ' 
            Me.btnAddTrans.BackColor = System.Drawing.Color.FromArgb(CType(126, Byte), CType(34, Byte), CType(206, Byte))
            Me.btnAddTrans.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAddTrans.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAddTrans.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnAddTrans.ForeColor = System.Drawing.Color.White
            Me.btnAddTrans.Location = New System.Drawing.Point(352, 12)
            Me.btnAddTrans.Name = "btnAddTrans"
            Me.btnAddTrans.Size = New System.Drawing.Size(140, 50)
            Me.btnAddTrans.TabIndex = 3
            Me.btnAddTrans.Text = "🔁 โอนเงิน"
            Me.btnAddTrans.UseVisualStyleBackColor = False

            ' 
            ' btnAddExp
            ' 
            Me.btnAddExp.BackColor = System.Drawing.Color.FromArgb(CType(180, Byte), CType(83, Byte), CType(9, Byte))
            Me.btnAddExp.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAddExp.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAddExp.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnAddExp.ForeColor = System.Drawing.Color.White
            Me.btnAddExp.Location = New System.Drawing.Point(184, 12)
            Me.btnAddExp.Name = "btnAddExp"
            Me.btnAddExp.Size = New System.Drawing.Size(160, 50)
            Me.btnAddExp.TabIndex = 4
            Me.btnAddExp.Text = "💸 บันทึกรายจ่าย"
            Me.btnAddExp.UseVisualStyleBackColor = False

            ' 
            ' btnAddInc
            ' 
            Me.btnAddInc.BackColor = System.Drawing.Color.FromArgb(CType(22, Byte), CType(163, Byte), CType(74, Byte))
            Me.btnAddInc.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnAddInc.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnAddInc.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnAddInc.ForeColor = System.Drawing.Color.White
            Me.btnAddInc.Location = New System.Drawing.Point(16, 12)
            Me.btnAddInc.Name = "btnAddInc"
            Me.btnAddInc.Size = New System.Drawing.Size(160, 50)
            Me.btnAddInc.TabIndex = 5
            Me.btnAddInc.Text = "💰 บันทึกรายรับ"
            Me.btnAddInc.UseVisualStyleBackColor = False

            ' 
            ' FrmTransactions
            ' 
            Me.AutoScroll = True
            Me.BackColor = System.Drawing.Color.FromArgb(CType(254, Byte), CType(249, Byte), CType(235, Byte))
            Me.ClientSize = New System.Drawing.Size(1400, 800)
            Me.Controls.Add(Me.dgvTransactions)
            Me.Controls.Add(Me.pActions)
            Me.Controls.Add(Me.lblSummary)
            Me.Controls.Add(Me.pFilter)
            Me.Controls.Add(Me.lblHeader)
            Me.Font = New System.Drawing.Font("Tahoma", 10.5!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
            Me.MinimumSize = New System.Drawing.Size(1180, 760)
            Me.Name = "FrmTransactions"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "รายการรับ-จ่ายทั้งหมด"
            Me.WindowState = System.Windows.Forms.FormWindowState.Maximized

            ' === Resume layouts ===
            Me.pFilter.ResumeLayout(False)
            Me.pFilter.PerformLayout()
            CType(Me.dgvTransactions, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pActions.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
