Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmLocationImport
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents lnkOpenImportFolder As LinkLabel
        Friend WithEvents rtbLog As RichTextBox
        Friend WithEvents prgImport As ProgressBar
        Friend WithEvents chkClearBeforeImport As CheckBox
        Friend WithEvents btnImport As Button
        Friend WithEvents btnUpdate As Button
        Friend WithEvents btnCheck As Button
        Friend WithEvents btnRebuild As Button
        Friend WithEvents btnClose As Button
        Friend WithEvents lblStatus As Label
        Friend WithEvents lblProgress As Label
        Friend WithEvents lblLastImport As Label
        Friend WithEvents lblLastImportTitle As Label
        Friend WithEvents lblProvinceCount As Label
        Friend WithEvents lblDistrictCount As Label
        Friend WithEvents lblSubDistrictCount As Label
        Friend WithEvents lblProvinceFile As Label
        Friend WithEvents lblDistrictFile As Label
        Friend WithEvents Label2 As Label
        Friend WithEvents Label3 As Label
        Friend WithEvents Label4 As Label
        Friend WithEvents grp1 As GroupBox
        Friend WithEvents grp2 As GroupBox
        Friend WithEvents grp3 As GroupBox
        Friend WithEvents grp4 As GroupBox
        Friend WithEvents grp5 As GroupBox
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
            Text = "นำเข้าข้อมูลจังหวัด"
            BackColor = Color.FromArgb(254, 249, 235)
            Font = New Font("Tahoma", 10.5!)
            ClientSize = New Size(1280, 820)
            MinimumSize = New Size(1180, 760)
            StartPosition = FormStartPosition.CenterScreen
            WindowState = FormWindowState.Maximized
            FormBorderStyle = FormBorderStyle.Sizable
            AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(12, 74, 110)
            lblHeader.BackColor = Color.FromArgb(186, 230, 253)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 66
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            grp1 = New GroupBox With {.Text = "ไฟล์ CSV ต้นฉบับ", .Location = New Point(20, 80), .Size = New Size(840, 130), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            lblProvinceFile = New Label With {.Text = "province.csv:", .Location = New Point(20, 30), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            lblDistrictFile = New Label With {.Text = "amphoe.csv:", .Location = New Point(20, 60), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            Label2 = New Label With {.Text = "tambon.csv:", .Location = New Point(20, 90), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            lblProvinceCount = New Label With {.Text = "-", .Location = New Point(200, 30), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lblDistrictCount = New Label With {.Text = "-", .Location = New Point(200, 60), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lblSubDistrictCount = New Label With {.Text = "-", .Location = New Point(200, 90), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lnkOpenImportFolder = New LinkLabel With {.Text = "📂 เปิดโฟลเดอร์ Import", .Location = New Point(580, 30), .AutoSize = True, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp1.Controls.AddRange(New Control() {lblProvinceFile, lblDistrictFile, Label2, lblProvinceCount, lblDistrictCount, lblSubDistrictCount, lnkOpenImportFolder})

            grp2 = New GroupBox With {.Text = "ขั้นตอนการนำเข้า", .Location = New Point(20, 220), .Size = New Size(840, 90), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            chkClearBeforeImport = New CheckBox With {.Text = "ล้างข้อมูลเก่าก่อนนำเข้า (แนะนำครั้งแรก)", .Location = New Point(20, 30), .Size = New Size(520, 40), .Font = New Font("Tahoma", 10.0!)}
            btnImport = New Button With {.Text = "1⃣ นำเข้าใหม่", .Location = New Point(380, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnUpdate = New Button With {.Text = "2⃣ อัปเดตเพิ่ม", .Location = New Point(530, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(5, 150, 105), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnRebuild = New Button With {.Text = "3⃣ สร้างใหม่ทั้งหมด", .Location = New Point(680, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(180, 83, 9), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            grp2.Controls.AddRange(New Control() {chkClearBeforeImport, btnImport, btnUpdate, btnRebuild})

            grp3 = New GroupBox With {.Text = "ตรวจสอบ", .Location = New Point(870, 80), .Size = New Size(330, 230), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            btnCheck = New Button With {.Text = "🔍 นับจำนวนในฐานข้อมูล", .Location = New Point(20, 30), .Size = New Size(290, 52), .BackColor = Color.FromArgb(79, 70, 229), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            Label3 = New Label With {.Text = "รายการในตาราง:", .Location = New Point(20, 96), .AutoSize = True}
            Label4 = New Label With {.Text = "Province / District / SubDistrict", .Location = New Point(20, 124), .AutoSize = True, .Font = New Font("Tahoma", 9.5!), .ForeColor = Color.FromArgb(75, 85, 99)}
            lblStatus = New Label With {.Text = "รอการตรวจสอบ", .Location = New Point(20, 160), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp3.Controls.AddRange(New Control() {btnCheck, Label3, Label4, lblStatus})

            grp4 = New GroupBox With {.Text = "Progress", .Location = New Point(20, 320), .Size = New Size(1180, 90), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            prgImport = New ProgressBar With {.Location = New Point(20, 32), .Size = New Size(800, 32), .Style = ProgressBarStyle.Continuous}
            lblProgress = New Label With {.Text = "รอการทำงาน", .Location = New Point(840, 32), .AutoSize = True, .Font = New Font("Tahoma", 10.5!, FontStyle.Bold)}
            grp4.Controls.AddRange(New Control() {prgImport, lblProgress})

            grp5 = New GroupBox With {.Text = "ประวัติการทำงานล่าสุด", .Location = New Point(20, 420), .Size = New Size(1180, 260), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            rtbLog = New RichTextBox With {.Location = New Point(16, 32), .Size = New Size(1148, 180), .Font = New Font("Consolas", 9.0!), .BackColor = Color.FromArgb(15, 23, 42), .ForeColor = Color.FromArgb(226, 232, 240), .ReadOnly = True}
            lblLastImportTitle = New Label With {.Text = "ครั้งล่าสุด:", .Location = New Point(16, 220), .AutoSize = True}
            lblLastImport = New Label With {.Text = "-", .Location = New Point(120, 220), .AutoSize = True, .ForeColor = Color.FromArgb(79, 70, 229), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp5.Controls.AddRange(New Control() {rtbLog, lblLastImportTitle, lblLastImport})

            btnClose = New Button With {.Text = "ปิดหน้านี้", .Location = New Point(1050, 690), .Size = New Size(150, 50), .BackColor = Color.FromArgb(75, 85, 99), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}

            Controls.AddRange(New Control() {lblHeader, grp1, grp2, grp3, grp4, grp5, btnClose})
        End Sub
    End Class
End Namespace

