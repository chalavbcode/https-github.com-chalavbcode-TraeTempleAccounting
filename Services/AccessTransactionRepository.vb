Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Data.OleDb
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' Microsoft Access implementation ของ ITransactionRepository
    ''' ใช้ OleDb ผ่าน Db.OpenConn() / Db.ExecuteNonQuery() / Db.InsertAndGetId()
    ''' </summary>
    Public Class AccessTransactionRepository
        Implements ITransactionRepository

        Public Function InsertIncome(tranDate As Date, categoryID As Integer, fundID As Integer,
                                      bankID As Integer?, description As String, amount As Decimal,
                                      remark As String, receiveChannel As String,
                                      referenceNo As String) As Integer Implements ITransactionRepository.InsertIncome

            Dim bankValue As Object = If(bankID.HasValue, CType(bankID.Value, Object), DBNull.Value)
            Using conn = Db.OpenConn()
                Return Db.InsertAndGetId(conn,
                    "INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note], ReceiveChannel, ReferenceNo, TransactionGUID) " &
                    "VALUES (" & Db.AccessDateLiteral(tranDate) & ",'Income',@c,@f,@b,@de,@a,@n,@rc,@rn,@g)",
                    New Tuple(Of String, Object)("@c", categoryID),
                    New Tuple(Of String, Object)("@f", fundID),
                    New Tuple(Of String, Object)("@b", bankValue),
                    New Tuple(Of String, Object)("@de", description),
                    New Tuple(Of String, Object)("@a", amount),
                    New Tuple(Of String, Object)("@n", remark),
                    New Tuple(Of String, Object)("@rc", If(String.IsNullOrEmpty(receiveChannel), DBNull.Value, receiveChannel)),
                    New Tuple(Of String, Object)("@rn", If(String.IsNullOrEmpty(referenceNo), DBNull.Value, referenceNo)),
                    New Tuple(Of String, Object)("@g", Guid.NewGuid().ToString("D")))
            End Using
        End Function

        Public Sub UpdateIncome(transactionID As Integer, tranDate As Date, categoryID As Integer,
                                fundID As Integer, bankID As Integer?, description As String,
                                amount As Decimal, remark As String, receiveChannel As String,
                                referenceNo As String) Implements ITransactionRepository.UpdateIncome

            Dim bankValue As Object = If(bankID.HasValue, CType(bankID.Value, Object), DBNull.Value)
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn,
                    "UPDATE Transactions SET TranDate=" & Db.AccessDateLiteral(tranDate) &
                    ", CategoryID=@c, FundID=@f, BankID=@b, [Detail]=@de, Amount=@a, [Note]=@n, ReceiveChannel=@rc, ReferenceNo=@rn WHERE ID=@id",
                    New Tuple(Of String, Object)("@c", categoryID),
                    New Tuple(Of String, Object)("@f", fundID),
                    New Tuple(Of String, Object)("@b", bankValue),
                    New Tuple(Of String, Object)("@de", description),
                    New Tuple(Of String, Object)("@a", amount),
                    New Tuple(Of String, Object)("@n", remark),
                    New Tuple(Of String, Object)("@rc", If(String.IsNullOrEmpty(receiveChannel), DBNull.Value, receiveChannel)),
                    New Tuple(Of String, Object)("@rn", If(String.IsNullOrEmpty(referenceNo), DBNull.Value, referenceNo)),
                    New Tuple(Of String, Object)("@id", transactionID))
            End Using
        End Sub

        Public Sub UpdateReceiptPath(transactionID As Integer, receiptPath As String) Implements ITransactionRepository.UpdateReceiptPath
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "UPDATE Transactions SET ReceiptPath = @rp WHERE ID = @id",
                    New Tuple(Of String, Object)("@rp", receiptPath),
                    New Tuple(Of String, Object)("@id", transactionID))
            End Using
        End Sub
    End Class
End Namespace
