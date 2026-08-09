Option Strict Off
Option Explicit On

Imports System

Namespace TempleAccounting
    ''' <summary>
    ''' Provider type enum — switchable at startup or via config.
    ''' </summary>
    Public Enum DbProviderType
        Access
        SqlServer
    End Enum

    ''' <summary>
    ''' Static factory that holds the active IDbProvider instance.
    ''' Set ProviderType before accessing any Db.* methods (default: Access).
    ''' 
    ''' Usage:
    '''   ' During app startup (Program.vb):
    '''   DbProviderFactory.ProviderType = DbProviderType.SqlServer  ' or Access
    '''   
    '''   ' Everywhere else — unchanged:
    '''   Using conn = Db.OpenConn()
    '''       Dim dt = Db.GetTable(conn, "SELECT * FROM ...")
    '''   End Using
    ''' </summary>
    Public Module DbProviderFactory
        Private _providerType As DbProviderType = DbProviderType.Access
        Private _current As IDbProvider

        ''' <summary>
        ''' Gets or sets the active provider type. Setting this resets the provider instance.
        ''' </summary>
        Public Property ProviderType As DbProviderType
            Get
                Return _providerType
            End Get
            Set(value As DbProviderType)
                If _providerType <> value OrElse _current Is Nothing Then
                    _providerType = value
                    _current = Nothing
                End If
            End Set
        End Property

        ''' <summary>
        ''' Returns the current IDbProvider, lazily creating it if needed.
        ''' </summary>
        Public ReadOnly Property Current As IDbProvider
            Get
                If _current Is Nothing Then
                    Select Case _providerType
                        Case DbProviderType.Access
                            _current = New AccessDbProvider()
                        Case DbProviderType.SqlServer
                            _current = New SqlServerDbProvider()
                        Case Else
                            _current = New AccessDbProvider()
                    End Select
                End If
                Return _current
            End Get
        End Property

        ''' <summary>
        ''' Resets the provider instance (e.g., after config change).
        ''' </summary>
        Public Sub Reset()
            _current = Nothing
        End Sub
    End Module
End Namespace
