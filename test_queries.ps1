$conn = New-Object System.Data.OleDb.OleDbConnection
$conn.ConnectionString = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Database\TempleAccounting.accdb'
$conn.Open()

Write-Host "=== ALL Personnel records ==="
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = "SELECT PersonnelID, Title, FirstName, LastName, FullName FROM Personnel"
$reader = $cmd2.ExecuteReader()
while ($reader.Read()) {
    Write-Host ("ID=" + $reader["PersonnelID"] + " | Title=" + $reader["Title"] + " | First=" + $reader["FirstName"] + " | Last=" + $reader["LastName"] + " | Full=" + $reader["FullName"])
}
$reader.Close()

Write-Host ""
Write-Host "=== TempleSetting - ALL columns ==="
$cmd3 = $conn.CreateCommand()
$cmd3.CommandText = "SELECT * FROM TempleSetting"
$reader3 = $cmd3.ExecuteReader()
if ($reader3.Read()) {
    for ($i = 0; $i -lt $reader3.FieldCount; $i++) {
        $colName = $reader3.GetName($i)
        $colValue = $reader3.GetValue($i)
        if ($colValue -eq [DBNull]::Value) { $colValue = "NULL" }
        Write-Host ($colName + " = " + $colValue)
    }
}
$reader3.Close()

$conn.Close()
