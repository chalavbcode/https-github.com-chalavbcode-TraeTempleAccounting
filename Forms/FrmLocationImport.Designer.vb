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

        Friend WithEvents lblHeader As Label
        Friend WithEvents grp1 As GroupBox
        Friend WithEvents lblProvinceFile As Label
        Friend WithEvents lblDistrictFile As Label
        Friend WithEvents Label2 As Label
        Friend WithEvents lblProvinceCount As Label
        Friend WithEvents lblDistrictCount As Label
        Friend WithEvents lblSubDistrictCount As Label
        Friend WithEvents lnkOpenImportFolder As LinkLabel
        
        Friend WithEvents grp2 As GroupBox
        Friend WithEvents chkClearBeforeImport As CheckBox
        Friend WithEvents btnImport As Button
        Friend WithEvents btnUpdate As Button
        Friend WithEvents btnRebuild As Button
        
        Friend WithEvents grp3 As GroupBox
        Friend WithEvents btnCheck As Button
        Friend WithEvents Label3 As Label
        Friend WithEvents Label4 As Label
        Friend WithEvents lblStatus As Label
        
        Friend WithEvents grp4 As GroupBox
        Friend WithEvents prgImport As ProgressBar
        Friend WithEvents lblProgress As Label
        
        Friend WithEvents grp5 As GroupBox
        Friend WithEvents rtbLog As RichTextBox
        Friend WithEvents lblLastImportTitle As Label
        Friend WithEvents lblLastImport As Label
        
        Friend WithEvents btnClose As Button
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
            Me.components = New Container()
            Me.ttMain = New ToolTip(Me.components)
            Me.lblHeader = New Label()
            Me.grp1 = New GroupBox()
            Me.lblProvinceFile = New Label()
            Me.lblDistrictFile = New Label()
            Me.Label2 = New Label()
            Me.lblProvinceCount = New Label()
            Me.lblDistrictCount = New Label()
            Me.lblSubDistrictCount = New Label()
            Me.lnkOpenImportFolder = New LinkLabel()
            Me.grp2 = New GroupBox()
            Me.chkClearBeforeImport = New CheckBox()
            Me.btnImport = New Button()
            Me.btnUpdate = New Button()
            Me.btnRebuild = New Button()
            Me.grp3 = New GroupBox()
            Me.btnCheck = New Button()
            Me.Label3 = New Label()
            Me.Label4 = New Label()
            Me.lblStatus = New Label()
            Me.grp4 = New GroupBox()
            Me.prgImport = New ProgressBar()
            Me.lblProgress = New Label()
            Me.grp5 = New GroupBox()
            Me.rtbLog = New RichTextBox()
            Me.lblLastImportTitle = New Label()
            Me.lblLastImport = New Label()
            Me.btnClose = New Button()
            Me.grp1.SuspendLayout()
            Me.grp2.SuspendLayout()
            Me.grp3.SuspendLayout()
            Me.grp4.SuspendLayout()
            Me.grp5.SuspendLayout()
            Me.SuspendLayout()
            ' 
            ' lblHeader
            ' 
            Me.lblHeader.BackColor = Color.FromArgb(186, 230, 253)
            Me.lblHeader.Dock = DockStyle.Top
            Me.lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            Me.lblHeader.ForeColor = Color.FromArgb(12, 74, 110)
            Me.lblHeader.Location = New Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New Size(1264, 42)
            Me.lblHeader.TabIndex = 0
            Me.lblHeader.Text = "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล"
            Me.lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' grp1
            ' 
            Me.grp1.BackColor = Color.White
            Me.grp1.Controls.Add(Me.lnkOpenImportFolder)
            Me.grp1.Controls.Add(Me.lblSubDistrictCount)
            Me.grp1.Controls.Add(Me.lblDistrictCount)
            Me.grp1.Controls.Add(Me.lblProvinceCount)
            Me.grp1.Controls.Add(Me.Label2)
            Me.grp1.Controls.Add(Me.lblDistrictFile)
            Me.grp1.Controls.Add(Me.lblProvinceFile)
            Me.grp1.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.grp1.ForeColor = Color.FromArgb(12, 74, 110)
            Me.grp1.Location = New Point(20, 60)
            Me.grp1.Name = "grp1"
            Me.grp1.Size = New Size(840, 130)
            Me.grp1.TabIndex = 1
            Me.grp1.TabStop = False
            Me.grp1.Text = "ไฟล์ CSV ต้นฉบับ"
            ' 
            ' lblProvinceFile
            ' 
            Me.lblProvinceFile.AutoSize = True
            Me.lblProvinceFile.Font = New Font("Tahoma", 10.0!)
            Me.lblProvinceFile.Location = New Point(20, 30)
            Me.lblProvinceFile.Name = "lblProvinceFile"
            Me.lblProvinceFile.Size = New Size(101, 21)
            Me.lblProvinceFile.TabIndex = 0
            Me.lblProvinceFile.Text = "province.csv:"
            ' 
            ' lblDistrictFile
            ' 
            Me.lblDistrictFile.AutoSize = True
            Me.lblDistrictFile.Font = New Font("Tahoma", 10.0!)
            Me.lblDistrictFile.Location = New Point(20, 60)
            Me.lblDistrictFile.Name = "lblDistrictFile"
            Me.lblDistrictFile.Size = New Size(100, 21)
            Me.lblDistrictFile.TabIndex = 1
            Me.lblDistrictFile.Text = "amphoe.csv:"
            ' 
            ' Label2
            ' 
            Me.Label2.AutoSize = True
            Me.Label2.Font = New Font("Tahoma", 10.0!)
            Me.Label2.Location = New Point(20, 90)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New Size(100, 21)
            Me.Label2.TabIndex = 2
            Me.Label2.Text = "tambon.csv:"
            ' 
            ' lblProvinceCount
            ' 
            Me.lblProvinceCount.AutoSize = True
            Me.lblProvinceCount.ForeColor = Color.FromArgb(22, 101, 52)
            Me.lblProvinceCount.Location = New Point(200, 30)
            Me.lblProvinceCount.Name = "lblProvinceCount"
            Me.lblProvinceCount.Size = New Size(17, 21)
            Me.lblProvinceCount.TabIndex = 3
            Me.lblProvinceCount.Text = "-"
            ' 
            ' lblDistrictCount
            ' 
            Me.lblDistrictCount.AutoSize = True
            Me.lblDistrictCount.ForeColor = Color.FromArgb(22, 101, 52)
            Me.lblDistrictCount.Location = New Point(200, 60)
            Me.lblDistrictCount.Name = "lblDistrictCount"
            Me.lblDistrictCount.Size = New Size(17, 21)
            Me.lblDistrictCount.TabIndex = 4
            Me.lblDistrictCount.Text = "-"
            ' 
            ' lblSubDistrictCount
            ' 
            Me.lblSubDistrictCount.AutoSize = True
            Me.lblSubDistrictCount.ForeColor = Color.FromArgb(22, 101, 52)
            Me.lblSubDistrictCount.Location = New Point(200, 90)
            Me.lblSubDistrictCount.Name = "lblSubDistrictCount"
            Me.lblSubDistrictCount.Size = New Size(17, 21)
            Me.lblSubDistrictCount.TabIndex = 5
            Me.lblSubDistrictCount.Text = "-"
            ' 
            ' lnkOpenImportFolder
            ' 
            Me.lnkOpenImportFolder.AutoSize = True
            Me.lnkOpenImportFolder.Location = New Point(580, 30)
            Me.lnkOpenImportFolder.Name = "lnkOpenImportFolder"
            Me.lnkOpenImportFolder.Size = New Size(198, 21)
            Me.lnkOpenImportFolder.TabIndex = 6
            Me.lnkOpenImportFolder.TabStop = True
            Me.lnkOpenImportFolder.Text = "📂 เปิดโฟลเดอร์ Import"
            ' 
            ' grp2
            ' 
            Me.grp2.BackColor = Color.White
            Me.grp2.Controls.Add(Me.btnRebuild)
            Me.grp2.Controls.Add(Me.btnUpdate)
            Me.grp2.Controls.Add(Me.btnImport)
            Me.grp2.Controls.Add(Me.chkClearBeforeImport)
            Me.grp2.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.grp2.ForeColor = Color.FromArgb(12, 74, 110)
            Me.grp2.Location = New Point(20, 200)
            Me.grp2.Name = "grp2"
            Me.grp2.Size = New Size(840, 90)
            Me.grp2.TabIndex = 2
            Me.grp2.TabStop = False
            Me.grp2.Text = "ขั้นตอนการนำเข้า"
            ' 
            ' chkClearBeforeImport
            ' 
            Me.chkClearBeforeImport.Font = New Font("Tahoma", 10.0!)
            Me.chkClearBeforeImport.Location = New Point(20, 30)
            Me.chkClearBeforeImport.Name = "chkClearBeforeImport"
            Me.chkClearBeforeImport.Size = New Size(340, 40)
            Me.chkClearBeforeImport.TabIndex = 0
            Me.chkClearBeforeImport.Text = "ล้างข้อมูลเก่าก่อนนำเข้า (แนะนำครั้งแรก)"
            ' 
            ' btnImport
            ' 
            Me.btnImport.BackColor = Color.FromArgb(37, 99, 235)
            Me.btnImport.Cursor = Cursors.Hand
            Me.btnImport.FlatStyle = FlatStyle.Flat
            Me.btnImport.ForeColor = Color.White
            Me.btnImport.Location = New Point(380, 22)
            Me.btnImport.Name = "btnImport"
            Me.btnImport.Size = New Size(140, 50)
            Me.btnImport.TabIndex = 1
            Me.btnImport.Text = "1⃣ นำเข้าใหม่"
            Me.btnImport.UseVisualStyleBackColor = False
            ' 
            ' btnUpdate
            ' 
            Me.btnUpdate.BackColor = Color.FromArgb(5, 150, 105)
            Me.btnUpdate.Cursor = Cursors.Hand
            Me.btnUpdate.FlatStyle = FlatStyle.Flat
            Me.btnUpdate.ForeColor = Color.White
            Me.btnUpdate.Location = New Point(530, 22)
            Me.btnUpdate.Name = "btnUpdate"
            Me.btnUpdate.Size = New Size(140, 50)
            Me.btnUpdate.TabIndex = 2
            Me.btnUpdate.Text = "2⃣ อัปเดตเพิ่ม"
            Me.btnUpdate.UseVisualStyleBackColor = False
            ' 
            ' btnRebuild
            ' 
            Me.btnRebuild.BackColor = Color.FromArgb(180, 83, 9)
            Me.btnRebuild.Cursor = Cursors.Hand
            Me.btnRebuild.FlatStyle = FlatStyle.Flat
            Me.btnRebuild.ForeColor = Color.White
            Me.btnRebuild.Location = New Point(680, 22)
            Me.btnRebuild.Name = "btnRebuild"
            Me.btnRebuild.Size = New Size(140, 50)
            Me.btnRebuild.TabIndex = 3
            Me.btnRebuild.Text = "3⃣ สร้างใหม่ทั้งหมด"
            Me.btnRebuild.UseVisualStyleBackColor = False
            ' 
            ' grp3
            ' 
            Me.grp3.BackColor = Color.White
            Me.grp3.Controls.Add(Me.lblStatus)
            Me.grp3.Controls.Add(Me.Label4)
            Me.grp3.Controls.Add(Me.Label3)
            Me.grp3.Controls.Add(Me.btnCheck)
            Me.grp3.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.grp3.ForeColor = Color.FromArgb(12, 74, 110)
            Me.grp3.Location = New Point(870, 60)
            Me.grp3.Name = "grp3"
            Me.grp3.Size = New Size(330, 230)
            Me.grp3.TabIndex = 3
            Me.grp3.TabStop = False
            Me.grp3.Text = "ตรวจสอบ"
            ' 
            ' btnCheck
            ' 
            Me.btnCheck.BackColor = Color.FromArgb(79, 70, 229)
            Me.btnCheck.Cursor = Cursors.Hand
            Me.btnCheck.FlatStyle = FlatStyle.Flat
            Me.btnCheck.ForeColor = Color.White
            Me.btnCheck.Location = New Point(20, 30)
            Me.btnCheck.Name = "btnCheck"
            Me.btnCheck.Size = New Size(290, 52)
            Me.btnCheck.TabIndex = 0
            Me.btnCheck.Text = "🔍 นับจำนวนในฐานข้อมูล"
            Me.btnCheck.UseVisualStyleBackColor = False
            ' 
            ' Label3
            ' 
            Me.Label3.AutoSize = True
            Me.Label3.Location = New Point(20, 96)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New Size(137, 21)
            Me.Label3.TabIndex = 1
            Me.Label3.Text = "รายการในตาราง:"
            ' 
            ' Label4
            ' 
            Me.Label4.AutoSize = True
            Me.Label4.Font = New Font("Tahoma", 9.5!)
            Me.Label4.ForeColor = Color.FromArgb(75, 85, 99)
            Me.Label4.Location = New Point(20, 124)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New Size(229, 19)
            Me.Label4.TabIndex = 2
            Me.Label4.Text = "Province / District / SubDistrict"
            ' 
            ' lblStatus
            ' 
            Me.lblStatus.AutoSize = True
            Me.lblStatus.ForeColor = Color.FromArgb(22, 101, 52)
            Me.lblStatus.Location = New Point(20, 160)
            Me.lblStatus.Name = "lblStatus"
            Me.lblStatus.Size = New Size(126, 21)
            Me.lblStatus.TabIndex = 3
            Me.lblStatus.Text = "รอการตรวจสอบ"
            ' 
            ' grp4
            ' 
            Me.grp4.BackColor = Color.White
            Me.grp4.Controls.Add(Me.lblProgress)
            Me.grp4.Controls.Add(Me.prgImport)
            Me.grp4.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.grp4.ForeColor = Color.FromArgb(12, 74, 110)
            Me.grp4.Location = New Point(20, 300)
            Me.grp4.Name = "grp4"
            Me.grp4.Size = New Size(1180, 90)
            Me.grp4.TabIndex = 4
            Me.grp4.TabStop = False
            Me.grp4.Text = "Progress"
            ' 
            ' prgImport
            ' 
            Me.prgImport.Location = New Point(20, 32)
            Me.prgImport.Name = "prgImport"
            Me.prgImport.Size = New Size(800, 32)
            Me.prgImport.TabIndex = 0
            ' 
            ' lblProgress
            ' 
            Me.lblProgress.AutoSize = True
            Me.lblProgress.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.lblProgress.Location = New Point(840, 32)
            Me.lblProgress.Name = "lblProgress"
            Me.lblProgress.Size = New Size(108, 22)
            Me.lblProgress.TabIndex = 1
            Me.lblProgress.Text = "รอการทำงาน"
            ' 
            ' grp5
            ' 
            Me.grp5.BackColor = Color.White
            Me.grp5.Controls.Add(Me.lblLastImport)
            Me.grp5.Controls.Add(Me.lblLastImportTitle)
            Me.grp5.Controls.Add(Me.rtbLog)
            Me.grp5.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.grp5.ForeColor = Color.FromArgb(12, 74, 110)
            Me.grp5.Location = New Point(20, 400)
            Me.grp5.Name = "grp5"
            Me.grp5.Size = New Size(1180, 280)
            Me.grp5.TabIndex = 5
            Me.grp5.TabStop = False
            Me.grp5.Text = "ประวัติการทำงานล่าสุด"
            ' 
            ' rtbLog
            ' 
            Me.rtbLog.BackColor = Color.FromArgb(15, 23, 42)
            Me.rtbLog.Font = New Font("Consolas", 9.0!)
            Me.rtbLog.ForeColor = Color.FromArgb(226, 232, 240)
            Me.rtbLog.Location = New Point(16, 32)
            Me.rtbLog.Name = "rtbLog"
            Me.rtbLog.ReadOnly = True
            Me.rtbLog.Size = New Size(1148, 200)
            Me.rtbLog.TabIndex = 0
            Me.rtbLog.Text = ""
            ' 
            ' lblLastImportTitle
            ' 
            Me.lblLastImportTitle.AutoSize = True
            Me.lblLastImportTitle.Location = New Point(16, 245)
            Me.lblLastImportTitle.Name = "lblLastImportTitle"
            Me.lblLastImportTitle.Size = New Size(93, 21)
            Me.lblLastImportTitle.TabIndex = 1
            Me.lblLastImportTitle.Text = "ครั้งล่าสุด:"
            ' 
            ' lblLastImport
            ' 
            Me.lblLastImport.AutoSize = True
            Me.lblLastImport.ForeColor = Color.FromArgb(79, 70, 229)
            Me.lblLastImport.Location = New Point(120, 245)
            Me.lblLastImport.Name = "lblLastImport"
            Me.lblLastImport.Size = New Size(17, 21)
            Me.lblLastImport.TabIndex = 2
            Me.lblLastImport.Text = "-"
            ' 
            ' btnClose
            ' 
            Me.btnClose.BackColor = Color.FromArgb(75, 85, 99)
            Me.btnClose.Cursor = Cursors.Hand
            Me.btnClose.FlatStyle = FlatStyle.Flat
            Me.btnClose.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.btnClose.ForeColor = Color.White
            Me.btnClose.Location = New Point(1050, 690)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New Size(150, 50)
            Me.btnClose.TabIndex = 6
            Me.btnClose.Text = "ปิดหน้านี้"
            Me.btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmLocationImport
            ' 
            Me.AutoScroll = True
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.ClientSize = New Size(1264, 761)
            Me.Controls.Add(Me.btnClose)
            Me.Controls.Add(Me.grp5)
            Me.Controls.Add(Me.grp4)
            Me.Controls.Add(Me.grp3)
            Me.Controls.Add(Me.grp2)
            Me.Controls.Add(Me.grp1)
            Me.Controls.Add(Me.lblHeader)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.MinimumSize = New Size(1180, 760)
            Me.Name = "FrmLocationImport"
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.Text = "นำเข้าข้อมูลจังหวัด"
            Me.grp1.ResumeLayout(False)
            Me.grp1.PerformLayout()
            Me.grp2.ResumeLayout(False)
            Me.grp3.ResumeLayout(False)
            Me.grp3.PerformLayout()
            Me.grp4.ResumeLayout(False)
            Me.grp4.PerformLayout()
            Me.grp5.ResumeLayout(False)
            Me.grp5.PerformLayout()
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
