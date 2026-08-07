Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmTempleSetting
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents lblHeader As Label
        Friend WithEvents pMainContainer As Panel
        Friend WithEvents pHeader As Panel
        Friend WithEvents pContent As Panel
        Friend WithEvents pBottom As Panel
        ' Labels
        Friend WithEvents lblTempleCode As Label
        Friend WithEvents lblTempleName As Label
        Friend WithEvents lblTempleAddress As Label
        Friend WithEvents lblTambon As Label
        Friend WithEvents lblAmphoe As Label
        Friend WithEvents lblProvince As Label
        Friend WithEvents lblPostCode As Label
        Friend WithEvents lblTemplePhone As Label
        Friend WithEvents lblAbbotName As Label
        Friend WithEvents lblWaiyawatName As Label
        Friend WithEvents lblBookkeeperName As Label
        Friend WithEvents lblPromptPayName As Label
        Friend WithEvents lblPromptPayID As Label

        ' Inputs
        Friend WithEvents txtTempleCode As TextBox
        Friend WithEvents txtTempleName As TextBox
        Friend WithEvents txtTempleAddress As TextBox
        Friend WithEvents txtPostCode As TextBox
        Friend WithEvents txtTemplePhone As TextBox
        Friend WithEvents cboAbbotName As ComboBox
        Friend WithEvents cboWaiyawatName As ComboBox
        Friend WithEvents cboBookkeeperName As ComboBox
        Friend WithEvents txtPromptPayName As TextBox
        Friend WithEvents txtPromptPayID As TextBox
        Friend WithEvents cboProvince As ComboBox
        Friend WithEvents cboAmphoe As ComboBox
        Friend WithEvents cboTambon As ComboBox
        Friend WithEvents chkUsePromptPay As CheckBox

        ' Layout
        Friend WithEvents tlpTempleInfo As TableLayoutPanel
        Friend WithEvents tlpContactInfo As TableLayoutPanel
        Friend WithEvents tlpPromptPay As TableLayoutPanel
        Friend WithEvents tlpPersonnel As TableLayoutPanel
        Friend WithEvents tlpPersonnelList As TableLayoutPanel
        Friend WithEvents gbTempleInfo As GroupBox
        Friend WithEvents gbContactInfo As GroupBox
        Friend WithEvents gbPromptPay As GroupBox
        Friend WithEvents gbPersonnel As GroupBox
        Friend WithEvents gbPersonnelList As GroupBox
        Friend WithEvents flpButtons As FlowLayoutPanel

        ' Buttons
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents btnLocationImport As Button
        Friend WithEvents btnManagePersonnel As Button
        Friend WithEvents ttMain As ToolTip

        ' Personnel Grid
        Friend WithEvents dgvPersonnel As DataGridView

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
            pMainContainer = New Panel()
            pContent = New Panel()
            gbPersonnelList = New GroupBox()
            tlpPersonnelList = New TableLayoutPanel()
            dgvPersonnel = New DataGridView()
            gbPersonnel = New GroupBox()
            tlpPersonnel = New TableLayoutPanel()
            lblAbbotName = New Label()
            cboAbbotName = New ComboBox()
            lblWaiyawatName = New Label()
            cboWaiyawatName = New ComboBox()
            lblBookkeeperName = New Label()
            cboBookkeeperName = New ComboBox()
            gbPromptPay = New GroupBox()
            tlpPromptPay = New TableLayoutPanel()
            chkUsePromptPay = New CheckBox()
            lblPromptPayName = New Label()
            txtPromptPayName = New TextBox()
            lblPromptPayID = New Label()
            txtPromptPayID = New TextBox()
            gbContactInfo = New GroupBox()
            tlpContactInfo = New TableLayoutPanel()
            lblProvince = New Label()
            cboProvince = New ComboBox()
            lblAmphoe = New Label()
            cboAmphoe = New ComboBox()
            lblTambon = New Label()
            cboTambon = New ComboBox()
            lblPostCode = New Label()
            txtPostCode = New TextBox()
            lblTemplePhone = New Label()
            txtTemplePhone = New TextBox()
            gbTempleInfo = New GroupBox()
            tlpTempleInfo = New TableLayoutPanel()
            lblTempleCode = New Label()
            txtTempleCode = New TextBox()
            lblTempleName = New Label()
            txtTempleName = New TextBox()
            lblTempleAddress = New Label()
            txtTempleAddress = New TextBox()
            pBottom = New Panel()
            flpButtons = New FlowLayoutPanel()
            btnClose = New Button()
            btnManagePersonnel = New Button()
            btnLocationImport = New Button()
            btnCancel = New Button()
            btnSave = New Button()
            pHeader = New Panel()
            pMainContainer.SuspendLayout()
            pContent.SuspendLayout()
            gbPersonnelList.SuspendLayout()
            tlpPersonnelList.SuspendLayout()
            CType(dgvPersonnel, ISupportInitialize).BeginInit()
            gbPersonnel.SuspendLayout()
            tlpPersonnel.SuspendLayout()
            gbPromptPay.SuspendLayout()
            tlpPromptPay.SuspendLayout()
            gbContactInfo.SuspendLayout()
            tlpContactInfo.SuspendLayout()
            gbTempleInfo.SuspendLayout()
            tlpTempleInfo.SuspendLayout()
            pBottom.SuspendLayout()
            flpButtons.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Fill
            lblHeader.Font = New Font("Tahoma", 14F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(100, 23)
            lblHeader.TabIndex = 0
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' pMainContainer
            ' 
            pMainContainer.BackColor = Color.Transparent
            pMainContainer.Controls.Add(pContent)
            pMainContainer.Controls.Add(pBottom)
            pMainContainer.Dock = DockStyle.Fill
            pMainContainer.Location = New Point(0, 0)
            pMainContainer.Name = "pMainContainer"
            pMainContainer.Padding = New Padding(10)
            pMainContainer.Size = New Size(1202, 817)
            pMainContainer.TabIndex = 0
            ' 
            ' pContent
            ' 
            pContent.AutoScroll = True
            pContent.BackColor = Color.White
            pContent.Controls.Add(gbPersonnelList)
            pContent.Controls.Add(gbPersonnel)
            pContent.Controls.Add(gbPromptPay)
            pContent.Controls.Add(gbContactInfo)
            pContent.Controls.Add(gbTempleInfo)
            pContent.Dock = DockStyle.Fill
            pContent.Location = New Point(10, 10)
            pContent.Name = "pContent"
            pContent.Padding = New Padding(10)
            pContent.Size = New Size(1182, 697)
            pContent.TabIndex = 2
            ' 
            ' gbPersonnelList
            ' 
            gbPersonnelList.Controls.Add(tlpPersonnelList)
            gbPersonnelList.Dock = DockStyle.Top
            gbPersonnelList.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbPersonnelList.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbPersonnelList.Location = New Point(10, 785)
            gbPersonnelList.Margin = New Padding(0, 0, 0, 12)
            gbPersonnelList.Name = "gbPersonnelList"
            gbPersonnelList.Padding = New Padding(10, 20, 10, 10)
            gbPersonnelList.Size = New Size(1136, 100)
            gbPersonnelList.TabIndex = 4
            gbPersonnelList.TabStop = False
            gbPersonnelList.Text = "📋 รายชื่อและบทบาทบุคลากร"
            ' 
            ' tlpPersonnelList
            ' 
            tlpPersonnelList.ColumnCount = 1
            tlpPersonnelList.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpPersonnelList.Controls.Add(dgvPersonnel, 0, 0)
            tlpPersonnelList.Dock = DockStyle.Fill
            tlpPersonnelList.Location = New Point(10, 45)
            tlpPersonnelList.Name = "tlpPersonnelList"
            tlpPersonnelList.RowCount = 1
            tlpPersonnelList.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
            tlpPersonnelList.Size = New Size(1116, 45)
            tlpPersonnelList.TabIndex = 0
            ' 
            ' dgvPersonnel
            ' 
            dgvPersonnel.AllowUserToAddRows = False
            dgvPersonnel.AllowUserToDeleteRows = False
            dgvPersonnel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvPersonnel.BackgroundColor = Color.White
            dgvPersonnel.BorderStyle = BorderStyle.Fixed3D
            dgvPersonnel.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvPersonnel.Dock = DockStyle.Fill
            dgvPersonnel.Location = New Point(3, 3)
            dgvPersonnel.MultiSelect = False
            dgvPersonnel.Name = "dgvPersonnel"
            dgvPersonnel.ReadOnly = True
            dgvPersonnel.RowHeadersVisible = False
            dgvPersonnel.RowHeadersWidth = 62
            dgvPersonnel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvPersonnel.Size = New Size(1110, 39)
            dgvPersonnel.TabIndex = 0
            ' 
            ' gbPersonnel
            ' 
            gbPersonnel.AutoSize = True
            gbPersonnel.AutoSizeMode = AutoSizeMode.GrowAndShrink
            gbPersonnel.Controls.Add(tlpPersonnel)
            gbPersonnel.Dock = DockStyle.Top
            gbPersonnel.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbPersonnel.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbPersonnel.Location = New Point(10, 610)
            gbPersonnel.Margin = New Padding(0, 0, 0, 12)
            gbPersonnel.Name = "gbPersonnel"
            gbPersonnel.Padding = New Padding(10, 20, 10, 10)
            gbPersonnel.Size = New Size(1136, 175)
            gbPersonnel.TabIndex = 3
            gbPersonnel.TabStop = False
            gbPersonnel.Text = "👤 ผู้ดำรงตำแหน่งในวัด"
            ' 
            ' tlpPersonnel
            ' 
            tlpPersonnel.AutoSize = True
            tlpPersonnel.AutoSizeMode = AutoSizeMode.GrowAndShrink
            tlpPersonnel.ColumnCount = 2
            tlpPersonnel.ColumnStyles.Add(New ColumnStyle())
            tlpPersonnel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpPersonnel.Controls.Add(lblAbbotName, 0, 0)
            tlpPersonnel.Controls.Add(cboAbbotName, 1, 0)
            tlpPersonnel.Controls.Add(lblWaiyawatName, 0, 1)
            tlpPersonnel.Controls.Add(cboWaiyawatName, 1, 1)
            tlpPersonnel.Controls.Add(lblBookkeeperName, 0, 2)
            tlpPersonnel.Controls.Add(cboBookkeeperName, 1, 2)
            tlpPersonnel.Dock = DockStyle.Top
            tlpPersonnel.Location = New Point(10, 45)
            tlpPersonnel.Name = "tlpPersonnel"
            tlpPersonnel.RowCount = 3
            tlpPersonnel.RowStyles.Add(New RowStyle())
            tlpPersonnel.RowStyles.Add(New RowStyle())
            tlpPersonnel.RowStyles.Add(New RowStyle())
            tlpPersonnel.Size = New Size(1116, 120)
            tlpPersonnel.TabIndex = 0
            ' 
            ' lblAbbotName
            ' 
            lblAbbotName.AutoSize = True
            lblAbbotName.Dock = DockStyle.Fill
            lblAbbotName.Font = New Font("Tahoma", 10F)
            lblAbbotName.Location = New Point(4, 4)
            lblAbbotName.Margin = New Padding(4)
            lblAbbotName.Name = "lblAbbotName"
            lblAbbotName.Size = New Size(126, 32)
            lblAbbotName.TabIndex = 16
            lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            lblAbbotName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAbbotName
            ' 
            cboAbbotName.Dock = DockStyle.Fill
            cboAbbotName.DropDownStyle = ComboBoxStyle.DropDownList
            cboAbbotName.Font = New Font("Tahoma", 10F)
            cboAbbotName.Location = New Point(138, 4)
            cboAbbotName.Margin = New Padding(4)
            cboAbbotName.Name = "cboAbbotName"
            cboAbbotName.Size = New Size(974, 32)
            cboAbbotName.TabIndex = 17
            ' 
            ' lblWaiyawatName
            ' 
            lblWaiyawatName.AutoSize = True
            lblWaiyawatName.Dock = DockStyle.Fill
            lblWaiyawatName.Font = New Font("Tahoma", 10F)
            lblWaiyawatName.Location = New Point(4, 44)
            lblWaiyawatName.Margin = New Padding(4)
            lblWaiyawatName.Name = "lblWaiyawatName"
            lblWaiyawatName.Size = New Size(126, 32)
            lblWaiyawatName.TabIndex = 18
            lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            lblWaiyawatName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboWaiyawatName
            ' 
            cboWaiyawatName.Dock = DockStyle.Fill
            cboWaiyawatName.DropDownStyle = ComboBoxStyle.DropDownList
            cboWaiyawatName.Font = New Font("Tahoma", 10F)
            cboWaiyawatName.Location = New Point(138, 44)
            cboWaiyawatName.Margin = New Padding(4)
            cboWaiyawatName.Name = "cboWaiyawatName"
            cboWaiyawatName.Size = New Size(974, 32)
            cboWaiyawatName.TabIndex = 19
            ' 
            ' lblBookkeeperName
            ' 
            lblBookkeeperName.AutoSize = True
            lblBookkeeperName.Dock = DockStyle.Fill
            lblBookkeeperName.Font = New Font("Tahoma", 10F)
            lblBookkeeperName.Location = New Point(4, 84)
            lblBookkeeperName.Margin = New Padding(4)
            lblBookkeeperName.Name = "lblBookkeeperName"
            lblBookkeeperName.Size = New Size(126, 32)
            lblBookkeeperName.TabIndex = 20
            lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            lblBookkeeperName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboBookkeeperName
            ' 
            cboBookkeeperName.Dock = DockStyle.Fill
            cboBookkeeperName.DropDownStyle = ComboBoxStyle.DropDownList
            cboBookkeeperName.Font = New Font("Tahoma", 10F)
            cboBookkeeperName.Location = New Point(138, 84)
            cboBookkeeperName.Margin = New Padding(4)
            cboBookkeeperName.Name = "cboBookkeeperName"
            cboBookkeeperName.Size = New Size(974, 32)
            cboBookkeeperName.TabIndex = 21
            ' 
            ' gbPromptPay
            ' 
            gbPromptPay.AutoSize = True
            gbPromptPay.AutoSizeMode = AutoSizeMode.GrowAndShrink
            gbPromptPay.Controls.Add(tlpPromptPay)
            gbPromptPay.Dock = DockStyle.Top
            gbPromptPay.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbPromptPay.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbPromptPay.Location = New Point(10, 439)
            gbPromptPay.Margin = New Padding(0, 0, 0, 12)
            gbPromptPay.Name = "gbPromptPay"
            gbPromptPay.Padding = New Padding(10, 20, 10, 10)
            gbPromptPay.Size = New Size(1136, 171)
            gbPromptPay.TabIndex = 2
            gbPromptPay.TabStop = False
            gbPromptPay.Text = "💳 พร้อมเพย์"
            ' 
            ' tlpPromptPay
            ' 
            tlpPromptPay.AutoSize = True
            tlpPromptPay.AutoSizeMode = AutoSizeMode.GrowAndShrink
            tlpPromptPay.ColumnCount = 2
            tlpPromptPay.ColumnStyles.Add(New ColumnStyle())
            tlpPromptPay.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpPromptPay.Controls.Add(chkUsePromptPay, 1, 0)
            tlpPromptPay.Controls.Add(lblPromptPayName, 0, 1)
            tlpPromptPay.Controls.Add(txtPromptPayName, 1, 1)
            tlpPromptPay.Controls.Add(lblPromptPayID, 0, 2)
            tlpPromptPay.Controls.Add(txtPromptPayID, 1, 2)
            tlpPromptPay.Dock = DockStyle.Top
            tlpPromptPay.Location = New Point(10, 45)
            tlpPromptPay.Name = "tlpPromptPay"
            tlpPromptPay.RowCount = 3
            tlpPromptPay.RowStyles.Add(New RowStyle())
            tlpPromptPay.RowStyles.Add(New RowStyle())
            tlpPromptPay.RowStyles.Add(New RowStyle())
            tlpPromptPay.Size = New Size(1116, 116)
            tlpPromptPay.TabIndex = 0
            ' 
            ' chkUsePromptPay
            ' 
            chkUsePromptPay.AutoSize = True
            chkUsePromptPay.Dock = DockStyle.Fill
            chkUsePromptPay.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            chkUsePromptPay.ForeColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            chkUsePromptPay.Location = New Point(222, 4)
            chkUsePromptPay.Margin = New Padding(4)
            chkUsePromptPay.Name = "chkUsePromptPay"
            chkUsePromptPay.Size = New Size(890, 28)
            chkUsePromptPay.TabIndex = 22
            chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"
            chkUsePromptPay.UseVisualStyleBackColor = True
            ' 
            ' lblPromptPayName
            ' 
            lblPromptPayName.AutoSize = True
            lblPromptPayName.Dock = DockStyle.Fill
            lblPromptPayName.Font = New Font("Tahoma", 10F)
            lblPromptPayName.Location = New Point(4, 40)
            lblPromptPayName.Margin = New Padding(4)
            lblPromptPayName.Name = "lblPromptPayName"
            lblPromptPayName.Size = New Size(210, 32)
            lblPromptPayName.TabIndex = 23
            lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            lblPromptPayName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayName
            ' 
            txtPromptPayName.Dock = DockStyle.Fill
            txtPromptPayName.Font = New Font("Tahoma", 10F)
            txtPromptPayName.Location = New Point(222, 40)
            txtPromptPayName.Margin = New Padding(4)
            txtPromptPayName.Name = "txtPromptPayName"
            txtPromptPayName.Size = New Size(890, 32)
            txtPromptPayName.TabIndex = 24
            ' 
            ' lblPromptPayID
            ' 
            lblPromptPayID.AutoSize = True
            lblPromptPayID.Dock = DockStyle.Fill
            lblPromptPayID.Font = New Font("Tahoma", 10F)
            lblPromptPayID.Location = New Point(4, 80)
            lblPromptPayID.Margin = New Padding(4)
            lblPromptPayID.Name = "lblPromptPayID"
            lblPromptPayID.Size = New Size(210, 32)
            lblPromptPayID.TabIndex = 25
            lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            lblPromptPayID.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayID
            ' 
            txtPromptPayID.Dock = DockStyle.Fill
            txtPromptPayID.Font = New Font("Tahoma", 10F)
            txtPromptPayID.Location = New Point(222, 80)
            txtPromptPayID.Margin = New Padding(4)
            txtPromptPayID.Name = "txtPromptPayID"
            txtPromptPayID.Size = New Size(890, 32)
            txtPromptPayID.TabIndex = 26
            ' 
            ' gbContactInfo
            ' 
            gbContactInfo.AutoSize = True
            gbContactInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink
            gbContactInfo.Controls.Add(tlpContactInfo)
            gbContactInfo.Dock = DockStyle.Top
            gbContactInfo.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbContactInfo.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbContactInfo.Location = New Point(10, 184)
            gbContactInfo.Margin = New Padding(0, 0, 0, 12)
            gbContactInfo.Name = "gbContactInfo"
            gbContactInfo.Padding = New Padding(10, 20, 10, 10)
            gbContactInfo.Size = New Size(1136, 255)
            gbContactInfo.TabIndex = 1
            gbContactInfo.TabStop = False
            gbContactInfo.Text = "📞 ข้อมูลติดต่อ"
            ' 
            ' tlpContactInfo
            ' 
            tlpContactInfo.AutoSize = True
            tlpContactInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink
            tlpContactInfo.ColumnCount = 2
            tlpContactInfo.ColumnStyles.Add(New ColumnStyle())
            tlpContactInfo.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpContactInfo.Controls.Add(lblProvince, 0, 0)
            tlpContactInfo.Controls.Add(cboProvince, 1, 0)
            tlpContactInfo.Controls.Add(lblAmphoe, 0, 1)
            tlpContactInfo.Controls.Add(cboAmphoe, 1, 1)
            tlpContactInfo.Controls.Add(lblTambon, 0, 2)
            tlpContactInfo.Controls.Add(cboTambon, 1, 2)
            tlpContactInfo.Controls.Add(lblPostCode, 0, 3)
            tlpContactInfo.Controls.Add(txtPostCode, 1, 3)
            tlpContactInfo.Controls.Add(lblTemplePhone, 0, 4)
            tlpContactInfo.Controls.Add(txtTemplePhone, 1, 4)
            tlpContactInfo.Dock = DockStyle.Top
            tlpContactInfo.Location = New Point(10, 45)
            tlpContactInfo.Name = "tlpContactInfo"
            tlpContactInfo.RowCount = 5
            tlpContactInfo.RowStyles.Add(New RowStyle())
            tlpContactInfo.RowStyles.Add(New RowStyle())
            tlpContactInfo.RowStyles.Add(New RowStyle())
            tlpContactInfo.RowStyles.Add(New RowStyle())
            tlpContactInfo.RowStyles.Add(New RowStyle())
            tlpContactInfo.Size = New Size(1116, 200)
            tlpContactInfo.TabIndex = 0
            ' 
            ' lblProvince
            ' 
            lblProvince.AutoSize = True
            lblProvince.Dock = DockStyle.Fill
            lblProvince.Font = New Font("Tahoma", 10F)
            lblProvince.Location = New Point(4, 4)
            lblProvince.Margin = New Padding(4)
            lblProvince.Name = "lblProvince"
            lblProvince.Size = New Size(128, 32)
            lblProvince.TabIndex = 6
            lblProvince.Text = "จังหวัด:"
            lblProvince.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboProvince
            ' 
            cboProvince.Dock = DockStyle.Fill
            cboProvince.DropDownStyle = ComboBoxStyle.DropDownList
            cboProvince.Font = New Font("Tahoma", 10F)
            cboProvince.Location = New Point(140, 4)
            cboProvince.Margin = New Padding(4)
            cboProvince.Name = "cboProvince"
            cboProvince.Size = New Size(972, 32)
            cboProvince.TabIndex = 7
            ' 
            ' lblAmphoe
            ' 
            lblAmphoe.AutoSize = True
            lblAmphoe.Dock = DockStyle.Fill
            lblAmphoe.Font = New Font("Tahoma", 10F)
            lblAmphoe.Location = New Point(4, 44)
            lblAmphoe.Margin = New Padding(4)
            lblAmphoe.Name = "lblAmphoe"
            lblAmphoe.Size = New Size(128, 32)
            lblAmphoe.TabIndex = 8
            lblAmphoe.Text = "อำเภอ:"
            lblAmphoe.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAmphoe
            ' 
            cboAmphoe.Dock = DockStyle.Fill
            cboAmphoe.DropDownStyle = ComboBoxStyle.DropDownList
            cboAmphoe.Font = New Font("Tahoma", 10F)
            cboAmphoe.Location = New Point(140, 44)
            cboAmphoe.Margin = New Padding(4)
            cboAmphoe.Name = "cboAmphoe"
            cboAmphoe.Size = New Size(972, 32)
            cboAmphoe.TabIndex = 9
            ' 
            ' lblTambon
            ' 
            lblTambon.AutoSize = True
            lblTambon.Dock = DockStyle.Fill
            lblTambon.Font = New Font("Tahoma", 10F)
            lblTambon.Location = New Point(4, 84)
            lblTambon.Margin = New Padding(4)
            lblTambon.Name = "lblTambon"
            lblTambon.Size = New Size(128, 32)
            lblTambon.TabIndex = 10
            lblTambon.Text = "ตำบล:"
            lblTambon.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboTambon
            ' 
            cboTambon.Dock = DockStyle.Fill
            cboTambon.DropDownStyle = ComboBoxStyle.DropDownList
            cboTambon.Font = New Font("Tahoma", 10F)
            cboTambon.Location = New Point(140, 84)
            cboTambon.Margin = New Padding(4)
            cboTambon.Name = "cboTambon"
            cboTambon.Size = New Size(972, 32)
            cboTambon.TabIndex = 11
            ' 
            ' lblPostCode
            ' 
            lblPostCode.AutoSize = True
            lblPostCode.Dock = DockStyle.Fill
            lblPostCode.Font = New Font("Tahoma", 10F)
            lblPostCode.Location = New Point(4, 124)
            lblPostCode.Margin = New Padding(4)
            lblPostCode.Name = "lblPostCode"
            lblPostCode.Size = New Size(128, 32)
            lblPostCode.TabIndex = 12
            lblPostCode.Text = "รหัสไปรษณีย์:"
            lblPostCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPostCode
            ' 
            txtPostCode.Dock = DockStyle.Fill
            txtPostCode.Font = New Font("Tahoma", 10F)
            txtPostCode.Location = New Point(140, 124)
            txtPostCode.Margin = New Padding(4)
            txtPostCode.Name = "txtPostCode"
            txtPostCode.Size = New Size(972, 32)
            txtPostCode.TabIndex = 13
            ' 
            ' lblTemplePhone
            ' 
            lblTemplePhone.AutoSize = True
            lblTemplePhone.Dock = DockStyle.Fill
            lblTemplePhone.Font = New Font("Tahoma", 10F)
            lblTemplePhone.Location = New Point(4, 164)
            lblTemplePhone.Margin = New Padding(4)
            lblTemplePhone.Name = "lblTemplePhone"
            lblTemplePhone.Size = New Size(128, 32)
            lblTemplePhone.TabIndex = 14
            lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            lblTemplePhone.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTemplePhone
            ' 
            txtTemplePhone.Dock = DockStyle.Fill
            txtTemplePhone.Font = New Font("Tahoma", 10F)
            txtTemplePhone.Location = New Point(140, 164)
            txtTemplePhone.Margin = New Padding(4)
            txtTemplePhone.Name = "txtTemplePhone"
            txtTemplePhone.Size = New Size(972, 32)
            txtTemplePhone.TabIndex = 15
            ' 
            ' gbTempleInfo
            ' 
            gbTempleInfo.AutoSize = True
            gbTempleInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink
            gbTempleInfo.Controls.Add(tlpTempleInfo)
            gbTempleInfo.Dock = DockStyle.Top
            gbTempleInfo.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbTempleInfo.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbTempleInfo.Location = New Point(10, 10)
            gbTempleInfo.Margin = New Padding(0, 0, 0, 12)
            gbTempleInfo.Name = "gbTempleInfo"
            gbTempleInfo.Padding = New Padding(10, 20, 10, 10)
            gbTempleInfo.Size = New Size(1136, 174)
            gbTempleInfo.TabIndex = 0
            gbTempleInfo.TabStop = False
            gbTempleInfo.Text = "🏛️ ข้อมูลพื้นฐานของวัด"
            ' 
            ' tlpTempleInfo
            ' 
            tlpTempleInfo.AutoSize = True
            tlpTempleInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink
            tlpTempleInfo.ColumnCount = 2
            tlpTempleInfo.ColumnStyles.Add(New ColumnStyle())
            tlpTempleInfo.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpTempleInfo.Controls.Add(lblTempleCode, 0, 0)
            tlpTempleInfo.Controls.Add(txtTempleCode, 1, 0)
            tlpTempleInfo.Controls.Add(lblTempleName, 0, 1)
            tlpTempleInfo.Controls.Add(txtTempleName, 1, 1)
            tlpTempleInfo.Controls.Add(lblTempleAddress, 0, 2)
            tlpTempleInfo.Controls.Add(txtTempleAddress, 1, 2)
            tlpTempleInfo.Dock = DockStyle.Top
            tlpTempleInfo.Location = New Point(10, 45)
            tlpTempleInfo.Name = "tlpTempleInfo"
            tlpTempleInfo.RowCount = 3
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.Size = New Size(1116, 119)
            tlpTempleInfo.TabIndex = 0
            ' 
            ' lblTempleCode
            ' 
            lblTempleCode.AutoSize = True
            lblTempleCode.Dock = DockStyle.Fill
            lblTempleCode.Font = New Font("Tahoma", 10F)
            lblTempleCode.Location = New Point(4, 4)
            lblTempleCode.Margin = New Padding(4)
            lblTempleCode.Name = "lblTempleCode"
            lblTempleCode.Size = New Size(77, 32)
            lblTempleCode.TabIndex = 0
            lblTempleCode.Text = "รหัสวัด:"
            lblTempleCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleCode
            ' 
            txtTempleCode.Dock = DockStyle.Fill
            txtTempleCode.Font = New Font("Tahoma", 10F)
            txtTempleCode.Location = New Point(89, 4)
            txtTempleCode.Margin = New Padding(4)
            txtTempleCode.Name = "txtTempleCode"
            txtTempleCode.Size = New Size(1023, 32)
            txtTempleCode.TabIndex = 1
            ' 
            ' lblTempleName
            ' 
            lblTempleName.AutoSize = True
            lblTempleName.Dock = DockStyle.Fill
            lblTempleName.Font = New Font("Tahoma", 10F)
            lblTempleName.Location = New Point(4, 44)
            lblTempleName.Margin = New Padding(4)
            lblTempleName.Name = "lblTempleName"
            lblTempleName.Size = New Size(77, 32)
            lblTempleName.TabIndex = 2
            lblTempleName.Text = "ชื่อวัด:"
            lblTempleName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleName
            ' 
            txtTempleName.Dock = DockStyle.Fill
            txtTempleName.Font = New Font("Tahoma", 10F)
            txtTempleName.Location = New Point(89, 44)
            txtTempleName.Margin = New Padding(4)
            txtTempleName.Name = "txtTempleName"
            txtTempleName.Size = New Size(1023, 32)
            txtTempleName.TabIndex = 3
            ' 
            ' lblTempleAddress
            ' 
            lblTempleAddress.AutoSize = True
            lblTempleAddress.Dock = DockStyle.Fill
            lblTempleAddress.Font = New Font("Tahoma", 10F)
            lblTempleAddress.Location = New Point(4, 84)
            lblTempleAddress.Margin = New Padding(4)
            lblTempleAddress.Name = "lblTempleAddress"
            lblTempleAddress.Size = New Size(77, 31)
            lblTempleAddress.TabIndex = 4
            lblTempleAddress.Text = "ที่อยู่วัด:"
            lblTempleAddress.TextAlign = ContentAlignment.TopRight
            ' 
            ' txtTempleAddress
            ' 
            txtTempleAddress.Dock = DockStyle.Fill
            txtTempleAddress.Font = New Font("Tahoma", 10F)
            txtTempleAddress.Location = New Point(89, 84)
            txtTempleAddress.Margin = New Padding(4)
            txtTempleAddress.Multiline = True
            txtTempleAddress.Name = "txtTempleAddress"
            txtTempleAddress.ScrollBars = ScrollBars.Vertical
            txtTempleAddress.Size = New Size(1023, 31)
            txtTempleAddress.TabIndex = 5
            ' 
            ' pBottom
            ' 
            pBottom.BackColor = Color.FromArgb(CByte(241), CByte(245), CByte(249))
            pBottom.Controls.Add(flpButtons)
            pBottom.Dock = DockStyle.Bottom
            pBottom.Location = New Point(10, 707)
            pBottom.Name = "pBottom"
            pBottom.Size = New Size(1182, 100)
            pBottom.TabIndex = 1
            ' 
            ' flpButtons
            ' 
            flpButtons.BackColor = Color.FromArgb(CByte(241), CByte(245), CByte(249))
            flpButtons.Controls.Add(btnClose)
            flpButtons.Controls.Add(btnManagePersonnel)
            flpButtons.Controls.Add(btnLocationImport)
            flpButtons.Controls.Add(btnCancel)
            flpButtons.Controls.Add(btnSave)
            flpButtons.Dock = DockStyle.Fill
            flpButtons.FlowDirection = FlowDirection.RightToLeft
            flpButtons.Location = New Point(0, 0)
            flpButtons.Name = "flpButtons"
            flpButtons.Padding = New Padding(10, 8, 10, 8)
            flpButtons.Size = New Size(1182, 100)
            flpButtons.TabIndex = 0
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1032, 8)
            btnClose.Margin = New Padding(6, 0, 0, 0)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(130, 45)
            btnClose.TabIndex = 4
            btnClose.Text = "❌ ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' btnManagePersonnel
            ' 
            btnManagePersonnel.BackColor = Color.FromArgb(CByte(147), CByte(51), CByte(234))
            btnManagePersonnel.FlatStyle = FlatStyle.Flat
            btnManagePersonnel.Font = New Font("Tahoma", 9F, FontStyle.Bold)
            btnManagePersonnel.ForeColor = Color.White
            btnManagePersonnel.Location = New Point(853, 8)
            btnManagePersonnel.Margin = New Padding(6, 0, 0, 0)
            btnManagePersonnel.Name = "btnManagePersonnel"
            btnManagePersonnel.Size = New Size(173, 45)
            btnManagePersonnel.TabIndex = 3
            btnManagePersonnel.Text = "👤 จัดการรายชื่อ..."
            btnManagePersonnel.UseVisualStyleBackColor = False
            ' 
            ' btnLocationImport
            ' 
            btnLocationImport.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnLocationImport.FlatStyle = FlatStyle.Flat
            btnLocationImport.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnLocationImport.ForeColor = Color.White
            btnLocationImport.Location = New Point(717, 8)
            btnLocationImport.Margin = New Padding(6, 0, 0, 0)
            btnLocationImport.Name = "btnLocationImport"
            btnLocationImport.Size = New Size(130, 45)
            btnLocationImport.TabIndex = 2
            btnLocationImport.Text = "📍 นำเข้าที่อยู่"
            btnLocationImport.UseVisualStyleBackColor = False
            ' 
            ' btnCancel
            ' 
            btnCancel.BackColor = Color.FromArgb(CByte(217), CByte(119), CByte(6))
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnCancel.ForeColor = Color.White
            btnCancel.Location = New Point(581, 8)
            btnCancel.Margin = New Padding(6, 0, 0, 0)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(130, 45)
            btnCancel.TabIndex = 1
            btnCancel.Text = "🔄 โหลดใหม่"
            btnCancel.UseVisualStyleBackColor = False
            ' 
            ' btnSave
            ' 
            btnSave.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSave.ForeColor = Color.White
            btnSave.Location = New Point(445, 8)
            btnSave.Margin = New Padding(6, 0, 0, 0)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(130, 45)
            btnSave.TabIndex = 0
            btnSave.Text = "💾 บันทึก"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' pHeader
            ' 
            pHeader.Location = New Point(0, 0)
            pHeader.Name = "pHeader"
            pHeader.Size = New Size(200, 100)
            pHeader.TabIndex = 0
            ' 
            ' FrmTempleSetting
            ' 
            AutoScaleDimensions = New SizeF(144F, 144F)
            AutoScaleMode = AutoScaleMode.Dpi
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1202, 817)
            Controls.Add(pMainContainer)
            Font = New Font("Tahoma", 10F)
            MinimumSize = New Size(800, 600)
            Name = "FrmTempleSetting"
            Text = "ตั้งค่าข้อมูลวัด"
            pMainContainer.ResumeLayout(False)
            pContent.ResumeLayout(False)
            pContent.PerformLayout()
            gbPersonnelList.ResumeLayout(False)
            tlpPersonnelList.ResumeLayout(False)
            CType(dgvPersonnel, ISupportInitialize).EndInit()
            gbPersonnel.ResumeLayout(False)
            gbPersonnel.PerformLayout()
            tlpPersonnel.ResumeLayout(False)
            tlpPersonnel.PerformLayout()
            gbPromptPay.ResumeLayout(False)
            gbPromptPay.PerformLayout()
            tlpPromptPay.ResumeLayout(False)
            tlpPromptPay.PerformLayout()
            gbContactInfo.ResumeLayout(False)
            gbContactInfo.PerformLayout()
            tlpContactInfo.ResumeLayout(False)
            tlpContactInfo.PerformLayout()
            gbTempleInfo.ResumeLayout(False)
            gbTempleInfo.PerformLayout()
            tlpTempleInfo.ResumeLayout(False)
            tlpTempleInfo.PerformLayout()
            pBottom.ResumeLayout(False)
            flpButtons.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
