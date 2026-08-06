Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Module HelpSystem
        ''' <summary>
        ''' ติดตั้งระบบช่วยเหลือสำหรับหน้าจอ (F1 Shortcut และ Status Bar Hint)
        ''' </summary>
        ''' <param name="frm">หน้าจอที่ต้องการติดตั้ง</param>
        ''' <param name="sectionName">ชื่อหัวข้อในคู่มือที่เกี่ยวข้อง (ถ้ามี)</param>
        Public Sub SetupHelp(frm As Form, Optional sectionName As String = "")
            ' 1. ตั้งค่า KeyPreview เพื่อให้ Form รับค่าการกดปุ่มก่อน Control อื่นๆ
            frm.KeyPreview = True

            ' 2. เพิ่ม Status Bar Hint
            AddStatusBarHint(frm)
        End Sub

        ''' <summary>
        ''' เพิ่มแถบสถานะพร้อมคำแนะนำการใช้ Help (ถ้ายังไม่มี)
        ''' </summary>
        Private Sub AddStatusBarHint(frm As Form)
            Try
                Dim statusStrip As StatusStrip = Nothing

                ' ค้นหา StatusStrip ที่มีอยู่แล้วใน Form
                For Each ctrl As Control In frm.Controls
                    If TypeOf ctrl Is StatusStrip Then
                        statusStrip = DirectCast(ctrl, StatusStrip)
                        Exit For
                    End If
                Next

                ' ถ้าไม่มี ให้สร้างใหม่
                If statusStrip Is Nothing Then
                    statusStrip = New StatusStrip()
                    frm.Controls.Add(statusStrip)
                End If

                ' เพิ่ม Label สำหรับ Hint (ถ้ายังไม่มี)
                Dim helpHintLabelName As String = "lblHelpHint"
                Dim hintLabel As ToolStripStatusLabel = Nothing

                For Each item As ToolStripItem In statusStrip.Items
                    If item.Name = helpHintLabelName Then
                        hintLabel = DirectCast(item, ToolStripStatusLabel)
                        Exit For
                    End If
                Next

                If hintLabel Is Nothing Then
                    hintLabel = New ToolStripStatusLabel()
                    hintLabel.Name = helpHintLabelName
                    hintLabel.Text = "💡 คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้าจอนี้"
                    hintLabel.Alignment = ToolStripItemAlignment.Right
                    statusStrip.Items.Add(hintLabel)
                Else
                    hintLabel.Text = "💡 คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้าจอนี้"
                End If
            Catch ex As Exception
                ' พยายามทำต่อเงียบๆ เพื่อไม่ให้กระทบการทำงานหลัก
                Debug.WriteLine("Error adding status bar hint: " & ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' เปิดหน้าต่างคู่มือการใช้งาน
        ''' </summary>
        ''' <param name="formName">ชื่อฟอร์มเพื่อแสดงเนื้อหาที่เกี่ยวข้อง</param>
        Public Sub ShowManual(Optional formName As String = "")
            Try
                ' ถ้าไม่ได้ระบุชื่อฟอร์ม ให้เปิดไฟล์โดยตรงเหมือนเดิม
                If String.IsNullOrEmpty(formName) Then
                    Dim manualPath = AppPaths.ManualFile
                    If File.Exists(manualPath) Then
                        Process.Start(New ProcessStartInfo(manualPath) With {.UseShellExecute = True})
                    Else
                        MessageBox.Show("ไม่พบไฟล์คู่มือการใช้งาน (USER_MANUAL_TH.md)", "ไม่พบข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    Return
                End If

                ' แสดง Modal Dialog สำหรับ Help
                Using dlg As New FrmHelpDialog(formName)
                    dlg.ShowDialog()
                End Using
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถเปิดคู่มือได้: " & ex.Message, "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Module
End Namespace
