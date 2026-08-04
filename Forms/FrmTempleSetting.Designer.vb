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
        Friend WithEvents pMain As Panel

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
        Friend WithEvents lblAbbotOfficeStatus As Label
        Friend WithEvents lblWaiyawatName As Label
        Friend WithEvents lblWaiyawatOfficeStatus As Label
        Friend WithEvents lblBookkeeperName As Label
        Friend WithEvents lblBookkeeperType As Label
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
        Friend WithEvents cboAbbotOfficeStatus As ComboBox
        Friend WithEvents cboWaiyawatOfficeStatus As ComboBox
        Friend WithEvents cboBookkeeperType As ComboBox
        Friend WithEvents chkUsePromptPay As CheckBox

        ' Layout
        Friend WithEvents tlpFields As TableLayoutPanel
        Friend WithEvents flpButtons As FlowLayoutPanel

        ' Buttons
        Friend WithEvents pButtons As Panel
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents btnLocationImport As Button
        Friend WithEvents btnManagePersonnel As Button
        Friend WithEvents ttMain As ToolTip

        <DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                if disposing AndAlso components IsNot Nothing Then
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
            pMain = New Panel()
            tlpFields = New TableLayoutPanel()
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
            lblAbbotName = New Label()
            cboAbbotName = New ComboBox()
            lblAbbotOfficeStatus = New Label()
            cboAbbotOfficeStatus = New ComboBox()
            lblWaiyawatName = New Label()
            cboWaiyawatName = New ComboBox()
            lblWaiyawatOfficeStatus = New Label()
            cboWaiyawatOfficeStatus = New ComboBox()
            lblBookkeeperName = New Label()
            cboBookkeeperName = New ComboBox()
            lblBookkeeperType = New Label()
            cboBookkeeperType = New ComboBox()
            chkUsePromptPay = New CheckBox()
            lblPromptPayName = New Label()
            txtPromptPayName = New TextBox()
            lblPromptPayID = New Label()
            txtPromptPayID = New TextBox()
            flpButtons = New FlowLayoutPanel()
            btnClose = New Button()
            btnManagePersonnel = New Button()
            btnLocationImport = New Button()
            btnCancel = New Button()
            btnSave = New Button()
            pButtons = New Panel()
            pMain.SuspendLayout()
            tlpFields.SuspendLayout()
            flpButtons.SuspendLayout()
            pButtons.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 14F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1350, 42)
            lblHeader.TabIndex = 0
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' pMain
            ' 
            pMain.AutoScroll = True
            pMain.BackColor = Color.White
            pMain.Controls.Add(tlpFields)
            pMain.Dock = DockStyle.Fill
            pMain.Location = New Point(0, 42)
            pMain.Name = "pMain"
            pMain.Padding = New Padding(20)
            pMain.Size = New Size(1350, 718)
            pMain.TabIndex = 1
            ' 
            ' tlpFields
            ' 
            tlpFields.AutoSize = True
            tlpFields.ColumnCount = 2
            tlpFields.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 220F))
            tlpFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
            tlpFields.Controls.Add(lblTempleCode, 0, 0)
            tlpFields.Controls.Add(txtTempleCode, 1, 0)
            tlpFields.Controls.Add(lblTempleName, 0, 1)
            tlpFields.Controls.Add(txtTempleName, 1, 1)
            tlpFields.Controls.Add(lblTempleAddress, 0, 2)
            tlpFields.Controls.Add(txtTempleAddress, 1, 2)
            tlpFields.Controls.Add(lblProvince, 0, 3)
            tlpFields.Controls.Add(cboProvince, 1, 3)
            tlpFields.Controls.Add(lblAmphoe, 0, 4)
            tlpFields.Controls.Add(cboAmphoe, 1, 4)
            tlpFields.Controls.Add(lblTambon, 0, 5)
            tlpFields.Controls.Add(cboTambon, 1, 5)
            tlpFields.Controls.Add(lblPostCode, 0, 6)
            tlpFields.Controls.Add(txtPostCode, 1, 6)
            tlpFields.Controls.Add(lblTemplePhone, 0, 7)
            tlpFields.Controls.Add(txtTemplePhone, 1, 7)
            tlpFields.Controls.Add(lblAbbotName, 0, 8)
            tlpFields.Controls.Add(cboAbbotName, 1, 8)
            tlpFields.Controls.Add(lblAbbotOfficeStatus, 0, 9)
            tlpFields.Controls.Add(cboAbbotOfficeStatus, 1, 9)
            tlpFields.Controls.Add(lblWaiyawatName, 0, 10)
            tlpFields.Controls.Add(cboWaiyawatName, 1, 10)
            tlpFields.Controls.Add(lblWaiyawatOfficeStatus, 0, 11)
            tlpFields.Controls.Add(cboWaiyawatOfficeStatus, 1, 11)
            tlpFields.Controls.Add(lblBookkeeperName, 0, 12)
            tlpFields.Controls.Add(cboBookkeeperName, 1, 12)
            tlpFields.Controls.Add(lblBookkeeperType, 0, 13)
            tlpFields.Controls.Add(cboBookkeeperType, 1, 13)
            tlpFields.Controls.Add(chkUsePromptPay, 1, 14)
            tlpFields.Controls.Add(lblPromptPayName, 0, 15)
            tlpFields.Controls.Add(txtPromptPayName, 1, 15)
            tlpFields.Controls.Add(lblPromptPayID, 0, 16)
            tlpFields.Controls.Add(txtPromptPayID, 1, 16)
            tlpFields.Dock = DockStyle.Top
            tlpFields.Location = New Point(20, 20)
            tlpFields.Name = "tlpFields"
            tlpFields.RowCount = 17
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 20F))
            tlpFields.Size = New Size(1310, 340)
            tlpFields.TabIndex = 0
            ' 
            ' lblTempleCode
            ' 
            lblTempleCode.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblTempleCode.Font = New Font("Tahoma", 10F)
            lblTempleCode.Location = New Point(3, 0)
            lblTempleCode.Name = "lblTempleCode"
            lblTempleCode.Size = New Size(214, 20)
            lblTempleCode.TabIndex = 0
            lblTempleCode.Text = "รหัสวัด:"
            lblTempleCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleCode
            ' 
            txtTempleCode.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtTempleCode.Font = New Font("Tahoma", 10.5F)
            txtTempleCode.Location = New Point(223, 3)
            txtTempleCode.Name = "txtTempleCode"
            txtTempleCode.Size = New Size(1084, 33)
            txtTempleCode.TabIndex = 1
            ' 
            ' lblTempleName
            ' 
            lblTempleName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblTempleName.Font = New Font("Tahoma", 10F)
            lblTempleName.Location = New Point(3, 20)
            lblTempleName.Name = "lblTempleName"
            lblTempleName.Size = New Size(214, 20)
            lblTempleName.TabIndex = 2
            lblTempleName.Text = "ชื่อวัด:"
            lblTempleName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleName
            ' 
            txtTempleName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtTempleName.Font = New Font("Tahoma", 10.5F)
            txtTempleName.Location = New Point(223, 23)
            txtTempleName.Name = "txtTempleName"
            txtTempleName.Size = New Size(1084, 33)
            txtTempleName.TabIndex = 3
            ' 
            ' lblTempleAddress
            ' 
            lblTempleAddress.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblTempleAddress.Font = New Font("Tahoma", 10F)
            lblTempleAddress.Location = New Point(3, 40)
            lblTempleAddress.Name = "lblTempleAddress"
            lblTempleAddress.Size = New Size(214, 20)
            lblTempleAddress.TabIndex = 4
            lblTempleAddress.Text = "ที่อยู่วัด:"
            lblTempleAddress.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleAddress
            ' 
            txtTempleAddress.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtTempleAddress.Font = New Font("Tahoma", 10.5F)
            txtTempleAddress.Location = New Point(223, 43)
            txtTempleAddress.Multiline = True
            txtTempleAddress.Name = "txtTempleAddress"
            txtTempleAddress.ScrollBars = ScrollBars.Vertical
            txtTempleAddress.Size = New Size(1084, 14)
            txtTempleAddress.TabIndex = 5
            ' 
            ' lblProvince
            ' 
            lblProvince.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblProvince.Font = New Font("Tahoma", 10F)
            lblProvince.Location = New Point(3, 60)
            lblProvince.Name = "lblProvince"
            lblProvince.Size = New Size(214, 20)
            lblProvince.TabIndex = 6
            lblProvince.Text = "จังหวัด:"
            lblProvince.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboProvince
            ' 
            cboProvince.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboProvince.DropDownStyle = ComboBoxStyle.DropDownList
            cboProvince.Font = New Font("Tahoma", 10F)
            cboProvince.Location = New Point(223, 63)
            cboProvince.Name = "cboProvince"
            cboProvince.Size = New Size(1084, 32)
            cboProvince.TabIndex = 7
            ' 
            ' lblAmphoe
            ' 
            lblAmphoe.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblAmphoe.Font = New Font("Tahoma", 10F)
            lblAmphoe.Location = New Point(3, 80)
            lblAmphoe.Name = "lblAmphoe"
            lblAmphoe.Size = New Size(214, 20)
            lblAmphoe.TabIndex = 8
            lblAmphoe.Text = "อำเภอ:"
            lblAmphoe.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAmphoe
            ' 
            cboAmphoe.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboAmphoe.DropDownStyle = ComboBoxStyle.DropDownList
            cboAmphoe.Font = New Font("Tahoma", 10F)
            cboAmphoe.Location = New Point(223, 83)
            cboAmphoe.Name = "cboAmphoe"
            cboAmphoe.Size = New Size(1084, 32)
            cboAmphoe.TabIndex = 9
            ' 
            ' lblTambon
            ' 
            lblTambon.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblTambon.Font = New Font("Tahoma", 10F)
            lblTambon.Location = New Point(3, 100)
            lblTambon.Name = "lblTambon"
            lblTambon.Size = New Size(214, 20)
            lblTambon.TabIndex = 10
            lblTambon.Text = "ตำบล:"
            lblTambon.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboTambon
            ' 
            cboTambon.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboTambon.DropDownStyle = ComboBoxStyle.DropDownList
            cboTambon.Font = New Font("Tahoma", 10F)
            cboTambon.Location = New Point(223, 103)
            cboTambon.Name = "cboTambon"
            cboTambon.Size = New Size(1084, 32)
            cboTambon.TabIndex = 11
            ' 
            ' lblPostCode
            ' 
            lblPostCode.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblPostCode.Font = New Font("Tahoma", 10F)
            lblPostCode.Location = New Point(3, 120)
            lblPostCode.Name = "lblPostCode"
            lblPostCode.Size = New Size(214, 20)
            lblPostCode.TabIndex = 12
            lblPostCode.Text = "รหัสไปรษณีย์:"
            lblPostCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPostCode
            ' 
            txtPostCode.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtPostCode.Font = New Font("Tahoma", 10.5F)
            txtPostCode.Location = New Point(223, 123)
            txtPostCode.Name = "txtPostCode"
            txtPostCode.Size = New Size(1084, 33)
            txtPostCode.TabIndex = 13
            ' 
            ' lblTemplePhone
            ' 
            lblTemplePhone.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblTemplePhone.Font = New Font("Tahoma", 10F)
            lblTemplePhone.Location = New Point(3, 140)
            lblTemplePhone.Name = "lblTemplePhone"
            lblTemplePhone.Size = New Size(214, 20)
            lblTemplePhone.TabIndex = 14
            lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            lblTemplePhone.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTemplePhone
            ' 
            txtTemplePhone.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtTemplePhone.Font = New Font("Tahoma", 10.5F)
            txtTemplePhone.Location = New Point(223, 143)
            txtTemplePhone.Name = "txtTemplePhone"
            txtTemplePhone.Size = New Size(1084, 33)
            txtTemplePhone.TabIndex = 15
            ' 
            ' lblAbbotName
            ' 
            lblAbbotName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblAbbotName.Font = New Font("Tahoma", 10F)
            lblAbbotName.Location = New Point(3, 160)
            lblAbbotName.Name = "lblAbbotName"
            lblAbbotName.Size = New Size(214, 20)
            lblAbbotName.TabIndex = 16
            lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            lblAbbotName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAbbotName
            ' 
            cboAbbotName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboAbbotName.DropDownStyle = ComboBoxStyle.DropDownList
            cboAbbotName.Font = New Font("Tahoma", 10.5F)
            cboAbbotName.Location = New Point(223, 163)
            cboAbbotName.Name = "cboAbbotName"
            cboAbbotName.Size = New Size(1084, 33)
            cboAbbotName.TabIndex = 17
            ' 
            ' lblAbbotOfficeStatus
            ' 
            lblAbbotOfficeStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblAbbotOfficeStatus.Font = New Font("Tahoma", 10F)
            lblAbbotOfficeStatus.Location = New Point(3, 180)
            lblAbbotOfficeStatus.Name = "lblAbbotOfficeStatus"
            lblAbbotOfficeStatus.Size = New Size(214, 20)
            lblAbbotOfficeStatus.TabIndex = 18
            lblAbbotOfficeStatus.Text = "ตำแหน่งเจ้าอาวาส:"
            lblAbbotOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAbbotOfficeStatus
            ' 
            cboAbbotOfficeStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboAbbotOfficeStatus.Font = New Font("Tahoma", 10F)
            cboAbbotOfficeStatus.Items.AddRange(New Object() {"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})
            cboAbbotOfficeStatus.Location = New Point(223, 183)
            cboAbbotOfficeStatus.Name = "cboAbbotOfficeStatus"
            cboAbbotOfficeStatus.Size = New Size(1084, 32)
            cboAbbotOfficeStatus.TabIndex = 19
            ' 
            ' lblWaiyawatName
            ' 
            lblWaiyawatName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblWaiyawatName.Font = New Font("Tahoma", 10F)
            lblWaiyawatName.Location = New Point(3, 200)
            lblWaiyawatName.Name = "lblWaiyawatName"
            lblWaiyawatName.Size = New Size(214, 20)
            lblWaiyawatName.TabIndex = 20
            lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            lblWaiyawatName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboWaiyawatName
            ' 
            cboWaiyawatName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboWaiyawatName.DropDownStyle = ComboBoxStyle.DropDownList
            cboWaiyawatName.Font = New Font("Tahoma", 10.5F)
            cboWaiyawatName.Location = New Point(223, 203)
            cboWaiyawatName.Name = "cboWaiyawatName"
            cboWaiyawatName.Size = New Size(1084, 33)
            cboWaiyawatName.TabIndex = 21
            ' 
            ' lblWaiyawatOfficeStatus
            ' 
            lblWaiyawatOfficeStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblWaiyawatOfficeStatus.Font = New Font("Tahoma", 10F)
            lblWaiyawatOfficeStatus.Location = New Point(3, 220)
            lblWaiyawatOfficeStatus.Name = "lblWaiyawatOfficeStatus"
            lblWaiyawatOfficeStatus.Size = New Size(214, 20)
            lblWaiyawatOfficeStatus.TabIndex = 22
            lblWaiyawatOfficeStatus.Text = "ตำแหน่งไวยาวัจกร:"
            lblWaiyawatOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboWaiyawatOfficeStatus
            ' 
            cboWaiyawatOfficeStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboWaiyawatOfficeStatus.Font = New Font("Tahoma", 10F)
            cboWaiyawatOfficeStatus.Items.AddRange(New Object() {"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})
            cboWaiyawatOfficeStatus.Location = New Point(223, 223)
            cboWaiyawatOfficeStatus.Name = "cboWaiyawatOfficeStatus"
            cboWaiyawatOfficeStatus.Size = New Size(1084, 32)
            cboWaiyawatOfficeStatus.TabIndex = 23
            ' 
            ' lblBookkeeperName
            ' 
            lblBookkeeperName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblBookkeeperName.Font = New Font("Tahoma", 10F)
            lblBookkeeperName.Location = New Point(3, 240)
            lblBookkeeperName.Name = "lblBookkeeperName"
            lblBookkeeperName.Size = New Size(214, 20)
            lblBookkeeperName.TabIndex = 24
            lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            lblBookkeeperName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboBookkeeperName
            ' 
            cboBookkeeperName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboBookkeeperName.DropDownStyle = ComboBoxStyle.DropDownList
            cboBookkeeperName.Font = New Font("Tahoma", 10.5F)
            cboBookkeeperName.Location = New Point(223, 243)
            cboBookkeeperName.Name = "cboBookkeeperName"
            cboBookkeeperName.Size = New Size(1084, 33)
            cboBookkeeperName.TabIndex = 25
            ' 
            ' lblBookkeeperType
            ' 
            lblBookkeeperType.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblBookkeeperType.Font = New Font("Tahoma", 10F)
            lblBookkeeperType.Location = New Point(3, 260)
            lblBookkeeperType.Name = "lblBookkeeperType"
            lblBookkeeperType.Size = New Size(214, 20)
            lblBookkeeperType.TabIndex = 26
            lblBookkeeperType.Text = "ประเภทผู้ทำบัญชี:"
            lblBookkeeperType.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboBookkeeperType
            ' 
            cboBookkeeperType.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            cboBookkeeperType.Font = New Font("Tahoma", 10F)
            cboBookkeeperType.Items.AddRange(New Object() {"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})
            cboBookkeeperType.Location = New Point(223, 263)
            cboBookkeeperType.Name = "cboBookkeeperType"
            cboBookkeeperType.Size = New Size(1084, 32)
            cboBookkeeperType.TabIndex = 27
            ' 
            ' chkUsePromptPay
            ' 
            chkUsePromptPay.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            chkUsePromptPay.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            chkUsePromptPay.ForeColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            chkUsePromptPay.Location = New Point(223, 283)
            chkUsePromptPay.Name = "chkUsePromptPay"
            chkUsePromptPay.Size = New Size(1084, 14)
            chkUsePromptPay.TabIndex = 28
            chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"
            ' 
            ' lblPromptPayName
            ' 
            lblPromptPayName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblPromptPayName.Font = New Font("Tahoma", 10F)
            lblPromptPayName.Location = New Point(3, 300)
            lblPromptPayName.Name = "lblPromptPayName"
            lblPromptPayName.Size = New Size(214, 20)
            lblPromptPayName.TabIndex = 29
            lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            lblPromptPayName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayName
            ' 
            txtPromptPayName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtPromptPayName.Font = New Font("Tahoma", 10.5F)
            txtPromptPayName.Location = New Point(223, 303)
            txtPromptPayName.Name = "txtPromptPayName"
            txtPromptPayName.Size = New Size(1084, 33)
            txtPromptPayName.TabIndex = 30
            ' 
            ' lblPromptPayID
            ' 
            lblPromptPayID.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            lblPromptPayID.Font = New Font("Tahoma", 10F)
            lblPromptPayID.Location = New Point(3, 320)
            lblPromptPayID.Name = "lblPromptPayID"
            lblPromptPayID.Size = New Size(214, 20)
            lblPromptPayID.TabIndex = 31
            lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            lblPromptPayID.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayID
            ' 
            txtPromptPayID.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtPromptPayID.Font = New Font("Tahoma", 10.5F)
            txtPromptPayID.Location = New Point(223, 323)
            txtPromptPayID.Name = "txtPromptPayID"
            txtPromptPayID.Size = New Size(1084, 33)
            txtPromptPayID.TabIndex = 32
            ' 
            ' flpButtons
            ' 
            flpButtons.Controls.Add(btnClose)
            flpButtons.Controls.Add(btnManagePersonnel)
            flpButtons.Controls.Add(btnLocationImport)
            flpButtons.Controls.Add(btnCancel)
            flpButtons.Controls.Add(btnSave)
            flpButtons.Dock = DockStyle.Fill
            flpButtons.FlowDirection = FlowDirection.RightToLeft
            flpButtons.Location = New Point(0, 0)
            flpButtons.Name = "flpButtons"
            flpButtons.Padding = New Padding(10, 25, 10, 0)
            flpButtons.Size = New Size(1350, 100)
            flpButtons.TabIndex = 0
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1227, 28)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(100, 50)
            btnClose.TabIndex = 3
            btnClose.Text = "ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' btnManagePersonnel
            ' 
            btnManagePersonnel.BackColor = Color.FromArgb(CByte(147), CByte(51), CByte(234))
            btnManagePersonnel.FlatStyle = FlatStyle.Flat
            btnManagePersonnel.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnManagePersonnel.ForeColor = Color.White
            btnManagePersonnel.Location = New Point(1001, 28)
            btnManagePersonnel.Name = "btnManagePersonnel"
            btnManagePersonnel.Size = New Size(220, 50)
            btnManagePersonnel.TabIndex = 4
            btnManagePersonnel.Text = "👤 จัดการรายชื่อบุคลากร..."
            btnManagePersonnel.UseVisualStyleBackColor = False
            ' 
            ' btnLocationImport
            ' 
            btnLocationImport.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnLocationImport.FlatStyle = FlatStyle.Flat
            btnLocationImport.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnLocationImport.ForeColor = Color.White
            btnLocationImport.Location = New Point(736, 28)
            btnLocationImport.Name = "btnLocationImport"
            btnLocationImport.Size = New Size(259, 50)
            btnLocationImport.TabIndex = 2
            btnLocationImport.Text = "📍 นำเข้าจังหวัด/อำเภอ"
            btnLocationImport.UseVisualStyleBackColor = False
            ' 
            ' btnCancel
            ' 
            btnCancel.BackColor = Color.FromArgb(CByte(217), CByte(119), CByte(6))
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnCancel.ForeColor = Color.White
            btnCancel.Location = New Point(580, 28)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(150, 50)
            btnCancel.TabIndex = 1
            btnCancel.Text = "🔄 โหลดใหม่"
            btnCancel.UseVisualStyleBackColor = False
            ' 
            ' btnSave
            ' 
            btnSave.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnSave.ForeColor = Color.White
            btnSave.Location = New Point(374, 28)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(200, 50)
            btnSave.TabIndex = 0
            btnSave.Text = "💾 บันทึกข้อมูล"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' pButtons
            ' 
            pButtons.BackColor = Color.FromArgb(CByte(245), CByte(245), CByte(240))
            pButtons.Controls.Add(flpButtons)
            pButtons.Dock = DockStyle.Bottom
            pButtons.Location = New Point(0, 760)
            pButtons.Name = "pButtons"
            pButtons.Size = New Size(1350, 100)
            pButtons.TabIndex = 2
            ' 
            ' FrmTempleSetting
            ' 
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1350, 860)
            Controls.Add(pMain)
            Controls.Add(pButtons)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            Name = "FrmTempleSetting"
            Text = "ตั้งค่าข้อมูลวัด"
            pMain.ResumeLayout(False)
            pMain.PerformLayout()
            tlpFields.ResumeLayout(False)
            tlpFields.PerformLayout()
            flpButtons.ResumeLayout(False)
            pButtons.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
