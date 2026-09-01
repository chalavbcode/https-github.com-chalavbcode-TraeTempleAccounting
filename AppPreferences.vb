Option Strict Off
Option Explicit On

Imports System.IO
Imports System.Text

Namespace TempleAccounting
    ''' <summary>
    ''' เก็บค่า "ประเภท" และ "กองทุน" ที่ผู้ใช้เลือกครั้งล่าสุดในแต่ละฟอร์ม
    ''' เพื่อคืนค่าให้อัตโนมัติเมื่อเปิดฟอร์มใหม่ หรือเปิดโปรแกรมใหม่ครั้งถัดไป
    ''' (ใช้ static variables ในหน่วยความจำ + ไฟล์ข้อความสำหรับคงค่าข้ามการเปิดโปรแกรม)
    ''' </summary>
    Public Module AppPreferences
        Private _lastExpenseCategoryID As Integer
        Private _lastExpenseFundID As Integer
        Private _lastIncomeCategoryID As Integer
        Private _lastIncomeFundID As Integer
        Private _isOnboardingCompleted As Boolean = True
        Private _loaded As Boolean

        Public Property LastExpenseCategoryID As Integer
            Get
                EnsureLoaded()
                Return _lastExpenseCategoryID
            End Get
            Set(value As Integer)
                _lastExpenseCategoryID = value
            End Set
        End Property

        Public Property LastExpenseFundID As Integer
            Get
                EnsureLoaded()
                Return _lastExpenseFundID
            End Get
            Set(value As Integer)
                _lastExpenseFundID = value
            End Set
        End Property

        Public Property LastIncomeCategoryID As Integer
            Get
                EnsureLoaded()
                Return _lastIncomeCategoryID
            End Get
            Set(value As Integer)
                _lastIncomeCategoryID = value
            End Set
        End Property

        Public Property LastIncomeFundID As Integer
            Get
                EnsureLoaded()
                Return _lastIncomeFundID
            End Get
            Set(value As Integer)
                _lastIncomeFundID = value
            End Set
        End Property

        ''' <summary>
        ''' True = onboarding wizard has been completed (or was never needed).
        ''' Default is True so existing users are NOT prompted to re-run onboarding.
        ''' Only a brand-new install (empty/no user_preferences.ini) will get the wizard.
        ''' </summary>
        Public Property IsOnboardingCompleted As Boolean
            Get
                EnsureLoaded()
                Return _isOnboardingCompleted
            End Get
            Set(value As Boolean)
                _isOnboardingCompleted = value
            End Set
        End Property

        Private ReadOnly Property ConfigFilePath As String
            Get
                Return Path.Combine(AppPaths.AppRoot, "user_preferences.ini")
            End Get
        End Property

        Private Sub EnsureLoaded()
            If _loaded Then Return
            _loaded = True
            Try
                Dim cfgPath = ConfigFilePath
                If Not File.Exists(cfgPath) Then Return
                For Each line As String In File.ReadAllLines(cfgPath, Encoding.UTF8)
                    Dim eq = line.IndexOf("="c)
                    If eq <= 0 Then Continue For
                    Dim key = line.Substring(0, eq).Trim()
                    Dim val = line.Substring(eq + 1).Trim()
                    Dim n As Integer
                    If Not Integer.TryParse(val, n) Then Continue For
                    Select Case key
                        Case "LastExpenseCategoryID" : _lastExpenseCategoryID = n
                        Case "LastExpenseFundID" : _lastExpenseFundID = n
                        Case "LastIncomeCategoryID" : _lastIncomeCategoryID = n
                        Case "LastIncomeFundID" : _lastIncomeFundID = n
                        Case "IsOnboardingCompleted" : _isOnboardingCompleted = (n <> 0)
                    End Select
                Next
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("AppPreferences.EnsureLoaded", ex)
            End Try
        End Sub

        Public Sub Save()
            Try
                Dim cfgPath = ConfigFilePath
                Dim dir = Path.GetDirectoryName(cfgPath)
                If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                    Directory.CreateDirectory(dir)
                End If
                Dim sb As New StringBuilder()
                sb.AppendLine("LastExpenseCategoryID=" & _lastExpenseCategoryID)
                sb.AppendLine("LastExpenseFundID=" & _lastExpenseFundID)
                sb.AppendLine("LastIncomeCategoryID=" & _lastIncomeCategoryID)
                sb.AppendLine("LastIncomeFundID=" & _lastIncomeFundID)
                sb.AppendLine("IsOnboardingCompleted=" & If(_isOnboardingCompleted, "1", "0"))
                File.WriteAllText(cfgPath, sb.ToString(), Encoding.UTF8)
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("AppPreferences.Save", ex)
            End Try
        End Sub
    End Module
End Namespace
