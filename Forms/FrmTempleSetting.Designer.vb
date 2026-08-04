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
        Friend WithEvents txtAbbotName As TextBox
        Friend WithEvents txtWaiyawatName As TextBox
        Friend WithEvents txtBookkeeperName As TextBox
        Friend WithEvents txtPromptPayName As TextBox
        Friend WithEvents txtPromptPayID As TextBox
        Friend WithEvents cboProvince As ComboBox
        Friend WithEvents cboAmphoe As ComboBox
        Friend WithEvents cboTambon As ComboBox
        Friend WithEvents cboAbbotOfficeStatus As ComboBox
        Friend WithEvents cboWaiyawatOfficeStatus As ComboBox
        Friend WithEvents cboBookkeeperType As ComboBox
        Friend WithEvents chkUsePromptPay As CheckBox

        ' Buttons
        Friend WithEvents pButtons As Panel
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents btnLocationImport As Button
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
            lblTempleCode = New Label()
            txtTempleCode = New TextBox()
            lblTempleName = New Label()
            txtTempleName = New TextBox()
            lblTempleAddress = New Label()
            txtTempleAddress = New TextBox()
            lblTambon = New Label()
            cboTambon = New ComboBox()
            lblAmphoe = New Label()
            cboAmphoe = New ComboBox()
            lblProvince = New Label()
            cboProvince = New ComboBox()
            lblPostCode = New Label()
            txtPostCode = New TextBox()
            lblTemplePhone = New Label()
            txtTemplePhone = New TextBox()
            lblAbbotName = New Label()
            txtAbbotName = New TextBox()
            lblAbbotOfficeStatus = New Label()
            cboAbbotOfficeStatus = New ComboBox()
            lblWaiyawatName = New Label()
            txtWaiyawatName = New TextBox()
            lblWaiyawatOfficeStatus = New Label()
            cboWaiyawatOfficeStatus = New ComboBox()
            lblBookkeeperName = New Label()
            txtBookkeeperName = New TextBox()
            lblBookkeeperType = New Label()
            cboBookkeeperType = New ComboBox()
            chkUsePromptPay = New CheckBox()
            lblPromptPayName = New Label()
            txtPromptPayName = New TextBox()
            lblPromptPayID = New Label()
            txtPromptPayID = New TextBox()
            pButtons = New Panel()
            btnSave = New Button()
            btnCancel = New Button()
            btnLocationImport = New Button()
            btnClose = New Button()
            pMain.SuspendLayout()
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
            lblHeader.Size = New Size(1250, 42)
            lblHeader.TabIndex = 0
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' pMain
            ' 
            pMain.AutoScroll = True
            pMain.BackColor = Color.White
            pMain.Controls.Add(lblTempleCode)
            pMain.Controls.Add(txtTempleCode)
            pMain.Controls.Add(lblTempleName)
            pMain.Controls.Add(txtTempleName)
            pMain.Controls.Add(lblTempleAddress)
            pMain.Controls.Add(txtTempleAddress)
            pMain.Controls.Add(lblTambon)
            pMain.Controls.Add(cboTambon)
            pMain.Controls.Add(lblAmphoe)
            pMain.Controls.Add(cboAmphoe)
            pMain.Controls.Add(lblProvince)
            pMain.Controls.Add(cboProvince)
            pMain.Controls.Add(lblPostCode)
            pMain.Controls.Add(txtPostCode)
            pMain.Controls.Add(lblTemplePhone)
            pMain.Controls.Add(txtTemplePhone)
            pMain.Controls.Add(lblAbbotName)
            pMain.Controls.Add(txtAbbotName)
            pMain.Controls.Add(lblAbbotOfficeStatus)
            pMain.Controls.Add(cboAbbotOfficeStatus)
            pMain.Controls.Add(lblWaiyawatName)
            pMain.Controls.Add(txtWaiyawatName)
            pMain.Controls.Add(lblWaiyawatOfficeStatus)
            pMain.Controls.Add(cboWaiyawatOfficeStatus)
            pMain.Controls.Add(lblBookkeeperName)
            pMain.Controls.Add(txtBookkeeperName)
            pMain.Controls.Add(lblBookkeeperType)
            pMain.Controls.Add(cboBookkeeperType)
            pMain.Controls.Add(chkUsePromptPay)
            pMain.Controls.Add(lblPromptPayName)
            pMain.Controls.Add(txtPromptPayName)
            pMain.Controls.Add(lblPromptPayID)
            pMain.Controls.Add(txtPromptPayID)
            pMain.Dock = DockStyle.Fill
            pMain.Location = New Point(0, 42)
            pMain.Name = "pMain"
            pMain.Padding = New Padding(20)
            pMain.Size = New Size(1250, 718)
            pMain.TabIndex = 1
            ' 
            ' lblTempleCode
            ' 
            lblTempleCode.Font = New Font("Tahoma", 10F)
            lblTempleCode.Location = New Point(40, 20)
            lblTempleCode.Name = "lblTempleCode"
            lblTempleCode.Size = New Size(200, 40)
            lblTempleCode.TabIndex = 0
            lblTempleCode.Text = "รหัสวัด:"
            lblTempleCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleCode
            ' 
            txtTempleCode.Font = New Font("Tahoma", 10.5F)
            txtTempleCode.Location = New Point(250, 20)
            txtTempleCode.Name = "txtTempleCode"
            txtTempleCode.Size = New Size(520, 33)
            txtTempleCode.TabIndex = 1
            ' 
            ' lblTempleName
            ' 
            lblTempleName.Font = New Font("Tahoma", 10F)
            lblTempleName.Location = New Point(40, 74)
            lblTempleName.Name = "lblTempleName"
            lblTempleName.Size = New Size(200, 40)
            lblTempleName.TabIndex = 2
            lblTempleName.Text = "ชื่อวัด:"
            lblTempleName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleName
            ' 
            txtTempleName.Font = New Font("Tahoma", 10.5F)
            txtTempleName.Location = New Point(250, 74)
            txtTempleName.Name = "txtTempleName"
            txtTempleName.Size = New Size(520, 33)
            txtTempleName.TabIndex = 3
            ' 
            ' lblTempleAddress
            ' 
            lblTempleAddress.Font = New Font("Tahoma", 10F)
            lblTempleAddress.Location = New Point(40, 128)
            lblTempleAddress.Name = "lblTempleAddress"
            lblTempleAddress.Size = New Size(200, 40)
            lblTempleAddress.TabIndex = 4
            lblTempleAddress.Text = "ที่อยู่วัด:"
            lblTempleAddress.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTempleAddress
            ' 
            txtTempleAddress.Font = New Font("Tahoma", 10.5F)
            txtTempleAddress.Location = New Point(250, 128)
            txtTempleAddress.Multiline = True
            txtTempleAddress.Name = "txtTempleAddress"
            txtTempleAddress.ScrollBars = ScrollBars.Vertical
            txtTempleAddress.Size = New Size(520, 70)
            txtTempleAddress.TabIndex = 5
            ' 
            ' lblTambon
            ' 
            lblTambon.Font = New Font("Tahoma", 10F)
            lblTambon.Location = New Point(40, 212)
            lblTambon.Name = "lblTambon"
            lblTambon.Size = New Size(200, 40)
            lblTambon.TabIndex = 6
            lblTambon.Text = "ตำบล:"
            lblTambon.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboTambon
            ' 
            cboTambon.DropDownStyle = ComboBoxStyle.DropDownList
            cboTambon.Font = New Font("Tahoma", 10F)
            cboTambon.Location = New Point(250, 212)
            cboTambon.Name = "cboTambon"
            cboTambon.Size = New Size(520, 32)
            cboTambon.TabIndex = 7
            ' 
            ' lblAmphoe
            ' 
            lblAmphoe.Font = New Font("Tahoma", 10F)
            lblAmphoe.Location = New Point(40, 266)
            lblAmphoe.Name = "lblAmphoe"
            lblAmphoe.Size = New Size(200, 40)
            lblAmphoe.TabIndex = 8
            lblAmphoe.Text = "อำเภอ:"
            lblAmphoe.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAmphoe
            ' 
            cboAmphoe.DropDownStyle = ComboBoxStyle.DropDownList
            cboAmphoe.Font = New Font("Tahoma", 10F)
            cboAmphoe.Location = New Point(250, 266)
            cboAmphoe.Name = "cboAmphoe"
            cboAmphoe.Size = New Size(520, 32)
            cboAmphoe.TabIndex = 9
            ' 
            ' lblProvince
            ' 
            lblProvince.Font = New Font("Tahoma", 10F)
            lblProvince.Location = New Point(40, 320)
            lblProvince.Name = "lblProvince"
            lblProvince.Size = New Size(200, 40)
            lblProvince.TabIndex = 10
            lblProvince.Text = "จังหวัด:"
            lblProvince.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboProvince
            ' 
            cboProvince.DropDownStyle = ComboBoxStyle.DropDownList
            cboProvince.Font = New Font("Tahoma", 10F)
            cboProvince.Location = New Point(250, 320)
            cboProvince.Name = "cboProvince"
            cboProvince.Size = New Size(520, 32)
            cboProvince.TabIndex = 11
            ' 
            ' lblPostCode
            ' 
            lblPostCode.Font = New Font("Tahoma", 10F)
            lblPostCode.Location = New Point(40, 374)
            lblPostCode.Name = "lblPostCode"
            lblPostCode.Size = New Size(200, 40)
            lblPostCode.TabIndex = 12
            lblPostCode.Text = "รหัสไปรษณีย์:"
            lblPostCode.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPostCode
            ' 
            txtPostCode.Font = New Font("Tahoma", 10.5F)
            txtPostCode.Location = New Point(250, 374)
            txtPostCode.Name = "txtPostCode"
            txtPostCode.Size = New Size(520, 33)
            txtPostCode.TabIndex = 13
            ' 
            ' lblTemplePhone
            ' 
            lblTemplePhone.Font = New Font("Tahoma", 10F)
            lblTemplePhone.Location = New Point(40, 428)
            lblTemplePhone.Name = "lblTemplePhone"
            lblTemplePhone.Size = New Size(200, 40)
            lblTemplePhone.TabIndex = 14
            lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            lblTemplePhone.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtTemplePhone
            ' 
            txtTemplePhone.Font = New Font("Tahoma", 10.5F)
            txtTemplePhone.Location = New Point(250, 428)
            txtTemplePhone.Name = "txtTemplePhone"
            txtTemplePhone.Size = New Size(520, 33)
            txtTemplePhone.TabIndex = 15
            ' 
            ' lblAbbotName
            ' 
            lblAbbotName.Font = New Font("Tahoma", 10F)
            lblAbbotName.Location = New Point(40, 482)
            lblAbbotName.Name = "lblAbbotName"
            lblAbbotName.Size = New Size(200, 40)
            lblAbbotName.TabIndex = 16
            lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            lblAbbotName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtAbbotName
            ' 
            txtAbbotName.Font = New Font("Tahoma", 10.5F)
            txtAbbotName.Location = New Point(250, 482)
            txtAbbotName.Name = "txtAbbotName"
            txtAbbotName.Size = New Size(520, 33)
            txtAbbotName.TabIndex = 17
            ' 
            ' lblAbbotOfficeStatus
            ' 
            lblAbbotOfficeStatus.Font = New Font("Tahoma", 10F)
            lblAbbotOfficeStatus.Location = New Point(40, 536)
            lblAbbotOfficeStatus.Name = "lblAbbotOfficeStatus"
            lblAbbotOfficeStatus.Size = New Size(200, 40)
            lblAbbotOfficeStatus.TabIndex = 18
            lblAbbotOfficeStatus.Text = "ตำแหน่งเจ้าอาวาส:"
            lblAbbotOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboAbbotOfficeStatus
            ' 
            cboAbbotOfficeStatus.Font = New Font("Tahoma", 10F)
            cboAbbotOfficeStatus.Items.AddRange(New Object() {"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})
            cboAbbotOfficeStatus.Location = New Point(250, 536)
            cboAbbotOfficeStatus.Name = "cboAbbotOfficeStatus"
            cboAbbotOfficeStatus.Size = New Size(520, 32)
            cboAbbotOfficeStatus.TabIndex = 19
            ' 
            ' lblWaiyawatName
            ' 
            lblWaiyawatName.Font = New Font("Tahoma", 10F)
            lblWaiyawatName.Location = New Point(40, 590)
            lblWaiyawatName.Name = "lblWaiyawatName"
            lblWaiyawatName.Size = New Size(200, 40)
            lblWaiyawatName.TabIndex = 20
            lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            lblWaiyawatName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtWaiyawatName
            ' 
            txtWaiyawatName.Font = New Font("Tahoma", 10.5F)
            txtWaiyawatName.Location = New Point(250, 590)
            txtWaiyawatName.Name = "txtWaiyawatName"
            txtWaiyawatName.Size = New Size(520, 33)
            txtWaiyawatName.TabIndex = 21
            ' 
            ' lblWaiyawatOfficeStatus
            ' 
            lblWaiyawatOfficeStatus.Font = New Font("Tahoma", 10F)
            lblWaiyawatOfficeStatus.Location = New Point(40, 644)
            lblWaiyawatOfficeStatus.Name = "lblWaiyawatOfficeStatus"
            lblWaiyawatOfficeStatus.Size = New Size(200, 40)
            lblWaiyawatOfficeStatus.TabIndex = 22
            lblWaiyawatOfficeStatus.Text = "ตำแหน่งไวยาวัจกร:"
            lblWaiyawatOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboWaiyawatOfficeStatus
            ' 
            cboWaiyawatOfficeStatus.Font = New Font("Tahoma", 10F)
            cboWaiyawatOfficeStatus.Items.AddRange(New Object() {"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})
            cboWaiyawatOfficeStatus.Location = New Point(250, 644)
            cboWaiyawatOfficeStatus.Name = "cboWaiyawatOfficeStatus"
            cboWaiyawatOfficeStatus.Size = New Size(520, 32)
            cboWaiyawatOfficeStatus.TabIndex = 23
            ' 
            ' lblBookkeeperName
            ' 
            lblBookkeeperName.Font = New Font("Tahoma", 10F)
            lblBookkeeperName.Location = New Point(40, 698)
            lblBookkeeperName.Name = "lblBookkeeperName"
            lblBookkeeperName.Size = New Size(200, 40)
            lblBookkeeperName.TabIndex = 24
            lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            lblBookkeeperName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtBookkeeperName
            ' 
            txtBookkeeperName.Font = New Font("Tahoma", 10.5F)
            txtBookkeeperName.Location = New Point(250, 698)
            txtBookkeeperName.Name = "txtBookkeeperName"
            txtBookkeeperName.Size = New Size(520, 33)
            txtBookkeeperName.TabIndex = 25
            ' 
            ' lblBookkeeperType
            ' 
            lblBookkeeperType.Font = New Font("Tahoma", 10F)
            lblBookkeeperType.Location = New Point(40, 752)
            lblBookkeeperType.Name = "lblBookkeeperType"
            lblBookkeeperType.Size = New Size(200, 40)
            lblBookkeeperType.TabIndex = 26
            lblBookkeeperType.Text = "ประเภทผู้ทำบัญชี:"
            lblBookkeeperType.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' cboBookkeeperType
            ' 
            cboBookkeeperType.Font = New Font("Tahoma", 10F)
            cboBookkeeperType.Items.AddRange(New Object() {"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})
            cboBookkeeperType.Location = New Point(250, 752)
            cboBookkeeperType.Name = "cboBookkeeperType"
            cboBookkeeperType.Size = New Size(520, 32)
            cboBookkeeperType.TabIndex = 27
            ' 
            ' chkUsePromptPay
            ' 
            chkUsePromptPay.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            chkUsePromptPay.ForeColor = Color.FromArgb(CByte(15), CByte(118), CByte(110))
            chkUsePromptPay.Location = New Point(250, 806)
            chkUsePromptPay.Name = "chkUsePromptPay"
            chkUsePromptPay.Size = New Size(280, 40)
            chkUsePromptPay.TabIndex = 28
            chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"
            ' 
            ' lblPromptPayName
            ' 
            lblPromptPayName.Font = New Font("Tahoma", 10F)
            lblPromptPayName.Location = New Point(40, 860)
            lblPromptPayName.Name = "lblPromptPayName"
            lblPromptPayName.Size = New Size(200, 40)
            lblPromptPayName.TabIndex = 29
            lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            lblPromptPayName.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayName
            ' 
            txtPromptPayName.Font = New Font("Tahoma", 10.5F)
            txtPromptPayName.Location = New Point(250, 860)
            txtPromptPayName.Name = "txtPromptPayName"
            txtPromptPayName.Size = New Size(520, 33)
            txtPromptPayName.TabIndex = 30
            ' 
            ' lblPromptPayID
            ' 
            lblPromptPayID.Font = New Font("Tahoma", 10F)
            lblPromptPayID.Location = New Point(40, 914)
            lblPromptPayID.Name = "lblPromptPayID"
            lblPromptPayID.Size = New Size(200, 40)
            lblPromptPayID.TabIndex = 31
            lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            lblPromptPayID.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' txtPromptPayID
            ' 
            txtPromptPayID.Font = New Font("Tahoma", 10.5F)
            txtPromptPayID.Location = New Point(250, 914)
            txtPromptPayID.Name = "txtPromptPayID"
            txtPromptPayID.Size = New Size(520, 33)
            txtPromptPayID.TabIndex = 32
            ' 
            ' pButtons
            ' 
            pButtons.BackColor = Color.FromArgb(CByte(245), CByte(245), CByte(240))
            pButtons.Controls.Add(btnSave)
            pButtons.Controls.Add(btnCancel)
            pButtons.Controls.Add(btnLocationImport)
            pButtons.Controls.Add(btnClose)
            pButtons.Dock = DockStyle.Bottom
            pButtons.Location = New Point(0, 760)
            pButtons.Name = "pButtons"
            pButtons.Size = New Size(1250, 100)
            pButtons.TabIndex = 2
            ' 
            ' btnSave
            ' 
            btnSave.BackColor = Color.FromArgb(CByte(22), CByte(163), CByte(74))
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnSave.ForeColor = Color.White
            btnSave.Location = New Point(151, 19)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(200, 50)
            btnSave.TabIndex = 0
            btnSave.Text = "💾 บันทึกข้อมูล"
            btnSave.UseVisualStyleBackColor = False
            ' 
            ' btnCancel
            ' 
            btnCancel.BackColor = Color.FromArgb(CByte(217), CByte(119), CByte(6))
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnCancel.ForeColor = Color.White
            btnCancel.Location = New Point(368, 19)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(150, 50)
            btnCancel.TabIndex = 1
            btnCancel.Text = "🔄 โหลดใหม่"
            btnCancel.UseVisualStyleBackColor = False
            ' 
            ' btnLocationImport
            ' 
            btnLocationImport.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnLocationImport.FlatStyle = FlatStyle.Flat
            btnLocationImport.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnLocationImport.ForeColor = Color.White
            btnLocationImport.Location = New Point(538, 19)
            btnLocationImport.Name = "btnLocationImport"
            btnLocationImport.Size = New Size(259, 50)
            btnLocationImport.TabIndex = 2
            btnLocationImport.Text = "📍 นำเข้าจังหวัด/อำเภอ"
            btnLocationImport.UseVisualStyleBackColor = False
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(820, 19)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(100, 50)
            btnClose.TabIndex = 3
            btnClose.Text = "ปิด"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmTempleSetting
            ' 
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1250, 860)
            Controls.Add(pMain)
            Controls.Add(pButtons)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            Name = "FrmTempleSetting"
            Text = "ตั้งค่าข้อมูลวัด"
            pMain.ResumeLayout(False)
            pMain.PerformLayout()
            pButtons.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
