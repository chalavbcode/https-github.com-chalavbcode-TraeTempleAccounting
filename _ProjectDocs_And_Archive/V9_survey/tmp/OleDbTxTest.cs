#:package System.Data.OleDb@10.0.10
using System;
using System.Data.OleDb;

var db = args[0];
var cs = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={db};Persist Security Info=False;";

// Test 1: command WITHOUT Transaction assigned while a local tx is pending
try
{
    using var conn = new OleDbConnection(cs);
    conn.Open();
    using var tx = conn.BeginTransaction();
    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "INSERT INTO LocalSettings (KeyName, KeyValue) VALUES ('TXTEST_A','1')";
        cmd.ExecuteNonQuery();  // Transaction NOT assigned (mirrors Db.InsertAndGetId)
    }
    tx.Rollback();
    Console.WriteLine("TEST1 (no Transaction assigned): SUCCESS - OleDb auto-enrolls command");
}
catch (Exception ex)
{
    Console.WriteLine("TEST1 (no Transaction assigned): THROWS -> " + ex.GetType().Name + ": " + ex.Message.Split('\n')[0]);
    Console.WriteLine(ex.StackTrace);
}

// Test 2: same but WITH Transaction assigned (control)
try
{
    using var conn = new OleDbConnection(cs);
    conn.Open();
    using var tx = conn.BeginTransaction();
    using (var cmd = conn.CreateCommand())
    {
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO LocalSettings (KeyName, KeyValue) VALUES ('TXTEST_B','1')";
        cmd.ExecuteNonQuery();
    }
    tx.Rollback();
    Console.WriteLine("TEST2 (Transaction assigned): SUCCESS");
}
catch (Exception ex)
{
    Console.WriteLine("TEST2 (Transaction assigned): THROWS -> " + ex.GetType().Name + ": " + ex.Message.Split('\n')[0]);
}

// cleanup any stray rows from failed runs
try
{
    using var conn = new OleDbConnection(cs);
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "DELETE FROM LocalSettings WHERE KeyName LIKE 'TXTEST%'";
    cmd.ExecuteNonQuery();
    Console.WriteLine("CLEANUP: done");
}
catch (Exception ex) { Console.WriteLine("CLEANUP failed: " + ex.Message); }
