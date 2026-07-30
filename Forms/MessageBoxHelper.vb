Imports System
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' Compatibility helper to replace existing MessageBoxHelper usages in the codebase.
    ''' Internally forwards to FrmMessageBox.ShowMessage (which uses Tahoma 12 and supports long text).
    ''' Add more overloads here if other call sites need them.
    ''' </summary>
    Public Module MessageBoxHelper

        Public Function Show(text As String) As DialogResult
            Return FrmMessageBox.ShowMessage(text, "ข้อความจากระบบ", MessageBoxButtons.OK, MessageBoxIcon.None, Nothing)
        End Function

        Public Function Show(text As String, caption As String) As DialogResult
            Return FrmMessageBox.ShowMessage(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, Nothing)
        End Function

        Public Function Show(text As String, caption As String, buttons As MessageBoxButtons) As DialogResult
            Return FrmMessageBox.ShowMessage(text, caption, buttons, MessageBoxIcon.None, Nothing)
        End Function

        Public Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
            Return FrmMessageBox.ShowMessage(text, caption, buttons, icon, Nothing)
        End Function

        Public Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
            Return FrmMessageBox.ShowMessage(text, caption, buttons, icon, owner)
        End Function

        ' Optional convenience wrappers matching common patterns
        Public Function ShowInfo(text As String, Optional caption As String = "ข้อความจากระบบ") As DialogResult
            Return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Function

        Public Function ShowWarning(text As String, Optional caption As String = "แจ้งเตือน") As DialogResult
            Return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Function

        Public Function ShowError(text As String, Optional caption As String = "ผิดพลาด") As DialogResult
            Return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Function

        Public Function ConfirmYesNo(text As String, Optional caption As String = "ยืนยัน") As Boolean
            Return Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
        End Function

    End Module
End Namespace
