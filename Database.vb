Option Strict Off
Option Explicit On

Imports System.Data
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' Public database facade. All methods delegate to the active IDbProvider
    ''' (configured via DbProviderFactory.ProviderType — default: Access).
    ''' 
    ''' Callers continue using the same Db.* static API; switching to SQL Server
    ''' requires only changing DbProviderFactory.ProviderType at startup.
    ''' </summary>
    Public Module Db
        Public ReadOnly Property ConnectionString As String
            Get
                Return DbProviderFactory.Current.ConnectionString
            End Get
        End Property

        ''' <summary>Opens a connection to the active database provider.</summary>
        Public Function OpenConn() As IDbConnection
            Return DbProviderFactory.Current.OpenConn()
        End Function

        ''' <summary>Creates tables, seeds data, and runs migrations if needed.</summary>
        Public Sub EnsureSchema()
            DbProviderFactory.Current.EnsureSchema()
        End Sub

        Public Function ExecuteNonQuery(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Return DbProviderFactory.Current.ExecuteNonQuery(conn, sql, params)
        End Function

        Public Function DbScalar(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object
            Return DbProviderFactory.Current.DbScalar(conn, sql, params)
        End Function

        Public Function GetTable(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable
            Return DbProviderFactory.Current.GetTable(conn, sql, params)
        End Function

        Public Function InsertAndGetId(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Return DbProviderFactory.Current.InsertAndGetId(conn, sql, params)
        End Function

        ''' <summary>Converts Buddhist-era year (>2400) to Gregorian.</summary>
        Public Function NormalizeGregorianDate(value As Date) As Date
            Return DbProviderFactory.Current.NormalizeGregorianDate(value)
        End Function

        ''' <summary>Formats a DateTime as a valid date literal for the active database engine.</summary>
        Public Function AccessDateLiteral(value As Date) As String
            Return DbProviderFactory.Current.AccessDateLiteral(value)
        End Function

        ''' <summary>Escapes special characters in LIKE-pattern text for the active database engine.</summary>
        Public Function EscapeLikeText(value As String) As String
            Return DbProviderFactory.Current.EscapeLikeText(value)
        End Function

        ''' <summary>DB-agnostic: safely converts a nullable value to Decimal, defaulting to 0.</summary>
        Public Function ToDecimalOrZero(value As Object) As Decimal
            If value Is Nothing OrElse value Is DBNull.Value Then Return 0D
            Return Convert.ToDecimal(value)
        End Function

        ''' <summary>DB-agnostic: safely converts a nullable value to Integer, defaulting to 0.</summary>
        Public Function ToIntOrZero(value As Object) As Integer
            If value Is Nothing OrElse value Is DBNull.Value Then Return 0
            Return Convert.ToInt32(value)
        End Function

        ''' <summary>Backs up the database file to the backup folder.</summary>
        Public Sub BackupDatabase(Optional targetDir As String = Nothing)
            DbProviderFactory.Current.BackupDatabase(targetDir)
        End Sub
    End Module
End Namespace
