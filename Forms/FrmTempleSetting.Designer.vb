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
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents btnLocationImport As Button
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
            Text = "ตั้งค่าข้อมูลวัด"
            BackColor = Color.FromArgb(254, 249, 235)
            Font = New Font("Tahoma", 10.5!)
            ClientSize = New Size(1320, 900)
            MinimumSize = New Size(1180, 780)
            StartPosition = FormStartPosition.CenterScreen
            WindowState = FormWindowState.Maximized
            FormBorderStyle = FormBorderStyle.Sizable
            AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 66
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            txtTempleCode = Tb(16)
            txtTempleName = Tb(16 + 40 + 14)
            txtTempleAddress = New TextBox With {.Font = New Font("Tahoma", 10.5!), .Multiline = True, .ScrollBars = ScrollBars.Vertical, .Size = New Size(520, 70), .Location = New Point(250, 16 + 2 * (40 + 14))}
            cboTambon = Cb(16 + 3 * (40 + 14) + 34)
            cboAmphoe = Cb(16 + 4 * (40 + 14) + 34)
            cboProvince = Cb(16 + 5 * (40 + 14) + 34)
            txtPostCode = Tb(16 + 6 * (40 + 14) + 34)
            txtTemplePhone = Tb(16 + 7 * (40 + 14) + 34)

            txtAbbotName = Tb(16 + 8 * (40 + 14) + 34)
            cboAbbotOfficeStatus = Cb(16 + 9 * (40 + 14) + 34)
            cboAbbotOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            cboAbbotOfficeStatus.Items.AddRange({"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})

            txtWaiyawatName = Tb(16 + 10 * (40 + 14) + 34)
            cboWaiyawatOfficeStatus = Cb(16 + 11 * (40 + 14) + 34)
            cboWaiyawatOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            cboWaiyawatOfficeStatus.Items.AddRange({"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})

            txtBookkeeperName = Tb(16 + 12 * (40 + 14) + 34)
            cboBookkeeperType = Cb(16 + 13 * (40 + 14) + 34)
            cboBookkeeperType.DropDownStyle = ComboBoxStyle.DropDown
            cboBookkeeperType.Items.AddRange({"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})

            chkUsePromptPay = New CheckBox With {.Text = "เปิดใช้งานพร้อมเพย์", .Location = New Point(250, 16 + 14 * (40 + 14) + 34), .Size = New Size(280, 40), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .ForeColor = Color.FromArgb(15, 118, 110)}
            txtPromptPayName = Tb(16 + 15 * (40 + 14) + 34)
            txtPromptPayID = Tb(16 + 16 * (40 + 14) + 34)

            Dim yBtns As Integer = 16 + 17 * (40 + 14) + 54
            btnSave = New Button With {.Text = "💾 บันทึกข้อมูล", .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(220, 56), .Location = New Point(250, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnCancel = New Button With {.Text = "🔄 โหลดใหม่", .BackColor = Color.FromArgb(217, 119, 6), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(170, 56), .Location = New Point(480, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnLocationImport = New Button With {.Text = "📍 นำเข้าจังหวัด/อำเภอ", .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(240, 56), .Location = New Point(660, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnClose = New Button With {.Text = "ปิด", .BackColor = Color.FromArgb(75, 85, 99), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(120, 56), .Location = New Point(910, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}

            Controls.Add(lblHeader)
            Dim labels As New List(Of Control) From {
                Lbl("รหัสวัด:", 40, 16), Lbl("ชื่อวัด:", 40, 16 + 1 * (40 + 14)),
                Lbl("ที่อยู่วัด:", 40, 16 + 2 * (40 + 14)),
                Lbl("ตำบล:", 40, 16 + 3 * (40 + 14) + 34), Lbl("อำเภอ:", 40, 16 + 4 * (40 + 14) + 34), Lbl("จังหวัด:", 40, 16 + 5 * (40 + 14) + 34),
                Lbl("รหัสไปรษณีย์:", 40, 16 + 6 * (40 + 14) + 34), Lbl("เบอร์ติดต่อวัด:", 40, 16 + 7 * (40 + 14) + 34),
                Lbl("ชื่อสมเด็จพระเจ้าอาวาส:", 40, 16 + 8 * (40 + 14) + 34), Lbl("ตำแหน่งเจ้าอาวาส:", 40, 16 + 9 * (40 + 14) + 34),
                Lbl("ชื่อไวยาวัจกร:", 40, 16 + 10 * (40 + 14) + 34), Lbl("ตำแหน่งไวยาวัจกร:", 40, 16 + 11 * (40 + 14) + 34),
                Lbl("ชื่อผู้ทำบัญชี:", 40, 16 + 12 * (40 + 14) + 34), Lbl("ประเภทผู้ทำบัญชี:", 40, 16 + 13 * (40 + 14) + 34),
                New Label With {.Location = New Point(40, 16 + 14 * (40 + 14) + 34), .Size = New Size(200, 40), .Visible = False},
                Lbl("ชื่อบัญชีพร้อมเพย์:", 40, 16 + 15 * (40 + 14) + 34), Lbl("เลขพร้อมเพย์/เลขบัญชี:", 40, 16 + 16 * (40 + 14) + 34)
            }
            Controls.AddRange(labels.ToArray())
            Controls.AddRange(New Control() {
                txtTempleCode, txtTempleName, txtTempleAddress, cboTambon, cboAmphoe, cboProvince, txtPostCode, txtTemplePhone,
                txtAbbotName, cboAbbotOfficeStatus, txtWaiyawatName, cboWaiyawatOfficeStatus, txtBookkeeperName, cboBookkeeperType,
                chkUsePromptPay, txtPromptPayName, txtPromptPayID,
                btnSave, btnCancel, btnLocationImport, btnClose
            })
        End Sub
    End Class
End Namespace

