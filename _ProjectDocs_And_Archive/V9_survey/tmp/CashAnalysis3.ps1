$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$src = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Database\TempleAccounting.accdb'
$dst = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcash_analysis.accdb'
Copy-Item -Path $src -Destination $dst -Force
Write-Host ("copied -> " + $dst)

$cs = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=' + $dst + ';Persist Security Info=False;'
$c = New-Object System.Data.OleDb.OleDbConnection $cs
$c.Open()

function RunQuery($sql, $label) {
    Write-Host ("`n===== " + $label + " =====")
    $cmd = $c.CreateCommand()
    $cmd.CommandText = $sql
    $r = $cmd.ExecuteReader()
    $colCount = $r.FieldCount
    $names = @()
    for ($i=0; $i -lt $colCount; $i++) { $names += $r.GetName($i) }
    Write-Host ($names -join ' | ')
    while ($r.Read()) {
        $vals = @()
        for ($i=0; $i -lt $colCount; $i++) {
            if ($r.IsDBNull($i)) { $vals += '(NULL)' } else { $vals += ([string]$r.GetValue($i)) }
        }
        Write-Host ($vals -join ' | ')
    }
    $r.Close()
}

# Q0 column list
RunQuery "SELECT * FROM Transactions WHERE 1=0" "Q0 Transactions columns"

# Q1 full dump ordered by date
RunQuery "SELECT t.ID, t.TranDate, Year(t.TranDate) AS Yr, t.TranType, t.Amount, t.Detail, t.FundID, f.FundName, t.BankID, b.BankName, t.ToFundID, tf.FundName AS ToFund, t.ToBankID, t.ReceiveChannel, t.Note FROM Transactions t LEFT JOIN Funds f ON t.FundID=f.ID LEFT JOIN BankAccounts b ON t.BankID=b.ID LEFT JOIN Funds tf ON t.ToFundID=tf.ID ORDER BY t.TranDate, t.ID" "Q1 All transactions ordered by date"

# Q2 per-month aggregates
RunQuery "SELECT Year(TranDate) AS Y, Month(TranDate) AS M, TranType, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions GROUP BY Year(TranDate), Month(TranDate), TranType ORDER BY Y, M, TranType" "Q2 Per-month counts and sums"

# Q3 rows with stored year > 2400 (Buddhist-era)
RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail FROM Transactions WHERE Year(TranDate) > 2400 ORDER BY TranDate" "Q3 Stored year > 2400"

# Q4 explicit window 2026-02-15 .. 2026-03-05
RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail, FundID, BankID, ToFundID, ToBankID, ReceiveChannel FROM Transactions WHERE TranDate >= #2026-02-15# AND TranDate < #2026-03-06# ORDER BY TranDate, ID" "Q4 2026-02-15 .. 2026-03-05"

# Q5 the transfer rows
RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail, FundID, BankID, ToFundID, ToBankID FROM Transactions WHERE TranType='Transfer' ORDER BY TranDate" "Q5 Transfer rows"

$c.Close()
Write-Host "`nDONE"
