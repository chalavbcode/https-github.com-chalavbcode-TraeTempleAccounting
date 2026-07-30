Imports System
Imports System.Windows.Forms

''' <summary>
''' Compatibility helper to replace existing MessageBox.Show usages in the codebase.
''' For simplicity this module is defined at the project root namespace (no Namespace wrapper)
''' so it is visible from all files regardless of the project's Root Namespace setting.
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

    ' Overloads that include default button and options
    Public Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton) As DialogResult
        Return Show(text, caption, buttons, icon, defaultButton, MessageBoxOptions.None)
    End Function

    Public Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton, options As MessageBoxOptions) As DialogResult
        Return FrmMessageBox.ShowMessageWithDefault(text, caption, buttons, icon, defaultButton, Nothing)
    End Function

    Public Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return FrmMessageBox.ShowMessage(text, caption, buttons, icon, owner)
    End Function

    Public Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton, options As MessageBoxOptions) As DialogResult
        Return FrmMessageBox.ShowMessageWithDefault(text, caption, buttons, icon, defaultButton, owner)
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
