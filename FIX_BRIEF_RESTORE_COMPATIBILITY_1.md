# FIX BRIEF — แก้ `RestoreFullSystem()` ให้เข้ากับโครงสร้าง Backup ใหม่ (Centralized Receipts)

**Repo path:** `C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422`
**อ้างอิงจาก:** `FIX_REPORT.md` หัวข้อ 9 (Append-Only Collision-Safe Receipt Backup)
**ไฟล์หลักที่แก้:** `DatabaseBackupHelper.vb`

---

## ปัญหา (สรุปให้ agent เข้าใจบริบท)

หัวข้อ 9 เพิ่งเปลี่ยนโครงสร้าง backup: `BackupFullSystem()` **เลิก** copy โฟลเดอร์ Receipts เข้าไปใน `Backup_{timestamp}/Receipts/` แล้ว หันไปเรียก `SyncReceiptsToBackup()` เก็บรูปใบเสร็จไว้ที่โฟลเดอร์กลาง `Backup/Receipts/` แทน

แต่ `FIX_REPORT.md` หัวข้อ 9.9 อ้างว่า **"Restore ยังทำงาน (RestoreFullSystem ไม่ได้ถูกแก้ไข)"** — นี่คือข้อสรุปที่ยังไม่ได้พิสูจน์ ถ้า `RestoreFullSystem()` เขียนมาให้อ่านรูปใบเสร็จจาก `Backup_{timestamp}/Receipts/` (โครงสร้างเดิม) การ restore จาก **backup ชุดใหม่ที่สร้างหลังแก้หัวข้อ 9** (ซึ่งไม่มีโฟลเดอร์ `Receipts/` อยู่ข้างในอีกแล้ว) อาจ**พังเงียบๆ หรือ restore รูปใบเสร็จไม่ได้เลย** ทั้งที่ database restore อาจสำเร็จปกติ

---

## สิ่งที่ต้องทำ

### 1. อ่านโค้ดจริงก่อน — ห้ามเดา

เปิด `DatabaseBackupHelper.vb` หาฟังก์ชัน `RestoreFullSystem()` (หรือชื่อเทียบเท่าที่ทำหน้าที่ restore) อ่านให้ครบว่า:
1. อ่านไฟล์ `.accdb` จากไหน (path pattern ที่คาดหวัง)
2. อ่านรูปใบเสร็จจากไหน (path pattern ที่คาดหวัง) — คาดว่าจะเป็น `{selectedBackupFolder}/Receipts/` แต่ต้องยืนยันจากโค้ดจริง
3. ถ้าโฟลเดอร์ที่คาดหวังไม่มีอยู่ (เช่น backup ใหม่ที่ไม่มี `Receipts/` ข้างใน) โค้ดปัจจุบันจะทำอะไร — throw exception, skip เงียบๆ, หรือมี error handling อยู่แล้ว?

**รายงานผลข้อ 1-3 ใน `FIX_REPORT.md` ก่อนเริ่มแก้เสมอ**

### 2. แก้ให้รองรับทั้งสองโครงสร้าง (เก่า + ใหม่)

เนื่องจากในเครื่องผู้ใช้จริงตอนนี้มี backup ทั้งแบบเก่า (มี `Receipts/` ในตัวเอง) และแบบใหม่ (ไม่มี ต้องไปดึงจากโฟลเดอร์กลาง) ปนกันอยู่ — `RestoreFullSystem()` ต้องรองรับทั้งสองแบบ:

```
ตรรกะที่ต้องการ:
1. Restore ไฟล์ .accdb จาก backup ที่เลือก (เหมือนเดิม ไม่เปลี่ยน)
2. เช็คว่า backup ที่เลือกมีโฟลเดอร์ Receipts/ ของตัวเองไหม
   - มี (backup แบบเก่า) → restore รูปจากตรงนั้นเหมือนเดิม
   - ไม่มี (backup แบบใหม่ หลังหัวข้อ 9) → ไปดึงรูปจากโฟลเดอร์กลาง Backup/Receipts/ แทน
     (คัดลอกทั้งโฟลเดอร์กลับไปที่ Receipts/ ที่ระบบใช้งานจริง หรือตามพฤติกรรม
      restore เดิมที่มีอยู่ — ให้ทำตามรูปแบบเดียวกับที่โค้ดเดิม restore จาก
      Backup_{timestamp}/Receipts/ ทำอยู่แล้ว แค่เปลี่ยนแหล่งที่มา)
```

