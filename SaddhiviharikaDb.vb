Option Strict Off
Option Explicit On

Imports System.IO
Imports Microsoft.Data.Sqlite

Namespace TempleAccounting
    ''' <summary>
    ''' SQLite database manager for "คลังประวัติและออกรายงานบัญชีสัทธิวิหาริก" (Saddhiviharika ordination registry).
    ''' Creates the Saddhiviharika.db file, builds tables + indexes, and seeds default data for อำเภอเมืองพิจิตร จังหวัดพิจิตร.
    ''' </summary>
    Public Class SaddhiviharikaDb
        Private Shared _schemaChecked As Boolean = False
        Private Shared ReadOnly _lock As New Object()

        ''' <summary>Returns a new open SqliteConnection.</summary>
        Public Shared Function OpenConn() As SqliteConnection
            Dim conn As New SqliteConnection($"Data Source={AppPaths.SaddhiviharikaDbFile}")
            conn.Open()
            ' Enable foreign key enforcement
            Using cmd = conn.CreateCommand()
                cmd.CommandText = "PRAGMA foreign_keys = ON;"
                cmd.ExecuteNonQuery()
            End Using
            Return conn
        End Function

        ''' <summary>Creates tables and indexes if they do not exist yet. Idempotent.</summary>
        Public Shared Sub EnsureSchema()
            SyncLock _lock
                If _schemaChecked Then Return
                Dim dbPath = AppPaths.SaddhiviharikaDbFile
                Dim dbDir = Path.GetDirectoryName(dbPath)
                If Not Directory.Exists(dbDir) Then Directory.CreateDirectory(dbDir)

                Using conn = OpenConn()
                    Using cmd = conn.CreateCommand()
                        ' ---------- temples ----------
                        cmd.CommandText = "
                            CREATE TABLE IF NOT EXISTS temples (
                                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                                temple_name TEXT    NOT NULL,
                                district    TEXT,
                                province    TEXT
                            );"
                        cmd.ExecuteNonQuery()

                        ' ---------- monks ----------
                        cmd.CommandText = "
                            CREATE TABLE IF NOT EXISTS monks (
                                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                                monk_name   TEXT    NOT NULL,
                                temple_id   INTEGER REFERENCES temples(id)
                            );"
                        cmd.ExecuteNonQuery()

                        ' ---------- saddhiviharika ----------
                        cmd.CommandText = "
                            CREATE TABLE IF NOT EXISTS saddhiviharika (
                                id                    INTEGER PRIMARY KEY AUTOINCREMENT,
                                seq_no                TEXT,
                                first_name            TEXT,
                                last_name             TEXT,
                                dhamma_name           TEXT,
                                birth_date            TEXT,
                                ordination_date       TEXT,
                                age                   INTEGER,
                                ordination_temple_id  INTEGER REFERENCES temples(id),
                                residence_temple_id   INTEGER REFERENCES temples(id),
                                kammavaca_monk_id     INTEGER REFERENCES monks(id),
                                anusavana_monk_id     INTEGER REFERENCES monks(id),
                                former_occupation     TEXT,
                                remarks               TEXT
                            );"
                        cmd.ExecuteNonQuery()

                        ' ---------- Indexes ----------
                        CreateIndexIfNotExists(cmd, "idx_temples_name", "temples", "temple_name")
                        CreateIndexIfNotExists(cmd, "idx_monks_name", "monks", "monk_name")
                        CreateIndexIfNotExists(cmd, "idx_saddhi_first_name", "saddhiviharika", "first_name")
                        CreateIndexIfNotExists(cmd, "idx_saddhi_last_name", "saddhiviharika", "last_name")
                        CreateIndexIfNotExists(cmd, "idx_saddhi_dhamma_name", "saddhiviharika", "dhamma_name")
                        CreateIndexIfNotExists(cmd, "idx_saddhi_ordination_date", "saddhiviharika", "ordination_date")
                    End Using

                    ' Seed default data
                    SeedDefaultData(conn)
                End Using
                _schemaChecked = True
            End SyncLock
        End Sub

        ''' <summary>Creates an index if it does not already exist.</summary>
        Private Shared Sub CreateIndexIfNotExists(cmd As SqliteCommand, indexName As String, tableName As String, columnName As String)
            cmd.CommandText = $"CREATE INDEX IF NOT EXISTS {indexName} ON {tableName} ({columnName});"
            cmd.ExecuteNonQuery()
        End Sub

        ''' <summary>
        ''' Seeds default data for อำเภอเมืองพิจิตร จังหวัดพิจิตร.
        ''' Skips if temples table already has records.
        ''' </summary>
        Private Shared Sub SeedDefaultData(conn As SqliteConnection)
            Using cmd = conn.CreateCommand()
                ' Check if data already exists
                cmd.CommandText = "SELECT COUNT(*) FROM temples;"
                Dim existingCount As Long = CLng(cmd.ExecuteScalar())
                If existingCount > 0 Then Return

                ' ----- Seed temples in อำเภอเมืองพิจิตร จังหวัดพิจิตร -----
                Dim temples As New List(Of (Name As String, District As String, Province As String)) From {
                    ("วัดท่าหลวง (พระอารามหลวง)", "อำเภอเมืองพิจิตร", "จังหวัดพิจิตร"),
                    ("วัดโรงช้าง", "อำเภอเมืองพิจิตร", "จังหวัดพิจิตร"),
                    ("วัดมหาธาตุ", "อำเภอเมืองพิจิตร", "จังหวัดพิจิตร"),
                    ("วัดนครชุม", "อำเภอเมืองพิจิตร", "จังหวัดพิจิตร"),
                    ("วัดราชดัด", "อำเภอเมืองพิจิตร", "จังหวัดพิจิตร")
                }

                Dim templeIds As New Dictionary(Of String, Integer)
                For Each t In temples
                    cmd.CommandText = "INSERT INTO temples (temple_name, district, province) VALUES (@n, @d, @p);"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@n", t.Name)
                    cmd.Parameters.AddWithValue("@d", t.District)
                    cmd.Parameters.AddWithValue("@p", t.Province)
                    cmd.ExecuteNonQuery()

                    cmd.CommandText = "SELECT last_insert_rowid();"
                    templeIds(t.Name) = CInt(CLng(cmd.ExecuteScalar()))
                Next

                ' ----- Seed monks (พระคู่สวดตัวอย่าง) -----
                Dim monks As New List(Of (Name As String, TempleName As String)) From {
                    ("พระครูพิจิตรวรเวทย์", "วัดท่าหลวง (พระอารามหลวง)"),
                    ("พระมหาสมชาย ฐานวโร", "วัดท่าหลวง (พระอารามหลวง)"),
                    ("พระครูวิบูลย์ธรรมคุณ", "วัดโรงช้าง"),
                    ("พระอธิการบุญมี สุจิตฺโต", "วัดมหาธาตุ"),
                    ("พระปลัดสุรพล กิตฺติสาโร", "วัดนครชุม"),
                    ("พระครูสังฆรักษ์ประจักษ์", "วัดราชดัด")
                }

                Dim monkIds As New Dictionary(Of String, Integer)
                For Each m In monks
                    cmd.CommandText = "INSERT INTO monks (monk_name, temple_id) VALUES (@n, @tid);"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@n", m.Name)
                    cmd.Parameters.AddWithValue("@tid", templeIds(m.TempleName))
                    cmd.ExecuteNonQuery()

                    cmd.CommandText = "SELECT last_insert_rowid();"
                    monkIds(m.Name) = CInt(CLng(cmd.ExecuteScalar()))
                Next

                ' ----- Seed a sample saddhiviharika record -----
                cmd.CommandText = "
                    INSERT INTO saddhiviharika (
                        seq_no, first_name, last_name, dhamma_name,
                        birth_date, ordination_date, age,
                        ordination_temple_id, residence_temple_id,
                        kammavaca_monk_id, anusavana_monk_id,
                        former_occupation, remarks
                    ) VALUES (
                        @seq, @fn, @ln, @dn,
                        @bd, @od, @age,
                        @otid, @rtid,
                        @kmid, @amid,
                        @fo, @rm
                    );"
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@seq", "001")
                cmd.Parameters.AddWithValue("@fn", "นายสมหมาย")
                cmd.Parameters.AddWithValue("@ln", "รักสงบ")
                cmd.Parameters.AddWithValue("@dn", "สุภทฺโท")
                cmd.Parameters.AddWithValue("@bd", "2540-03-15")
                cmd.Parameters.AddWithValue("@od", "2568-07-20")
                cmd.Parameters.AddWithValue("@age", 28)
                cmd.Parameters.AddWithValue("@otid", templeIds("วัดท่าหลวง (พระอารามหลวง)"))
                cmd.Parameters.AddWithValue("@rtid", templeIds("วัดท่าหลวง (พระอารามหลวง)"))
                cmd.Parameters.AddWithValue("@kmid", monkIds("พระครูพิจิตรวรเวทย์"))
                cmd.Parameters.AddWithValue("@amid", monkIds("พระมหาสมชาย ฐานวโร"))
                cmd.Parameters.AddWithValue("@fo", "พนักงานบริษัท")
                cmd.Parameters.AddWithValue("@rm", "จบปริญญาตรี คณะวิศวกรรมศาสตร์")
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        ' =====================================================================
        ' Public helper methods for CRUD operations
        ' =====================================================================

        ''' <summary>Returns all temples as a list of Tuples.</summary>
        Public Shared Function GetAllTemples() As List(Of (Id As Integer, Name As String, District As String, Province As String))
            Dim result As New List(Of (Integer, String, String, String))
            Using conn = OpenConn()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "SELECT id, temple_name, district, province FROM temples ORDER BY temple_name;"
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add((reader.GetInt32(0), reader.GetString(1),
                                        If(reader.IsDBNull(2), "", reader.GetString(2)),
                                        If(reader.IsDBNull(3), "", reader.GetString(3))))
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        ''' <summary>Returns all monks with their temple names.</summary>
        Public Shared Function GetAllMonks() As List(Of (Id As Integer, Name As String, TempleName As String))
            Dim result As New List(Of (Integer, String, String))
            Using conn = OpenConn()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "
                        SELECT m.id, m.monk_name, COALESCE(t.temple_name, '') AS temple_name
                        FROM monks m
                        LEFT JOIN temples t ON m.temple_id = t.id
                        ORDER BY m.monk_name;"
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)))
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        ''' <summary>Inserts a new saddhiviharika record and returns the generated id.</summary>
        Public Shared Function InsertSaddhiviharika(
                                seqNo As String, firstName As String, lastName As String, dhammaName As String,
                                birthDate As String, ordinationDate As String, age As Integer,
                                ordinationTempleId As Integer, residenceTempleId As Integer,
                                kammavacaMonkId As Integer, anusavanaMonkId As Integer,
                                formerOccupation As String, remarks As String) As Integer
            Using conn = OpenConn()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "
                        INSERT INTO saddhiviharika (
                            seq_no, first_name, last_name, dhamma_name,
                            birth_date, ordination_date, age,
                            ordination_temple_id, residence_temple_id,
                            kammavaca_monk_id, anusavana_monk_id,
                            former_occupation, remarks
                        ) VALUES (
                            @seq, @fn, @ln, @dn,
                            @bd, @od, @age,
                            @otid, @rtid,
                            @kmid, @amid,
                            @fo, @rm
                        );
                        SELECT last_insert_rowid();"
                    cmd.Parameters.AddWithValue("@seq", If(seqNo, DBNull.Value))
                    cmd.Parameters.AddWithValue("@fn", If(firstName, DBNull.Value))
                    cmd.Parameters.AddWithValue("@ln", If(lastName, DBNull.Value))
                    cmd.Parameters.AddWithValue("@dn", If(dhammaName, DBNull.Value))
                    cmd.Parameters.AddWithValue("@bd", If(birthDate, DBNull.Value))
                    cmd.Parameters.AddWithValue("@od", If(ordinationDate, DBNull.Value))
                    cmd.Parameters.AddWithValue("@age", age)
                    cmd.Parameters.AddWithValue("@otid", ordinationTempleId)
                    cmd.Parameters.AddWithValue("@rtid", residenceTempleId)
                    cmd.Parameters.AddWithValue("@kmid", kammavacaMonkId)
                    cmd.Parameters.AddWithValue("@amid", anusavanaMonkId)
                    cmd.Parameters.AddWithValue("@fo", If(formerOccupation, DBNull.Value))
                    cmd.Parameters.AddWithValue("@rm", If(remarks, DBNull.Value))
                    Return CInt(CLng(cmd.ExecuteScalar()))
                End Using
            End Using
        End Function
    End Class
End Namespace
