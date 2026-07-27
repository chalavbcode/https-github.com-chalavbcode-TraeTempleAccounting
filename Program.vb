Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Module Program
    Private _logPath As String = ""

    Private Sub C(msg As String)
        Try
            Console.WriteLine("[" & DateTime.Now.ToString("HH:mm:ss.fff") & "]  " & msg)
        Catch
        End Try
    End Sub

    Private Sub BootLog(msg As String)
        Try
            C(msg)
            If String.IsNullOrEmpty(_logPath) Then
                _logPath = Path.Combine(TempleAccounting.AppPaths.LogsFolder, "crash.log")
            End If
            File.AppendAllText(_logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & "  [BOOT]  " & msg & Environment.NewLine, Encoding.UTF8)
        Catch
        End Try
    End Sub

    Private Sub BootCrash(ex As Exception, place As String)
        Try
            BootLog("!! EXCEPTION at " & place & "  ::  " & ex.GetType().Name & "  " & ex.Message)
            If ex.InnerException IsNot Nothing Then
                BootLog("    INNER: " & ex.InnerException.GetType().Name & "  " & ex.InnerException.Message)
            End If
            BootLog("STACK: " & ex.StackTrace)
            Dim sb As New StringBuilder()
            sb.AppendLine()
            sb.AppendLine("========== UNHANDLED BOOT EXCEPTION @ " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " ==========")
            sb.AppendLine("📍 Location: " & place)
            sb.AppendLine("Message: " & ex.Message)
            sb.AppendLine("Type: " & ex.GetType().FullName)
            If ex.InnerException IsNot Nothing Then
                sb.AppendLine("Inner: " & ex.InnerException.Message)
            End If
            sb.AppendLine("Stack: ")
            sb.AppendLine(ex.StackTrace)
            File.AppendAllText(_logPath, sb.ToString(), Encoding.UTF8)
        Catch
        End Try
    End Sub

    <STAThread>
    Public Sub Main()
        Try
            AddHandler AppDomain.CurrentDomain.UnhandledException,
                Sub(s, e)
                    Try
                        Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
                        If ex Is Nothing Then
                            BootLog("AppDomain Unhandled NON-Exception Object terminating=" & e.IsTerminating.ToString() & "  -> " & If(e.ExceptionObject Is Nothing, "null", e.ExceptionObject.ToString()))
                        Else
                            BootCrash(ex, "AppDomain.UnhandledException terminating=" & e.IsTerminating.ToString())
                            MessageBox.Show("⛔ โปรแกรมพบข้อผิดพลาดร้ายแรง (ข้อมูลถูกบันทึกลง Crash Log): " & vbCrLf & vbCrLf &
                                            ex.Message & vbCrLf & vbCrLf &
                                            "📄 Log อยู่ที่: " & _logPath & vbCrLf &
                                            "🙏 กรุณาส่งไฟล์นี้มาทางผู้พัฒนาเพื่อแก้ไขต่อ",
                                            "TempleAccounting - หยุดทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        End If
                    Catch
                    End Try
                End Sub
            AddHandler Application.ThreadException,
                Sub(s, e)
                    Try
                        BootCrash(e.Exception, "Application.ThreadException")
                        MessageBox.Show("⚠️ พบข้อผิดพลาดในหน้าจอ (ข้อมูลถูกบันทึกแล้ว): " & vbCrLf &
                                        e.Exception.Message & vbCrLf & vbCrLf &
                                        "📄 Log อยู่ที่: " & _logPath,
                                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Catch
                    End Try
                End Sub
        Catch ex As Exception
            BootCrash(ex, "Step 1 - Exception handlers")
            MessageBox.Show("ไม่สามารถติดตั้งตัวจับข้อผิดพลาดได้: " & ex.Message, "Startup Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        Try
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Catch ex As Exception
            BootCrash(ex, "Step 2 - WinForms init")
            MessageBox.Show("❌ Beacon 3 FAILED: ไม่สามารถเปิด WinForms subsystem ได้:" & vbCrLf & ex.Message,
                            "Beacon 3 FAILED", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End Try

        Try
            Dim ignore = TempleAccounting.AppPaths.DatabaseFile
            If Not File.Exists(TempleAccounting.AppPaths.DatabaseFile) Then
                BootLog("!! DATABASE FILE NOT FOUND :: " & TempleAccounting.AppPaths.DatabaseFile)
            End If
        Catch ex As Exception
            BootCrash(ex, "Step 3 - AppPaths init")
            MessageBox.Show("❌ Beacon 4 FAILED (AppPaths):" & vbCrLf & ex.Message, "Beacon 4 FAILED", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        Try
            Try
                TempleAccounting.Db.EnsureSchema()
            Catch ex As Exception
                BootCrash(ex, "Step 4 - Db.EnsureSchema")
                MessageBox.Show("⚠️  Beacon 5: Database Schema ไม่สามารถเตรียมได้ (จะเปิด UI หลักได้ยัง) เหตุผล:" & vbCrLf &
                                ex.Message & vbCrLf & vbCrLf &
                                "สาเหตุที่พบบ่อยที่สุด = ไม่ได้ติดตั้ง Microsoft Access Database Engine 2016 Redistributable (ไม่ตรง 32/64-bit)" & vbCrLf &
                                "ดาวน์โหลด: https://www.microsoft.com/en-us/download/details.aspx?id=54920" & vbCrLf & vbCrLf &
                                "กด OK เพื่อเปิด UI หลักต่อ...",
                                "Beacon 5/6 - Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            BootCrash(ex, "Step 4 outer")
        End Try

        Dim mainForm As TempleAccounting.frmMain = Nothing
        Try
            mainForm = New TempleAccounting.frmMain()
            Try
                mainForm.StartPosition = FormStartPosition.CenterScreen
                mainForm.WindowState = FormWindowState.Maximized
            Catch
            End Try
        Catch ex As Exception
            BootCrash(ex, "Step 5 - frmMain constructor")
            MessageBox.Show("❌ Beacon 6 FAILED (สร้าง frmMain ไม่ได้):" & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                            "Crash Log: " & _logPath,
                            "Beacon 6 FAILED", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Environment.ExitCode = 1
            Return
        End Try

        Try
            Application.Run(mainForm)
        Catch ex As Exception
            BootCrash(ex, "Step 6 - Application.Run(frmMain)")
            MessageBox.Show("⛔ ไม่สามารถเปิดหน้าจอหลักได้ (Application.Run FAIL): " & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                            "📄 ดูรายละเอียดทั้งหมดที่: " & _logPath,
                            "TempleAccounting - หยุดทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Environment.ExitCode = 1
            Return
        End Try
    End Sub
End Module
