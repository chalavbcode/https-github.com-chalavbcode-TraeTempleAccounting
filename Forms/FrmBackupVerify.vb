Option Strict Off
Option Explicit On

Imports System
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading
Imports System.Threading.Tasks

Namespace TempleAccounting
    ''' <summary>
    ''' หน้าต่างตรวจสอบชุดสำรองข้อมูล (Backup)
    ''' • สแกนไดรฟ์ที่พร้อมใช้งาน (background thread + progress + ปุ่มยกเลิกที่ใช้ได้จริง)
    ''' • ประเมินทุกชุดที่พบ (รวมชุดรุ่นเก่าที่ไม่มี manifest) และเพิ่มเข้าสู่ registry ทุกชุด
    ''' • แสดงทุกชุดที่พบในตาราง พร้อมสถานะ/เหตุผล และชุดที่ไม่สมบูรณ์ก็มองเห็นได้
    ''' • สรุปไฟล์ที่ยังไม่ได้สำรองโดยเทียบกับชุดสำรองที่สมบูรณ์ล่าสุดเท่านั้น
    ''' </summary>
    Public Class FrmBackupVerify
        Inherits Form

        Private dgvResults As DataGridView
        Private btnVerify As Button
        Private btnScan As Button
        Private btnCancel As Button
        Private btnClose As Button
        Private lblStatus As Label
        Private lblAlert As Label
        Private prgScan As ProgressBar
        Private txtSummary As TextBox
        Private _scanCts As CancellationTokenSource = Nothing
        Private _scanning As Boolean = False

        Public Sub New()
            InitializeUI()
        End Sub

        Private Sub InitializeUI()
            Me.Text = "ตรวจสอบ Backup"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Size = New Size(1280, 720)
            Me.MinimumSize = New Size(980, 560)
            Me.BackColor = Color.FromArgb(255, 253, 244)
            Me.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.MaximizeBox = True

            ' ── แถบบน: หัวข้อ + ปุ่ม ──
            Dim pnlTop As New Panel()
            pnlTop.Dock = DockStyle.Top
            pnlTop.Height = 108
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
            lblHint.Text = "สแกนไดรฟ์ที่เชื่อมต่อ ประเมินทุกชุดสำรอง (รวมชุดรุ่นเก่าที่ไม่มี manifest) แล้วเพิ่มเข้าสู่รายการ"
            lblHint.Location = New Point(16, 38)
            pnlTop.Controls.Add(lblHint)

            btnVerify = MakeButton("🛡️ ตรวจสอบ Backup", New Point(16, 62), New Size(160, 38), Color.FromArgb(5, 150, 105))
            AddHandler btnVerify.Click, AddressOf VerifyButton_Click
            pnlTop.Controls.Add(btnVerify)

            btnScan = MakeButton("🔎 สแกนไดรฟ์หา Backup", New Point(184, 62), New Size(180, 38), Color.FromArgb(37, 99, 235))
            AddHandler btnScan.Click, AddressOf ScanButton_Click
            pnlTop.Controls.Add(btnScan)

            btnCancel = MakeButton("⏹ ยกเลิกการสแกน", New Point(372, 62), New Size(160, 38), Color.FromArgb(153, 27, 27))
            btnCancel.Visible = False
            AddHandler btnCancel.Click, AddressOf CancelButton_Click
            pnlTop.Controls.Add(btnCancel)

            btnClose = MakeButton("ปิด", New Point(Me.ClientSize.Width - 106, 62), New Size(90, 38), Color.FromArgb(146, 64, 14))
            btnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            AddHandler btnClose.Click, Sub(s, e) Me.Close()
            pnlTop.Controls.Add(btnClose)

            ' ── แถบล่าง: เตือน + progress + สถานะ + สรุป ──
            Dim pnlBottom As New Panel()
            pnlBottom.Dock = DockStyle.Bottom
            pnlBottom.Height = 200
            pnlBottom.Padding = New Padding(16, 6, 16, 10)

            lblAlert = New Label()
            lblAlert.Dock = DockStyle.Top
            lblAlert.Height = 28
            lblAlert.Font = New Font("Tahoma", 10.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblAlert.ForeColor = Color.FromArgb(153, 27, 27)
            lblAlert.Text = "กำลังตรวจสอบสถานะชุดสำรอง..."

            txtSummary = New TextBox()
            txtSummary.Dock = DockStyle.Fill
            txtSummary.Multiline = True
            txtSummary.ReadOnly = True
            txtSummary.ScrollBars = ScrollBars.Vertical
            txtSummary.WordWrap = True
            txtSummary.Font = New Font("Tahoma", 9.5!, FontStyle.Regular, GraphicsUnit.Point, 222)
            txtSummary.BackColor = Color.FromArgb(255, 253, 244)
            txtSummary.BorderStyle = BorderStyle.FixedSingle
            pnlBottom.Controls.Add(txtSummary)

            lblStatus = New Label()
            lblStatus.Dock = DockStyle.Top
            lblStatus.Height = 24
            lblStatus.Font = New Font("Tahoma", 9.5!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblStatus.ForeColor = Color.FromArgb(90, 70, 40)
            lblStatus.Text = "กด 'ตรวจสอบ Backup' เพื่อเริ่มตรวจสอบชุดสำรองล่าสุด"
            pnlBottom.Controls.Add(lblStatus)

            prgScan = New ProgressBar()
            prgScan.Dock = DockStyle.Top
            prgScan.Height = 12
            prgScan.Minimum = 0
            prgScan.Maximum = 100
            prgScan.Style = ProgressBarStyle.Continuous
            pnlBottom.Controls.Add(prgScan)

            pnlBottom.Controls.Add(lblAlert)

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
            dgvResults.DefaultCellStyle.WrapMode = DataGridViewTriState.False
            dgvResults.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(120, 53, 15)
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            dgvResults.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgvResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            dgvResults.EnableHeadersVisualStyles = False
            dgvResults.RowTemplate.Height = 26
            BuildGridColumns()

            Me.Controls.Add(dgvResults)
            Me.Controls.Add(pnlBottom)
            Me.Controls.Add(pnlTop)

            AddHandler Me.Shown, Sub(s, e) VerifyButton_Click(Nothing, EventArgs.Empty)
        End Sub

        Private Function MakeButton(text As String, location As Point, size As Size, backColor As Color) As Button
            Dim b As New Button()
            b.Text = text
            b.Size = size
            b.Location = location
            b.FlatStyle = FlatStyle.Flat
            b.FlatAppearance.BorderSize = 0
            b.BackColor = backColor
            b.ForeColor = Color.White
            b.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            b.Cursor = Cursors.Hand
            Return b
        End Function

        Private Sub BuildGridColumns()
            dgvResults.Columns.Clear()
            AddColumn("colNum", "ครั้งที่", 70)
            AddColumn("colDate", "วันที่", 150)
            AddColumn("colDrive", "ไดรฟ์ + ตำแหน่ง", 300)
            AddColumn("colDb", "ฐานข้อมูล", 130)
            AddColumn("colReceipts", "ใบเสร็จ (n/N)", 130)
            AddColumn("colOverall", "สถานะรวม", 150)
            AddColumn("colNotes", "หมายเหตุ / เหตุผล", 240)
        End Sub

        Private Sub AddColumn(name As String, header As String, minWidth As Integer)
            Dim col As New DataGridViewTextBoxColumn()
            col.Name = name
            col.HeaderText = header
            col.MinimumWidth = minWidth
            col.FillWeight = minWidth
            col.SortMode = DataGridViewColumnSortMode.NotSortable
            dgvResults.Columns.Add(col)
        End Sub

        ' ───────────────────────── ตรวจสอบชุดสำรองล่าสุด ─────────────────────────

        Private Async Sub VerifyButton_Click(sender As Object, e As EventArgs)
            If _scanning Then Return
            Await VerifyBackupsAsync()
        End Sub

        Private Async Function VerifyBackupsAsync() As Task
            SetBusy(True)
            lblStatus.Text = "กำลังตรวจสอบชุดสำรองล่าสุด..."
            prgScan.Style = ProgressBarStyle.Marquee
            Try
                Dim rows As List(Of DatabaseBackupHelper.BackupVerifyRow) =
                    Await Task.Run(Function() DatabaseBackupHelper.VerifyLastBackups(5))
                If Me.IsDisposed OrElse Me.Disposing Then Return
                PopulateGrid(rows)

                Dim info As BackupSummaryInfo =
                    Await Task.Run(Function() DatabaseBackupHelper.BuildStatusSummary())
                If Me.IsDisposed OrElse Me.Disposing Then Return
                ApplySummary(info)

                If rows.Count = 0 Then
                    lblStatus.Text = "ยังไม่มีชุดสำรองในรายการ — กด 'สแกนไดรฟ์หา Backup' เพื่อค้นหาและเพิ่ม"
                Else
                    lblStatus.Text = "ตรวจสอบเสร็จสิ้น — " & rows.Count.ToString() & " ชุดสำรองล่าสุด"
                End If
            Catch ex As Exception
                lblStatus.Text = "เกิดข้อผิดพลาดระหว่างตรวจสอบ"
                ErrorLogHelper.LogSilentError("FrmBackupVerify.VerifyButton_Click", ex)
                MessageBoxHelper.ShowError("ตรวจสอบชุดสำรองไม่สำเร็จ: " & ex.Message, "ตรวจสอบไม่สำเร็จ")
            Finally
                prgScan.Style = ProgressBarStyle.Continuous
                SetBusy(False)
            End Try
        End Function

        ' ───────────────────────── สแกนไดรฟ์ ─────────────────────────

        Private Async Sub ScanButton_Click(sender As Object, e As EventArgs)
            If _scanning Then
                CancelScan()
                Return
            End If

            _scanning = True
            _scanCts = New CancellationTokenSource()
            Dim token As CancellationToken = _scanCts.Token

            btnVerify.Enabled = False
            btnScan.Enabled = False
            btnScan.Text = "กำลังสแกน..."
            btnCancel.Visible = True
            btnCancel.Enabled = True
            prgScan.Style = ProgressBarStyle.Continuous
            prgScan.Value = 0
            lblStatus.Text = "กำลังสแกนไดรฟ์..."

            Try
                Dim prog As New Progress(Of DatabaseBackupHelper.BackupScanProgress)(
                    Sub(p)
                        If p Is Nothing OrElse Me.IsDisposed Then Return
                        Try
                            If Not prgScan.IsDisposed Then prgScan.Value = p.Percent
                            If Not lblStatus.IsDisposed AndAlso Not String.IsNullOrEmpty(p.Status) Then lblStatus.Text = p.Status
                        Catch
                        End Try
                    End Sub)

                Dim res As DatabaseBackupHelper.BackupScanResult =
                    Await Task.Run(Function() DatabaseBackupHelper.ScanAndRegisterBackups(prog, token))
                If Me.IsDisposed OrElse Me.Disposing Then Return

                If token.IsCancellationRequested OrElse res.Cancelled Then
                    lblStatus.Text = "ยกเลิกการสแกนแล้ว (ชุดที่ตรวจเสร็จก่อนยกเลิกถูกบันทึกแล้ว)"
                    If res.Outcomes.Count > 0 Then PopulateGrid(BuildRowsFromOutcomes(res.Outcomes))
                    Dim cancelledInfo As BackupSummaryInfo =
                        Await Task.Run(Function() DatabaseBackupHelper.BuildStatusSummary())
                    ApplySummary(cancelledInfo)
                    Return
                End If

                PopulateGrid(BuildRowsFromOutcomes(res.Outcomes))

                Dim info As BackupSummaryInfo =
                    Await Task.Run(Function() DatabaseBackupHelper.BuildStatusSummary())
                If Me.IsDisposed OrElse Me.Disposing Then Return
                ApplySummary(info)

                If res.Failed Then
                    lblStatus.Text = "สแกนไดรฟ์ไม่สำเร็จ"
                Else
                    lblStatus.Text = "สแกนเสร็จ — พบ " & res.FoundCount.ToString() &
                                     " ชุด • เพิ่ม " & res.AddedCount.ToString() &
                                     " ชุด • ข้าม " & res.SkippedCount.ToString() & " ชุด"
                End If

                ShowScanResult(res)
            Catch ex As Exception
                lblStatus.Text = "เกิดข้อผิดพลาดระหว่างสแกนไดรฟ์"
                ErrorLogHelper.LogSilentError("FrmBackupVerify.ScanButton_Click", ex)
                MessageBoxHelper.ShowError("สแกนไดรฟ์ไม่สำเร็จ: " & ex.Message, "สแกนไม่สำเร็จ")
            Finally
                _scanning = False
                If _scanCts IsNot Nothing Then
                    _scanCts.Dispose()
                    _scanCts = Nothing
                End If
                btnScan.Text = "🔎 สแกนไดรฟ์หา Backup"
                btnCancel.Visible = False
                btnCancel.Enabled = True
                prgScan.Style = ProgressBarStyle.Continuous
                SetBusy(False)
            End Try
        End Sub

        Private Sub CancelButton_Click(sender As Object, e As EventArgs)
            CancelScan()
        End Sub

        Private Sub CancelScan()
            If _scanCts IsNot Nothing AndAlso Not _scanCts.IsCancellationRequested Then
                _scanCts.Cancel()
                lblStatus.Text = "กำลังยกเลิกการสแกน..."
                btnCancel.Enabled = False
            End If
        End Sub

        ''' <summary>แสดงผลการสแกนด้วย dialog ที่ถูกต้อง + อธิบายจำนวน/เหตุผลภาษาไทย</summary>
        Private Sub ShowScanResult(res As DatabaseBackupHelper.BackupScanResult)
            If res Is Nothing Then Return

            If res.Failed Then
                MessageBoxHelper.ShowError("สแกนไดรฟ์ไม่สำเร็จ: " & res.ErrorMessage, "สแกนไม่สำเร็จ")
                Return
            End If

            Dim sb As New StringBuilder()
            sb.AppendLine("พบชุดสำรอง " & res.FoundCount.ToString() & " ชุด")
            sb.AppendLine("เพิ่มเข้าสู่รายการ " & res.AddedCount.ToString() & " ชุด")
            sb.AppendLine("ข้าม " & res.SkippedCount.ToString() & " ชุด")

            If res.FoundCount = 0 Then
                sb.AppendLine()
                sb.AppendLine("ไม่พบโฟลเดอร์ชุดสำรอง (Backup_*) บนไดรฟ์ที่เชื่อมต่อ")
                sb.AppendLine("กรุณาตรวจสอบว่าไดรฟ์สำรองถูกเชื่อมต่ออยู่")
            Else
                sb.AppendLine()
                sb.AppendLine("รายละเอียดแต่ละชุด:")
                For Each o In res.Outcomes
                    If o Is Nothing OrElse o.Evaluation Is Nothing Then Continue For
                    Dim ev As LegacyEvaluation = o.Evaluation
                    Dim folderName As String = Path.GetFileName(ev.BackupFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    sb.AppendLine("• " & folderName)
                    If o.Added Then
                        Dim line As String = "    เพิ่มแล้ว — " & ev.StatusText
                        If Not ev.IsComplete Then line &= " (ไม่นับเป็นชุดสำรองที่ใช้ได้)"
                        If Not String.IsNullOrEmpty(ev.Reason) Then line &= ": " & ev.Reason
                        sb.AppendLine(line)
                    Else
                        sb.AppendLine("    ข้าม — " & If(String.IsNullOrEmpty(o.SkipReason), "ไม่ทราบสาเหตุ", o.SkipReason))
                    End If
                Next
            End If

            If res.AddedCount > 0 Then
                MessageBoxHelper.ShowInfo(sb.ToString(), "สำเร็จ")
            Else
                MessageBox.Show(Me, sb.ToString(), "ไม่ได้เพิ่มชุดสำรอง",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End Sub

        ' ───────────────────────── ตาราง ─────────────────────────

        Private Function BuildRowsFromOutcomes(outcomes As List(Of DatabaseBackupHelper.BackupScanOutcome)) As List(Of DatabaseBackupHelper.BackupVerifyRow)
            Dim rows As New List(Of DatabaseBackupHelper.BackupVerifyRow)()
            If outcomes Is Nothing Then Return rows
            Dim number As Integer = 1
            For Each o In outcomes
                If o Is Nothing OrElse o.Evaluation Is Nothing Then Continue For
                Dim r As DatabaseBackupHelper.BackupVerifyRow = DatabaseBackupHelper.BuildRow(number, o.Evaluation)
                If Not o.Added Then
                    r.Notes = If(String.IsNullOrEmpty(r.Notes), o.SkipReason, r.Notes & " · " & o.SkipReason)
                End If
                rows.Add(r)
                number += 1
            Next
            Return rows
        End Function

        Private Sub PopulateGrid(rows As List(Of DatabaseBackupHelper.BackupVerifyRow))
            dgvResults.Rows.Clear()
            If rows Is Nothing Then Return

            For Each r In rows
                Dim idx = dgvResults.Rows.Add()
                Dim row = dgvResults.Rows(idx)
                row.Cells("colNum").Value = r.BackupNumber
                row.Cells("colDate").Value = If(String.IsNullOrEmpty(r.Timestamp), "-", r.Timestamp)
                row.Cells("colDrive").Value = r.DriveAndPath
                row.Cells("colDb").Value = r.DatabaseStatus
                row.Cells("colReceipts").Value = r.ReceiptsStatus
                row.Cells("colOverall").Value = r.OverallStatus
                row.Cells("colNotes").Value = r.Notes

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

        Private Sub ApplySummary(info As BackupSummaryInfo)
            If info Is Nothing Then Return
            txtSummary.Text = info.Text

            If info.HasCompleteBackup Then
                lblAlert.ForeColor = Color.FromArgb(22, 101, 52)
                lblAlert.Text = "✅ มีชุดสำรองที่ครบถ้วนแล้ว " & info.CompleteCount.ToString() & " ชุด"
            Else
                lblAlert.ForeColor = Color.FromArgb(153, 27, 27)
                lblAlert.Text = "⚠️ ยังไม่มีชุดสำรองที่ครบถ้วน - กรุณากดสำรองข้อมูล"
            End If
        End Sub

        Private Sub SetBusy(busy As Boolean, Optional disableScan As Boolean = True)
            btnVerify.Enabled = Not busy
            If disableScan Then btnScan.Enabled = Not busy
            Me.UseWaitCursor = busy AndAlso Not _scanning
        End Sub
    End Class
End Namespace
