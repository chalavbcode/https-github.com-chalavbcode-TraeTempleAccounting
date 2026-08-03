Option Strict Off
Option Explicit On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO

Namespace TempleAccounting
    Public Class ReceiptImageHelper
        Private Const MaxSize As Integer = 1600
        Private Const JpegQuality As Long = 80L

        ''' <summary>
        ''' ปรับขนาดและบีบอัดรูปภาพจากไฟล์ แล้วบันทึกลงโฟลเดอร์ Receipts
        ''' </summary>
        Public Shared Function SaveOptimizedReceipt(sourcePath As String, transactionID As Integer) As String
            Try
                If Not File.Exists(sourcePath) Then Return ""

                Using img As Image = Image.FromFile(sourcePath)
                    Return SaveOptimizedReceipt(img, transactionID)
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "ReceiptImageHelper.SaveFromFile")
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' ปรับขนาดและบีบอัดรูปภาพจาก Image Object แล้วบันทึกลงโฟลเดอร์ Receipts
        ''' </summary>
        Public Shared Function SaveOptimizedReceipt(sourceImg As Image, transactionID As Integer) As String
            Try
                AppPaths.EnsureDirectoriesExist()

                ' 1. คำนวณขนาดใหม่คง Aspect Ratio
                Dim newWidth As Integer = sourceImg.Width
                Dim newHeight As Integer = sourceImg.Height

                If sourceImg.Width > MaxSize OrElse sourceImg.Height > MaxSize Then
                    Dim ratio As Double = Math.Min(MaxSize / sourceImg.Width, MaxSize / sourceImg.Height)
                    newWidth = CInt(sourceImg.Width * ratio)
                    newHeight = CInt(sourceImg.Height * ratio)
                End If

                ' 2. สร้าง Bitmap ใหม่และวาดรูปที่ปรับขนาดแล้ว
                Using newImg As New Bitmap(newWidth, newHeight)
                    Using g As Graphics = Graphics.FromImage(newImg)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.DrawImage(sourceImg, 0, 0, newWidth, newHeight)
                    End Using

                    ' 3. ตั้งค่า JPEG Compression
                    Dim jpegEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                    Dim encoderParameters As New EncoderParameters(1)
                    encoderParameters.Param(0) = New EncoderParameter(Encoder.Quality, JpegQuality)

                    ' 4. บันทึกไฟล์
                    Dim fileName As String = $"Receipt_{transactionID}.jpg"
                    Dim destPath As String = Path.Combine(AppPaths.ReceiptsDir, fileName)

                    newImg.Save(destPath, jpegEncoder, encoderParameters)

                    Return fileName
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "ReceiptImageHelper.SaveFromImage")
                Return ""
            End Try
        End Function

        Private Shared Function GetEncoder(format As ImageFormat) As ImageCodecInfo
            Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
            For Each codec As ImageCodecInfo In codecs
                If codec.FormatID = format.Guid Then
                    Return codec
                End If
            Next
            Return Nothing
        End Function
    End Class
End Namespace
