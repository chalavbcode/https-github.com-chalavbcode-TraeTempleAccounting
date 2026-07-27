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
                    Dim dbDir = Path.Combine(AppRoot, "Database")
                    If Not Directory.Exists(dbDir) Then Directory.CreateDirectory(dbDir)
                    _databaseFile = Path.Combine(dbDir, "TempleAccounting.accdb")
                    ' Database is stored alongside the executable output so the whole folder stays portable.
                End If
                Return _databaseFile
            End Get
        End Property

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
    End Module
End Namespace
