Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Drawing

Namespace TempleAccounting
    Public Class FrmHelpDialog
        Public Sub New(formName As String)
            InitializeComponent()
            LoadHelpContent(formName)
        End Sub

        Private Sub LoadHelpContent(formName As String)
            Try
                Dim manualPath = AppPaths.ManualFile
                If Not File.Exists(manualPath) Then
                    rtbContent.Text = "ไม่พบไฟล์คู่มือการใช้งาน (USER_MANUAL_TH.md)"
                    Return
                End If

                Dim content = File.ReadAllText(manualPath, Encoding.UTF8)
                Dim helpData = ExtractSection(content, formName)

                If String.IsNullOrEmpty(helpData.Content) Then
                    lblTitle.Text = "คู่มือการใช้งาน: " & formName
                    rtbContent.Text = "ขออภัย ไม่พบข้อมูลคำแนะนำสำหรับหน้าจอนี้ในคู่มือ"
                Else
                    lblTitle.Text = "คู่มือการใช้งาน: " & helpData.Title
                    RenderMarkdownAsRichText(helpData.Content)
                End If
            Catch ex As Exception
                rtbContent.Text = "เกิดข้อผิดพลาดในการโหลดข้อมูล: " & ex.Message
            End Try
        End Sub

        Private Structure HelpSection
            Public Title As String
            Public Content As String
        End Structure

        ''' <summary>
        ''' ดึงส่วนของเนื้อหาจาก Markdown ตามชื่อฟอร์ม
        ''' </summary>
        Private Function ExtractSection(content As String, formName As String) As HelpSection
            Dim result As New HelpSection With {.Title = "", .Content = ""}
            
            ' ค้นหาหัวข้อที่มีชื่อฟอร์มในวงเล็บ เช่น ## 1. หน้าจอ... (FrmTransactions)
            ' ดึงชื่อหัวข้อ (Title) และเนื้อหา (Content)
            Dim pattern = "## (.*?)\(" & Regex.Escape(formName) & "\)([\s\S]*?)(?=---|\n##|$)"
            Dim match = Regex.Match(content, pattern, RegexOptions.IgnoreCase)
            
            If match.Success Then
                result.Title = match.Groups(1).Value.Trim()
                ' ลบตัวเลขลำดับข้างหน้าออก (เช่น "1. ")
                result.Title = Regex.Replace(result.Title, "^\d+\.\s*", "")
                result.Content = match.Groups(2).Value.Trim()
            End If
            Return result
        End Function

        ''' <summary>
        ''' แสดงผล Markdown อย่างง่ายใน RichTextBox (รองรับตัวหนาและหัวข้อเบื้องต้น)
        ''' </summary>
        Private Sub RenderMarkdownAsRichText(text As String)
            rtbContent.Clear()
            
            ' แยกแต่ละบรรทัดเพื่อประมวลผล
            Dim lines = text.Split({Environment.NewLine, vbLf}, StringSplitOptions.None)
            
            For Each line In lines
                Dim trimmedLine = line.Trim()
                
                If String.IsNullOrWhiteSpace(trimmedLine) Then
                    rtbContent.AppendText(Environment.NewLine)
                    Continue For
                End If

                ' ตรวจสอบตัวหนา **text**
                Dim segments = Regex.Split(line, "(\*\*.*?\*\*)")
                For Each segment In segments
                    If segment.StartsWith("**") AndAlso segment.EndsWith("**") Then
                        Dim boldText = segment.Substring(2, segment.Length - 4)
                        AppendTextWithStyle(boldText, FontStyle.Bold)
                    Else
                        AppendTextWithStyle(segment, FontStyle.Regular)
                    End If
                Next
                rtbContent.AppendText(Environment.NewLine)
            Next
        End Sub

        Private Sub AppendTextWithStyle(text As String, style As FontStyle)
            Dim start = rtbContent.TextLength
            rtbContent.AppendText(text)
            rtbContent.Select(start, text.Length)
            rtbContent.SelectionFont = New Font(rtbContent.Font, style)
            rtbContent.SelectionColor = If(style = FontStyle.Bold, Color.FromArgb(0, 102, 204), Color.Black)
            rtbContent.SelectionLength = 0
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub
    End Class
End Namespace
