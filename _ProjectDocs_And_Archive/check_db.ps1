
$conn = New-Object System.Data.OleDb.OleDbConnection('Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database\TempleAccounting.accdb')
$conn.Open()
$cmd = $conn.CreateCommand()

$cmd.CommandText = "SELECT COUNT(*) FROM Transactions"
$totalCount = $cmd.ExecuteScalar()

$cmd.CommandText = "SELECT TOP 10 TranDate FROM Transactions ORDER BY TranDate DESC"
$reader = $cmd.ExecuteReader()
$dates = New-Object System.Collections.Generic.List[string]
while ($reader.Read()) {
    $dates.Add($reader.GetValue(0).ToString())
}
$reader.Close()

$conn.Close()

Write-Output "Total Transactions: $totalCount"
Write-Output "Latest Dates: $($dates -join ', ')"
