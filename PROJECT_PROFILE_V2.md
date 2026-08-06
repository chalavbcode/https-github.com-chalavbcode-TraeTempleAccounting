# PROJECT_PROFILE_V2

## Section 1: Comprehensive Project Overview

### Project Name & Architecture
- **Project Name:** Temple Accounting System (TempleAccounting)
- **Architecture:** Visual Basic .NET (WinForms / .NET Framework)
- **Database:** MS Access Database (`TempleAccounting.accdb`)

### Key Modules & Core Features
- **Transaction Management:** [FrmTransactions.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb) - Income & Expense data entry.
- **Temple Profile Settings:** [FrmTempleSetting.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTempleSetting.vb) - Managing temple identity and personnel settings.
- **Reporting Engine:**
    - [ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb) - Shared drawing utilities and rendering engine.
    - [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb) - Business logic for Summary (แบบย่อ) and Detailed (แบบรายละเอียด) financial reports.
    - [FrmReports.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmReports.vb) - UI for generating and previewing reports.

---

## Section 2: Critical Bug Fix History & Regression Prevention Guide

This section documents resolved issues to prevent future regressions.

### 1. Report Signature Cut-Off at Page Bottom (A4 Overflow)
- **Issue:** Signature titles (เจ้าอาวาส, ไวยาวัจกร) overflowed the printable bottom margin of A4 paper, displaying only diacritics (ไม้โท, ไม้หันอากาศ).
- **Solution Rule:** Keep footer Y-coordinate offset safely elevated (15-25px clearance from printable bottom). Ensure line spacing accounts for `Font.Height` + padding.
- **Related File:** [ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb)

### 2. Mid-Table Grid Gaps & Blank Row Alignment
- **Issue:** Padded blank rows caused disjointed horizontal grid lines and gaps between Income and Expense columns.
- **Solution Rule:** Use a unified `currentY` tracker and `maxRows = Math.Max(countIncome, countExpense)`. Both side columns MUST increment Y-coordinates synchronously in a single unified rendering loop.
- **Related File:** [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb)

### 3. Page 2 Multi-Page Overflow on Low-Transaction Reports
- **Issue:** Hardcoded row limits forced small reports (4-5 transactions) onto Page 2.
- **Solution Rule:** Implement dynamic row capacity calculations so that Header + Table + Summary Footer + Signatures strictly remain on Page 1 for short transaction counts.
- **Related File:** [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb)

### 4. Official Temple Accounting Summary Footer Layout & Row Alignment
- **Issue:** Redundant รวมทั้งสิ้น rows, incorrect amount cell bordering, and mismatched row alignment between Income and Expense sides.
- **Solution Rule (Official Format):**
    - **Row 1:** Income รวมรายรับ (`_totalIncome`) | Expense รวมรายจ่าย (`_totalExpense`).
    - **Row 2 (Horizontally Aligned):** Income รวมทั้งสิ้น with a 4-sided `DrawRectangle` box around net balance (`_totalIncome - _totalExpense`) | Expense ยอดคงเหลือ ณ ... in red text.
    - **Row 3:** Income side **LEFT BLANK** | Expense รวมทั้งสิ้น with a 4-sided `DrawRectangle box around grand total `_totalIncome`.
- **Bordering Rule:** Text labels (รวมทั้งสิ้น) MUST be plain text without full-row table grid borders. Only numerical amount cells get enclosed in `g.DrawRectangle`.
- **Related File:** [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb)

### 5. Header Formatting Standard
- **Issue:** Cluttered title layout and incorrect date formatting.
- **Solution Rule:**
    - **Main Title:** `สรุปบัญชีรายรับ - รายจ่าย`.
    - **Date Format:** `ประจำปี พ.ศ. ... ตั้งแต่วันที่ (๑ ... พ.ศ. ... – ๓๑ ... พ.ศ. ...) (แบบย่อ)`.
    - **Indicator:** The indicator (แบบย่อ) / (แบบรายละเอียด) must use the smaller signature font size.
- **Related File:** [ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb)
