Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmMasterData
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents lblHeader As Label
        Friend WithEvents TabControl1 As TabControl
        Friend WithEvents tpCategory As TabPage
        Friend WithEvents tpFund As TabPage
        Friend WithEvents tpBank As TabPage
        
        ' Category
        Friend WithEvents dgvCategory As DataGridView
        Friend WithEvents pCatTop As Panel
        Friend WithEvents lblCatName As Label
        Friend WithEvents txtCatName As TextBox
        Friend WithEvents lblCatType As Label
        Friend WithEvents cboCatType As ComboBox
        Friend WithEvents btnCatAdd As Button
        Friend WithEvents btnCatEdit As Button
        Friend WithEvents btnCatDel As Button

        ' Fund
        Friend WithEvents dgvFund As DataGridView
        Friend WithEvents pFundTop As Panel
        Friend WithEvents lblFundName As Label
        Friend WithEvents txtFundName As TextBox
        Friend WithEvents btnFundAdd As Button
        Friend WithEvents btnFundDel As Button

        ' Bank
        Friend WithEvents dgvBank As DataGridView
        Friend WithEvents pBankTop As Panel
        Friend WithEvents lblBankName As Label
        Friend WithEvents txtBankName As TextBox
        Friend WithEvents lblBankAccountNo As Label
        Friend WithEvents txtBankAccountNo As TextBox
        Friend WithEvents lblBankAccountName As Label
        Friend WithEvents txtBankAccountName As TextBox
        Friend WithEvents btnBankAdd As Button
        Friend WithEvents btnBankDel As Button

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
            Me.components = New Container()
            Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
            
            Me.ttMain = New ToolTip(Me.components)
            Me.lblHeader = New Label()
            Me.TabControl1 = New TabControl()
            
            Me.tpCategory = New TabPage()
            Me.dgvCategory = New DataGridView()
            Me.pCatTop = New Panel()
            Me.lblCatName = New Label()
            Me.txtCatName = New TextBox()
            Me.lblCatType = New Label()
            Me.cboCatType = New ComboBox()
            Me.btnCatAdd = New Button()
            Me.btnCatEdit = New Button()
            Me.btnCatDel = New Button()
            
            Me.tpFund = New TabPage()
            Me.dgvFund = New DataGridView()
            Me.pFundTop = New Panel()
            Me.lblFundName = New Label()
            Me.txtFundName = New TextBox()
            Me.btnFundAdd = New Button()
            Me.btnFundDel = New Button()
            
            Me.tpBank = New TabPage()
            Me.dgvBank = New DataGridView()
            Me.pBankTop = New Panel()
            Me.lblBankName = New Label()
            Me.txtBankName = New TextBox()
            Me.lblBankAccountNo = New Label()
            Me.txtBankAccountNo = New TextBox()
            Me.lblBankAccountName = New Label()
            Me.txtBankAccountName = New TextBox()
            Me.btnBankAdd = New Button()
            Me.btnBankDel = New Button()

            Me.TabControl1.SuspendLayout()
            Me.tpCategory.SuspendLayout()
            CType(Me.dgvCategory, ISupportInitialize).BeginInit()
            Me.pCatTop.SuspendLayout()
            Me.tpFund.SuspendLayout()
            CType(Me.dgvFund, ISupportInitialize).BeginInit()
            Me.pFundTop.SuspendLayout()
            Me.tpBank.SuspendLayout()
            CType(Me.dgvBank, ISupportInitialize).BeginInit()
            Me.pBankTop.SuspendLayout()
            Me.SuspendLayout()

            ' FrmMasterData
            Me.Text = "จัดการข้อมูลหลัก"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.ClientSize = New Size(1178, 672)
            Me.FormBorderStyle = FormBorderStyle.None

            ' lblHeader
            Me.lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            Me.lblHeader.Dock = DockStyle.Top
            Me.lblHeader.Font = New Font("Tahoma", 12.5F, FontStyle.Bold)
            Me.lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblHeader.Location = New Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New Size(1178, 42)
            Me.lblHeader.TabIndex = 1
            Me.lblHeader.Text = "🗂️ จัดการข้อมูลหลัก ประเภทรายการ / กองทุน / บัญชีธนาคาร"
            Me.lblHeader.TextAlign = ContentAlignment.MiddleCenter

            ' TabControl1
            Me.TabControl1.Controls.Add(Me.tpCategory)
            Me.TabControl1.Controls.Add(Me.tpFund)
            Me.TabControl1.Controls.Add(Me.tpBank)
            Me.TabControl1.Dock = DockStyle.Fill
            Me.TabControl1.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            Me.TabControl1.Location = New Point(0, 42)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.Size = New Size(1178, 630)
            Me.TabControl1.TabIndex = 0

            ' tpCategory
            Me.tpCategory.BackColor = Color.FromArgb(254, 249, 235)
            Me.tpCategory.Controls.Add(Me.dgvCategory)
            Me.tpCategory.Controls.Add(Me.pCatTop)
            Me.tpCategory.Location = New Point(4, 33)
            Me.tpCategory.Name = "tpCategory"
            Me.tpCategory.Size = New Size(1170, 593)
            Me.tpCategory.TabIndex = 0
            Me.tpCategory.Text = "ประเภทรายการ (Category)"

            ' dgvCategory
            Me.dgvCategory.AllowUserToAddRows = False
            DataGridViewCellStyle1.BackColor = Color.FromArgb(255, 251, 235)
            Me.dgvCategory.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            Me.dgvCategory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvCategory.BackgroundColor = Color.White
            Me.dgvCategory.BorderStyle = BorderStyle.None
            Me.dgvCategory.ColumnHeadersHeight = 34
            Me.dgvCategory.Dock = DockStyle.Fill
            Me.dgvCategory.Font = New Font("Tahoma", 10F)
            Me.dgvCategory.Location = New Point(0, 90)
            Me.dgvCategory.Name = "dgvCategory"
            Me.dgvCategory.ReadOnly = True
            Me.dgvCategory.RowHeadersWidth = 62
            Me.dgvCategory.RowTemplate.Height = 32
            Me.dgvCategory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvCategory.Size = New Size(1170, 503)
            Me.dgvCategory.TabIndex = 1

            ' pCatTop
            Me.pCatTop.BackColor = Color.FromArgb(254, 249, 235)
            Me.pCatTop.Controls.Add(Me.lblCatName)
            Me.pCatTop.Controls.Add(Me.txtCatName)
            Me.pCatTop.Controls.Add(Me.lblCatType)
            Me.pCatTop.Controls.Add(Me.cboCatType)
            Me.pCatTop.Controls.Add(Me.btnCatAdd)
            Me.pCatTop.Controls.Add(Me.btnCatEdit)
            Me.pCatTop.Controls.Add(Me.btnCatDel)
            Me.pCatTop.Dock = DockStyle.Top
            Me.pCatTop.Location = New Point(0, 0)
            Me.pCatTop.Name = "pCatTop"
            Me.pCatTop.Padding = New Padding(12, 10, 12, 10)
            Me.pCatTop.Size = New Size(1170, 90)
            Me.pCatTop.TabIndex = 0

            ' lblCatName
            Me.lblCatName.AutoSize = True
            Me.lblCatName.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblCatName.Location = New Point(12, 14)
            Me.lblCatName.Name = "lblCatName"
            Me.lblCatName.Size = New Size(116, 24)
            Me.lblCatName.TabIndex = 0
            Me.lblCatName.Text = "ชื่อประเภท:"

            ' txtCatName
            Me.txtCatName.Font = New Font("Tahoma", 10.5F)
            Me.txtCatName.Location = New Point(134, 14)
            Me.txtCatName.Name = "txtCatName"
            Me.txtCatName.Size = New Size(385, 33)
            Me.txtCatName.TabIndex = 1

            ' lblCatType
            Me.lblCatType.AutoSize = True
            Me.lblCatType.Location = New Point(542, 14)
            Me.lblCatType.Name = "lblCatType"
            Me.lblCatType.Size = New Size(60, 24)
            Me.lblCatType.TabIndex = 2
            Me.lblCatType.Text = "ชนิด:"

            ' cboCatType
            Me.cboCatType.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cboCatType.Font = New Font("Tahoma", 10F)
            Me.cboCatType.Items.AddRange(New Object() {"Income (รายรับ)", "Expense (รายจ่าย)"})
            Me.cboCatType.Location = New Point(608, 11)
            Me.cboCatType.Name = "cboCatType"
            Me.cboCatType.Size = New Size(200, 32)
            Me.cboCatType.TabIndex = 3

            ' btnCatAdd
            Me.btnCatAdd.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnCatAdd.Cursor = Cursors.Hand
            Me.btnCatAdd.FlatStyle = FlatStyle.Flat
            Me.btnCatAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnCatAdd.ForeColor = Color.White
            Me.btnCatAdd.Location = New Point(12, 48)
            Me.btnCatAdd.Name = "btnCatAdd"
            Me.btnCatAdd.Size = New Size(130, 34)
            Me.btnCatAdd.TabIndex = 4
            Me.btnCatAdd.Text = "➕ เพิ่ม"
            Me.btnCatAdd.UseVisualStyleBackColor = False

            ' btnCatEdit
            Me.btnCatEdit.BackColor = Color.FromArgb(217, 119, 6)
            Me.btnCatEdit.Cursor = Cursors.Hand
            Me.btnCatEdit.FlatStyle = FlatStyle.Flat
            Me.btnCatEdit.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnCatEdit.ForeColor = Color.White
            Me.btnCatEdit.Location = New Point(148, 48)
            Me.btnCatEdit.Name = "btnCatEdit"
            Me.btnCatEdit.Size = New Size(130, 34)
            Me.btnCatEdit.TabIndex = 5
            Me.btnCatEdit.Text = "📝 แก้ไข"
            Me.btnCatEdit.UseVisualStyleBackColor = False

            ' btnCatDel
            Me.btnCatDel.BackColor = Color.FromArgb(153, 27, 27)
            Me.btnCatDel.Cursor = Cursors.Hand
            Me.btnCatDel.FlatStyle = FlatStyle.Flat
            Me.btnCatDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnCatDel.ForeColor = Color.White
            Me.btnCatDel.Location = New Point(284, 48)
            Me.btnCatDel.Name = "btnCatDel"
            Me.btnCatDel.Size = New Size(150, 34)
            Me.btnCatDel.TabIndex = 6
            Me.btnCatDel.Text = "🗑️ ลบที่เลือก"
            Me.btnCatDel.UseVisualStyleBackColor = False

            ' tpFund
            Me.tpFund.BackColor = Color.FromArgb(254, 249, 235)
            Me.tpFund.Controls.Add(Me.dgvFund)
            Me.tpFund.Controls.Add(Me.pFundTop)
            Me.tpFund.Location = New Point(4, 33)
            Me.tpFund.Name = "tpFund"
            Me.tpFund.Size = New Size(1170, 593)
            Me.tpFund.TabIndex = 1
            Me.tpFund.Text = "กองทุน (Funds)"

            ' dgvFund
            Me.dgvFund.AllowUserToAddRows = False
            DataGridViewCellStyle2.BackColor = Color.FromArgb(255, 251, 235)
            Me.dgvFund.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle2
            Me.dgvFund.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvFund.BackgroundColor = Color.White
            Me.dgvFund.BorderStyle = BorderStyle.None
            Me.dgvFund.ColumnHeadersHeight = 34
            Me.dgvFund.Dock = DockStyle.Fill
            Me.dgvFund.Font = New Font("Tahoma", 10F)
            Me.dgvFund.Location = New Point(0, 78)
            Me.dgvFund.Name = "dgvFund"
            Me.dgvFund.ReadOnly = True
            Me.dgvFund.RowHeadersWidth = 62
            Me.dgvFund.RowTemplate.Height = 32
            Me.dgvFund.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvFund.Size = New Size(1170, 515)
            Me.dgvFund.TabIndex = 1

            ' pFundTop
            Me.pFundTop.BackColor = Color.FromArgb(254, 249, 235)
            Me.pFundTop.Controls.Add(Me.lblFundName)
            Me.pFundTop.Controls.Add(Me.txtFundName)
            Me.pFundTop.Controls.Add(Me.btnFundAdd)
            Me.pFundTop.Controls.Add(Me.btnFundDel)
            Me.pFundTop.Dock = DockStyle.Top
            Me.pFundTop.Location = New Point(0, 0)
            Me.pFundTop.Name = "pFundTop"
            Me.pFundTop.Padding = New Padding(12, 10, 12, 10)
            Me.pFundTop.Size = New Size(1170, 78)
            Me.pFundTop.TabIndex = 0

            ' lblFundName
            Me.lblFundName.AutoSize = True
            Me.lblFundName.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblFundName.Location = New Point(12, 14)
            Me.lblFundName.Name = "lblFundName"
            Me.lblFundName.Size = New Size(112, 24)
            Me.lblFundName.TabIndex = 0
            Me.lblFundName.Text = "ชื่อกองทุน:"

            ' txtFundName
            Me.txtFundName.Font = New Font("Tahoma", 10.5F)
            Me.txtFundName.Location = New Point(110, 10)
            Me.txtFundName.Name = "txtFundName"
            Me.txtFundName.Size = New Size(520, 33)
            Me.txtFundName.TabIndex = 1

            ' btnFundAdd
            Me.btnFundAdd.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnFundAdd.Cursor = Cursors.Hand
            Me.btnFundAdd.FlatStyle = FlatStyle.Flat
            Me.btnFundAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnFundAdd.ForeColor = Color.White
            Me.btnFundAdd.Location = New Point(12, 44)
            Me.btnFundAdd.Name = "btnFundAdd"
            Me.btnFundAdd.Size = New Size(190, 30)
            Me.btnFundAdd.TabIndex = 2
            Me.btnFundAdd.Text = "➕ เพิ่ม/อัปเดต"
            Me.btnFundAdd.UseVisualStyleBackColor = False

            ' btnFundDel
            Me.btnFundDel.BackColor = Color.FromArgb(153, 27, 27)
            Me.btnFundDel.Cursor = Cursors.Hand
            Me.btnFundDel.FlatStyle = FlatStyle.Flat
            Me.btnFundDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnFundDel.ForeColor = Color.White
            Me.btnFundDel.Location = New Point(208, 44)
            Me.btnFundDel.Name = "btnFundDel"
            Me.btnFundDel.Size = New Size(160, 30)
            Me.btnFundDel.TabIndex = 3
            Me.btnFundDel.Text = "🗑️ ลบที่เลือก"
            Me.btnFundDel.UseVisualStyleBackColor = False

            ' tpBank
            Me.tpBank.BackColor = Color.FromArgb(254, 249, 235)
            Me.tpBank.Controls.Add(Me.dgvBank)
            Me.tpBank.Controls.Add(Me.pBankTop)
            Me.tpBank.Location = New Point(4, 33)
            Me.tpBank.Name = "tpBank"
            Me.tpBank.Size = New Size(1170, 593)
            Me.tpBank.TabIndex = 2
            Me.tpBank.Text = "บัญชีธนาคาร (Bank Accounts)"

            ' dgvBank
            Me.dgvBank.AllowUserToAddRows = False
            DataGridViewCellStyle3.BackColor = Color.FromArgb(255, 251, 235)
            Me.dgvBank.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle3
            Me.dgvBank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvBank.BackgroundColor = Color.White
            Me.dgvBank.BorderStyle = BorderStyle.None
            Me.dgvBank.ColumnHeadersHeight = 34
            Me.dgvBank.Dock = DockStyle.Fill
            Me.dgvBank.Font = New Font("Tahoma", 10F)
            Me.dgvBank.Location = New Point(0, 114)
            Me.dgvBank.Name = "dgvBank"
            Me.dgvBank.ReadOnly = True
            Me.dgvBank.RowHeadersWidth = 62
            Me.dgvBank.RowTemplate.Height = 32
            Me.dgvBank.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvBank.Size = New Size(1170, 479)
            Me.dgvBank.TabIndex = 1

            ' pBankTop
            Me.pBankTop.BackColor = Color.FromArgb(254, 249, 235)
            Me.pBankTop.Controls.Add(Me.lblBankName)
            Me.pBankTop.Controls.Add(Me.txtBankName)
            Me.pBankTop.Controls.Add(Me.lblBankAccountNo)
            Me.pBankTop.Controls.Add(Me.txtBankAccountNo)
            Me.pBankTop.Controls.Add(Me.lblBankAccountName)
            Me.pBankTop.Controls.Add(Me.txtBankAccountName)
            Me.pBankTop.Controls.Add(Me.btnBankAdd)
            Me.pBankTop.Controls.Add(Me.btnBankDel)
            Me.pBankTop.Dock = DockStyle.Top
            Me.pBankTop.Location = New Point(0, 0)
            Me.pBankTop.Name = "pBankTop"
            Me.pBankTop.Padding = New Padding(12, 10, 12, 10)
            Me.pBankTop.Size = New Size(1170, 114)
            Me.pBankTop.TabIndex = 0

            ' lblBankName
            Me.lblBankName.AutoSize = True
            Me.lblBankName.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblBankName.Location = New Point(12, 14)
            Me.lblBankName.Name = "lblBankName"
            Me.lblBankName.Size = New Size(118, 24)
            Me.lblBankName.TabIndex = 0
            Me.lblBankName.Text = "ชื่อธนาคาร:"

            ' txtBankName
            Me.txtBankName.Font = New Font("Tahoma", 10.5F)
            Me.txtBankName.Location = New Point(110, 10)
            Me.txtBankName.Name = "txtBankName"
            Me.txtBankName.Size = New Size(520, 33)
            Me.txtBankName.TabIndex = 1

            ' lblBankAccountNo
            Me.lblBankAccountNo.AutoSize = True
            Me.lblBankAccountNo.Location = New Point(12, 50)
            Me.lblBankAccountNo.Name = "lblBankAccountNo"
            Me.lblBankAccountNo.Size = New Size(114, 24)
            Me.lblBankAccountNo.TabIndex = 2
            Me.lblBankAccountNo.Text = "เลขที่บัญชี:"

            ' txtBankAccountNo
            Me.txtBankAccountNo.Font = New Font("Tahoma", 10.5F)
            Me.txtBankAccountNo.Location = New Point(110, 46)
            Me.txtBankAccountNo.Name = "txtBankAccountNo"
            Me.txtBankAccountNo.Size = New Size(220, 33)
            Me.txtBankAccountNo.TabIndex = 3

            ' lblBankAccountName
            Me.lblBankAccountName.AutoSize = True
            Me.lblBankAccountName.Location = New Point(344, 50)
            Me.lblBankAccountName.Name = "lblBankAccountName"
            Me.lblBankAccountName.Size = New Size(93, 24)
            Me.lblBankAccountName.TabIndex = 4
            Me.lblBankAccountName.Text = "ชื่อบัญชี:"

            ' txtBankAccountName
            Me.txtBankAccountName.Font = New Font("Tahoma", 10.5F)
            Me.txtBankAccountName.Location = New Point(412, 46)
            Me.txtBankAccountName.Name = "txtBankAccountName"
            Me.txtBankAccountName.Size = New Size(280, 33)
            Me.txtBankAccountName.TabIndex = 5

            ' btnBankAdd
            Me.btnBankAdd.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnBankAdd.Cursor = Cursors.Hand
            Me.btnBankAdd.FlatStyle = FlatStyle.Flat
            Me.btnBankAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnBankAdd.ForeColor = Color.White
            Me.btnBankAdd.Location = New Point(12, 80)
            Me.btnBankAdd.Name = "btnBankAdd"
            Me.btnBankAdd.Size = New Size(190, 30)
            Me.btnBankAdd.TabIndex = 6
            Me.btnBankAdd.Text = "➕ เพิ่ม/อัปเดต"
            Me.btnBankAdd.UseVisualStyleBackColor = False

            ' btnBankDel
            Me.btnBankDel.BackColor = Color.FromArgb(153, 27, 27)
            Me.btnBankDel.Cursor = Cursors.Hand
            Me.btnBankDel.FlatStyle = FlatStyle.Flat
            Me.btnBankDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            Me.btnBankDel.ForeColor = Color.White
            Me.btnBankDel.Location = New Point(208, 80)
            Me.btnBankDel.Name = "btnBankDel"
            Me.btnBankDel.Size = New Size(210, 30)
            Me.btnBankDel.TabIndex = 7
            Me.btnBankDel.Text = "🗑️ ลบบัญชีที่เลือก"
            Me.btnBankDel.UseVisualStyleBackColor = False

            Me.TabControl1.ResumeLayout(False)
            Me.tpCategory.ResumeLayout(False)
            CType(Me.dgvCategory, ISupportInitialize).EndInit()
            Me.pCatTop.ResumeLayout(False)
            Me.pCatTop.PerformLayout()
            Me.tpFund.ResumeLayout(False)
            CType(Me.dgvFund, ISupportInitialize).EndInit()
            Me.pFundTop.ResumeLayout(False)
            Me.pFundTop.PerformLayout()
            Me.tpBank.ResumeLayout(False)
            CType(Me.dgvBank, ISupportInitialize).EndInit()
            Me.pBankTop.ResumeLayout(False)
            Me.pBankTop.PerformLayout()
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
