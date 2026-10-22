Option Strict Off
Option Explicit On

Imports System.IO
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' สร้างไอคอนลัด (Desktop Shortcut) ชี้ไปที่ TempleAccounting.exe
    ''' ใช้ WScript.Shell (COM) — ไม่ต้องเพิ่ม reference เพิ่มเติม เพราะโปรเจกต์เป็น Option Strict Off
    ''' </summary>
    Public Module DesktopShortcutHelper
        ''' <summary>ชื่อไฟล์ Shortcut บน Desktop</summary>
        Public ReadOnly Property ShortcutName As String
            Get
                Return "ระบบบัญชีวัด.lnk"
            End Get
        End Property

        ''' <summary>
        ''' สร้าง Shortcut บน Desktop (ถ้ายังไม่มี) แบบเงียบ ๆ ใช้ตอนเปิดโปรแกรมครั้งแรก
        ''' </summary>
        Public Function CreateDesktopShortcutIfMissing() As Boolean
            If DesktopShortcutExists() Then Return True
            Return CreateDesktopShortcut(showMessage:=False)
        End Function

        ''' <summary>
        ''' ตรวจสอบว่า Shortcut อยู่บน Desktop แล้วหรือยัง
        ''' </summary>
        Public Function DesktopShortcutExists() As Boolean
            Try
                Dim desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                Return File.Exists(Path.Combine(desktop, ShortcutName))
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' สร้าง Desktop Shortcut (.lnk) ชี้ไปที่ไฟล์ TempleAccounting.exe ปัจจุบัน
        ''' </summary>
        ''' <param name="showMessage">True = แสดง MessageBox ผลลัพธ์, False = เงียบ</param>
        Public Function CreateDesktopShortcut(Optional showMessage As Boolean = True) As Boolean
            Try
                Dim exePath = Application.ExecutablePath
                If String.IsNullOrEmpty(exePath) OrElse Not File.Exists(exePath) Then
                    exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lamYangI&E.exe")
                End If
                If Not File.Exists(exePath) Then
                    Throw New FileNotFoundException("ไม่พบไฟล์ TempleAccounting.exe สำหรับสร้าง Shortcut")
                End If

                Dim desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                Dim lnkPath = Path.Combine(desktop, ShortcutName)

                ' สร้าง .lnk ด้วย WScript.Shell (COM — ใช้ได้บน Windows ทุกเครื่อง)
                Dim shell = CreateObject("WScript.Shell")
                Dim lnk = shell.CreateShortcut(lnkPath)
                lnk.TargetPath = exePath
                lnk.WorkingDirectory = Path.GetDirectoryName(exePath)
                lnk.IconLocation = exePath & ", 0"
                lnk.Description = "ระบบบัญชีวัดแหลมยาง (Temple Accounting System)"
                lnk.Save()

                If showMessage Then
                    MessageBox.Show("สร้างไอคอนลัด (Shortcut) ลงบน Desktop เรียบร้อยแล้ว" & vbCrLf & vbCrLf &
                                    "ชื่อ: " & ShortcutName, "สำเร็จ",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                Return True
            Catch ex As Exception
                AppPaths.LogCrash(ex, "DesktopShortcutHelper.CreateDesktopShortcut")
                If showMessage Then
                    MessageBox.Show("ไม่สามารถสร้างไอคอนลัดได้: " & ex.Message, "ผิดพลาด",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                Return False
            End Try
        End Function
    End Module
End Namespace
