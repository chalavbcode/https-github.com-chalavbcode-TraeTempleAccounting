Option Strict Off
Option Explicit On

Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Threading
Imports System.Threading.Tasks

Namespace TempleAccounting
    ''' <summary>
    ''' หน้าต่างตรวจสอบชุดสำรองข้อมูล (Backup)
    ''' • แสดงผลตรวจสอบชุดสำรองล่าสุด 5 ชุดใน DataGridView
    ''' • สแกนไดรฟ์ที่พร้อมใช้งานเพื่อหาชุดสำรองที่ยังไม่อยู่ใน registry (มี progress + ยกเลิก)
    ''' • แสดงคำเตือนความปลอดภัย (สำรองครบ 2 ไดรฟ์) และไฟล์ใหม่ที่ยังไม่ได้สำรอง
    ''' ทำงานบน background thread (UI ไม่ค้าง)
    ''' </summary>
    Public Class FrmBackupVerify
        Inherits Form

        Private dgvResults As DataGridView
        Private btnVerify As Button
        Private btnScan As Button
        Private btnClose As Button
        Private lblStatus As Label
        Private txtSummary As TextBox
        Private _scanCts As CancellationTokenSource = Nothing

        Public Sub New()
            InitializeUI()
        End Sub

        Private Sub InitializeUI()
            Me.Text = "ตรวจสอบ Backup"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Size = New Size(980, 640)
            Me.MinimumSize = New Size(760, 480)
            Me.BackColor = Color.FromArgb(255, 253, 244)
            Me.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.MaximizeBox = True

            ' ── แถบบน: หัวข้อ + ปุ่ม ──
            Dim pnlTop As New Panel()
            pnlTop.Dock = DockStyle.Top
            pnlTop.Height = 92
            pnlTop.Padding = New Padding(16, 12, 16, 8)

            Dim lblTitle As New Label()
            lblTitle.AutoSize = True
            lblTitle.Font = New Font("Tahoma", 15.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTitle.ForeColor = Color.FromArgb(120, 53, 15)
            lblTitle.Text = "🔍 ตรวจสอบชุดสำรองข้อมูล"
            lblTitle.Location = New Point(16, 10)
            pnlTop.Controls.Add(lblTitle)

            Dim lblHint As New Label()
            lblHint.AutoSize = True
            lblHint.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblHint.ForeColor = Color.FromArgb(120, 80, 40)
            lblHint.Text = "ตรวจสอบชุดสำรองล่าสุด 5 ชุด และสแกนไดรฟ์ที่เชื่อมต่อเพื่อหาชุดสำรองที่ยังไม่อยู่ในรายการ"
            lblHint.Location = New Point(16, 40)
            pnlTop.Controls.Add(lblHint)

            btnVerify = New Button()
            btnVerify.Text = "🛡️ ตรวจสอบ Backup"
            btnVerify.Size = New Size(160, 38)
            btnVerify.Location = New Point(16, 62)
            btnVerify.FlatStyle = FlatStyle.Flat
            btnVerify.FlatAppearance.BorderSize = 0
            btnVerify.BackColor = Color.FromArgb(5, 150, 105)
            btnVerify.ForeColor = Color.White
            btnVerify.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            btnVerify.Cursor = Cursors.Hand
            AddHandler btnVerify.Click, AddressOf VerifyButton_Click
            pnlTop.Controls.Add(btnVerify)

            btnScan = New Button()
            btnScan.Text = "🔎 สแกนไดรฟ์หา Backup"
            btnScan.Size = New Size(180, 38)
            btnScan.Location = New Point(184, 62)
            btnScan.FlatStyle = FlatStyle.Flat
            btnScan.FlatAppearance.BorderSize = 0
            btnScan.BackColor = Color.FromArgb(37, 99, 235)
            btnScan.ForeColor = Color.White
            btnScan.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            btnScan.Cursor = Cursors.Hand
            AddHandler btnScan.Click, AddressOf ScanButton_Click
            pnlTop.Controls.Add(btnScan)

            btnClose = New Button()
            btnClose.Text = "ปิด"
            btnClose.Size = New Size(90, 38)
            btnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            btnClose.Location = New Point(Me.ClientSize.Width - 106, 62)
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.FlatAppearance.BorderSize = 0
            btnClose.BackColor = Color.FromArgb(146, 64, 14)
            btnClose.ForeColor = Color.White
            btnClose.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            btnClose.Cursor = Cursors.Hand
            AddHandler btnClose.Click, Sub(s, e) Me.Close()
            pnlTop.Controls.Add(btnClose)

            ' ── แถบล่าง: สถานะ + สรุป ──
            Dim pnlBottom As New Panel()
            pnlBottom.Dock = DockStyle.Bottom
            pnlBottom.Height = 150
            pnlBottom.Padding = New Padding(16, 6, 16, 10)

            lblStatus = New Label()
            lblStatus.Dock = DockStyle.Top
            lblStatus.Height = 24
            lblStatus.Font = New Font("Tahoma", 9.5!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblStatus.ForeColor = Color.FromArgb(90, 70, 40)
            lblStatus.Text = "กด 'ตรวจสอบ Backup' เพื่อเริ่มตรวจสอบชุดสำรองล่าสุด"
            pnlBottom.Controls.Add(lblStatus)

            txtSummary = New TextBox()
            txtSummary.Dock = DockStyle.Fill
            txtSummary.Multiline = True
            txtSummary.ReadOnly = True
            txtSummary.ScrollBars = ScrollBars.Vertical
            txtSummary.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            txtSummary.BackColor = Color.FromArgb(255, 253, 244)
            txtSummary.BorderStyle = BorderStyle.FixedSingle
            pnlBottom.Controls.Add(txtSummary)

            ' ── DataGridView ──
            dgvResults = New DataGridView()
            dgvResults.Dock = DockStyle.Fill
            dgvResults.ReadOnly = True
            dgvResults.AllowUserToAddRows = False
            dgvResults.AllowUserToDeleteRows = False
            dgvResults.AllowUserToOrderColumns = False
            dgvResults.RowHeadersVisible = False
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvResults.BackgroundColor = Color.White
            dgvResults.BorderStyle = BorderStyle.None
            dgvResults.DefaultCellStyle.Font = New Font("Tahoma", 9.5!, FontStyle.Regular, GraphicsUnit.Point, 222)
            dgvResults.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(120, 53, 15)
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            dgvResults.EnableHeadersVisualStyles = False
            dgvResults.ColumnHeadersHeight = 36
            BuildGridColumns()

            Me.Controls.Add(dgvResults)
            Me.Controls.Add(pnlBottom)
            Me.Controls.Add(pnlTop)

            ' ── เริ่มตรวจสอบทันทีเมื่อเปิดหน้าต่าง ──
            AddHandler Me.Shown, Sub(s, e) VerifyButton_Click(Nothing, EventArgs.Empty)
        End Sub

        Private Sub BuildGridColumns()
            dgvResults.Columns.Clear()
            AddColumn("colNum", "ครั้งที่", 60)
            AddColumn("colDate", "วันที่", 140)
            AddColumn("colDrive", "ไดรฟ์ + ตำแหน่ง", 320)
            AddColumn("colDb", "ฐานข้อมูล", 120)
            AddColumn("colReceipts", "ใบเสร็จ (n / n)", 120)
            AddColumn("colOverall", "สถานะรวม", 140)
        End Sub

        Private Sub AddColumn(name As String, header As String, width As Integer)
            Dim col As New DataGridViewTextBoxColumn()
            col.Name = name
            col.HeaderText = header
            col.FillWeight = width
            col.MinimumWidth = 60
            col.SortMode = DataGridViewColumnSortMode.NotSortable
            dgvResults.Columns.Add(col)
        End Sub

        Private Async Sub VerifyButton_Click(sender As Object, e As EventArgs)
            Await VerifyBackupsAsync()
        End Sub

        Private Async Function VerifyBackupsAsync() As Task
            SetBusy(True)
            lblStatus.Text = "กำลังตรวจสอบชุดสำรองล่าสุด..."
            Try
                Dim rows As List(Of DatabaseBackupHelper.BackupVerifyRow) =
                    Await Task.Run(Function() DatabaseBackupHelper.VerifyLastBackups(5))
                PopulateGrid(rows)
                lblStatus.Text = "ตรวจสอบเสร็จสิ้น — " & rows.Count.ToString() & " ชุดสำรอง"
                UpdateSummary()
            Catch ex As Exception
                lblStatus.Text = "เกิดข้อผิดพลาดระหว่างตรวจสอบ"
                ErrorLogHelper.LogSilentError("FrmBackupVerify.VerifyButton_Click", ex)
            Finally
                SetBusy(False)
            End Try
        End Function

        Private Async Sub ScanButton_Click(sender As Object, e As EventArgs)
            If _scanCts IsNot Nothing Then
                _scanCts.Cancel()
                lblStatus.Text = "กำลังยกเลิกการสแกน..."
                Return
            End If

            _scanCts = New CancellationTokenSource()
            Dim token As CancellationToken = _scanCts.Token
            btnScan.Text = "⏹ ยกเลิกการสแกน"
            SetBusy(True, disableScan:=False)
            lblStatus.Text = "กำลังสแกนไดรฟ์..."

            Try
                Dim prog As New Progress(Of Integer)(Sub(p)
                    If lblStatus IsNot Nothing AndAlso Not lblStatus.IsDisposed Then
                        lblStatus.Text = "กำลังสแกนไดรฟ์... " & p.ToString() & "%"
                    End If
                End Sub)

                Dim found As List(Of String) = Await Task.Run(Function() DatabaseBackupHelper.ScanDrivesForBackups(prog, token))

                If token.IsCancellationRequested Then
                    lblStatus.Text = "ยกเลิกการสแกนแล้ว"
                    Return
                End If

                lblStatus.Text = "สแกนเสร็จ — พบ " & found.Count.ToString() & " ชุดสำรอง"

                Dim missing As List(Of String) = DatabaseBackupHelper.FindBackupsMissingFromRegistry(found)
                If missing.Count > 0 Then
                    Dim sb As New System.Text.StringBuilder()
                    sb.AppendLine("พบชุดสำรองบนไดรฟ์ที่ยังไม่อยู่ในรายการ (registry) จำนวน " & missing.Count.ToString() & " ชุด:")
                    For Each m In missing
                        sb.AppendLine("• " & m)
                    Next
                    sb.AppendLine()
                    sb.AppendLine("ต้องการตรวจสอบและเพิ่มเข้าสู่รายการหรือไม่?")
                    Dim result = MessageBox.Show(Me, sb.ToString(), "พบชุดสำรองใหม่",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Dim added As Integer = 0
                        For Each m In missing
                            If DatabaseBackupHelper.AddScannedBackupToRegistry(m) Then added += 1
                        Next
                        MessageBox.Show(Me, "เพิ่มเข้าสู่รายการแล้ว " & added.ToString() & " ชุด (จาก " & missing.Count.ToString() & " ชุด)",
                                        "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        ' รีเฟรชผลตรวจสอบหลังเพิ่ม
                        Await VerifyBackupsAsync()
                        Return
                    End If
                ElseIf found.Count > 0 Then
                    MessageBox.Show(Me, "พบชุดสำรอง " & found.Count.ToString() & " ชุด และทั้งหมดอยู่ในรายการแล้ว",
                                    "สแกนไดรฟ์", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show(Me, "ไม่พบชุดสำรอง (Backup_*) บนไดรฟ์ที่เชื่อมต่อ",
                                    "สแกนไดรฟ์", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                UpdateSummary()
            Catch ex As Exception
                lblStatus.Text = "เกิดข้อผิดพลาดระหว่างสแกนไดรฟ์"
                ErrorLogHelper.LogSilentError("FrmBackupVerify.ScanButton_Click", ex)
            Finally
                If _scanCts IsNot Nothing Then
                    _scanCts.Dispose()
                    _scanCts = Nothing
                End If
                btnScan.Text = "🔎 สแกนไดรฟ์หา Backup"
                SetBusy(False)
            End Try
        End Sub

        Private Sub PopulateGrid(rows As List(Of DatabaseBackupHelper.BackupVerifyRow))
            dgvResults.Rows.Clear()
            If rows Is Nothing Then Return

            For Each r In rows
                Dim idx = dgvResults.Rows.Add()
                Dim row = dgvResults.Rows(idx)
                row.Cells("colNum").Value = r.BackupNumber
                row.Cells("colDate").Value = r.Timestamp
                row.Cells("colDrive").Value = r.DriveAndPath
                row.Cells("colDb").Value = r.DatabaseStatus
                row.Cells("colReceipts").Value = r.ReceiptsStatus
                row.Cells("colOverall").Value = r.OverallStatus

                Select Case r.Classification
                    Case BackupStatusClass.Identical
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231)
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52)
                    Case BackupStatusClass.OlderButIntact
                        row.DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195)
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(161, 98, 7)
                    Case BackupStatusClass.Corrupted, BackupStatusClass.Incomplete
                        row.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226)
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27)
                    Case BackupStatusClass.DriveMissing
                        row.DefaultCellStyle.BackColor = Color.FromArgb(226, 232, 240)
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105)
                End Select
            Next
        End Sub

        Private Sub UpdateSummary()
            Dim sb As New System.Text.StringBuilder()

            Dim safety As String = DatabaseBackupHelper.SafetyCheckTwoDrives()
            If Not String.IsNullOrEmpty(safety) Then
                sb.AppendLine(safety)
                sb.AppendLine()
            End If

            Dim unbacked As List(Of String) = DatabaseBackupHelper.FindNewUnbackedFiles()
            If unbacked.Count > 0 Then
                sb.AppendLine("📄 ไฟล์ใหม่ที่ยังไม่ได้สำรอง (" & unbacked.Count.ToString() & " ไฟล์):")
                For Each f In unbacked
                    sb.AppendLine("• " & f)
                Next
            Else
                sb.AppendLine("✅ ไม่มีไฟล์ใหม่ที่ยังไม่ได้สำรอง")
            End If

            txtSummary.Text = sb.ToString()
        End Sub

        Private Sub SetBusy(busy As Boolean, Optional disableScan As Boolean = True)
            btnVerify.Enabled = Not busy
            If disableScan Then btnScan.Enabled = Not busy
            Me.UseWaitCursor = busy
        End Sub
    End Class
End Namespace
