Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmExportForTransfer
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents lblHeader As Label

        Friend WithEvents grpMachine As GroupBox
        Friend WithEvents lblMachineStatus As Label
        Friend WithEvents txtMachineID As TextBox
        Friend WithEvents btnSaveMachineID As Button
        Friend WithEvents lblMachineHint As Label

        Friend WithEvents grpRange As GroupBox
        Friend WithEvents rbSinceLast As RadioButton
        Friend WithEvents rbCustom As RadioButton
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents lblTo As Label
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents rbAll As RadioButton
        Friend WithEvents lblLastExport As Label

        Friend WithEvents grpActions As GroupBox
        Friend WithEvents btnExportForTransfer As Button
        Friend WithEvents btnOpenExportFolder As Button

        Friend WithEvents grpLog As GroupBox
        Friend WithEvents rtbLog As RichTextBox
        Friend WithEvents prgExport As ProgressBar
        Friend WithEvents lblProgress As Label

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
            grpMachine = New GroupBox()
            lblMachineStatus = New Label()
            txtMachineID = New TextBox()
            btnSaveMachineID = New Button()
            lblMachineHint = New Label()
            grpRange = New GroupBox()
            rbSinceLast = New RadioButton()
            rbCustom = New RadioButton()
            dtpFrom = New DateTimePicker()
            lblTo = New Label()
            dtpTo = New DateTimePicker()
            rbAll = New RadioButton()
            lblLastExport = New Label()
            grpActions = New GroupBox()
            btnExportForTransfer = New Button()
            btnOpenExportFolder = New Button()
            grpLog = New GroupBox()
            rtbLog = New RichTextBox()
            prgExport = New ProgressBar()
            lblProgress = New Label()
            btnClose = New Button()
            grpMachine.SuspendLayout()
            grpRange.SuspendLayout()
            grpActions.SuspendLayout()
            grpLog.SuspendLayout()
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
            lblHeader.Text = "📤 ส่งออกข้อมูลไปเครื่องอื่น (Export Data for Transfer / Flash Drive)"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' grpMachine
            ' 
            grpMachine.BackColor = Color.White
            grpMachine.Controls.Add(lblMachineStatus)
            grpMachine.Controls.Add(txtMachineID)
            grpMachine.Controls.Add(btnSaveMachineID)
            grpMachine.Controls.Add(lblMachineHint)
            grpMachine.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpMachine.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpMachine.Location = New Point(20, 60)
            grpMachine.Name = "grpMachine"
            grpMachine.Size = New Size(965, 140)
            grpMachine.TabIndex = 1
            grpMachine.TabStop = False
            grpMachine.Text = "1️⃣ รหัสเครื่องต้นทาง (Machine ID) — ตั้งครั้งเดียวต่อเครื่อง"
            ' 
            ' lblMachineStatus
            ' 
            lblMachineStatus.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            lblMachineStatus.ForeColor = Color.FromArgb(153, 27, 27)
            lblMachineStatus.Location = New Point(20, 30)
            lblMachineStatus.Name = "lblMachineStatus"
            lblMachineStatus.Size = New Size(925, 26)
            lblMachineStatus.TabIndex = 0
            lblMachineStatus.Text = "⚠️ ยังไม่ได้ตั้งค่ารหัสเครื่อง — ต้องตั้งก่อนส่งออกครั้งแรก"
            ' 
            ' txtMachineID
            ' 
            txtMachineID.Font = New Font("Tahoma", 11F)
            txtMachineID.Location = New Point(20, 64)
            txtMachineID.Name = "txtMachineID"
            txtMachineID.Size = New Size(260, 30)
            txtMachineID.TabIndex = 1
            ' 
            ' btnSaveMachineID
            ' 
            btnSaveMachineID.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSaveMachineID.Cursor = Cursors.Hand
            btnSaveMachineID.FlatStyle = FlatStyle.Flat
            btnSaveMachineID.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSaveMachineID.ForeColor = Color.White
            btnSaveMachineID.Location = New Point(290, 62)
            btnSaveMachineID.Name = "btnSaveMachineID"
            btnSaveMachineID.Size = New Size(240, 34)
            btnSaveMachineID.TabIndex = 2
            btnSaveMachineID.Text = "💾 ตั้งค่า/บันทึกรหัสเครื่อง"
            btnSaveMachineID.UseVisualStyleBackColor = False
            ' 
            ' lblMachineHint
            ' 
            lblMachineHint.Font = New Font("Tahoma", 9F)
            lblMachineHint.ForeColor = Color.FromArgb(75, 85, 99)
            lblMachineHint.Location = New Point(20, 102)
            lblMachineHint.Name = "lblMachineHint"
            lblMachineHint.Size = New Size(925, 32)
            lblMachineHint.TabIndex = 3
            lblMachineHint.Text = "💡 เช่น KIOSK-01, Main Office — รหัสนี้จะถูกฝังในไฟล์ export ทุกไฟล์ (Metadata)" & vbCrLf &
                                 "ฝั่งนำเข้า (FrmMultiMachineImport) จะอ่านจากไฟล์โดยตรง ไม่ใช้ชื่อไฟล์เป็นตัวระบุ"
            ' 
            ' grpRange
            ' 
            grpRange.BackColor = Color.White
            grpRange.Controls.Add(rbSinceLast)
            grpRange.Controls.Add(rbCustom)
            grpRange.Controls.Add(dtpFrom)
            grpRange.Controls.Add(lblTo)
            grpRange.Controls.Add(dtpTo)
            grpRange.Controls.Add(rbAll)
            grpRange.Controls.Add(lblLastExport)
            grpRange.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpRange.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpRange.Location = New Point(991, 60)
            grpRange.Name = "grpRange"
            grpRange.Size = New Size(588, 200)
            grpRange.TabIndex = 2
            grpRange.TabStop = False
            grpRange.Text = "2️⃣ ช่วงข้อมูลที่จะส่งออก"
            ' 
            ' rbSinceLast
            ' 
            rbSinceLast.AutoSize = True
            rbSinceLast.Checked = True
            rbSinceLast.Font = New Font("Tahoma", 10F)
            rbSinceLast.ForeColor = Color.FromArgb(30, 41, 59)
            rbSinceLast.Location = New Point(16, 32)
            rbSinceLast.Name = "rbSinceLast"
            rbSinceLast.Size = New Size(420, 24)
            rbSinceLast.TabIndex = 0
            rbSinceLast.TabStop = True
            rbSinceLast.Text = "เฉพาะรายการใหม่หลังการส่งออกครั้งล่าสุด (แนะนำ)"
            rbSinceLast.UseVisualStyleBackColor = True
            ' 
            ' rbCustom
            ' 
            rbCustom.AutoSize = True
            rbCustom.Font = New Font("Tahoma", 10F)
            rbCustom.ForeColor = Color.FromArgb(30, 41, 59)
            rbCustom.Location = New Point(16, 70)
            rbCustom.Name = "rbCustom"
            rbCustom.Size = New Size(190, 24)
            rbCustom.TabIndex = 1
            rbCustom.TabStop = True
            rbCustom.Text = "ช่วงวันที่กำหนด:"
            rbCustom.UseVisualStyleBackColor = True
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 10F)
            dtpFrom.Format = DateTimePickerFormat.Short
            dtpFrom.Location = New Point(200, 66)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(110, 28)
            dtpFrom.TabIndex = 2
            ' 
            ' lblTo
            ' 
            lblTo.AutoSize = True
            lblTo.Font = New Font("Tahoma", 10F)
            lblTo.ForeColor = Color.FromArgb(30, 41, 59)
            lblTo.Location = New Point(318, 71)
            lblTo.Name = "lblTo"
            lblTo.Size = New Size(27, 24)
            lblTo.TabIndex = 3
            lblTo.Text = "ถึง"
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 10F)
            dtpTo.Format = DateTimePickerFormat.Short
            dtpTo.Location = New Point(352, 66)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(110, 28)
            dtpTo.TabIndex = 4
            ' 
            ' rbAll
            ' 
            rbAll.AutoSize = True
            rbAll.Font = New Font("Tahoma", 10F)
            rbAll.ForeColor = Color.FromArgb(30, 41, 59)
            rbAll.Location = New Point(16, 108)
            rbAll.Name = "rbAll"
            rbAll.Size = New Size(430, 24)
            rbAll.TabIndex = 5
            rbAll.TabStop = True
            rbAll.Text = "ข้อมูลทั้งหมด (กรณีพิเศษ เช่น ตั้งเครื่องใหม่)"
            rbAll.UseVisualStyleBackColor = True
            ' 
            ' lblLastExport
            ' 
            lblLastExport.Font = New Font("Tahoma", 9F)
            lblLastExport.ForeColor = Color.FromArgb(75, 85, 99)
            lblLastExport.Location = New Point(16, 150)
            lblLastExport.Name = "lblLastExport"
            lblLastExport.Size = New Size(556, 40)
            lblLastExport.TabIndex = 6
            lblLastExport.Text = "ครั้งล่าสุด: ยังไม่เคยส่งออก"
            ' 
            ' grpActions
            ' 
            grpActions.BackColor = Color.White
            grpActions.Controls.Add(btnExportForTransfer)
            grpActions.Controls.Add(btnOpenExportFolder)
            grpActions.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpActions.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpActions.Location = New Point(20, 210)
            grpActions.Name = "grpActions"
            grpActions.Size = New Size(1559, 90)
            grpActions.TabIndex = 3
            grpActions.TabStop = False
            grpActions.Text = "3️⃣ ส่งออกไฟล์"
            ' 
            ' btnExportForTransfer
            ' 
            btnExportForTransfer.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnExportForTransfer.Cursor = Cursors.Hand
            btnExportForTransfer.FlatStyle = FlatStyle.Flat
            btnExportForTransfer.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnExportForTransfer.ForeColor = Color.White
            btnExportForTransfer.Location = New Point(20, 30)
            btnExportForTransfer.Name = "btnExportForTransfer"
            btnExportForTransfer.Size = New Size(340, 50)
            btnExportForTransfer.TabIndex = 0
            btnExportForTransfer.Text = "📤 ส่งออกข้อมูลไปเครื่องอื่น (Export)"
            btnExportForTransfer.UseVisualStyleBackColor = False
            ' 
            ' btnOpenExportFolder
            ' 
            btnOpenExportFolder.BackColor = Color.FromArgb(CByte(100), CByte(116), CByte(139))
            btnOpenExportFolder.Cursor = Cursors.Hand
            btnOpenExportFolder.FlatStyle = FlatStyle.Flat
            btnOpenExportFolder.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnOpenExportFolder.ForeColor = Color.White
            btnOpenExportFolder.Location = New Point(370, 30)
            btnOpenExportFolder.Name = "btnOpenExportFolder"
            btnOpenExportFolder.Size = New Size(220, 50)
            btnOpenExportFolder.TabIndex = 1
            btnOpenExportFolder.Text = "📂 เปิดโฟลเดอร์ Export"
            btnOpenExportFolder.UseVisualStyleBackColor = False
            ' 
            ' grpLog
            ' 
            grpLog.BackColor = Color.White
            grpLog.Controls.Add(rtbLog)
            grpLog.Controls.Add(prgExport)
            grpLog.Controls.Add(lblProgress)
            grpLog.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpLog.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpLog.Location = New Point(20, 310)
            grpLog.Name = "grpLog"
            grpLog.Size = New Size(1559, 120)
            grpLog.TabIndex = 4
            grpLog.TabStop = False
            grpLog.Text = "Log ประวัติการทำงาน"
            ' 
            ' rtbLog
            ' 
            rtbLog.BackColor = Color.FromArgb(CByte(15), CByte(23), CByte(42))
            rtbLog.Font = New Font("Consolas", 9F)
            rtbLog.ForeColor = Color.FromArgb(CByte(226), CByte(232), CByte(240))
            rtbLog.Location = New Point(16, 32)
            rtbLog.Name = "rtbLog"
            rtbLog.ReadOnly = True
            rtbLog.Size = New Size(1527, 52)
            rtbLog.TabIndex = 0
            rtbLog.Text = ""
            ' 
            ' prgExport
            ' 
            prgExport.Location = New Point(16, 92)
            prgExport.Name = "prgExport"
            prgExport.Size = New Size(1300, 20)
            prgExport.TabIndex = 1
            ' 
            ' lblProgress
            ' 
            lblProgress.AutoSize = True
            lblProgress.Font = New Font("Tahoma", 10F)
            lblProgress.Location = New Point(1330, 90)
            lblProgress.Name = "lblProgress"
            lblProgress.Size = New Size(160, 24)
            lblProgress.TabIndex = 2
            lblProgress.Text = "รอการทำงาน"
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.Cursor = Cursors.Hand
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(1429, 440)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(150, 50)
            btnClose.TabIndex = 5
            btnClose.Text = "ปิดหน้านี้"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmExportForTransfer
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1591, 501)
            Controls.Add(btnClose)
            Controls.Add(grpLog)
            Controls.Add(grpActions)
            Controls.Add(grpRange)
            Controls.Add(grpMachine)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1180, 460)
            Name = "FrmExportForTransfer"
            StartPosition = FormStartPosition.CenterScreen
            Text = "ส่งออกข้อมูลไปเครื่องอื่น"
            grpMachine.ResumeLayout(False)
            grpMachine.PerformLayout()
            grpRange.ResumeLayout(False)
            grpRange.PerformLayout()
            grpActions.ResumeLayout(False)
            grpActions.PerformLayout()
            grpLog.ResumeLayout(False)
            grpLog.PerformLayout()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
