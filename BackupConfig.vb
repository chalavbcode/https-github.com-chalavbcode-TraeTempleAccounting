Option Strict Off
Option Explicit On

Imports System.Collections.Generic

Namespace TempleAccounting
    ''' <summary>
    ''' จุดเดียวที่กำหนดรายการที่จะสำรอง (BACKUP_ITEMS) และเส้นทางรากของปลายทางสำรอง
    ''' เพิ่มรายการใหม่ได้ที่นี่เพียงที่เดียว โดยเรียก AddItem หนึ่งบรรทัด
    ''' </summary>
    Public Module BackupConfig

        ''' <summary>เส้นทางรากปลายทางสำรอง (เพิ่มได้หลายไดรฟ์/โฟลเดอร์) — ค่าเริ่มต้นคือโฟลเดอร์ Backup ภายในแอป</summary>
        Public ReadOnly Property BackupRootPaths As List(Of String)
            Get
                Dim roots As New List(Of String)()
                roots.Add(AppPaths.BackupFolder)
                Return roots
            End Get
        End Property

        ''' <summary>
        ''' รายการที่ต้องสำรอง: ไฟล์เดี่ยว (IsFolder=False) หรือโฟลเดอร์รวมซับโฟลเดอร์ (IsFolder=True)
        ''' การเพิ่มรายการใหม่ = เพิ่ม AddItem หนึ่งบรรทัด
        ''' </summary>
        Public ReadOnly Property BackupItems As List(Of BackupItemSpec)
            Get
                Dim items As New List(Of BackupItemSpec)()

                AddItem(items, AppPaths.DatabaseFile, False, "ฐานข้อมูลหลัก (.accdb)")
                AddItem(items, AppPaths.ReceiptsDir, True, "โฟลเดอร์รูปใบเสร็จ (Receipts) รวมซับโฟลเดอร์")

                Return items
            End Get
        End Property

        ''' <summary>เพิ่มรายการสำรองหนึ่งรายการ (ใช้เพื่อให้การเพิ่มรายการเป็นการเปลี่ยนแปลงบรรทัดเดียว)</summary>
        Private Sub AddItem(items As List(Of BackupItemSpec), sourcePath As String, isFolder As Boolean, description As String)
            items.Add(New BackupItemSpec() With {
                .SourcePath = sourcePath,
                .IsFolder = isFolder,
                .Description = description
            })
        End Sub

    End Module
End Namespace
