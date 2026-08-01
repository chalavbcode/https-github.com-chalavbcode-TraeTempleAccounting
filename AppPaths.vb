Option Strict Off
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Module AppPaths
        Private _appRoot As String
        Private _databaseFile As String
        Private _importFolder As String
        Private _backupFolder As String
        Private _exportFolder As String
        Private _logsFolder As String
        Private _logPath As String

        Public ReadOnly Property LogsFolder As String
            Get
                If String.IsNullOrEmpty(_logsFolder) Then
                    _logsFolder = Path.Combine(AppRoot, "Logs")
                    If Not Directory.Exists(_logsFolder) Then Directory.CreateDirectory(_logsFolder)
                    _logPath = Path.Combine(_logsFolder, "crash.log")
                End If
                Return _logsFolder
            End Get
        End Property

        Public Sub LogStartup(msg As String)
            Try
                Dim ignore = LogsFolder
                File.AppendAllText(_logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "  [STARTUP]  " & msg & Environment.NewLine, Encoding.UTF8)
            Catch
            End Try
        End Sub

        Public Sub LogCrash(ex As Exception, Optional place As String = "")
            Try
                Dim ignore = LogsFolder
                Dim sb As New StringBuilder()
                sb.AppendLine()
                sb.AppendLine("========== UNHANDLED EXCEPTION @ " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " ==========")
                If Not String.IsNullOrWhiteSpace(place) Then sb.AppendLine("📍 Location: " & place)
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

        Public ReadOnly Property AppRoot As String
            Get
                If String.IsNullOrEmpty(_appRoot) Then
                    _appRoot = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    If Not Directory.Exists(_appRoot) Then Directory.CreateDirectory(_appRoot)
                End If
                Return _appRoot
            End Get
        End Property

        Public ReadOnly Property DatabaseFile As String
            Get
                If String.IsNullOrEmpty(_databaseFile) Then
                    Dim preferred = FindPreferredDatabaseFile(AppDomain.CurrentDomain.BaseDirectory)
                    If Not String.IsNullOrWhiteSpace(preferred) Then
                        _databaseFile = preferred
                    Else
                        Dim dbDir = Path.Combine(AppRoot, "Database")
                        If Not Directory.Exists(dbDir) Then Directory.CreateDirectory(dbDir)
                        _databaseFile = Path.Combine(dbDir, "TempleAccounting.accdb")
                    End If
                End If
                Return _databaseFile
            End Get
        End Property

        Private Function FindPreferredDatabaseFile(startDirectory As String) As String
            Try
                Dim dir = startDirectory
                If String.IsNullOrWhiteSpace(dir) Then Return ""

                Dim projectRoot As String = ""
                Dim lastDbFileFound As String = ""

                For i As Integer = 0 To 12
                    If File.Exists(Path.Combine(dir, "TempleAccounting.vbproj")) OrElse File.Exists(Path.Combine(dir, "TempleAccounting.slnx")) Then
                        projectRoot = dir
                        Exit For
                    End If

                    Dim dbFile = Path.Combine(dir, "Database", "TempleAccounting.accdb")
                    If File.Exists(dbFile) Then
                        lastDbFileFound = dbFile
                    End If

                    Dim parent = Directory.GetParent(dir)
                    If parent Is Nothing Then Exit For
                    dir = parent.FullName
                Next

                If projectRoot <> "" Then
                    Dim projectDb = Path.Combine(projectRoot, "Database", "TempleAccounting.accdb")
                    If File.Exists(projectDb) Then Return projectDb
                End If

                If lastDbFileFound <> "" Then Return lastDbFileFound

                Return ""
            Catch
                Return ""
            End Try
        End Function

        Public ReadOnly Property ImportFolder As String
            Get
                If String.IsNullOrEmpty(_importFolder) Then
                    _importFolder = Path.Combine(AppRoot, "Import")
                    If Not Directory.Exists(_importFolder) Then Directory.CreateDirectory(_importFolder)
                End If
                Return _importFolder
            End Get
        End Property

        Public ReadOnly Property BackupFolder As String
            Get
                If String.IsNullOrEmpty(_backupFolder) Then
                    _backupFolder = Path.Combine(AppRoot, "Backup")
                    If Not Directory.Exists(_backupFolder) Then Directory.CreateDirectory(_backupFolder)
                End If
                Return _backupFolder
            End Get
        End Property

        Public ReadOnly Property ExportFolder As String
            Get
                If String.IsNullOrEmpty(_exportFolder) Then
                    _exportFolder = Path.Combine(AppRoot, "Export")
                    If Not Directory.Exists(_exportFolder) Then Directory.CreateDirectory(_exportFolder)
                End If
                Return _exportFolder
            End Get
        End Property

        Public Function ProvinceCsv() As String
            Return Path.Combine(ImportFolder, "province.csv")
        End Function
        Public Function DistrictCsv() As String
            Return Path.Combine(ImportFolder, "amphoe.csv")
        End Function
        Public Function SubDistrictCsv() As String
            Return Path.Combine(ImportFolder, "tambon.csv")
        End Function

        ' ... โค้ดเดิมที่มีอยู่ใน AppPaths.vb ...

        ' --- 1. เพิ่ม Property สำหรับโฟลเดอร์ Receipts ---
        Public ReadOnly Property ReceiptsDir As String
                Get
                    Dim dir = IO.Path.Combine(BaseDir, "Receipts")
                    If Not IO.Directory.Exists(dir) Then IO.Directory.CreateDirectory(dir)
                    Return dir
                End Get
            End Property

            ' --- 2. เพิ่ม Method ตรวจสอบและสร้างโฟลเดอร์ทั้งหมด ---
            Public Sub EnsureDirectoriesExist()
                Try
                    If Not IO.Directory.Exists(DatabaseDir) Then IO.Directory.CreateDirectory(DatabaseDir)
                    If Not IO.Directory.Exists(BackupDir) Then IO.Directory.CreateDirectory(BackupDir)
                    If Not IO.Directory.Exists(ExportDir) Then IO.Directory.CreateDirectory(ExportDir)
                    If Not IO.Directory.Exists(LogsDir) Then IO.Directory.CreateDirectory(LogsDir)
                    If Not IO.Directory.Exists(ReceiptsDir) Then IO.Directory.CreateDirectory(ReceiptsDir)
                Catch ex As Exception
                    LogCrash(ex, "AppPaths.EnsureDirectoriesExist")
                End Try
            End Sub

        End Module
End Namespace
