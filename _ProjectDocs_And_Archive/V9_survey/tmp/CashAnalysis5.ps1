$ErrorActionPreference = 'Continue'
$dst = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcash_analysis.accdb'
$out = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\cash_dump.txt'
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("=== Cash analysis dump ===")

$cs = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=' + $dst + ';Persist Security Info=False;'
$c = New-Object System.Data.OleDb.OleDbConnection $cs
$c.Open()

function RunQuery($sql, $label) {
    $script:sb.AppendLine("`n===== " + $label + " =====") | Out-Null
    try {
        $cmd = $c.CreateCommand()
        $cmd.CommandText = $sql
        $r = $cmd.ExecuteReader()
        $colCount = $r.FieldCount
        $names = @()
        for ($i=0; $i -lt $colCount; $i++) { $names += $r.GetName($i) }
        $script:sb.AppendLine(($names -join ' | ')) | Out-Null
        while ($r.Read()) {
            $vals = @()
            for ($i=0; $i -lt $colCount; $i++) {
                if ($r.IsDBNull($i)) { $vals += '(NULL)' } else { $vals += ([string]$r.GetValue($i)) }
            }
            $script:sb.AppendLine(($vals -join ' | ')) | Out-Null
        }
        $r.Close()
    } catch {
        $script:sb.AppendLine("ERROR: " + $_.Exception.Message) | Out-Null
    }
}

RunQuery "SELECT ID, FundName FROM Funds ORDER BY ID" "Funds"
RunQuery "SELECT ID, BankName, AccountNo, AccountName FROM BankAccounts ORDER BY ID" "BankAccounts"

RunQuery "SELECT t.ID, t.TranDate, Year(t.TranDate) AS Yr, t.TranType, t.Amount, t.Detail, t.FundID, f.FundName, t.BankID, b.BankName, t.ToFundID, tf.FundName AS ToFund, t.ToBankID, t.ReceiveChannel FROM ((Transactions t LEFT JOIN Funds f ON t.FundID=f.ID) LEFT JOIN BankAccounts b ON t.BankID=b.ID) LEFT JOIN Funds tf ON t.ToFundID=tf.ID ORDER BY t.TranDate, t.ID" "Q1 All transactions ordered by date"

RunQuery "SELECT Year(TranDate) AS Y, Month(TranDate) AS M, TranType, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions GROUP BY Year(TranDate), Month(TranDate), TranType ORDER BY Year(TranDate), Month(TranDate), TranType" "Q2 Per-month counts and sums"

RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail FROM Transactions WHERE Year(TranDate) > 2400 ORDER BY TranDate" "Q3 Stored year > 2400"

RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail, FundID, BankID, ToFundID, ToBankID, ReceiveChannel FROM Transactions WHERE TranDate >= #2026-02-15# AND TranDate < #2026-03-06# ORDER BY TranDate, ID" "Q4 2026-02-15 .. 2026-03-05"

RunQuery "SELECT ID, TranDate, Year(TranDate) AS Yr, TranType, Amount, Detail, FundID, BankID, ToFundID, ToBankID FROM Transactions WHERE TranType='Transfer' ORDER BY TranDate" "Q5 Transfer rows"

$c.Close()

[System.IO.File]::WriteAllText($out, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host ("WROTE " + $out + " lines=" + $sb.ToString().Split("`n").Count)
