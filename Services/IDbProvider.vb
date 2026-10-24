Option Strict Off
Option Explicit On

Imports System.Data
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' Database provider abstraction — mirrors the public API of Database.vb.
    ''' Implementations: AccessDbProvider (OleDb), SqlServerDbProvider (T-SQL).
    ''' All DB-agnostic consumers call through this interface via DbProviderFactory.Current.
    ''' </summary>
    Public Interface IDbProvider
        ReadOnly Property ConnectionString As String

        Function OpenConn() As IDbConnection

        ''' <summary>Creates tables, seeds data, and runs migrations if needed.</summary>
        Sub EnsureSchema()

        Function ExecuteNonQuery(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer

        Function DbScalar(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object

        Function GetTable(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable

        Function InsertAndGetId(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer

        ''' <summary>
        ''' Transaction-aware overloads. OleDb/SqlClient throw "The Transaction property of the
        ''' command has not been initialized" when a command runs on a connection that has a
        ''' pending transaction but the command's Transaction is not set — so every command
        ''' executed inside a caller-managed transaction MUST go through these overloads and
        ''' receive the ambient IDbTransaction.
        ''' </summary>
        Function ExecuteNonQuery(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer

        Function DbScalar(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object

        Function GetTable(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable

        Function InsertAndGetId(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer

        ''' <summary>Converts Buddhist-era year (>2400) to Gregorian.</summary>
        Function NormalizeGregorianDate(value As Date) As Date

        ''' <summary>Formats a DateTime as a valid date literal for this DB engine.</summary>
        Function AccessDateLiteral(value As Date) As String

        ''' <summary>Escapes special characters in LIKE-pattern text for this DB engine.</summary>
        Function EscapeLikeText(value As String) As String

        ''' <summary>Copies/saves the database file to a backup directory.</summary>
        Sub BackupDatabase(Optional targetDir As String = Nothing)
    End Interface
End Namespace
