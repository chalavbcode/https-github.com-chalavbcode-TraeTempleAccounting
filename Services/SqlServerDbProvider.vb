Option Strict Off
Option Explicit On

Imports System.Data
Imports Microsoft.Data.SqlClient
Imports System.IO
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' SQL Server implementation of IDbProvider.
    ''' Uses SqlConnection/SqlCommand with T-SQL syntax.
    ''' Translates Access-specific SQL (COUNTER→IDENTITY, TEXT→NVARCHAR, MEMO→NVARCHAR(MAX),
    ''' CURRENCY→MONEY, DateSerial→DATEFROMPARTS, IIF→CASE WHEN, #→' date literals, @@IDENTITY→SCOPE_IDENTITY).
    ''' </summary>
    Public Class SqlServerDbProvider
        Implements IDbProvider

        Private _schemaChecked As Boolean = False

        ''' <summary>
        ''' Connection string target: localhost\SQLEXPRESS, Database=TempleAccountingDb, Integrated Security.
        ''' </summary>
        Public ReadOnly Property ConnectionString As String Implements IDbProvider.ConnectionString
            Get
                Return "Server=localhost\SQLEXPRESS;Database=TempleAccountingDb;Integrated Security=True;TrustServerCertificate=True;"
            End Get
        End Property

        Public Function OpenConn() As IDbConnection Implements IDbProvider.OpenConn
            Dim conn As New SqlConnection(ConnectionString)
            conn.Open()
            Return conn
        End Function

        Public Sub EnsureSchema() Implements IDbProvider.EnsureSchema
            If _schemaChecked Then Return

            Try
                EnsureDatabaseExists()
                Using conn = TryCast(OpenConn(), SqlConnection)
                    ' Create tables with T-SQL DDL (IF NOT EXISTS pattern)
                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Province')
                        CREATE TABLE Province (ProvinceID INT PRIMARY KEY, ProvinceName NVARCHAR(100) NOT NULL)")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'District')
                        CREATE TABLE District (DistrictID INT PRIMARY KEY, ProvinceID INT NOT NULL, DistrictName NVARCHAR(100) NOT NULL)")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubDistrict')
                        CREATE TABLE SubDistrict (SubDistrictID INT PRIMARY KEY, DistrictID INT NOT NULL, SubDistrictName NVARCHAR(100) NOT NULL, ZipCode NVARCHAR(10))")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TempleSetting')
                        CREATE TABLE TempleSetting (ID INT IDENTITY(1,1) PRIMARY KEY, TempleCode NVARCHAR(20), TempleName NVARCHAR(200), TempleAddress NVARCHAR(MAX), Tambon NVARCHAR(100), Amphoe NVARCHAR(100), Province NVARCHAR(100), PostCode NVARCHAR(10), TemplePhone NVARCHAR(30), AbbotPersonnelID INT, WaiyawatPersonnelID INT, BookkeeperPersonnelID INT, PromptPayName NVARCHAR(100), PromptPayID NVARCHAR(50))")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
                        CREATE TABLE Categories (ID INT IDENTITY(1,1) PRIMARY KEY, CategoryName NVARCHAR(200) NOT NULL, TranType NVARCHAR(10) NOT NULL)")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Funds')
                        CREATE TABLE Funds (ID INT IDENTITY(1,1) PRIMARY KEY, FundName NVARCHAR(200) NOT NULL)")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BankAccounts')
                        CREATE TABLE BankAccounts (ID INT IDENTITY(1,1) PRIMARY KEY, BankName NVARCHAR(100) NOT NULL, AccountNo NVARCHAR(50), AccountName NVARCHAR(200))")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Transactions')
                        CREATE TABLE Transactions (ID INT IDENTITY(1,1) PRIMARY KEY, TranDate DATETIME2 NOT NULL, TranType NVARCHAR(10) NOT NULL, CategoryID INT, FundID INT, BankID INT, Detail NVARCHAR(255), Amount MONEY NOT NULL, Note NVARCHAR(MAX), CreateDate DATETIME2 DEFAULT GETDATE(), ToFundID INT, ToBankID INT, ReceiptPath NVARCHAR(255))")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Personnel')
                        CREATE TABLE Personnel (PersonnelID INT IDENTITY(1,1) PRIMARY KEY, Title NVARCHAR(50), FirstName NVARCHAR(100), LastName NVARCHAR(100), FullName NVARCHAR(255), PersonType NVARCHAR(50), PositionID INT, Phone NVARCHAR(50))")

                    ExecuteNonQueryInternal(conn, "
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Positions')
                        CREATE TABLE Positions (PositionID INT IDENTITY(1,1) PRIMARY KEY, PositionName NVARCHAR(100), PositionGroup NVARCHAR(20))")

                    ' Run column migrations
                    MigrateColumns(conn)

                    ' Seed data
                    SeedCategories(conn)
                    SeedFunds(conn)
                    SeedPositions(conn)
                    ' BankAccounts / Personnel are NOT seeded — they hold this temple's identity
                    ' (formerly seeded "วัดแหลมยาง" bank account and 6 sample people into every new DB)
                End Using
                _schemaChecked = True
            Catch ex As Exception
                _schemaChecked = False
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Ensures the TempleAccountingDb database exists on the target SQL Server instance.
        ''' Connects to 'master' first, then creates the database if missing.
        ''' </summary>
        Private Sub EnsureDatabaseExists()
            Dim masterConnStr = "Server=localhost\SQLEXPRESS;Database=master;Integrated Security=True;TrustServerCertificate=True;"
            Using conn As New SqlConnection(masterConnStr)
                conn.Open()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TempleAccountingDb')
                        BEGIN
                            CREATE DATABASE TempleAccountingDb
                        END"
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' ---------------- Column migrations (INFORMATION_SCHEMA) ----------------

        Private Sub MigrateColumns(conn As SqlConnection)
            ' Positions: PositionGroup
            If Not ColumnExists(conn, "Positions", "PositionGroup") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Positions ADD PositionGroup NVARCHAR(20) NULL")
            End If

            ' Personnel: PositionID
            If Not ColumnExists(conn, "Personnel", "PositionID") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Personnel ADD PositionID INT NULL")
                ' Set default positions for existing personnel
                ExecuteNonQueryInternal(conn, "UPDATE Personnel SET PositionID = 1 WHERE PersonType = 'Monk'")
                ExecuteNonQueryInternal(conn, "UPDATE Personnel SET PositionID = 2 WHERE PersonType = 'Layperson'")
            End If

            ' Transactions: ToFundID, ToBankID, ReceiptPath, TransactionGUID, ReceiveChannel, ReferenceNo
            If Not ColumnExists(conn, "Transactions", "ToFundID") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD ToFundID INT NULL")
            End If
            If Not ColumnExists(conn, "Transactions", "ToBankID") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD ToBankID INT NULL")
            End If
            If Not ColumnExists(conn, "Transactions", "ReceiptPath") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD ReceiptPath NVARCHAR(255) NULL")
            End If
            If Not ColumnExists(conn, "Transactions", "TransactionGUID") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD TransactionGUID NVARCHAR(36) NULL")
            End If
            If Not ColumnExists(conn, "Transactions", "ReceiveChannel") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD ReceiveChannel NVARCHAR(20) NULL")
            End If
            If Not ColumnExists(conn, "Transactions", "ReferenceNo") Then
                ExecuteNonQueryInternal(conn, "ALTER TABLE Transactions ADD ReferenceNo NVARCHAR(50) NULL")
            End If
        End Sub

        ''' <summary>Checks if a column exists in a table using INFORMATION_SCHEMA.COLUMNS.</summary>
        Private Function ColumnExists(conn As SqlConnection, tableName As String, columnName As String) As Boolean
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "
                    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName"
                cmd.Parameters.AddWithValue("@TableName", tableName)
                cmd.Parameters.AddWithValue("@ColumnName", columnName)
                Return CInt(cmd.ExecuteScalar()) > 0
            End Using
        End Function

        ' ---------------- Seed methods (T-SQL syntax) ----------------

        Private Sub SeedCategories(conn As SqlConnection)
            Dim count As Integer = CInt(DbScalar(conn, "SELECT COUNT(*) FROM Categories"))
            If count > 0 Then Return
            Dim list As New List(Of Tuple(Of String, String)) From {
                Tuple.Create("เงินบริจาคทั่วไป", "Income"),
                Tuple.Create("เงินทอดพระเนตร", "Income"),
                Tuple.Create("ดอกเบี้ยเงินฝาก", "Income"),
                Tuple.Create("รายได้อื่นๆ", "Income"),
                Tuple.Create("ค่าอาหารและข้าวสาร", "Expense"),
                Tuple.Create("ค่าน้ำประปา", "Expense"),
                Tuple.Create("ค่าไฟฟ้า", "Expense"),
                Tuple.Create("ค่าซ่อมบำรุง", "Expense"),
                Tuple.Create("ค่าอุปกรณ์วัด", "Expense"),
                Tuple.Create("รายจ่ายอื่นๆ", "Expense")
            }
            For Each item In list
                ExecuteNonQueryInternal(conn, "INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)",
                    New Tuple(Of String, Object)("@n", item.Item1),
                    New Tuple(Of String, Object)("@t", item.Item2))
            Next
        End Sub

        Private Sub SeedFunds(conn As SqlConnection)
            Dim count As Integer = CInt(DbScalar(conn, "SELECT COUNT(*) FROM Funds"))
            If count > 0 Then Return
            For Each n In {"กองทุนทั่วไป", "กองทุนกฤติยาธรรม", "กองทุนกู้ภัยวัด", "กองทุนซ่อมแซม"}
                ExecuteNonQueryInternal(conn, "INSERT INTO Funds (FundName) VALUES (@n)", New Tuple(Of String, Object)("@n", n))
            Next
        End Sub

        Private Sub SeedPositions(conn As SqlConnection)
            Dim posList = New List(Of (Name As String, Group As String)) From {
                ("เจ้าอาวาส", "พระ"), ("รองเจ้าอาวาส", "พระ"), ("ผู้ช่วยเจ้าอาวาส", "พระ"), ("เลขานุการเจ้าอาวาส", "พระ"),
                ("พระภิกษุ", "พระ"), ("พระลูกวัด", "พระ"), ("พระอาจารย์", "พระ"), ("สามเณร", "พระ"),
                ("ไวยาวัจกร", "ฆราวาส"), ("รองไวยาวัจกร", "ฆราวาส"), ("ผู้ช่วยไวยาวัจกร", "ฆราวาส"), ("เหรัญญิก", "ฆราวาส"),
                ("ผู้ทำบัญชี", "ฆราวาส"), ("เจ้าหน้าที่การเงิน", "ฆราวาส"), ("กรรมการวัด", "ฆราวาส"), ("ผู้ดูแลทรัพย์สิน", "ฆราวาส"),
                ("เจ้าหน้าที่สำนักงานวัด", "ฆราวาส"), ("อาสาสมัคร", "ทั่วไป"), ("อื่น ๆ", "ทั่วไป")
            }

            For Each pos In posList
                Dim exists = DbScalar(conn, "SELECT COUNT(*) FROM Positions WHERE PositionName = @n", New Tuple(Of String, Object)("@n", pos.Name))
                If CInt(exists) = 0 Then
                    ExecuteNonQueryInternal(conn, "INSERT INTO Positions (PositionName, PositionGroup) VALUES (@n, @g)",
                        New Tuple(Of String, Object)("@n", pos.Name),
                        New Tuple(Of String, Object)("@g", pos.Group))
                Else
                    ' Update only when the value actually differs — rewriting every boot
                    ' changes the DB file hash and invalidates backup hash-equality.
                    Dim currentGroup = DbScalar(conn, "SELECT PositionGroup FROM Positions WHERE PositionName = @n",
                        New Tuple(Of String, Object)("@n", pos.Name))
                    If Not String.Equals(CStr(currentGroup), pos.Group, StringComparison.Ordinal) Then
                        ExecuteNonQueryInternal(conn, "UPDATE Positions SET PositionGroup = @g WHERE PositionName = @n",
                            New Tuple(Of String, Object)("@g", pos.Group),
                            New Tuple(Of String, Object)("@n", pos.Name))
                    End If
                End If
            Next
        End Sub

        ' ---------------- Internal query helpers (work with SqlConnection directly) ----------------

        Private Function ExecuteNonQueryInternal(conn As SqlConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        ' ---------------- Public IDbProvider implementation ----------------

        Public Function ExecuteNonQuery(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.ExecuteNonQuery
            Return ExecuteNonQuery(conn, CType(Nothing, IDbTransaction), sql, params)
        End Function

        Public Function ExecuteNonQuery(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.ExecuteNonQuery
            Dim sqlConn = TryCast(conn, SqlConnection)
            Using cmd = sqlConn.CreateCommand()
                cmd.CommandText = TranslateSql(sql)
                If tx IsNot Nothing Then cmd.Transaction = TryCast(tx, SqlTransaction)
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        Public Function DbScalar(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object Implements IDbProvider.DbScalar
            Return DbScalar(conn, CType(Nothing, IDbTransaction), sql, params)
        End Function

        Public Function DbScalar(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object Implements IDbProvider.DbScalar
            Dim sqlConn = TryCast(conn, SqlConnection)
            Using cmd = sqlConn.CreateCommand()
                cmd.CommandText = TranslateSql(sql)
                If tx IsNot Nothing Then cmd.Transaction = TryCast(tx, SqlTransaction)
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Dim r = cmd.ExecuteScalar()
                If r Is DBNull.Value Then Return Nothing
                Return r
            End Using
        End Function

        Public Function GetTable(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable Implements IDbProvider.GetTable
            Return GetTable(conn, CType(Nothing, IDbTransaction), sql, params)
        End Function

        Public Function GetTable(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable Implements IDbProvider.GetTable
            Dim sqlConn = TryCast(conn, SqlConnection)
            Using cmd = sqlConn.CreateCommand()
                cmd.CommandText = TranslateSql(sql)
                If tx IsNot Nothing Then cmd.Transaction = TryCast(tx, SqlTransaction)
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Public Function InsertAndGetId(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.InsertAndGetId
            Return InsertAndGetId(conn, CType(Nothing, IDbTransaction), sql, params)
        End Function

        Public Function InsertAndGetId(conn As IDbConnection, tx As IDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.InsertAndGetId
            ExecuteNonQuery(conn, tx, sql, params)
            ' SQL Server uses SCOPE_IDENTITY() — no @@IDENTITY (which may return trigger-generated identity)
            Return CInt(DbScalar(conn, tx, "SELECT SCOPE_IDENTITY()"))
        End Function

        Public Function NormalizeGregorianDate(value As Date) As Date Implements IDbProvider.NormalizeGregorianDate
            If value.Year > 2400 Then
                Return New Date(value.Year - 543, value.Month, value.Day, value.Hour, value.Minute, value.Second)
            End If
            Return value
        End Function

        Public Function AccessDateLiteral(value As Date) As String Implements IDbProvider.AccessDateLiteral
            Dim safeValue = NormalizeGregorianDate(value)
            ' T-SQL uses single-quoted ISO 8601 strings for date literals
            Return "'" & safeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) & "'"
        End Function

        Public Function EscapeLikeText(value As String) As String Implements IDbProvider.EscapeLikeText
            If String.IsNullOrEmpty(value) Then Return ""
            ' T-SQL LIKE escapes: % _ [  are special; escape with [] wrapping
            Return value.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]")
        End Function

        Public Sub BackupDatabase(Optional targetDir As String = Nothing) Implements IDbProvider.BackupDatabase
            If String.IsNullOrEmpty(targetDir) Then targetDir = AppPaths.BackupFolder
            If Not Directory.Exists(targetDir) Then Directory.CreateDirectory(targetDir)
            Dim fn = $"TempleAccounting_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
            Dim dst = Path.Combine(targetDir, fn)

            Using conn = TryCast(OpenConn(), SqlConnection)
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = $"BACKUP DATABASE TempleAccountingDb TO DISK = @dst WITH FORMAT, INIT"
                    cmd.Parameters.AddWithValue("@dst", dst)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' ---------------- SQL Translation ----------------

        ''' <summary>
        ''' Translates Access-specific SQL constructs to T-SQL conventions.
        ''' Handles: DateSerial → DATEFROMPARTS, IIF → CASE WHEN, Now() → GETDATE(),
        '''          #date# → 'date' (already handled by AccessDateLiteral at the application level).
        ''' Also converts Access-style INNER JOIN in UPDATE to FROM-based UPDATE.
        ''' </summary>
        Private Shared Function TranslateSql(sql As String) As String
            If String.IsNullOrEmpty(sql) Then Return sql

            Dim result = sql

            ' DateSerial(y, m, d) → DATEFROMPARTS(y, m, d)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                "DateSerial\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)",
                "DATEFROMPARTS($1, $2, $3)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            ' Now() → GETDATE()
            result = System.Text.RegularExpressions.Regex.Replace(result,
                "\bNow\(\)", "GETDATE()",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            ' IIF(cond, trueVal, falseVal) → CASE WHEN cond THEN trueVal ELSE falseVal END
            ' Simple IIF with balanced parentheses
            result = ReplaceIIF(result)

            ' Access-style UPDATE with INNER JOIN → T-SQL UPDATE FROM
            ' e.g., "UPDATE T1 INNER JOIN T2 ON ... SET ... WHERE ..."
            ' →    "UPDATE T1 SET ... FROM T1 INNER JOIN T2 ON ... WHERE ..."
            result = FixUpdateJoin(result)

            Return result
        End Function

        ''' <summary>Replaces IIF(a,b,c) with CASE WHEN a THEN b ELSE c END (recursive for nested).</summary>
        Private Shared Function ReplaceIIF(sql As String) As String
            ' Handle simple IIF: IIF(condition, thenValue, elseValue)
            ' Use a state machine to handle nesting
            Dim result = System.Text.RegularExpressions.Regex.Replace(sql,
                "IIF\s*\(\s*",
                "CASE WHEN ",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            ' But after the first comma in what was IIF, we need THEN, and after second comma we need ELSE ... END
            ' This is complex for regex, so do a manual pass for simple cases
            ' Strategy: find "CASE WHEN" markers and fix up the THEN/ELSE/END
            Return FixCaseWhen(result)
        End Function

        ''' <summary>After regex replaces IIF( with CASE WHEN, fixes commas to THEN/ELSE/END.</summary>
        Private Shared Function FixCaseWhen(sql As String) As String
            ' Find positions of "CASE WHEN " that were originally IIF
            Dim pos As Integer = 0
            While True
                pos = sql.IndexOf("CASE WHEN ", pos, StringComparison.OrdinalIgnoreCase)
                If pos < 0 Then Exit While

                ' Find the matching closing paren for this IIF
                ' Start scanning from the position after CASE WHEN
                Dim scanStart = pos + "CASE WHEN ".Length
                Dim depth As Integer = 0
                Dim comma1 As Integer = -1
                Dim comma2 As Integer = -1
                Dim closeParen As Integer = -1

                For i As Integer = scanStart To sql.Length - 1
                    Select Case sql(i)
                        Case "("c
                            depth += 1
                        Case ")"c
                            If depth = 0 Then
                                closeParen = i
                                Exit For
                            End If
                            depth -= 1
                        Case ","c
                            If depth = 0 Then
                                If comma1 < 0 Then
                                    comma1 = i
                                ElseIf comma2 < 0 Then
                                    comma2 = i
                                End If
                            End If
                    End Select
                Next

                If comma1 >= 0 AndAlso comma2 >= 0 AndAlso closeParen >= 0 Then
                    ' Replace: comma1 → " THEN", comma2 → " ELSE", closeParen → " END)"
                    Dim sb As New System.Text.StringBuilder(sql)
                    sb(comma1) = " "c ' just mark, we insert
                    sb.Insert(comma1, " THEN")
                    ' After insertion, positions shift
                    Dim shift1 = " THEN".Length - 1
                    sb(comma2 + shift1) = " "c
                    sb.Insert(comma2 + shift1, " ELSE")
                    Dim shift2 = shift1 + " ELSE".Length - 1
                    sb(closeParen + shift2) = ")"c
                    sb.Insert(closeParen + shift2, " END")
                    sql = sb.ToString()
                End If
                pos += "CASE WHEN ".Length
            End While
            Return sql
        End Function

        ''' <summary>
        ''' Converts Access-style "UPDATE t1 INNER JOIN t2 ON ... SET ..." 
        ''' to T-SQL "UPDATE t1 SET ... FROM t1 INNER JOIN t2 ON ..."
        ''' </summary>
        Private Shared Function FixUpdateJoin(sql As String) As String
            ' Pattern: UPDATE TableAlias INNER JOIN ... ON ... SET ...
            Dim pattern = "UPDATE\s+(\w+)\s+INNER\s+JOIN"
            Dim match = System.Text.RegularExpressions.Regex.Match(sql, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            If Not match.Success Then Return sql

            ' Find SET position
            Dim setPos = sql.IndexOf(" SET ", StringComparison.OrdinalIgnoreCase)
            If setPos < 0 Then Return sql

            Dim tableAlias = match.Groups(1).Value
            Dim afterUpdate = sql.Substring(match.Index + match.Length)
            Dim beforeSet = sql.Substring(0, setPos)
            Dim afterSet = sql.Substring(setPos)

            Return $"UPDATE {tableAlias}{afterSet} FROM {tableAlias} INNER JOIN{afterUpdate}"
        End Function
    End Class
End Namespace
