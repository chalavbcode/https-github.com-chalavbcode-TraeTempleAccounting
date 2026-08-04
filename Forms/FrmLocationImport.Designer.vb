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
            components = New Container()
            ttMain = New ToolTip(components)
            lblHeader = New Label()
            grp1 = New GroupBox()
            lnkOpenImportFolder = New LinkLabel()
            lblSubDistrictCount = New Label()
            lblDistrictCount = New Label()
            lblProvinceCount = New Label()
            Label2 = New Label()
            lblDistrictFile = New Label()
            lblProvinceFile = New Label()
            grp2 = New GroupBox()
            btnRebuild = New Button()
            btnUpdate = New Button()
            btnImport = New Button()
            chkClearBeforeImport = New CheckBox()
            grp3 = New GroupBox()
            lblStatus = New Label()
            Label4 = New Label()
            Label3 = New Label()
            btnCheck = New Button()
            grp4 = New GroupBox()
            lblProgress = New Label()
            prgImport = New ProgressBar()
            grp5 = New GroupBox()
            lblLastImport = New Label()
            lblLastImportTitle = New Label()
            rtbLog = New RichTextBox()
            btnClose = New Button()
            grp1.SuspendLayout()
            grp2.SuspendLayout()
            grp3.SuspendLayout()
            grp4.SuspendLayout()
            grp5.SuspendLayout()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(186), CByte(230), CByte(253))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 14F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1591, 42)
            lblHeader.TabIndex = 0
            lblHeader.Text = "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' grp1
            ' 
            grp1.BackColor = Color.White
            grp1.Controls.Add(lnkOpenImportFolder)
            grp1.Controls.Add(lblSubDistrictCount)
            grp1.Controls.Add(lblDistrictCount)
            grp1.Controls.Add(lblProvinceCount)
            grp1.Controls.Add(Label2)
            grp1.Controls.Add(lblDistrictFile)
            grp1.Controls.Add(lblProvinceFile)
            grp1.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grp1.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grp1.Location = New Point(20, 60)
            grp1.Name = "grp1"
            grp1.Size = New Size(965, 130)
            grp1.TabIndex = 1
            grp1.TabStop = False
            grp1.Text = "ไฟล์ CSV ต้นฉบับ"
            ' 
            ' lnkOpenImportFolder
            ' 
            lnkOpenImportFolder.AutoSize = True
            lnkOpenImportFolder.Location = New Point(580, 30)
            lnkOpenImportFolder.Name = "lnkOpenImportFolder"
            lnkOpenImportFolder.Size = New Size(236, 24)
            lnkOpenImportFolder.TabIndex = 6
            lnkOpenImportFolder.TabStop = True
            lnkOpenImportFolder.Text = "📂 เปิดโฟลเดอร์ Import"
            ' 
            ' lblSubDistrictCount
            ' 
            lblSubDistrictCount.AutoSize = True
            lblSubDistrictCount.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            lblSubDistrictCount.Location = New Point(200, 90)
            lblSubDistrictCount.Name = "lblSubDistrictCount"
            lblSubDistrictCount.Size = New Size(19, 24)
            lblSubDistrictCount.TabIndex = 5
            lblSubDistrictCount.Text = "-"
            ' 
            ' lblDistrictCount
            ' 
            lblDistrictCount.AutoSize = True
            lblDistrictCount.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            lblDistrictCount.Location = New Point(200, 60)
            lblDistrictCount.Name = "lblDistrictCount"
            lblDistrictCount.Size = New Size(19, 24)
            lblDistrictCount.TabIndex = 4
            lblDistrictCount.Text = "-"
            ' 
            ' lblProvinceCount
            ' 
            lblProvinceCount.AutoSize = True
            lblProvinceCount.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            lblProvinceCount.Location = New Point(200, 30)
            lblProvinceCount.Name = "lblProvinceCount"
            lblProvinceCount.Size = New Size(19, 24)
            lblProvinceCount.TabIndex = 3
            lblProvinceCount.Text = "-"
            ' 
            ' Label2
            ' 
            Label2.AutoSize = True
            Label2.Font = New Font("Tahoma", 10F)
            Label2.Location = New Point(20, 90)
            Label2.Name = "Label2"
            Label2.Size = New Size(119, 24)
            Label2.TabIndex = 2
            Label2.Text = "tambon.csv:"
            ' 
            ' lblDistrictFile
            ' 
            lblDistrictFile.AutoSize = True
            lblDistrictFile.Font = New Font("Tahoma", 10F)
            lblDistrictFile.Location = New Point(20, 60)
            lblDistrictFile.Name = "lblDistrictFile"
            lblDistrictFile.Size = New Size(123, 24)
            lblDistrictFile.TabIndex = 1
            lblDistrictFile.Text = "amphoe.csv:"
            ' 
            ' lblProvinceFile
            ' 
            lblProvinceFile.AutoSize = True
            lblProvinceFile.Font = New Font("Tahoma", 10F)
            lblProvinceFile.Location = New Point(20, 30)
            lblProvinceFile.Name = "lblProvinceFile"
            lblProvinceFile.Size = New Size(126, 24)
            lblProvinceFile.TabIndex = 0
            lblProvinceFile.Text = "province.csv:"
            ' 
            ' grp2
            ' 
            grp2.BackColor = Color.White
            grp2.Controls.Add(btnRebuild)
            grp2.Controls.Add(btnUpdate)
            grp2.Controls.Add(btnImport)
            grp2.Controls.Add(chkClearBeforeImport)
            grp2.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grp2.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grp2.Location = New Point(20, 200)
            grp2.Name = "grp2"
            grp2.Size = New Size(965, 90)
            grp2.TabIndex = 2
            grp2.TabStop = False
            grp2.Text = "ขั้นตอนการนำเข้า"
            ' 
            ' btnRebuild
            ' 
            btnRebuild.BackColor = Color.FromArgb(CByte(180), CByte(83), CByte(9))
            btnRebuild.Cursor = Cursors.Hand
            btnRebuild.FlatStyle = FlatStyle.Flat
            btnRebuild.ForeColor = Color.White
            btnRebuild.Location = New Point(746, 22)
            btnRebuild.Name = "btnRebuild"
            btnRebuild.Size = New Size(213, 50)
            btnRebuild.TabIndex = 3
            btnRebuild.Text = "3⃣   สร้างใหม่ทั้งหมด"
            btnRebuild.UseVisualStyleBackColor = False
            ' 
            ' btnUpdate
            ' 
            btnUpdate.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnUpdate.Cursor = Cursors.Hand
            btnUpdate.FlatStyle = FlatStyle.Flat
            btnUpdate.ForeColor = Color.White
            btnUpdate.Location = New Point(577, 20)
            btnUpdate.Name = "btnUpdate"
            btnUpdate.Size = New Size(163, 50)
            btnUpdate.TabIndex = 2
            btnUpdate.Text = "2⃣   อัปเดตเพิ่ม"
            btnUpdate.UseVisualStyleBackColor = False
            ' 
            ' btnImport
            ' 
            btnImport.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnImport.Cursor = Cursors.Hand
            btnImport.FlatStyle = FlatStyle.Flat
            btnImport.ForeColor = Color.White
            btnImport.Location = New Point(416, 22)
            btnImport.Name = "btnImport"
            btnImport.Size = New Size(155, 50)
            btnImport.TabIndex = 1
            btnImport.Text = "1⃣   นำเข้าใหม่"
            btnImport.UseVisualStyleBackColor = False
            ' 
            ' chkClearBeforeImport
            ' 
            chkClearBeforeImport.Font = New Font("Tahoma", 10F)
            chkClearBeforeImport.Location = New Point(20, 30)
            chkClearBeforeImport.Name = "chkClearBeforeImport"
            chkClearBeforeImport.Size = New Size(390, 40)
            chkClearBeforeImport.TabIndex = 0
            chkClearBeforeImport.Text = "ล้างข้อมูลเก่าก่อนนำเข้า (แนะนำครั้งแรก)"
            ' 
            ' grp3
            ' 
            grp3.BackColor = Color.White
            grp3.Controls.Add(lblStatus)
            grp3.Controls.Add(Label4)
            grp3.Controls.Add(Label3)
            grp3.Controls.Add(btnCheck)
            grp3.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grp3.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grp3.Location = New Point(991, 60)
            grp3.Name = "grp3"
            grp3.Size = New Size(588, 230)
            grp3.TabIndex = 3
            grp3.TabStop = False
            grp3.Text = "ตรวจสอบ"
            ' 
            ' lblStatus
            ' 
            lblStatus.AutoSize = True
            lblStatus.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            lblStatus.Location = New Point(20, 160)
            lblStatus.Name = "lblStatus"
            lblStatus.Size = New Size(158, 24)
            lblStatus.TabIndex = 3
            lblStatus.Text = "รอการตรวจสอบ"
            ' 
            ' Label4
            ' 
            Label4.AutoSize = True
            Label4.Font = New Font("Tahoma", 9.5F)
            Label4.ForeColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            Label4.Location = New Point(20, 124)
            Label4.Name = "Label4"
            Label4.Size = New Size(265, 23)
            Label4.TabIndex = 2
            Label4.Text = "Province / District / SubDistrict"
            ' 
            ' Label3
            ' 
            Label3.AutoSize = True
            Label3.Location = New Point(20, 96)
            Label3.Name = "Label3"
            Label3.Size = New Size(170, 24)
            Label3.TabIndex = 1
            Label3.Text = "รายการในตาราง:"
            ' 
            ' btnCheck
            ' 
            btnCheck.BackColor = Color.FromArgb(CByte(79), CByte(70), CByte(229))
            btnCheck.Cursor = Cursors.Hand
            btnCheck.FlatStyle = FlatStyle.Flat
            btnCheck.ForeColor = Color.White
            btnCheck.Location = New Point(20, 30)
            btnCheck.Name = "btnCheck"
            btnCheck.Size = New Size(290, 52)
            btnCheck.TabIndex = 0
            btnCheck.Text = "🔍 นับจำนวนในฐานข้อมูล"
            btnCheck.UseVisualStyleBackColor = False
            ' 
            ' grp4
            ' 
            grp4.BackColor = Color.White
            grp4.Controls.Add(lblProgress)
            grp4.Controls.Add(prgImport)
            grp4.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grp4.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grp4.Location = New Point(20, 300)
            grp4.Name = "grp4"
            grp4.Size = New Size(1559, 90)
            grp4.TabIndex = 4
            grp4.TabStop = False
            grp4.Text = "Progress"
            ' 
            ' lblProgress
            ' 
            lblProgress.AutoSize = True
            lblProgress.Font = New Font("Tahoma", 10.5F, FontStyle.Bold)
            lblProgress.Location = New Point(1375, 39)
            lblProgress.Name = "lblProgress"
            lblProgress.Size = New Size(134, 25)
            lblProgress.TabIndex = 1
            lblProgress.Text = "รอการทำงาน"
            ' 
            ' prgImport
            ' 
            prgImport.Location = New Point(20, 32)
            prgImport.Name = "prgImport"
            prgImport.Size = New Size(1333, 32)
            prgImport.TabIndex = 0
            ' 
            ' grp5
            ' 
            grp5.BackColor = Color.White
            grp5.Controls.Add(lblLastImport)
            grp5.Controls.Add(lblLastImportTitle)
            grp5.Controls.Add(rtbLog)
            grp5.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grp5.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grp5.Location = New Point(20, 400)
            grp5.Name = "grp5"
            grp5.Size = New Size(1559, 280)
            grp5.TabIndex = 5
            grp5.TabStop = False
            grp5.Text = "ประวัติการทำงานล่าสุด"
            ' 
            ' lblLastImport
            ' 
            lblLastImport.AutoSize = True
            lblLastImport.ForeColor = Color.FromArgb(CByte(79), CByte(70), CByte(229))
            lblLastImport.Location = New Point(120, 245)
            lblLastImport.Name = "lblLastImport"
            lblLastImport.Size = New Size(19, 24)
            lblLastImport.TabIndex = 2
            lblLastImport.Text = "-"
            ' 
            ' lblLastImportTitle
            ' 
            lblLastImportTitle.AutoSize = True
            lblLastImportTitle.Location = New Point(16, 245)
            lblLastImportTitle.Name = "lblLastImportTitle"
            lblLastImportTitle.Size = New Size(104, 24)
            lblLastImportTitle.TabIndex = 1
            lblLastImportTitle.Text = "ครั้งล่าสุด:"
            ' 
            ' rtbLog
            ' 
            rtbLog.BackColor = Color.FromArgb(CByte(15), CByte(23), CByte(42))
            rtbLog.Font = New Font("Consolas", 9F)
            rtbLog.ForeColor = Color.FromArgb(CByte(226), CByte(232), CByte(240))
            rtbLog.Location = New Point(16, 32)
            rtbLog.Name = "rtbLog"
            rtbLog.ReadOnly = True
            rtbLog.Size = New Size(1523, 200)
            rtbLog.TabIndex = 0
            rtbLog.Text = ""
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.Cursor = Cursors.Hand
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1429, 686)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(150, 50)
            btnClose.TabIndex = 6
            btnClose.Text = "ปิดหน้านี้"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmLocationImport
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1591, 781)
            Controls.Add(btnClose)
            Controls.Add(grp5)
            Controls.Add(grp4)
            Controls.Add(grp3)
            Controls.Add(grp2)
            Controls.Add(grp1)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1180, 760)
            Name = "FrmLocationImport"
            StartPosition = FormStartPosition.CenterScreen
            Text = "นำเข้าข้อมูลจังหวัด"
            grp1.ResumeLayout(False)
            grp1.PerformLayout()
            grp2.ResumeLayout(False)
            grp3.ResumeLayout(False)
            grp3.PerformLayout()
            grp4.ResumeLayout(False)
            grp4.PerformLayout()
            grp5.ResumeLayout(False)
            grp5.PerformLayout()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
