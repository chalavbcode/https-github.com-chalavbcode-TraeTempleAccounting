Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Data.OleDb
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Class DatabaseBackupHelper
        ''' <summary>
        ''' ทำการสำรองฐานข้อมูลไปยังโฟลเดอร์ที่กำหนด
        ''' </summary>
        ''' <param name="targetDirectory">โฟลเดอร์ปลายทางที่ต้องการเก็บไฟล์สำรอง</param>
        ''' <param name="isAuto">เป็นการสำรองข้อมูลอัตโนมัติหรือไม่ (ถ้าใช่จะไม่แสดง MessageBox สำเร็จ)</param>
        ''' <returns>True หากสำรองข้อมูลสำเร็จ</returns>
        Public Shared Function BackupDatabase(Optional targetDirectory As String = "", Optional isAuto As Boolean = False) As Boolean
            Try
                ' 1. เตรียมเส้นทาง
                Dim sourceFile As String = AppPaths.DatabaseFile
                If Not File.Exists(sourceFile) Then
                    If Not isAuto Then MessageBoxHelper.ShowError("ไม่พบไฟล์ฐานข้อมูลต้นทาง: " & sourceFile)
                    Return False
                End If

                ' ถ้าไม่ได้ระบุ targetDirectory ให้ใช้ค่าเริ่มต้นจาก AppPaths.BackupFolder
                If String.IsNullOrEmpty(targetDirectory) Then
                    targetDirectory = AppPaths.BackupFolder
                End If

                ' ตรวจสอบและสร้างโฟลเดอร์ปลายทาง
                If Not Directory.Exists(targetDirectory) Then
                    Directory.CreateDirectory(targetDirectory)
                End If

                ' 2. จัดการ Connection: พยายามคัดลอกไฟล์
                ' ใน .NET Core/10.0 OleDb อาจไม่รองรับ ClearAllPools() 
                ' เราจะพึ่งพาการปิด Connection ด้วย Using ในส่วนอื่นๆ ของโปรแกรม
                
                ' 3. ตั้งชื่อไฟล์สำรองพร้อม Timestamp
                ' รูปแบบ: TempleDB_Backup_YYYYMMDD_HHMMSS.accdb
                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
                Dim fileName As String = $"TempleDB_Backup_{timestamp}.accdb"
                Dim destFile As String = Path.Combine(targetDirectory, fileName)

                ' 4. ทำการคัดลอกไฟล์
                File.Copy(sourceFile, destFile, True)

                ' 5. แจ้งเตือน (เฉพาะ Manual Backup)
                If Not isAuto Then
                    MessageBoxHelper.ShowInfo($"สำรองฐานข้อมูลสำเร็จ!{Environment.NewLine}ตำแหน่งไฟล์: {destFile}")
                End If

                Return True
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DatabaseBackupHelper.BackupDatabase")
                If Not isAuto Then
                    MessageBoxHelper.ShowError("เกิดข้อผิดพลาดในการสำรองข้อมูล: " & ex.Message)
                End If
                Return False
            End Try
        End Function
    End Class
End Namespace
