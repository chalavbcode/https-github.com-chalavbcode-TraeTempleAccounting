---
name: report-rendering-layout
description: report-rendering-layout skill
---

# report-rendering-layout

ควบคุมการ render รายงาน PDF (Income/Expense Report) — ครอบคลุม signature block layout, page balancing, margin/coordinate constants, และ GDI+ rendering patterns

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **Signature Block Layout Churn (15+ commits):** `IncomeExpenseReport.vb` ถูกแก้ไข 16 ครั้ง ส่วนใหญ่เป็นการปรับ signature block positioning — เปลี่ยนค่า constants เช่น `labelToSignLine`, `signLineToName`, `bottomGapPx`, `FinalSignatureBlockHeight` ซ้ำแล้วซ้ำอีก
2. **Orphan Summary Page:** หน้าสุดท้ายมีเฉพาะ summary rows + signature block โดยไม่มี data rows — แก้ไขด้วย `PreCalculatePageBreaks()` algorithm
3. **Signature Height Mismatch:** `IncomeExpenseReport.vb` ค่า `FinalSignatureBlockHeight` (148px) ต้องตรงกับ `ReportEngine.DrawSignatureBlock` ค่า `totalSignatureHeight` (24+52+14+26+8+24 = 148px) — ถ้าเปลี่ยนค่าหนึ่งต้องเปลี่ยนอีกค่าหนึ่งด้วย
4. **Footer Gap Drift:** ค่า `FinalFooterGapHeight` (8px) และ `bottomGapPx` (4px) เปลี่ยนไปมาหลายครั้ง ทำให้ signature block ล้นขอบล่างหรือลอยสูงเกินไป
5. **Debug Logging Residue:** มี `Debug.WriteLine` หลายจุดที่ debug page balancing — ต้องลบก่อน release
6. **Chart Rendering Churn (6+ Refactors):** `frmMain.vb` chart code ถูก refactor ใน commits `98415b9`, `a981a87`, `5d619dd`, `10c2a36`, `7b8edf5`, `71c27f4` — ทุกครั้งบางส่วน ทำให้ logic กระจาย
7. **FrmChartPreview Fixed Size (commit `9dcdde1`):** `FrmChartPreview` ตั้ง fixed size 1200x800 — ไม่ responsive กับหน้าจอเล็ก
8. **ReportEngine Module Grew to 500+ Lines:** เริ่มจาก small helper ตอนนี้มี data preparation + layout + rendering + formatting ทั้งหมดในไฟล์เดียว

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Signature Block Constants Must Be in Sync
`IncomeExpenseReport.vb` และ `ReportEngine.vb` ต้องมีค่า signature height เท่ากัน

```vb
' Reports/IncomeExpenseReport.vb:
Private Const FinalSignatureBlockHeight As Integer = 148

' Reports/ReportEngine.vb — DrawSignatureBlock():
' labelHeight(24) + labelToSignLine(52) + signLineToName(14) + nameHeight(26) + nameToPosition(8) + positionHeight(24) = 148
```

**กฎ:** เมื่อเปลี่ยนค่า `totalSignatureHeight` ใน ReportEngine → ต้องเปลี่ยน `FinalSignatureBlockHeight` ใน IncomeExpenseReport ทันที

### Rule 2: Signature Block Positioning — Bottom-Up
Signature block ถูกคำนวณจาก **ขอบล่าง** ขึ้นมา:

```vb
' ReportEngine.vb — DrawSignatureBlock():
Const bottomGapPx As Integer = 4    ' gap จากขอบล่าง
Dim posCenterOffset = CInt((positionHeight - safeRowFont.Height) / 2)
Dim blockBottom = pageBottom - bottomGapPx - posCenterOffset

' Bottom-up positioning:
Dim signatureLabelY = blockBottom - totalSignatureHeight
Dim signatureLineY = signatureLabelY + labelHeight + labelToSignLine
Dim signatureNameY = signatureLineY + signLineToName
Dim signaturePositionY = signatureNameY + nameHeight + nameToPosition
```

**ห้ามคำนวณแบบ top-down** — signature block ต้องชิดขอบล่างเสมอ

### Rule 3: Page Balance Algorithm
`PreCalculatePageBreaks()` ป้องกันหน้าสุดท้ายที่มีแต่ summary+signature (orphan page):

```vb
' Algorithm:
' 1. จำลองการ fill หน้าตามปกติ
' 2. ถ้าหน้าสุดท้ายมี data rows < 5 → ดึง rows จากหน้าก่อนหน้า
' 3. ลด maxRows ของหน้าก่อนหน้า ~3 rows ต่อหน้า
' 4. Store break indices
```

**อย่าลบ algorithm นี้** — ถ้าลบจะเกิด orphan summary pages อีกครั้ง

### Rule 4: CalculateFinalContentHeight Must Match Actual Drawing
```vb
Private Function CalculateFinalContentHeight(rowH As Integer) As Integer
    ' Summary rows: 3 * 28 = 84px + gap 4px = 88px
    Dim summaryHeight = (FinalSummaryRows * rowH) + 4
    ' Signature block: 148px
    Dim signatureHeight = FinalSignatureBlockHeight
    ' Footer gap: 8px
    Dim footerGap = FinalFooterGapHeight
    ' Top gap: 4px
    Dim topGap = 4
    ' Total = 88 + 148 + 8 + 4 = 248px
    Return summaryHeight + signatureHeight + footerGap + topGap
End Function
```

**ห้ามเปลี่ยน layout constants ใน DrawSignatureBlock โดยไม่ update ค่าใน CalculateFinalContentHeight**