**ห้ามลบความสามารถ restore backup แบบเก่าที่มีอยู่ในเครื่องแล้ว** — ต้อง backward compatible กับ backup ที่สร้างไว้ก่อนหัวข้อ 9 ด้วย

### 3. Edge case ที่ต้องคิดด้วย

- ถ้าผู้ใช้ restore backup ของ Weekly/Monthly/Yearly tier (ที่เป็นไฟล์ `.accdb` เดี่ยว ไม่ใช่โฟลเดอร์ `Backup_{timestamp}/`) — `RestoreFullSystem()` รองรับการ restore จาก tier เหล่านี้อยู่แล้วหรือไม่? ถ้าโค้ดเดิมออกแบบมาให้ restore ได้แค่จากโฟลเดอร์ `Backup_{timestamp}/` เท่านั้น (ไม่รองรับไฟล์เดี่ยวจาก tier) ให้ระบุปัญหานี้ไว้ใน `FIX_REPORT.md` เป็นข้อค้นพบใหม่ — **ไม่ต้องแก้ในรอบนี้ถ้าเป็นงานใหญ่เกินขอบเขต** แค่รายงานว่าเจอ แล้วรอคำสั่งแยกต่างหาก

### 4. Build ตรวจสอบ
`dotnet build -c Release` ต้องผ่าน 0 Warning / 0 Error

---

## สิ่งที่ห้ามทำ

- ห้ามแก้ `SyncReceiptsToBackup()`, `BackupFullSystem()` ส่วนที่ backup (แก้เฉพาะฝั่ง restore เท่านั้น)
- ห้ามลบความสามารถ restore backup รุ่นเก่าที่มีอยู่ในเครื่องผู้ใช้แล้ว

---

## การทดสอบ (สำคัญมาก — นี่คือจุดที่ทำให้เชื่อว่าแก้ถูกจริง)

ใช้ temporary test project หรือถ้า `RestoreFullSystem()` ทดสอบผ่าน integration test ง่ายกว่า ให้เลือกวิธีที่เหมาะกับโค้ดจริง (ลบทิ้ง + ยืนยันด้วย `git status` ก่อนจบงานเหมือนเดิม):

1. **Restore backup แบบเก่า (มี `Receipts/` ในตัวเอง):** จำลอง/ใช้โฟลเดอร์ `Backup_{timestamp}/` ที่มี `.accdb` + `Receipts/` ครบ → restore → ยืนยันว่า database และรูปใบเสร็จกลับมาครบเหมือนเดิม (regression — ต้องไม่พังจากที่เคยทำงานได้)
2. **Restore backup แบบใหม่ (ไม่มี `Receipts/` ในตัวเอง):** จำลองโฟลเดอร์ `Backup_{timestamp}/` ที่มีแค่ `.accdb` (ไม่มี Receipts/ ข้างใน) + จำลองโฟลเดอร์กลาง `Backup/Receipts/` ที่มีรูปทดสอบอยู่ → restore → ยืนยันว่ารูปใบเสร็จถูกดึงมาจากโฟลเดอร์กลางได้ถูกต้อง ไม่ error ไม่ skip เงียบๆ
3. **Restore เมื่อโฟลเดอร์กลางว่างเปล่า:** จำลองกรณี `Backup/Receipts/` ไม่มีไฟล์เลย → restore → ยืนยันว่าไม่ throw exception (database ยัง restore ได้ แค่ไม่มีรูปใบเสร็จให้ restore ซึ่งเป็นเรื่องปกติ)

---

## ผลลัพธ์ที่ต้องการ

เขียนต่อท้าย `FIX_REPORT.md` เป็นหัวข้อใหม่ **"10. Restore Compatibility Fix"** ระบุ:
- ผลตรวจสอบโค้ดจริงของ `RestoreFullSystem()` ก่อนแก้ (ข้อ 1 ด้านบน) — ระบุว่าก่อนแก้จะพังจริงหรือไม่ในกรณี backup แบบใหม่ (พิสูจน์ด้วยการทดสอบจริง ไม่ใช่แค่คาดเดา)
- Diff ของสิ่งที่แก้
- ผลทดสอบทั้ง 3 กรณี
- ถ้าเจอ edge case เรื่อง restore จาก tier folder (ข้อ 3) ให้รายงานไว้ชัดเจนว่าเป็นงานที่ยังไม่ได้ทำ รอคำสั่งแยก
