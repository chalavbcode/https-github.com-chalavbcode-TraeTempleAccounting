$cs = 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcopy.accdb;Persist Security Info=False;'
try {
  $c = New-Object System.Data.OleDb.OleDbConnection $cs
  $c.Open()
  $tx = $c.BeginTransaction()

  $del = $c.CreateCommand()
  $del.Transaction = $tx
  $del.CommandText = "DELETE FROM LocalSettings WHERE KeyName = 'ZZTEST'"
  $del.ExecuteNonQuery()

  # INSERT without Transaction assigned — exactly mirrors Db.InsertAndGetId inside a transaction
  $ins = $c.CreateCommand()
  $ins.CommandText = "INSERT INTO LocalSettings (KeyName, KeyValue) VALUES ('ZZTEST','1')"
  try {
    $ins.ExecuteNonQuery()
    Write-Host "INSERT without Transaction: SUCCEEDED (no exception)"
  } catch {
    Write-Host "INSERT without Transaction: THREW ->" $_.Exception.Message
  }

  # Control: same INSERT WITH transaction assigned
  $ins2 = $c.CreateCommand()
  $ins2.Transaction = $tx
  $ins2.CommandText = "INSERT INTO LocalSettings (KeyName, KeyValue) VALUES ('ZZTEST2','1')"
  try {
    $ins2.ExecuteNonQuery()
    Write-Host "INSERT with Transaction: SUCCEEDED"
  } catch {
    Write-Host "INSERT with Transaction: THREW ->" $_.Exception.Message
  }

  $tx.Rollback()
  $c.Close()
  Write-Host "ROLLBACK done"
} catch {
  Write-Host "OUTER FAIL:" $_.Exception.Message
}

# verify no stray rows
try {
  $c2 = New-Object System.Data.OleDb.OleDbConnection $cs
  $c2.Open()
  $q = $c2.CreateCommand()
  $q.CommandText = "SELECT COUNT(*) FROM LocalSettings WHERE KeyName LIKE 'ZZTEST%'"
  Write-Host "stray rows after rollback:" $q.ExecuteScalar()
  $c2.Close()
} catch { Write-Host "verify fail:" $_.Exception.Message }
