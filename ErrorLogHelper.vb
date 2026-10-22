Option Strict Off
Option Explicit On

Imports System
Imports System.IO

Namespace TempleAccounting
    ''' <summary>
    ''' Helper กลางสำหรับบันทึก error ที่ถูก catch แบบเงียบๆ (ไม่ popup ให้ผู้ใช้เห็น)
    ''' ใช้แทน empty Catch block ที่ควรจะมีร่องรอยไว้ตรวจสอบ
    ''' เขียนลง {LogsFolder}/error.log  alongside crash.log ที่มีอยู่แล้ว
    ''' </summary>
    Public Module ErrorLogHelper
        Private _errorLogPath As String = Nothing

        ''' <summary>
        ''' บันทึก error ที่ถูก catch แบบเงียบๆ ลงไฟล์ error.log
        ''' </summary>
        ''' <param name="context">ตำแหน่ง/บริบทที่เกิด error (เช่น "AppPreferences.Load", "frmMain.SetActiveForm")</param>
        ''' <param name="ex">Exception ที่เกิดขึ้น</param>
        Public Sub LogSilentError(context As String, ex As Exception)
            Try
                If String.IsNullOrEmpty(_errorLogPath) Then
                    _errorLogPath = Path.Combine(AppPaths.LogsFolder, "error.log")
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Dim exType As String = If(ex IsNot Nothing, ex.GetType().Name, "Unknown")
                Dim exMsg As String = If(ex IsNot Nothing AndAlso ex.Message IsNot Nothing, ex.Message, "(no message)")

                Dim line As String = $"[{timestamp}] {context}: {exType} — {exMsg}"

                File.AppendAllText(_errorLogPath, line & Environment.NewLine)
            Catch
                ' การ log เอง fail ก็ไม่ควรทำให้โปรแกรมพัง
                ' ยอมให้เงียบเฉพาะจุดนี้จุดเดียวในทั้งโปรเจกต์
            End Try
        End Sub
    End Module
End Namespace
