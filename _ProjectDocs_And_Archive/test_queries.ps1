$conn = New-Object System.Data.OleDb.OleDbConnection
$conn.ConnectionString = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Database\TempleAccounting.accdb'
$conn.Open()

Write-Host "=== TempleSetting Personnel IDs ==="
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT AbbotPersonnelID, WaiyawatPersonnelID, BookkeeperPersonnelID FROM TempleSetting"
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    $abbot = $reader["AbbotPersonnelID"]
    $waiyawat = $reader["WaiyawatPersonnelID"]
    $bookkeeper = $reader["BookkeeperPersonnelID"]
    
    if ($abbot -eq [DBNull]::Value) { $abbot = "NULL" }
    if ($waiyawat -eq [DBNull]::Value) { $waiyawat = "NULL" }
    if ($bookkeeper -eq [DBNull]::Value) { $bookkeeper = "NULL" }
    
    Write-Host "AbbotPersonnelID = $abbot"
    Write-Host "WaiyawatPersonnelID = $waiyawat"
    Write-Host "BookkeeperPersonnelID = $bookkeeper"
}
$reader.Close()

$conn.Close()
