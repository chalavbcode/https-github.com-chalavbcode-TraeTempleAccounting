Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Security.Cryptography
Imports System.Collections.Generic
Imports System.Text.Json
Imports System.Text.Encodings.Web

Namespace TempleAccounting

    ''' <summary>
    ''' หนึ่งรายการที่ต้องสำรอง: เป็นไฟล์เดี่ยว (IsFolder=False) หรือโฟลเดอร์ทั้งโฟลเดอร์
    ''' รวมซับโฟลเดอร์ทั้งหมด (IsFolder=True)
    ''' </summary>
    Public Class BackupItemSpec
        Public Property SourcePath As String
        Public Property IsFolder As Boolean
        Public Property Description As String
    End Class

    ''' <summary>บันทึกผลของไฟล์หนึ่งไฟล์ใน manifest</summary>
    Public Class BackupFileRecord
        Public Property RelativePath As String
        Public Property SourcePath As String
        Public Property SizeBytes As Long
        Public Property Sha256 As String
        Public Property Status As String
        Public Property ErrorMessage As String
    End Class

    ''' <summary>ผลลัพธ์การสำรอง/ตรวจสอบข้อมูล</summary>
    Public Class BackupResult
        Public Property Success As Boolean
        Public Property DestinationFolder As String
        Public Property SourceFileCount As Integer
        Public Property CopiedFileCount As Integer
        Public Property VerifiedFileCount As Integer
        Public Property FailedFileCount As Integer
        Public Property Problems As New List(Of String)
        Public Property FileRecords As New List(Of BackupFileRecord)
        Public Property ManifestJsonPath As String
        Public Property ManifestCsvPath As String
        Public Property LogPath As String
    End Class

    ''' <summary>ไฟล์ที่วางแผนจะสำรอง (ต้นทาง + path สัมพัทธ์ภายใน backup folder)</summary>
    Friend Class PlannedFile
        Public Property SourcePath As String
        Public Property RelativePath As String
    End Class

    ''' <summary>เอกสาร manifest.json</summary>
    Public Class ManifestDocument
        Public Property Created As String
        Public Property SourceFileCount As Integer
        Public Property CopiedFileCount As Integer
        Public Property VerifiedFileCount As Integer
        Public Property FailedFileCount As Integer
        Public Property Verified As Boolean
        Public Property Files As List(Of BackupFileRecord)
    End Class

    ''' <summary>
    ''' กลไกสำรองข้อมูลแบบสมบูรณ์และตรวจสอบได้
    ''' • คัดลอกทุกรายการ (ไฟล์/โฟลเดอร์รวมซับโฟลเดอร์) ลงโฟลเดอร์ปลายทาง
    ''' • ตรวจสอบจำนวนไฟล์ ขนาด และ SHA-256 ทีละไฟล์
    ''' • เขียน manifest.json / manifest.csv และ backup.log
    ''' • retry เมื่อไฟล์ถูกล็อก พร้อมแจ้งปัญหา (ไม่ข้ามแบบเงียบ ๆ)
    ''' เป็นโค้ดล้วน (pure) ไม่ผูกกับ WinForms/AppPaths จึงทดสอบอัตโนมัติได้
    ''' </summary>
    Public Class BackupEngine

        Public Const MaxRetryAttempts As Integer = 3
        Public Const RetryDelayMs As Integer = 500

        ''' <summary>สำรองข้อมูลทุกรายการลง destinationFolder แล้วตรวจสอบความถูกต้อง</summary>
        Public Shared Function ExecuteBackup(items As List(Of BackupItemSpec), destinationFolder As String) As BackupResult
            Dim result As New BackupResult()
            result.DestinationFolder = destinationFolder
            Try
                If items Is Nothing OrElse items.Count = 0 Then
                    result.Problems.Add("ไม่มีรายการที่ต้องสำรอง (BACKUP_ITEMS ว่าง)")
                    result.Success = False
                    Return result
                End If

                If Not Directory.Exists(destinationFolder) Then Directory.CreateDirectory(destinationFolder)

                ' 1. ระบุไฟล์ทั้งหมดที่ต้องสำรอง
                Dim planned As List(Of PlannedFile) = BuildFilePlan(items, result)
                result.SourceFileCount = planned.Count

                ' 2. คัดลอกทีละไฟล์ (retry เมื่อไฟล์ถูกล็อก)
                For Each p In planned
                    Dim destPath As String = Path.Combine(destinationFolder, p.RelativePath)
                    Dim destDir As String = Path.GetDirectoryName(destPath)
                    If Not String.IsNullOrEmpty(destDir) AndAlso Not Directory.Exists(destDir) Then
                        Directory.CreateDirectory(destDir)
                    End If

                    If CopyWithRetry(p.SourcePath, destPath) Then
                        result.CopiedFileCount += 1
                    Else
                        result.FailedFileCount += 1
                        result.Problems.Add("คัดลอกไม่สำเร็จ (ไฟล์อาจถูกล็อก/ถูกใช้งานอยู่): " & p.SourcePath)
                    End If
                Next

                ' 3. ตรวจสอบความถูกต้อง (จำนวน/ขนาด/SHA-256) เทียบต้นทางกับปลายทาง
                VerifyFiles(planned, destinationFolder, result)

                ' 4. เขียน manifest + log
                WriteManifest(destinationFolder, result)
                result.LogPath = WriteLog(destinationFolder, items, result)

                result.Success = result.Problems.Count = 0
            Catch ex As Exception
                result.Success = False
                result.Problems.Add("เกิดข้อผิดพลาดระหว่างสำรองข้อมูล: " & ex.Message)
            End Try
            Return result
        End Function

        ''' <summary>ตรวจสอบ backup folder ที่มีอยู่แล้วเทียบกับรายการต้นทาง (ใช้ทดสอบ "ไฟล์หาย")</summary>
        Public Shared Function VerifyBackup(items As List(Of BackupItemSpec), backupFolder As String) As BackupResult
            Dim result As New BackupResult()
            result.DestinationFolder = backupFolder
            Try
                If Not Directory.Exists(backupFolder) Then
                    result.Problems.Add("ไม่พบโฟลเดอร์สำรองข้อมูล: " & backupFolder)
                    result.Success = False
                    Return result
                End If

                Dim planned As List(Of PlannedFile) = BuildFilePlan(items, result)
                result.SourceFileCount = planned.Count
                VerifyFiles(planned, backupFolder, result)
                result.Success = result.Problems.Count = 0
            Catch ex As Exception
                result.Success = False
                result.Problems.Add("เกิดข้อผิดพลาดระหว่างตรวจสอบ: " & ex.Message)
            End Try
            Return result
        End Function

        ''' <summary>ระบุรายชื่อไฟล์ทั้งหมดที่จะสำรองจาก BACKUP_ITEMS (รวมซับโฟลเดอร์)</summary>
        Private Shared Function BuildFilePlan(items As List(Of BackupItemSpec), result As BackupResult) As List(Of PlannedFile)
            Dim planned As New List(Of PlannedFile)()

            For Each item In items
                If String.IsNullOrEmpty(item.SourcePath) Then
                    result.Problems.Add("รายการสำรองไม่มีเส้นทางต้นทาง: " & If(item.Description, "(ไม่มีคำอธิบาย)"))
                    Continue For
                End If

                If item.IsFolder Then
                    If Not Directory.Exists(item.SourcePath) Then
                        result.Problems.Add("ไม่พบโฟลเดอร์ต้นทาง: " & item.SourcePath)
                        Continue For
                    End If

                    ' ใช้ชื่อโฟลเดอร์ต้นทางเป็นโฟลเดอร์ย่อยใน backup (เช่น "Receipts")
                    Dim folderName As String = Path.GetFileName(item.SourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))

                    ' ค้นหาไฟล์ทั้งหมดรวมซับโฟลเดอร์
                    Dim files As String() = Directory.GetFiles(item.SourcePath, "*", SearchOption.AllDirectories)
                    For Each f In files
                        Dim relWithin As String = Path.GetRelativePath(item.SourcePath, f)
                        Dim rel As String = Path.Combine(folderName, relWithin)
                        planned.Add(New PlannedFile() With {.SourcePath = f, .RelativePath = rel})
                    Next
                Else
                    If File.Exists(item.SourcePath) Then
                        Dim name As String = Path.GetFileName(item.SourcePath)
                        planned.Add(New PlannedFile() With {.SourcePath = item.SourcePath, .RelativePath = name})
                    Else
                        result.Problems.Add("ไม่พบไฟล์ต้นทาง: " & item.SourcePath)
                    End If
                End If
            Next

            Return planned
        End Function

        ''' <summary>ตรวจสอบแต่ละไฟล์: มีอยู่/ขนาด/SHA-256 ตรงกัน และเก็บผลลง manifest</summary>
        Private Shared Sub VerifyFiles(planned As List(Of PlannedFile), backupFolder As String, result As BackupResult)
            For Each p In planned
                Dim rec As New BackupFileRecord() With {
                    .RelativePath = p.RelativePath,
                    .SourcePath = p.SourcePath,
                    .Status = "FAILED"
                }

                Try
                    Dim srcSize As Long = New FileInfo(p.SourcePath).Length
                    rec.SizeBytes = srcSize
                    rec.Sha256 = ComputeSha256(p.SourcePath)

                    Dim destPath As String = Path.Combine(backupFolder, p.RelativePath)
                    If Not File.Exists(destPath) Then
                        rec.ErrorMessage = "ไม่พบไฟล์ในโฟลเดอร์สำรอง"
                        result.Problems.Add("ไม่พบไฟล์ในสำรอง: " & p.RelativePath)
                        result.FailedFileCount += 1
                    Else
                        Dim destSize As Long = New FileInfo(destPath).Length
                        If destSize <> srcSize Then
                            rec.ErrorMessage = "ขนาดไฟล์ไม่ตรงกัน"
                            result.Problems.Add("ขนาดไฟล์ไม่ตรงกัน: " & p.RelativePath)
                            result.FailedFileCount += 1
                        Else
                            Dim destHash As String = ComputeSha256(destPath)
                            If Not String.Equals(destHash, rec.Sha256, StringComparison.OrdinalIgnoreCase) Then
                                rec.ErrorMessage = "ค่า SHA-256 ไม่ตรงกัน"
                                result.Problems.Add("ค่า SHA-256 ไม่ตรงกัน: " & p.RelativePath)
                                result.FailedFileCount += 1
                            Else
                                rec.Status = "OK"
                                result.VerifiedFileCount += 1
                            End If
                        End If
                    End If
                Catch ex As Exception
                    rec.ErrorMessage = ex.Message
                    result.Problems.Add("ตรวจสอบไม่ได้: " & p.RelativePath & " (" & ex.Message & ")")
                    result.FailedFileCount += 1
                End Try

                result.FileRecords.Add(rec)
            Next
        End Sub

        ''' <summary>คัดลอกไฟล์พร้อม retry เมื่อไฟล์ถูกล็อก (เช่น .accdb ที่เปิดอยู่)</summary>
        Private Shared Function CopyWithRetry(srcPath As String, destPath As String) As Boolean
            For attempt As Integer = 1 To MaxRetryAttempts
                Try
                    File.Copy(srcPath, destPath, True)
                    Return True
                Catch ex As Exception
                    If attempt < MaxRetryAttempts Then
                        Thread.Sleep(RetryDelayMs)
                    End If
                End Try
            Next
            Return False
        End Function

        ''' <summary>คำนวณ SHA-256 ของไฟล์ (ยอมให้อ่านไฟล์ที่เปิดใช้งานอยู่ได้)</summary>
        Private Shared Function ComputeSha256(filePath As String) As String
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                Using sha As SHA256 = SHA256.Create()
                    Dim hashBytes As Byte() = sha.ComputeHash(fs)
                    Return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant()
                End Using
            End Using
        End Function

        Private Shared Sub WriteManifest(folder As String, result As BackupResult)
            Dim doc As New ManifestDocument() With {
                .Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                .SourceFileCount = result.SourceFileCount,
                .CopiedFileCount = result.CopiedFileCount,
                .VerifiedFileCount = result.VerifiedFileCount,
                .FailedFileCount = result.FailedFileCount,
                .Verified = result.Problems.Count = 0,
                .Files = result.FileRecords
            }

            Dim options As New JsonSerializerOptions() With {
                .WriteIndented = True,
                .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }

            result.ManifestJsonPath = Path.Combine(folder, "manifest.json")
            File.WriteAllText(result.ManifestJsonPath, JsonSerializer.Serialize(doc, options), New UTF8Encoding(False))

            result.ManifestCsvPath = Path.Combine(folder, "manifest.csv")
            Dim sb As New StringBuilder()
            sb.AppendLine("RelativePath,SourcePath,SizeBytes,SHA256,Status,ErrorMessage")
            For Each r In result.FileRecords
                sb.AppendLine(CsvField(r.RelativePath) & "," &
                              CsvField(r.SourcePath) & "," &
                              r.SizeBytes.ToString() & "," &
                              CsvField(r.Sha256) & "," &
                              CsvField(r.Status) & "," &
                              CsvField(r.ErrorMessage))
            Next
            File.WriteAllText(result.ManifestCsvPath, sb.ToString(), New UTF8Encoding(False))
        End Sub

        Private Shared Function WriteLog(folder As String, items As List(Of BackupItemSpec), result As BackupResult) As String
            Dim logPath As String = Path.Combine(folder, "backup.log")
            Dim sb As New StringBuilder()
            sb.AppendLine("===== BACKUP LOG =====")
            sb.AppendLine("วันที่/เวลา: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            sb.AppendLine("แหล่งที่มา (BACKUP_ITEMS):")
            For Each item In items
                sb.AppendLine("  - " & If(item.Description, "(ไม่มีคำอธิบาย)") & " : " & item.SourcePath)
            Next
            sb.AppendLine("ปลายทาง: " & folder)
            sb.AppendLine("จำนวนไฟล์ต้นทาง: " & result.SourceFileCount)
            sb.AppendLine("จำนวนไฟล์ที่คัดลอก: " & result.CopiedFileCount)
            sb.AppendLine("จำนวนไฟล์ที่ตรวจสอบผ่าน: " & result.VerifiedFileCount)
            sb.AppendLine("จำนวนไฟล์ที่ผิดพลาด: " & result.FailedFileCount)
            sb.AppendLine("ผลการตรวจสอบ: " & If(result.Problems.Count = 0, "ผ่าน (OK)", "ไม่ผ่าน (FAILED)"))
            If result.Problems.Count > 0 Then
                sb.AppendLine("ไฟล์/รายการที่มีปัญหา:")
                For Each p In result.Problems
                    sb.AppendLine("  - " & p)
                Next
            End If
            sb.AppendLine("===== END =====")
            File.WriteAllText(logPath, sb.ToString(), New UTF8Encoding(False))
            Return logPath
        End Function

        Private Shared Function CsvField(value As String) As String
            If value Is Nothing Then value = ""
            Return """" & value.Replace("""", """""") & """"
        End Function

    End Class
End Namespace
