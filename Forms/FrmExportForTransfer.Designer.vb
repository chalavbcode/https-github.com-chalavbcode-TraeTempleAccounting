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
            lblHeader.Size = New Size(880, 46)
            lblHeader.TabIndex = 0
            lblHeader.Text = "📤 ส่งออกข้อมูลไปเครื่องอื่น (โอนไปยังเครื่องอื่น / แฟลชไดรฟ์)"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' grpMachine
            ' 
            grpMachine.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            grpMachine.BackColor = Color.White
            grpMachine.Controls.Add(lblMachineStatus)
            grpMachine.Controls.Add(txtMachineID)
            grpMachine.Controls.Add(btnSaveMachineID)
            grpMachine.Controls.Add(lblMachineHint)
            grpMachine.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpMachine.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpMachine.Location = New Point(20, 56)
            grpMachine.Name = "grpMachine"
            grpMachine.Size = New Size(840, 145)
            grpMachine.TabIndex = 1
            grpMachine.TabStop = False
            grpMachine.Text = "1️⃣ รหัสเครื่องต้นทาง — ตั้งครั้งเดียวต่อเครื่อง"
            ' 
            ' lblMachineStatus
            ' 
            lblMachineStatus.AutoSize = True
            lblMachineStatus.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            lblMachineStatus.ForeColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            lblMachineStatus.Location = New Point(20, 28)
            lblMachineStatus.Name = "lblMachineStatus"
            lblMachineStatus.Size = New Size(493, 23)
            lblMachineStatus.TabIndex = 0
            lblMachineStatus.Text = "⚠️ ยังไม่ได้ตั้งค่ารหัสเครื่อง — ต้องตั้งก่อนส่งออกครั้งแรก"
            ' 
            ' txtMachineID
            ' 
            txtMachineID.Font = New Font("Tahoma", 11F)
            txtMachineID.Location = New Point(20, 62)
            txtMachineID.Name = "txtMachineID"
            txtMachineID.Size = New Size(300, 34)
            txtMachineID.TabIndex = 1
            ' 
            ' btnSaveMachineID
            ' 
            btnSaveMachineID.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSaveMachineID.Cursor = Cursors.Hand
            btnSaveMachineID.FlatStyle = FlatStyle.Flat
            btnSaveMachineID.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSaveMachineID.ForeColor = Color.White
            btnSaveMachineID.Location = New Point(332, 60)
            btnSaveMachineID.Name = "btnSaveMachineID"
            btnSaveMachineID.Size = New Size(260, 34)
            btnSaveMachineID.TabIndex = 2
            btnSaveMachineID.Text = "💾 ตั้งค่า/บันทึกรหัสเครื่อง"
            btnSaveMachineID.UseVisualStyleBackColor = False
            ' 
            ' lblMachineHint
            ' 
            lblMachineHint.AutoSize = True
            lblMachineHint.Font = New Font("Tahoma", 9F)
            lblMachineHint.ForeColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            lblMachineHint.Location = New Point(20, 102)
            lblMachineHint.Name = "lblMachineHint"
            lblMachineHint.Size = New Size(633, 44)
            lblMachineHint.TabIndex = 3
            lblMachineHint.Text = "💡 เช่น KIOSK-01, สำนักงานใหญ่ — รหัสนี้จะถูกฝังในไฟล์ส่งออกทุกไฟล์" & vbCrLf & "ฝั่งนำเข้า (FrmMultiMachineImport) จะอ่านจากไฟล์โดยตรง ไม่ใช้ชื่อไฟล์เป็นตัวระบุ"
            ' 
            ' grpRange
            ' 
            grpRange.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
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
            grpRange.Location = New Point(20, 209)
            grpRange.Name = "grpRange"
            grpRange.Size = New Size(840, 185)
            grpRange.TabIndex = 2
            grpRange.TabStop = False
            grpRange.Text = "2️⃣ ช่วงข้อมูลที่จะส่งออก"
            ' 
            ' rbSinceLast
            ' 
            rbSinceLast.AutoSize = True
            rbSinceLast.Checked = True
            rbSinceLast.Font = New Font("Tahoma", 10F)
            rbSinceLast.ForeColor = Color.FromArgb(CByte(30), CByte(41), CByte(59))
            rbSinceLast.Location = New Point(16, 30)
            rbSinceLast.Name = "rbSinceLast"
            rbSinceLast.Size = New Size(461, 28)
            rbSinceLast.TabIndex = 0
            rbSinceLast.TabStop = True
            rbSinceLast.Text = "เฉพาะรายการใหม่หลังการส่งออกครั้งล่าสุด (แนะนำ)"
            rbSinceLast.UseVisualStyleBackColor = True
            ' 
            ' rbCustom
            ' 
            rbCustom.AutoSize = True
            rbCustom.Font = New Font("Tahoma", 10F)
            rbCustom.ForeColor = Color.FromArgb(CByte(30), CByte(41), CByte(59))
            rbCustom.Location = New Point(16, 68)
            rbCustom.Name = "rbCustom"
            rbCustom.Size = New Size(170, 28)
            rbCustom.TabIndex = 1
            rbCustom.TabStop = True
            rbCustom.Text = "ช่วงวันที่กำหนด:"
            rbCustom.UseVisualStyleBackColor = True
            ' 
            ' dtpFrom
            ' 
            dtpFrom.Font = New Font("Tahoma", 10F)
            dtpFrom.Format = DateTimePickerFormat.Short
            dtpFrom.Location = New Point(210, 64)
            dtpFrom.Name = "dtpFrom"
            dtpFrom.Size = New Size(160, 32)
            dtpFrom.TabIndex = 2
            ' 
            ' lblTo
            ' 
            lblTo.AutoSize = True
            lblTo.Font = New Font("Tahoma", 10F)
            lblTo.ForeColor = Color.FromArgb(CByte(30), CByte(41), CByte(59))
            lblTo.Location = New Point(370, 69)
            lblTo.Name = "lblTo"
            lblTo.Size = New Size(32, 24)
            lblTo.TabIndex = 3
            lblTo.Text = "ถึง"
            ' 
            ' dtpTo
            ' 
            dtpTo.Font = New Font("Tahoma", 10F)
            dtpTo.Format = DateTimePickerFormat.Short
            dtpTo.Location = New Point(417, 63)
            dtpTo.Name = "dtpTo"
            dtpTo.Size = New Size(159, 32)
            dtpTo.TabIndex = 4
            ' 
            ' rbAll
            ' 
            rbAll.AutoSize = True
            rbAll.Font = New Font("Tahoma", 10F)
            rbAll.ForeColor = Color.FromArgb(CByte(30), CByte(41), CByte(59))
            rbAll.Location = New Point(16, 106)
            rbAll.Name = "rbAll"
            rbAll.Size = New Size(402, 28)
            rbAll.TabIndex = 5
            rbAll.TabStop = True
            rbAll.Text = "ข้อมูลทั้งหมด (กรณีพิเศษ เช่น ตั้งเครื่องใหม่)"
            rbAll.UseVisualStyleBackColor = True
            ' 
            ' lblLastExport
            ' 
            lblLastExport.AutoSize = True
            lblLastExport.Font = New Font("Tahoma", 9F)
            lblLastExport.ForeColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            lblLastExport.Location = New Point(16, 146)
            lblLastExport.Name = "lblLastExport"
            lblLastExport.Size = New Size(211, 22)
            lblLastExport.TabIndex = 6
            lblLastExport.Text = "ครั้งล่าสุด: ยังไม่เคยส่งออก"
            ' 
            ' grpActions
            ' 
            grpActions.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            grpActions.BackColor = Color.White
            grpActions.Controls.Add(btnExportForTransfer)
            grpActions.Controls.Add(btnOpenExportFolder)
            grpActions.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpActions.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpActions.Location = New Point(20, 402)
            grpActions.Name = "grpActions"
            grpActions.Size = New Size(840, 90)
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
            btnExportForTransfer.Text = "📤 ส่งออกข้อมูลไปเครื่องอื่น"
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
            btnOpenExportFolder.Text = "📂 เปิดโฟลเดอร์ไฟล์ส่งออก"
            btnOpenExportFolder.UseVisualStyleBackColor = False
            ' 
            ' grpLog
            ' 
            grpLog.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            grpLog.BackColor = Color.White
            grpLog.Controls.Add(rtbLog)
            grpLog.Controls.Add(prgExport)
            grpLog.Controls.Add(lblProgress)
            grpLog.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpLog.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpLog.Location = New Point(20, 500)
            grpLog.Name = "grpLog"
            grpLog.Size = New Size(840, 115)
            grpLog.TabIndex = 4
            grpLog.TabStop = False
            grpLog.Text = "ประวัติการทำงาน"
            ' 
            ' rtbLog
            ' 
            rtbLog.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            rtbLog.BackColor = Color.FromArgb(CByte(15), CByte(23), CByte(42))
            rtbLog.Font = New Font("Consolas", 9F)
            rtbLog.ForeColor = Color.FromArgb(CByte(226), CByte(232), CByte(240))
            rtbLog.Location = New Point(16, 30)
            rtbLog.Name = "rtbLog"
            rtbLog.ReadOnly = True
            rtbLog.Size = New Size(808, 48)
            rtbLog.TabIndex = 0
            rtbLog.Text = ""
            ' 
            ' prgExport
            ' 
            prgExport.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            prgExport.Location = New Point(16, 86)
            prgExport.Name = "prgExport"
            prgExport.Size = New Size(660, 20)
            prgExport.TabIndex = 1
            ' 
            ' lblProgress
            ' 
            lblProgress.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            lblProgress.AutoSize = True
            lblProgress.Font = New Font("Tahoma", 10F)
            lblProgress.Location = New Point(690, 84)
            lblProgress.Name = "lblProgress"
            lblProgress.Size = New Size(118, 24)
            lblProgress.TabIndex = 2
            lblProgress.Text = "รอการทำงาน"
            ' 
            ' btnClose
            ' 
            btnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            btnClose.BackColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
            btnClose.Cursor = Cursors.Hand
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClose.ForeColor = Color.White
            btnClose.Location = New Point(710, 623)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(150, 44)
            btnClose.TabIndex = 5
            btnClose.Text = "ปิดหน้านี้"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmExportForTransfer
            ' 
            AutoScaleDimensions = New SizeF(144F, 144F)
            AutoScaleMode = AutoScaleMode.Dpi
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(880, 690)
            Controls.Add(btnClose)
            Controls.Add(grpLog)
            Controls.Add(grpActions)
            Controls.Add(grpRange)
            Controls.Add(grpMachine)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(850, 550)
            Name = "FrmExportForTransfer"
            StartPosition = FormStartPosition.CenterScreen
            Text = "ส่งออกข้อมูลไปเครื่องอื่น"
            grpMachine.ResumeLayout(False)
            grpMachine.PerformLayout()
            grpRange.ResumeLayout(False)
            grpRange.PerformLayout()
            grpActions.ResumeLayout(False)
            grpLog.ResumeLayout(False)
            grpLog.PerformLayout()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
