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
        Private _saddhiviharikaDbFile As String
        Private _importFolder As String
        Private _backupFolder As String
        Private _exportFolder As String
        Private _logsFolder As String
        Private _logPath As String
        Private _masterReceiptsDir As String
        Private _projectRoot As String
        Private _projectRootSearched As Boolean

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

        ''' <summary>SQLite database for Saddhiviharika ordination registry system.</summary>
        Public ReadOnly Property SaddhiviharikaDbFile As String
            Get
                If String.IsNullOrEmpty(_saddhiviharikaDbFile) Then
                    Dim dbDir = Path.Combine(AppRoot, "Database")
                    If Not Directory.Exists(dbDir) Then Directory.CreateDirectory(dbDir)
                    _saddhiviharikaDbFile = Path.Combine(dbDir, "Saddhiviharika.db")
                End If
                Return _saddhiviharikaDbFile
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

        Public ReadOnly Property ManualFile As String
            Get
                Return Path.Combine(AppRoot, "USER_MANUAL_TH.md")
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

        ' --- Project Root (find directory containing TempleAccounting.vbproj/.slnx) ---
        ''' <summary>
        ''' Finds the project root by walking up from AppRoot, looking for TempleAccounting.vbproj or TempleAccounting.slnx.
        ''' Returns Nothing if not found (production deployment scenario).
        ''' </summary>
        Public ReadOnly Property ProjectRoot As String
            Get
                If Not _projectRootSearched Then
                    _projectRootSearched = True
                    Try
                        Dim dir = AppRoot
                        For i As Integer = 0 To 12
                            If File.Exists(Path.Combine(dir, "TempleAccounting.vbproj")) OrElse
                               File.Exists(Path.Combine(dir, "TempleAccounting.slnx")) Then
                                _projectRoot = dir
                                Exit For
                            End If
                            Dim parent = Directory.GetParent(dir)
                            If parent Is Nothing Then Exit For
                            dir = parent.FullName
                        Next
                    Catch
                        _projectRoot = Nothing
                    End Try
                End If
                Return _projectRoot
            End Get
        End Property

        ' --- Permanent Master Receipts Directory (survives VS Build/Rebuild) ---
        ''' <summary>
        ''' The permanent master receipts directory located at {ProjectRoot}\Receipts.
        ''' Falls back to {AppRoot}\Receipts when ProjectRoot is not found (production).
        ''' This directory is OUTSIDE bin\ so it never gets wiped by VS Clean/Rebuild.
        ''' </summary>
        Public ReadOnly Property MasterReceiptsDir As String
            Get
                If String.IsNullOrEmpty(_masterReceiptsDir) Then
                    Dim baseFolder As String = If(ProjectRoot IsNot Nothing, ProjectRoot, AppRoot)
                    _masterReceiptsDir = Path.Combine(baseFolder, "Receipts")
                    If Not Directory.Exists(_masterReceiptsDir) Then
                        Directory.CreateDirectory(_masterReceiptsDir)
                    End If
                End If
                Return _masterReceiptsDir
            End Get
        End Property

        ' --- ReceiptsDir (primary save target — now points to the permanent master dir) ---
        Public ReadOnly Property ReceiptsDir As String
            Get
                ' During development: save to project-root Receipts (survives rebuilds).
                ' In production: save to AppRoot\Receipts (which IS the final deployment folder).
                Return MasterReceiptsDir
            End Get
        End Property

        ' --- One-time migration: copy any existing receipt files from legacy locations
        '     (e.g., old bin\Debug\Receipts) to the permanent master directory ---
        ''' <summary>
        ''' Migrates receipt files (*.jpg) from legacy locations (bin\Debug\Receipts etc.)
        ''' into the permanent MasterReceiptsDir. Non-destructive: skips files that already exist.
        ''' Call once on application startup.
        ''' </summary>
        Public Sub MigrateReceiptsToMaster()
            Try
                Dim masterDir = MasterReceiptsDir
                Dim migratedCount As Integer = 0

                For Each srcFolder As String In GetLegacyReceiptFolders()
                    If Not Directory.Exists(srcFolder) Then Continue For
                    ' Skip if source is the master directory itself
                    If String.Equals(srcFolder.TrimEnd(Path.DirectorySeparatorChar),
                                    masterDir.TrimEnd(Path.DirectorySeparatorChar),
                                    StringComparison.OrdinalIgnoreCase) Then
                        Continue For
                    End If
                    Try
                        For Each filePath As String In Directory.GetFiles(srcFolder, "*.jpg")
                            Try
                                Dim fileName = Path.GetFileName(filePath)
                                Dim destPath = Path.Combine(masterDir, fileName)
                                If Not File.Exists(destPath) Then
                                    File.Copy(filePath, destPath, overwrite:=False)
                                    migratedCount += 1
                                End If
                            Catch
                                ' Skip individual file copy failures silently
                            End Try
                        Next
                    Catch
                    End Try
                Next

                If migratedCount > 0 Then
                    LogStartup($"Migrated {migratedCount} receipt image(s) to permanent master directory: {masterDir}")
                End If
            Catch ex As Exception
                LogCrash(ex, "AppPaths.MigrateReceiptsToMaster")
            End Try
        End Sub

        ''' <summary>
        ''' Legacy receipt folders that may contain images from before the permanent-directory fix.
        ''' These are searched for migration and also as fallback lookup locations.
        ''' </summary>
        Public Function GetLegacyReceiptFolders() As List(Of String)
            Dim folders As New List(Of String)()

            ' 1. Old location: AppRoot\Receipts (bin\Debug\net10.0-windows\Receipts)
            folders.Add(Path.Combine(AppRoot, "Receipts"))

            ' 2. Parent of AppRoot (e.g. bin\Debug\net10.0-windows -> bin\Debug)
            Try
                Dim parent = Directory.GetParent(AppRoot)
                If parent IsNot Nothing Then
                    folders.Add(Path.Combine(parent.FullName, "Receipts"))
                End If
            Catch
            End Try

            ' 3. Project root Receipts (if different from AppRoot — covers legacy project-root saves)
            If ProjectRoot IsNot Nothing Then
                folders.Add(Path.Combine(ProjectRoot, "Receipts"))
                Try
                    folders.Add(Path.Combine(ProjectRoot, "Images", "Receipts"))
                Catch
                End Try
            End If

            ' 4. Current directory Receipts
            Try
                folders.Add(Path.Combine(Environment.CurrentDirectory, "Receipts"))
                folders.Add(Path.Combine(Environment.CurrentDirectory, "Images", "Receipts"))
            Catch
            End Try

            ' Deduplicate (case-insensitive)
            Dim result As New List(Of String)()
            For Each f As String In folders
                If Not result.Contains(f, StringComparer.OrdinalIgnoreCase) Then
                    result.Add(f)
                End If
            Next
            Return result
        End Function

        ' --- 2. เพิ่ม Method ตรวจสอบและสร้างโฟลเดอร์ทั้งหมด ---
        Public Sub EnsureDirectoriesExist()
            Try
                Dim dbDir = IO.Path.GetDirectoryName(DatabaseFile)
                If Not IO.Directory.Exists(dbDir) Then IO.Directory.CreateDirectory(dbDir)
                If Not IO.Directory.Exists(BackupFolder) Then IO.Directory.CreateDirectory(BackupFolder)
                If Not IO.Directory.Exists(ExportFolder) Then IO.Directory.CreateDirectory(ExportFolder)
                If Not IO.Directory.Exists(LogsFolder) Then IO.Directory.CreateDirectory(LogsFolder)
                If Not IO.Directory.Exists(ReceiptsDir) Then IO.Directory.CreateDirectory(ReceiptsDir)
                ' Ensure master directory is created (triggers property getter which creates if needed)
                Dim ignore = MasterReceiptsDir
            Catch ex As Exception
                LogCrash(ex, "AppPaths.EnsureDirectoriesExist")
            End Try
        End Sub

        End Module
End Namespace
