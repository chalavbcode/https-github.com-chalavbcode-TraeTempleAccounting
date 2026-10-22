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
        ''' <summary>
        ''' ทำการสำรองฐานข้อมูลและโฟลเดอร์รูปภาพใบเสร็จ
        ''' </summary>
        ''' <param name="targetBaseDir">โฟลเดอร์ปลายทางหลัก (เช่น Flash Drive)</param>
        ''' <param name="isAuto">เป็นการสำรองข้อมูลอัตโนมัติหรือไม่</param>
        ''' <returns>True หากสำรองข้อมูลสำเร็จ</returns>
        Public Shared Function BackupFullSystem(Optional targetBaseDir As String = "", Optional isAuto As Boolean = False) As Boolean
            Return BackupFullSystemAsync(targetBaseDir, isAuto, Nothing).GetAwaiter().GetResult()
        End Function

        ''' <summary>เส้นทางไฟล์ registry (backup_registry.json) ใน app data folder</summary>
        Private Shared ReadOnly Property RegistryPath As String
            Get
                Return Path.Combine(AppPaths.AppDataFolder, "backup_registry.json")
            End Get
        End Property

        ''' <summary>
        ''' สำรองข้อมูลแบบ async บน background thread พร้อม progress (UI ไม่ค้าง)
        ''' และบันทึก registry + mirror manifest ด้วยผลจริงเท่านั้น
        ''' </summary>
        Public Shared Async Function BackupFullSystemAsync(Optional targetBaseDir As String = "", Optional isAuto As Boolean = False, Optional progress As IProgress(Of BackupProgress) = Nothing) As Task(Of Boolean)
            Try
                If String.IsNullOrEmpty(targetBaseDir) Then
                    targetBaseDir = AppPaths.BackupFolder
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)
                Dim backupPath As String = Path.Combine(targetBaseDir, "Backup_" & timestamp)

                GC.Collect()
                GC.WaitForPendingFinalizers()

                Dim result As BackupResult = Await BackupEngine.ExecuteBackupAsync(BackupConfig.BackupItems, backupPath, progress)

                ' บันทึก registry + mirror manifest ด้วยผลจริงเท่านั้น
                If result.SourceFileCount > 0 Then
                    Dim entry As BackupRegistryEntry = BuildRegistryEntry(result, backupPath)
                    BackupRegistry.AddEntry(RegistryPath, entry)
                    BackupRegistry.WriteManifestMirror(entry)
                End If

                ErrorLogHelper.LogInfo("DatabaseBackupHelper.BackupFullSystemAsync",
                                       "สำรอง -> " & backupPath & " | สำเร็จ=" & result.Success.ToString() &
                                       " | ไฟล์=" & result.SourceFileCount.ToString() & " | ปัญหา=" & result.Problems.Count.ToString())

                If result.Success Then
                    If Not isAuto Then
                        MessageBoxHelper.ShowInfo("สำรองข้อมูลครบถ้วนและตรวจสอบผ่านแล้ว!" & Environment.NewLine &
                                                 "จำนวนไฟล์: " & result.SourceFileCount & Environment.NewLine &
                                                 "ตรวจสอบผ่าน: " & result.VerifiedFileCount & Environment.NewLine &
                                                 "ตำแหน่ง: " & backupPath)
                    End If
                    Return True
                Else
                    ' ไฟล์ .accdb ถูกล็อก → แจ้งให้ปิดฐานข้อมูล (ไม่ข้ามแบบเงียบ ๆ)
                    If result.DatabaseLocked AndAlso Not isAuto Then
                        MessageBoxHelper.ShowError("ไฟล์ฐานข้อมูล (.accdb) กำลังถูกเปิดใช้งานอยู่" & Environment.NewLine &
                                                   "กรุณาปิดฐานข้อมูล (และโปรแกรมอื่นที่ใช้ฐานข้อมูล) แล้วลองสำรองใหม่อีกครั้ง")
                    End If

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
                    Return False
                End If
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
        End Class

        ''' <summary>ตรวจสอบชุดสำรองล่าสุด n รายการจาก registry แล้วจัดหมวด (เรียกจาก code ได้)</summary>
        Public Shared Function VerifyLastBackups(Optional count As Integer = 5) As List(Of BackupVerifyRow)
            Dim rows As New List(Of BackupVerifyRow)()
            Try
                Dim entries = BackupRegistry.GetLast(RegistryPath, count)
                Dim items = BackupConfig.BackupItems
                Dim num As Integer = 1
                For Each e In entries
                    Dim v = BackupEngine.ClassifyBackup(e.DestinationPath, e.Files, items)
                    Dim drivePath As String = e.DestinationPath
                    If Not String.IsNullOrEmpty(e.DriveLetter) Then drivePath = e.DriveLetter & "  " & e.DestinationPath
                    rows.Add(New BackupVerifyRow() With {
                        .BackupNumber = num,
                        .Timestamp = e.Timestamp,
                        .DriveAndPath = drivePath,
                        .DatabaseStatus = v.DatabaseStatus,
                        .ReceiptsStatus = v.ReceiptsStatus,
                        .OverallStatus = v.OverallStatus,
                        .Classification = v.Classification,
                        .NewFiles = v.NewFiles,
                        .Problems = v.Problems
                    })
                    num += 1
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.VerifyLastBackups", ex)
            End Try
            Return rows
        End Function

        ''' <summary>ไฟล์ในข้อมูลปัจจุบันที่ยังไม่เคยถูกสำรองในชุดสำรองใดเลย</summary>
        Public Shared Function FindNewUnbackedFiles() As List(Of String)
            Try
                Dim entries = BackupRegistry.GetAll(RegistryPath)
                Dim paths = BackupRegistry.CollectBackedUpPaths(entries)
                Return BackupEngine.FindUnbackedFiles(BackupConfig.BackupItems, paths)
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.FindNewUnbackedFiles", ex)
                Return New List(Of String)()
            End Try
        End Function

        ''' <summary>สแกนไดรฟ์ที่พร้อมใช้งานเพื่อหาชุดสำรอง (Backup_* / backup_manifest.json) พร้อม progress + cancel</summary>
        Public Shared Function ScanDrivesForBackups(Optional progress As IProgress(Of Integer) = Nothing, Optional token As CancellationToken = Nothing) As List(Of String)
            Dim roots As New List(Of String)()
            Try
                For Each di In DriveInfo.GetDrives()
                    If di.IsReady Then roots.Add(di.RootDirectory.FullName)
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.ScanDrivesForBackups.GetDrives", ex)
            End Try

            Dim found As New List(Of String)()
            Dim total As Integer = roots.Count
            For i As Integer = 0 To roots.Count - 1
                If token.IsCancellationRequested Then Exit For
                If progress IsNot Nothing Then progress.Report(If(total > 0, CInt(i * 100.0 / total), 0))
                Dim r = BackupEngine.ScanForBackupFolders(New List(Of String) From {roots(i)}, BackupRegistry.ProjectId, 3)
                For Each f In r
                    If Not found.Contains(f, StringComparer.OrdinalIgnoreCase) Then found.Add(f)
                Next
                If progress IsNot Nothing Then progress.Report(If(total > 0, CInt((i + 1) * 100.0 / total), 100))
            Next

            ErrorLogHelper.LogInfo("DatabaseBackupHelper.ScanDrivesForBackups", "สแกน " & total.ToString() & " ไดรฟ์ พบ " & found.Count.ToString() & " ชุดสำรอง")
            Return found
        End Function

        ''' <summary>หาชุดสำรองที่สแกนพบแต่ยังไม่มีใน registry</summary>
        Public Shared Function FindBackupsMissingFromRegistry(foundFolders As List(Of String)) As List(Of String)
            Dim missing As New List(Of String)()
            If foundFolders Is Nothing Then Return missing
            Dim known As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Try
                For Each e In BackupRegistry.GetAll(RegistryPath)
                    If Not String.IsNullOrEmpty(e.DestinationPath) Then known.Add(NormalizePath(e.DestinationPath))
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.FindBackupsMissingFromRegistry", ex)
            End Try
            For Each f In foundFolders
                If Not known.Contains(NormalizePath(f)) Then missing.Add(f)
            Next
            Return missing
        End Function

        ''' <summary>เพิ่มชุดสำรองที่สแกนพบเข้าสู่ registry หลังจากตรวจสอบผ่านแล้วเท่านั้น</summary>
        Public Shared Function AddScannedBackupToRegistry(backupFolder As String) As Boolean
            Try
                If Not Directory.Exists(backupFolder) Then Return False
                Dim verify = BackupEngine.VerifyBackup(BackupConfig.BackupItems, backupFolder)
                If Not verify.Success Then
                    ErrorLogHelper.LogInfo("DatabaseBackupHelper.AddScannedBackupToRegistry", "ตรวจสอบไม่ผ่าน ไม่เพิ่มเข้าสู่ registry: " & backupFolder)
                    Return False
                End If
                Dim entry As New BackupRegistryEntry() With {
                    .Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    .DestinationPath = backupFolder,
                    .VerificationResult = "สำเร็จ (เพิ่มจากการสแกนไดรฟ์)"
                }
                CaptureDrive(backupFolder, entry)
                For Each rec In verify.FileRecords
                    entry.Files.Add(New BackupFileEntry() With {
                        .RelativePath = rec.RelativePath,
                        .SizeBytes = rec.SizeBytes,
                        .Sha256 = rec.Sha256
                    })
                Next
                BackupRegistry.AddEntry(RegistryPath, entry)
                BackupRegistry.WriteManifestMirror(entry)
                ErrorLogHelper.LogInfo("DatabaseBackupHelper.AddScannedBackupToRegistry", "เพิ่มเข้าสู่ registry แล้ว: " & backupFolder)
                Return True
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.AddScannedBackupToRegistry", ex)
                Return False
            End Try
        End Function

        ''' <summary>เตือนเมื่อยังไม่มีชุดสำรองที่สมบูรณ์+ตรวจสอบผ่าน บนไดรฟ์จริงอย่างน้อย 2 ไดรฟ์</summary>
        Public Shared Function SafetyCheckTwoDrives() As String
            Try
                Dim entries = BackupRegistry.GetAll(RegistryPath)
                Dim items = BackupConfig.BackupItems
                Dim completeDrives As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                For Each e In entries
                    Dim cls = BackupEngine.ClassifyBackup(e.DestinationPath, e.Files, items)
                    If cls.Classification = BackupStatusClass.Identical OrElse cls.Classification = BackupStatusClass.OlderButIntact Then
                        Dim root = Path.GetPathRoot(e.DestinationPath)
                        If Not String.IsNullOrEmpty(root) Then completeDrives.Add(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    End If
                Next
                If completeDrives.Count < 2 Then
                    Return "⚠️ คำเตือน: ยังไม่มีชุดสำรองที่สมบูรณ์และตรวจสอบผ่านบนไดรฟ์จริงอย่างน้อย 2 ไดรฟ์ (พบ " & completeDrives.Count.ToString() & " ไดรฟ์)"
                End If
                Return ""
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("DatabaseBackupHelper.SafetyCheckTwoDrives", ex)
                Return ""
            End Try
        End Function

        Private Shared Sub CaptureDrive(folderPath As String, entry As BackupRegistryEntry)
            Try
                Dim root = Path.GetPathRoot(folderPath)
                If String.IsNullOrEmpty(root) Then Return
                Dim di As New DriveInfo(root)
                entry.DriveLetter = di.Name.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                entry.VolumeLabel = di.VolumeLabel
            Catch
            End Try
        End Sub

        Private Shared Function NormalizePath(p As String) As String
            If String.IsNullOrEmpty(p) Then Return ""
            Return p.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant()
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
