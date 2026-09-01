Option Strict Off
Option Explicit On

Namespace TempleAccounting
    ''' <summary>
    ''' ตัวช่วยจัดการค่า Key-Value สำหรับการตั้งค่าต่างๆ ของโปรแกรม
    ''' ปัจจุบันใช้ AppPreferences Module (ไฟล์ user_preferences.ini) ในการเก็บข้อมูล
    ''' สามารถขยายไปใช้ฐานข้อมูลได้ในอนาคต
    ''' </summary>
    Public Class AppPreferenceHelper

        ''' <summary>ตรวจสอบว่า Onboarding Wizard เสร็จสมบูรณ์แล้วหรือยัง</summary>
        Public Shared Function IsOnboardingCompleted() As Boolean
            Return AppPreferences.IsOnboardingCompleted
        End Function

        ''' <summary>ตั้งค่าสถานะ Onboarding Wizard (เสร็จ/ยังไม่เสร็จ) พร้อมบันทึกลงไฟล์</summary>
        Public Shared Sub SetOnboardingCompleted(value As Boolean)
            AppPreferences.IsOnboardingCompleted = value
            AppPreferences.Save()
        End Sub

        ''' <summary>ตั้งค่าให้ Onboarding ยังไม่เสร็จ (สำหรับรีเซ็ต)</summary>
        Public Shared Sub MarkOnboardingIncomplete()
            SetOnboardingCompleted(False)
        End Sub

        ''' <summary>ตั้งค่าให้ Onboarding เสร็จแล้ว (สำหรับข้าม wizard)</summary>
        Public Shared Sub MarkOnboardingComplete()
            SetOnboardingCompleted(True)
        End Sub

    End Class
End Namespace
