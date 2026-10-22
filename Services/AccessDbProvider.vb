Option Strict Off
Option Explicit On

Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' Microsoft Access (OleDb) implementation of IDbProvider.
    ''' Wraps the original Database.vb internals — all existing SQL and schema logic preserved.
    ''' </summary>
    Public Class AccessDbProvider
        Implements IDbProvider

        Private _schemaChecked As Boolean = False

        Public ReadOnly Property ConnectionString As String Implements IDbProvider.ConnectionString
            Get
                Dim dbPath = AppPaths.DatabaseFile
                Return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;"
            End Get
        End Property

        Public Function OpenConn() As IDbConnection Implements IDbProvider.OpenConn
            Dim conn As New OleDbConnection(ConnectionString)
            conn.Open()
            Return conn
        End Function

        Public Sub EnsureSchema() Implements IDbProvider.EnsureSchema
            If _schemaChecked Then Return

            Try
                If Not File.Exists(AppPaths.DatabaseFile) Then
                    Throw New FileNotFoundException("ไม่พบไฟล์ฐานข้อมูล TempleAccounting.accdb")
                End If

                Using conn = TryCast(OpenConn(), OleDbConnection)
                    TryCreateTable(conn, "Province", "CREATE TABLE Province (ProvinceID INTEGER PRIMARY KEY, ProvinceName TEXT(100) NOT NULL)")
                    TryCreateTable(conn, "District", "CREATE TABLE District (DistrictID INTEGER PRIMARY KEY, ProvinceID INTEGER NOT NULL, DistrictName TEXT(100) NOT NULL)")
                    TryCreateTable(conn, "SubDistrict", "CREATE TABLE SubDistrict (SubDistrictID INTEGER PRIMARY KEY, DistrictID INTEGER NOT NULL, SubDistrictName TEXT(100) NOT NULL, ZipCode TEXT(10))")

                    TryCreateTable(conn, "TempleSetting", "CREATE TABLE TempleSetting (ID COUNTER PRIMARY KEY, TempleCode TEXT(20), TempleName TEXT(200), TempleAddress MEMO, Tambon TEXT(100), Amphoe TEXT(100), Province TEXT(100), PostCode TEXT(10), TemplePhone TEXT(30), AbbotPersonnelID INTEGER, WaiyawatPersonnelID INTEGER, BookkeeperPersonnelID INTEGER, PromptPayName TEXT(100), PromptPayID TEXT(50))")

                    TryCreateTable(conn, "Categories", "CREATE TABLE Categories (ID COUNTER PRIMARY KEY, CategoryName TEXT(200) NOT NULL, TranType TEXT(10) NOT NULL)")
                    TryCreateTable(conn, "Funds", "CREATE TABLE Funds (ID COUNTER PRIMARY KEY, FundName TEXT(200) NOT NULL)")
                    TryCreateTable(conn, "BankAccounts", "CREATE TABLE BankAccounts (ID COUNTER PRIMARY KEY, BankName TEXT(100) NOT NULL, AccountNo TEXT(50), AccountName TEXT(200))")
                    TryCreateTable(conn, "Transactions", "CREATE TABLE Transactions (ID COUNTER PRIMARY KEY, TranDate DATETIME NOT NULL, TranType TEXT(10) NOT NULL, CategoryID INTEGER, FundID INTEGER, BankID INTEGER, Detail TEXT(255), Amount CURRENCY NOT NULL, Note MEMO, CreateDate DATETIME DEFAULT Now(), ToFundID INTEGER, ToBankID INTEGER, ReceiptPath TEXT(255))")
                    TryCreateTable(conn, "Personnel", "CREATE TABLE Personnel (PersonnelID COUNTER PRIMARY KEY, Title TEXT(50), FirstName TEXT(100), LastName TEXT(100), FullName TEXT(255), PersonType TEXT(50), PositionID INTEGER, Phone TEXT(50))")
                    TryCreateTable(conn, "Positions", "CREATE TABLE Positions (PositionID COUNTER PRIMARY KEY, PositionName TEXT(100), PositionGroup TEXT(20))")

                    ' Migrations — same logic as original Database.vb
                    MigratePositions(conn)
                    MigrateTempleSetting(conn)
                    MigratePersonnel(conn)
                    MigrateTransactions(conn)

                    SeedCategories(conn)
                    SeedFunds(conn)
                    SeedBankAccounts(conn)
                    SeedPositions(conn)
                    SeedPersonnel(conn)
                End Using
                _schemaChecked = True
            Catch ex As Exception
                _schemaChecked = False
                Throw
            End Try
        End Sub

        ' ---------------- Migration helpers ----------------

        Private Sub MigratePositions(conn As OleDbConnection)
            Try
                Dim columns = conn.GetSchema("Columns", New String() {Nothing, Nothing, "Positions", Nothing})
                Dim hasPositionGroup As Boolean = False
                For Each row As DataRow In columns.Rows
                    If Convert.ToString(row("COLUMN_NAME")).Equals("PositionGroup", StringComparison.OrdinalIgnoreCase) Then
                        hasPositionGroup = True
                        Exit For
                    End If
                Next
                If Not hasPositionGroup Then
                    ExecuteNonQuery(conn, "ALTER TABLE Positions ADD COLUMN PositionGroup TEXT(20)")
                End If
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AccessDbProvider.MigratePositions")
            End Try
        End Sub

        Private Sub MigrateTempleSetting(conn As OleDbConnection)
            Try
                Dim columns = conn.GetSchema("Columns", New String() {Nothing, Nothing, "TempleSetting", Nothing})
                Dim colNames As New List(Of String)
                For Each row As DataRow In columns.Rows
                    colNames.Add(Convert.ToString(row("COLUMN_NAME")).ToUpper())
                Next

                If Not colNames.Contains("ABBOTPERSONNELID") Then ExecuteNonQuery(conn, "ALTER TABLE TempleSetting ADD COLUMN AbbotPersonnelID INTEGER")
                If Not colNames.Contains("WAIYAWATPERSONNELID") Then ExecuteNonQuery(conn, "ALTER TABLE TempleSetting ADD COLUMN WaiyawatPersonnelID INTEGER")
                If Not colNames.Contains("BOOKKEEPERPERSONNELID") Then ExecuteNonQuery(conn, "ALTER TABLE TempleSetting ADD COLUMN BookkeeperPersonnelID INTEGER")

                If colNames.Contains("ABBOTNAME") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting INNER JOIN Personnel ON TempleSetting.AbbotName = Personnel.FullName SET TempleSetting.AbbotPersonnelID = Personnel.PersonnelID WHERE TempleSetting.AbbotPersonnelID IS NULL")
                ElseIf colNames.Contains("ABBOTID") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting SET AbbotPersonnelID = AbbotID WHERE AbbotPersonnelID IS NULL")
                End If

                If colNames.Contains("WAIYAWATNAME") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting INNER JOIN Personnel ON TempleSetting.WaiyawatName = Personnel.FullName SET TempleSetting.WaiyawatPersonnelID = Personnel.PersonnelID WHERE TempleSetting.WaiyawatPersonnelID IS NULL")
                ElseIf colNames.Contains("WAIYAWATID") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting SET WaiyawatPersonnelID = WaiyawatID WHERE WaiyawatPersonnelID IS NULL")
                End If

                If colNames.Contains("BOOKKEEPERNAME") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting INNER JOIN Personnel ON TempleSetting.BookkeeperName = Personnel.FullName SET TempleSetting.BookkeeperPersonnelID = Personnel.PersonnelID WHERE TempleSetting.BookkeeperPersonnelID IS NULL")
                ElseIf colNames.Contains("BOOKKEEPERID") Then
                    ExecuteNonQuery(conn, "UPDATE TempleSetting SET BookkeeperPersonnelID = BookkeeperID WHERE BookkeeperPersonnelID IS NULL")
                End If

                Dim legacyCols = {"ABBOTNAME", "ABBOTOFFICESTATUS", "WAIYAWATNAME", "WAIYAWATOFFICESTATUS", "BOOKKEEPERNAME", "BOOKKEEPERTYPE", "ABBOTID", "WAIYAWATID", "BOOKKEEPERID"}
                For Each lc In legacyCols
                    If colNames.Contains(lc) Then
                        Try
                            ExecuteNonQuery(conn, $"ALTER TABLE TempleSetting DROP COLUMN {lc}")
                        Catch
                        End Try
                    End If
                Next
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AccessDbProvider.MigrateTempleSetting")
            End Try
        End Sub

        Private Sub MigratePersonnel(conn As OleDbConnection)
            Try
                Dim columns = conn.GetSchema("Columns", New String() {Nothing, Nothing, "Personnel", Nothing})
                Dim hasPositionID As Boolean = False
                For Each row As DataRow In columns.Rows
                    If Convert.ToString(row("COLUMN_NAME")).Equals("PositionID", StringComparison.OrdinalIgnoreCase) Then
                        hasPositionID = True
                        Exit For
                    End If
                Next
                If Not hasPositionID Then
                    ExecuteNonQuery(conn, "ALTER TABLE Personnel ADD COLUMN PositionID INTEGER")
                    ExecuteNonQuery(conn, "UPDATE Personnel SET PositionID = 1 WHERE PersonType = 'Monk'")
                    ExecuteNonQuery(conn, "UPDATE Personnel SET PositionID = 2 WHERE PersonType = 'Layperson'")
                End If
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AccessDbProvider.MigratePersonnel")
            End Try
        End Sub

        Private Sub MigrateTransactions(conn As OleDbConnection)
            Try
                Dim columns = conn.GetSchema("Columns", New String() {Nothing, Nothing, "Transactions", Nothing})
                Dim colMap As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
                For Each row As DataRow In columns.Rows
                    colMap(Convert.ToString(row("COLUMN_NAME"))) = True
                Next

                If Not colMap.ContainsKey("ToFundID") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ToFundID INTEGER")
                End If
                If Not colMap.ContainsKey("ToBankID") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ToBankID INTEGER")
                End If
                If Not colMap.ContainsKey("ReceiptPath") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ReceiptPath TEXT(255)")
                End If
                If Not colMap.ContainsKey("TransactionGUID") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN TransactionGUID TEXT(36)")
                End If
                If Not colMap.ContainsKey("ReceiveChannel") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ReceiveChannel TEXT(20)")
                End If
                If Not colMap.ContainsKey("ReferenceNo") Then
                    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ReferenceNo TEXT(50)")
                End If
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AccessDbProvider.MigrateTransactions")
            End Try
        End Sub

        ' ---------------- Table / Seed helpers ----------------

        Private Sub TryCreateTable(conn As OleDbConnection, tableName As String, sql As String)
            Try
                Dim test = conn.GetSchema("Tables", New String() {Nothing, Nothing, tableName, "TABLE"})
                If test.Rows.Count = 0 Then
                    Using cmd = conn.CreateCommand()
                        cmd.CommandText = sql
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            Catch
            End Try
        End Sub

        Private Sub SeedCategories(conn As OleDbConnection)
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
                ExecuteNonQuery(conn, "INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)",
                                New Tuple(Of String, Object)("@n", item.Item1),
                                New Tuple(Of String, Object)("@t", item.Item2))
            Next
        End Sub

        Private Sub SeedFunds(conn As OleDbConnection)
            Dim count As Integer = CInt(DbScalar(conn, "SELECT COUNT(*) FROM Funds"))
            If count > 0 Then Return
            For Each n In {"กองทุนทั่วไป", "กองทุนกฤติยาธรรม", "กองทุนกู้ภัยวัด", "กองทุนซ่อมแซม"}
                ExecuteNonQuery(conn, "INSERT INTO Funds (FundName) VALUES (@n)", New Tuple(Of String, Object)("@n", n))
            Next
        End Sub

        Private Sub SeedBankAccounts(conn As OleDbConnection)
            Dim count As Integer = CInt(DbScalar(conn, "SELECT COUNT(*) FROM BankAccounts"))
            If count > 0 Then Return
            ExecuteNonQuery(conn, "INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n, @a, @nm)",
                            New Tuple(Of String, Object)("@n", "ธนาคารออมสิน"),
                            New Tuple(Of String, Object)("@a", "-"),
                            New Tuple(Of String, Object)("@nm", "วัดแหลมยาง"))
        End Sub

        Private Sub SeedPositions(conn As OleDbConnection)
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
                    ExecuteNonQuery(conn, "INSERT INTO Positions (PositionName, PositionGroup) VALUES (@n, @g)",
                                    New Tuple(Of String, Object)("@n", pos.Name),
                                    New Tuple(Of String, Object)("@g", pos.Group))
                Else
                    ExecuteNonQuery(conn, "UPDATE Positions SET PositionGroup = @g WHERE PositionName = @n",
                                    New Tuple(Of String, Object)("@g", pos.Group),
                                    New Tuple(Of String, Object)("@n", pos.Name))
                End If
            Next
        End Sub

        Private Sub SeedPersonnel(conn As OleDbConnection)
            Dim samples = New List(Of (Title As String, First As String, Last As String, PosName As String, PType As String)) From {
                ("พระครูสมุห์", "สมชาย", "", "เจ้าอาวาส", "Monk"),
                ("พระมหา", "วิทยา", "", "พระภิกษุ", "Monk"),
                ("นาย", "สมศักดิ์", "ใจดี", "ไวยาวัจกร", "Layperson"),
                ("นาย", "ประเสริฐ", "บุญมี", "เหรัญญิก", "Layperson"),
                ("นางสาว", "พรทิพย์", "สุขใจ", "ผู้ทำบัญชี", "Layperson"),
                ("นาย", "อนันต์", "แสงทอง", "กรรมการวัด", "Layperson")
            }

            For Each item In samples
                Dim fullName = $"{item.Title}{item.First}"
                If Not String.IsNullOrWhiteSpace(item.Last) Then fullName &= $" {item.Last}"

                Dim exists = DbScalar(conn, "SELECT COUNT(*) FROM Personnel WHERE FullName = @fn", New Tuple(Of String, Object)("@fn", fullName))
                If CInt(exists) = 0 Then
                    Dim posId = DbScalar(conn, "SELECT PositionID FROM Positions WHERE PositionName = @pn", New Tuple(Of String, Object)("@pn", item.PosName))
                    ExecuteNonQuery(conn, "INSERT INTO Personnel (Title, FirstName, LastName, FullName, PersonType, PositionID) VALUES (@t, @f, @l, @fn, @pt, @pid)",
                                    New Tuple(Of String, Object)("@t", item.Title),
                                    New Tuple(Of String, Object)("@f", item.First),
                                    New Tuple(Of String, Object)("@l", item.Last),
                                    New Tuple(Of String, Object)("@fn", fullName),
                                    New Tuple(Of String, Object)("@pt", item.PType),
                                    New Tuple(Of String, Object)("@pid", If(posId Is Nothing, DBNull.Value, posId)))
                End If
            Next
        End Sub

        ' ---------------- Public data-access methods ----------------

        Public Function ExecuteNonQuery(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.ExecuteNonQuery
            Dim oledbConn = TryCast(conn, OleDbConnection)
            Using cmd = oledbConn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        Public Function DbScalar(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object Implements IDbProvider.DbScalar
            Dim oledbConn = TryCast(conn, OleDbConnection)
            Using cmd = oledbConn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Dim r = cmd.ExecuteScalar()
                If r Is DBNull.Value Then Return Nothing
                Return r
            End Using
        End Function

        Public Function GetTable(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable Implements IDbProvider.GetTable
            Dim oledbConn = TryCast(conn, OleDbConnection)
            Using cmd = oledbConn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Using da As New OleDbDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Public Function InsertAndGetId(conn As IDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer Implements IDbProvider.InsertAndGetId
            ExecuteNonQuery(conn, sql, params)
            Return CInt(DbScalar(conn, "SELECT @@IDENTITY"))
        End Function

        Public Function NormalizeGregorianDate(value As Date) As Date Implements IDbProvider.NormalizeGregorianDate
            If value.Year > 2400 Then
                Return New Date(value.Year - 543, value.Month, value.Day, value.Hour, value.Minute, value.Second)
            End If
            Return value
        End Function

        Public Function AccessDateLiteral(value As Date) As String Implements IDbProvider.AccessDateLiteral
            Dim safeValue = NormalizeGregorianDate(value)
            Return "#" & safeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) & "#"
        End Function

        Public Function EscapeLikeText(value As String) As String Implements IDbProvider.EscapeLikeText
            If String.IsNullOrEmpty(value) Then Return ""
            Return value.Replace("[", "[[]").Replace("*", "[*]").Replace("?", "[?]").Replace("#", "[#]")
        End Function

        Public Sub BackupDatabase(Optional targetDir As String = Nothing) Implements IDbProvider.BackupDatabase
            Dim src = AppPaths.DatabaseFile
            If Not File.Exists(src) Then Throw New FileNotFoundException("ไม่พบไฟล์ฐานข้อมูลสำหรับสำรองข้อมูล")
            If String.IsNullOrEmpty(targetDir) Then targetDir = AppPaths.BackupFolder
            If Not Directory.Exists(targetDir) Then Directory.CreateDirectory(targetDir)
            Dim fn = $"TempleAccounting_{DateTime.Now:yyyyMMdd_HHmmss}.accdb"
            Dim dst = Path.Combine(targetDir, fn)
            File.Copy(src, dst, True)
        End Sub
    End Class
End Namespace
