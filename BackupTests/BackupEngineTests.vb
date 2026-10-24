Option Strict Off
Option Explicit On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Diagnostics

Namespace TempleAccounting

    <TestClass>
    Public Class BackupEngineTests
        Private _tempRoots As New List(Of String)()

        <TestCleanup>
        Public Sub Cleanup()
            For Each root In _tempRoots
                Try
                    If Directory.Exists(root) Then Directory.Delete(root, True)
                Catch
                End Try
            Next
            _tempRoots.Clear()
        End Sub

        Private Function MakeTemp() As String
            Dim dir As String = Path.Combine(Path.GetTempPath(), "BackupEngineTests_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(dir)
            _tempRoots.Add(dir)
            Return dir
        End Function

        Private Function BuildItems(dbFile As String, receiptsDir As String) As List(Of BackupItemSpec)
            Dim items As New List(Of BackupItemSpec)()
            items.Add(New BackupItemSpec() With {.SourcePath = dbFile, .IsFolder = False, .Description = "ฐานข้อมูล"})
            items.Add(New BackupItemSpec() With {.SourcePath = receiptsDir, .IsFolder = True, .Description = "ใบเสร็จ"})
            Return items
        End Function

        ''' <summary>สร้างต้นทาง: TempleDB.accdb + Receipts/ (2 ไฟล์ + 1 ซับโฟลเดอร์ 1 ไฟล์)</summary>
        Private Function BuildSource() As (sourceRoot As String, dbFile As String, receiptsDir As String)
            Dim root As String = MakeTemp()
            Dim dbFile As String = Path.Combine(root, "TempleDB.accdb")
            File.WriteAllText(dbFile, "fake-db-content")

            Dim receiptsDir As String = Path.Combine(root, "Receipts")
            Directory.CreateDirectory(receiptsDir)
            File.WriteAllText(Path.Combine(receiptsDir, "r1.jpg"), "receipt-1")
            File.WriteAllText(Path.Combine(receiptsDir, "r2.jpg"), "receipt-2")
            Dim subDir As String = Path.Combine(receiptsDir, "sub")
            Directory.CreateDirectory(subDir)
            File.WriteAllText(Path.Combine(subDir, "r3.jpg"), "receipt-3")

            Return (root, dbFile, receiptsDir)
        End Function

        <TestMethod>
        Public Sub ReceiptsComplete_BackupPasses()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_run")

            Dim result = BackupEngine.ExecuteBackup(items, dest)

            Assert.IsTrue(result.Success, "สำรองข้อมูลควรสำเร็จ: " & String.Join(" | ", result.Problems))
            Assert.AreEqual(4, result.SourceFileCount, "ควรมีไฟล์ทั้งหมด 4 ไฟล์ (db + 3 receipts)")
            Assert.AreEqual(4, result.CopiedFileCount)
            Assert.AreEqual(4, result.VerifiedFileCount)
            Assert.AreEqual(0, result.FailedFileCount)

            ' ไฟล์ทั้งหมดรวมซับโฟลเดอร์ต้องถูกสำรอง
            Assert.IsTrue(File.Exists(Path.Combine(dest, "TempleDB.accdb")))
            Assert.IsTrue(File.Exists(Path.Combine(dest, "Receipts", "r1.jpg")))
            Assert.IsTrue(File.Exists(Path.Combine(dest, "Receipts", "sub", "r3.jpg")))

            ' manifest + log ต้องถูกเขียน
            Assert.IsTrue(File.Exists(Path.Combine(dest, "manifest.json")))
            Assert.IsTrue(File.Exists(Path.Combine(dest, "manifest.csv")))
            Assert.IsTrue(File.Exists(Path.Combine(dest, "backup.log")))
            Assert.IsTrue(File.ReadAllText(Path.Combine(dest, "manifest.json")).Contains("Sha256"))
        End Sub

        <TestMethod>
        Public Sub ReceiptsRemovedFromBackup_VerificationFails()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_run")

            Dim backup = BackupEngine.ExecuteBackup(items, dest)
            Assert.IsTrue(backup.Success, "สำรองข้อมูลเริ่มต้นควรสำเร็จ")

            ' จำลอง: มีคนลบไฟล์ Receipts ออกจาก backup
            File.Delete(Path.Combine(dest, "Receipts", "r1.jpg"))

            Dim verify = BackupEngine.VerifyBackup(items, dest)

            Assert.IsFalse(verify.Success, "การตรวจสอบต้องล้มเหลวเมื่อไฟล์หายไป")
            Assert.IsTrue(verify.Problems.Exists(Function(p) p.Contains("r1.jpg")), "ต้องระบุไฟล์ที่หายไป")
        End Sub

        <TestMethod>
        Public Sub NewFileInSubfolder_IsCaptured()
            Dim root As String = MakeTemp()
            Dim dbFile As String = Path.Combine(root, "TempleDB.accdb")
            File.WriteAllText(dbFile, "db")
            Dim receiptsDir As String = Path.Combine(root, "Receipts")
            Directory.CreateDirectory(receiptsDir)

            ' ไฟล์ใหม่ในซับโฟลเดอร์ใหม่ (หลายระดับ)
            Dim newSub As String = Path.Combine(receiptsDir, "2026", "10")
            Directory.CreateDirectory(newSub)
            File.WriteAllText(Path.Combine(newSub, "new-receipt.jpg"), "new")

            Dim items = BuildItems(dbFile, receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_run")

            Dim result = BackupEngine.ExecuteBackup(items, dest)

            Assert.IsTrue(result.Success, String.Join(" | ", result.Problems))
            Assert.IsTrue(File.Exists(Path.Combine(dest, "Receipts", "2026", "10", "new-receipt.jpg")),
                          "ไฟล์ใหม่ในซับโฟลเดอร์ต้องถูกสำรองด้วย")
        End Sub

        ' ── Helpers สำหรับชุดทดสอบ registry/verify ──

        Private Function ToEntries(result As BackupResult) As List(Of BackupFileEntry)
            Dim list As New List(Of BackupFileEntry)()
            For Each rec In result.FileRecords
                list.Add(New BackupFileEntry() With {
                    .RelativePath = rec.RelativePath,
                    .SizeBytes = rec.SizeBytes,
                    .Sha256 = rec.Sha256
                })
            Next
            Return list
        End Function

        ''' <summary>หาอักษรไดรฟ์ที่ไม่มีอยู่จริง (จำลองไดรฟ์ที่ถูกถอด/ไม่เชื่อมต่อ)</summary>
        Private Function GetMissingDrivePath() As String
            Dim existing As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Try
                For Each di In DriveInfo.GetDrives()
                    existing.Add(di.Name)
                Next
            Catch
            End Try
            For c As Integer = AscW("Z"c) To AscW("D"c) Step -1
                Dim letter As String = ChrW(c) & ":\"
                If Not existing.Contains(letter) Then
                    Return Path.Combine(letter, "TempleBackup", "Backup_20260101_000000")
                End If
            Next
            Return Path.Combine("Z:\", "TempleBackup", "Backup_20260101_000000")
        End Function

        ' ── 1. สำรองครบสองไดรฟ์ → ผ่าน ──

        <TestMethod>
        Public Sub CompleteBackupsOnTwoDrives_ClassifyAsIdentical()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim driveA = MakeTemp()
            Dim driveB = MakeTemp()

            Dim b1 = BackupEngine.ExecuteBackup(items, Path.Combine(driveA, "Backup_1"))
            Dim b2 = BackupEngine.ExecuteBackup(items, Path.Combine(driveB, "Backup_2"))
            Assert.IsTrue(b1.Success AndAlso b2.Success, "สำรองทั้งสองไดรฟ์ต้องสำเร็จ: " & String.Join(" | ", b1.Problems) & " ; " & String.Join(" | ", b2.Problems))

            Dim v1 = BackupEngine.ClassifyBackup(Path.Combine(driveA, "Backup_1"), ToEntries(b1), items)
            Dim v2 = BackupEngine.ClassifyBackup(Path.Combine(driveB, "Backup_2"), ToEntries(b2), items)

            Assert.AreEqual(BackupStatusClass.Identical, v1.Classification)
            Assert.AreEqual(BackupStatusClass.Identical, v2.Classification)
            Assert.AreEqual("ตรงกัน", v1.DatabaseStatus)
            Assert.AreEqual("3 / 3", v1.ReceiptsStatus)
        End Sub

        ' ── 2. ใบเสร็จหาย → ไม่สมบูรณ์ ──

        <TestMethod>
        Public Sub ReceiptMissingFromBackup_ClassifyAsIncomplete()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest = Path.Combine(MakeTemp(), "Backup_run")

            Dim backup = BackupEngine.ExecuteBackup(items, dest)
            Assert.IsTrue(backup.Success, "สำรองเริ่มต้นต้องสำเร็จ")

            Dim recorded = ToEntries(backup)
            File.Delete(Path.Combine(dest, "Receipts", "r1.jpg"))

            Dim v = BackupEngine.ClassifyBackup(dest, recorded, items)

            Assert.AreEqual(BackupStatusClass.Incomplete, v.Classification)
            Assert.AreEqual("ไม่สมบูรณ์", v.OverallStatus)
            Assert.IsTrue(v.Problems.Exists(Function(p) p.Contains("r1.jpg")), "ต้องระบุไฟล์ใบเสร็จที่หายไป")
        End Sub

        ' ── 3. ไฟล์ถูกแก้ไข → เสียหาย ──

        <TestMethod>
        Public Sub AlteredFile_ClassifyAsCorrupted()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest = Path.Combine(MakeTemp(), "Backup_run")

            Dim backup = BackupEngine.ExecuteBackup(items, dest)
            Assert.IsTrue(backup.Success, "สำรองเริ่มต้นต้องสำเร็จ")

            Dim recorded = ToEntries(backup)
            ' แก้ไขเนื้อหาไฟล์ในสำรองให้ต่างจากต้นฉบับ
            File.WriteAllText(Path.Combine(dest, "Receipts", "r2.jpg"), "tampered-content")

            Dim v = BackupEngine.ClassifyBackup(dest, recorded, items)

            Assert.AreEqual(BackupStatusClass.Corrupted, v.Classification)
            Assert.AreEqual("เสียหาย", v.OverallStatus)
        End Sub

        ' ── 4. ไดรฟ์ถูกถอด/ไม่เชื่อมต่อ → "ไม่พบไดรฟ์" ──

        <TestMethod>
        Public Sub DriveDisconnected_ClassifyAsDriveMissing()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest = Path.Combine(MakeTemp(), "Backup_run")
            Dim backup = BackupEngine.ExecuteBackup(items, dest)

            Dim missingDrive = GetMissingDrivePath()

            Dim v = BackupEngine.ClassifyBackup(missingDrive, ToEntries(backup), items)

            Assert.AreEqual(BackupStatusClass.DriveMissing, v.Classification)
            Assert.AreEqual("ไม่พบไดรฟ์", v.DatabaseStatus)
            Assert.AreEqual("ไม่พบไดรฟ์", v.OverallStatus)
        End Sub

        ' ── 5. ใบเสร็จใหม่หลังสำรอง → "ยังไม่ได้สำรอง" ──

        <TestMethod>
        Public Sub NewReceiptAfterBackup_IsReportedAsUnbacked()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest = Path.Combine(MakeTemp(), "Backup_run")

            Dim backup = BackupEngine.ExecuteBackup(items, dest)
            Assert.IsTrue(backup.Success, "สำรองเริ่มต้นต้องสำเร็จ")

            ' เพิ่มใบเสร็จใหม่หลังการสำรอง
            File.WriteAllText(Path.Combine(src.receiptsDir, "new-receipt.jpg"), "new")

            Dim entry = New BackupRegistryEntry()
            For Each f In ToEntries(backup)
                entry.Files.Add(f)
            Next
            Dim backedPaths = BackupRegistry.CollectBackedUpPaths(New List(Of BackupRegistryEntry) From {entry})

            Dim unbacked = BackupEngine.FindUnbackedFiles(items, backedPaths)

            Dim expected = Path.Combine("Receipts", "new-receipt.jpg")
            Assert.IsTrue(unbacked.Contains(expected), "ไฟล์ใหม่ต้องถูกระบุว่าเป็นไฟล์ที่ยังไม่ได้สำรอง: " & String.Join(", ", unbacked))
        End Sub

        ' ── 6. สำรองบนดิสก์แต่ไม่อยู่ใน registry → สแกนพบ ──

        <TestMethod>
        Public Sub BackupOnDiskButNotInRegistry_IsDetectedByScan()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim driveRoot = MakeTemp()  ' จำลองรากของไดรฟ์
            Dim dest = Path.Combine(driveRoot, "Backup_20260101_000000")

            Dim backup = BackupEngine.ExecuteBackup(items, dest)
            Assert.IsTrue(backup.Success, "สำรองเริ่มต้นต้องสำเร็จ")

            Dim found = BackupEngine.ScanForBackupFolders(New List(Of String) From {driveRoot}, BackupRegistry.ProjectId, 3)

            Assert.IsTrue(found.Exists(Function(f) String.Equals(f, dest, StringComparison.OrdinalIgnoreCase)),
                          "สแกนต้องพบชุดสำรองที่อยู่บนดิสก์แม้ยังไม่มีใน registry")
        End Sub

        ' ──────────────────────────────────────────────────────────────
        ' A4 — การสำรองแบบ async / ล็อก / ยกเลิก / ปลายทาง / exception
        ' ──────────────────────────────────────────────────────────────

        ''' <summary>นับรายงาน progress แบบ synchronous (ไม่ผ่าน SynchronizationContext เพื่อทดสอบได้ deterministic)</summary>
        Private Class SyncProgress
            Implements IProgress(Of BackupProgress)
            Public Count As Integer = 0
            Public Sub Report(value As BackupProgress) Implements IProgress(Of BackupProgress).Report
                Count += 1
            End Sub
        End Class

        ''' <summary>สำรองผ่าน async path สำเร็จ และรายงาน progress อย่างน้อยหนึ่งครั้ง</summary>
        <TestMethod>
        Public Async Function AsyncBackup_Completes() As Task
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_async")
            Dim progress As New SyncProgress()

            Dim result = Await BackupEngine.ExecuteBackupAsync(items, dest, progress, CancellationToken.None)

            Assert.IsTrue(result.Success, "สำรอง async ควรสำเร็จ: " & String.Join(" | ", result.Problems))
            Assert.AreEqual(4, result.SourceFileCount)
            Assert.AreEqual(4, result.VerifiedFileCount)
            Assert.IsFalse(result.Cancelled)
            Assert.IsTrue(progress.Count > 0, "ต้องรายงาน progress อย่างน้อยหนึ่งครั้ง")
        End Function

        ''' <summary>ฐานข้อมูลถูกล็อก → retry ครบจำนวนครั้งแล้วหยุด ไม่ค้าง</summary>
        <TestMethod>
        Public Sub LockedDatabase_StopsAfterMaxRetries_WithoutHanging()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_locked")

            Dim lockStream As FileStream = Nothing
            Try
                ' ล็อก .accdb แบบ exclusive เพื่อจำลองฐานข้อมูลที่ถูกเปิดใช้งานอยู่
                lockStream = New FileStream(src.dbFile, FileMode.Open, FileAccess.Read, FileShare.None)

                Dim sw = Stopwatch.StartNew()
                Dim result = BackupEngine.ExecuteBackup(items, dest)
                sw.Stop()

                Assert.IsTrue(result.DatabaseLocked, "ต้องตรวจพบว่าฐานข้อมูลถูกล็อก")
                Assert.IsFalse(result.Success, "สำรองต้องไม่สำเร็จเมื่อฐานข้อมูลถูกล็อก")
                ' retry ต้องหยุดหลังครบ MaxRetryAttempts ไม่ค้าง (buffer เผื่อเวลาทำงานจริง)
                Assert.IsTrue(sw.Elapsed < TimeSpan.FromSeconds(10),
                              "retry ต้องหยุดในเวลาจำกัด ไม่ค้าง: " & sw.Elapsed.ToString())
            Finally
                If lockStream IsNot Nothing Then lockStream.Dispose()
            End Try
        End Sub

        ''' <summary>ยกเลิกกลางทาง → กลับมาเร็ว ไม่ค้าง และทำเครื่องหมายว่ายกเลิก/ไม่สมบูรณ์</summary>
        <TestMethod>
        Public Async Function CancelMidBackup_MarksIncompleteAndReturns() As Task
            Dim src = BuildSource()
            ' เพิ่มไฟล์จำนวนมากเพื่อให้มีงานคัดลอกให้ยกเลิกได้
            For i As Integer = 1 To 200
                File.WriteAllText(Path.Combine(src.receiptsDir, "many_" & i.ToString() & ".jpg"), New String("x"c, 4096))
            Next
            Dim items = BuildItems(src.dbFile, src.receiptsDir)
            Dim dest As String = Path.Combine(MakeTemp(), "Backup_cancel")

            Using cts As New CancellationTokenSource()
                Dim backupTask As Task(Of BackupResult) = BackupEngine.ExecuteBackupAsync(items, dest, Nothing, cts.Token)
                cts.Cancel()

                Dim timeoutTask As Task = Task.Delay(TimeSpan.FromSeconds(10))
                Dim firstDone As Task = Await Task.WhenAny(backupTask, timeoutTask)
                Assert.AreSame(backupTask, firstDone, "การยกเลิกต้องคืนค่าภายในเวลาที่กำหนด (ไม่ค้าง)")

                Dim result = Await backupTask
                Assert.IsTrue(result.Cancelled, "ต้องทำเครื่องหมายว่ายกเลิก")
                Assert.IsFalse(result.Success, "ชุดสำรองที่ถูกยกเลิกต้องไม่ถือว่าสมบูรณ์")
            End Using
        End Function

        ''' <summary>ปลายทางอยู่ภายในโฟลเดอร์ต้นทาง → ถูกปฏิเสธ</summary>
        <TestMethod>
        Public Sub DestinationInsideSource_IsRefused()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)

            ' ปลายทางอยู่ภายในโฟลเดอร์ต้นทาง (Receipts) → ต้องถูกปฏิเสธ
            Dim insideReceipts As String = Path.Combine(src.receiptsDir, "sub")
            Dim problem As String = BackupEngine.FindContainmentProblem(items, insideReceipts)
            Assert.IsFalse(String.IsNullOrEmpty(problem), "ปลายทางในโฟลเดอร์ต้นทางต้องถูกปฏิเสธ")
            Assert.IsTrue(problem.Contains("ไม่สามารถสำรองข้อมูลลงในโฟลเดอร์ต้นทางได้"), "ต้องแจ้งเป็นภาษาไทย")

            ' ปลายทางเท่ากับโฟลเดอร์แม่ของฐานข้อมูล → ต้องถูกปฏิเสธด้วย
            Dim sameRoot As String = Path.GetDirectoryName(src.dbFile)
            Dim problem2 As String = BackupEngine.FindContainmentProblem(items, sameRoot)
            Assert.IsFalse(String.IsNullOrEmpty(problem2), "ปลายทางเท่ากับโฟลเดอร์ต้นทางต้องถูกปฏิเสธ")

            ' ปลายทางอยู่นอกโฟลเดอร์ต้นทาง → ปลอดภัย
            Dim safe As String = Path.Combine(MakeTemp(), "outside")
            Assert.AreEqual("", BackupEngine.FindContainmentProblem(items, safe))
        End Sub

        ''' <summary>เกิด exception ระหว่างสำรอง → คืนค่า Failure แทนที่จะ throw/ค้าง</summary>
        <TestMethod>
        Public Sub ExceptionDuringBackup_ReturnsFailureInsteadOfThrowing()
            Dim src = BuildSource()
            Dim items = BuildItems(src.dbFile, src.receiptsDir)

            ' สร้างไฟล์ชื่อเดียวกับปลายทาง → Directory.CreateDirectory จะ throw IOException
            Dim destFile As String = Path.Combine(MakeTemp(), "not_a_folder")
            File.WriteAllText(destFile, "x")

            Dim result = BackupEngine.ExecuteBackup(items, destFile)

            Assert.IsFalse(result.Success, "ต้องคืนค่า Failure แทนที่จะ throw")
            Assert.IsTrue(result.Problems.Count > 0, "ต้องบันทึกปัญหาไว้ใน result")
        End Sub

    End Class
End Namespace
