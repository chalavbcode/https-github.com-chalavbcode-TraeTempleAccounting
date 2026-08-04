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
            Me.lblHeader = New System.Windows.Forms.Label()
            Me.pMain = New System.Windows.Forms.Panel()
            Me.tlpFields = New System.Windows.Forms.TableLayoutPanel()
            Me.lblTempleCode = New System.Windows.Forms.Label()
            Me.txtTempleCode = New System.Windows.Forms.TextBox()
            Me.lblTempleName = New System.Windows.Forms.Label()
            Me.txtTempleName = New System.Windows.Forms.TextBox()
            Me.lblTempleAddress = New System.Windows.Forms.Label()
            Me.txtTempleAddress = New System.Windows.Forms.TextBox()
            Me.lblProvince = New System.Windows.Forms.Label()
            Me.cboProvince = New System.Windows.Forms.ComboBox()
            Me.lblAmphoe = New System.Windows.Forms.Label()
            Me.cboAmphoe = New System.Windows.Forms.ComboBox()
            Me.lblTambon = New System.Windows.Forms.Label()
            Me.cboTambon = New System.Windows.Forms.ComboBox()
            Me.lblPostCode = New System.Windows.Forms.Label()
            Me.txtPostCode = New System.Windows.Forms.TextBox()
            Me.lblTemplePhone = New System.Windows.Forms.Label()
            Me.txtTemplePhone = New System.Windows.Forms.TextBox()
            Me.lblAbbotName = New System.Windows.Forms.Label()
            Me.cboAbbotName = New System.Windows.Forms.ComboBox()
            Me.lblAbbotOfficeStatus = New System.Windows.Forms.Label()
            Me.cboAbbotOfficeStatus = New System.Windows.Forms.ComboBox()
            Me.lblWaiyawatName = New System.Windows.Forms.Label()
            Me.cboWaiyawatName = New System.Windows.Forms.ComboBox()
            Me.lblWaiyawatOfficeStatus = New System.Windows.Forms.Label()
            Me.cboWaiyawatOfficeStatus = New System.Windows.Forms.ComboBox()
            Me.lblBookkeeperName = New System.Windows.Forms.Label()
            Me.cboBookkeeperName = New System.Windows.Forms.ComboBox()
            Me.lblBookkeeperType = New System.Windows.Forms.Label()
            Me.cboBookkeeperType = New System.Windows.Forms.ComboBox()
            Me.chkUsePromptPay = New System.Windows.Forms.CheckBox()
            Me.lblPromptPayName = New System.Windows.Forms.Label()
            Me.txtPromptPayName = New System.Windows.Forms.TextBox()
            Me.lblPromptPayID = New System.Windows.Forms.Label()
            Me.txtPromptPayID = New System.Windows.Forms.TextBox()
            Me.pButtons = New System.Windows.Forms.Panel()
            Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnManagePersonnel = New System.Windows.Forms.Button()
            Me.btnLocationImport = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.pMain.SuspendLayout()
            Me.tlpFields.SuspendLayout()
            Me.pButtons.SuspendLayout()
            Me.flpButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblHeader
            '
            Me.lblHeader.BackColor = System.Drawing.Color.FromArgb(253, 230, 138)
            Me.lblHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblHeader.Font = New System.Drawing.Font("Tahoma", 14.0!, System.Drawing.FontStyle.Bold)
            Me.lblHeader.ForeColor = System.Drawing.Color.FromArgb(69, 26, 3)
            Me.lblHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New System.Drawing.Size(900, 42)
            Me.lblHeader.TabIndex = 0
            Me.lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'pMain
            '
            Me.pMain.AutoScroll = True
            Me.pMain.BackColor = System.Drawing.Color.White
            Me.pMain.Controls.Add(Me.tlpFields)
            Me.pMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pMain.Location = New System.Drawing.Point(0, 42)
            Me.pMain.Name = "pMain"
            Me.pMain.Padding = New System.Windows.Forms.Padding(20)
            Me.pMain.Size = New System.Drawing.Size(900, 508)
            Me.pMain.TabIndex = 1
            '
            'tlpFields
            '
            Me.tlpFields.AutoSize = True
            Me.tlpFields.ColumnCount = 2
            Me.tlpFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180.0!))
            Me.tlpFields.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpFields.Controls.Add(Me.lblTempleCode, 0, 0)
            Me.tlpFields.Controls.Add(Me.txtTempleCode, 1, 0)
            Me.tlpFields.Controls.Add(Me.lblTempleName, 0, 1)
            Me.tlpFields.Controls.Add(Me.txtTempleName, 1, 1)
            Me.tlpFields.Controls.Add(Me.lblTempleAddress, 0, 2)
            Me.tlpFields.Controls.Add(Me.txtTempleAddress, 1, 2)
            Me.tlpFields.Controls.Add(Me.lblProvince, 0, 3)
            Me.tlpFields.Controls.Add(Me.cboProvince, 1, 3)
            Me.tlpFields.Controls.Add(Me.lblAmphoe, 0, 4)
            Me.tlpFields.Controls.Add(Me.cboAmphoe, 1, 4)
            Me.tlpFields.Controls.Add(Me.lblTambon, 0, 5)
            Me.tlpFields.Controls.Add(Me.cboTambon, 1, 5)
            Me.tlpFields.Controls.Add(Me.lblPostCode, 0, 6)
            Me.tlpFields.Controls.Add(Me.txtPostCode, 1, 6)
            Me.tlpFields.Controls.Add(Me.lblTemplePhone, 0, 7)
            Me.tlpFields.Controls.Add(Me.txtTemplePhone, 1, 7)
            Me.tlpFields.Controls.Add(Me.lblAbbotName, 0, 8)
            Me.tlpFields.Controls.Add(Me.cboAbbotName, 1, 8)
            Me.tlpFields.Controls.Add(Me.lblAbbotOfficeStatus, 0, 9)
            Me.tlpFields.Controls.Add(Me.cboAbbotOfficeStatus, 1, 9)
            Me.tlpFields.Controls.Add(Me.lblWaiyawatName, 0, 10)
            Me.tlpFields.Controls.Add(Me.cboWaiyawatName, 1, 10)
            Me.tlpFields.Controls.Add(Me.lblWaiyawatOfficeStatus, 0, 11)
            Me.tlpFields.Controls.Add(Me.cboWaiyawatOfficeStatus, 1, 11)
            Me.tlpFields.Controls.Add(Me.lblBookkeeperName, 0, 12)
            Me.tlpFields.Controls.Add(Me.cboBookkeeperName, 1, 12)
            Me.tlpFields.Controls.Add(Me.lblBookkeeperType, 0, 13)
            Me.tlpFields.Controls.Add(Me.cboBookkeeperType, 1, 13)
            Me.tlpFields.Controls.Add(Me.chkUsePromptPay, 1, 14)
            Me.tlpFields.Controls.Add(Me.lblPromptPayName, 0, 15)
            Me.tlpFields.Controls.Add(Me.txtPromptPayName, 1, 15)
            Me.tlpFields.Controls.Add(Me.lblPromptPayID, 0, 16)
            Me.tlpFields.Controls.Add(Me.txtPromptPayID, 1, 16)
            Me.tlpFields.Dock = System.Windows.Forms.DockStyle.Top
            Me.tlpFields.Location = New System.Drawing.Point(20, 20)
            Me.tlpFields.Name = "tlpFields"
            Me.tlpFields.RowCount = 17
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80.0!)) ' Address
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
            Me.tlpFields.Size = New System.Drawing.Size(840, 720)
            Me.tlpFields.TabIndex = 0
            '
            'lblTempleCode
            '
            Me.lblTempleCode.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblTempleCode.AutoSize = True
            Me.lblTempleCode.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTempleCode.Location = New System.Drawing.Point(3, 9)
            Me.lblTempleCode.Name = "lblTempleCode"
            Me.lblTempleCode.Size = New System.Drawing.Size(174, 21)
            Me.lblTempleCode.TabIndex = 0
            Me.lblTempleCode.Text = "รหัสวัด:"
            Me.lblTempleCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtTempleCode
            '
            Me.txtTempleCode.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtTempleCode.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtTempleCode.Location = New System.Drawing.Point(183, 6)
            Me.txtTempleCode.Name = "txtTempleCode"
            Me.txtTempleCode.Size = New System.Drawing.Size(654, 28)
            Me.txtTempleCode.TabIndex = 1
            '
            'lblTempleName
            '
            Me.lblTempleName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblTempleName.AutoSize = True
            Me.lblTempleName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTempleName.Location = New System.Drawing.Point(3, 49)
            Me.lblTempleName.Name = "lblTempleName"
            Me.lblTempleName.Size = New System.Drawing.Size(174, 21)
            Me.lblTempleName.TabIndex = 2
            Me.lblTempleName.Text = "ชื่อวัด:"
            Me.lblTempleName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtTempleName
            '
            Me.txtTempleName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtTempleName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtTempleName.Location = New System.Drawing.Point(183, 46)
            Me.txtTempleName.Name = "txtTempleName"
            Me.txtTempleName.Size = New System.Drawing.Size(654, 28)
            Me.txtTempleName.TabIndex = 3
            '
            'lblTempleAddress
            '
            Me.lblTempleAddress.Anchor = CType(System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblTempleAddress.AutoSize = True
            Me.lblTempleAddress.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTempleAddress.Location = New System.Drawing.Point(3, 80)
            Me.lblTempleAddress.Name = "lblTempleAddress"
            Me.lblTempleAddress.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
            Me.lblTempleAddress.Size = New System.Drawing.Size(174, 31)
            Me.lblTempleAddress.TabIndex = 4
            Me.lblTempleAddress.Text = "ที่อยู่วัด:"
            Me.lblTempleAddress.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'txtTempleAddress
            '
            Me.txtTempleAddress.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtTempleAddress.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtTempleAddress.Location = New System.Drawing.Point(183, 85)
            Me.txtTempleAddress.Multiline = True
            Me.txtTempleAddress.Name = "txtTempleAddress"
            Me.txtTempleAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txtTempleAddress.Size = New System.Drawing.Size(654, 70)
            Me.txtTempleAddress.TabIndex = 5
            '
            'lblProvince
            '
            Me.lblProvince.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblProvince.AutoSize = True
            Me.lblProvince.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblProvince.Location = New System.Drawing.Point(3, 169)
            Me.lblProvince.Name = "lblProvince"
            Me.lblProvince.Size = New System.Drawing.Size(174, 21)
            Me.lblProvince.TabIndex = 6
            Me.lblProvince.Text = "จังหวัด:"
            Me.lblProvince.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboProvince
            '
            Me.cboProvince.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboProvince.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboProvince.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboProvince.Location = New System.Drawing.Point(183, 165)
            Me.cboProvince.Name = "cboProvince"
            Me.cboProvince.Size = New System.Drawing.Size(654, 29)
            Me.cboProvince.TabIndex = 7
            '
            'lblAmphoe
            '
            Me.lblAmphoe.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblAmphoe.AutoSize = True
            Me.lblAmphoe.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblAmphoe.Location = New System.Drawing.Point(3, 209)
            Me.lblAmphoe.Name = "lblAmphoe"
            Me.lblAmphoe.Size = New System.Drawing.Size(174, 21)
            Me.lblAmphoe.TabIndex = 8
            Me.lblAmphoe.Text = "อำเภอ:"
            Me.lblAmphoe.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboAmphoe
            '
            Me.cboAmphoe.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboAmphoe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboAmphoe.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboAmphoe.Location = New System.Drawing.Point(183, 205)
            Me.cboAmphoe.Name = "cboAmphoe"
            Me.cboAmphoe.Size = New System.Drawing.Size(654, 29)
            Me.cboAmphoe.TabIndex = 9
            '
            'lblTambon
            '
            Me.lblTambon.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblTambon.AutoSize = True
            Me.lblTambon.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTambon.Location = New System.Drawing.Point(3, 249)
            Me.lblTambon.Name = "lblTambon"
            Me.lblTambon.Size = New System.Drawing.Size(174, 21)
            Me.lblTambon.TabIndex = 10
            Me.lblTambon.Text = "ตำบล:"
            Me.lblTambon.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboTambon
            '
            Me.cboTambon.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboTambon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboTambon.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboTambon.Location = New System.Drawing.Point(183, 245)
            Me.cboTambon.Name = "cboTambon"
            Me.cboTambon.Size = New System.Drawing.Size(654, 29)
            Me.cboTambon.TabIndex = 11
            '
            'lblPostCode
            '
            Me.lblPostCode.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblPostCode.AutoSize = True
            Me.lblPostCode.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPostCode.Location = New System.Drawing.Point(3, 289)
            Me.lblPostCode.Name = "lblPostCode"
            Me.lblPostCode.Size = New System.Drawing.Size(174, 21)
            Me.lblPostCode.TabIndex = 12
            Me.lblPostCode.Text = "รหัสไปรษณีย์:"
            Me.lblPostCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtPostCode
            '
            Me.txtPostCode.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtPostCode.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtPostCode.Location = New System.Drawing.Point(183, 286)
            Me.txtPostCode.Name = "txtPostCode"
            Me.txtPostCode.Size = New System.Drawing.Size(654, 28)
            Me.txtPostCode.TabIndex = 13
            '
            'lblTemplePhone
            '
            Me.lblTemplePhone.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblTemplePhone.AutoSize = True
            Me.lblTemplePhone.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTemplePhone.Location = New System.Drawing.Point(3, 329)
            Me.lblTemplePhone.Name = "lblTemplePhone"
            Me.lblTemplePhone.Size = New System.Drawing.Size(174, 21)
            Me.lblTemplePhone.TabIndex = 14
            Me.lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            Me.lblTemplePhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtTemplePhone
            '
            Me.txtTemplePhone.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtTemplePhone.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtTemplePhone.Location = New System.Drawing.Point(183, 326)
            Me.txtTemplePhone.Name = "txtTemplePhone"
            Me.txtTemplePhone.Size = New System.Drawing.Size(654, 28)
            Me.txtTemplePhone.TabIndex = 15
            '
            'lblAbbotName
            '
            Me.lblAbbotName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblAbbotName.AutoSize = True
            Me.lblAbbotName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblAbbotName.Location = New System.Drawing.Point(3, 369)
            Me.lblAbbotName.Name = "lblAbbotName"
            Me.lblAbbotName.Size = New System.Drawing.Size(174, 21)
            Me.lblAbbotName.TabIndex = 16
            Me.lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            Me.lblAbbotName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboAbbotName
            '
            Me.cboAbbotName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboAbbotName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboAbbotName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboAbbotName.Location = New System.Drawing.Point(183, 365)
            Me.cboAbbotName.Name = "cboAbbotName"
            Me.cboAbbotName.Size = New System.Drawing.Size(654, 29)
            Me.cboAbbotName.TabIndex = 17
            '
            'lblAbbotOfficeStatus
            '
            Me.lblAbbotOfficeStatus.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblAbbotOfficeStatus.AutoSize = True
            Me.lblAbbotOfficeStatus.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblAbbotOfficeStatus.Location = New System.Drawing.Point(3, 409)
            Me.lblAbbotOfficeStatus.Name = "lblAbbotOfficeStatus"
            Me.lblAbbotOfficeStatus.Size = New System.Drawing.Size(174, 21)
            Me.lblAbbotOfficeStatus.TabIndex = 18
            Me.lblAbbotOfficeStatus.Text = "ตำแหน่งเจ้าอาวาส:"
            Me.lblAbbotOfficeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboAbbotOfficeStatus
            '
            Me.cboAbbotOfficeStatus.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboAbbotOfficeStatus.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboAbbotOfficeStatus.Items.AddRange(New Object() {"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})
            Me.cboAbbotOfficeStatus.Location = New System.Drawing.Point(183, 405)
            Me.cboAbbotOfficeStatus.Name = "cboAbbotOfficeStatus"
            Me.cboAbbotOfficeStatus.Size = New System.Drawing.Size(654, 29)
            Me.cboAbbotOfficeStatus.TabIndex = 19
            '
            'lblWaiyawatName
            '
            Me.lblWaiyawatName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblWaiyawatName.AutoSize = True
            Me.lblWaiyawatName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblWaiyawatName.Location = New System.Drawing.Point(3, 449)
            Me.lblWaiyawatName.Name = "lblWaiyawatName"
            Me.lblWaiyawatName.Size = New System.Drawing.Size(174, 21)
            Me.lblWaiyawatName.TabIndex = 20
            Me.lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            Me.lblWaiyawatName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboWaiyawatName
            '
            Me.cboWaiyawatName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboWaiyawatName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboWaiyawatName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboWaiyawatName.Location = New System.Drawing.Point(183, 445)
            Me.cboWaiyawatName.Name = "cboWaiyawatName"
            Me.cboWaiyawatName.Size = New System.Drawing.Size(654, 29)
            Me.cboWaiyawatName.TabIndex = 21
            '
            'lblWaiyawatOfficeStatus
            '
            Me.lblWaiyawatOfficeStatus.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblWaiyawatOfficeStatus.AutoSize = True
            Me.lblWaiyawatOfficeStatus.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblWaiyawatOfficeStatus.Location = New System.Drawing.Point(3, 489)
            Me.lblWaiyawatOfficeStatus.Name = "lblWaiyawatOfficeStatus"
            Me.lblWaiyawatOfficeStatus.Size = New System.Drawing.Size(174, 21)
            Me.lblWaiyawatOfficeStatus.TabIndex = 22
            Me.lblWaiyawatOfficeStatus.Text = "ตำแหน่งไวยาวัจกร:"
            Me.lblWaiyawatOfficeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboWaiyawatOfficeStatus
            '
            Me.cboWaiyawatOfficeStatus.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboWaiyawatOfficeStatus.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboWaiyawatOfficeStatus.Items.AddRange(New Object() {"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})
            Me.cboWaiyawatOfficeStatus.Location = New System.Drawing.Point(183, 485)
            Me.cboWaiyawatOfficeStatus.Name = "cboWaiyawatOfficeStatus"
            Me.cboWaiyawatOfficeStatus.Size = New System.Drawing.Size(654, 29)
            Me.cboWaiyawatOfficeStatus.TabIndex = 23
            '
            'lblBookkeeperName
            '
            Me.lblBookkeeperName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblBookkeeperName.AutoSize = True
            Me.lblBookkeeperName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblBookkeeperName.Location = New System.Drawing.Point(3, 529)
            Me.lblBookkeeperName.Name = "lblBookkeeperName"
            Me.lblBookkeeperName.Size = New System.Drawing.Size(174, 21)
            Me.lblBookkeeperName.TabIndex = 24
            Me.lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            Me.lblBookkeeperName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboBookkeeperName
            '
            Me.cboBookkeeperName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboBookkeeperName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboBookkeeperName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboBookkeeperName.Location = New System.Drawing.Point(183, 525)
            Me.cboBookkeeperName.Name = "cboBookkeeperName"
            Me.cboBookkeeperName.Size = New System.Drawing.Size(654, 29)
            Me.cboBookkeeperName.TabIndex = 25
            '
            'lblBookkeeperType
            '
            Me.lblBookkeeperType.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblBookkeeperType.AutoSize = True
            Me.lblBookkeeperType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblBookkeeperType.Location = New System.Drawing.Point(3, 569)
            Me.lblBookkeeperType.Name = "lblBookkeeperType"
            Me.lblBookkeeperType.Size = New System.Drawing.Size(174, 21)
            Me.lblBookkeeperType.TabIndex = 26
            Me.lblBookkeeperType.Text = "ประเภทผู้ทำบัญชี:"
            Me.lblBookkeeperType.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'cboBookkeeperType
            '
            Me.cboBookkeeperType.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboBookkeeperType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboBookkeeperType.Items.AddRange(New Object() {"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})
            Me.cboBookkeeperType.Location = New System.Drawing.Point(183, 565)
            Me.cboBookkeeperType.Name = "cboBookkeeperType"
            Me.cboBookkeeperType.Size = New System.Drawing.Size(654, 29)
            Me.cboBookkeeperType.TabIndex = 27
            '
            'chkUsePromptPay
            '
            Me.chkUsePromptPay.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.chkUsePromptPay.AutoSize = True
            Me.chkUsePromptPay.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.chkUsePromptPay.ForeColor = System.Drawing.Color.FromArgb(15, 118, 110)
            Me.chkUsePromptPay.Location = New System.Drawing.Point(183, 607)
            Me.chkUsePromptPay.Name = "chkUsePromptPay"
            Me.chkUsePromptPay.Size = New System.Drawing.Size(654, 25)
            Me.chkUsePromptPay.TabIndex = 28
            Me.chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"
            Me.chkUsePromptPay.UseVisualStyleBackColor = True
            '
            'lblPromptPayName
            '
            Me.lblPromptPayName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblPromptPayName.AutoSize = True
            Me.lblPromptPayName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPromptPayName.Location = New System.Drawing.Point(3, 649)
            Me.lblPromptPayName.Name = "lblPromptPayName"
            Me.lblPromptPayName.Size = New System.Drawing.Size(174, 21)
            Me.lblPromptPayName.TabIndex = 29
            Me.lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            Me.lblPromptPayName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtPromptPayName
            '
            Me.txtPromptPayName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtPromptPayName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtPromptPayName.Location = New System.Drawing.Point(183, 646)
            Me.txtPromptPayName.Name = "txtPromptPayName"
            Me.txtPromptPayName.Size = New System.Drawing.Size(654, 28)
            Me.txtPromptPayName.TabIndex = 30
            '
            'lblPromptPayID
            '
            Me.lblPromptPayID.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.lblPromptPayID.AutoSize = True
            Me.lblPromptPayID.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPromptPayID.Location = New System.Drawing.Point(3, 689)
            Me.lblPromptPayID.Name = "lblPromptPayID"
            Me.lblPromptPayID.Size = New System.Drawing.Size(174, 21)
            Me.lblPromptPayID.TabIndex = 31
            Me.lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            Me.lblPromptPayID.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'txtPromptPayID
            '
            Me.txtPromptPayID.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtPromptPayID.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtPromptPayID.Location = New System.Drawing.Point(183, 686)
            Me.txtPromptPayID.Name = "txtPromptPayID"
            Me.txtPromptPayID.Size = New System.Drawing.Size(654, 28)
            Me.txtPromptPayID.TabIndex = 32
            '
            'pButtons
            '
            Me.pButtons.BackColor = System.Drawing.Color.FromArgb(245, 245, 240)
            Me.pButtons.Controls.Add(Me.flpButtons)
            Me.pButtons.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pButtons.Location = New System.Drawing.Point(0, 550)
            Me.pButtons.Name = "pButtons"
            Me.pButtons.Size = New System.Drawing.Size(900, 70)
            Me.pButtons.TabIndex = 2
            '
            'flpButtons
            '
            Me.flpButtons.Controls.Add(Me.btnClose)
            Me.flpButtons.Controls.Add(Me.btnManagePersonnel)
            Me.flpButtons.Controls.Add(Me.btnLocationImport)
            Me.flpButtons.Controls.Add(Me.btnCancel)
            Me.flpButtons.Controls.Add(Me.btnSave)
            Me.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            Me.flpButtons.Location = New System.Drawing.Point(0, 0)
            Me.flpButtons.Name = "flpButtons"
            Me.flpButtons.Padding = New System.Windows.Forms.Padding(10, 15, 10, 15)
            Me.flpButtons.Size = New System.Drawing.Size(900, 70)
            Me.flpButtons.TabIndex = 0
            '
            'btnClose
            '
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(75, 85, 99)
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(757, 18)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(130, 40)
            Me.btnClose.TabIndex = 3
            Me.btnClose.Text = "ปิด"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnManagePersonnel
            '
            Me.btnManagePersonnel.BackColor = System.Drawing.Color.FromArgb(147, 51, 234)
            Me.btnManagePersonnel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnManagePersonnel.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnManagePersonnel.ForeColor = System.Drawing.Color.White
            Me.btnManagePersonnel.Location = New System.Drawing.Point(591, 18)
            Me.btnManagePersonnel.Name = "btnManagePersonnel"
            Me.btnManagePersonnel.Size = New System.Drawing.Size(160, 40)
            Me.btnManagePersonnel.TabIndex = 4
            Me.btnManagePersonnel.Text = "👤 จัดการรายชื่อ..."
            Me.btnManagePersonnel.UseVisualStyleBackColor = False
            '
            'btnLocationImport
            '
            Me.btnLocationImport.BackColor = System.Drawing.Color.FromArgb(37, 99, 235)
            Me.btnLocationImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnLocationImport.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnLocationImport.ForeColor = System.Drawing.Color.White
            Me.btnLocationImport.Location = New System.Drawing.Point(425, 18)
            Me.btnLocationImport.Name = "btnLocationImport"
            Me.btnLocationImport.Size = New System.Drawing.Size(160, 40)
            Me.btnLocationImport.TabIndex = 2
            Me.btnLocationImport.Text = "📍 นำเข้าที่อยู่"
            Me.btnLocationImport.UseVisualStyleBackColor = False
            '
            'btnCancel
            '
            Me.btnCancel.BackColor = System.Drawing.Color.FromArgb(217, 119, 6)
            Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnCancel.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnCancel.ForeColor = System.Drawing.Color.White
            Me.btnCancel.Location = New System.Drawing.Point(289, 18)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(130, 40)
            Me.btnCancel.TabIndex = 1
            Me.btnCancel.Text = "🔄 โหลดใหม่"
            Me.btnCancel.UseVisualStyleBackColor = False
            '
            'btnSave
            '
            Me.btnSave.BackColor = System.Drawing.Color.FromArgb(22, 163, 74)
            Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSave.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnSave.ForeColor = System.Drawing.Color.White
            Me.btnSave.Location = New System.Drawing.Point(133, 18)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(150, 40)
            Me.btnSave.TabIndex = 0
            Me.btnSave.Text = "💾 บันทึกข้อมูล"
            Me.btnSave.UseVisualStyleBackColor = False
            '
            'FrmTempleSetting
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.FromArgb(254, 249, 235)
            Me.ClientSize = New System.Drawing.Size(900, 620)
            Me.Controls.Add(Me.pMain)
            Me.Controls.Add(Me.pButtons)
            Me.Controls.Add(Me.lblHeader)
            Me.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.Name = "FrmTempleSetting"
            Me.Text = "ตั้งค่าข้อมูลวัด"
            Me.pMain.ResumeLayout(False)
            Me.pMain.PerformLayout()
            Me.tlpFields.ResumeLayout(False)
            Me.tlpFields.PerformLayout()
            Me.pButtons.ResumeLayout(False)
            Me.flpButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
