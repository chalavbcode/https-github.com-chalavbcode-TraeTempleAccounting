Option Strict Off
Option Explicit On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections.Generic

Namespace TempleAccounting

    ''' <summary>
    ''' ทดสอบการประเมินชุดสำรองรุ่นเก่า (ไม่มี manifest) ด้วยโฟลเดอร์ชั่วคราว
    ''' ครอบคลุม: ไม่มี Receipts, สมบูรณ์, ไฟล์เปลี่ยน, ไดรฟ์ไม่เชื่อมต่อ, สแกนซ้ำ, ไม่พบชุด
    ''' </summary>
    <TestClass>
    Public Class LegacyBackupTests
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
            Dim dir As String = Path.Combine(Path.GetTempPath(), "LegacyBackupTests_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(dir)
            _tempRoots.Add(dir)
            Return dir
        End Function

        ''' <summary>สร้างไฟล์ .accdb ปลอมที่มี header ของ Access ถูกต้อง (เพื่อให้ไม่ถูกจัดเป็น corrupted)</summary>
        Private Function MakeAccessFile(path As String, content As String) As String
            Dim header As Byte() = {0, 1, 0, 0}
            Dim sig As Byte() = Encoding.ASCII.GetBytes("Standard ACE DB")
            Dim tail As Byte() = {0, 2, 0, 0}
            Dim body As Byte() = Encoding.UTF8.GetBytes(content)
            Using fs As New FileStream(path, FileMode.Create, FileAccess.Write)
                fs.Write(header, 0, header.Length)
                fs.Write(sig, 0, sig.Length)
                fs.Write(tail, 0, tail.Length)
                fs.Write(body, 0, body.Length)
            End Using
            Return path
        End Function

        ''' <summary>ต้นฉบับ: TempleAccounting.accdb + Receipts (2 ไฟล์)</summary>
        Private Function BuildCurrent() As (dbPath As String, receiptsDir As String, items As List(Of BackupItemSpec))
            Dim root As String = MakeTemp()
            Dim dbPath As String = MakeAccessFile(Path.Combine(root, "TempleAccounting.accdb"), "db-v1")
            Dim receiptsDir As String = Path.Combine(root, "Receipts")
            Directory.CreateDirectory(receiptsDir)
            File.WriteAllText(Path.Combine(receiptsDir, "r1.jpg"), "receipt-1")
            File.WriteAllText(Path.Combine(receiptsDir, "r2.jpg"), "receipt-2")

            Dim items As New List(Of BackupItemSpec)()
            items.Add(New BackupItemSpec() With {.SourcePath = dbPath, .IsFolder = False, .Description = "ฐานข้อมูล"})
            items.Add(New BackupItemSpec() With {.SourcePath = receiptsDir, .IsFolder = True, .Description = "ใบเสร็จ"})
            Return (dbPath, receiptsDir, items)
        End Function

        ''' <summary>สร้างชุดสำรองรุ่นเก่า (ชื่อโฟลเดอร์ Backup_* ไม่มี manifest)</summary>
        Private Function MakeLegacyBackup(parent As String, dbSource As String, receiptsSource As String,
                                          Optional includeReceipts As Boolean = True,
                                          Optional folderName As String = "Backup_20260101_010101") As String
            Dim folder As String = Path.Combine(parent, folderName)
            Directory.CreateDirectory(folder)
            File.Copy(dbSource, Path.Combine(folder, "TempleDB_legacy.accdb"), True)
            If includeReceipts AndAlso Directory.Exists(receiptsSource) Then
                Dim dest As String = Path.Combine(folder, "Receipts")
                Directory.CreateDirectory(dest)
                For Each f As String In Directory.GetFiles(receiptsSource)
                    File.Copy(f, Path.Combine(dest, Path.GetFileName(f)), True)
                Next
            End If
            Return folder
        End Function

        <TestMethod>
        Public Sub LegacySet_WithoutManifestAndWithoutReceipts_IsAddedAsIncomplete()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir, includeReceipts:=False)

            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)
            Assert.IsTrue(ev.IsLegacy, "ชุดที่ไม่มี manifest ต้องถูกทำเครื่องหมาย legacy")
            Assert.AreEqual(BackupStatusClass.Incomplete, ev.Classification)
            Assert.IsFalse(ev.IsComplete, "ชุดที่ไม่มี Receipts ต้องไม่ถูกนับว่าใช้ได้")
            StringAssert.Contains(ev.Reason, "Receipts")
            Assert.IsTrue(ev.DatabaseExists)
            Assert.IsTrue(ev.Files.Count >= 1, "ต้องมี manifest ที่คำนวณได้เก็บไว้ใน registry")

            Dim regPath As String = Path.Combine(MakeTemp(), "backup_registry.json")
            Assert.IsTrue(BackupLegacyEvaluator.RegisterBackup(regPath, ev), "ต้องเพิ่มชุดที่ไม่สมบูรณ์เข้าสู่ registry")

            Dim doc As RegistryDocument = BackupRegistry.Load(regPath)
            Assert.AreEqual(1, doc.Entries.Count)
            Assert.IsFalse(doc.Entries(0).IsComplete)
            Assert.IsTrue(doc.Entries(0).IsLegacy)
            Assert.IsTrue(doc.Entries(0).Files.Count > 0)

            ' ต้องไม่เขียนไฟล์ใดลงในโฟลเดอร์สำรองรุ่นเก่า
            Assert.IsFalse(File.Exists(Path.Combine(folder, "backup_manifest.json")), "ห้ามเขียน manifest ลงโฟลเดอร์เก่า")
        End Sub

        <TestMethod>
        Public Sub LegacySet_Intact_IsIdenticalAndComplete()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir)

            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)
            Assert.AreEqual(BackupStatusClass.Identical, ev.Classification)
            Assert.IsTrue(ev.IsComplete)
            Assert.IsTrue(ev.DatabaseMatchesCurrent)
            Assert.AreEqual(2, ev.ReceiptsOk)
            Assert.AreEqual(2, ev.CurrentReceiptCount)
            Assert.AreEqual("", ev.Reason)
        End Sub

        <TestMethod>
        Public Sub LegacySet_OlderButIntact_WhenReceiptContentChanged()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir)

            ' ต้นฉบับเปลี่ยนหลังการสำรอง → ชุดสำรองยังสมบูรณ์แต่เก่ากว่า
            File.WriteAllText(Path.Combine(cur.receiptsDir, "r1.jpg"), "receipt-1-modified")
            Dim snap2 As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)

            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap2)
            Assert.AreEqual(BackupStatusClass.OlderButIntact, ev.Classification)
            Assert.IsTrue(ev.IsComplete, "ชุดสำรองที่เก่ากว่าแต่ครบถ้วนยังใช้ได้")
            Assert.AreEqual(1, ev.ReceiptsChanged.Count)
        End Sub

        <TestMethod>
        Public Sub LegacySet_ZeroByteReceipt_IsCorrupted()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir)

            ' ทำไฟล์ใบเสร็จในชุดสำรองเสียหาย (ขนาด 0)
            File.WriteAllBytes(Path.Combine(folder, "Receipts", "r1.jpg"), New Byte() {})

            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)
            Assert.AreEqual(BackupStatusClass.Corrupted, ev.Classification)
            Assert.IsFalse(ev.IsComplete)
        End Sub

        <TestMethod>
        Public Sub LegacySet_BrokenDatabaseHeader_IsCorrupted()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir)

            ' เขียนทับไฟล์ฐานข้อมูลในชุดสำรองด้วยข้อมูลที่ไม่ใช่ Access
            File.WriteAllText(Path.Combine(folder, "TempleDB_legacy.accdb"), "not-an-access-file")

            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)
            Assert.AreEqual(BackupStatusClass.Corrupted, ev.Classification)
            Assert.IsFalse(ev.IsComplete)
        End Sub

        <TestMethod>
        Public Sub DriveNotConnected_IsDriveMissing()
            If Directory.Exists("Q:\") Then
                Assert.Inconclusive("ไดรฟ์ Q: มีอยู่จริงในเครื่องนี้ จึงทดสอบไดรฟ์ที่ไม่เชื่อมต่อไม่ได้")
            End If

            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate("Q:\Backup_20260101_010101", snap)

            Assert.AreEqual(BackupStatusClass.DriveMissing, ev.Classification)
            Assert.IsFalse(ev.DriveReady)
            Assert.IsFalse(ev.IsComplete)
        End Sub

        <TestMethod>
        Public Sub DuplicateScan_NotAddedTwice()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim folder As String = MakeLegacyBackup(MakeTemp(), cur.dbPath, cur.receiptsDir)
            Dim ev As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)

            Dim regPath As String = Path.Combine(MakeTemp(), "backup_registry.json")
            Assert.IsTrue(BackupLegacyEvaluator.RegisterBackup(regPath, ev), "ครั้งแรกต้องเพิ่มสำเร็จ")

            ' สแกนซ้ำ: ประเมินใหม่แล้วเพิ่มอีกครั้ง → ต้องไม่เพิ่มซ้ำ
            Dim ev2 As LegacyEvaluation = BackupLegacyEvaluator.Evaluate(folder, snap)
            Assert.IsFalse(BackupLegacyEvaluator.RegisterBackup(regPath, ev2), "ครั้งที่สองต้องถูกข้าม")

            Dim doc As RegistryDocument = BackupRegistry.Load(regPath)
            Assert.AreEqual(1, doc.Entries.Count)
        End Sub

        <TestMethod>
        Public Sub ScanWithZeroSets_ReturnsEmptyList()
            Dim emptyRoot As String = MakeTemp()
            Dim found As List(Of String) = BackupEngine.ScanForBackupFolders(
                New List(Of String) From {emptyRoot}, BackupRegistry.ProjectId, 3)
            Assert.IsNotNull(found)
            Assert.AreEqual(0, found.Count)
        End Sub

        <TestMethod>
        Public Sub Summary_NoCompleteBackup_ListsAllFilesAndPrompt()
            Dim cur = BuildCurrent()
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)
            Dim regPath As String = Path.Combine(MakeTemp(), "backup_registry.json")

            Dim info As BackupSummaryInfo = BackupStatusBuilder.Build(regPath, snap)
            Assert.IsFalse(info.HasCompleteBackup)
            StringAssert.Contains(info.Text, "ยังไม่มีชุดสำรองที่ครบถ้วน")
            StringAssert.Contains(info.Text, "ไฟล์ที่ยังไม่ได้สำรอง (3 ไฟล์)")
            StringAssert.Contains(info.Text, "ฐานข้อมูล (1)")
            StringAssert.Contains(info.Text, "ใบเสร็จ (2)")
        End Sub

        <TestMethod>
        Public Sub Summary_LegacyCompleteWithChangedDb_NotesHashDifference()
            Dim cur = BuildCurrent()
            Dim parent As String = MakeTemp()
            Dim folder As String = MakeLegacyBackup(parent, cur.dbPath, cur.receiptsDir)
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)

            Dim regPath As String = Path.Combine(MakeTemp(), "backup_registry.json")
            Assert.IsTrue(BackupLegacyEvaluator.RegisterBackup(regPath, BackupLegacyEvaluator.Evaluate(folder, snap)))

            ' ฐานข้อมูลถูกแก้ไขหลังการสำรอง → ต้องถูกระบุว่า hash ต่าง และเตือนว่าอาจถูกแก้ไข/ถูกล็อก
            MakeAccessFile(cur.dbPath, "db-v2-after-backup")
            Dim snap2 As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)

            Dim info As BackupSummaryInfo = BackupStatusBuilder.Build(regPath, snap2)
            Assert.IsTrue(info.HasCompleteBackup)
            StringAssert.Contains(info.Text, "ไฟล์ที่ยังไม่ได้สำรอง/เปลี่ยนแปลง (1 ไฟล์)")
            StringAssert.Contains(info.Text, "hash ต่างจากชุดสำรองล่าสุด")
            StringAssert.Contains(info.Text, "ถูกเปิดใช้งานอยู่")
        End Sub

        <TestMethod>
        Public Sub Summary_IdenticalBackup_HasNoUnbackedFiles()
            Dim cur = BuildCurrent()
            Dim parent As String = MakeTemp()
            Dim folder As String = MakeLegacyBackup(parent, cur.dbPath, cur.receiptsDir)
            Dim snap As CurrentDataSnapshot = BackupLegacyEvaluator.BuildCurrentSnapshot(cur.items)

            Dim regPath As String = Path.Combine(MakeTemp(), "backup_registry.json")
            Assert.IsTrue(BackupLegacyEvaluator.RegisterBackup(regPath, BackupLegacyEvaluator.Evaluate(folder, snap)))

            Dim info As BackupSummaryInfo = BackupStatusBuilder.Build(regPath, snap)
            Assert.IsTrue(info.HasCompleteBackup)
            StringAssert.Contains(info.Text, "ไม่มีไฟล์ใหม่ที่ยังไม่ได้สำรอง")
        End Sub

        <TestMethod>
        Public Sub ScanFindsLegacyFolder_AndParsesBuddhistTimestamp()
            Dim cur = BuildCurrent()
            Dim parent As String = MakeTemp()
            Dim folder As String = MakeLegacyBackup(parent, cur.dbPath, cur.receiptsDir, folderName:="Backup_25690804_145723")

            Dim found As List(Of String) = BackupEngine.ScanForBackupFolders(
                New List(Of String) From {parent}, BackupRegistry.ProjectId, 3)
            Assert.AreEqual(1, found.Count)
            Assert.AreEqual(folder, found(0))

            Dim ts As Date? = BackupLegacyEvaluator.ParseFolderTimestamp(folder)
            Assert.IsTrue(ts.HasValue, "ต้องอ่านวันที่จากชื่อโฟลเดอร์ได้")
            Assert.AreEqual(New Date(2026, 8, 4, 14, 57, 23), ts.Value)
        End Sub

    End Class

End Namespace
