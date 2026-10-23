$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$src = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Database\TempleAccounting.accdb'
$dst = 'C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcash_analysis.accdb'
Copy-Item -Path $src -Destination $dst -Force
Write-Host ("copied to " + $dst)

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

# Q0 tran type distribution
RunQuery "SELECT TranType, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions GROUP BY TranType ORDER BY TranType" "Q0 TranType distribution"

# Q1 income ReceiveChannel
RunQuery "SELECT IIF(ReceiveChannel IS NULL,'(NULL)',ReceiveChannel) AS Channel, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Income' GROUP BY IIF(ReceiveChannel IS NULL,'(NULL)',ReceiveChannel)" "Q1 Income by ReceiveChannel"

# Q2 income BankID null/not
RunQuery "SELECT IIF(BankID IS NULL,'NULL','NOT NULL') AS Bank, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Income' GROUP BY IIF(BankID IS NULL,'NULL','NOT NULL')" "Q2 Income by BankID"

# Q3 income cross-tab
RunQuery "SELECT IIF(ReceiveChannel IS NULL,'(NULL)',ReceiveChannel) AS Channel, IIF(BankID IS NULL,'NULL','NOTNULL') AS Bank, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Income' GROUP BY IIF(ReceiveChannel IS NULL,'(NULL)',ReceiveChannel), IIF(BankID IS NULL,'NULL','NOTNULL')" "Q3 Income cross-tab Channel x BankID"

# Q4 expense bank
RunQuery "SELECT IIF(BankID IS NULL,'NULL','NOT NULL') AS Bank, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Expense' GROUP BY IIF(BankID IS NULL,'NULL','NOT NULL')" "Q4 Expense by BankID"

# Q5 transfer from/to bank
RunQuery "SELECT IIF(BankID IS NULL,'NULL','NOTNULL') AS FromBank, IIF(ToBankID IS NULL,'NULL','NOTNULL') AS ToBank, IIF(FundID IS NULL,'NULL','NOTNULL') AS FromFund, IIF(ToFundID IS NULL,'NULL','NOTNULL') AS ToFund, COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Transfer' GROUP BY IIF(BankID IS NULL,'NULL','NOTNULL'), IIF(ToBankID IS NULL,'NULL','NOTNULL'), IIF(FundID IS NULL,'NULL','NOTNULL'), IIF(ToFundID IS NULL,'NULL','NOTNULL')" "Q5 Transfer from/to"

# Q6 opening balance
RunQuery "SELECT Detail, ROUND(Amount,2) AS Amount, TranDate FROM Transactions WHERE TranType='OpeningBalance'" "Q6 OpeningBalance rows"

# Q7 bank accounts
RunQuery "SELECT ID, BankName, AccountNo, AccountName FROM BankAccounts ORDER BY ID" "Q7 BankAccounts"

# Q8 income where channel non-null but bank null (inconsistency)
RunQuery "SELECT COUNT(*) AS N, ROUND(SUM(Amount),2) AS SumAmt FROM Transactions WHERE TranType='Income' AND BankID IS NULL AND ReceiveChannel IS NOT NULL AND ReceiveChannel <> 'Cash'" "Q8 Income: channel!=Cash but BankID NULL"

# Q9 funds
RunQuery "SELECT ID, FundName FROM Funds ORDER BY ID" "Q9 Funds"

# Q10 columns of Transactions
$schema = $c.GetSchema("Columns", @($null,$null,"Transactions",$null))
Write-Host "`n===== Q10 Transactions columns ====="
foreach ($row in $schema.Rows) { Write-Host ($row["COLUMN_NAME"].ToString() + " : " + $row["DATA_TYPE"].ToString()) }

$c.Close()
Write-Host "`nDONE"
