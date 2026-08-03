Imports System.Drawing
Imports System.Windows.Forms

Public Class MessageBoxHelper

    Private Shared ReadOnly LargeFont As New Font("Tahoma", 8) ' ปรับขนาด/ฟอนต์ตามต้องการ

    ' Overload พื้นฐาน: MessageBox.Show("text")
    Public Shared Function Show(text As String) As DialogResult
        Return ShowLargeMessageBox(text, "", MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    ' MessageBox.Show("text", "caption")
    Public Shared Function Show(text As String, caption As String) As DialogResult
        Return ShowLargeMessageBox(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None)
    End Function

    ' MessageBox.Show("text", "caption", buttons)
    Public Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons) As DialogResult
        Return ShowLargeMessageBox(text, caption, buttons, MessageBoxIcon.None)
    End Function

    ' MessageBox.Show("text", "caption", buttons, icon)
    Public Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return ShowLargeMessageBox(text, caption, buttons, icon)
    End Function

    ' ฟังก์ชันหลักที่ทำการ Render จริง (ของเดิมที่คุณมีอยู่แล้ว)
    Public Shared Function ShowLargeMessageBox(text As String, caption As String,
                                                buttons As MessageBoxButtons,
                                                icon As MessageBoxIcon) As DialogResult
        ' ตัวอย่างการปรับขนาดตัวอักษร — แก้ตามโค้ดจริงที่คุณมีอยู่แล้ว
        Dim result As DialogResult
        Using tempForm As New Form()
            tempForm.Font = LargeFont
            result = MessageBox.Show(tempForm, text, caption, buttons, icon)
        End Using
        Return result
    End Function

End Class

