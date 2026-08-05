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
            pHeader = New Panel()
            pContent = New Panel()
            pBottom = New Panel()
            gbTempleInfo = New GroupBox()
            tlpTempleInfo = New TableLayoutPanel()
            gbContactInfo = New GroupBox()
            tlpContactInfo = New TableLayoutPanel()
            gbPromptPay = New GroupBox()
            tlpPromptPay = New TableLayoutPanel()
            lblTempleCode = New Label()
            txtTempleCode = New TextBox()
            lblTempleName = New Label()
            txtTempleName = New TextBox()
            lblTempleAddress = New Label()
            txtTempleAddress = New TextBox()
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
            chkUsePromptPay = New CheckBox()
            lblPromptPayName = New Label()
            txtPromptPayName = New TextBox()
            lblPromptPayID = New Label()
            txtPromptPayID = New TextBox()
            gbPersonnel = New GroupBox()
            tlpPersonnel = New TableLayoutPanel()
            lblAbbotName = New Label()
            cboAbbotName = New ComboBox()
            lblWaiyawatName = New Label()
            cboWaiyawatName = New ComboBox()
            lblBookkeeperName = New Label()
            cboBookkeeperName = New ComboBox()
            gbPersonnelList = New GroupBox()
            tlpPersonnelList = New TableLayoutPanel()
            dgvPersonnel = New DataGridView()
            flpButtons = New FlowLayoutPanel()
            btnClose = New Button()
            btnManagePersonnel = New Button()
            btnLocationImport = New Button()
            btnCancel = New Button()
            btnSave = New Button()
            pMainContainer.SuspendLayout()
            pHeader.SuspendLayout()
            pContent.SuspendLayout()
            pBottom.SuspendLayout()
            gbTempleInfo.SuspendLayout()
            tlpTempleInfo.SuspendLayout()
            gbPersonnel.SuspendLayout()
            tlpPersonnel.SuspendLayout()
            gbPersonnelList.SuspendLayout()
            tlpPersonnelList.SuspendLayout()
            CType(dgvPersonnel, ISupportInitialize).BeginInit()
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
            lblHeader.Size = New Size(880, 42)
            lblHeader.TabIndex = 0
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' pMainContainer
            ' 
            pMainContainer.BackColor = Color.Transparent
            pMainContainer.Controls.Add(pContent)
            pMainContainer.Controls.Add(pBottom)
            pMainContainer.Controls.Add(pHeader)
            pMainContainer.Dock = DockStyle.Fill
            pMainContainer.Location = New Point(0, 0)
            pMainContainer.Name = "pMainContainer"
            pMainContainer.Padding = New Padding(10)
            pMainContainer.Size = New Size(900, 817)
            pMainContainer.TabIndex = 0
            ' 
            ' pHeader
            ' 
            pHeader.AutoSize = True
            pHeader.Controls.Add(lblHeader)
            pHeader.Dock = DockStyle.Top
            pHeader.Location = New Point(10, 10)
            pHeader.Name = "pHeader"
            pHeader.Size = New Size(880, 42)
            pHeader.TabIndex = 0
            ' 
            ' pBottom
            ' 
            pBottom.BackColor = Color.FromArgb(CByte(241), CByte(245), CByte(249))
            pBottom.Controls.Add(flpButtons)
            pBottom.Dock = DockStyle.Bottom
            pBottom.Location = New Point(10, 737)
            pBottom.Name = "pBottom"
            pBottom.Size = New Size(880, 70)
            pBottom.TabIndex = 1
            ' 
            ' pContent
            ' 
            pContent.AutoScroll = True
            pContent.BackColor = Color.White
            pContent.Controls.Add(gbPersonnelList)
            pContent.Controls.Add(gbPersonnel)
            pContent.Controls.Add(gbTempleInfo)
            pContent.Dock = DockStyle.Fill
            pContent.Location = New Point(10, 52)
            pContent.Name = "pContent"
            pContent.Padding = New Padding(10)
            pContent.Size = New Size(880, 685)
            pContent.TabIndex = 2
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
            gbTempleInfo.Size = New Size(844, 520)
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
            tlpTempleInfo.Controls.Add(lblProvince, 0, 3)
            tlpTempleInfo.Controls.Add(cboProvince, 1, 3)
            tlpTempleInfo.Controls.Add(lblAmphoe, 0, 4)
            tlpTempleInfo.Controls.Add(cboAmphoe, 1, 4)
            tlpTempleInfo.Controls.Add(lblTambon, 0, 5)
            tlpTempleInfo.Controls.Add(cboTambon, 1, 5)
            tlpTempleInfo.Controls.Add(lblPostCode, 0, 6)
            tlpTempleInfo.Controls.Add(txtPostCode, 1, 6)
            tlpTempleInfo.Controls.Add(lblTemplePhone, 0, 7)
            tlpTempleInfo.Controls.Add(txtTemplePhone, 1, 7)
            tlpTempleInfo.Controls.Add(chkUsePromptPay, 1, 8)
            tlpTempleInfo.Controls.Add(lblPromptPayName, 0, 9)
            tlpTempleInfo.Controls.Add(txtPromptPayName, 1, 9)
            tlpTempleInfo.Controls.Add(lblPromptPayID, 0, 10)
            tlpTempleInfo.Controls.Add(txtPromptPayID, 1, 10)
            tlpTempleInfo.Dock = DockStyle.Top
            tlpTempleInfo.Location = New Point(10, 45)
            tlpTempleInfo.Name = "tlpTempleInfo"
            tlpTempleInfo.RowCount = 11
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.RowStyles.Add(New RowStyle())
            tlpTempleInfo.Size = New Size(824, 465)
            tlpTempleInfo.TabIndex = 0
            ' 
            ' gbPersonnel
            ' 
            gbPersonnel.AutoSize = True
            gbPersonnel.AutoSizeMode = AutoSizeMode.GrowAndShrink
            gbPersonnel.Controls.Add(tlpPersonnel)
            gbPersonnel.Dock = DockStyle.Top
            gbPersonnel.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbPersonnel.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbPersonnel.Location = New Point(10, 542)
            gbPersonnel.Margin = New Padding(0, 0, 0, 12)
            gbPersonnel.Name = "gbPersonnel"
            gbPersonnel.Padding = New Padding(10, 20, 10, 10)
            gbPersonnel.Size = New Size(844, 160)
            gbPersonnel.TabIndex = 1
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
            tlpPersonnel.Size = New Size(824, 105)
            tlpPersonnel.TabIndex = 0
            ' 
            ' gbPersonnelList
            ' 
            gbPersonnelList.Controls.Add(tlpPersonnelList)
            gbPersonnelList.Dock = DockStyle.Top
            gbPersonnelList.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            gbPersonnelList.ForeColor = Color.FromArgb(CByte(30), CByte(64), CByte(175))
            gbPersonnelList.Location = New Point(10, 714)
            gbPersonnelList.Margin = New Padding(0, 0, 0, 12)
            gbPersonnelList.Name = "gbPersonnelList"
            gbPersonnelList.Padding = New Padding(10, 20, 10, 10)
            gbPersonnelList.Size = New Size(844, 320)
            gbPersonnelList.TabIndex = 2
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
            tlpPersonnelList.Size = New Size(824, 265)
            tlpPersonnelList.TabIndex = 0
            ' 
            ' lblTempleCode
            ' 
            lblTempleCode.AutoSize = True
            lblTempleCode.Dock = DockStyle.Fill
            lblTempleCode.Font = New Font("Tahoma", 10F)
            lblTempleCode.Location = New Point(3, 3)
            lblTempleCode.Margin = New Padding(4)
            lblTempleCode.Name = "lblTempleCode"
            lblTempleCode.Size = New Size(174, 24)
            lblTempleCode.TabIndex = 0
            lblTempleCode.Text = "รหัสวัด:"
            lblTempleCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleCode
            ' 
            txtTempleCode.Dock = DockStyle.Fill
            txtTempleCode.Font = New Font("Tahoma", 10F)
            txtTempleCode.Location = New Point(183, 3)
            txtTempleCode.Margin = New Padding(4)
            txtTempleCode.Name = "txtTempleCode"
            txtTempleCode.Size = New Size(648, 32)
            txtTempleCode.TabIndex = 1
            ' 
            ' lblTempleName
            ' 
            lblTempleName.AutoSize = True
            lblTempleName.Dock = DockStyle.Fill
            lblTempleName.Font = New Font("Tahoma", 10F)
            lblTempleName.Location = New Point(3, 43)
            lblTempleName.Margin = New Padding(4)
            lblTempleName.Name = "lblTempleName"
            lblTempleName.Size = New Size(174, 24)
            lblTempleName.TabIndex = 2
            lblTempleName.Text = "ชื่อวัด:"
            lblTempleName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleName
            ' 
            txtTempleName.Dock = DockStyle.Fill
            txtTempleName.Font = New Font("Tahoma", 10F)
            txtTempleName.Location = New Point(183, 43)
            txtTempleName.Margin = New Padding(4)
            txtTempleName.Name = "txtTempleName"
            txtTempleName.Size = New Size(648, 32)
            txtTempleName.TabIndex = 3
            ' 
            ' lblTempleAddress
            ' 
            lblTempleAddress.AutoSize = True
            lblTempleAddress.Dock = DockStyle.Fill
            lblTempleAddress.Font = New Font("Tahoma", 10F)
            lblTempleAddress.Location = New Point(3, 83)
            lblTempleAddress.Margin = New Padding(4)
            lblTempleAddress.Name = "lblTempleAddress"
            lblTempleAddress.Size = New Size(174, 24)
            lblTempleAddress.TabIndex = 4
            lblTempleAddress.Text = "ที่อยู่วัด:"
            lblTempleAddress.TextAlign = ContentAlignment.TopRight
            ' 
            ' txtTempleAddress
            ' 
            txtTempleAddress.Dock = DockStyle.Fill
            txtTempleAddress.Font = New Font("Tahoma", 10F)
            txtTempleAddress.Location = New Point(183, 83)
            txtTempleAddress.Margin = New Padding(4)
            txtTempleAddress.Multiline = True
            txtTempleAddress.Name = "txtTempleAddress"
            txtTempleAddress.ScrollBars = ScrollBars.Vertical
            txtTempleAddress.Size = New Size(648, 70)
            txtTempleAddress.TabIndex = 5
            ' 
            ' lblProvince
            ' 
            lblProvince.AutoSize = True
            lblProvince.Dock = DockStyle.Fill
            lblProvince.Font = New Font("Tahoma", 10F)
            lblProvince.Location = New Point(3, 163)
            lblProvince.Margin = New Padding(4)
            lblProvince.Name = "lblProvince"
            lblProvince.Size = New Size(174, 24)
            lblProvince.TabIndex = 6
            lblProvince.Text = "จังหวัด:"
            lblProvince.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboProvince
            ' 
            cboProvince.Dock = DockStyle.Fill
            cboProvince.DropDownStyle = ComboBoxStyle.DropDownList
            cboProvince.Font = New Font("Tahoma", 10F)
            cboProvince.Location = New Point(183, 163)
            cboProvince.Margin = New Padding(4)
            cboProvince.Name = "cboProvince"
            cboProvince.Size = New Size(648, 32)
            cboProvince.TabIndex = 7
            ' 
            ' lblAmphoe
            ' 
            lblAmphoe.AutoSize = True
            lblAmphoe.Dock = DockStyle.Fill
            lblAmphoe.Font = New Font("Tahoma", 10F)
            lblAmphoe.Location = New Point(3, 203)
            lblAmphoe.Margin = New Padding(4)
            lblAmphoe.Name = "lblAmphoe"
            lblAmphoe.Size = New Size(174, 24)
            lblAmphoe.TabIndex = 8
            lblAmphoe.Text = "อำเภอ:"
            lblAmphoe.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAmphoe
            ' 
            cboAmphoe.Dock = DockStyle.Fill
            cboAmphoe.DropDownStyle = ComboBoxStyle.DropDownList
            cboAmphoe.Font = New Font("Tahoma", 10F)
            cboAmphoe.Location = New Point(183, 203)
            cboAmphoe.Margin = New Padding(4)
            cboAmphoe.Name = "cboAmphoe"
            cboAmphoe.Size = New Size(648, 32)
            cboAmphoe.TabIndex = 9
            ' 
            ' lblTambon
            ' 
            lblTambon.AutoSize = True
            lblTambon.Dock = DockStyle.Fill
            lblTambon.Font = New Font("Tahoma", 10F)
            lblTambon.Location = New Point(3, 243)
            lblTambon.Margin = New Padding(4)
            lblTambon.Name = "lblTambon"
            lblTambon.Size = New Size(174, 24)
            lblTambon.TabIndex = 10
            lblTambon.Text = "ตำบล:"
            lblTambon.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboTambon
            ' 
            cboTambon.Dock = DockStyle.Fill
            cboTambon.DropDownStyle = ComboBoxStyle.DropDownList
            cboTambon.Font = New Font("Tahoma", 10F)
            cboTambon.Location = New Point(183, 243)
            cboTambon.Margin = New Padding(4)
            cboTambon.Name = "cboTambon"
            cboTambon.Size = New Size(648, 32)
            cboTambon.TabIndex = 11
            ' 
            ' lblPostCode
            ' 
            lblPostCode.AutoSize = True
            lblPostCode.Dock = DockStyle.Fill
            lblPostCode.Font = New Font("Tahoma", 10F)
            lblPostCode.Location = New Point(3, 283)
            lblPostCode.Margin = New Padding(4)
            lblPostCode.Name = "lblPostCode"
            lblPostCode.Size = New Size(174, 24)
            lblPostCode.TabIndex = 12
            lblPostCode.Text = "รหัสไปรษณีย์:"
            lblPostCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPostCode
            ' 
            txtPostCode.Dock = DockStyle.Fill
            txtPostCode.Font = New Font("Tahoma", 10F)
            txtPostCode.Location = New Point(183, 283)
            txtPostCode.Margin = New Padding(4)
            txtPostCode.Name = "txtPostCode"
            txtPostCode.Size = New Size(648, 32)
            txtPostCode.TabIndex = 13
            ' 
            ' lblTemplePhone
            ' 
            lblTemplePhone.AutoSize = True
            lblTemplePhone.Dock = DockStyle.Fill
            lblTemplePhone.Font = New Font("Tahoma", 10F)
            lblTemplePhone.Location = New Point(3, 323)
            lblTemplePhone.Margin = New Padding(4)
            lblTemplePhone.Name = "lblTemplePhone"
            lblTemplePhone.Size = New Size(174, 24)
            lblTemplePhone.TabIndex = 14
            lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            lblTemplePhone.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTemplePhone
            ' 
            txtTemplePhone.Dock = DockStyle.Fill
            txtTemplePhone.Font = New Font("Tahoma", 10F)
            txtTemplePhone.Location = New Point(183, 323)
            txtTemplePhone.Margin = New Padding(4)
            txtTemplePhone.Name = "txtTemplePhone"
            txtTemplePhone.Size = New Size(648, 32)
            txtTemplePhone.TabIndex = 15
            ' 
            ' lblAbbotName
            ' 
            lblAbbotName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            lblAbbotName.AutoSize = True
            lblAbbotName.Font = New Font("Tahoma", 10F)
            lblAbbotName.Location = New Point(3, 368)
            lblAbbotName.Name = "lblAbbotName"
            lblAbbotName.Size = New Size(174, 24)
            lblAbbotName.TabIndex = 16
            lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            lblAbbotName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAbbotName
            ' 
            cboAbbotName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            cboAbbotName.DropDownStyle = ComboBoxStyle.DropDownList
            cboAbbotName.Font = New Font("Tahoma", 10F)
            cboAbbotName.Location = New Point(183, 364)
            cboAbbotName.Name = "cboAbbotName"
            cboAbbotName.Size = New Size(648, 32)
            cboAbbotName.TabIndex = 17
            ' 
            ' lblWaiyawatName
            ' 
            lblWaiyawatName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            lblWaiyawatName.AutoSize = True
            lblWaiyawatName.Font = New Font("Tahoma", 10F)
            lblWaiyawatName.Location = New Point(3, 448)
            lblWaiyawatName.Name = "lblWaiyawatName"
            lblWaiyawatName.Size = New Size(174, 24)
            lblWaiyawatName.TabIndex = 18
            lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            lblWaiyawatName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboWaiyawatName
            ' 
            cboWaiyawatName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            cboWaiyawatName.DropDownStyle = ComboBoxStyle.DropDownList
            cboWaiyawatName.Font = New Font("Tahoma", 10F)
            cboWaiyawatName.Location = New Point(183, 444)
            cboWaiyawatName.Name = "cboWaiyawatName"
            cboWaiyawatName.Size = New Size(648, 32)
            cboWaiyawatName.TabIndex = 19
            ' 
            ' lblBookkeeperName
            ' 
            lblBookkeeperName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            lblBookkeeperName.AutoSize = True
            lblBookkeeperName.Font = New Font("Tahoma", 10F)
            lblBookkeeperName.Location = New Point(3, 528)
            lblBookkeeperName.Name = "lblBookkeeperName"
            lblBookkeeperName.Size = New Size(174, 24)
            lblBookkeeperName.TabIndex = 20
            lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            lblBookkeeperName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboBookkeeperName
            ' 
            cboBookkeeperName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            cboBookkeeperName.DropDownStyle = ComboBoxStyle.DropDownList
            cboBookkeeperName.Font = New Font("Tahoma", 10F)
            cboBookkeeperName.Location = New Point(183, 524)
            cboBookkeeperName.Name = "cboBookkeeperName"
            cboBookkeeperName.Size = New Size(648, 32)
            cboBookkeeperName.TabIndex = 21
            ' 
            ' chkUsePromptPay
            ' 
            chkUsePromptPay.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            chkUsePromptPay.AutoSize = True
            chkUsePromptPay.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            chkUsePromptPay.ForeColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            chkUsePromptPay.Location = New Point(183, 606)
            chkUsePromptPay.Name = "chkUsePromptPay"
            chkUsePromptPay.Size = New Size(648, 28)
            chkUsePromptPay.TabIndex = 22
            chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"
            chkUsePromptPay.UseVisualStyleBackColor = True
            ' 
            ' lblPromptPayName
            ' 
            lblPromptPayName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            lblPromptPayName.AutoSize = True
            lblPromptPayName.Font = New Font("Tahoma", 10F)
            lblPromptPayName.Location = New Point(3, 648)
            lblPromptPayName.Name = "lblPromptPayName"
            lblPromptPayName.Size = New Size(174, 24)
            lblPromptPayName.TabIndex = 23
            lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            lblPromptPayName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayName
            ' 
            txtPromptPayName.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            txtPromptPayName.Font = New Font("Tahoma", 10F)
            txtPromptPayName.Location = New Point(183, 644)
            txtPromptPayName.Name = "txtPromptPayName"
            txtPromptPayName.Size = New Size(648, 32)
            txtPromptPayName.TabIndex = 24
            ' 
            ' lblPromptPayID
            ' 
            lblPromptPayID.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            lblPromptPayID.AutoSize = True
            lblPromptPayID.Font = New Font("Tahoma", 10F)
            lblPromptPayID.Location = New Point(3, 680)
            lblPromptPayID.Name = "lblPromptPayID"
            lblPromptPayID.Size = New Size(174, 40)
            lblPromptPayID.TabIndex = 25
            lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            lblPromptPayID.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayID
            ' 
            txtPromptPayID.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            txtPromptPayID.Font = New Font("Tahoma", 10F)
            txtPromptPayID.Location = New Point(183, 684)
            txtPromptPayID.Name = "txtPromptPayID"
            txtPromptPayID.Size = New Size(648, 32)
            txtPromptPayID.TabIndex = 26
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
            dgvPersonnel.Location = New Point(0, 0)
            dgvPersonnel.MultiSelect = False
            dgvPersonnel.Name = "dgvPersonnel"
            dgvPersonnel.ReadOnly = True
            dgvPersonnel.RowHeadersVisible = False
            dgvPersonnel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvPersonnel.Size = New Size(824, 265)
            dgvPersonnel.TabIndex = 0

            ' 
            ' flpButtons
            ' 
            flpButtons.BackColor = Color.FromArgb(CByte(248), CByte(250), CByte(252))
            flpButtons.Controls.Add(btnClose)
            flpButtons.Controls.Add(btnManagePersonnel)
            flpButtons.Controls.Add(btnLocationImport)
            flpButtons.Controls.Add(btnCancel)
            flpButtons.Controls.Add(btnSave)
            flpButtons.Dock = DockStyle.Fill
            flpButtons.FlowDirection = FlowDirection.RightToLeft
            flpButtons.Location = New Point(0, 1)
            flpButtons.Name = "flpButtons"
            flpButtons.Padding = New Padding(10, 14, 10, 14)
            flpButtons.Size = New Size(880, 69)
            flpButtons.TabIndex = 0
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(740, 14)
            btnClose.Margin = New Padding(5, 0, 5, 0)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(130, 42)
            btnClose.TabIndex = 3
            btnClose.Text = "❌ ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' btnManagePersonnel
            ' 
            btnManagePersonnel.BackColor = Color.FromArgb(CByte(147), CByte(51), CByte(234))
            btnManagePersonnel.FlatStyle = FlatStyle.Flat
            btnManagePersonnel.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnManagePersonnel.ForeColor = Color.White
            btnManagePersonnel.Location = New Point(575, 14)
            btnManagePersonnel.Margin = New Padding(5, 0, 5, 0)
            btnManagePersonnel.Name = "btnManagePersonnel"
            btnManagePersonnel.Size = New Size(160, 42)
            btnManagePersonnel.TabIndex = 4
            btnManagePersonnel.Text = "👤 จัดการรายชื่อ..."
            btnManagePersonnel.UseVisualStyleBackColor = False
            ' 
            ' btnLocationImport
            ' 
            btnLocationImport.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnLocationImport.FlatStyle = FlatStyle.Flat
            btnLocationImport.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnLocationImport.ForeColor = Color.White
            btnLocationImport.Location = New Point(410, 14)
            btnLocationImport.Margin = New Padding(5, 0, 5, 0)
            btnLocationImport.Name = "btnLocationImport"
            btnLocationImport.Size = New Size(160, 42)
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
            btnCancel.Location = New Point(275, 14)
            btnCancel.Margin = New Padding(5, 0, 5, 0)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(130, 42)
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
            btnSave.Location = New Point(120, 14)
            btnSave.Margin = New Padding(5, 0, 5, 0)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(150, 42)
            btnSave.TabIndex = 0
            btnSave.Text = "💾 บันทึกข้อมูล"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' FrmTempleSetting
            ' 
            AutoScaleDimensions = New SizeF(144F, 144F)
            AutoScaleMode = AutoScaleMode.Dpi
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(900, 817)
            Controls.Add(pMainContainer)
            Font = New Font("Tahoma", 10F)
            MinimumSize = New Size(800, 600)
            Name = "FrmTempleSetting"
            Text = "ตั้งค่าข้อมูลวัด"
            pMainContainer.ResumeLayout(False)
            pMainContainer.PerformLayout()
            pHeader.ResumeLayout(False)
            pContent.ResumeLayout(False)
            pContent.PerformLayout()
            pBottom.ResumeLayout(False)
            gbTempleInfo.ResumeLayout(False)
            gbTempleInfo.PerformLayout()
            tlpTempleInfo.ResumeLayout(False)
            tlpTempleInfo.PerformLayout()
            gbPersonnel.ResumeLayout(False)
            gbPersonnel.PerformLayout()
            tlpPersonnel.ResumeLayout(False)
            tlpPersonnel.PerformLayout()
            gbPersonnelList.ResumeLayout(False)
            tlpPersonnelList.ResumeLayout(False)
            CType(dgvPersonnel, ISupportInitialize).EndInit()
            flpButtons.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
