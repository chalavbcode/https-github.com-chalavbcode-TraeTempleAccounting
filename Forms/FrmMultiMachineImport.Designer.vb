Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmMultiMachineImport
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents lblHeader As Label

        Friend WithEvents grpFiles As GroupBox
        Friend WithEvents btnSelectFiles As Button
        Friend WithEvents btnClearFiles As Button
        Friend WithEvents lblFileSummary As Label

        Friend WithEvents grpMachine As GroupBox
        Friend WithEvents lblMachineInfo As Label
        Friend WithEvents lblMachineStatus As Label

        Friend WithEvents grpPreview As GroupBox
        Friend WithEvents dgvImportPreview As DataGridView
        Friend WithEvents btnViewDuplicates As Button
        Friend WithEvents lblPreviewNote As Label

        Friend WithEvents grpMerge As GroupBox
        Friend WithEvents btnConfirmMerge As Button
        Friend WithEvents lblMergeStatus As Label

        Friend WithEvents grpLog As GroupBox
        Friend WithEvents rtbLog As RichTextBox
        Friend WithEvents prgImport As ProgressBar
        Friend WithEvents lblProgress As Label

        Friend WithEvents btnClose As Button
        Friend WithEvents ttMain As ToolTip

        Friend WithEvents colFileName As DataGridViewTextBoxColumn
        Friend WithEvents colMachineId As DataGridViewTextBoxColumn
        Friend WithEvents colHashStatus As DataGridViewTextBoxColumn
        Friend WithEvents colTotalCount As DataGridViewTextBoxColumn
        Friend WithEvents colIncomeSum As DataGridViewTextBoxColumn
        Friend WithEvents colExpenseSum As DataGridViewTextBoxColumn
        Friend WithEvents colDupStatus As DataGridViewTextBoxColumn
        Friend WithEvents colWarnings As DataGridViewTextBoxColumn

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
            grpFiles = New GroupBox()
            btnSelectFiles = New Button()
            btnClearFiles = New Button()
            lblFileSummary = New Label()
            grpMachine = New GroupBox()
            lblMachineInfo = New Label()
            lblMachineStatus = New Label()
            grpPreview = New GroupBox()
            dgvImportPreview = New DataGridView()
            colFileName = New DataGridViewTextBoxColumn()
            colMachineId = New DataGridViewTextBoxColumn()
            colHashStatus = New DataGridViewTextBoxColumn()
            colTotalCount = New DataGridViewTextBoxColumn()
            colIncomeSum = New DataGridViewTextBoxColumn()
            colExpenseSum = New DataGridViewTextBoxColumn()
            colDupStatus = New DataGridViewTextBoxColumn()
            colWarnings = New DataGridViewTextBoxColumn()
            btnViewDuplicates = New Button()
            lblPreviewNote = New Label()
            grpMerge = New GroupBox()
            btnConfirmMerge = New Button()
            lblMergeStatus = New Label()
            grpLog = New GroupBox()
            rtbLog = New RichTextBox()
            prgImport = New ProgressBar()
            lblProgress = New Label()
            btnClose = New Button()
            grpFiles.SuspendLayout()
            grpMachine.SuspendLayout()
            grpPreview.SuspendLayout()
            CType(dgvImportPreview, ISupportInitialize).BeginInit()
            grpMerge.SuspendLayout()
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
            lblHeader.Text = "🖥️ นำเข้าข้อมูลหลายเครื่อง (Multi-Machine Import / Flash Drive Merge)"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' grpFiles
            ' 
            grpFiles.BackColor = Color.White
            grpFiles.Controls.Add(btnSelectFiles)
            grpFiles.Controls.Add(btnClearFiles)
            grpFiles.Controls.Add(lblFileSummary)
            grpFiles.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpFiles.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpFiles.Location = New Point(20, 60)
            grpFiles.Name = "grpFiles"
            grpFiles.Size = New Size(965, 120)
            grpFiles.TabIndex = 1
            grpFiles.TabStop = False
            grpFiles.Text = "1️⃣ เลือกไฟล์ Export จากหลายเครื่อง / Flash Drive"
            ' 
            ' btnSelectFiles
            ' 
            btnSelectFiles.BackColor = Color.FromArgb(CByte(37), CByte(99), CByte(235))
            btnSelectFiles.Cursor = Cursors.Hand
            btnSelectFiles.FlatStyle = FlatStyle.Flat
            btnSelectFiles.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnSelectFiles.ForeColor = Color.White
            btnSelectFiles.Location = New Point(20, 30)
            btnSelectFiles.Name = "btnSelectFiles"
            btnSelectFiles.Size = New Size(300, 50)
            btnSelectFiles.TabIndex = 0
            btnSelectFiles.Text = "📂 เลือกไฟล์ Export (เลือกได้หลายไฟล์)"
            btnSelectFiles.UseVisualStyleBackColor = False
            ' 
            ' btnClearFiles
            ' 
            btnClearFiles.BackColor = Color.FromArgb(CByte(100), CByte(116), CByte(139))
            btnClearFiles.Cursor = Cursors.Hand
            btnClearFiles.FlatStyle = FlatStyle.Flat
            btnClearFiles.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnClearFiles.ForeColor = Color.White
            btnClearFiles.Location = New Point(330, 30)
            btnClearFiles.Name = "btnClearFiles"
            btnClearFiles.Size = New Size(150, 50)
            btnClearFiles.TabIndex = 1
            btnClearFiles.Text = "🗑️ ล้างรายการ"
            btnClearFiles.UseVisualStyleBackColor = False
            ' 
            ' lblFileSummary
            ' 
            lblFileSummary.AutoSize = True
            lblFileSummary.Font = New Font("Tahoma", 10F)
            lblFileSummary.ForeColor = Color.FromArgb(75, 85, 99)
            lblFileSummary.Location = New Point(20, 92)
            lblFileSummary.Name = "lblFileSummary"
            lblFileSummary.Size = New Size(320, 24)
            lblFileSummary.TabIndex = 2
            lblFileSummary.Text = "ยังไม่ได้เลือกไฟล์ — คลิกปุ่มด้านบนเพื่อเริ่ม"
            ' 
            ' grpMachine
            ' 
            grpMachine.BackColor = Color.White
            grpMachine.Controls.Add(lblMachineInfo)
            grpMachine.Controls.Add(lblMachineStatus)
            grpMachine.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpMachine.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpMachine.Location = New Point(991, 60)
            grpMachine.Name = "grpMachine"
            grpMachine.Size = New Size(588, 120)
            grpMachine.TabIndex = 2
            grpMachine.TabStop = False
            grpMachine.Text = "2️⃣ รหัสเครื่องต้นทาง (Machine ID) และ File Hash"
            ' 
            ' lblMachineInfo
            ' 
            lblMachineInfo.Font = New Font("Tahoma", 9.5F)
            lblMachineInfo.ForeColor = Color.FromArgb(75, 85, 99)
            lblMachineInfo.Location = New Point(16, 26)
            lblMachineInfo.Name = "lblMachineInfo"
            lblMachineInfo.Size = New Size(556, 60)
            lblMachineInfo.TabIndex = 0
            lblMachineInfo.Text = "ระบบจะอ่าน MachineID / KioskCode ที่ฝังอยู่ในตัวไฟล์ export" & vbCrLf &
                                 "(ไม่ใช้ชื่อไฟล์หรือชื่อเครื่อง Windows เป็นตัวระบุ)" & vbCrLf &
                                 "พร้อมคำนวณ SHA-256 ของไฟล์ทั้งไฟล์เพื่อกันการนำเข้าซ้ำ"
            ' 
            ' lblMachineStatus
            ' 
            lblMachineStatus.AutoSize = True
            lblMachineStatus.Font = New Font("Tahoma", 9.5F, FontStyle.Bold)
            lblMachineStatus.ForeColor = Color.FromArgb(153, 27, 27)
            lblMachineStatus.Location = New Point(16, 92)
            lblMachineStatus.Name = "lblMachineStatus"
            lblMachineStatus.Size = New Size(240, 23)
            lblMachineStatus.TabIndex = 1
            lblMachineStatus.Text = "⏳ ยังไม่มีการเลือกไฟล์"
            ' 
            ' grpPreview
            ' 
            grpPreview.BackColor = Color.White
            grpPreview.Controls.Add(dgvImportPreview)
            grpPreview.Controls.Add(btnViewDuplicates)
            grpPreview.Controls.Add(lblPreviewNote)
            grpPreview.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpPreview.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpPreview.Location = New Point(20, 190)
            grpPreview.Name = "grpPreview"
            grpPreview.Size = New Size(1559, 300)
            grpPreview.TabIndex = 3
            grpPreview.TabStop = False
            grpPreview.Text = "3️⃣ ตัวอย่างข้อมูลก่อนรวม (Preview ต่อไฟล์)"
            ' 
            ' dgvImportPreview
            ' 
            dgvImportPreview.AllowUserToAddRows = False
            dgvImportPreview.AllowUserToDeleteRows = False
            dgvImportPreview.AllowUserToOrderColumns = False
            dgvImportPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvImportPreview.BackgroundColor = Color.FromArgb(CByte(255), CByte(253), CByte(244))
            dgvImportPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvImportPreview.Columns.AddRange(New DataGridViewColumn() {colFileName, colMachineId, colHashStatus, colTotalCount, colIncomeSum, colExpenseSum, colDupStatus, colWarnings})
            dgvImportPreview.Location = New Point(16, 32)
            dgvImportPreview.Name = "dgvImportPreview"
            dgvImportPreview.ReadOnly = True
            dgvImportPreview.RowHeadersVisible = False
            dgvImportPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvImportPreview.Size = New Size(1527, 200)
            dgvImportPreview.TabIndex = 0
            ' 
            ' colFileName
            ' 
            colFileName.FillWeight = 110F
            colFileName.HeaderText = "Source File Name"
            colFileName.MinimumWidth = 150
            colFileName.Name = "colFileName"
            colFileName.ReadOnly = True
            ' 
            ' colMachineId
            ' 
            colMachineId.FillWeight = 70F
            colMachineId.HeaderText = "Machine ID"
            colMachineId.MinimumWidth = 100
            colMachineId.Name = "colMachineId"
            colMachineId.ReadOnly = True
            ' 
            ' colHashStatus
            ' 
            colHashStatus.FillWeight = 90F
            colHashStatus.HeaderText = "File Hash Status"
            colHashStatus.MinimumWidth = 130
            colHashStatus.Name = "colHashStatus"
            colHashStatus.ReadOnly = True
            ' 
            ' colTotalCount
            ' 
            colTotalCount.FillWeight = 45F
            colTotalCount.HeaderText = "รายการทั้งหมด"
            colTotalCount.MinimumWidth = 70
            colTotalCount.Name = "colTotalCount"
            colTotalCount.ReadOnly = True
            ' 
            ' colIncomeSum
            ' 
            colIncomeSum.FillWeight = 55F
            colIncomeSum.HeaderText = "Total Income Sum"
            colIncomeSum.MinimumWidth = 90
            colIncomeSum.Name = "colIncomeSum"
            colIncomeSum.ReadOnly = True
            ' 
            ' colExpenseSum
            ' 
            colExpenseSum.FillWeight = 55F
            colExpenseSum.HeaderText = "Total Expense Sum"
            colExpenseSum.MinimumWidth = 90
            colExpenseSum.Name = "colExpenseSum"
            colExpenseSum.ReadOnly = True
            ' 
            ' colDupStatus
            ' 
            colDupStatus.FillWeight = 95F
            colDupStatus.HeaderText = "Duplicate Status"
            colDupStatus.MinimumWidth = 140
            colDupStatus.Name = "colDupStatus"
            colDupStatus.ReadOnly = True
            ' 
            ' colWarnings
            ' 
            colWarnings.FillWeight = 70F
            colWarnings.HeaderText = "คำเตือน (Warning)"
            colWarnings.MinimumWidth = 120
            colWarnings.Name = "colWarnings"
            colWarnings.ReadOnly = True
            ' 
            ' btnViewDuplicates
            ' 
            btnViewDuplicates.BackColor = Color.FromArgb(CByte(124), CByte(58), CByte(237))
            btnViewDuplicates.Cursor = Cursors.Hand
            btnViewDuplicates.FlatStyle = FlatStyle.Flat
            btnViewDuplicates.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnViewDuplicates.ForeColor = Color.White
            btnViewDuplicates.Location = New Point(16, 242)
            btnViewDuplicates.Name = "btnViewDuplicates"
            btnViewDuplicates.Size = New Size(280, 48)
            btnViewDuplicates.TabIndex = 1
            btnViewDuplicates.Text = "👁️ ดูรายการซ้ำที่ตรวจพบ (ละเอียด)"
            btnViewDuplicates.UseVisualStyleBackColor = False
            ' 
            ' lblPreviewNote
            ' 
            lblPreviewNote.Font = New Font("Tahoma", 9.5F)
            lblPreviewNote.ForeColor = Color.FromArgb(75, 85, 99)
            lblPreviewNote.Location = New Point(310, 256)
            lblPreviewNote.Name = "lblPreviewNote"
            lblPreviewNote.Size = New Size(1220, 30)
            lblPreviewNote.TabIndex = 2
            lblPreviewNote.Text = "💡 ดับเบิลคลิกแถวไฟล์เพื่อดูรายการซ้ำของไฟล์นั้น / คลิกปุ่มด้านซ้ายเพื่อดูรายการซ้ำทั้งหมดก่อนตัดสินใจ merge — ระบบจะไม่ข้ามรายการซ้ำแบบเงียบๆ"
            ' 
            ' grpMerge
            ' 
            grpMerge.BackColor = Color.White
            grpMerge.Controls.Add(btnConfirmMerge)
            grpMerge.Controls.Add(lblMergeStatus)
            grpMerge.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpMerge.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpMerge.Location = New Point(20, 500)
            grpMerge.Name = "grpMerge"
            grpMerge.Size = New Size(1559, 90)
            grpMerge.TabIndex = 4
            grpMerge.TabStop = False
            grpMerge.Text = "4️⃣ บันทึกรวมข้อมูลลงฐานข้อมูลหลัก"
            ' 
            ' btnConfirmMerge
            ' 
            btnConfirmMerge.BackColor = Color.FromArgb(CByte(5), CByte(150), CByte(105))
            btnConfirmMerge.Cursor = Cursors.Hand
            btnConfirmMerge.Enabled = False
            btnConfirmMerge.FlatStyle = FlatStyle.Flat
            btnConfirmMerge.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            btnConfirmMerge.ForeColor = Color.White
            btnConfirmMerge.Location = New Point(20, 30)
            btnConfirmMerge.Name = "btnConfirmMerge"
            btnConfirmMerge.Size = New Size(330, 50)
            btnConfirmMerge.TabIndex = 0
            btnConfirmMerge.Text = "✅ บันทึกรวมข้อมูลลงฐานข้อมูลหลัก"
            btnConfirmMerge.UseVisualStyleBackColor = False
            ' 
            ' lblMergeStatus
            ' 
            lblMergeStatus.AutoSize = True
            lblMergeStatus.Font = New Font("Tahoma", 10F)
            lblMergeStatus.ForeColor = Color.FromArgb(75, 85, 99)
            lblMergeStatus.Location = New Point(370, 40)
            lblMergeStatus.Name = "lblMergeStatus"
            lblMergeStatus.Size = New Size(380, 24)
            lblMergeStatus.TabIndex = 1
            lblMergeStatus.Text = "ระบบจะสำรองฐานข้อมูลอัตโนมัติก่อนรวมทุกครั้ง"
            ' 
            ' grpLog
            ' 
            grpLog.BackColor = Color.White
            grpLog.Controls.Add(rtbLog)
            grpLog.Controls.Add(prgImport)
            grpLog.Controls.Add(lblProgress)
            grpLog.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            grpLog.ForeColor = Color.FromArgb(CByte(12), CByte(74), CByte(110))
            grpLog.Location = New Point(20, 600)
            grpLog.Name = "grpLog"
            grpLog.Size = New Size(1559, 120)
            grpLog.TabIndex = 5
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
            ' prgImport
            ' 
            prgImport.Location = New Point(16, 92)
            prgImport.Name = "prgImport"
            prgImport.Size = New Size(1300, 20)
            prgImport.TabIndex = 1
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
            btnClose.Location = New Point(1429, 730)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(150, 50)
            btnClose.TabIndex = 6
            btnClose.Text = "ปิดหน้านี้"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' FrmMultiMachineImport
            ' 
            AutoScroll = True
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1591, 781)
            Controls.Add(btnClose)
            Controls.Add(grpLog)
            Controls.Add(grpMerge)
            Controls.Add(grpPreview)
            Controls.Add(grpMachine)
            Controls.Add(grpFiles)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            MinimumSize = New Size(1180, 700)
            Name = "FrmMultiMachineImport"
            StartPosition = FormStartPosition.CenterScreen
            Text = "นำเข้าข้อมูลหลายเครื่อง"
            grpFiles.ResumeLayout(False)
            grpFiles.PerformLayout()
            grpMachine.ResumeLayout(False)
            grpMachine.PerformLayout()
            grpPreview.ResumeLayout(False)
            CType(dgvImportPreview, ISupportInitialize).EndInit()
            grpMerge.ResumeLayout(False)
            grpMerge.PerformLayout()
            grpLog.ResumeLayout(False)
            grpLog.PerformLayout()
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
