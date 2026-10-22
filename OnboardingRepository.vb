Option Strict Off
Option Explicit On

Imports System.Data

Namespace TempleAccounting
    ''' <summary>
    ''' Data Access Layer เฉพาะของ Onboarding Wizard
    ''' จัดการการอ่าน/เขียนข้อมูลที่เกี่ยวข้องกับขั้นตอนการตั้งค่าเริ่มต้น
    ''' ใช้ Db.* facade สำหรับการเชื่อมต่อฐานข้อมูล (รองรับทั้ง Access และ SQL Server)
    ''' </summary>
    Public Class OnboardingRepository

#Region "===== Step 1: ข้อมูลวัด (TempleSetting) ====="

        ''' <summary>นับจำนวนรายการในตาราง TempleSetting</summary>
        Public Shared Function GetTempleSettingCount() As Integer
            Using conn = Db.OpenConn()
                Return Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM TempleSetting"))
            End Using
        End Function

        ''' <summary>
        ''' บันทึกข้อมูลวัด — ลบข้อมูลเก่าแล้ว INSERT ใหม่ (ภายใน Transaction)
        ''' </summary>
        Public Shared Sub SaveTempleSetting(templeCode As String, templeName As String,
                                            templeAddress As String, tambon As String,
                                            amphoe As String, province As String,
                                            postCode As String, phone As String)
            Using conn = Db.OpenConn()
                Using trans = conn.BeginTransaction()
                    Try
                        Dim oledbConn = TryCast(conn, OleDb.OleDbConnection)
                        ' ลบข้อมูลเก่า
                        Using cmd = oledbConn.CreateCommand()
                            cmd.Transaction = TryCast(trans, OleDb.OleDbTransaction)
                            cmd.CommandText = "DELETE FROM TempleSetting"
                            cmd.ExecuteNonQuery()
                        End Using
                        ' INSERT ข้อมูลใหม่
                        Db.InsertAndGetId(conn,
                            "INSERT INTO TempleSetting (TempleCode,TempleName,TempleAddress,Tambon,Amphoe,Province,PostCode,TemplePhone) VALUES (@c,@n,@a,@t,@am,@p,@z,@ph)",
                            New Tuple(Of String, Object)("@c", templeCode),
                            New Tuple(Of String, Object)("@n", templeName),
                            New Tuple(Of String, Object)("@a", templeAddress),
                            New Tuple(Of String, Object)("@t", tambon),
                            New Tuple(Of String, Object)("@am", amphoe),
                            New Tuple(Of String, Object)("@p", province),
                            New Tuple(Of String, Object)("@z", If(postCode, "")),
                            New Tuple(Of String, Object)("@ph", phone))
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

        ''' <summary>โหลดข้อมูลวัดล่าสุด (ใช้สำหรับ reload เมื่อกลับขั้นตอน)</summary>
        Public Shared Function LoadTempleSetting() As DataRow
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT TOP 1 * FROM TempleSetting ORDER BY ID DESC")
                If dt.Rows.Count > 0 Then Return dt.Rows(0)
                Return Nothing
            End Using
        End Function

#End Region

