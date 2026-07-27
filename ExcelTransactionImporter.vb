Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text

Namespace TempleAccounting
    Public Class TransactionImportResult
        Public Property SourceFile As String
        Public Property InsertedCount As Integer
        Public Property DuplicateCount As Integer
        Public Property BlankRowCount As Integer
        Public Property ErrorCount As Integer
        Public Property CreatedCategoryCount As Integer
        Public Property CreatedFundCount As Integer
        Public Property CreatedBankCount As Integer
        Public ReadOnly Property ErrorMessages As New List(Of String)()

        Public Function BuildSummaryMessage(tranTypeLabel As String) As String
            Dim sb As New StringBuilder()
            sb.AppendLine("นำเข้าข้อมูล" & tranTypeLabel & "เรียบร้อย")
            sb.AppendLine("ไฟล์: " & SourceFile)
            sb.AppendLine("เพิ่มสำเร็จ: " & InsertedCount.ToString("n0") & " รายการ")
            sb.AppendLine("ข้ามรายการซ้ำ: " & DuplicateCount.ToString("n0") & " รายการ")
            sb.AppendLine("ข้ามแถวว่าง: " & BlankRowCount.ToString("n0") & " แถว")
            sb.AppendLine("สร้างประเภทใหม่: " & CreatedCategoryCount.ToString("n0"))
            sb.AppendLine("สร้างกองทุนใหม่: " & CreatedFundCount.ToString("n0"))
            sb.AppendLine("สร้างบัญชีธนาคารใหม่: " & CreatedBankCount.ToString("n0"))
            sb.AppendLine("แถวที่ผิดพลาด: " & ErrorCount.ToString("n0") & " แถว")
            If ErrorMessages.Count > 0 Then
                sb.AppendLine()
                sb.AppendLine("ตัวอย่างปัญหา:")
                For Each msg In ErrorMessages.Take(8)
                    sb.AppendLine("- " & msg)
                Next
                If ErrorMessages.Count > 8 Then
                    sb.AppendLine("- ... และอีก " & (ErrorMessages.Count - 8).ToString("n0") & " แถว")
                End If
            End If
            Return sb.ToString().Trim()
        End Function
    End Class

    Public Module ExcelTransactionImporter
        Private ReadOnly DateAliases As String() = {"วันที่", "date", "trandate", "transactiondate", "วันทำรายการ"}
        Private ReadOnly DetailAliases As String() = {"รายการ", "detail", "description", "รายละเอียด", "desc"}
        Private ReadOnly AmountAliases As String() = {"จำนวนเงิน", "amount", "ยอดเงิน", "ยอด", "money"}
        Private ReadOnly CategoryAliases As String() = {"ประเภท", "category", "categoryname", "ประเภทรายการ"}
        Private ReadOnly FundAliases As String() = {"กองทุน", "fund", "fundname"}
        Private ReadOnly BankAliases As String() = {"ธนาคาร", "bank", "bankname", "bankinfo", "บัญชีธนาคาร"}
        Private ReadOnly NoteAliases As String() = {"หมายเหตุ", "note", "remark", "remarks"}

        Public Function ImportTransactionsFromExcel(filePath As String,
                                                    tranType As String,
                                                    defaultCategoryId As Integer,
                                                    defaultFundId As Integer,
                                                    defaultBankId As Integer?) As TransactionImportResult
            If String.IsNullOrWhiteSpace(filePath) OrElse Not File.Exists(filePath) Then
                Throw New FileNotFoundException("ไม่พบไฟล์ Excel ที่ต้องการนำเข้า")
            End If

            Dim sourceTable = LoadExcelSourceTable(filePath, tranType)
            Dim dateCol = FindColumn(sourceTable, DateAliases)
            Dim detailCol = FindColumn(sourceTable, DetailAliases)
            Dim amountCol = FindColumn(sourceTable, AmountAliases)
            Dim categoryCol = FindColumn(sourceTable, CategoryAliases)
            Dim fundCol = FindColumn(sourceTable, FundAliases)
            Dim bankCol = FindColumn(sourceTable, BankAliases)
            Dim noteCol = FindColumn(sourceTable, NoteAliases)

            Dim missing As New List(Of String)()
            If String.IsNullOrEmpty(dateCol) Then missing.Add("วันที่")
            If String.IsNullOrEmpty(detailCol) Then missing.Add("รายการ")
            If String.IsNullOrEmpty(amountCol) Then missing.Add("จำนวนเงิน")
            If missing.Count > 0 Then
                Throw New ApplicationException("ไฟล์นำเข้ายังไม่ครบคอลัมน์หลัก: " & String.Join(", ", missing) &
                                               vbCrLf & "คอลัมน์ที่รองรับเช่น วันที่/Date, รายการ/Detail, จำนวนเงิน/Amount")
            End If

            Db.EnsureSchema()
            Db.BackupDatabase()

            Dim result As New TransactionImportResult() With {.SourceFile = filePath}
            Using conn = Db.OpenConn()
                Dim categoryMap = LoadNameMap(conn, "SELECT ID, CategoryName FROM Categories WHERE TranType=@t ORDER BY ID", New Tuple(Of String, Object)("@t", tranType))
                Dim fundMap = LoadNameMap(conn, "SELECT ID, FundName FROM Funds ORDER BY ID")
                Dim bankMap = LoadNameMap(conn, "SELECT ID, BankName FROM BankAccounts ORDER BY ID")

                For rowIndex As Integer = 0 To sourceTable.Rows.Count - 1
                    Dim row = sourceTable.Rows(rowIndex)
                    Dim rowNumber = rowIndex + 2
                    Try
                        If IsBlankImportRow(row, {dateCol, detailCol, amountCol, categoryCol, fundCol, bankCol, noteCol}) Then
                            result.BlankRowCount += 1
                            Continue For
                        End If

                        Dim tranDate As Date
                        If Not TryParseImportDate(row(dateCol), tranDate) Then
                            Throw New ApplicationException("วันที่ไม่ถูกต้อง")
                        End If

                        Dim detail = SafeCellText(row, detailCol).Trim()
                        If String.IsNullOrWhiteSpace(detail) Then
                            Throw New ApplicationException("ไม่มีรายละเอียดรายการ")
                        End If

                        Dim amount As Decimal
                        If Not TryParseImportAmount(row(amountCol), amount) OrElse amount <= 0D Then
                            Throw New ApplicationException("จำนวนเงินไม่ถูกต้อง")
                        End If

                        Dim categoryId = ResolveCategoryId(conn, tranType, SafeCellText(row, categoryCol), defaultCategoryId, categoryMap, result)
                        Dim fundId = ResolveFundId(conn, SafeCellText(row, fundCol), defaultFundId, fundMap, result)
                        Dim bankId = ResolveBankId(conn, SafeCellText(row, bankCol), defaultBankId, bankMap, result)
                        Dim note = SafeCellText(row, noteCol).Trim()
                        Dim normalizedDate = Db.NormalizeGregorianDate(tranDate.Date)
                        Dim bankCompareValue As Integer = If(TypeOf bankId Is DBNull, 0, Convert.ToInt32(bankId))

                        Dim exists = Db.ToIntOrZero(Db.DbScalar(conn,
                            "SELECT COUNT(*) FROM Transactions " &
                            "WHERE TranType=@t AND DateValue(TranDate)=" & Db.AccessDateLiteral(normalizedDate) &
                            " AND CategoryID=@c AND FundID=@f AND IIF(BankID IS NULL,0,BankID)=@b AND [Detail]=@d AND Amount=@a AND IIF([Note] IS NULL,'',[Note])=@n",
                            New Tuple(Of String, Object)("@t", tranType),
                            New Tuple(Of String, Object)("@c", categoryId),
                            New Tuple(Of String, Object)("@f", fundId),
                            New Tuple(Of String, Object)("@b", bankCompareValue),
                            New Tuple(Of String, Object)("@d", detail),
                            New Tuple(Of String, Object)("@a", amount),
                            New Tuple(Of String, Object)("@n", note)))
                        If exists > 0 Then
                            result.DuplicateCount += 1
                            Continue For
                        End If

                        Db.ExecuteNonQuery(conn,
                            "INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note]) VALUES (" &
                            Db.AccessDateLiteral(normalizedDate) & ", @t, @c, @f, @b, @d, @a, @n)",
                            New Tuple(Of String, Object)("@t", tranType),
                            New Tuple(Of String, Object)("@c", categoryId),
                            New Tuple(Of String, Object)("@f", fundId),
                            New Tuple(Of String, Object)("@b", bankId),
                            New Tuple(Of String, Object)("@d", detail),
                            New Tuple(Of String, Object)("@a", amount),
                            New Tuple(Of String, Object)("@n", note))
                        result.InsertedCount += 1
                    Catch ex As Exception
                        result.ErrorCount += 1
                        result.ErrorMessages.Add("แถว " & rowNumber.ToString() & ": " & ex.Message)
                    End Try
                Next
            End Using

            Return result
        End Function

        Private Function LoadExcelSourceTable(filePath As String, tranType As String) As DataTable
            Dim ext = Path.GetExtension(filePath).ToLowerInvariant()
            Dim excelConnStr As String
            Select Case ext
                Case ".xlsx", ".xlsm", ".xlsb"
                    excelConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1"";"
                Case ".xls"
                    excelConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties=""Excel 8.0;HDR=YES;IMEX=1"";"
                Case Else
                    Throw New ApplicationException("รองรับเฉพาะไฟล์ Excel นามสกุล .xlsx, .xlsm, .xlsb, .xls")
            End Select

            Using excelConn As New OleDbConnection(excelConnStr)
                excelConn.Open()
                Dim sheetNames = GetWorksheetNames(excelConn)
                If sheetNames.Count = 0 Then
                    Throw New ApplicationException("ไม่พบชีตข้อมูลในไฟล์ Excel")
                End If

                Dim orderedSheetNames = OrderWorksheetNames(sheetNames, tranType)
                For Each sheetName In orderedSheetNames
                    Dim dt = LoadWorksheet(excelConn, sheetName)
                    If HasRequiredColumns(dt) Then
                        Return dt
                    End If
                Next
            End Using

            Throw New ApplicationException("นำเข้าข้อมูลไม่สำเร็จ: ไม่พบชีตที่มีคอลัมน์ วันที่, รายการ, จำนวนเงิน" &
                                           vbCrLf & "ถ้าใช้ไฟล์ template ให้เลือกชีต 'รายรับ' หรือ 'รายจ่าย'")
        End Function

        Private Function LoadWorksheet(excelConn As OleDbConnection, sheetName As String) As DataTable
            Using cmd = excelConn.CreateCommand()
                cmd.CommandText = "SELECT * FROM [" & sheetName & "]"
                Using da As New OleDbDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Private Function GetWorksheetNames(excelConn As OleDbConnection) As List(Of String)
            Dim names As New List(Of String)()
            Dim schema = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            If schema Is Nothing Then Return names

            For Each row As DataRow In schema.Rows
                Dim tableName = row("TABLE_NAME").ToString()
                If tableName.EndsWith("$") OrElse tableName.EndsWith("$'") Then
                    names.Add(tableName.Trim("'"c))
                End If
            Next

            Return names
        End Function

        Private Function OrderWorksheetNames(sheetNames As IEnumerable(Of String), tranType As String) As List(Of String)
            Dim preferredNames As New List(Of String)()
            If String.Equals(tranType, "Income", StringComparison.OrdinalIgnoreCase) Then
                preferredNames.AddRange({"รายรับ$", "income$", "receipts$", "incomes$"})
            ElseIf String.Equals(tranType, "Expense", StringComparison.OrdinalIgnoreCase) Then
                preferredNames.AddRange({"รายจ่าย$", "expense$", "expenses$", "payments$"})
            End If

            Dim ordered As New List(Of String)()
            For Each preferred In preferredNames
                Dim match = sheetNames.FirstOrDefault(Function(name) String.Equals(name, preferred, StringComparison.OrdinalIgnoreCase))
                If Not String.IsNullOrWhiteSpace(match) AndAlso Not ordered.Contains(match, StringComparer.OrdinalIgnoreCase) Then
                    ordered.Add(match)
                End If
            Next

            For Each name In sheetNames
                If Not ordered.Contains(name, StringComparer.OrdinalIgnoreCase) Then
                    ordered.Add(name)
                End If
            Next

            Return ordered
        End Function

        Private Function HasRequiredColumns(sourceTable As DataTable) As Boolean
            Return Not String.IsNullOrEmpty(FindColumn(sourceTable, DateAliases)) AndAlso
                   Not String.IsNullOrEmpty(FindColumn(sourceTable, DetailAliases)) AndAlso
                   Not String.IsNullOrEmpty(FindColumn(sourceTable, AmountAliases))
        End Function

        Private Function LoadNameMap(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            Dim dt = Db.GetTable(conn, sql, params)
            For Each row As DataRow In dt.Rows
                Dim nameValue = row(1).ToString().Trim()
                If Not String.IsNullOrWhiteSpace(nameValue) AndAlso Not map.ContainsKey(nameValue) Then
                    map(nameValue) = Convert.ToInt32(row(0))
                End If
            Next
            Return map
        End Function

        Private Function FindColumn(sourceTable As DataTable, aliases As IEnumerable(Of String)) As String
            Dim normalizedAliases = New HashSet(Of String)(aliases.Select(Function(a) NormalizeColumnKey(a)), StringComparer.OrdinalIgnoreCase)
            For Each col As DataColumn In sourceTable.Columns
                If normalizedAliases.Contains(NormalizeColumnKey(col.ColumnName)) Then
                    Return col.ColumnName
                End If
            Next
            Return ""
        End Function

        Private Function NormalizeColumnKey(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then Return ""
            Return New String(value.Trim().ToLowerInvariant().Where(Function(ch) Char.IsLetterOrDigit(ch)).ToArray())
        End Function

        Private Function SafeCellText(row As DataRow, columnName As String) As String
            If String.IsNullOrWhiteSpace(columnName) Then Return ""
            If row Is Nothing OrElse row.Table Is Nothing OrElse Not row.Table.Columns.Contains(columnName) Then Return ""
            If row.IsNull(columnName) Then Return ""
            Return Convert.ToString(row(columnName)).Replace(ChrW(160), " ").Trim()
        End Function

        Private Function IsBlankImportRow(row As DataRow, columnNames As IEnumerable(Of String)) As Boolean
            For Each columnName In columnNames
                If String.IsNullOrWhiteSpace(columnName) Then Continue For
                If row.Table.Columns.Contains(columnName) AndAlso Not String.IsNullOrWhiteSpace(SafeCellText(row, columnName)) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Function TryParseImportDate(value As Object, ByRef parsedDate As Date) As Boolean
            parsedDate = Date.MinValue
            If value Is Nothing OrElse value Is DBNull.Value Then Return False

            If TypeOf value Is Date Then
                parsedDate = Db.NormalizeGregorianDate(CDate(value))
                Return True
            End If

            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                parsedDate = Db.NormalizeGregorianDate(DateTime.FromOADate(Convert.ToDouble(value)))
                Return True
            End If

            Dim text = Convert.ToString(value).Replace(ChrW(160), " ").Trim()
            If String.IsNullOrWhiteSpace(text) Then Return False

            Dim oaValue As Double
            If Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, oaValue) AndAlso oaValue > 20000 Then
                parsedDate = Db.NormalizeGregorianDate(DateTime.FromOADate(oaValue))
                Return True
            End If

            Dim cultures = {
                CultureInfo.CurrentCulture,
                New CultureInfo("th-TH"),
                CultureInfo.InvariantCulture,
                New CultureInfo("en-US")
            }
            For Each culture In cultures
                Dim tempDate As DateTime
                If DateTime.TryParse(text, culture, DateTimeStyles.AllowWhiteSpaces, tempDate) Then
                    parsedDate = Db.NormalizeGregorianDate(tempDate)
                    Return True
                End If
            Next

            Return False
        End Function

        Private Function TryParseImportAmount(value As Object, ByRef amount As Decimal) As Boolean
            amount = 0D
            If value Is Nothing OrElse value Is DBNull.Value Then Return False

            If TypeOf value Is Decimal OrElse TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                amount = Convert.ToDecimal(value)
                Return True
            End If

            Dim text = Convert.ToString(value).Replace(ChrW(160), " ").Replace(",", "").Replace("บาท", "").Trim()
            If String.IsNullOrWhiteSpace(text) Then Return False

            If Decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, amount) Then Return True
            If Decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, amount) Then Return True
            Return False
        End Function

        Private Function ResolveCategoryId(conn As OleDbConnection,
                                           tranType As String,
                                           categoryName As String,
                                           defaultCategoryId As Integer,
                                           categoryMap As Dictionary(Of String, Integer),
                                           result As TransactionImportResult) As Integer
            Dim useName = categoryName.Trim()
            If String.IsNullOrWhiteSpace(useName) Then Return defaultCategoryId
            If categoryMap.ContainsKey(useName) Then Return categoryMap(useName)

            Dim newId = Db.InsertAndGetId(conn,
                "INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)",
                New Tuple(Of String, Object)("@n", useName),
                New Tuple(Of String, Object)("@t", tranType))
            categoryMap(useName) = newId
            result.CreatedCategoryCount += 1
            Return newId
        End Function

        Private Function ResolveFundId(conn As OleDbConnection,
                                       sourceName As String,
                                       defaultId As Integer,
                                       cache As Dictionary(Of String, Integer),
                                       result As TransactionImportResult) As Integer
            Dim useName = sourceName.Trim()
            If String.IsNullOrWhiteSpace(useName) Then Return defaultId
            If cache.ContainsKey(useName) Then Return cache(useName)

            Dim newId = Db.InsertAndGetId(conn,
                "INSERT INTO Funds (FundName) VALUES (@n)",
                New Tuple(Of String, Object)("@n", useName))
            cache(useName) = newId
            result.CreatedFundCount += 1
            Return newId
        End Function

        Private Function ResolveBankId(conn As OleDbConnection,
                                       bankName As String,
                                       defaultBankId As Integer?,
                                       bankMap As Dictionary(Of String, Integer),
                                       result As TransactionImportResult) As Object
            Dim useName = bankName.Trim()
            If String.IsNullOrWhiteSpace(useName) Then
                If defaultBankId.HasValue Then Return defaultBankId.Value
                Return DBNull.Value
            End If
            If bankMap.ContainsKey(useName) Then Return bankMap(useName)

            Dim newId = Db.InsertAndGetId(conn,
                "INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n, @a, @nm)",
                New Tuple(Of String, Object)("@n", useName),
                New Tuple(Of String, Object)("@a", DBNull.Value),
                New Tuple(Of String, Object)("@nm", DBNull.Value))
            bankMap(useName) = newId
            result.CreatedBankCount += 1
            Return newId
        End Function
    End Module
End Namespace
