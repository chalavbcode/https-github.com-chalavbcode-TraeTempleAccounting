Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Module Program
    Private _logPath As String = ""

    <System.Runtime.InteropServices.DllImport("shell32.dll", SetLastError:=True)>
    Private Function SetCurrentProcessExplicitAppUserModelID(<System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)> appId As String) As Integer
    End Function

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
                                            "ระบบบัญชีวัด - หยุดทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Stop)
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
                                        "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Catch
                    End Try
                End Sub
        Catch ex As Exception
            BootCrash(ex, "Step 1 - Exception handlers")
            MessageBox.Show("ไม่สามารถติดตั้งตัวจับข้อผิดพลาดได้: " & ex.Message, "เปิดโปรแกรมไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        Try
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
        Catch ex As Exception
            BootCrash(ex, "Step 2 - WinForms init")
            MessageBox.Show("❌ ขั้นตอนที่ 3 ล้มเหลว: ไม่สามารถเปิดระบบหน้าต่างของโปรแกรมได้:" & vbCrLf & ex.Message,
                            "เปิดระบบหน้าต่างไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End Try

        Try
            ' ตั้ง AppUserModelID ที่ไม่ซ้ำกัน เพื่อให้ Windows จัดกลุ่มไอคอน Taskbar ตามแอปนี้
            ' และไม่ไปรวมกับ Shortcut เก่าที่ถูกแคชไว้
            SetCurrentProcessExplicitAppUserModelID("LamyangIE.TempleAccounting.App.v1")
            BootLog("AppUserModelID set: LamyangIE.TempleAccounting.App.v1")
        Catch ex As Exception
            BootCrash(ex, "Step 2b - AppUserModelID")
        End Try

        Try
            Dim ignore = TempleAccounting.AppPaths.DatabaseFile
            If Not File.Exists(TempleAccounting.AppPaths.DatabaseFile) Then
                BootLog("!! DATABASE FILE NOT FOUND :: " & TempleAccounting.AppPaths.DatabaseFile)
            End If
        Catch ex As Exception
            BootCrash(ex, "Step 3 - AppPaths init")
            MessageBox.Show("❌ ขั้นตอนที่ 4 ล้มเหลว (กำหนดตำแหน่งไฟล์):" & vbCrLf & ex.Message, "กำหนดตำแหน่งไฟล์ไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        Try
            Try
                TempleAccounting.Db.EnsureSchema()
            Catch ex As Exception
                BootCrash(ex, "Step 4 - Db.EnsureSchema")
                MessageBox.Show("⚠️  ขั้นตอนที่ 5: โครงสร้างฐานข้อมูลไม่สามารถเตรียมได้ (จะยังเปิดหน้าจอหลักไม่ได้) เหตุผล:" & vbCrLf &
                                ex.Message & vbCrLf & vbCrLf &
                                "สาเหตุที่พบบ่อยที่สุด = ไม่ได้ติดตั้ง Microsoft Access Database Engine 2016 Redistributable (ไม่ตรง 32/64-bit)" & vbCrLf &
                                "ดาวน์โหลด: https://www.microsoft.com/en-us/download/details.aspx?id=54920" & vbCrLf & vbCrLf &
                                "กด OK เพื่อเปิดหน้าจอหลักต่อ...",
                                "โครงสร้างฐานข้อมูลไม่พร้อม", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        Catch ex As Exception
            BootCrash(ex, "Step 4 outer")
        End Try

        ' --- Initialize Saddhiviharika SQLite database (ordination registry) ---
        Try
            TempleAccounting.SaddhiviharikaDb.EnsureSchema()
            BootLog("SaddhiviharikaDb initialized successfully")
        Catch ex As Exception
            BootCrash(ex, "Step 4c - SaddhiviharikaDb.EnsureSchema")
        End Try

        ' --- Migrate existing receipt images to permanent master directory (survives VS Rebuild) ---
        Try
            TempleAccounting.AppPaths.MigrateReceiptsToMaster()
        Catch ex As Exception
            BootCrash(ex, "Step 4b - MigrateReceiptsToMaster")
        End Try

        Try
            If Not TempleAccounting.AppPreferences.IsOnboardingCompleted Then
                Using wizard As New TempleAccounting.FrmOnboardingWizard()
                    If wizard.ShowDialog() <> DialogResult.OK Then Return
                End Using
            End If
        Catch ex As Exception
            BootCrash(ex, "Step 4d - OnboardingWizard")
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
            MessageBox.Show("❌ ขั้นตอนที่ 6 ล้มเหลว (สร้างหน้าจอหลักไม่ได้):" & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                            "Crash Log: " & _logPath,
                            "สร้างหน้าจอหลักไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Environment.ExitCode = 1
            Return
        End Try

        Try
            Application.Run(mainForm)
        Catch ex As Exception
            BootCrash(ex, "Step 6 - Application.Run(frmMain)")
            MessageBox.Show("⛔ ไม่สามารถเปิดหน้าจอหลักได้: " & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                            "📄 ดูรายละเอียดทั้งหมดที่: " & _logPath,
                            "ระบบบัญชีวัด - หยุดทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Environment.ExitCode = 1
            Return
        End Try
    End Sub
End Module
