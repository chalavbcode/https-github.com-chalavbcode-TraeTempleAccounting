$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$dst = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcash_analysis.accdb'
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

RunQuery "SELECT t.FundID, f.FundName, COUNT(*) AS N, ROUND(SUM(t.Amount),2) AS SumAmt FROM Transactions t LEFT JOIN Funds f ON t.FundID=f.ID WHERE t.TranType='Income' GROUP BY t.FundID, f.FundName ORDER BY t.FundID" "A1 Income by FundID"

RunQuery "SELECT t.FundID, f.FundName, COUNT(*) AS N, ROUND(SUM(t.Amount),2) AS SumAmt FROM Transactions t LEFT JOIN Funds f ON t.FundID=f.ID WHERE t.TranType='Expense' GROUP BY t.FundID, f.FundName ORDER BY t.FundID" "A2 Expense by FundID"

RunQuery "SELECT * FROM Transactions WHERE TranType='Transfer'" "A3 Transfer full row"

RunQuery "SELECT t.FundID, f.FundName, IIF(t.ReceiveChannel IS NULL,'(NULL)',t.ReceiveChannel) AS Ch, COUNT(*) AS N FROM Transactions t LEFT JOIN Funds f ON t.FundID=f.ID WHERE t.TranType='Income' AND t.ReceiveChannel IS NULL GROUP BY t.FundID, f.FundName" "A4 NULL-channel income by FundID"

RunQuery "SELECT IIF(t.ToFundID IS NULL,'(NULL)',t.ToFundID) AS ToFundID, tf.FundName AS ToFundName, IIF(t.ToBankID IS NULL,'(NULL)',t.ToBankID) AS ToBankID, COUNT(*) AS N, ROUND(SUM(t.Amount),2) AS SumAmt FROM Transactions t LEFT JOIN Funds tf ON t.ToFundID=tf.ID WHERE t.TranType='Transfer' GROUP BY IIF(t.ToFundID IS NULL,'(NULL)',t.ToFundID), tf.FundName, IIF(t.ToBankID IS NULL,'(NULL)',t.ToBankID)" "A5 Transfer destination detail"

RunQuery "SELECT t.BankID, b.BankName, COUNT(*) AS N FROM Transactions t LEFT JOIN BankAccounts b ON t.BankID=b.ID GROUP BY t.BankID, b.BankName" "A6 All transactions by BankID"

$c.Close()
Write-Host "`nDONE"
