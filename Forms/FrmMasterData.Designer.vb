Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmMasterData
        Private components As IContainer = Nothing
        Friend WithEvents TabControl1 As TabControl
        Friend WithEvents tpCategory, tpFund, tpBank As TabPage
        Friend WithEvents lblHeader As Label
        Friend WithEvents dgvCategory, dgvFund, dgvBank As DataGridView
        Friend WithEvents txtCatName As TextBox, cboCatType As ComboBox
        Friend WithEvents btnCatAdd, btnCatEdit, btnCatDel As Button
        Friend WithEvents lblCatName, lblCatType As Label
        Friend WithEvents txtFundName As TextBox
        Friend WithEvents btnFundAdd, btnFundDel As Button
        Friend WithEvents lblFundName As Label
        Friend WithEvents txtBankName, txtBankAccountNo, txtBankAccountName As TextBox
        Friend WithEvents btnBankAdd, btnBankDel As Button
        Friend WithEvents lblBankName, lblBankAccountNo, lblBankAccountName As Label
        Friend WithEvents pCatTop As Panel
        Friend WithEvents pFundTop As Panel
        Friend WithEvents pBankTop As Panel

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
            Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
            lblHeader = New Label()
            TabControl1 = New TabControl()
            tpCategory = New TabPage()
            dgvCategory = New DataGridView()
            pCatTop = New Panel()
            lblCatName = New Label()
            txtCatName = New TextBox()
            lblCatType = New Label()
            cboCatType = New ComboBox()
            btnCatAdd = New Button()
            btnCatEdit = New Button()
            btnCatDel = New Button()
            tpFund = New TabPage()
            dgvFund = New DataGridView()
            pFundTop = New Panel()
            lblFundName = New Label()
            txtFundName = New TextBox()
            btnFundAdd = New Button()
            btnFundDel = New Button()
            tpBank = New TabPage()
            dgvBank = New DataGridView()
            pBankTop = New Panel()
            lblBankName = New Label()
            txtBankName = New TextBox()
            lblBankAccountNo = New Label()
            txtBankAccountNo = New TextBox()
            lblBankAccountName = New Label()
            txtBankAccountName = New TextBox()
            btnBankAdd = New Button()
            btnBankDel = New Button()
            TabControl1.SuspendLayout()
            tpCategory.SuspendLayout()
            CType(dgvCategory, ISupportInitialize).BeginInit()
            pCatTop.SuspendLayout()
            tpFund.SuspendLayout()
            CType(dgvFund, ISupportInitialize).BeginInit()
            pFundTop.SuspendLayout()
            tpBank.SuspendLayout()
            CType(dgvBank, ISupportInitialize).BeginInit()
            pBankTop.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 12.5F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1178, 42)
            lblHeader.TabIndex = 1
            lblHeader.Text = "🗂️ จัดการข้อมูลหลัก ประเภทรายการ / กองทุน / บัญชีธนาคาร"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' TabControl1
            ' 
            TabControl1.Controls.Add(tpCategory)
            TabControl1.Controls.Add(tpFund)
            TabControl1.Controls.Add(tpBank)
            TabControl1.Dock = DockStyle.Fill
            TabControl1.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            TabControl1.Location = New Point(0, 42)
            TabControl1.Name = "TabControl1"
            TabControl1.SelectedIndex = 0
            TabControl1.Size = New Size(1178, 630)
            TabControl1.TabIndex = 0
            ' 
            ' tpCategory
            ' 
            tpCategory.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpCategory.Controls.Add(dgvCategory)
            tpCategory.Controls.Add(pCatTop)
            tpCategory.Location = New Point(4, 33)
            tpCategory.Name = "tpCategory"
            tpCategory.Size = New Size(1170, 593)
            tpCategory.TabIndex = 0
            tpCategory.Text = "ประเภทรายการ (Category)"
            ' 
            ' dgvCategory
            ' 
            dgvCategory.AllowUserToAddRows = False
            DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvCategory.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
            dgvCategory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvCategory.BackgroundColor = Color.White
            dgvCategory.BorderStyle = BorderStyle.None
            dgvCategory.ColumnHeadersHeight = 34
            dgvCategory.Dock = DockStyle.Fill
            dgvCategory.Font = New Font("Tahoma", 10F)
            dgvCategory.Location = New Point(0, 90)
            dgvCategory.Name = "dgvCategory"
            dgvCategory.ReadOnly = True
            dgvCategory.RowHeadersWidth = 62
            dgvCategory.RowTemplate.Height = 32
            dgvCategory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvCategory.Size = New Size(1170, 503)
            dgvCategory.TabIndex = 1
            ' 
            ' pCatTop
            ' 
            pCatTop.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            pCatTop.Controls.Add(lblCatName)
            pCatTop.Controls.Add(txtCatName)
            pCatTop.Controls.Add(lblCatType)
            pCatTop.Controls.Add(cboCatType)
            pCatTop.Controls.Add(btnCatAdd)
            pCatTop.Controls.Add(btnCatEdit)
            pCatTop.Controls.Add(btnCatDel)
            pCatTop.Dock = DockStyle.Top
            pCatTop.Location = New Point(0, 0)
            pCatTop.Name = "pCatTop"
            pCatTop.Padding = New Padding(12, 10, 12, 10)
            pCatTop.Size = New Size(1170, 90)
            pCatTop.TabIndex = 0
            ' 
            ' lblCatName
            ' 
            lblCatName.AutoSize = True
            lblCatName.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblCatName.Location = New Point(12, 14)
            lblCatName.Name = "lblCatName"
            lblCatName.Size = New Size(116, 24)
            lblCatName.TabIndex = 0
            lblCatName.Text = "ชื่อประเภท:"
            ' 
            ' txtCatName
            ' 
            txtCatName.Font = New Font("Tahoma", 10.5F)
            txtCatName.Location = New Point(151, 11)
            txtCatName.Name = "txtCatName"
            txtCatName.Size = New Size(385, 33)
            txtCatName.TabIndex = 1
            ' 
            ' lblCatType
            ' 
            lblCatType.AutoSize = True
            lblCatType.Location = New Point(567, 14)
            lblCatType.Name = "lblCatType"
            lblCatType.Size = New Size(60, 24)
            lblCatType.TabIndex = 2
            lblCatType.Text = "ชนิด:"
            ' 
            ' cboCatType
            ' 
            cboCatType.DropDownStyle = ComboBoxStyle.DropDownList
            cboCatType.Font = New Font("Tahoma", 10F)
            cboCatType.Items.AddRange(New Object() {"Income (รายรับ)", "Expense (รายจ่าย)"})
            cboCatType.Location = New Point(660, 11)
            cboCatType.Name = "cboCatType"
            cboCatType.Size = New Size(200, 32)
            cboCatType.TabIndex = 3
            ' 
            ' btnCatAdd
            ' 
            btnCatAdd.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnCatAdd.Cursor = Cursors.Hand
            btnCatAdd.FlatStyle = FlatStyle.Flat
            btnCatAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnCatAdd.ForeColor = Color.White
            btnCatAdd.Location = New Point(12, 48)
            btnCatAdd.Name = "btnCatAdd"
            btnCatAdd.Size = New Size(130, 34)
            btnCatAdd.TabIndex = 4
            btnCatAdd.Text = "➕ เพิ่ม"
            btnCatAdd.UseVisualStyleBackColor = False
            ' 
            ' btnCatEdit
            ' 
            btnCatEdit.BackColor = Color.FromArgb(CByte(217), CByte(119), CByte(6))
            btnCatEdit.Cursor = Cursors.Hand
            btnCatEdit.FlatStyle = FlatStyle.Flat
            btnCatEdit.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnCatEdit.ForeColor = Color.White
            btnCatEdit.Location = New Point(148, 48)
            btnCatEdit.Name = "btnCatEdit"
            btnCatEdit.Size = New Size(130, 34)
            btnCatEdit.TabIndex = 5
            btnCatEdit.Text = "📝 แก้ไข"
            btnCatEdit.UseVisualStyleBackColor = False
            ' 
            ' btnCatDel
            ' 
            btnCatDel.BackColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            btnCatDel.Cursor = Cursors.Hand
            btnCatDel.FlatStyle = FlatStyle.Flat
            btnCatDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnCatDel.ForeColor = Color.White
            btnCatDel.Location = New Point(284, 48)
            btnCatDel.Name = "btnCatDel"
            btnCatDel.Size = New Size(150, 34)
            btnCatDel.TabIndex = 6
            btnCatDel.Text = "🗑️ ลบที่เลือก"
            btnCatDel.UseVisualStyleBackColor = False
            ' 
            ' tpFund
            ' 
            tpFund.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpFund.Controls.Add(dgvFund)
            tpFund.Controls.Add(pFundTop)
            tpFund.Location = New Point(4, 33)
            tpFund.Name = "tpFund"
            tpFund.Size = New Size(1170, 593)
            tpFund.TabIndex = 1
            tpFund.Text = "กองทุน (Funds)"
            ' 
            ' dgvFund
            ' 
            dgvFund.AllowUserToAddRows = False
            DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvFund.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle2
            dgvFund.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvFund.BackgroundColor = Color.White
            dgvFund.BorderStyle = BorderStyle.None
            dgvFund.ColumnHeadersHeight = 34
            dgvFund.Dock = DockStyle.Fill
            dgvFund.Font = New Font("Tahoma", 10F)
            dgvFund.Location = New Point(0, 78)
            dgvFund.Name = "dgvFund"
            dgvFund.ReadOnly = True
            dgvFund.RowHeadersWidth = 62
            dgvFund.RowTemplate.Height = 32
            dgvFund.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvFund.Size = New Size(1170, 515)
            dgvFund.TabIndex = 1
            ' 
            ' pFundTop
            ' 
            pFundTop.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            pFundTop.Controls.Add(lblFundName)
            pFundTop.Controls.Add(txtFundName)
            pFundTop.Controls.Add(btnFundAdd)
            pFundTop.Controls.Add(btnFundDel)
            pFundTop.Dock = DockStyle.Top
            pFundTop.Location = New Point(0, 0)
            pFundTop.Name = "pFundTop"
            pFundTop.Padding = New Padding(12, 10, 12, 10)
            pFundTop.Size = New Size(1170, 78)
            pFundTop.TabIndex = 0
            ' 
            ' lblFundName
            ' 
            lblFundName.AutoSize = True
            lblFundName.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblFundName.Location = New Point(12, 14)
            lblFundName.Name = "lblFundName"
            lblFundName.Size = New Size(106, 24)
            lblFundName.TabIndex = 0
            lblFundName.Text = "ชื่อกองทุน:"
            ' 
            ' txtFundName
            ' 
            txtFundName.Font = New Font("Tahoma", 10.5F)
            txtFundName.Location = New Point(151, 11)
            txtFundName.Name = "txtFundName"
            txtFundName.Size = New Size(385, 33)
            txtFundName.TabIndex = 1
            ' 
            ' btnFundAdd
            ' 
            btnFundAdd.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnFundAdd.Cursor = Cursors.Hand
            btnFundAdd.FlatStyle = FlatStyle.Flat
            btnFundAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnFundAdd.ForeColor = Color.White
            btnFundAdd.Location = New Point(12, 48)
            btnFundAdd.Name = "btnFundAdd"
            btnFundAdd.Size = New Size(130, 34)
            btnFundAdd.TabIndex = 2
            btnFundAdd.Text = "➕ เพิ่ม"
            btnFundAdd.UseVisualStyleBackColor = False
            ' 
            ' btnFundDel
            ' 
            btnFundDel.BackColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            btnFundDel.Cursor = Cursors.Hand
            btnFundDel.FlatStyle = FlatStyle.Flat
            btnFundDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnFundDel.ForeColor = Color.White
            btnFundDel.Location = New Point(148, 48)
            btnFundDel.Name = "btnFundDel"
            btnFundDel.Size = New Size(150, 34)
            btnFundDel.TabIndex = 3
            btnFundDel.Text = "🗑️ ลบที่เลือก"
            btnFundDel.UseVisualStyleBackColor = False
            ' 
            ' tpBank
            ' 
            tpBank.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpBank.Controls.Add(dgvBank)
            tpBank.Controls.Add(pBankTop)
            tpBank.Location = New Point(4, 33)
            tpBank.Name = "tpBank"
            tpBank.Size = New Size(1170, 593)
            tpBank.TabIndex = 2
            tpBank.Text = "บัญชีธนาคาร (Bank Accounts)"
            ' 
            ' dgvBank
            ' 
            dgvBank.AllowUserToAddRows = False
            DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvBank.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle3
            dgvBank.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvBank.BackgroundColor = Color.White
            dgvBank.BorderStyle = BorderStyle.None
            dgvBank.ColumnHeadersHeight = 34
            dgvBank.Dock = DockStyle.Fill
            dgvBank.Font = New Font("Tahoma", 10F)
            dgvBank.Location = New Point(0, 120)
            dgvBank.Name = "dgvBank"
            dgvBank.ReadOnly = True
            dgvBank.RowHeadersWidth = 62
            dgvBank.RowTemplate.Height = 32
            dgvBank.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvBank.Size = New Size(1170, 473)
            dgvBank.TabIndex = 1
            ' 
            ' pBankTop
            ' 
            pBankTop.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            pBankTop.Controls.Add(lblBankName)
            pBankTop.Controls.Add(txtBankName)
            pBankTop.Controls.Add(lblBankAccountNo)
            pBankTop.Controls.Add(txtBankAccountNo)
            pBankTop.Controls.Add(lblBankAccountName)
            pBankTop.Controls.Add(txtBankAccountName)
            pBankTop.Controls.Add(btnBankAdd)
            pBankTop.Controls.Add(btnBankDel)
            pBankTop.Dock = DockStyle.Top
            pBankTop.Location = New Point(0, 0)
            pBankTop.Name = "pBankTop"
            pBankTop.Padding = New Padding(12, 10, 12, 10)
            pBankTop.Size = New Size(1170, 120)
            pBankTop.TabIndex = 0
            ' 
            ' lblBankName
            ' 
            lblBankName.AutoSize = True
            lblBankName.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblBankName.Location = New Point(12, 14)
            lblBankName.Name = "lblBankName"
            lblBankName.Size = New Size(115, 24)
            lblBankName.TabIndex = 0
            lblBankName.Text = "ชื่อธนาคาร:"
            ' 
            ' txtBankName
            ' 
            txtBankName.Font = New Font("Tahoma", 10.5F)
            txtBankName.Location = New Point(151, 11)
            txtBankName.Name = "txtBankName"
            txtBankName.Size = New Size(300, 33)
            txtBankName.TabIndex = 1
            ' 
            ' lblBankAccountNo
            ' 
            lblBankAccountNo.AutoSize = True
            lblBankAccountNo.Location = New Point(470, 14)
            lblBankAccountNo.Name = "lblBankAccountNo"
            lblBankAccountNo.Size = New Size(100, 24)
            lblBankAccountNo.TabIndex = 2
            lblBankAccountNo.Text = "เลขบัญชี:"
            ' 
            ' txtBankAccountNo
            ' 
            txtBankAccountNo.Font = New Font("Tahoma", 10.5F)
            txtBankAccountNo.Location = New Point(580, 11)
            txtBankAccountNo.Name = "txtBankAccountNo"
            txtBankAccountNo.Size = New Size(250, 33)
            txtBankAccountNo.TabIndex = 3
            ' 
            ' lblBankAccountName
            ' 
            lblBankAccountName.AutoSize = True
            lblBankAccountName.Location = New Point(12, 53)
            lblBankAccountName.Name = "lblBankAccountName"
            lblBankAccountName.Size = New Size(92, 24)
            lblBankAccountName.TabIndex = 4
            lblBankAccountName.Text = "ชื่อบัญชี:"
            ' 
            ' txtBankAccountName
            ' 
            txtBankAccountName.Font = New Font("Tahoma", 10.5F)
            txtBankAccountName.Location = New Point(151, 50)
            txtBankAccountName.Name = "txtBankAccountName"
            txtBankAccountName.Size = New Size(300, 33)
            txtBankAccountName.TabIndex = 5
            ' 
            ' btnBankAdd
            ' 
            btnBankAdd.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnBankAdd.Cursor = Cursors.Hand
            btnBankAdd.FlatStyle = FlatStyle.Flat
            btnBankAdd.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnBankAdd.ForeColor = Color.White
            btnBankAdd.Location = New Point(12, 85)
            btnBankAdd.Name = "btnBankAdd"
            btnBankAdd.Size = New Size(130, 34)
            btnBankAdd.TabIndex = 6
            btnBankAdd.Text = "➕ เพิ่ม"
            btnBankAdd.UseVisualStyleBackColor = False
            ' 
            ' btnBankDel
            ' 
            btnBankDel.BackColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            btnBankDel.Cursor = Cursors.Hand
            btnBankDel.FlatStyle = FlatStyle.Flat
            btnBankDel.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            btnBankDel.ForeColor = Color.White
            btnBankDel.Location = New Point(148, 85)
            btnBankDel.Name = "btnBankDel"
            btnBankDel.Size = New Size(150, 34)
            btnBankDel.TabIndex = 7
            btnBankDel.Text = "🗑️ ลบที่เลือก"
            btnBankDel.UseVisualStyleBackColor = False
            ' 
            ' FrmMasterData
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(255), CByte(253), CByte(244))
            ClientSize = New Size(1178, 672)
            Controls.Add(TabControl1)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            FormBorderStyle = FormBorderStyle.Sizable
            MinimumSize = New Size(1000, 700)
            Name = "FrmMasterData"
            StartPosition = FormStartPosition.CenterScreen
            Text = "จัดการข้อมูลหลัก"
            WindowState = FormWindowState.Maximized
            TabControl1.ResumeLayout(False)
            tpCategory.ResumeLayout(False)
            CType(dgvCategory, ISupportInitialize).EndInit()
            pCatTop.ResumeLayout(False)
            pCatTop.PerformLayout()
            tpFund.ResumeLayout(False)
            CType(dgvFund, ISupportInitialize).EndInit()
            pFundTop.ResumeLayout(False)
            pFundTop.PerformLayout()
            tpBank.ResumeLayout(False)
            CType(dgvBank, ISupportInitialize).EndInit()
            pBankTop.ResumeLayout(False)
            pBankTop.PerformLayout()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
