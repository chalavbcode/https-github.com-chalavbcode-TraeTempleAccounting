Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.Encodings.Web
Imports System.Collections.Generic

Namespace TempleAccounting

    ''' <summary>หนึ่งไฟล์ที่ถูกบันทึกไว้ใน registry/manifest (rel path + size + SHA-256)</summary>
    Public Class BackupFileEntry
        Public Property RelativePath As String
        Public Property SizeBytes As Long
        Public Property Sha256 As String
    End Class

    ''' <summary>หนึ่งรายการ backup ใน registry (backup_registry.json)</summary>
    Public Class BackupRegistryEntry
        Public Property Timestamp As String
        Public Property DestinationPath As String
        Public Property DriveLetter As String
        Public Property VolumeLabel As String
        Public Property VerificationResult As String
        Public Property Files As New List(Of BackupFileEntry)

        ' ── ข้อมูลที่เพิ่มสำหรับชุดสำรองแบบ legacy (ไม่มี backup_manifest.json) ──
        ''' <summary>True = ชุดสำรองรุ่นเก่าที่ไม่มี backup_manifest.json (ประเมินจากการสแกนโฟลเดอร์)</summary>
        Public Property IsLegacy As Boolean
        ''' <summary>True = ชุดสำรองครบถ้วน (ตรงกับปัจจุบัน หรือเก่ากว่าแต่สมบูรณ์) — ใช้ได้จริง</summary>
        Public Property IsComplete As Boolean
        ''' <summary>ข้อความสถานะล่าสุด เช่น "ตรงกับปัจจุบัน" / "ไม่สมบูรณ์"</summary>
        Public Property Status As String
        ''' <summary>เวลาของชุดสำรองจากชื่อโฟลเดอร์ (yyyy-MM-dd HH:mm:ss) ใช้เป็นกุญแจกันซ้ำ</summary>
        Public Property FolderTimestamp As String
        ''' <summary>เหตุผล/ปัญหาโดยละเอียดของชุดสำรองนี้</summary>
        Public Property Problems As New List(Of String)
    End Class

    ''' <summary>เอกสารรวม registry</summary>
    Public Class RegistryDocument
        Public Property ProjectId As String
        Public Property Entries As New List(Of BackupRegistryEntry)
    End Class

    ''' <summary>
    ''' จัดเก็บประวัติการสำรองข้อมูลลง backup_registry.json และ mirror backup_manifest.json
    ''' เป็นโค้ดล้วน (pure) ไม่ผูกกับ AppPaths/WinForms จึงทดสอบอัตโนมัติได้
    ''' โดยส่ง path ของ registry เข้ามาเป็นพารามิเตอร์เสมอ
    ''' </summary>
    Public Module BackupRegistry

        ''' <summary>รหัสโปรเจกต์นี้ — ใช้ยืนยัน backup_manifest.json ว่าเป็นของระบบนี้</summary>
        Public Const ProjectId As String = "TempleAccounting"

        Private Function SerializerOptions() As JsonSerializerOptions
            Return New JsonSerializerOptions() With {
                .WriteIndented = True,
                .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        End Function

        ''' <summary>โหลด registry จาก path (ถ้าไม่มีหรือไฟล์เสีย ให้คืน document ใหม่)</summary>
        Public Function Load(registryPath As String) As RegistryDocument
            Try
                If File.Exists(registryPath) Then
                    Dim json As String = File.ReadAllText(registryPath)
                    Dim doc As RegistryDocument = JsonSerializer.Deserialize(Of RegistryDocument)(json)
                    If doc IsNot Nothing Then
                        If doc.Entries Is Nothing Then doc.Entries = New List(Of BackupRegistryEntry)()
                        If String.IsNullOrEmpty(doc.ProjectId) Then doc.ProjectId = ProjectId
                        Return doc
                    End If
                End If
            Catch
                ' ไฟล์เสีย → เริ่มใหม่ (ไม่ throw)
            End Try
            Return New RegistryDocument() With {.ProjectId = ProjectId}
        End Function

        ''' <summary>บันทึก registry ลง path</summary>
        Public Sub Save(registryPath As String, doc As RegistryDocument)
            If doc Is Nothing Then doc = New RegistryDocument() With {.ProjectId = ProjectId}
            If String.IsNullOrEmpty(doc.ProjectId) Then doc.ProjectId = ProjectId
            Dim dir As String = Path.GetDirectoryName(registryPath)
            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            File.WriteAllText(registryPath, JsonSerializer.Serialize(doc, SerializerOptions()), New UTF8Encoding(False))
        End Sub

        ''' <summary>เพิ่ม entry ใหม่แล้วบันทึก (เก็บประวัติไว้ ไม่ลบของเดิม)</summary>
        Public Sub AddEntry(registryPath As String, entry As BackupRegistryEntry)
            Dim doc As RegistryDocument = Load(registryPath)
            doc.Entries.Add(entry)
            Save(registryPath, doc)
        End Sub

        ''' <summary>คืน entry ล่าสุด n รายการ (เรียงใหม่ → เก่า)</summary>
        Public Function GetLast(registryPath As String, count As Integer) As List(Of BackupRegistryEntry)
            Dim doc As RegistryDocument = Load(registryPath)
            Dim result As New List(Of BackupRegistryEntry)()
            For i As Integer = doc.Entries.Count - 1 To 0 Step -1
                result.Add(doc.Entries(i))
                If result.Count >= count Then Exit For
            Next
            Return result
        End Function

        ''' <summary>คืน entry ทั้งหมด (เรียงใหม่ → เก่า)</summary>
        Public Function GetAll(registryPath As String) As List(Of BackupRegistryEntry)
            Return GetLast(registryPath, Integer.MaxValue)
        End Function

        ''' <summary>mirror backup_manifest.json ลงในโฟลเดอร์สำรองที่ entry ชี้ไป</summary>
        Public Sub WriteManifestMirror(entry As BackupRegistryEntry)
            If entry Is Nothing OrElse String.IsNullOrEmpty(entry.DestinationPath) Then Return
            Try
                Dim doc As New RegistryDocument() With {.ProjectId = ProjectId}
                doc.Entries.Add(entry)
                Dim manifestPath As String = Path.Combine(entry.DestinationPath, "backup_manifest.json")
                File.WriteAllText(manifestPath, JsonSerializer.Serialize(doc, SerializerOptions()), New UTF8Encoding(False))
            Catch
            End Try
        End Sub

        ''' <summary>อ่าน ProjectId จาก backup_manifest.json ในโฟลเดอร์ (ถ้ามี)</summary>
        Public Function ReadManifestProjectId(backupFolder As String) As String
            Try
                Dim manifestPath As String = Path.Combine(backupFolder, "backup_manifest.json")
                If File.Exists(manifestPath) Then
                    Dim doc As RegistryDocument = JsonSerializer.Deserialize(Of RegistryDocument)(File.ReadAllText(manifestPath))
                    If doc IsNot Nothing Then Return doc.ProjectId
                End If
            Catch
            End Try
            Return ""
        End Function

        ''' <summary>ปรับ path ให้เทียบกันได้ (ตัด separator ท้าย + ตัวพิมพ์เล็ก)</summary>
        Public Function NormalizeFolderPath(folder As String) As String
            If String.IsNullOrEmpty(folder) Then Return ""
            Return folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant()
        End Function

        ''' <summary>
        ''' ค้นหา entry ของชุดสำรองจาก path เต็ม — กุญแจกันซ้ำคือ "path เต็ม + เวลาของโฟลเดอร์"
        ''' (path เดียวกันย่อมเป็นชุดเดียวกัน จึงถือว่าซ้ำเสมอ แม้ FolderTimestamp จะว่าง)
        ''' </summary>
        Public Function FindEntryForFolder(doc As RegistryDocument, folder As String) As BackupRegistryEntry
            If doc Is Nothing OrElse doc.Entries Is Nothing OrElse String.IsNullOrEmpty(folder) Then Return Nothing
            Dim key As String = NormalizeFolderPath(folder)
            For Each e In doc.Entries
                If e Is Nothing OrElse String.IsNullOrEmpty(e.DestinationPath) Then Continue For
                If NormalizeFolderPath(e.DestinationPath) = key Then Return e
            Next
            Return Nothing
        End Function

        ''' <summary>ตรวจว่าชุดสำรองนี้ถูกเพิ่มเข้าสู่ registry แล้วหรือไม่</summary>
        Public Function IsRegistered(doc As RegistryDocument, folder As String) As Boolean
            Return FindEntryForFolder(doc, folder) IsNot Nothing
        End Function

        ''' <summary>คืน union ของ RelativePath ทั้งหมดที่เคยถูกสำรองใน entries (ใช้หาไฟล์ใหม่ที่ยังไม่ได้สำรอง)</summary>
        Public Function CollectBackedUpPaths(entries As IEnumerable(Of BackupRegistryEntry)) As HashSet(Of String)
            Dim set_ As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            If entries Is Nothing Then Return set_
            For Each e In entries
                If e Is Nothing OrElse e.Files Is Nothing Then Continue For
                For Each f In e.Files
                    If Not String.IsNullOrEmpty(f.RelativePath) Then set_.Add(f.RelativePath)
                Next
            Next
            Return set_
        End Function

    End Module

End Namespace
