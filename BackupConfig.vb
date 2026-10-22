Option Strict Off
Option Explicit On

Imports System.Collections.Generic

Namespace TempleAccounting
    ''' <summary>
    ''' จุดเดียวที่กำหนดรายการที่จะสำรอง (BACKUP_ITEMS)
    ''' เพิ่มรายการใหม่ได้ที่นี่เพียงที่เดียว: เพิ่ม BackupItemSpec เข้าในรายการ
    ''' </summary>
    Public Module BackupConfig
        Public ReadOnly Property BackupItems As List(Of BackupItemSpec)
            Get
                Dim items As New List(Of BackupItemSpec)()

                ' 1. ฐานข้อมูลหลัก (.accdb)
                items.Add(New BackupItemSpec() With {
                    .SourcePath = AppPaths.DatabaseFile,
                    .IsFolder = False,
                    .Description = "ฐานข้อมูลหลัก (.accdb)"
                })

                ' 2. โฟลเดอร์รูปใบเสร็จทั้งหมด (รวมซับโฟลเดอร์)
                items.Add(New BackupItemSpec() With {
                    .SourcePath = AppPaths.ReceiptsDir,
                    .IsFolder = True,
                    .Description = "โฟลเดอร์รูปใบเสร็จ (Receipts) รวมซับโฟลเดอร์"
                })

                Return items
            End Get
        End Property
    End Module
End Namespace
