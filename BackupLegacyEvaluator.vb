Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading

Namespace TempleAccounting

    ''' <summary>
    ''' ค่า SHA-256 และการตรวจ header ของไฟล์ — ใช้ร่วมกันทั้งการสำรองและการประเมินชุดสำรองรุ่นเก่า
    ''' อ่านไฟล์แบบ FileShare.ReadWrite เพื่อไม่แตะ/ไม่ล็อกไฟล์ต้นฉบับ
    ''' </summary>
    Public Module BackupHashing

        ''' <summary>คำนวณ SHA-256 ของไฟล์ (อ่านไฟล์ที่เปิดใช้งานอยู่ได้ ไม่แก้ไขไฟล์)</summary>
        Public Function ComputeSha256File(filePath As String) As String
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                Using sha As SHA256 = SHA256.Create()
                    Dim hashBytes As Byte() = sha.ComputeHash(fs)
                    Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
                End Using
            End Using
        End Function

        ''' <summary>
        ''' ตรวจ header ของไฟล์ Access (.accdb/.mdb) — ไฟล์ที่ถูกตัดทอน/เขียนทับจะไม่ผ่าน
        ''' (ไฟล์ Access ที่สมบูรณ์มีข้อความ "Standard ACE DB" หรือ "Standard Jet DB" ที่ offset 4)
        ''' </summary>
        Public Function HasAccessHeader(filePath As String) As Boolean
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                Dim buf(31) As Byte
                Dim n As Integer = fs.Read(buf, 0, buf.Length)
                If n < 20 Then Return False
                Dim sig As String = Encoding.ASCII.GetString(buf, 4, 15)
                Return sig.StartsWith("Standard ACE DB", StringComparison.Ordinal) OrElse
                       sig.StartsWith("Standard Jet DB", StringComparison.Ordinal)
            End Using
        End Function

    End Module

    ''' <summary>สรุปข้อมูลปัจจุบัน (ต้นฉบับ) ที่ใช้เทียบกับชุดสำรองแต่ละชุด — คำนวณครั้งเดียวต่อการสแกน</summary>
    Public Class CurrentDataSnapshot
        Public Property DatabaseSourcePath As String
        Public Property DatabaseFileName As String
        Public Property DatabaseSize As Long
        Public Property DatabaseSha256 As String
        Public Property DatabaseReadError As String
        Public Property ReceiptsFolderName As String = "Receipts"
        ''' <summary>ไฟล์ใบเสร็จปัจจุบัน (RelativePath = \"Receipts\\ชื่อไฟล์\")</summary>
        Public Property Receipts As New List(Of BackupFileEntry)()
        Public Property ReceiptHashErrors As New List(Of String)()
    End Class

    ''' <summary>ผลการประเมินชุดสำรองหนึ่งชุด (ใช้ได้ทั้งชุดรุ่นเก่าและชุดที่มี manifest)</summary>
    Public Class LegacyEvaluation
        Public Property BackupFolder As String
        ''' <summary>เวลาจากชื่อโฟลเดอร์ (yyyy-MM-dd HH:mm:ss) — ว่างถ้าอ่านไม่ได้</summary>
        Public Property FolderTimestamp As String
        Public Property DriveRoot As String
        Public Property DriveReady As Boolean
        Public Property FolderExists As Boolean
        ''' <summary>มี backup_manifest.json (manifest ที่ registry เขียนลงโฟลเดอร์) หรือไม่</summary>
        Public Property ManifestExists As Boolean
        ''' <summary>มี manifest.json ของ BackupEngine หรือไม่</summary>
        Public Property EngineManifestExists As Boolean
        ''' <summary>True = ชุดรุ่นเก่า (ไม่มี backup_manifest.json) → ประเมินจากการสแกนโฟลเดอร์</summary>
        Public Property IsLegacy As Boolean
        Public Property DatabaseRelativePath As String
        Public Property DatabaseExists As Boolean
        Public Property DatabaseSizeBytes As Long
        Public Property DatabaseSha256 As String
        ''' <summary>True = ฐานข้อมูลในชุดสำรองเหมือนต้นฉบับปัจจุบันทุกไบต์</summary>
        Public Property DatabaseMatchesCurrent As Boolean
        Public Property ReceiptsFolderExists As Boolean
        ''' <summary>จำนวนไฟล์ใบเสร็จที่อยู่ในชุดสำรอง (รวมซับโฟลเดอร์)</summary>
        Public Property BackupReceiptCount As Integer
        ''' <summary>จำนวนไฟล์ใบเสร็จปัจจุบันที่หาเจอและตรงกันทุกไบต์</summary>
        Public Property ReceiptsOk As Integer
        ''' <summary>จำนวนไฟล์ใบเสร็จปัจจุบันทั้งหมดที่ใช้เทียบ</summary>
        Public Property CurrentReceiptCount As Integer
        Public Property ReceiptsMissing As New List(Of String)()
        Public Property ReceiptsChanged As New List(Of String)()
        Public Property ReceiptsExtra As Integer
        Public Property Classification As BackupStatusClass
        Public Property StatusText As String = ""
        ''' <summary>เหตุผลสั้น ๆ ของการจัดหมวด (ว่างถ้าสมบูรณ์และตรงกับปัจจุบัน)</summary>
        Public Property Reason As String = ""
        Public Property Problems As New List(Of String)()
        ''' <summary>manifest ที่คำนวณได้จริงจากการสแกนโฟลเดอร์ (เก็บเฉพาะใน registry ไม่เขียนลงโฟลเดอร์เก่า)</summary>
        Public Property Files As New List(Of BackupFileEntry)()
        ''' <summary>True = เป็นชุดสำรองที่ครบถ้วน (ใช้เป็นชุดสำรองจริงได้)</summary>
        Public Property IsComplete As Boolean
    End Class

    ''' <summary>
    ''' ประเมินชุดสำรองรุ่นเก่า (ก่อนมี registry/manifest) โดยสแกนโฟลเดอร์จริง อ่านขนาด + SHA-256
    ''' แล้วเทียบกับต้นฉบับปัจจุบัน — อ่านเท่านั้น ไม่เขียน/ลบ/ย้ายไฟล์ในโฟลเดอร์สำรองเลย
    ''' เป็นโค้ดล้วน (pure) ไม่ผูกกับ WinForms/AppPaths จึงทดสอบอัตโนมัติได้
    ''' </summary>
    Public Module BackupLegacyEvaluator

        ''' <summary>สร้าง snapshot ของต้นฉบับปัจจุบัน (ฐานข้อมูล + ใบเสร็จ) พร้อม SHA-256</summary>
        Public Function BuildCurrentSnapshot(items As List(Of BackupItemSpec), Optional token As CancellationToken = Nothing) As CurrentDataSnapshot
            Dim snap As New CurrentDataSnapshot()
            If items Is Nothing Then Return snap

            For Each item In items
                If item Is Nothing OrElse String.IsNullOrEmpty(item.SourcePath) Then Continue For
                If token.IsCancellationRequested Then Exit For

                If item.IsFolder Then
                    Dim folderName As String = Path.GetFileName(item.SourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    If Not String.IsNullOrEmpty(folderName) Then snap.ReceiptsFolderName = folderName
                    If Not Directory.Exists(item.SourcePath) Then Continue For

                    Dim files As String()
                    Try
                        files = Directory.GetFiles(item.SourcePath, "*", SearchOption.AllDirectories)
                    Catch ex As Exception
                        snap.ReceiptHashErrors.Add(item.SourcePath & " (" & ex.Message & ")")
                        Continue For
                    End Try

                    For Each f As String In files
                        If token.IsCancellationRequested Then Exit For
                        Dim relWithin As String = Path.GetRelativePath(item.SourcePath, f)
                        Dim entry As New BackupFileEntry() With {
                            .RelativePath = Path.Combine(folderName, relWithin),
                            .SizeBytes = SafeFileSize(f)
                        }
                        Try
                            entry.Sha256 = BackupHashing.ComputeSha256File(f)
                        Catch ex As Exception
                            entry.Sha256 = ""
                            snap.ReceiptHashErrors.Add(entry.RelativePath & " (" & ex.Message & ")")
                        End Try
                        snap.Receipts.Add(entry)
                    Next
                Else
                    ' รายการไฟล์เดี่ยว = ฐานข้อมูล
                    If snap.DatabaseSourcePath Is Nothing Then
                        snap.DatabaseSourcePath = item.SourcePath
                        snap.DatabaseFileName = Path.GetFileName(item.SourcePath)
                        If File.Exists(item.SourcePath) Then
                            snap.DatabaseSize = SafeFileSize(item.SourcePath)
                            Try
                                snap.DatabaseSha256 = BackupHashing.ComputeSha256File(item.SourcePath)
                            Catch ex As Exception
                                snap.DatabaseReadError = ex.Message
                            End Try
                        Else
                            snap.DatabaseReadError = "ไม่พบไฟล์ต้นฉบับ: " & item.SourcePath
                        End If
                    End If
                End If
            Next

            Return snap
        End Function

        ''' <summary>
        ''' ประเมินชุดสำรองหนึ่งชุดจากไฟล์จริงในโฟลเดอร์ แล้วจัดหมวด:
        ''' ตรงกับปัจจุบัน / เก่ากว่าแต่สมบูรณ์ / ไม่สมบูรณ์ / เสียหาย / ไม่พบไดรฟ์
        ''' ไม่อ่านหรือเขียนไฟล์ใดนอกโฟลเดอร์สำรอง (อ่านเท่านั้น)
        ''' </summary>
        Public Function Evaluate(backupFolder As String, snapshot As CurrentDataSnapshot, Optional token As CancellationToken = Nothing) As LegacyEvaluation
            Dim e As New LegacyEvaluation() With {.BackupFolder = backupFolder}
            If snapshot Is Nothing Then snapshot = New CurrentDataSnapshot()
            e.CurrentReceiptCount = snapshot.Receipts.Count

            ' 1. ไดรฟ์ / โฟลเดอร์
            Try
                e.DriveRoot = Path.GetPathRoot(backupFolder)
            Catch
                e.DriveRoot = ""
            End Try
            If String.IsNullOrEmpty(e.DriveRoot) Then
                e.DriveReady = False
            Else
                e.DriveReady = Directory.Exists(e.DriveRoot)
            End If

            Dim ts As Date? = ParseFolderTimestamp(backupFolder)
            If ts.HasValue Then e.FolderTimestamp = ts.Value.ToString("yyyy-MM-dd HH:mm:ss")

            If Not e.DriveReady Then
                e.FolderExists = False
                e.Classification = BackupStatusClass.DriveMissing
                e.StatusText = "ไม่พบไดรฟ์"
                e.Reason = "ไม่พบไดรฟ์ " & If(e.DriveRoot, "(ไม่ทราบ)")
                e.Problems.Add("ไม่พบไดรฟ์: " & If(e.DriveRoot, backupFolder))
                Return e
            End If

            e.FolderExists = Directory.Exists(backupFolder)
            If Not e.FolderExists Then
                e.Classification = BackupStatusClass.Incomplete
                e.StatusText = StatusTextFor(BackupStatusClass.Incomplete)
                e.Reason = "ไม่พบโฟลเดอร์สำรองข้อมูล"
                e.Problems.Add("ไม่พบโฟลเดอร์สำรองข้อมูล: " & backupFolder)
                Return e
            End If

            ' 2. manifest (มีหรือไม่มี)
            e.ManifestExists = File.Exists(Path.Combine(backupFolder, "backup_manifest.json"))
            e.EngineManifestExists = File.Exists(Path.Combine(backupFolder, "manifest.json"))
            e.IsLegacy = Not e.ManifestExists

            Dim corrupted As New List(Of String)()
            Dim hashCache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            ' 3. ฐานข้อมูล (.accdb) ในโฟลเดอร์นี้
            Dim dbPath As String = FindDatabaseFile(backupFolder)
            If String.IsNullOrEmpty(dbPath) Then
                e.DatabaseExists = False
                e.Classification = BackupStatusClass.Incomplete
                e.StatusText = StatusTextFor(BackupStatusClass.Incomplete)
                e.Reason = "ไม่พบฐานข้อมูล (.accdb)"
                e.Problems.Add("ไม่พบฐานข้อมูล (.accdb) ในโฟลเดอร์สำรอง")
                ' ยังเก็บ manifest ของใบเสร็จไว้ใน registry แม้ชุดนี้จะไม่มีฐานข้อมูล
                AddReceiptsOnly(e, backupFolder, snapshot, hashCache, corrupted, token)
                Return e
            End If

            e.DatabaseExists = True
            e.DatabaseRelativePath = Path.GetFileName(dbPath)
            e.DatabaseSizeBytes = SafeFileSize(dbPath)

            If token.IsCancellationRequested Then Return e

            Try
                If e.DatabaseSizeBytes <= 0 Then
                    corrupted.Add("ไฟล์ฐานข้อมูลมีขนาด 0 ไบต์")
                    e.Problems.Add("ไฟล์ฐานข้อมูลมีขนาด 0 ไบต์: " & e.DatabaseRelativePath)
                ElseIf Not BackupHashing.HasAccessHeader(dbPath) Then
                    corrupted.Add("ไฟล์ฐานข้อมูลไม่สมบูรณ์ (header ของ Access ไม่ถูกต้อง)")
                    e.Problems.Add("ไฟล์ฐานข้อมูลไม่สมบูรณ์ (header ของ Access ไม่ถูกต้อง): " & e.DatabaseRelativePath)
                Else
                    Dim h As String = BackupHashing.ComputeSha256File(dbPath)
                    e.DatabaseSha256 = h
                    hashCache(dbPath) = h
                    If Not String.IsNullOrEmpty(snapshot.DatabaseSha256) Then
                        e.DatabaseMatchesCurrent = String.Equals(h, snapshot.DatabaseSha256, StringComparison.OrdinalIgnoreCase)
                    End If
                End If
            Catch ex As Exception
                corrupted.Add("อ่านไฟล์ฐานข้อมูลไม่ได้ (" & ex.Message & ")")
                e.Problems.Add("อ่านไฟล์ฐานข้อมูลไม่ได้: " & ex.Message)
            End Try

            If Not String.IsNullOrEmpty(e.DatabaseRelativePath) Then
                e.Files.Add(New BackupFileEntry() With {
                    .RelativePath = e.DatabaseRelativePath,
                    .SizeBytes = e.DatabaseSizeBytes,
                    .Sha256 = e.DatabaseSha256
                })
            End If

            ' 4. ใบเสร็จ
            AddReceiptsOnly(e, backupFolder, snapshot, hashCache, corrupted, token)

            ' 5. จัดหมวด
            FinalizeClassification(e, corrupted)
            Return e
        End Function

        ''' <summary>ประเมินใบเสร็จ (ใช้ทั้งกรณีมี/ไม่มีฐานข้อมูล) — เติม Receipts*, Files, Problems</summary>
        Private Sub AddReceiptsOnly(e As LegacyEvaluation, backupFolder As String, snapshot As CurrentDataSnapshot,
                                    hashCache As Dictionary(Of String, String), corrupted As List(Of String),
                                    token As CancellationToken)
            Dim receiptsDir As String = Path.Combine(backupFolder, "Receipts")
            e.ReceiptsFolderExists = Directory.Exists(receiptsDir)
            e.CurrentReceiptCount = snapshot.Receipts.Count

            ' 4.1 นับ/แฮชไฟล์ใบเสร็จที่อยู่ในชุดสำรอง (เพื่อทำ manifest ที่คำนวณได้)
            If e.ReceiptsFolderExists Then
                Try
                    For Each f As String In Directory.GetFiles(receiptsDir, "*", SearchOption.AllDirectories)
                        If token.IsCancellationRequested Then Exit For
                        Dim rel As String = Path.Combine("Receipts", Path.GetRelativePath(receiptsDir, f))
                        Dim size As Long = SafeFileSize(f)
                        Dim h As String = ""
                        Try
                            If hashCache.ContainsKey(f) Then
                                h = hashCache(f)
                            Else
                                If size > 0 Then h = BackupHashing.ComputeSha256File(f)
                                hashCache(f) = h
                            End If
                        Catch ex As Exception
                            corrupted.Add("อ่านไฟล์ใบเสร็จไม่ได้: " & rel & " (" & ex.Message & ")")
                            e.Problems.Add("อ่านไฟล์ใบเสร็จไม่ได้: " & rel & " (" & ex.Message & ")")
                        End Try
                        If size <= 0 Then
                            corrupted.Add("ไฟล์ใบเสร็จมีขนาด 0 ไบต์: " & rel)
                            e.Problems.Add("ไฟล์ใบเสร็จมีขนาด 0 ไบต์: " & rel)
                        End If
                        e.BackupReceiptCount += 1
                        e.Files.Add(New BackupFileEntry() With {.RelativePath = rel, .SizeBytes = size, .Sha256 = h})
                    Next
                Catch ex As Exception
                    e.Problems.Add("อ่านโฟลเดอร์ Receipts ไม่ได้: " & ex.Message)
                End Try
            End If

            ' 4.2 เทียบไฟล์ใบเสร็จปัจจุบันกับชุดสำรอง
            Dim backupNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            If e.ReceiptsFolderExists Then
                Try
                    For Each f As String In Directory.GetFiles(receiptsDir, "*", SearchOption.AllDirectories)
                        backupNames.Add(Path.GetFileName(f))
                    Next
                Catch
                End Try
            End If

            For Each cur In snapshot.Receipts
                If token.IsCancellationRequested Then Exit For
                Dim found As String = ResolveReceiptFile(backupFolder, e.ReceiptsFolderExists, cur.RelativePath)
                If String.IsNullOrEmpty(found) Then
                    e.ReceiptsMissing.Add(cur.RelativePath)
                    Continue For
                End If

                If String.IsNullOrEmpty(cur.Sha256) Then
                    ' ต้นฉบับอ่านไม่ได้ → เทียบไม่ได้ ถือว่ายังไม่ยืนยัน
                    e.ReceiptsMissing.Add(cur.RelativePath)
                    Continue For
                End If

                Try
                    Dim h As String
                    If hashCache.ContainsKey(found) Then
                        h = hashCache(found)
                    Else
                        h = BackupHashing.ComputeSha256File(found)
                        hashCache(found) = h
                    End If
                    If String.Equals(h, cur.Sha256, StringComparison.OrdinalIgnoreCase) Then
                        e.ReceiptsOk += 1
                    Else
                        e.ReceiptsChanged.Add(cur.RelativePath)
                    End If
                Catch ex As Exception
                    corrupted.Add("อ่านไฟล์ใบเสร็จในชุดสำรองไม่ได้: " & cur.RelativePath & " (" & ex.Message & ")")
                    e.Problems.Add("อ่านไฟล์ใบเสร็จในชุดสำรองไม่ได้: " & cur.RelativePath & " (" & ex.Message & ")")
                    e.ReceiptsMissing.Add(cur.RelativePath)
                End Try
            Next

            ' นับไฟล์ในชุดสำรองที่ไม่มีในต้นฉบับปัจจุบัน
            Dim currentNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each cur In snapshot.Receipts
                currentNames.Add(Path.GetFileName(cur.RelativePath))
            Next
            For Each n In backupNames
                If Not currentNames.Contains(n) Then e.ReceiptsExtra += 1
            Next
        End Sub

        ''' <summary>สรุปหมวดรวมจากข้อมูลที่เก็บไว้</summary>
        Private Sub FinalizeClassification(e As LegacyEvaluation, corrupted As List(Of String))
            If corrupted.Count > 0 Then
                e.Classification = BackupStatusClass.Corrupted
                e.StatusText = StatusTextFor(BackupStatusClass.Corrupted)
                e.Reason = corrupted(0)
                e.IsComplete = False
                Return
            End If

            Dim reasons As New List(Of String)()
            If Not e.DatabaseExists Then reasons.Add("ไม่พบฐานข้อมูล (.accdb)")
            If Not e.ReceiptsFolderExists AndAlso e.CurrentReceiptCount > 0 Then reasons.Add("ไม่มีโฟลเดอร์ Receipts")
            If e.ReceiptsMissing.Count > 0 Then
                reasons.Add("ใบเสร็จไม่ครบ (" & e.ReceiptsOk.ToString() & "/" & e.CurrentReceiptCount.ToString() & ")")
            End If

            If reasons.Count > 0 Then
                e.Classification = BackupStatusClass.Incomplete
                e.StatusText = StatusTextFor(BackupStatusClass.Incomplete)
                e.Reason = String.Join(" · ", reasons)
                For Each r In reasons
                    e.Problems.Add(r)
                Next
                If e.ReceiptsMissing.Count > 0 Then
                    For Each m In e.ReceiptsMissing
                        e.Problems.Add("ไม่พบไฟล์ใบเสร็จ: " & m)
                    Next
                End If
                e.IsComplete = False
                Return
            End If

            Dim dbSame As Boolean = e.DatabaseMatchesCurrent
            If dbSame AndAlso e.ReceiptsChanged.Count = 0 Then
                e.Classification = BackupStatusClass.Identical
                e.StatusText = StatusTextFor(BackupStatusClass.Identical)
                e.Reason = ""
                e.IsComplete = True
                Return
            End If

            e.Classification = BackupStatusClass.OlderButIntact
            e.StatusText = StatusTextFor(BackupStatusClass.OlderButIntact)
            Dim oldReasons As New List(Of String)()
            If e.DatabaseExists AndAlso Not dbSame Then oldReasons.Add("ฐานข้อมูลถูกแก้ไขหลังการสำรอง (hash ไม่ตรงกับปัจจุบัน)")
            If e.ReceiptsChanged.Count > 0 Then oldReasons.Add("ใบเสร็จเปลี่ยน " & e.ReceiptsChanged.Count.ToString() & " ไฟล์")
            e.Reason = String.Join(" · ", oldReasons)
            For Each r In oldReasons
                e.Problems.Add(r)
            Next
            e.IsComplete = True
        End Sub

        Public Function StatusTextFor(cls As BackupStatusClass) As String
            Select Case cls
                Case BackupStatusClass.Identical : Return "ตรงกับปัจจุบัน"
                Case BackupStatusClass.OlderButIntact : Return "เก่ากว่าแต่สมบูรณ์"
                Case BackupStatusClass.Incomplete : Return "ไม่สมบูรณ์"
                Case BackupStatusClass.Corrupted : Return "เสียหาย"
                Case BackupStatusClass.DriveMissing : Return "ไม่พบไดรฟ์"
                Case Else : Return "-"
            End Select
        End Function

        ''' <summary>หาไฟล์ฐานข้อมูล (.accdb) ระดับบนสุดของโฟลเดอร์สำรอง (ข้ามไฟล์ lock ชั่วคราว)</summary>
        Public Function FindDatabaseFile(backupFolder As String) As String
            Try
                If Not Directory.Exists(backupFolder) Then Return ""
                Dim candidates As New List(Of String)()
                For Each f As String In Directory.GetFiles(backupFolder, "*.accdb")
                    Dim n As String = Path.GetFileName(f)
                    If n.StartsWith("~$", StringComparison.Ordinal) Then Continue For
                    If n.EndsWith(".laccdb", StringComparison.OrdinalIgnoreCase) Then Continue For
                    candidates.Add(f)
                Next
                If candidates.Count = 0 Then Return ""
                candidates.Sort(StringComparer.OrdinalIgnoreCase)
                ' ถ้ามีหลายไฟล์ ให้เลือกไฟล์ที่ชื่อตรงกับต้นฉบับก่อน
                Return candidates(0)
            Catch
                Return ""
            End Try
        End Function

        ''' <summary>หาตำแหน่งไฟล์ใบเสร็จในชุดสำรอง (รองรับทั้ง Receipts\... และไฟล์วางราบในโฟลเดอร์)</summary>
        Private Function ResolveReceiptFile(backupFolder As String, receiptsFolderExists As Boolean, relativePath As String) As String
            Try
                Dim direct As String = Path.Combine(backupFolder, relativePath)
                If File.Exists(direct) Then Return direct
                Dim name As String = Path.GetFileName(relativePath)
                If receiptsFolderExists Then
                    Dim inReceipts As String = Path.Combine(backupFolder, "Receipts", name)
                    If File.Exists(inReceipts) Then Return inReceipts
                End If
                Dim flat As String = Path.Combine(backupFolder, name)
                If File.Exists(flat) Then Return flat
            Catch
            End Try
            Return ""
        End Function

        ''' <summary>
        ''' อ่านเวลาของชุดสำรองจากชื่อโฟลเดอร์ Backup_yyyyMMdd_HHmmss
        ''' รองรับปีพุทธศักราช (เช่น 25690804 → 2026-08-04) คืน Nothing ถ้าอ่านไม่ได้
        ''' </summary>
        Public Function ParseFolderTimestamp(folder As String) As Date?
            Try
                Dim name As String = Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                If String.IsNullOrEmpty(name) Then Return Nothing
                If Not name.StartsWith("Backup_", StringComparison.OrdinalIgnoreCase) Then Return Nothing
                Dim rest As String = name.Substring("Backup_".Length)
                Dim parts As String() = rest.Split("_"c)
                If parts.Length < 2 Then Return Nothing
                If parts(0).Length <> 8 OrElse parts(1).Length <> 6 Then Return Nothing

                Dim y As Integer, mo As Integer, d As Integer
                Dim hh As Integer, mi As Integer, ss As Integer
                If Not Integer.TryParse(parts(0).Substring(0, 4), y) Then Return Nothing
                If Not Integer.TryParse(parts(0).Substring(4, 2), mo) Then Return Nothing
                If Not Integer.TryParse(parts(0).Substring(6, 2), d) Then Return Nothing
                If Not Integer.TryParse(parts(1).Substring(0, 2), hh) Then Return Nothing
                If Not Integer.TryParse(parts(1).Substring(2, 2), mi) Then Return Nothing
                If Not Integer.TryParse(parts(1).Substring(4, 2), ss) Then Return Nothing
                If y > 2400 Then y -= 543
                If y < 1900 OrElse y > 2200 Then Return Nothing
                Return New Date(y, mo, d, hh, mi, ss)
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>สร้าง registry entry จากผลประเมิน (manifest ที่คำนวณได้เก็บใน registry เท่านั้น)</summary>
        Public Function BuildRegistryEntry(evaluation As LegacyEvaluation) As BackupRegistryEntry
            Dim entry As New BackupRegistryEntry() With {
                .DestinationPath = evaluation.BackupFolder,
                .DriveLetter = If(evaluation.DriveRoot, "").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                .VolumeLabel = TryVolumeLabel(evaluation.DriveRoot),
                .IsLegacy = evaluation.IsLegacy,
                .IsComplete = evaluation.IsComplete,
                .Status = evaluation.StatusText,
                .FolderTimestamp = If(evaluation.FolderTimestamp, ""),
                .Files = If(evaluation.Files, New List(Of BackupFileEntry)()),
                .Problems = If(evaluation.Problems, New List(Of String)())
            }
            If Not String.IsNullOrEmpty(evaluation.FolderTimestamp) Then
                entry.Timestamp = evaluation.FolderTimestamp
            Else
                entry.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            End If
            entry.VerificationResult = evaluation.StatusText &
                If(evaluation.IsLegacy, " (legacy — ประเมินจากการสแกนโฟลเดอร์)", " (เพิ่มจากการสแกนไดรฟ์)")
            Return entry
        End Function

        ''' <summary>
        ''' เพิ่มชุดสำรองเข้าสู่ registry (ถ้ายังไม่มี) — คืน True เมื่อเพิ่มใหม่, False เมื่อมีอยู่แล้ว/ไม่สำเร็จ
        ''' กุญแจกันซ้ำ = path เต็มของโฟลเดอร์ + เวลาของโฟลเดอร์ (ดู BackupRegistry.FindEntryForFolder)
        ''' </summary>
        Public Function RegisterBackup(registryPath As String, evaluation As LegacyEvaluation) As Boolean
            If evaluation Is Nothing OrElse String.IsNullOrEmpty(evaluation.BackupFolder) Then Return False
            Try
                Dim doc As RegistryDocument = BackupRegistry.Load(registryPath)
                If BackupRegistry.IsRegistered(doc, evaluation.BackupFolder) Then Return False
                doc.Entries.Add(BuildRegistryEntry(evaluation))
                BackupRegistry.Save(registryPath, doc)
                Return True
            Catch
                Return False
            End Try
        End Function

        ''' <summary>เพิ่ม entry ที่สร้างไว้แล้วลงเอกสาร registry (ใช้เมื่อต้องการบันทึกครั้งเดียวทั้งชุด)</summary>
        Public Function AddEvaluationToDocument(doc As RegistryDocument, evaluation As LegacyEvaluation) As Boolean
            If doc Is Nothing OrElse evaluation Is Nothing Then Return False
            If String.IsNullOrEmpty(evaluation.BackupFolder) Then Return False
            If BackupRegistry.IsRegistered(doc, evaluation.BackupFolder) Then Return False
            doc.Entries.Add(BuildRegistryEntry(evaluation))
            Return True
        End Function

        Private Function TryVolumeLabel(driveRoot As String) As String
            Try
                If String.IsNullOrEmpty(driveRoot) Then Return ""
                Dim di As New DriveInfo(driveRoot)
                Return di.VolumeLabel
            Catch
                Return ""
            End Try
        End Function

        Private Function SafeFileSize(path As String) As Long
            Try
                Return New FileInfo(path).Length
            Catch
                Return 0
            End Try
        End Function

    End Module

End Namespace