#Region "===== Step 2: บุคลากร (Personnel) ====="

        ''' <summary>นับจำนวนบุคลากรทั้งหมดในตาราง Personnel</summary>
        Public Shared Function GetPersonnelCount() As Integer
            Using conn = Db.OpenConn()
                Return Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Personnel"))
            End Using
        End Function

        ''' <summary>ดึงรายชื่อบุคลากรตามกลุ่ม PositionGroup (พระ/ฆราวาส)</summary>
        Public Shared Function GetPersonnelByGroup(positionGroup As String) As DataTable
            Using conn = Db.OpenConn()
                Return Db.GetTable(conn,
                    "SELECT p.PersonnelID, p.FullName " &
                    "FROM Personnel p INNER JOIN Positions pos ON p.PositionID = pos.PositionID " &
                    "WHERE pos.PositionGroup = @g ORDER BY p.FullName",
                    New Tuple(Of String, Object)("@g", positionGroup))
            End Using
        End Function

        ''' <summary>ลิงก์บุคลากรเข้ากับ TempleSetting (Abbot/Waiyawat/Bookkeeper)</summary>
        Public Shared Sub LinkPersonnelToTempleSetting(abbotID As Object, waiyawatID As Object, bookkeeperID As Object)
            Using conn = Db.OpenConn()
                Using trans = conn.BeginTransaction()
                    Try
                        Dim oledbConn = TryCast(conn, OleDb.OleDbConnection)
                        Using cmd = oledbConn.CreateCommand()
                            cmd.Transaction = TryCast(trans, OleDb.OleDbTransaction)
                            cmd.CommandText =
                                "UPDATE TempleSetting SET AbbotPersonnelID=@a, WaiyawatPersonnelID=@w, BookkeeperPersonnelID=@b"
                            cmd.Parameters.AddWithValue("@a", If(abbotID IsNot Nothing, abbotID, DBNull.Value))
                            cmd.Parameters.AddWithValue("@w", If(waiyawatID IsNot Nothing, waiyawatID, DBNull.Value))
                            cmd.Parameters.AddWithValue("@b", If(bookkeeperID IsNot Nothing, bookkeeperID, DBNull.Value))
                            cmd.ExecuteNonQuery()
                        End Using
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

        ''' <summary>ดึงสรุปบุคลากรที่ลิงก์แล้ว (สำหรับ Step 2 DataGridView)</summary>
        Public Shared Function GetLinkedPersonnelSummary() As DataTable
            Using conn = Db.OpenConn()
                Dim sql As String =
                    "SELECT 'เจ้าอาวาส' AS ColRole, p.FullName AS ColName, pos.PositionName AS ColPos " &
                    "FROM (TempleSetting t INNER JOIN Personnel p ON t.AbbotPersonnelID = p.PersonnelID) " &
                    "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID " &
                    "UNION ALL " &
                    "SELECT 'ไวยาวัจกร' AS ColRole, p.FullName AS ColName, pos.PositionName AS ColPos " &
                    "FROM (TempleSetting t INNER JOIN Personnel p ON t.WaiyawatPersonnelID = p.PersonnelID) " &
                    "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID " &
                    "UNION ALL " &
                    "SELECT 'ผู้ทำบัญชี' AS ColRole, p.FullName AS ColName, pos.PositionName AS ColPos " &
                    "FROM (TempleSetting t INNER JOIN Personnel p ON t.BookkeeperPersonnelID = p.PersonnelID) " &
                    "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID"
                Return Db.GetTable(conn, sql)
            End Using
        End Function

#End Region

