
$conn = New-Object System.Data.OleDb.OleDbConnection('Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database\TempleAccounting.accdb')
$conn.Open()
$cmd = $conn.CreateCommand()

$sqlBase = "WHERE DateSerial(IIF(Year(TranDate) > 2400, Year(TranDate) - 543, Year(TranDate)), Month(TranDate), Day(TranDate)) BETWEEN #2026-07-01# AND #2026-07-31 23:59:59#"

$cmd.CommandText = "SELECT COUNT(*) FROM Transactions $sqlBase"
$count = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT SUM(Amount) FROM Transactions $sqlBase AND TranType='Income'"
$income = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT SUM(Amount) FROM Transactions $sqlBase AND TranType='Expense'"
$expense = $cmd.ExecuteScalar()

$conn.Close()

$cmd.CommandText = "SELECT COUNT(*) FROM Transactions WHERE TranDate BETWEEN #2026-07-01# AND #2026-07-31 23:59:59#"
$rawCount = $cmd.ExecuteScalar()
Write-Output "RESULT: Count=$count | RawCount=$rawCount | Income=$income | Expense=$expense"