### Rule 5: Margin/Coordinate Constants
| Constant | Value | Location | หมายเหตุ |
|---|---|---|---|
| `bottomGapPx` | 4px | ReportEngine.vb L1042 | Gap จากขอบล่างถึง signature block |
| `posCenterOffset` | dynamic | ReportEngine.vb L1043 | ชดเชย glyph centering |
| `FinalFooterGapHeight` | 8px | IncomeExpenseReport.vb L71 | Gap ก่อน footer |
| `FinalSignatureBlockHeight` | 148px | IncomeExpenseReport.vb L69 | ต้องตรงกับ ReportEngine |
| `FinalSummaryRows` | 3 | IncomeExpenseReport.vb L69 | จำนวน summary rows |
| `SectionGap` | 12px | IncomeExpenseReport.vb L73 | Gap ระหว่าง left/right sections |
| Top margin | 50px | IncomeExpenseReport.vb | ขอบบนกระดาษ |
| Bottom margin | 50px | IncomeExpenseReport.vb | ขอบล่างกระดาษ |

### Rule 6: Dotted Signature Line
ใช้ `DrawDottedLine()` สำหรับเส้นเซ็นต์ — **ห้ามใช้ `DrawLine` แบบ solid**

```vb
' ✅ Correct:
ReportEngine.DrawDottedLine(g, x, y, lineWidth, dotSpacing:=8, dotRadius:=1.5F)

' ❌ Wrong — เส้นทึบไม่ใช่เส้นประ:
g.DrawLine(blackPen, x, y, x + lineWidth, y)
```

### Rule 7: Personnel Name Fallback
ชื่อผู้ลงนามต้อง fallback เป็นจุดเซ็นต์เมื่อว่าง:

```vb
' ✅ Correct:
Dim displayName = If(String.IsNullOrWhiteSpace(personnelName),
    "( .................................... )",
    "( " & personnelName.Trim() & " )")

' ❌ Wrong — hardcode จุดเสมอ แม้มีชื่อจริง:
Dim displayName = "( .................................... )"
```

### Rule 8: Debug Logging Cleanup
**ห้ามมี Debug.WriteLine ใน production report code** — สัญญาณที่ต้องลบ:

```vb
' ลบสิ่งเหล่านี้ทั้งหมด:
System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Signature Reserved Height = " & ...)
System.Diagnostics.Debug.WriteLine("[PAGE BALANCING] TotalRows=" & ...)
System.Diagnostics.Debug.WriteLine("[PAGE DEBUG] ..." & ...)
```

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| เปลี่ยน signature height ใน ReportEngine โดยไม่ update IncomeExpenseReport | Signature ล้นขอบหรือลอย | Sync ค่าทั้งสองจุดเสมอ |
| คำนวณ signature position top-down | Signature ลอยสูงเกินไป | คำนวณ bottom-up จาก pageBottom |
| ลบ PreCalculatePageBreaks | Orphan summary pages | เก็บ algorithm ไว้เสมอ |
| ใช้ solid line แทน dotted line สำหรับ signature | ไม่เป็นธรรมเนียมรายงาน | ใช้ DrawDottedLine เสมอ |
| Hardcode จุดเซ็นต์แทน personnel name | แสดงจุดเสมอแม้มีชื่อจริง | ใช้ If IsNullOrWhiteSpace fallback |
| ไม่ลบ Debug.WriteLine | Report ช้า, output เกินจำเป็น | ลบ debug logging ก่อน release |
| เปลี่ยน rowH constant โดยไม่ update CalculateFinalContentHeight | Final content overflow | Update ทั้งคู่เสมอ |

---

## GDI+ Rendering Notes

| Element | Method | Font | Notes |
|---|---|---|---|
| Report header | `g.DrawString()` | Bold 10pt | หัวเรื่องรายงาน |
| Table header | `g.DrawString()` | Bold 10pt | 2 rows (column headers + sub-headers) |
| Data rows | `g.DrawString()` | 10pt | Alternating row colors |
| Mid-summary | `g.DrawString()` | Bold 10pt | รวมทั้งสิ้น row |
| Final summary | `g.DrawString()` | Bold 10pt | 3 rows at bottom |
| Signature title | `g.DrawString()` | Bold 10pt | "เจ้าอาวาส" / "ไวยาวัจกร" |
| Signature name | `g.DrawString()` | Bold 12pt (fitted) | `( ชื่อจริง )` |
| Signature position | `g.DrawString()` | 10pt | ตำแหน่ง |
| Dotted line | `DrawDottedLine()` | N/A | dotSpacing=8, dotRadius=1.5F |

---

## Git Commit Evidence

| Commit | ปัญหาที่แก้ |
|---|---|
| `b23d60f` | Refactor signature block rendering in report |
| `b2b84bf` | Streamline report print/preview workflow and remember printer |
| `4fed94d` | Improve signature block layout and personnel name logic |
| `e65930b` | Refactor signature block rendering and name formatting |
| `856b78b` | Refactor signature block layout and data binding |
| `ccf4b67` | Improve signature block positioning in report layout |
| `be6faab` | Align footer and signature margins to layout box |
| `cffae35` | Reduce bottom gap for signature/footer in report layout |
| `468d485` | Align signature block bottom margin to 19px standard |
| `37453e8` | Improve FormatSignerName formatting and placeholder logic |
| `9209b16` | Improve PDF export workflow and UI feedback |
| `72c0cd1` | Add "Save as PDF" to reports and improve form layouts |
| `25a230a` | Refactor report data logic into ReportEngine module |
| `e867b6f` | Add chart totals toggle & improve report pagination/layout |
| `c74ddb5` | Fix isLastPage calculation after drawing all rows |
