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

        ''' <summary>
        ''' ค้นหาเส้นทางจริงของไฟล์ใบเสร็จ (fileName เช่น Receipt_124.jpg)
        ''' ลำดับการค้นหา:
        ''' 1) โฟลเดอร์มาตรฐาน <แอป>\Receipts (ตำแหน่งที่โปรแกรมบันทึกใหม่)
        ''' 2) โฟลเดอร์ Receipts ข้างโฟลเดอร์ฐานข้อมูล (<root>\Receipts) — ครอบคลุมกรณี
        '''    โปรแกรมรันจาก bin\... แต่ฐานข้อมูลอยู่ที่ root\Database\
        ''' 3) <root>\Images\Receipts และโฟลเดอร์ Receipts ของพ่อแม่ exe (bin\Debug\Receipts)
        ''' 4) โฟลเดอร์ทำงานปัจจุบัน (CurrentDirectory)
        ''' คืนค่าเส้นทางไฟล์แรกที่พบ หรือ "" ถ้าไม่พบ
        ''' </summary>
        Public Shared Function ResolveReceiptPath(fileName As String) As String
            If String.IsNullOrWhiteSpace(fileName) Then Return ""

            For Each folder As String In GetReceiptCandidateFolders()
                Dim candidate = Path.Combine(folder, fileName)
                If File.Exists(candidate) Then
                    Return candidate
                End If
            Next
            Return ""
        End Function

        ''' <summary>
        ''' รายชื่อโฟลเดอร์ที่ใช้ค้นหาใบเสร็จ (ไม่ซ้ำกัน) — ใช้ทั้งในการค้นหาไฟล์
        ''' และแสดงเส้นทางที่ค้นหาเมื่อไม่พบไฟล์
        ''' </summary>
        Public Shared Function GetReceiptCandidateFolders() As List(Of String)
            Dim folders As New List(Of String)()

            ' 1. โฟลเดอร์มาตรฐาน (ตำแหน่งบันทึกของโปรแกรม)
            folders.Add(AppPaths.ReceiptsDir)

            ' 2. ข้างโฟลเดอร์ฐานข้อมูล (project root) — ฐานข้อมูลอาจถูกค้นพบสูงขึ้นไปในโครงสร้างไดเรกทอรี
            Try
                Dim dbDir = Path.GetDirectoryName(AppPaths.DatabaseFile)
                If Not String.IsNullOrWhiteSpace(dbDir) Then
                    Dim projRoot = Directory.GetParent(dbDir).FullName
                    folders.Add(Path.Combine(projRoot, "Receipts"))
                    folders.Add(Path.Combine(projRoot, "Images", "Receipts"))
                End If
            Catch
            End Try

            ' 3. โฟลเดอร์พ่อแม่ของ exe (เช่น bin\Debug\Receipts เมื่อรันจาก bin\Debug\net10.0-windows)
            Try
                Dim exeParent = Directory.GetParent(AppPaths.AppRoot).FullName
                folders.Add(Path.Combine(exeParent, "Receipts"))
            Catch
            End Try

            ' 4. โฟลเดอร์ทำงานปัจจุบัน
            Try
                folders.Add(Path.Combine(Environment.CurrentDirectory, "Receipts"))
                folders.Add(Path.Combine(Environment.CurrentDirectory, "Images", "Receipts"))
            Catch
            End Try

            ' ตัดรายการซ้ำ (case-insensitive)
            Dim result As New List(Of String)()
            For Each f As String In folders
                If Not result.Contains(f, StringComparer.OrdinalIgnoreCase) Then
                    result.Add(f)
                End If
            Next
            Return result
        End Function
    End Class
End Namespace
