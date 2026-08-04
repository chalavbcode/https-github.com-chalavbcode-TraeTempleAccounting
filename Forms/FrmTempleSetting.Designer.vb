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
            Me.components = New Container()
            Me.ttMain = New ToolTip(Me.components)
            Me.lblHeader = New Label()
            Me.pMain = New Panel()
            Me.pButtons = New Panel()

            Me.lblTempleCode = New Label()
            Me.lblTempleName = New Label()
            Me.lblTempleAddress = New Label()
            Me.lblTambon = New Label()
            Me.lblAmphoe = New Label()
            Me.lblProvince = New Label()
            Me.lblPostCode = New Label()
            Me.lblTemplePhone = New Label()
            Me.lblAbbotName = New Label()
            Me.lblAbbotOfficeStatus = New Label()
            Me.lblWaiyawatName = New Label()
            Me.lblWaiyawatOfficeStatus = New Label()
            Me.lblBookkeeperName = New Label()
            Me.lblBookkeeperType = New Label()
            Me.lblPromptPayName = New Label()
            Me.lblPromptPayID = New Label()

            Me.txtTempleCode = New TextBox()
            Me.txtTempleName = New TextBox()
            Me.txtTempleAddress = New TextBox()
            Me.txtPostCode = New TextBox()
            Me.txtTemplePhone = New TextBox()
            Me.txtAbbotName = New TextBox()
            Me.txtWaiyawatName = New TextBox()
            Me.txtBookkeeperName = New TextBox()
            Me.txtPromptPayName = New TextBox()
            Me.txtPromptPayID = New TextBox()
            Me.cboProvince = New ComboBox()
            Me.cboAmphoe = New ComboBox()
            Me.cboTambon = New ComboBox()
            Me.cboAbbotOfficeStatus = New ComboBox()
            Me.cboWaiyawatOfficeStatus = New ComboBox()
            Me.cboBookkeeperType = New ComboBox()
            Me.chkUsePromptPay = New CheckBox()

            Me.btnSave = New Button()
            Me.btnCancel = New Button()
            Me.btnLocationImport = New Button()
            Me.btnClose = New Button()

            Me.pMain.SuspendLayout()
            Me.pButtons.SuspendLayout()
            Me.SuspendLayout()

            ' 
            ' lblHeader
            ' 
            Me.lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            Me.lblHeader.Dock = DockStyle.Top
            Me.lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            Me.lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblHeader.Location = New Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New Size(1250, 42)
            Me.lblHeader.TabIndex = 0
            Me.lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            Me.lblHeader.TextAlign = ContentAlignment.MiddleCenter

            ' 
            ' pMain
            ' 
            Me.pMain.AutoScroll = True
            Me.pMain.BackColor = Color.White
            Me.pMain.Controls.Add(Me.lblTempleCode)
            Me.pMain.Controls.Add(Me.txtTempleCode)
            Me.pMain.Controls.Add(Me.lblTempleName)
            Me.pMain.Controls.Add(Me.txtTempleName)
            Me.pMain.Controls.Add(Me.lblTempleAddress)
            Me.pMain.Controls.Add(Me.txtTempleAddress)
            Me.pMain.Controls.Add(Me.lblTambon)
            Me.pMain.Controls.Add(Me.cboTambon)
            Me.pMain.Controls.Add(Me.lblAmphoe)
            Me.pMain.Controls.Add(Me.cboAmphoe)
            Me.pMain.Controls.Add(Me.lblProvince)
            Me.pMain.Controls.Add(Me.cboProvince)
            Me.pMain.Controls.Add(Me.lblPostCode)
            Me.pMain.Controls.Add(Me.txtPostCode)
            Me.pMain.Controls.Add(Me.lblTemplePhone)
            Me.pMain.Controls.Add(Me.txtTemplePhone)
            Me.pMain.Controls.Add(Me.lblAbbotName)
            Me.pMain.Controls.Add(Me.txtAbbotName)
            Me.pMain.Controls.Add(Me.lblAbbotOfficeStatus)
            Me.pMain.Controls.Add(Me.cboAbbotOfficeStatus)
            Me.pMain.Controls.Add(Me.lblWaiyawatName)
            Me.pMain.Controls.Add(Me.txtWaiyawatName)
            Me.pMain.Controls.Add(Me.lblWaiyawatOfficeStatus)
            Me.pMain.Controls.Add(Me.cboWaiyawatOfficeStatus)
            Me.pMain.Controls.Add(Me.lblBookkeeperName)
            Me.pMain.Controls.Add(Me.txtBookkeeperName)
            Me.pMain.Controls.Add(Me.lblBookkeeperType)
            Me.pMain.Controls.Add(Me.cboBookkeeperType)
            Me.pMain.Controls.Add(Me.chkUsePromptPay)
            Me.pMain.Controls.Add(Me.lblPromptPayName)
            Me.pMain.Controls.Add(Me.txtPromptPayName)
            Me.pMain.Controls.Add(Me.lblPromptPayID)
            Me.pMain.Controls.Add(Me.txtPromptPayID)
            Me.pMain.Dock = DockStyle.Fill
            Me.pMain.Location = New Point(0, 42)
            Me.pMain.Name = "pMain"
            Me.pMain.Padding = New Padding(20)
            Me.pMain.Size = New Size(1250, 718)
            Me.pMain.TabIndex = 1

            ' Row 0: TempleCode
            Me.lblTempleCode.Font = New Font("Tahoma", 10.0!)
            Me.lblTempleCode.Location = New Point(40, 20)
            Me.lblTempleCode.Name = "lblTempleCode"
            Me.lblTempleCode.Size = New Size(200, 40)
            Me.lblTempleCode.TabIndex = 0
            Me.lblTempleCode.Text = "รหัสวัด:"
            Me.lblTempleCode.TextAlign = ContentAlignment.MiddleRight
            Me.txtTempleCode.Font = New Font("Tahoma", 10.5!)
            Me.txtTempleCode.Location = New Point(250, 20)
            Me.txtTempleCode.Name = "txtTempleCode"
            Me.txtTempleCode.Size = New Size(520, 33)
            Me.txtTempleCode.TabIndex = 1

            ' Row 1: TempleName
            Me.lblTempleName.Font = New Font("Tahoma", 10.0!)
            Me.lblTempleName.Location = New Point(40, 74)
            Me.lblTempleName.Name = "lblTempleName"
            Me.lblTempleName.Size = New Size(200, 40)
            Me.lblTempleName.TabIndex = 2
            Me.lblTempleName.Text = "ชื่อวัด:"
            Me.lblTempleName.TextAlign = ContentAlignment.MiddleRight
            Me.txtTempleName.Font = New Font("Tahoma", 10.5!)
            Me.txtTempleName.Location = New Point(250, 74)
            Me.txtTempleName.Name = "txtTempleName"
            Me.txtTempleName.Size = New Size(520, 33)
            Me.txtTempleName.TabIndex = 3

            ' Row 2: TempleAddress
            Me.lblTempleAddress.Font = New Font("Tahoma", 10.0!)
            Me.lblTempleAddress.Location = New Point(40, 128)
            Me.lblTempleAddress.Name = "lblTempleAddress"
            Me.lblTempleAddress.Size = New Size(200, 40)
            Me.lblTempleAddress.TabIndex = 4
            Me.lblTempleAddress.Text = "ที่อยู่วัด:"
            Me.lblTempleAddress.TextAlign = ContentAlignment.MiddleRight
            Me.txtTempleAddress.Font = New Font("Tahoma", 10.5!)
            Me.txtTempleAddress.Location = New Point(250, 128)
            Me.txtTempleAddress.Multiline = True
            Me.txtTempleAddress.Name = "txtTempleAddress"
            Me.txtTempleAddress.ScrollBars = ScrollBars.Vertical
            Me.txtTempleAddress.Size = New Size(520, 70)
            Me.txtTempleAddress.TabIndex = 5

            ' Row 3: Tambon
            Me.lblTambon.Font = New Font("Tahoma", 10.0!)
            Me.lblTambon.Location = New Point(40, 212)
            Me.lblTambon.Name = "lblTambon"
            Me.lblTambon.Size = New Size(200, 40)
            Me.lblTambon.TabIndex = 6
            Me.lblTambon.Text = "ตำบล:"
            Me.lblTambon.TextAlign = ContentAlignment.MiddleRight
            Me.cboTambon.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cboTambon.Font = New Font("Tahoma", 10.0!)
            Me.cboTambon.Location = New Point(250, 212)
            Me.cboTambon.Name = "cboTambon"
            Me.cboTambon.Size = New Size(520, 33)
            Me.cboTambon.TabIndex = 7

            ' Row 4: Amphoe
            Me.lblAmphoe.Font = New Font("Tahoma", 10.0!)
            Me.lblAmphoe.Location = New Point(40, 266)
            Me.lblAmphoe.Name = "lblAmphoe"
            Me.lblAmphoe.Size = New Size(200, 40)
            Me.lblAmphoe.TabIndex = 8
            Me.lblAmphoe.Text = "อำเภอ:"
            Me.lblAmphoe.TextAlign = ContentAlignment.MiddleRight
            Me.cboAmphoe.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cboAmphoe.Font = New Font("Tahoma", 10.0!)
            Me.cboAmphoe.Location = New Point(250, 266)
            Me.cboAmphoe.Name = "cboAmphoe"
            Me.cboAmphoe.Size = New Size(520, 33)
            Me.cboAmphoe.TabIndex = 9

            ' Row 5: Province
            Me.lblProvince.Font = New Font("Tahoma", 10.0!)
            Me.lblProvince.Location = New Point(40, 320)
            Me.lblProvince.Name = "lblProvince"
            Me.lblProvince.Size = New Size(200, 40)
            Me.lblProvince.TabIndex = 10
            Me.lblProvince.Text = "จังหวัด:"
            Me.lblProvince.TextAlign = ContentAlignment.MiddleRight
            Me.cboProvince.DropDownStyle = ComboBoxStyle.DropDownList
            Me.cboProvince.Font = New Font("Tahoma", 10.0!)
            Me.cboProvince.Location = New Point(250, 320)
            Me.cboProvince.Name = "cboProvince"
            Me.cboProvince.Size = New Size(520, 33)
            Me.cboProvince.TabIndex = 11

            ' Row 6: PostCode
            Me.lblPostCode.Font = New Font("Tahoma", 10.0!)
            Me.lblPostCode.Location = New Point(40, 374)
            Me.lblPostCode.Name = "lblPostCode"
            Me.lblPostCode.Size = New Size(200, 40)
            Me.lblPostCode.TabIndex = 12
            Me.lblPostCode.Text = "รหัสไปรษณีย์:"
            Me.lblPostCode.TextAlign = ContentAlignment.MiddleRight
            Me.txtPostCode.Font = New Font("Tahoma", 10.5!)
            Me.txtPostCode.Location = New Point(250, 374)
            Me.txtPostCode.Name = "txtPostCode"
            Me.txtPostCode.Size = New Size(520, 33)
            Me.txtPostCode.TabIndex = 13

            ' Row 7: TemplePhone
            Me.lblTemplePhone.Font = New Font("Tahoma", 10.0!)
            Me.lblTemplePhone.Location = New Point(40, 428)
            Me.lblTemplePhone.Name = "lblTemplePhone"
            Me.lblTemplePhone.Size = New Size(200, 40)
            Me.lblTemplePhone.TabIndex = 14
            Me.lblTemplePhone.Text = "เบอร์ติดต่อวัด:"
            Me.lblTemplePhone.TextAlign = ContentAlignment.MiddleRight
            Me.txtTemplePhone.Font = New Font("Tahoma", 10.5!)
            Me.txtTemplePhone.Location = New Point(250, 428)
            Me.txtTemplePhone.Name = "txtTemplePhone"
            Me.txtTemplePhone.Size = New Size(520, 33)
            Me.txtTemplePhone.TabIndex = 15

            ' Row 8: AbbotName
            Me.lblAbbotName.Font = New Font("Tahoma", 10.0!)
            Me.lblAbbotName.Location = New Point(40, 482)
            Me.lblAbbotName.Name = "lblAbbotName"
            Me.lblAbbotName.Size = New Size(200, 40)
            Me.lblAbbotName.TabIndex = 16
            Me.lblAbbotName.Text = "ชื่อเจ้าอาวาส:"
            Me.lblAbbotName.TextAlign = ContentAlignment.MiddleRight
            Me.txtAbbotName.Font = New Font("Tahoma", 10.5!)
            Me.txtAbbotName.Location = New Point(250, 482)
            Me.txtAbbotName.Name = "txtAbbotName"
            Me.txtAbbotName.Size = New Size(520, 33)
            Me.txtAbbotName.TabIndex = 17

            ' Row 9: AbbotOfficeStatus
            Me.lblAbbotOfficeStatus.Font = New Font("Tahoma", 10.0!)
            Me.lblAbbotOfficeStatus.Location = New Point(40, 536)
            Me.lblAbbotOfficeStatus.Name = "lblAbbotOfficeStatus"
            Me.lblAbbotOfficeStatus.Size = New Size(200, 40)
            Me.lblAbbotOfficeStatus.TabIndex = 18
            Me.lblAbbotOfficeStatus.Text = "ตำแหน่งเจ้าอาวาส:"
            Me.lblAbbotOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            Me.cboAbbotOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            Me.cboAbbotOfficeStatus.Font = New Font("Tahoma", 10.0!)
            Me.cboAbbotOfficeStatus.Items.AddRange(New Object() {"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})
            Me.cboAbbotOfficeStatus.Location = New Point(250, 536)
            Me.cboAbbotOfficeStatus.Name = "cboAbbotOfficeStatus"
            Me.cboAbbotOfficeStatus.Size = New Size(520, 33)
            Me.cboAbbotOfficeStatus.TabIndex = 19

            ' Row 10: WaiyawatName
            Me.lblWaiyawatName.Font = New Font("Tahoma", 10.0!)
            Me.lblWaiyawatName.Location = New Point(40, 590)
            Me.lblWaiyawatName.Name = "lblWaiyawatName"
            Me.lblWaiyawatName.Size = New Size(200, 40)
            Me.lblWaiyawatName.TabIndex = 20
            Me.lblWaiyawatName.Text = "ชื่อไวยาวัจกร:"
            Me.lblWaiyawatName.TextAlign = ContentAlignment.MiddleRight
            Me.txtWaiyawatName.Font = New Font("Tahoma", 10.5!)
            Me.txtWaiyawatName.Location = New Point(250, 590)
            Me.txtWaiyawatName.Name = "txtWaiyawatName"
            Me.txtWaiyawatName.Size = New Size(520, 33)
            Me.txtWaiyawatName.TabIndex = 21

            ' Row 11: WaiyawatOfficeStatus
            Me.lblWaiyawatOfficeStatus.Font = New Font("Tahoma", 10.0!)
            Me.lblWaiyawatOfficeStatus.Location = New Point(40, 644)
            Me.lblWaiyawatOfficeStatus.Name = "lblWaiyawatOfficeStatus"
            Me.lblWaiyawatOfficeStatus.Size = New Size(200, 40)
            Me.lblWaiyawatOfficeStatus.TabIndex = 22
            Me.lblWaiyawatOfficeStatus.Text = "ตำแหน่งไวยาวัจกร:"
            Me.lblWaiyawatOfficeStatus.TextAlign = ContentAlignment.MiddleRight
            Me.cboWaiyawatOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            Me.cboWaiyawatOfficeStatus.Font = New Font("Tahoma", 10.0!)
            Me.cboWaiyawatOfficeStatus.Items.AddRange(New Object() {"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})
            Me.cboWaiyawatOfficeStatus.Location = New Point(250, 644)
            Me.cboWaiyawatOfficeStatus.Name = "cboWaiyawatOfficeStatus"
            Me.cboWaiyawatOfficeStatus.Size = New Size(520, 33)
            Me.cboWaiyawatOfficeStatus.TabIndex = 23

            ' Row 12: BookkeeperName
            Me.lblBookkeeperName.Font = New Font("Tahoma", 10.0!)
            Me.lblBookkeeperName.Location = New Point(40, 698)
            Me.lblBookkeeperName.Name = "lblBookkeeperName"
            Me.lblBookkeeperName.Size = New Size(200, 40)
            Me.lblBookkeeperName.TabIndex = 24
            Me.lblBookkeeperName.Text = "ชื่อผู้ทำบัญชี:"
            Me.lblBookkeeperName.TextAlign = ContentAlignment.MiddleRight
            Me.txtBookkeeperName.Font = New Font("Tahoma", 10.5!)
            Me.txtBookkeeperName.Location = New Point(250, 698)
            Me.txtBookkeeperName.Name = "txtBookkeeperName"
            Me.txtBookkeeperName.Size = New Size(520, 33)
            Me.txtBookkeeperName.TabIndex = 25

            ' Row 13: BookkeeperType
            Me.lblBookkeeperType.Font = New Font("Tahoma", 10.0!)
            Me.lblBookkeeperType.Location = New Point(40, 752)
            Me.lblBookkeeperType.Name = "lblBookkeeperType"
            Me.lblBookkeeperType.Size = New Size(200, 40)
            Me.lblBookkeeperType.TabIndex = 26
            Me.lblBookkeeperType.Text = "ประเภทผู้ทำบัญชี:"
            Me.lblBookkeeperType.TextAlign = ContentAlignment.MiddleRight
            Me.cboBookkeeperType.DropDownStyle = ComboBoxStyle.DropDown
            Me.cboBookkeeperType.Font = New Font("Tahoma", 10.0!)
            Me.cboBookkeeperType.Items.AddRange(New Object() {"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})
            Me.cboBookkeeperType.Location = New Point(250, 752)
            Me.cboBookkeeperType.Name = "cboBookkeeperType"
            Me.cboBookkeeperType.Size = New Size(520, 33)
            Me.cboBookkeeperType.TabIndex = 27

            ' Row 14: UsePromptPay
            Me.chkUsePromptPay.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.chkUsePromptPay.ForeColor = Color.FromArgb(15, 118, 110)
            Me.chkUsePromptPay.Location = New Point(250, 806)
            Me.chkUsePromptPay.Name = "chkUsePromptPay"
            Me.chkUsePromptPay.Size = New Size(280, 40)
            Me.chkUsePromptPay.TabIndex = 28
            Me.chkUsePromptPay.Text = "เปิดใช้งานพร้อมเพย์"

            ' Row 15: PromptPayName
            Me.lblPromptPayName.Font = New Font("Tahoma", 10.0!)
            Me.lblPromptPayName.Location = New Point(40, 860)
            Me.lblPromptPayName.Name = "lblPromptPayName"
            Me.lblPromptPayName.Size = New Size(200, 40)
            Me.lblPromptPayName.TabIndex = 29
            Me.lblPromptPayName.Text = "ชื่อบัญชีพร้อมเพย์:"
            Me.lblPromptPayName.TextAlign = ContentAlignment.MiddleRight
            Me.txtPromptPayName.Font = New Font("Tahoma", 10.5!)
            Me.txtPromptPayName.Location = New Point(250, 860)
            Me.txtPromptPayName.Name = "txtPromptPayName"
            Me.txtPromptPayName.Size = New Size(520, 33)
            Me.txtPromptPayName.TabIndex = 30

            ' Row 16: PromptPayID
            Me.lblPromptPayID.Font = New Font("Tahoma", 10.0!)
            Me.lblPromptPayID.Location = New Point(40, 914)
            Me.lblPromptPayID.Name = "lblPromptPayID"
            Me.lblPromptPayID.Size = New Size(200, 40)
            Me.lblPromptPayID.TabIndex = 31
            Me.lblPromptPayID.Text = "เลขพร้อมเพย์/เลขบัญชี:"
            Me.lblPromptPayID.TextAlign = ContentAlignment.MiddleRight
            Me.txtPromptPayID.Font = New Font("Tahoma", 10.5!)
            Me.txtPromptPayID.Location = New Point(250, 914)
            Me.txtPromptPayID.Name = "txtPromptPayID"
            Me.txtPromptPayID.Size = New Size(520, 33)
            Me.txtPromptPayID.TabIndex = 32

            ' 
            ' pButtons
            ' 
            Me.pButtons.BackColor = Color.FromArgb(245, 245, 240)
            Me.pButtons.Controls.Add(Me.btnSave)
            Me.pButtons.Controls.Add(Me.btnCancel)
            Me.pButtons.Controls.Add(Me.btnLocationImport)
            Me.pButtons.Controls.Add(Me.btnClose)
            Me.pButtons.Dock = DockStyle.Bottom
            Me.pButtons.Location = New Point(0, 760)
            Me.pButtons.Name = "pButtons"
            Me.pButtons.Size = New Size(1250, 100)
            Me.pButtons.TabIndex = 2

            ' btnSave
            Me.btnSave.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnSave.FlatStyle = FlatStyle.Flat
            Me.btnSave.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.btnSave.ForeColor = Color.White
            Me.btnSave.Location = New Point(250, 20)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New Size(200, 50)
            Me.btnSave.TabIndex = 0
            Me.btnSave.Text = "💾 บันทึกข้อมูล"
            Me.btnSave.UseVisualStyleBackColor = False

            ' btnCancel
            Me.btnCancel.BackColor = Color.FromArgb(217, 119, 6)
            Me.btnCancel.FlatStyle = FlatStyle.Flat
            Me.btnCancel.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.btnCancel.ForeColor = Color.White
            Me.btnCancel.Location = New Point(470, 20)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New Size(150, 50)
            Me.btnCancel.TabIndex = 1
            Me.btnCancel.Text = "🔄 โหลดใหม่"
            Me.btnCancel.UseVisualStyleBackColor = False

            ' btnLocationImport
            Me.btnLocationImport.BackColor = Color.FromArgb(37, 99, 235)
            Me.btnLocationImport.FlatStyle = FlatStyle.Flat
            Me.btnLocationImport.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.btnLocationImport.ForeColor = Color.White
            Me.btnLocationImport.Location = New Point(640, 20)
            Me.btnLocationImport.Name = "btnLocationImport"
            Me.btnLocationImport.Size = New Size(220, 50)
            Me.btnLocationImport.TabIndex = 2
            Me.btnLocationImport.Text = "📍 นำเข้าจังหวัด/อำเภอ"
            Me.btnLocationImport.UseVisualStyleBackColor = False

            ' btnClose
            Me.btnClose.BackColor = Color.FromArgb(75, 85, 99)
            Me.btnClose.FlatStyle = FlatStyle.Flat
            Me.btnClose.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.btnClose.ForeColor = Color.White
            Me.btnClose.Location = New Point(880, 20)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New Size(100, 50)
            Me.btnClose.TabIndex = 3
            Me.btnClose.Text = "ปิด"
            Me.btnClose.UseVisualStyleBackColor = False

            ' 
            ' FrmTempleSetting
            ' 
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.ClientSize = New Size(1250, 860)
            Me.Controls.Add(Me.pMain)
            Me.Controls.Add(Me.pButtons)
            Me.Controls.Add(Me.lblHeader)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.Name = "FrmTempleSetting"
            Me.Text = "ตั้งค่าข้อมูลวัด"
            Me.pMain.ResumeLayout(False)
            Me.pMain.PerformLayout()
            Me.pButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
