Option Strict Off
Option Explicit On

Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Globalization

Namespace TempleAccounting
    Public Module Db
        Private _schemaChecked As Boolean = False

        Public ReadOnly Property ConnectionString As String
            Get
                Dim dbPath = AppPaths.DatabaseFile
                Return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;"
            End Get
        End Property

        Public Function OpenConn() As OleDbConnection
            Dim conn As New OleDbConnection(ConnectionString)
            conn.Open()
            Return conn
        End Function

        Public Sub EnsureSchema()
            If _schemaChecked Then Return
            _schemaChecked = True

            If Not File.Exists(AppPaths.DatabaseFile) Then
                Throw New FileNotFoundException("ไม่พบไฟล์ฐานข้อมูล TempleAccounting.accdb")
            End If

            Using conn = OpenConn()
                TryCreateTable(conn, "Province", "CREATE TABLE Province (ProvinceID INTEGER PRIMARY KEY, ProvinceName TEXT(100) NOT NULL)")
                TryCreateTable(conn, "District", "CREATE TABLE District (DistrictID INTEGER PRIMARY KEY, ProvinceID INTEGER NOT NULL, DistrictName TEXT(100) NOT NULL)")
                TryCreateTable(conn, "SubDistrict", "CREATE TABLE SubDistrict (SubDistrictID INTEGER PRIMARY KEY, DistrictID INTEGER NOT NULL, SubDistrictName TEXT(100) NOT NULL, ZipCode TEXT(10))")

                TryCreateTable(conn, "TempleSetting", "CREATE TABLE TempleSetting (ID COUNTER PRIMARY KEY, TempleCode TEXT(20), TempleName TEXT(200), TempleAddress MEMO, Tambon TEXT(100), Amphoe TEXT(100), Province TEXT(100), PostCode TEXT(10), TemplePhone TEXT(30), AbbotName TEXT(100), AbbotOfficeStatus TEXT(50), WaiyawatName TEXT(100), WaiyawatOfficeStatus TEXT(50), BookkeeperName TEXT(100), BookkeeperType TEXT(50), PromptPayName TEXT(100), PromptPayID TEXT(50))")

                TryCreateTable(conn, "Categories", "CREATE TABLE Categories (ID COUNTER PRIMARY KEY, CategoryName TEXT(200) NOT NULL, TranType TEXT(10) NOT NULL)")
                TryCreateTable(conn, "Funds", "CREATE TABLE Funds (ID COUNTER PRIMARY KEY, FundName TEXT(200) NOT NULL)")
                TryCreateTable(conn, "BankAccounts", "CREATE TABLE BankAccounts (ID COUNTER PRIMARY KEY, BankName TEXT(100) NOT NULL, AccountNo TEXT(50), AccountName TEXT(200))")
                TryCreateTable(conn, "Transactions", "CREATE TABLE Transactions (ID COUNTER PRIMARY KEY, TranDate DATETIME NOT NULL, TranType TEXT(10) NOT NULL, CategoryID INTEGER, FundID INTEGER, BankID INTEGER, Detail TEXT(255), Amount CURRENCY NOT NULL, Note MEMO, CreateDate DATETIME DEFAULT Now(), ToFundID INTEGER, ToBankID INTEGER)")

                SeedCategories(conn)
                SeedFunds(conn)
                SeedBankAccounts(conn)
            End Using
        End Sub

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

        Public Function ExecuteNonQuery(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        Public Function DbScalar(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                For Each p In params
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                Next
                Dim r = cmd.ExecuteScalar()
                If r Is DBNull.Value Then Return Nothing
                Return r
            End Using
        End Function

        Public Function ToDecimalOrZero(value As Object) As Decimal
            If value Is Nothing OrElse value Is DBNull.Value Then Return 0D
            Return Convert.ToDecimal(value)
        End Function

        Public Function ToIntOrZero(value As Object) As Integer
            If value Is Nothing OrElse value Is DBNull.Value Then Return 0
            Return Convert.ToInt32(value)
        End Function

        Public Function GetTable(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As DataTable
            Using cmd = conn.CreateCommand()
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

        Public Function InsertAndGetId(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            ExecuteNonQuery(conn, sql, params)
            Return CInt(DbScalar(conn, "SELECT @@IDENTITY"))
        End Function

        Public Function NormalizeGregorianDate(value As Date) As Date
            If value.Year > 2400 Then
                Return New Date(value.Year - 543, value.Month, value.Day, value.Hour, value.Minute, value.Second)
            End If
            Return value
        End Function

        Public Function AccessDateLiteral(value As Date) As String
            Dim safeValue = NormalizeGregorianDate(value)
            Return "#" & safeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) & "#"
        End Function

        Public Function EscapeLikeText(value As String) As String
            If String.IsNullOrEmpty(value) Then Return ""
            Return value.Replace("[", "[[]").Replace("*", "[*]").Replace("?", "[?]").Replace("#", "[#]")
        End Function

        Public Sub BackupDatabase(Optional targetDir As String = Nothing)
            Dim src = AppPaths.DatabaseFile
            If Not File.Exists(src) Then Throw New FileNotFoundException("ไม่พบไฟล์ฐานข้อมูลสำหรับ Backup")
            If String.IsNullOrEmpty(targetDir) Then targetDir = AppPaths.BackupFolder
            If Not Directory.Exists(targetDir) Then Directory.CreateDirectory(targetDir)
            Dim fn = $"TempleAccounting_{DateTime.Now:yyyyMMdd_HHmmss}.accdb"
            Dim dst = Path.Combine(targetDir, fn)
            File.Copy(src, dst, True)
        End Sub
    End Module
End Namespace
