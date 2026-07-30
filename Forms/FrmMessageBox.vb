Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Class FrmMessageBox
    Inherits Form

    Private picIcon As PictureBox
    Private txtMessage As TextBox
    Private pnlButtons As FlowLayoutPanel

    Public Sub New(message As String, title As String, buttons As MessageBoxButtons, icon As MessageBoxIcon)
        Me.Text = If(String.IsNullOrEmpty(title), "ข้อความจากระบบ", title)
        Me.Font = New Font("Tahoma", 12.0F)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.ShowInTaskbar = False
        Me.AutoScaleMode = AutoScaleMode.None
        Me.ClientSize = New Size(560, 260)

        picIcon = New PictureBox() With {
            .Size = New Size(48, 48),
            .Location = New Point(16, 16),
            .SizeMode = PictureBoxSizeMode.CenterImage
        }

        txtMessage = New TextBox() With {
            .Multiline = True,
            .ReadOnly = True,
            .ScrollBars = ScrollBars.Vertical,
            .BorderStyle = BorderStyle.None,
            .Location = New Point(80, 16),
            .Size = New Size(456, 180),
            .BackColor = SystemColors.Window,
            .Font = New Font("Tahoma", 12.0F),
            .WordWrap = True
        }
        txtMessage.Text = message

        pnlButtons = New FlowLayoutPanel() With {
            .FlowDirection = FlowDirection.RightToLeft,
            .Dock = DockStyle.Bottom,
            .Height = 56,
            .Padding = New Padding(8)
        }

        Me.Controls.Add(picIcon)
        Me.Controls.Add(txtMessage)
        Me.Controls.Add(pnlButtons)

        SetIcon(icon)
        CreateButtons(buttons)

        ' Adjust size based on message length up to limits
        AdjustSizeForContent()
    End Sub

    Private Sub SetIcon(icon As MessageBoxIcon)
        Try
            Select Case icon
                Case MessageBoxIcon.Error
                    picIcon.Image = SystemIcons.Error.ToBitmap()
                Case MessageBoxIcon.Warning
                    picIcon.Image = SystemIcons.Warning.ToBitmap()
                Case MessageBoxIcon.Information
                    picIcon.Image = SystemIcons.Information.ToBitmap()
                Case MessageBoxIcon.Question
                    picIcon.Image = SystemIcons.Question.ToBitmap()
                Case Else
                    picIcon.Image = Nothing
            End Select
        Catch
            picIcon.Image = Nothing
        End Try
    End Sub

    Private Sub CreateButtons(buttons As MessageBoxButtons)
        pnlButtons.Controls.Clear()

        Dim addBtn As Action(Of String, DialogResult) = Sub(text, res)
                                                           Dim b As New Button() With {
                                                               .Text = text,
                                                               .AutoSize = True,
                                                               .Font = New Font("Tahoma", 12.0F),
                                                               .DialogResult = res,
                                                               .Padding = New Padding(8)
                                                           }
                                                           AddHandler b.Click, Sub(s, e) Me.DialogResult = res : Me.Close()
                                                           pnlButtons.Controls.Add(b)
                                                       End Sub

        Select Case buttons
            Case MessageBoxButtons.OK
                addBtn("ตกลง", DialogResult.OK)
            Case MessageBoxButtons.OKCancel
                addBtn("ยกเลิก", DialogResult.Cancel)
                addBtn("ตกลง", DialogResult.OK)
            Case MessageBoxButtons.YesNo
                addBtn("ไม่", DialogResult.No)
                addBtn("ใช่", DialogResult.Yes)
            Case MessageBoxButtons.YesNoCancel
                addBtn("ยกเลิก", DialogResult.Cancel)
                addBtn("ไม่", DialogResult.No)
                addBtn("ใช่", DialogResult.Yes)
            Case MessageBoxButtons.RetryCancel
                addBtn("ยกเลิก", DialogResult.Cancel)
                addBtn("ลองอีกครั้ง", DialogResult.Retry)
            Case MessageBoxButtons.AbortRetryIgnore
                addBtn("ยกเลิก", DialogResult.Abort)
                addBtn("ลองอีกครั้ง", DialogResult.Retry)
                addBtn("ละเว้น", DialogResult.Ignore)
            Case Else
                addBtn("ตกลง", DialogResult.OK)
        End Select

        ' Make first added button the AcceptButton for Enter key
        If pnlButtons.Controls.Count > 0 Then
            Me.AcceptButton = TryCast(pnlButtons.Controls(pnlButtons.Controls.Count - 1), Button)
        End If
    End Sub

    Private Sub AdjustSizeForContent()
        ' Try to size the textbox height to fit content (within limits)
        Try
            Using g = txtMessage.CreateGraphics()
                Dim lines = If(txtMessage.TextLength > 0, txtMessage.GetLineFromCharIndex(Math.Max(0, txtMessage.TextLength - 1)) + 1, 1)
                Dim approxLineHeight = TextRenderer.MeasureText(g, "A", txtMessage.Font).Height
                Dim desiredHeight = Math.Min(approxLineHeight * Math.Max(3, lines) + 24, 600)
                txtMessage.Height = Math.Max(80, desiredHeight)

                ' Adjust form height
                Dim newHeight = txtMessage.Bottom + pnlButtons.Height + 16
                newHeight = Math.Min(Math.Max(newHeight, 160), 700)
                Me.ClientSize = New Size(Math.Max(Me.ClientSize.Width, txtMessage.Width + 100), newHeight)

                ' Ensure controls placement
                pnlButtons.Top = Me.ClientSize.Height - pnlButtons.Height
                pnlButtons.Left = 0
                pnlButtons.Width = Me.ClientSize.Width
                txtMessage.Width = Me.ClientSize.Width - txtMessage.Left - 16
            End Using
        Catch
            ' ignore sizing errors
        End Try
    End Sub

    Public Shared Function ShowMessage(text As String, Optional caption As String = "ข้อความจากระบบ", Optional buttons As MessageBoxButtons = MessageBoxButtons.OK, Optional icon As MessageBoxIcon = MessageBoxIcon.None, Optional owner As IWin32Window = Nothing) As DialogResult
        Using f As New FrmMessageBox(text, caption, buttons, icon)
            If owner IsNot Nothing Then
                Return f.ShowDialog(owner)
            Else
                Return f.ShowDialog()
            End If
        End Using
    End Function
End Class
