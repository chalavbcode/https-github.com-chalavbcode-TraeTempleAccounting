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
        Friend WithEvents TabControl1 As TabControl
        Friend WithEvents tpCategory As TabPage
        Friend WithEvents tpFund As TabPage
        Friend WithEvents tpBank As TabPage
        Friend WithEvents dgvCategory As DataGridView
        Friend WithEvents dgvFund As DataGridView
        Friend WithEvents dgvBank As DataGridView
        Friend WithEvents txtCatName As TextBox
        Friend WithEvents cboCatType As ComboBox
        Friend WithEvents btnCatAdd As Button
        Friend WithEvents btnCatEdit As Button
        Friend WithEvents btnCatDel As Button
        Friend WithEvents txtFundName As TextBox
        Friend WithEvents btnFundAdd As Button
        Friend WithEvents btnFundDel As Button
        Friend WithEvents txtBankName As TextBox
        Friend WithEvents txtBankAccountNo As TextBox
        Friend WithEvents txtBankAccountName As TextBox
        Friend WithEvents btnBankAdd As Button
        Friend WithEvents btnBankDel As Button
        Friend WithEvents ttMain As ToolTip
        Friend WithEvents lblHeader As Label

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
            Me.ttMain = New ToolTip(Me.components)
            Me.TabControl1 = New TabControl()
            Me.tpCategory = New TabPage()
            Me.tpFund = New TabPage()
            Me.tpBank = New TabPage()
            Me.dgvCategory = New DataGridView()
            Me.dgvFund = New DataGridView()
            Me.dgvBank = New DataGridView()
            Me.txtCatName = New TextBox()
            Me.cboCatType = New ComboBox()
            Me.btnCatAdd = New Button()
            Me.btnCatEdit = New Button()
            Me.btnCatDel = New Button()
            Me.txtFundName = New TextBox()
            Me.btnFundAdd = New Button()
            Me.btnFundDel = New Button()
            Me.txtBankName = New TextBox()
            Me.txtBankAccountNo = New TextBox()
            Me.txtBankAccountName = New TextBox()
            Me.btnBankAdd = New Button()
            Me.btnBankDel = New Button()
            Me.lblHeader = New Label()

            Me.Text = "จัดการข้อมูลหลัก"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.ClientSize = New Size(1200, 750)
            Me.MinimumSize = New Size(1000, 650)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.AutoScroll = False

            ' Header
            lblHeader.Text = "⚙️ จัดการข้อมูลหลัก"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(12, 74, 110)
            lblHeader.BackColor = Color.FromArgb(186, 230, 253)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 50
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            ' TabControl
            TabControl1.Dock = DockStyle.Fill
            TabControl1.Controls.AddRange(New Control() {tpCategory, tpFund, tpBank})

            ' Tab: Category
            tpCategory.Text = "📋 ประเภทรายการ"
            tpCategory.BackColor = Color.White

            dgvCategory.Dock = DockStyle.Top
            dgvCategory.Height = 280
            dgvCategory.BackgroundColor = Color.White
            dgvCategory.BorderStyle = BorderStyle.None
            dgvCategory.RowHeadersVisible = False
            dgvCategory.AllowUserToAddRows = False
            dgvCategory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvCategory.MultiSelect = False
            dgvCategory.ReadOnly = True
            dgvCategory.Font = New Font("Tahoma", 10.0!)

            Dim yCat As Integer = 300
            Dim lblCatName As New Label() With {.Text = "ชื่อประเภท:", .Location = New Point(30, yCat), .AutoSize = True}
            txtCatName.Location = New Point(140, yCat - 3)
            txtCatName.Size = New Size(300, 30)
            txtCatName.Font = New Font("Tahoma", 10.5!)

            Dim lblCatType As New Label() With {.Text = "ชนิด:", .Location = New Point(460, yCat), .AutoSize = True}
            cboCatType.Location = New Point(530, yCat - 3)
            cboCatType.Size = New Size(200, 30)
            cboCatType.DropDownStyle = ComboBoxStyle.DropDownList
            cboCatType.Items.AddRange(New String() {"รายรับ (Income)", "รายจ่าย (Expense)"})
            cboCatType.SelectedIndex = 0

            btnCatAdd.Location = New Point(750, yCat - 5)
            btnCatAdd.Size = New Size(100, 35)
            btnCatAdd.Text = "➕ เพิ่ม"
            btnCatAdd.BackColor = Color.FromArgb(37, 99, 235)
            btnCatAdd.ForeColor = Color.White
            btnCatAdd.FlatStyle = FlatStyle.Flat
            btnCatAdd.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            btnCatEdit.Location = New Point(860, yCat - 5)
            btnCatEdit.Size = New Size(100, 35)
            btnCatEdit.Text = "✏️ แก้ไข"
            btnCatEdit.BackColor = Color.FromArgb(5, 150, 105)
            btnCatEdit.ForeColor = Color.White
            btnCatEdit.FlatStyle = FlatStyle.Flat
            btnCatEdit.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            btnCatDel.Location = New Point(970, yCat - 5)
            btnCatDel.Size = New Size(100, 35)
            btnCatDel.Text = "🗑️ ลบ"
            btnCatDel.BackColor = Color.FromArgb(185, 28, 28)
            btnCatDel.ForeColor = Color.White
            btnCatDel.FlatStyle = FlatStyle.Flat
            btnCatDel.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            tpCategory.Controls.AddRange(New Control() {dgvCategory, lblCatName, txtCatName, lblCatType, cboCatType, btnCatAdd, btnCatEdit, btnCatDel})

            ' Tab: Fund
            tpFund.Text = "💰 กองทุน"
            tpFund.BackColor = Color.White

            dgvFund.Dock = DockStyle.Top
            dgvFund.Height = 320
            dgvFund.BackgroundColor = Color.White
            dgvFund.BorderStyle = BorderStyle.None
            dgvFund.RowHeadersVisible = False
            dgvFund.AllowUserToAddRows = False
            dgvFund.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvFund.MultiSelect = False
            dgvFund.ReadOnly = True
            dgvFund.Font = New Font("Tahoma", 10.0!)

            Dim yFund As Integer = 340
            Dim lblFundName As New Label() With {.Text = "ชื่อกองทุน:", .Location = New Point(30, yFund), .AutoSize = True}
            txtFundName.Location = New Point(140, yFund - 3)
            txtFundName.Size = New Size(400, 30)
            txtFundName.Font = New Font("Tahoma", 10.5!)

            btnFundAdd.Location = New Point(560, yFund - 5)
            btnFundAdd.Size = New Size(100, 35)
            btnFundAdd.Text = "➕ เพิ่ม"
            btnFundAdd.BackColor = Color.FromArgb(37, 99, 235)
            btnFundAdd.ForeColor = Color.White
            btnFundAdd.FlatStyle = FlatStyle.Flat
            btnFundAdd.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            btnFundDel.Location = New Point(670, yFund - 5)
            btnFundDel.Size = New Size(100, 35)
            btnFundDel.Text = "🗑️ ลบ"
            btnFundDel.BackColor = Color.FromArgb(185, 28, 28)
            btnFundDel.ForeColor = Color.White
            btnFundDel.FlatStyle = FlatStyle.Flat
            btnFundDel.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            tpFund.Controls.AddRange(New Control() {dgvFund, lblFundName, txtFundName, btnFundAdd, btnFundDel})

            ' Tab: Bank
            tpBank.Text = "🏦 บัญชีธนาคาร"
            tpBank.BackColor = Color.White

            dgvBank.Dock = DockStyle.Top
            dgvBank.Height = 300
            dgvBank.BackgroundColor = Color.White
            dgvBank.BorderStyle = BorderStyle.None
            dgvBank.RowHeadersVisible = False
            dgvBank.AllowUserToAddRows = False
            dgvBank.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvBank.MultiSelect = False
            dgvBank.ReadOnly = True
            dgvBank.Font = New Font("Tahoma", 10.0!)

            Dim yBank As Integer = 320
            Dim lblBankName As New Label() With {.Text = "ชื่อธนาคาร:", .Location = New Point(30, yBank), .AutoSize = True}
            txtBankName.Location = New Point(140, yBank - 3)
            txtBankName.Size = New Size(250, 30)
            txtBankName.Font = New Font("Tahoma", 10.5!)

            Dim lblBankAcc As New Label() With {.Text = "เลขบัญชี:", .Location = New Point(410, yBank), .AutoSize = True}
            txtBankAccountNo.Location = New Point(510, yBank - 3)
            txtBankAccountNo.Size = New Size(200, 30)
            txtBankAccountNo.Font = New Font("Tahoma", 10.5!)

            Dim lblBankAccName As New Label() With {.Text = "ชื่อบัญชี:", .Location = New Point(730, yBank), .AutoSize = True}
            txtBankAccountName.Location = New Point(830, yBank - 3)
            txtBankAccountName.Size = New Size(250, 30)
            txtBankAccountName.Font = New Font("Tahoma", 10.5!)

            btnBankAdd.Location = New Point(1100, yBank - 5)
            btnBankAdd.Size = New Size(100, 35)
            btnBankAdd.Text = "➕ เพิ่ม"
            btnBankAdd.BackColor = Color.FromArgb(37, 99, 235)
            btnBankAdd.ForeColor = Color.White
            btnBankAdd.FlatStyle = FlatStyle.Flat
            btnBankAdd.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            btnBankDel.Location = New Point(30, yBank + 45)
            btnBankDel.Size = New Size(100, 35)
            btnBankDel.Text = "🗑️ ลบ"
            btnBankDel.BackColor = Color.FromArgb(185, 28, 28)
            btnBankDel.ForeColor = Color.White
            btnBankDel.FlatStyle = FlatStyle.Flat
            btnBankDel.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)

            tpBank.Controls.AddRange(New Control() {dgvBank, lblBankName, txtBankName, lblBankAcc, txtBankAccountNo, lblBankAccName, txtBankAccountName, btnBankAdd, btnBankDel})

            Me.Controls.AddRange(New Control() {lblHeader, TabControl1})
        End Sub
    End Class
End Namespace
