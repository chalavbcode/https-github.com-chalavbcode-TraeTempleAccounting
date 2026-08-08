Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' หน้าต่างแสดงรูปใบเสร็จ (ไม่ล็อคไฟล์ต้นฉบับ — โหลดผ่าน MemoryStream)
    ''' เปิดจากปุ่ม "🔍 ดูใบเสร็จ" ใน FrmTransactions
    ''' </summary>
    <DesignerCategory("Form")>
    Partial Public Class FrmReceiptViewer
        Inherits Form

        Private ReadOnly _receiptPath As String
        Private _stream As MemoryStream = Nothing
        Private _image As Image = Nothing
        Private _isFitMode As Boolean = True

        Public Sub New(receiptPath As String)
            InitializeComponent()
            _receiptPath = receiptPath
        End Sub

        Private Sub FrmReceiptViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            lblFileInfo.Text = _receiptPath
            Try
                ' โหลดผ่าน MemoryStream เพื่อไม่ให้ไฟล์ต้นฉบับถูกล็อคโดย OS
                _stream = New MemoryStream(File.ReadAllBytes(_receiptPath))
                _image = Image.FromStream(_stream)
                picReceipt.Image = _image
                ApplyFitMode()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmReceiptViewer.Load")
                MessageBox.Show("ไม่สามารถเปิดรูปภาพได้: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub ApplyFitMode()
            _isFitMode = True
            pnlScroll.AutoScroll = False
            picReceipt.Dock = DockStyle.Fill
            picReceipt.SizeMode = PictureBoxSizeMode.Zoom
            picReceipt.Location = Point.Empty
        End Sub

        Private Sub ApplyActualMode()
            _isFitMode = False
            picReceipt.Dock = DockStyle.None
            picReceipt.SizeMode = PictureBoxSizeMode.AutoSize
            If _image IsNot Nothing Then
                picReceipt.Size = _image.Size
            End If
            picReceipt.Location = Point.Empty
            pnlScroll.AutoScroll = True
            pnlScroll.AutoScrollMinSize = picReceipt.Size
        End Sub

        Private Sub btnFit_Click(sender As Object, e As EventArgs) Handles btnFit.Click
            ApplyFitMode()
        End Sub

        Private Sub btnActualSize_Click(sender As Object, e As EventArgs) Handles btnActualSize.Click
            ApplyActualMode()
        End Sub

        Private Sub btnOpenExternal_Click(sender As Object, e As EventArgs) Handles btnOpenExternal.Click
            Try
                Process.Start(New ProcessStartInfo(_receiptPath) With {.UseShellExecute = True})
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmReceiptViewer.OpenExternal")
                MessageBox.Show("ไม่สามารถเปิดด้วยโปรแกรมภายนอกได้: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            MyBase.OnFormClosing(e)
            ' ปล่อยทรัพยากรภาพและ MemoryStream
            If _image IsNot Nothing Then
                _image.Dispose()
                _image = Nothing
            End If
            If _stream IsNot Nothing Then
                _stream.Dispose()
                _stream = Nothing
            End If
            If picReceipt.Image IsNot Nothing Then
                picReceipt.Image = Nothing
            End If
        End Sub
    End Class
End Namespace
