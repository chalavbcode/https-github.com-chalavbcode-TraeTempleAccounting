$ErrorActionPreference = 'Stop'
$src = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Database\TempleAccounting.accdb'
$dst = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcash_step2.accdb'

# 1. Fresh disposable copy (never touch live DB)
Copy-Item -Path $src -Destination $dst -Force
Write-Host ("Copied live DB -> " + $dst)

$cs = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=' + $dst + ';Persist Security Info=False;'
$c = New-Object System.Data.OleDb.OleDbConnection $cs
$c.Open()

function Exec($sql) {
    $cmd = $c.CreateCommand()
    $cmd.CommandText = $sql
    [void]$cmd.ExecuteNonQuery()
}
function Scalar($sql) {
    $cmd = $c.CreateCommand()
    $cmd.CommandText = $sql
    $v = $cmd.ExecuteScalar()
    if ($null -eq $v -or $v -is [System.DBNull]) { return $null }
    return $v
}
function NewReader($sql) {
    $cmd = $c.CreateCommand()
    $cmd.CommandText = $sql
    return ,$cmd.ExecuteReader()
}

# 2. Verify distinct TranType and counts
Write-Host "`n=== Distinct TranType ==="
$r = NewReader "SELECT DISTINCT TranType FROM Transactions ORDER BY TranType"
while ($r.Read()) { Write-Host ("  " + $r.GetValue(0)) }
$r.Close()

Write-Host "`n=== Count by TranType ==="
$r = NewReader "SELECT TranType, COUNT(*), ROUND(SUM(Amount),2) FROM Transactions GROUP BY TranType ORDER BY TranType"
while ($r.Read()) { Write-Host ("  " + $r.GetValue(0) + " | n=" + $r.GetValue(1) + " | sum=" + $r.GetValue(2)) }
$r.Close()

# 3. Create CashCounts table (fresh copy so no pre-existing table)
Exec @"
CREATE TABLE CashCounts (
  ID COUNTER PRIMARY KEY,
  CountDate DATETIME,
  D1 CURRENCY, D2 CURRENCY, D5 CURRENCY, D10 CURRENCY, D20 CURRENCY, D50 CURRENCY, D100 CURRENCY, D500 CURRENCY, D1000 CURRENCY,
  TotalCounted CURRENCY,
  ExpectedBefore CURRENCY,
  Variance CURRENCY,
  [Note] TEXT(255),
  [Source] TEXT(50),
  CreatedAt DATETIME
)
"@
Write-Host "`nCashCounts table created."

# 4. Insert zero-start row + 2 historical counts; verify denomination sum == total
$rows = @(
    @{ Date='2026-02-20'; D=@(0,0,0,0,0,0,0,0,0); Total=0; Source='Cutover, opening cash 0'; Note=$null },
    @{ Date='2026-04-30'; D=@(130,80,30,0,1480,200,8400,5500,11000); Total=26820; Source='Historical manual import'; Note=$null },
    @{ Date='2026-06-09'; D=@(130,80,30,0,1000,450,6700,2000,9000); Total=19390; Source='Historical manual import'; Note=$null }
)

$ids = @()
foreach ($row in $rows) {
    $sum = 0
    foreach ($d in $row.D) { $sum += $d }
    $ok = ($sum -eq $row.Total)
    Write-Host ("  verify " + $row.Date + ": sum(denom)=" + $sum + " total=" + $row.Total + " -> " + $(if($ok){'OK'}else{'MISMATCH'}))
    if (-not $ok) { throw "Denomination sum mismatch for " + $row.Date }

    $noteLit = if ($null -eq $row.Note) { 'NULL' } else { "'" + $row.Note + "'" }
    $sql = "INSERT INTO CashCounts (CountDate,D1,D2,D5,D10,D20,D50,D100,D500,D1000,TotalCounted,ExpectedBefore,Variance,[Note],[Source],CreatedAt) VALUES (" +
           "#" + $row.Date + "#," + ($row.D -join ',') + "," + $row.Total + ",0,0," + $noteLit + ",'" + $row.Source + "',Now())"
    Exec $sql
    $newId = Scalar "SELECT MAX(ID) FROM CashCounts"
    $ids += $newId
    Write-Host ("  inserted ID=" + $newId + " date=" + $row.Date + " total=" + $row.Total + " source='" + $row.Source + "'")
}

# 5. Compute ExpectedBefore via cash-movement sums (per STEP 1 rule)
function CashSumBetween($fromDate, $toDate) {
    $sql = "SELECT SUM(" +
        "IIf(TranType='Income' AND (ReceiveChannel IS NULL OR ReceiveChannel='Cash'), Amount, " +
        "IIf(TranType='Expense' AND BankID IS NULL, -Amount, " +
        "IIf(TranType='Transfer' AND BankID IS NOT NULL AND ToFundID=1, Amount, " +
        "IIf(TranType='Transfer' AND FundID=1 AND ToBankID IS NOT NULL, -Amount, 0))))" +
        ") FROM Transactions WHERE TranDate > #" + $fromDate + "# AND TranDate <= #" + $toDate + "#"
    $v = Scalar $sql
    if ($null -eq $v) { return 0 }
    return [decimal]$v
}

$eb1 = [decimal](CashSumBetween '2026-02-20' '2026-04-30')
$var1 = 26820 - $eb1
Exec ("UPDATE CashCounts SET ExpectedBefore=" + $eb1 + ", Variance=" + $var1 + " WHERE ID=" + $ids[1])
Write-Host ("`n  count 04-30: ExpectedBefore = 0 + cashSum(02-21..04-30) = " + $eb1 + " ; Variance = " + $var1)

$inc2 = [decimal](CashSumBetween '2026-04-30' '2026-06-09')
$eb2 = [decimal](26820 + $inc2)
$var2 = 19390 - $eb2
Exec ("UPDATE CashCounts SET ExpectedBefore=" + $eb2 + ", Variance=" + $var2 + " WHERE ID=" + $ids[2])
Write-Host ("  count 06-09: ExpectedBefore = 26820 + cashSum(05-01..06-09) = " + $eb2 + " ; Variance = " + $var2)

# 6. Show resulting rows
Write-Host "`n=== CashCounts (result) ==="
$r = NewReader "SELECT ID, CountDate, D1,D2,D5,D10,D20,D50,D100,D500,D1000, TotalCounted, ExpectedBefore, Variance, [Source] FROM CashCounts ORDER BY CountDate"
$names = @('ID','CountDate','D1','D2','D5','D10','D20','D50','D100','D500','D1000','TotalCounted','ExpectedBefore','Variance','Source')
Write-Host ($names -join ' | ')
while ($r.Read()) {
    $vals = @()
    for ($i=0; $i -lt $r.FieldCount; $i++) {
        if ($r.IsDBNull($i)) { $vals += '(NULL)' } else { $vals += ([string]$r.GetValue($i)) }
    }
    Write-Host ($vals -join ' | ')
}
$r.Close()

$c.Close()
Write-Host "`nDONE. DB copy at: $dst"
