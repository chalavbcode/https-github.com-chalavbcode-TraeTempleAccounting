# PROJECT_PROFILE_V3

## Project Identity & Architecture

### Technology Stack
- **Language:** Visual Basic .NET (WinForms / .NET Framework)
- **Database:** Microsoft Access Database (`TempleAccounting.accdb`)
- **Data Access:** OLEDB provider with custom repository helpers for SQL execution and schema management.
- **Reporting:** Custom GDI+ Drawing Engine for pixel-perfect A4 report generation.

### Application Flow
1. **Entry Point ([Program.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Program.vb)):** Initializes application, checks database connectivity, and handles global exceptions.
2. **Main Dashboard:** Central navigation hub for all accounting and management modules.
3. **Database Layer ([Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb)):** Manages connection pooling, SQL literals (especially Access-specific date formatting), and automated schema updates.

---

## Core Modules & Business Logic

### 1. Transaction Management ([FrmTransactions.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb))
- Handles Income and Expense entry.
- Supports category-based classification and fund association.
- Integrated search and filtering capabilities.

### 2. Fund Transfer System ([FrmTransfer.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransfer.vb))
- Logic for moving money between different funds or bank accounts.
- Ensures double-entry consistency for internal transfers.

### 3. Personnel Management ([FrmPersonnelManagement.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmPersonnelManagement.vb))
- Tracks temple personnel, including roles (Abbot, Waiyawat, etc.) and profiles.
- Linked to report signature logic for identity mapping.

### 4. Temple Settings ([FrmTempleSetting.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTempleSetting.vb))
- Stores temple metadata: name, address, and official representatives.
- Source of truth for report headers and signatures.

### 5. Reporting Engine ([ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb) & [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb))
- **ReportEngine:** Provides reusable GDI+ methods for drawing headers, footers, borders, and Thai page numbering.
- **IncomeExpenseReport:** Implements the core logic for Summary and Detailed financial reports, including opening balance calculation and complex dual-column grid rendering.

---

## Database & Data Structure Summary

### Connection & Access
- Centralized through [Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb).
- Uses `Db.NormalizeGregorianDate()` and `Db.AccessDateLiteral()` to ensure MS Access date compatibility (YYYY-MM-DD format wrapped in `#`).

### Key Entities
- **Transactions:** Core table for financial entries (`TranDate`, `Amount`, `CategoryID`, `FundID`).
- **Categories:** Master list of income/expense types.
- **TempleSetting:** Identity and personnel configuration.
- **Personnel:** Directory of temple members.

---

## Critical Resolved Issues & Anti-Regression Rules (HIGH PRIORITY)

### 1. Footer Signature Cut-off (A4 Overflow)
- **Problem:** Signatures (Abbot, Waiyawat) cut off at the bottom of A4 pages.
- **Rule:** Maintain a Y-coordinate offset with 15-25px clearance from the printable bottom margin. Vertical spacing between name and title lines must be at least `Font.Height + 12px`.

### 2. Dynamic Table Height & Gap Elimination
- **Problem:** Mismatched row counts between Income and Expense columns caused grid gaps.
- **Rule:** Use a unified `currentY` tracker and `maxRows = Math.Max(countIncome, countExpense)`. Both columns MUST increment Y-coordinates synchronously in a single loop to ensure seamless horizontal grid lines.

### 3. A4 Page 1 Enforcement
- **Problem:** Small reports unnecessarily spilled onto Page 2.
- **Rule:** Dynamically calculate remaining space. If (Header + Table + Summary + Signatures) fits, they MUST be rendered on Page 1. The signature block threshold is approximately 235px.

### 4. Official Temple Summary Footer Layout (Strict Format)
- **Row 1:** Income "รวมรายรับ" (`_totalIncome`) | Expense "รวมรายจ่าย" (`_totalExpense`).
- **Row 2 (Aligned):**
    - **Income Side:** "รวมทั้งสิ้น" text with a 4-sided `DrawRectangle` box around net balance (`_totalIncome - _totalExpense`).
    - **Expense Side:** "ยอดคงเหลือ ณ ... ยกไปปี ..." in **Red Text**.
- **Row 3:**
    - **Income Side:** **LEFT BLANK** (No text or grid).
    - **Expense Side:** "รวมทั้งสิ้น" text with a 4-sided `DrawRectangle` box around grand total (`_totalIncome`).
- **Bordering Rule:** "รวมทั้งสิ้น" labels must be plain text. Only numerical amount cells are enclosed in `g.DrawRectangle`.

### 5. Header Formatting Standard
- **Main Title:** `สรุปบัญชีรายรับ - รายจ่าย`.
- **Date String:** `ประจำปี พ.ศ. ... ตั้งแต่วันที่ (๑ ... พ.ศ. ... – ๓๑ ... พ.ศ. ...)`.
- **Indicator:** Report type labels like `(แบบย่อ)` or `(แบบละเอียด)` must be appended to the date line using the smaller signature font size (10pt).

### 6. Contextual Help System (F1 Shortcut)
- **Problem:** Users unable to access help documentation quickly.
- **Rule:** Every primary form MUST implement F1 support.
    - **KeyPreview:** Set `Me.KeyPreview = True` in `Form_Load`.
    - **KeyDown Handler:** Use `Handles Me.KeyDown` to catch `Keys.F1`.
    - **Logic:** Call `HelpSystem.ShowManual("FormName")` and set `e.Handled = True` / `e.SuppressKeyPress = True`.
    - **Visual Hint:** Call `HelpSystem.SetupHelp(Me, "FormName")` in `Form_Load` to add the Status Bar instruction label.