#Region "===== Step 3: บัญชีธนาคาร (BankAccounts) ====="

        ''' <summary>นับจำนวนบัญชีธนาคาร</summary>
        Public Shared Function GetBankAccountCount() As Integer
            Using conn = Db.OpenConn()
                Return Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM BankAccounts"))
            End Using
        End Function

        ''' <summary>ดึงรายการบัญชีธนาคารทั้งหมด</summary>
        Public Shared Function GetBankAccounts() As DataTable
            Using conn = Db.OpenConn()
                Return Db.GetTable(conn, "SELECT ID, BankName, AccountNo, AccountName FROM BankAccounts ORDER BY BankName")
            End Using
        End Function

        ''' <summary>เพิ่มบัญชีธนาคารใหม่ (คืนค่า ID ที่สร้าง)</summary>
        Public Shared Function AddBankAccount(bankName As String, accountNo As String, accountName As String) As Integer
            Using conn = Db.OpenConn()
                Return Db.InsertAndGetId(conn,
                    "INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@bn, @an, @nm)",
                    New Tuple(Of String, Object)("@bn", bankName),
                    New Tuple(Of String, Object)("@an", accountNo),
                    New Tuple(Of String, Object)("@nm", accountName))
            End Using
        End Function

        ''' <summary>นับจำนวนกองทุน</summary>
        Public Shared Function GetFundCount() As Integer
            Using conn = Db.OpenConn()
                Return Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Funds"))
            End Using
        End Function

        ''' <summary>ดึงรายการกองทุนทั้งหมด</summary>
        Public Shared Function GetFunds() As DataTable
            Using conn = Db.OpenConn()
                Return Db.GetTable(conn, "SELECT ID, FundName FROM Funds ORDER BY FundName")
            End Using
        End Function

#End Region

#Region "===== Step 4: ยอดยกมา (Opening Balance) ====="

        ''' <summary>บันทึกยอดยกมาเริ่มต้น — ลบรายการเก่าแล้ว INSERT ใหม่ (ภายใน Transaction)</summary>
        Public Shared Sub SaveOpeningBalance(balanceDate As Date, cashAmount As Decimal, bankAmount As Decimal)
            Using conn = Db.OpenConn()
                Using trans = conn.BeginTransaction()
                    Try
                        Dim oledbConn = TryCast(conn, OleDb.OleDbConnection)
                        ' ลบยอดยกมาเดิม
                        Using cmd = oledbConn.CreateCommand()
                            cmd.Transaction = TryCast(trans, OleDb.OleDbTransaction)
                            cmd.CommandText = "DELETE FROM Transactions WHERE TranType='OpeningBalance'"
                            cmd.ExecuteNonQuery()
                        End Using
                        Dim dateLit = Db.AccessDateLiteral(Db.NormalizeGregorianDate(balanceDate))
                        ' INSERT เงินสด (ถ้ามี)
                        If cashAmount > 0 Then
                            Db.InsertAndGetId(conn,
                                "INSERT INTO Transactions (TranDate,TranType,Detail,Amount,Note) VALUES (" &
                                dateLit & ",'OpeningBalance','Cash Opening',@a,'')",
                                New Tuple(Of String, Object)("@a", cashAmount))
                        End If
                        ' INSERT เงินฝาก (ถ้ามี)
                        If bankAmount > 0 Then
                            Db.InsertAndGetId(conn,
                                "INSERT INTO Transactions (TranDate,TranType,Detail,Amount,Note) VALUES (" &
                                dateLit & ",'OpeningBalance','Bank Opening',@a,'')",
                                New Tuple(Of String, Object)("@a", bankAmount))
                        End If
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

        ''' <summary>ตรวจสอบว่ามียอดยกมาแล้วหรือยัง</summary>
        Public Shared Function HasOpeningBalance() As Boolean
            Using conn = Db.OpenConn()
                Dim cnt = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranType='OpeningBalance'"))
                Return cnt > 0
            End Using
        End Function

#End Region

#Region "===== Step 5: สรุปข้อมูล ====="

        ''' <summary>ดึงข้อมูลสรุปสำหรับแสดงใน Step 5</summary>
        Public Shared Function GetOnboardingSummary() As DataTable
            Using conn = Db.OpenConn()
                Dim dt As New DataTable()
                dt.Columns.Add("Field", GetType(String))
                dt.Columns.Add("Value", GetType(String))

                ' ข้อมูลวัด
                Dim templeName = Db.DbScalar(conn, "SELECT TempleName FROM TempleSetting ORDER BY ID DESC")
                dt.Rows.Add("ชื่อวัด", If(templeName IsNot Nothing, templeName.ToString(), "-"))

                Dim templeAddr = Db.DbScalar(conn, "SELECT TempleAddress FROM TempleSetting ORDER BY ID DESC")
                dt.Rows.Add("ที่อยู่", If(templeAddr IsNot Nothing, templeAddr.ToString(), "-"))

                ' บุคลากร
                Dim abbotName = Db.DbScalar(conn,
                    "SELECT TOP 1 p.FullName FROM TempleSetting t INNER JOIN Personnel p ON t.AbbotPersonnelID = p.PersonnelID")
                dt.Rows.Add("เจ้าอาวาส", If(abbotName IsNot Nothing, abbotName.ToString(), "-"))

                Dim waiyawatName = Db.DbScalar(conn,
                    "SELECT TOP 1 p.FullName FROM TempleSetting t INNER JOIN Personnel p ON t.WaiyawatPersonnelID = p.PersonnelID")
                dt.Rows.Add("ไวยาวัจกร", If(waiyawatName IsNot Nothing, waiyawatName.ToString(), "-"))

                Dim bookkeeperName = Db.DbScalar(conn,
                    "SELECT TOP 1 p.FullName FROM TempleSetting t INNER JOIN Personnel p ON t.BookkeeperPersonnelID = p.PersonnelID")
                dt.Rows.Add("ผู้ทำบัญชี", If(bookkeeperName IsNot Nothing, bookkeeperName.ToString(), "-"))

                ' บัญชีธนาคาร
                Dim bankCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM BankAccounts"))
                dt.Rows.Add("บัญชีธนาคาร", bankCount.ToString() & " บัญชี")

                ' กองทุน
                Dim fundCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Funds"))
                dt.Rows.Add("กองทุน", fundCount.ToString() & " กองทุน")

                ' ยอดยกมา
                Dim cashBal = Db.ToDecimalOrZero(Db.DbScalar(conn,
                    "SELECT SUM(Amount) FROM Transactions WHERE TranType='OpeningBalance' AND Detail='Cash Opening'"))
                Dim bankBal = Db.ToDecimalOrZero(Db.DbScalar(conn,
                    "SELECT SUM(Amount) FROM Transactions WHERE TranType='OpeningBalance' AND Detail='Bank Opening'"))
                dt.Rows.Add("ยอดเงินสดยกมา", cashBal.ToString("#,##0.00"))
                dt.Rows.Add("ยอดเงินฝากยกมา", bankBal.ToString("#,##0.00"))

                Return dt
            End Using
        End Function

#End Region

#Region "===== รีเซ็ตสถานะ Onboarding ====="

        ''' <summary>
        ''' รีเซ็ตสถานะ Onboarding — ตั้งค่า IsOnboardingCompleted = False
        ''' (ไม่ลบข้อมูลวัด/บุคลากร แค่รีเซ็ต flag เพื่อให้ Wizard แสดงอีกครั้ง)
        ''' </summary>
        Public Shared Sub ResetOnboardingStatus()
            AppPreferences.IsOnboardingCompleted = False
            AppPreferences.Save()
        End Sub

#End Region

    End Class
End Namespace
