Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports System.Collections.Generic
Imports System.Data.OleDb
Imports System.Windows.Forms
Imports System.Globalization
Imports System.Threading
Imports System.Threading.Tasks

Namespace TempleAccounting
    Public Class DatabaseBackupHelper
        ''' <summary>เส้นทางไฟล์ registry (backup_registry.json) ใน app data folder</summary>
        Private Shared ReadOnly Property RegistryPath As String
            Get
                Return Path.Combine(AppPaths.AppDataFolder, "backup_registry.json")
            End Get
        End Property

        ''' <summary>
        ''' สำรองข้อมูลแบบ async บน background thread พร้อม progress (UI ไม่ค้าง)
        ''' และบันทึก registry + mirror manifest ด้วยผลจริงเท่านั้น
        ''' รองรับการยกเลิกผ่าน token และปฏิเสธปลายทางที่อยู่ภายในโฟลเดอร์ต้นทาง
        ''' </summary>
        Public Shared Async Function BackupFullSystemAsync(Optional targetBaseDir As String = "", Optional isAuto As Boolean = False, Optional progress As IProgress(Of BackupProgress) = Nothing, Optional token As CancellationToken = Nothing) As Task(Of Boolean)
            Try
                If String.IsNullOrEmpty(targetBaseDir) Then
                    targetBaseDir = AppPaths.BackupFolder
                End If

                ' ปฏิเสธปลายทางที่อยู่ภายใน (หรือเป็น) โฟลเดอร์ต้นทาง
                Dim containment As String = BackupEngine.FindContainmentProblem(BackupConfig.BackupItems, targetBaseDir)
                If Not String.IsNullOrEmpty(containment) Then
                    ErrorLogHelper.LogInfo("DatabaseBackupHelper.BackupFullSystemAsync", "ปฏิเสธปลายทาง: " & containment)
                    If Not isAuto Then MessageBoxHelper.ShowError(containment)
                    Return False
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                Dim backupPath As String = Path.Combine(targetBaseDir, "Backup_" & timestamp)

                GC.Collect()
                GC.WaitForPendingFinalizers()

                Dim result As BackupResult = Await BackupEngine.ExecuteBackupAsync(BackupConfig.BackupItems, backupPath, progress, token)

                ' ฐานข้อมูลถูกล็อก → เปิด Retry/Cancel ให้ผู้ใช้ (จำกัดจำนวนครั้ง)
                If result.DatabaseLocked AndAlso Not result.Cancelled AndAlso Not isAuto AndAlso Not token.IsCancellationRequested Then
                    Dim retryCount As Integer = 0
                    Do While result.DatabaseLocked AndAlso retryCount < BackupEngine.MaxRetryAttempts AndAlso Not token.IsCancellationRequested
                        retryCount += 1
                        Dim choice As DialogResult = MessageBoxHelper.Show(
                            "ไฟล์ฐานข้อมูล (.accdb) กำลังถูกเปิดใช้งานอยู่" & Environment.NewLine &
                            "กรุณาปิดฐานข้อมูล (และโปรแกรมอื่นที่ใช้ฐานข้อมูล) แล้วเลือก:" & Environment.NewLine &
                            "• ลองใหม่ (Retry) — จะสำรองข้อมูลอีกครั้ง" & Environment.NewLine &
                            "• ยกเลิก (Cancel) — หยุดการสำรอง",
                            "ฐานข้อมูลถูกล็อก", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning)
                        If choice = DialogResult.Retry Then
                            GC.Collect()
                            GC.WaitForPendingFinalizers()
                            result = Await BackupEngine.ExecuteBackupAsync(BackupConfig.BackupItems, backupPath, progress, token)
                        Else
                            result.Cancelled = True
                            result.Problems.Add("ยกเลิกการสำรองเนื่องจากฐานข้อมูลถูกล็อก")
                            Exit Do
                        End If
                    Loop
                End If

                ' บันทึก registry + mirror manifest ด้วยผลจริงเท่านั้น (ไม่ลงทะเบียนชุดที่ถูกยกเลิก)
                If result.SourceFileCount > 0 AndAlso Not result.Cancelled Then
                    Dim entry As BackupRegistryEntry = BuildRegistryEntry(result, backupPath)
                    BackupRegistry.AddEntry(RegistryPath, entry)
                    BackupRegistry.WriteManifestMirror(entry)
                End If

                ErrorLogHelper.LogInfo("DatabaseBackupHelper.BackupFullSystemAsync",
                                       "สำรอง -> " & backupPath & " | สำเร็จ=" & result.Success.ToString() &
                                       " | ยกเลิก=" & result.Cancelled.ToString() &
                                       " | ไฟล์=" & result.SourceFileCount.ToString() & " | ปัญหา=" & result.Problems.Count.ToString())

                If result.Cancelled Then
                    If Not isAuto Then
                        MessageBoxHelper.ShowInfo("การสำรองข้อมูลถูกยกเลิก" & Environment.NewLine &
                                                 "ไฟล์บางส่วนอาจยังไม่ครบถ้วน (ชุดสำรองนี้ถูกทำเครื่องหมายว่าไม่สมบูรณ์)")
                    End If
                    Return False
                End If

                If result.Success Then
                    If Not isAuto Then
                        MessageBoxHelper.ShowInfo("สำรองข้อมูลครบถ้วนและตรวจสอบผ่านแล้ว!" & Environment.NewLine &
                                                 "จำนวนไฟล์: " & result.SourceFileCount & Environment.NewLine &
                                                 "ตรวจสอบผ่าน: " & result.VerifiedFileCount & Environment.NewLine &
                                                 "ตำแหน่ง: " & backupPath)
                    End If
                    Return True
                Else
                    If result.DatabaseLocked AndAlso Not isAuto Then
                        MessageBoxHelper.ShowError("ไม่สามารถสำรองฐานข้อมูลได้: ไฟล์ .accdb ยังคงถูกล็อกอยู่" & Environment.NewLine &
                                                   "กรุณาปิดฐานข้อมูล (และโปรแกรมอื่นที่ใช้ฐานข้อมูล) แล้วลองสำรองใหม่อีกครั้ง")
                    Else
                        Dim msg As New StringBuilder()
                        msg.AppendLine("❌ การสำรองข้อมูลไม่สมบูรณ์ (ไม่ถือว่าสำเร็จ)")
                        msg.AppendLine()
                        msg.AppendLine("ไฟล์/รายการที่มีปัญหา:")
                        For Each p As String In result.Problems
                            msg.AppendLine("• " & p)
                        Next
                        If Not String.IsNullOrEmpty(result.LogPath) Then
                            msg.AppendLine()
                            msg.AppendLine("ดูรายละเอียดเพิ่มเติมที่: " & result.LogPath)
                        End If
                        If Not isAuto Then
                            MessageBoxHelper.ShowError(msg.ToString())
                        End If
                    End If
                    Return False
                End If
            Catch ex As OperationCanceledException
                ErrorLogHelper.LogInfo("DatabaseBackupHelper.BackupFullSystemAsync", "ยกเลิกการสำรอง (OperationCanceledException)")
                Return False
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.BackupFullSystemAsync")
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.BackupFullSystemAsync", ex)
                If Not isAuto Then
                    MessageBoxHelper.ShowError("เกิดข้อผิดพลาดในการสำรองข้อมูล: " & ex.Message)
                End If
                Return False
            End Try
        End Function

        ''' <summary>สร้าง registry entry จากผลการสำรอง (บันทึกเฉพาะผลจริง)</summary>
        Private Shared Function BuildRegistryEntry(result As BackupResult, backupPath As String) As BackupRegistryEntry
            Dim entry As New BackupRegistryEntry() With {
                .Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                .DestinationPath = backupPath,
                .DriveLetter = result.DriveLetter,
                .VolumeLabel = result.VolumeLabel,
                .VerificationResult = If(result.Success, "สำเร็จ", "ไม่สมบูรณ์")
            }
            For Each rec In result.FileRecords
                entry.Files.Add(New BackupFileEntry() With {
                    .RelativePath = rec.RelativePath,
                    .SizeBytes = rec.SizeBytes,
                    .Sha256 = rec.Sha256
                })
            Next
            Return entry
        End Function

        ''' <summary>หนึ่งแถวสำหรับแสดงผลตรวจสอบใน DataGridView</summary>
        Public Class BackupVerifyRow
            Public Property BackupNumber As Integer
            Public Property Timestamp As String
            Public Property DriveAndPath As String
            Public Property DatabaseStatus As String
            Public Property ReceiptsStatus As String
            Public Property OverallStatus As String
            Public Property Classification As BackupStatusClass
            Public Property NewFiles As New List(Of String)
            Public Property Problems As New List(Of String)
            ''' <summary>หมายเหตุ/เหตุผลที่ชุดนี้ไม่สมบูรณ์ พร้อมป้าย legacy</summary>
            Public Property Notes As String
            Public Property IsLegacy As Boolean
            Public Property IsComplete As Boolean
        End Class

        ''' <summary>ความคืบหน้าการสแกน (เปอร์เซ็นต์ + ข้อความ) สำหรับ IProgress</summary>
        Public Class BackupScanProgress
            Public Property Percent As Integer
            Public Property Status As String
        End Class

        ''' <summary>ผลของชุดสำรองหนึ่งชุดที่สแกนพบ</summary>
        Public Class BackupScanOutcome
            Public Property Evaluation As LegacyEvaluation
            Public Property Added As Boolean
            ''' <summary>เหตุผลเมื่อไม่ได้เพิ่มเข้าสู่รายการ</summary>
            Public Property SkipReason As String = ""
        End Class

        ''' <summary>ผลรวมของการสแกน + เพิ่มเข้าสู่ registry</summary>
        Public Class BackupScanResult
            Public Property Outcomes As New List(Of BackupScanOutcome)()
            Public Property FoundCount As Integer
            Public Property AddedCount As Integer
            Public Property SkippedCount As Integer
            Public Property Cancelled As Boolean
            Public Property Failed As Boolean
            Public Property ErrorMessage As String = ""
        End Class

        ''' <summary>ข้อความสรุปสถานะสำรอง + ไฟล์ที่ยังไม่ได้สำรอง</summary>
        Public Class BackupSummaryInfo
            Public Property Text As String = ""
            Public Property HasCompleteBackup As Boolean
            Public Property CompleteCount As Integer
            Public Property LatestCompletePath As String = ""
        End Class

        ''' <summary>ตรวจสอบชุดสำรองล่าสุด n รายการจาก registry แล้วจัดหมวดใหม่จากไฟล์จริง (ใช้ได้ทั้งชุด legacy และชุดที่มี manifest)</summary>
        Public Shared Function VerifyLastBackups(Optional count As Integer = 5, Optional snapshot As CurrentDataSnapshot = Nothing) As List(Of BackupVerifyRow)
            Dim rows As New List(Of BackupVerifyRow)()
            Try
                Dim entries = BackupRegistry.GetLast(RegistryPath, count)
                If entries.Count = 0 Then Return rows
                If snapshot Is Nothing Then
                    snapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(BackupConfig.BackupItems)
                End If

                Dim num As Integer = 1
                For Each e In entries
                    If e Is Nothing OrElse String.IsNullOrEmpty(e.DestinationPath) Then Continue For
                    Dim eval = BackupLegacyEvaluator.Evaluate(e.DestinationPath, snapshot)
                    rows.Add(BuildRow(num, eval))
                    num += 1
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.VerifyLastBackups", ex)
            End Try
            Return rows
        End Function

        ''' <summary>สร้างหนึ่งแถวตารางจากผลประเมิน</summary>
        Public Shared Function BuildRow(number As Integer, eval As LegacyEvaluation) As BackupVerifyRow
            Dim drivePath As String = eval.BackupFolder
            If Not String.IsNullOrEmpty(eval.DriveRoot) Then
                drivePath = eval.DriveRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) & "  " & eval.BackupFolder
            End If

            Dim dbStatus As String
            If Not eval.DatabaseExists Then
                dbStatus = "ไม่พบฐานข้อมูล"
            ElseIf eval.Classification = BackupStatusClass.Corrupted AndAlso String.IsNullOrEmpty(eval.DatabaseSha256) Then
                dbStatus = "เสียหาย"
            ElseIf eval.DatabaseMatchesCurrent Then
                dbStatus = "ตรงกับปัจจุบัน"
            Else
                dbStatus = "เก่ากว่าปัจจุบัน"
            End If

            Dim receiptsStatus As String
            If Not eval.ReceiptsFolderExists Then
                receiptsStatus = "ไม่มีโฟลเดอร์"
            Else
                receiptsStatus = eval.ReceiptsOk.ToString() & " / " & eval.CurrentReceiptCount.ToString()
            End If

            Dim notes As String = eval.Reason
            If eval.IsLegacy Then
                notes = If(String.IsNullOrEmpty(notes), "legacy (ไม่มี manifest)", "legacy · " & notes)
            End If

            Dim ts As String = If(Not String.IsNullOrEmpty(eval.FolderTimestamp), eval.FolderTimestamp, "")

            Return New BackupVerifyRow() With {
                .BackupNumber = number,
                .Timestamp = ts,
                .DriveAndPath = drivePath,
                .DatabaseStatus = dbStatus,
                .ReceiptsStatus = receiptsStatus,
                .OverallStatus = eval.StatusText,
                .Classification = eval.Classification,
                .NewFiles = New List(Of String)(),
                .Problems = eval.Problems,
                .Notes = notes,
                .IsLegacy = eval.IsLegacy,
                .IsComplete = eval.IsComplete
            }
        End Function

        ''' <summary>ไดรฟ์ที่พร้อมใช้งาน (root path)</summary>
        Public Shared Function GetReadyDriveRoots() As List(Of String)
            Dim roots As New List(Of String)()
            Try
                For Each di In DriveInfo.GetDrives()
                    If di.IsReady Then roots.Add(di.RootDirectory.FullName)
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.GetReadyDriveRoots", ex)
            End Try
            Return roots
        End Function

        ''' <summary>สแกนไดรฟ์ที่พร้อมใช้งานเพื่อหาชุดสำรอง (Backup_* / backup_manifest.json) พร้อม progress + cancel</summary>
        Public Shared Function ScanDrivesForBackups(Optional progress As IProgress(Of Integer) = Nothing, Optional token As CancellationToken = Nothing) As List(Of String)
            Dim roots As List(Of String) = GetReadyDriveRoots()
            Dim found As New List(Of String)()
            Dim total As Integer = roots.Count
            For i As Integer = 0 To roots.Count - 1
                If token.IsCancellationRequested Then Exit For
                If progress IsNot Nothing Then progress.Report(If(total > 0, CInt(i * 100.0 / total), 0))
                Dim r = BackupEngine.ScanForBackupFolders(New List(Of String) From {roots(i)}, BackupRegistry.ProjectId, 3, token)
                For Each f In r
                    If Not found.Contains(f, StringComparer.OrdinalIgnoreCase) Then found.Add(f)
                Next
                If progress IsNot Nothing Then progress.Report(If(total > 0, CInt((i + 1) * 100.0 / total), 100))
            Next
            ErrorLogHelper.LogInfo("DatabaseBackupHelper.ScanDrivesForBackups", "สแกน " & total.ToString() & " ไดรฟ์ พบ " & found.Count.ToString() & " ชุดสำรอง")
            Return found
        End Function

        ''' <summary>
        ''' สแกนหาชุดสำรองทุกชุด → ประเมินจากไฟล์จริง (legacy หรือมี manifest ก็ได้) → เพิ่มเข้าสู่ registry ทุกชุด
        ''' ชุดที่ไม่สมบูรณ์ก็ถูกเพิ่ม (ทำเครื่องหมายชัดเจน) เพื่อให้ผู้ใช้เห็น แต่ไม่ถูกนับว่าใช้ได้
        ''' ไม่อ่าน/เขียน/ลบ/ย้ายไฟล์ใดในโฟลเดอร์สำรอง (เขียนเฉพาะ registry ใน app data)
        ''' </summary>
        Public Shared Function ScanAndRegisterBackups(Optional progress As IProgress(Of BackupScanProgress) = Nothing,
                                                      Optional token As CancellationToken = Nothing) As BackupScanResult
            Dim result As New BackupScanResult()
            Try
                Dim roots As List(Of String) = GetReadyDriveRoots()
                ReportScanProgress(progress, 2, "กำลังสแกนไดรฟ์...")

                Dim found As New List(Of String)()
                Dim totalDrives As Integer = Math.Max(roots.Count, 1)
                For i As Integer = 0 To roots.Count - 1
                    If token.IsCancellationRequested Then
                        result.Cancelled = True
                        Return result
                    End If
                    Dim driveIdx As Integer = i
                    Dim visited As Integer = 0
                    ReportScanProgress(progress, CInt(driveIdx * 55.0 / totalDrives), "กำลังสแกน " & roots(i))
                    Dim r = BackupEngine.ScanForBackupFolders(
                        New List(Of String) From {roots(i)}, BackupRegistry.ProjectId, 3, token,
                        Sub(dir)
                            visited += 1
                            If (visited Mod 250) = 0 Then
                                ReportScanProgress(progress, CInt(driveIdx * 55.0 / totalDrives), "กำลังสแกน " & dir)
                            End If
                        End Sub)
                    For Each f In r
                        If Not found.Contains(f, StringComparer.OrdinalIgnoreCase) Then found.Add(f)
                    Next
                Next

                If token.IsCancellationRequested Then
                    result.Cancelled = True
                    Return result
                End If

                result.FoundCount = found.Count
                If found.Count = 0 Then
                    ReportScanProgress(progress, 100, "ไม่พบชุดสำรอง")
                    Return result
                End If

                ReportScanProgress(progress, 60, "กำลังอ่านข้อมูลปัจจุบัน (SHA-256)...")
                Dim snapshot As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(BackupConfig.BackupItems, token)
                If token.IsCancellationRequested Then
                    result.Cancelled = True
                    Return result
                End If

                found.Sort(StringComparer.OrdinalIgnoreCase)
                Dim doc As RegistryDocument = BackupRegistry.Load(RegistryPath)
                For idx As Integer = 0 To found.Count - 1
                    If token.IsCancellationRequested Then
                        result.Cancelled = True
                        Return result
                    End If
                    ReportScanProgress(progress, 60 + CInt((idx + 1) * 38.0 / found.Count),
                                       "กำลังตรวจสอบ " & (idx + 1).ToString() & "/" & found.Count.ToString() & ": " & found(idx))

                    Dim eval As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(found(idx), snapshot, token)
                    Dim outcome As New BackupScanOutcome() With {.Evaluation = eval}

                    If BackupRegistry.IsRegistered(doc, eval.BackupFolder) Then
                        outcome.Added = False
                        outcome.SkipReason = "มีอยู่ในรายการแล้ว (ข้ามไม่เพิ่มซ้ำ)"
                        result.SkippedCount += 1
                    ElseIf Not eval.FolderExists Then
                        outcome.Added = False
                        outcome.SkipReason = If(String.IsNullOrEmpty(eval.Reason), "ไม่พบโฟลเดอร์", eval.Reason)
                        result.SkippedCount += 1
                    Else
                        If BackupLegacyEvaluator.AddEvaluationToDocument(doc, eval) Then
                            outcome.Added = True
                            result.AddedCount += 1
                        Else
                            outcome.Added = False
                            outcome.SkipReason = "เพิ่มเข้าสู่รายการไม่สำเร็จ"
                            result.SkippedCount += 1
                        End If
                    End If

                    result.Outcomes.Add(outcome)
                Next

                If result.AddedCount > 0 Then
                    BackupRegistry.Save(RegistryPath, doc)
                End If

                ReportScanProgress(progress, 100, "สแกนเสร็จ — พบ " & result.FoundCount.ToString() & " ชุด")
                ErrorLogHelper.LogInfo("DatabaseBackupHelper.ScanAndRegisterBackups",
                                       "พบ=" & result.FoundCount.ToString() & " เพิ่ม=" & result.AddedCount.ToString() &
                                       " ข้าม=" & result.SkippedCount.ToString())
            Catch ex As OperationCanceledException
                result.Cancelled = True
            Catch ex As Exception
                result.Failed = True
                result.ErrorMessage = ex.Message
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.ScanAndRegisterBackups", ex)
            End Try
            Return result
        End Function

        Private Shared Sub ReportScanProgress(progress As IProgress(Of BackupScanProgress), percent As Integer, status As String)
            If progress Is Nothing Then Return
            Try
                progress.Report(New BackupScanProgress() With {
                    .Percent = Math.Max(0, Math.Min(100, percent)),
                    .Status = status
                })
            Catch
            End Try
        End Sub

        ''' <summary>เตือนเมื่อยังไม่มีชุดสำรองที่สมบูรณ์บนไดรฟ์จริงอย่างน้อย 2 ไดรฟ์ (ใช้สถานะที่บันทึกไว้ ไม่แฮชซ้ำ)</summary>
        Public Shared Function SafetyCheckTwoDrives() As String
            Try
                Dim doc = BackupRegistry.Load(RegistryPath)
                Return BuildTwoDriveWarning(CompleteEntries(doc))
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.SafetyCheckTwoDrives", ex)
                Return ""
            End Try
        End Function

        ''' <summary>รายการชุดสำรองที่สมบูรณ์ (ใหม่ → เก่า) จาก registry</summary>
        Private Shared Function CompleteEntries(doc As RegistryDocument) As List(Of BackupRegistryEntry)
            Dim complete As New List(Of BackupRegistryEntry)()
            If doc Is Nothing OrElse doc.Entries Is Nothing Then Return complete
            For i As Integer = doc.Entries.Count - 1 To 0 Step -1
                Dim e = doc.Entries(i)
                If e Is Nothing Then Continue For
                If IsCompleteEntry(e) Then complete.Add(e)
            Next
            Return complete
        End Function

        Private Shared Function IsCompleteEntry(e As BackupRegistryEntry) As Boolean
            If e.IsComplete Then Return True
            ' รายการรุ่นเก่าที่บันทึกไว้ก่อนมีสถานะ: ตรวจจากข้อความผลการสำรอง
            If String.IsNullOrEmpty(e.Status) AndAlso Not String.IsNullOrEmpty(e.VerificationResult) Then
                Return e.VerificationResult.StartsWith("สำเร็จ", StringComparison.Ordinal) AndAlso
                       Not e.VerificationResult.Contains("ไม่สมบูรณ์")
            End If
            Return False
        End Function

        Private Shared Function BuildTwoDriveWarning(complete As List(Of BackupRegistryEntry)) As String
            Dim drives As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each e In complete
                Dim root As String = Path.GetPathRoot(e.DestinationPath)
                If Not String.IsNullOrEmpty(root) AndAlso Directory.Exists(root) Then
                    drives.Add(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                End If
            Next
            If drives.Count < 2 Then
                Return "⚠️ คำเตือน: ยังไม่มีชุดสำรองที่สมบูรณ์บนไดรฟ์จริงอย่างน้อย 2 ไดรฟ์ (พบ " & drives.Count.ToString() & " ไดรฟ์)"
            End If
            Return ""
        End Function

        ''' <summary>
        ''' สรุปสถานะสำรอง + รายการไฟล์ที่ยังไม่ได้สำรอง (เทียบกับชุดสำรองที่สมบูรณ์ล่าสุดเท่านั้น)
        ''' ทำแยกฐานข้อมูลกับใบเสร็จ และระบุสาเหตุเมื่อฐานข้อมูลเปลี่ยนหลังการสำรอง
        ''' </summary>
        Public Shared Function BuildStatusSummary(Optional snapshot As CurrentDataSnapshot = Nothing) As BackupSummaryInfo
            Dim info As New BackupSummaryInfo()
            Try
                If snapshot Is Nothing Then
                    snapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(BackupConfig.BackupItems)
                End If

                Dim doc = BackupRegistry.Load(RegistryPath)
                Dim complete As List(Of BackupRegistryEntry) = CompleteEntries(doc)
                info.CompleteCount = complete.Count

                Dim sb As New StringBuilder()
                Dim eval As LegacyEvaluation = Nothing

                If complete.Count > 0 Then
                    info.HasCompleteBackup = True
                    info.LatestCompletePath = complete(0).DestinationPath
                    eval = BackupLegacyEvaluator.Evaluate(info.LatestCompletePath, snapshot)
                    sb.AppendLine("✅ ชุดสำรองที่สมบูรณ์ล่าสุด: " & info.LatestCompletePath)
                    Dim line As String = "   สถานะ: " & eval.StatusText
                    If Not String.IsNullOrEmpty(eval.FolderTimestamp) Then line &= "  •  วันที่ " & eval.FolderTimestamp
                    If eval.IsLegacy Then line &= "  •  legacy (ไม่มี manifest)"
                    sb.AppendLine(line)
                Else
                    info.HasCompleteBackup = False
                    sb.AppendLine("ยังไม่มีชุดสำรองที่ครบถ้วน - กรุณากดสำรองข้อมูล")
                    sb.AppendLine("ไฟล์ปัจจุบันทั้งหมดยังไม่ได้รับการสำรองที่ตรวจสอบได้")
                End If

                Dim safety As String = BuildTwoDriveWarning(complete)
                If Not String.IsNullOrEmpty(safety) Then
                    sb.AppendLine()
                    sb.AppendLine(safety)
                End If

                ' ── เทียบกับชุดสำรองที่สมบูรณ์ล่าสุดเท่านั้น ──
                Dim dbUnbacked As Boolean = True
                Dim dbNote As String = ""
                Dim receiptsUnbacked As New List(Of String)()

                If eval Is Nothing Then
                    dbNote = "ยังไม่มีชุดสำรองที่สมบูรณ์ให้เทียบ"
                    For Each r In snapshot.Receipts
                        receiptsUnbacked.Add(r.RelativePath)
                    Next
                Else
                    dbUnbacked = Not eval.DatabaseMatchesCurrent
                    If Not eval.DatabaseExists Then
                        dbNote = "ไม่พบไฟล์ฐานข้อมูลในชุดสำรองล่าสุด"
                    ElseIf Not eval.DatabaseMatchesCurrent Then
                        dbNote = "hash ต่างจากชุดสำรองล่าสุด — ไฟล์อาจถูกแก้ไขหลังการสำรอง หรือกำลังถูกเปิดใช้งานอยู่ (locked)"
                    End If
                    For Each r In eval.ReceiptsMissing
                        receiptsUnbacked.Add(r)
                    Next
                    For Each r In eval.ReceiptsChanged
                        receiptsUnbacked.Add(r & " (เนื้อหาเปลี่ยน)")
                    Next
                End If

                Dim total As Integer = receiptsUnbacked.Count + If(dbUnbacked, 1, 0)
                sb.AppendLine()
                If total = 0 Then
                    sb.AppendLine("✅ ไม่มีไฟล์ใหม่ที่ยังไม่ได้สำรอง (ฐานข้อมูลและใบเสร็จตรงกับชุดสำรองล่าสุดแล้ว)")
                Else
                    If eval Is Nothing Then
                        sb.AppendLine("📄 ไฟล์ที่ยังไม่ได้สำรอง (" & total.ToString() & " ไฟล์):")
                    Else
                        sb.AppendLine("📄 ไฟล์ที่ยังไม่ได้สำรอง/เปลี่ยนแปลง (" & total.ToString() & " ไฟล์) เทียบกับชุดสำรองล่าสุด:")
                    End If
                    If dbUnbacked Then
                        sb.AppendLine("   ฐานข้อมูล (1):")
                        sb.AppendLine("   • " & If(String.IsNullOrEmpty(snapshot.DatabaseFileName), "(ไม่พบไฟล์ฐานข้อมูล)", snapshot.DatabaseFileName) &
                                      If(String.IsNullOrEmpty(dbNote), "", " — " & dbNote))
                    End If
                    If receiptsUnbacked.Count > 0 Then
                        sb.AppendLine("   ใบเสร็จ (" & receiptsUnbacked.Count.ToString() & "):")
                        Dim shown As Integer = 0
                        For Each r In receiptsUnbacked
                            If shown >= 15 Then Exit For
                            sb.AppendLine("   • " & r)
                            shown += 1
                        Next
                        If receiptsUnbacked.Count > shown Then
                            sb.AppendLine("   … และอีก " & (receiptsUnbacked.Count - shown).ToString() & " ไฟล์")
                        End If
                    End If
                End If

                info.Text = sb.ToString()
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.BuildStatusSummary", ex)
                info.Text = "เกิดข้อผิดพลาดระหว่างสร้างสรุป: " & ex.Message
            End Try
            Return info
        End Function

        ''' <summary>
        ''' คัดลอกโฟลเดอร์แบบ Recursive พร้อมจัดการ Error หากไฟล์ถูก Lock
        ''' </summary>
        Private Shared Sub CopyDirectory(sourceDir As String, destDir As String)
            Try
                ' สร้างโฟลเดอร์ปลายทาง
                If Not Directory.Exists(destDir) Then
                    Directory.CreateDirectory(destDir)
                End If

                ' คัดลอกไฟล์ในโฟลเดอร์ปัจจุบัน
                Dim dirInfo As New DirectoryInfo(sourceDir)
                For Each fileItem In dirInfo.GetFiles()
                    Try
                        Dim destFilePath As String = Path.Combine(destDir, fileItem.Name)
                        fileItem.CopyTo(destFilePath, True)
                    Catch ex As Exception
                        ' หากไฟล์ถูก Lock หรือมีปัญหา ให้ข้ามไปไฟล์ถัดไปตามเงื่อนไข
                        AppPaths.LogCrash(ex, $"CopyDirectory.File: {fileItem.FullName}")
                    End Try
                Next

                ' คัดลอกโฟลเดอร์ย่อย (Recursive)
                For Each subDir In dirInfo.GetDirectories()
                    Dim destSubDir As String = Path.Combine(destDir, subDir.Name)
                    CopyDirectory(subDir.FullName, destSubDir)
                Next
            Catch ex As Exception
                AppPaths.LogCrash(ex, $"CopyDirectory.Dir: {sourceDir}")
            End Try
        End Sub

        ''' <summary>
        ''' คืนค่าข้อมูลจากโฟลเดอร์ Backup (ฐานข้อมูลและรูปใบเสร็จ)
        ''' </summary>
        ''' <param name="backupFolder">โฟลเดอร์ชุดสำรองข้อมูลที่เลือก</param>
        ''' <returns>True หากคืนค่าสำเร็จ</returns>
        Public Shared Function RestoreFullSystem(backupFolder As String) As Boolean
            Dim safetyPath As String = Path.Combine(AppPaths.AppRoot, "PreRestore_Temp")
            Try
                ' 1. ตรวจสอบและปรับจูน Path (Smart Path Resolution)
                If Not Directory.Exists(backupFolder) Then
                    MessageBoxHelper.ShowError("ไม่พบโฟลเดอร์สำรองข้อมูลที่เลือก")
                    Return False
                End If

                ' หากผู้ใช้เลือกไฟล์ข้างในโฟลเดอร์ Receipts ให้ขยับออกมาที่โฟลเดอร์หลัก
                If backupFolder.EndsWith("Receipts", StringComparison.OrdinalIgnoreCase) Then
                    Dim parentDir = Directory.GetParent(backupFolder)
                    If parentDir IsNot Nothing Then backupFolder = parentDir.FullName
                End If

                ' ตรวจสอบหาไฟล์ฐานข้อมูล (.accdb)
                Dim dbBackupFile As String = ""
                Dim files = Directory.GetFiles(backupFolder, "*.accdb")
                
                ' หากไม่เจอในโฟลเดอร์นี้ ให้ลองหาใน Parent อีก 1 ระดับ (เผื่อกรณีเลือกโฟลเดอร์ย่อยอื่น)
                If files.Length = 0 Then
                    Dim parentDir = Directory.GetParent(backupFolder)
                    If parentDir IsNot Nothing Then
                        files = Directory.GetFiles(parentDir.FullName, "*.accdb")
                        If files.Length > 0 Then
                            backupFolder = parentDir.FullName
                        End If
                    End If
                End If

                If files.Length > 0 Then
                    dbBackupFile = files(0)
                Else
                    MessageBoxHelper.ShowError("ไม่พบไฟล์ฐานข้อมูล (.accdb) ในโฟลเดอร์ที่เลือกหรือโฟลเดอร์ระดับบน" & Environment.NewLine & 
                                             "กรุณาเลือกไฟล์สำรองข้อมูลให้ถูกต้อง")
                    Return False
                End If

                ' 2. จัดการ Connection: ตัด Pool
                GC.Collect()
                GC.WaitForPendingFinalizers()

                ' 3. Safety Backup: เก็บข้อมูลปัจจุบันไว้กันพลาด
                If Directory.Exists(safetyPath) Then Directory.Delete(safetyPath, True)
                Directory.CreateDirectory(safetyPath)
                
                ' ก๊อบปี้ DB ปัจจุบัน
                If File.Exists(AppPaths.DatabaseFile) Then
                    File.Copy(AppPaths.DatabaseFile, Path.Combine(safetyPath, "CurrentDB.accdb"), True)
                End If
                ' ก๊อบปี้ Receipts ปัจจุบัน
                If Directory.Exists(AppPaths.ReceiptsDir) Then
                    CopyDirectory(AppPaths.ReceiptsDir, Path.Combine(safetyPath, "Receipts"))
                End If

                ' 4. ทำการ Restore ฐานข้อมูล
                File.Copy(dbBackupFile, AppPaths.DatabaseFile, True)

                ' 5. ทำการ Restore รูปใบเสร็จ (Receipt Images)
                ' ตรวจสอบทั้งในโฟลเดอร์ที่เลือก และโฟลเดอร์ย่อยชื่อ Receipts
                Dim receiptsSourcePaths As New List(Of String)()
                
                ' กรณี 1: มีโฟลเดอร์ Receipts อยู่ข้างใน (โครงสร้างมาตรฐาน — legacy backup)
                Dim subDirReceipts = Path.Combine(backupFolder, "Receipts")
                If Directory.Exists(subDirReceipts) Then receiptsSourcePaths.Add(subDirReceipts)
                
                ' กรณี 2: ผู้ใช้เลือกโฟลเดอร์ที่มีรูปอยู่โดยตรง
                receiptsSourcePaths.Add(backupFolder)

                ' กรณี 3: Backup ใหม่ (หลัง Fix 9) ไม่มี Receipts/ ข้างใน
                '         → fallback ไปใช้โฟลเดอร์กลาง Backup/Receipts/ แทน
                If receiptsSourcePaths.Count = 1 Then
                    Dim centralizedReceipts = Path.Combine(AppPaths.BackupFolder, "Receipts")
                    If centralizedReceipts <> subDirReceipts AndAlso Directory.Exists(centralizedReceipts) Then
                        receiptsSourcePaths.Add(centralizedReceipts)
                    End If
                End If

                ' ทำการคัดลอกไฟล์รูปภาพจากทุกแหล่งที่พบ
                For Each srcPath In receiptsSourcePaths
                    CopyImageFilesOnly(srcPath, AppPaths.ReceiptsDir)
                Next

                ' ลบ Safety Backup เมื่อสำเร็จ
                Try
                    Directory.Delete(safetyPath, True)
                Catch ex As Exception
                    ErrorLogHelper.LogSilentError("DatabaseBackupHelper.RestoreFullSystem.TempCleanup", ex)
                End Try

                Return True
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.RestoreFullSystem")
                
                ' Rollback: พยายามกู้คืนจาก Safety Backup
                Try
                    If Directory.Exists(safetyPath) Then
                        If File.Exists(Path.Combine(safetyPath, "CurrentDB.accdb")) Then
                            File.Copy(Path.Combine(safetyPath, "CurrentDB.accdb"), AppPaths.DatabaseFile, True)
                        End If
                        If Directory.Exists(Path.Combine(safetyPath, "Receipts")) Then
                            CopyDirectory(Path.Combine(safetyPath, "Receipts"), AppPaths.ReceiptsDir)
                        End If
                    End If
                Catch rollbackEx As Exception
                    AppPaths.LogCrash(rollbackEx, "Restore Rollback Failed")
                End Try

                MessageBoxHelper.ShowError($"เกิดข้อผิดพลาดระหว่างคืนค่าข้อมูล: {ex.Message}{Environment.NewLine}ระบบพยายามกู้คืนข้อมูลเดิมกลับมาแล้ว")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' คัดลอกเฉพาะไฟล์รูปภาพจากโฟลเดอร์ต้นทางไปยังปลายทาง
        ''' </summary>
        Private Shared Sub CopyImageFilesOnly(sourceDir As String, destDir As String)
            Try
                If Not Directory.Exists(sourceDir) Then Return
                If Not Directory.Exists(destDir) Then Directory.CreateDirectory(destDir)

                Dim dirInfo As New DirectoryInfo(sourceDir)
                ' รองรับนามสกุลรูปภาพหลักๆ (Case-insensitive)
                Dim extensions As String() = {".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp"}
                
                For Each fileItem In dirInfo.GetFiles()
                    Dim ext = fileItem.Extension.ToLower()
                    ' กู้คืนไฟล์รูปภาพ: ตรวจสอบจากนามสกุล OR ตรวจสอบจากชื่อไฟล์ (เผื่อไม่มีนามสกุล)
                    If Array.IndexOf(extensions, ext) >= 0 OrElse fileItem.Name.StartsWith("Receipt_", StringComparison.OrdinalIgnoreCase) Then
                        Try
                            Dim destFilePath = Path.Combine(destDir, fileItem.Name)
                            ' หากปลายทางไม่มีนามสกุล แต่ต้นทางรู้ว่าเป็นรูป ให้เติม .jpg ให้ (ถ้าจำเป็น)
                            ' แต่ในที่นี้เราจะก๊อบปี้ตามชื่อเดิมเพื่อความแม่นยำของฐานข้อมูล
                            fileItem.CopyTo(destFilePath, True)
                        Catch ex As Exception
                            ' ข้ามไฟล์ที่ติด Lock หรือมีปัญหา
                            AppPaths.LogCrash(ex, $"CopyImageFilesOnly.Skip: {fileItem.Name}")
                        End Try
                    End If
                Next
            Catch ex As Exception
                AppPaths.LogCrash(ex, $"CopyImageFilesOnly.Error: {sourceDir}")
            End Try
        End Sub

        ' ──────────────────────────────────────────────────────────────
        ' Tiered Backup Retention (Weekly / Monthly / Yearly)
        ' ──────────────────────────────────────────────────────────────

        Private Const MaxWeeklyBackups As Integer = 8
        Private Const MaxMonthlyBackups As Integer = 15
        Private Const MaxYearlyBackups As Integer = 5

        ''' <summary>
        ''' สร้างโฟลเดอร์ย่อยสำหรับ Tiered Backup ทั้ง 3 ระดับ (ถ้ายังไม่มี)
        ''' </summary>
        Private Shared Sub EnsureBackupTierFolders()
            Dim tiers = {"Weekly", "Monthly", "Yearly"}
            For Each tier In tiers
                Dim folder = Path.Combine(AppPaths.BackupFolder, tier)
                If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
            Next
        End Sub

        ''' <summary>
        ''' ทำการสำรองข้อมูลแบบ Tiered (Weekly + Monthly + Yearly) — เรียกตอนปิดโปรแกรม
        ''' </summary>
        ''' <param name="overrideDate">วันที่จำลองสำหรับทดสอบ (ถ้า Nothing ใช้ Date.Today)</param>
        Public Shared Sub RunTieredBackup(Optional overrideDate As Date? = Nothing)
            Dim d As Date = If(overrideDate, Date.Today)
            Try
                EnsureBackupTierFolders()
                RunWeeklyBackup(d)
                RunMonthlyBackup(d)
                If d.Month = 12 Then RunYearlyBackup(d)

                ' 4. คัดลอกใบเสร็จไปโฟลเดอร์กลาง (หลัง copy DB ทุก tier เสร็จแล้ว)
                SyncReceiptsToBackup()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.RunTieredBackup")
            End Try
        End Sub

        ''' <summary>
        ''' สำรองแบบ Weekly — ชื่อไฟล์ Backup_Weekly_{ISOYear}_W{weekNum:D2}.accdb
        ''' เขียนทับไฟล์เดิมของสัปดาห์เดียวกันเสมอ (overwrite-in-place)
        ''' </summary>
        Public Shared Sub RunWeeklyBackup(d As Date)
            Dim isoYear = ISOWeek.GetYear(d)
            Dim weekNum = ISOWeek.GetWeekOfYear(d)
            ' ใช้ isoYear (Integer จาก ISOWeek) ไม่ใช่ d:yyyy เพราะบนระบบ Thai locale
            ' ตัว format specifier {d:yyyy} จะคืนค่าปี พ.ศ. (Buddhist Era) แทน ค.ศ.
            Dim fileName = $"Backup_Weekly_{isoYear}_W{weekNum:D2}.accdb"
            CopyDatabaseToTierFolder("Weekly", fileName)
            CleanupWeeklyBackups()
        End Sub

        ''' <summary>
        ''' สำรองแบบ Monthly — ชื่อไฟล์ Backup_Monthly_{yyyy}_{MM}.accdb
        ''' เขียนทับไฟล์เดิมของเดือนเดียวกันเสมอ
        ''' </summary>
        Public Shared Sub RunMonthlyBackup(d As Date)
            ' ใช้ d.Year / d.Month แทน d:yyyy / d:MM เพราะบนระบบ Thai locale
            ' ตัว format specifier {d:yyyy} จะคืนค่าปี พ.ศ. (Buddhist Era) แทน ค.ศ.
            Dim fileName = $"Backup_Monthly_{d.Year}_{d.Month:D2}.accdb"
            CopyDatabaseToTierFolder("Monthly", fileName)
            CleanupMonthlyBackups()
        End Sub

        ''' <summary>
        ''' สำรองแบบ Yearly — ชื่อไฟล์ Backup_Yearly_{yyyy}.accdb
        ''' เรียกเฉพาะเดือนธันวาคมเท่านั้น
        ''' </summary>
        Public Shared Sub RunYearlyBackup(d As Date)
            ' ใช้ d.Year แทน d:yyyy เพราะบนระบบ Thai locale
            ' ตัว format specifier {d:yyyy} จะคืนค่าปี พ.ศ. (Buddhist Era) แทน ค.ศ.
            Dim fileName = $"Backup_Yearly_{d.Year}.accdb"
            CopyDatabaseToTierFolder("Yearly", fileName)
            CleanupYearlyBackups()
        End Sub

        ''' <summary>
        ''' คัดลอกไฟล์ DB ไปยังโฟลเดอร์ tier ที่ระบุ (overwrite ถ้ามีอยู่แล้ว)
        ''' </summary>
        Private Shared Sub CopyDatabaseToTierFolder(tier As String, fileName As String)
            Dim dbSource As String = AppPaths.DatabaseFile
            If Not File.Exists(dbSource) Then Return

            Dim destDir = Path.Combine(AppPaths.BackupFolder, tier)
            Dim destPath = Path.Combine(destDir, fileName)

            Try
                ' Best-effort: ลอง collect ก่อน copy เพื่อลดโอกาส file lock
                GC.Collect()
                GC.WaitForPendingFinalizers()
                File.Copy(dbSource, destPath, True)
            Catch ex As Exception
                AppPaths.LogCrash(ex, $"CopyDatabaseToTierFolder({tier}/{fileName})")
            End Try
        End Sub

        ' ──────────────────────────────────────────────────────────────
        ' Centralized Receipt Backup (Append-Only, Collision-Safe)
        ' ──────────────────────────────────────────────────────────────

        ''' <summary>
        ''' คัดลอกใบเสร็จจากโฟลเดอร์ต้นทาง (Receipts/) ไปโฟลเดอร์กลาง (Backup/Receipts/) แบบ Append-Only
        ''' • ไฟล์ที่ hash ตรงกัน → skip (ไม่ copy ซ้ำ)
        ''' • ไฟล์ชื่อเดียวกันแต่เนื้อหาต่างกัน → เก็บด้วยชื่อใหม่แบบ deterministic (ไม่ overwrite)
        ''' • ห้ามลบไฟล์ใดๆ ออกจาก Backup/Receipts/ (Append-Only)
        ''' </summary>
        Public Shared Sub SyncReceiptsToBackup()
            Try
                Dim sourceDir As String = AppPaths.ReceiptsDir
                If Not Directory.Exists(sourceDir) Then Return

                Dim destDir As String = Path.Combine(AppPaths.BackupFolder, "Receipts")
                If Not Directory.Exists(destDir) Then Directory.CreateDirectory(destDir)

                For Each srcFile In Directory.GetFiles(sourceDir)
                    Try
                        Dim fileName As String = Path.GetFileName(srcFile)
                        Dim destFilePath As String = Path.Combine(destDir, fileName)

                        If File.Exists(destFilePath) Then
                            ' ไฟล์ปลายทางชื่อเดียวกันมีอยู่แล้ว → ตรวจสอบว่าเนื้อหาเดียวกันไหม
                            Dim srcBytes As Byte() = File.ReadAllBytes(srcFile)
                            Dim destBytes As Byte() = File.ReadAllBytes(destFilePath)
                            Dim srcHash As String = ComputeMd5Hash(srcBytes)
                            Dim destHash As String = ComputeMd5Hash(destBytes)

                            If srcHash = destHash Then
                                ' Hash ตรงกัน → ไฟล์เดียวกันจริง → skip (ไม่ copy ซ้ำ)
                                Continue For
                            Else
                                ' Size หรือ hash ต่างกัน → ชื่อชนกัน คนละไฟล์ → ต้องเก็บทั้งคู่
                                ' ตั้งชื่อไฟล์ collision ด้วย hash สั้นของเนื้อหา (deterministic)
                                Dim shortHash As String = GetShortMd5Hash(srcBytes)
                                Dim ext As String = Path.GetExtension(fileName)
                                Dim nameNoExt As String = Path.GetFileNameWithoutExtension(fileName)
                                Dim collisionName As String = $"{nameNoExt}_{shortHash}{ext}"
                                Dim collisionPath As String = Path.Combine(destDir, collisionName)

                                ' ป้องกันกรณี rare ที่ collision name ซ้ำอีก → เพิ่มตัวเลข
                                Dim finalPath As String = collisionPath
                                If File.Exists(finalPath) Then
                                    Dim finalHash As String = ComputeMd5Hash(File.ReadAllBytes(finalPath))
                                    If finalHash = srcHash Then Continue For  ' มีอยู่แล้ว
                                    Dim counter As Integer = 1
                                    Do While File.Exists(finalPath)
                                        finalPath = Path.Combine(destDir, $"{nameNoExt}_{shortHash}_{counter}{ext}")
                                        counter += 1
                                    Loop
                                End If
                                File.WriteAllBytes(finalPath, srcBytes)
                            End If
                        Else
                            ' ไฟล์ปลายทางไม่มี → copy ตรงๆ
                            File.Copy(srcFile, destFilePath)
                        End If
                    Catch ex As Exception
                        AppPaths.LogCrash(ex, $"SyncReceiptsToBackup.File: {srcFile}")
                    End Try
                Next
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.SyncReceiptsToBackup")
            End Try
        End Sub

        ''' <summary>
        ''' คำนวณ MD5 hash จาก byte array
        ''' </summary>
        Private Shared Function ComputeMd5Hash(data As Byte()) As String
            Using md5 As MD5 = MD5.Create()
                Dim hashBytes As Byte() = md5.ComputeHash(data)
                Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
            End Using
        End Function

        ''' <summary>
        ''' คืน MD5 hash สั้น 8 ตัวอักษรแรก (สำหรับตั้งชื่อไฟล์ collision)
        ''' </summary>
        Private Shared Function GetShortMd5Hash(data As Byte()) As String
            Return ComputeMd5Hash(data).Substring(0, 8)
        End Function

        ' ── Cleanup helpers (FIFO ตาม max count, sort จากชื่อไฟล์) ──

        Public Shared Sub CleanupWeeklyBackups()
            CleanupTierFolder(Path.Combine(AppPaths.BackupFolder, "Weekly"), "Backup_Weekly_*.accdb", MaxWeeklyBackups)
        End Sub

        Public Shared Sub CleanupMonthlyBackups()
            CleanupTierFolder(Path.Combine(AppPaths.BackupFolder, "Monthly"), "Backup_Monthly_*.accdb", MaxMonthlyBackups)
        End Sub

        Public Shared Sub CleanupYearlyBackups()
            CleanupTierFolder(Path.Combine(AppPaths.BackupFolder, "Yearly"), "Backup_Yearly_*.accdb", MaxYearlyBackups)
        End Sub

        ''' <summary>
        ''' ลบไฟล์ที่เก่าที่สุดในโฟลเดอร์จนเหลือไม่เกิน maxCount
        ''' เรียงลำดับจากชื่อไฟล์ (ไม่ใช่ File.CreationTime) เพื่อความแม่นยำ
        ''' </summary>
        Private Shared Sub CleanupTierFolder(folder As String, searchPattern As String, maxCount As Integer)
            Try
                If Not Directory.Exists(folder) Then Return

                Dim files = Directory.GetFiles(folder, searchPattern)
                If files.Length <= maxCount Then Return

                ' เรียงจากชื่อไฟล์ (ชื่อมี timestamp อยู่แล้ว → เรียงเก่าไปใหม่ได้ถูกต้อง)
                Array.Sort(files, StringComparer.OrdinalIgnoreCase)

                ' ลบไฟล์ที่เก่าที่สุดออกจนเหลือ maxCount
                Dim toDelete = files.Length - maxCount
                For i As Integer = 0 To toDelete - 1
                    Try
                        File.Delete(files(i))
                    Catch ex As Exception
                        AppPaths.LogCrash(ex, $"CleanupTierFolder.Delete: {files(i)}")
                    End Try
                Next
            Catch ex As Exception
                AppPaths.LogCrash(ex, $"CleanupTierFolder({folder})")
            End Try
        End Sub
    End Class
End Namespace
