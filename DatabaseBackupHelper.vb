Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Data.OleDb
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Class DatabaseBackupHelper
        ''' <summary>
        ''' ทำการสำรองฐานข้อมูลและโฟลเดอร์รูปภาพใบเสร็จ
        ''' </summary>
        ''' <param name="targetBaseDir">โฟลเดอร์ปลายทางหลัก (เช่น Flash Drive)</param>
        ''' <param name="isAuto">เป็นการสำรองข้อมูลอัตโนมัติหรือไม่</param>
        ''' <returns>True หากสำรองข้อมูลสำเร็จ</returns>
        Public Shared Function BackupFullSystem(Optional targetBaseDir As String = "", Optional isAuto As Boolean = False) As Boolean
            Try
                ' 1. เตรียมเส้นทางปลายทาง
                If String.IsNullOrEmpty(targetBaseDir) Then
                    targetBaseDir = AppPaths.BackupFolder
                End If

                ' สร้างโฟลเดอร์ Timestamp สำหรับชุดสำรองข้อมูลนี้
                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
                Dim backupSubFolderName As String = $"Backup_{timestamp}"
                Dim backupPath As String = Path.Combine(targetBaseDir, backupSubFolderName)

                If Not Directory.Exists(backupPath) Then
                    Directory.CreateDirectory(backupPath)
                End If

                ' 2. สำรองฐานข้อมูล (.accdb)
                Dim dbSource As String = AppPaths.DatabaseFile
                If File.Exists(dbSource) Then
                    ' ใน .NET 10.0 OleDbConnection.ClearAllPools() อาจไม่มี 
                    ' เราจะพึ่งพา GC เพื่อช่วยเคลียร์ Connection ที่ค้างอยู่ (Best Effort)
                    GC.Collect()
                    GC.WaitForPendingFinalizers()

                    Dim dbDestName As String = $"TempleDB_{timestamp}.accdb"
                    Dim dbDestPath As String = Path.Combine(backupPath, dbDestName)
                    File.Copy(dbSource, dbDestPath, True)
                Else
                    If Not isAuto Then MessageBoxHelper.ShowError("ไม่พบไฟล์ฐานข้อมูลต้นทาง")
                End If

                ' 3. สำรองโฟลเดอร์รูปใบเสร็จ (Receipts)
                Dim receiptsSource As String = AppPaths.ReceiptsDir
                If Directory.Exists(receiptsSource) Then
                    Dim receiptsDest As String = Path.Combine(backupPath, "Receipts")
                    CopyDirectory(receiptsSource, receiptsDest)
                End If

                ' 4. แจ้งเตือน (เฉพาะ Manual)
                If Not isAuto Then
                    MessageBoxHelper.ShowInfo($"สำรองข้อมูลครบถ้วนแล้ว!{Environment.NewLine}ตำแหน่ง: {backupPath}")
                End If

                Return True
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.BackupFullSystem")
                If Not isAuto Then
                    MessageBoxHelper.ShowError("เกิดข้อผิดพลาดในการสำรองข้อมูล: " & ex.Message)
                End If
                Return False
            End Try
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
                ' 1. ตรวจสอบเบื้องต้น
                If Not Directory.Exists(backupFolder) Then
                    MessageBoxHelper.ShowError("ไม่พบโฟลเดอร์สำรองข้อมูลที่เลือก")
                    Return False
                End If

                ' ตรวจสอบว่ามีไฟล์ฐานข้อมูลในโฟลเดอร์ backup หรือไม่ (รองรับทั้งชื่อตรงตัวและชื่อมี Timestamp)
                Dim dbBackupFile As String = ""
                Dim files = Directory.GetFiles(backupFolder, "*.accdb")
                If files.Length > 0 Then
                    dbBackupFile = files(0) ' ใช้ไฟล์แรกที่เจอ
                Else
                    MessageBoxHelper.ShowError("ไม่พบไฟล์ฐานข้อมูล (.accdb) ในโฟลเดอร์สำรองข้อมูลนี้")
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

                ' 5. ทำการ Restore รูปใบเสร็จ
                Dim receiptsBackupPath As String = Path.Combine(backupFolder, "Receipts")
                If Directory.Exists(receiptsBackupPath) Then
                    CopyDirectory(receiptsBackupPath, AppPaths.ReceiptsDir)
                End If

                ' ลบ Safety Backup เมื่อสำเร็จ
                Try
                    Directory.Delete(safetyPath, True)
                Catch
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
    End Class
End Namespace
