Option Strict Off
Option Explicit On

Imports System
Imports System.Data
Imports System.Data.OleDb

Namespace TempleAccounting

    Public Class PositionService

        Private Const POSITION_ABBOT As String = "เจ้าอาวาส"
        Private Const POSITION_WAIYAWAT As String = "ไวยาวัจกร"
        Private Const POSITION_BOOKKEEPER As String = "ผู้ทำบัญชี"

        ''' <summary>
        ''' ดึงข้อมูลเจ้าอาวาสปัจจุบัน
        ''' </summary>
        ''' <returns>Tuple(Of PersonnelID, FullName, PositionName) หรือ Nothing</returns>
        Public Shared Function GetCurrentAbbot() As (PersonnelID As Integer, FullName As String, PositionName As String)?
            Return GetPersonnelByPosition(POSITION_ABBOT, "AbbotPersonnelID")
        End Function

        ''' <summary>
        ''' ดึงข้อมูลไวยาวัจกรปัจจุบัน
        ''' </summary>
        ''' <returns>Tuple(Of PersonnelID, FullName, PositionName) หรือ Nothing</returns>
        Public Shared Function GetCurrentWaiyawat() As (PersonnelID As Integer, FullName As String, PositionName As String)?
            Return GetPersonnelByPosition(POSITION_WAIYAWAT, "WaiyawatPersonnelID")
        End Function

        ''' <summary>
        ''' ดึงข้อมูลผู้ทำบัญชีปัจจุบัน
        ''' </summary>
        ''' <returns>Tuple(Of PersonnelID, FullName, PositionName) หรือ Nothing</returns>
        Public Shared Function GetCurrentBookkeeper() As (PersonnelID As Integer, FullName As String, PositionName As String)?
            Return GetPersonnelByPosition(POSITION_BOOKKEEPER, "BookkeeperPersonnelID")
        End Function

        ''' <summary>
        ''' ดึงข้อมูลบุคลากรตามชื่อตำแหน่ง
        ''' ถ้ามีหลายรายการ จะดึงจาก PrimaryAssignment (TempleSetting) ก่อน
        ''' ถ้า PrimaryAssignment ไม่มี จะดึงรายการแรกตาม PersonnelID
        ''' </summary>
        Private Shared Function GetPersonnelByPosition(positionName As String, roleFieldName As String) As (PersonnelID As Integer, FullName As String, PositionName As String)?
            Try
                Using conn = Db.OpenConn()
                    ' ดึง PersonnelID จาก TempleSetting (PrimaryAssignment)
                    Dim primaryIdObj = Db.DbScalar(conn,
                        $"SELECT {roleFieldName} FROM TempleSetting ORDER BY ID DESC")
                    Dim primaryId As Integer? = Nothing
                    If primaryIdObj IsNot Nothing AndAlso primaryIdObj IsNot DBNull.Value Then
                        primaryId = CInt(primaryIdObj)
                    End If

                    ' ดึงรายชื่อบุคลากรที่มีตำแหน่งตรงกัน พร้อมตำแหน่ง
                    Dim sql = "SELECT p.PersonnelID, p.Title & ' ' & p.FirstName & ' ' & p.LastName AS FullName, " &
                              "pos.PositionName " &
                              "FROM (Personnel p INNER JOIN Positions pos ON p.PositionID = pos.PositionID) " &
                              "WHERE pos.PositionName = @pname " &
                              "ORDER BY p.PersonnelID"

                    Dim dt = Db.GetTable(conn, sql, New Tuple(Of String, Object)("@pname", positionName))

                    If dt.Rows.Count = 0 Then
                        Return Nothing
                    End If

                    ' ถ้ามี PrimaryAssignment และตรงกับรายการในฐาน
                    If primaryId.HasValue Then
                        For Each row As DataRow In dt.Rows
                            If CInt(row("PersonnelID")) = primaryId.Value Then
                                Return (PersonnelID:=CInt(row("PersonnelID")),
                                        FullName:=row("FullName")?.ToString(),
                                        PositionName:=row("PositionName")?.ToString())
                            End If
                        Next
                    End If

                    ' ถ้าไม่มี PrimaryAssignment หรือไม่ตรง ใช้รายการแรก
                    Dim firstRow = dt.Rows(0)
                    Return (PersonnelID:=CInt(firstRow("PersonnelID")),
                            FullName:=firstRow("FullName")?.ToString(),
                            PositionName:=firstRow("PositionName")?.ToString())
                End Using

            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("[PositionService] Error: " & ex.ToString())
                Return Nothing
            End Try
        End Function

    End Class

End Namespace
