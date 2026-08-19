# PROJECT_PROFILE_V7 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V7 (full current-state snapshot + V6→V7 UI standardization delta)
> **Generated:** 2026-08-19
> **Scope:** Complete inspection of `frmMain`, `FrmIncome`, `FrmExpense`, `FrmTransactions`, `FrmReports`, `FrmTransfer`, `FrmMasterData`, `FrmTempleSetting` + the OLEDB data layer (`Database.vb`, `Services\*`). Documents the **V7 layout standardization** (metric strip, gold table header, fixed 10-row grid height, unified action-button row) and the full database schema.
> **Companion documents (historical, NOT modified):** `PROJECT_PROFILE_V6.md`, `V5`, `V4`, `V3`, `V2`, `Project_Profile_Document.md`.

---

## 1. Executive Overview & Tech Stack

### 1.1 Application Identity

| Field | Value |
|---|---|
| Product name | Temple Accounting System — ระบบบัญชีวัด (lamYangI&E) |
| Purpose | บันทึกบัญชีรับ-จ่ายเงินวัด: รายรับ (เงินบริจาค), รายจ่ายของวัด, โอนเงินภายในกองทุน/บัญชีธนาคาร, รายงานสรุป (ละเอียด/ย่อ/รายเดือน), แนบรูปใบเสร็จ, นำเข้า/ส่งออกข้อมูลข้ามเครื่อง, สนับสนุน พร้อมเพย์ / e-Donation |
| Publisher (installer) | วัดดอนเมือง |
| Version | 1.0.0 (Inno Setup `TempleAccountingSetup.iss`, `SimpleInstaller.ps1`) |
| License | The Unlicense |
| Solution | `TempleAccounting.slnx` (SDK-style, `EnableDefaultCompileItems=false`, explicit `<Compile>`/`<EmbeddedResource>` items) |
| Output | `lamYangI&E.exe` (`bin\Release\net10.0-windows`) |

### 1.2 Technology Stack

| Layer | Technology |
|---|---|
| Language | VB.NET, `Option Strict Off` / `Option Explicit On`, `StartupObject = TempleAccounting.Program` |
| Runtime | .NET 10.0-windows (`net10.0-windows`) |
| UI | WinForms; panel-hosted (non-MDI) child forms; standard font **Tahoma**; `AutoScaleMode.Font` (baseline), `AutoScaleMode.Dpi` on `FrmTempleSetting` / `FrmMultiMachineImport` / `FrmChartPreview` |
| Database | MS Access `.accdb` via OLEDB `Microsoft.ACE.OLEDB.12.0` (NuGet `System.Data.OleDb` 10.0.10); self-healing schema via `Db.EnsureSchema()` |
| Data layer | Provider-agnostic facade `Db` → `IDbProvider` (`AccessDbProvider`, `SqlServerDbProvider`, `DbProviderFactory`); repository pattern `ITransactionRepository` / `AccessTransactionRepository` / `TransactionService` |
| Excel | `ExcelTransactionImporter.vb` (นำเข้ารายรับ/รายจ่าย), `TransactionExcelExporter.vb` (ส่งออก CSV/Excel), `TransferFormat.vb` |
| Reports | GDI+ `PrintDocument` subclass `IncomeExpenseReport` + `ReportEngine` module; charts via `System.Windows.Forms.DataVisualization` |
| Help | Markdown `USER_MANUAL_TH.md` + `HelpSystem` / `FrmHelpDialog` (F1 contextual, 12 sections) |

### 1.3 Build Verification

- `dotnet build -c Release` → `Build succeeded, 0 Warning(s), 0 Error(s)` (re-verified at V7, 2026-08-19).
- Run: `Start-Process ".\bin\Release\net10.0-windows\lamYangI&E.exe"`.

---

## 2. Core UI Layout & Navigation Architecture

### 2.1 Parent Dashboard (`frmMain.vb`)

**Panel-hosted navigation — NOT MDI.** Child forms are hosted inside `pnlFormHostBody` (fallback `pnlFormHost`) with `TopLevel = False`, `FormBorderStyle = None`, `Dock = Fill`, `BackColor = (255,253,244)`, `Font = Tahoma 10.5`.

**Dashboard visibility rules (strict contract):**

| State | `pnlOverview` | `pnlFormHostHeader` | `picHomeLogo` | `lblFormHostHint` |
|---|---|---|---|---|
| Home (Dashboard) | `Visible = True` | `Visible = True` | `Visible = True` | `Visible = True` |
| Child form open | `Visible = False` | `Visible = False` | `Visible = False` | hidden |

- `ShowDashboard()` — `CloseActiveForm()` → show logo → `pnlOverview.Visible = True` + `pnlFormHostHeader.Visible = True` → `SetActiveButton(btnDashboard)` → title `"🪟 หน้าหลัก - ภาพรวมงานประจำวัน"` → `LoadActualDashboardFromDb()` (fallback `LoadSampleDashboardData()`).
- `ShowFormInPanel(childForm, titleOverride)` — `CloseActiveForm()` → hide logo + overview + header → assign host, `Dock = Fill`, `Show()`.
- `CloseActiveForm()` — removes the child from the host, then `Close()` + `Dispose()` (defensive `IsDisposed` guard).

**Nav menu → child form mapping (`NavMenu_Click` Select Case on `clickedBtn.Name`):**

| Button | Target form | Panel title |
|---|---|---|
| `btnDashboard` | — (dashboard) | 🪟 หน้าหลัก - ภาพรวมงานประจำวัน |
| `btnDonation` | `FrmIncome` | 💰 บันทึกรายรับเงินบริจาค |
| `btnExpense` | `FrmExpense` | 💸 บันทึกรายจ่ายของวัด |
| `btnReport` | `FrmReports` | 📊 ศูนย์รายงานและส่งออก CSV/Excel |
| `btnMember` | `FrmTransactions` | 👥 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ) |
| `btnVip` | `FrmTempleSetting` | 🥇 ข้อมูลวัด - พระ/อาวาส/ผู้ทำบัญชี/พร้อมเพย์ |
| `btnActivity` | `FrmTransfer` | 🎎 โอนเงินภายในระหว่างกองทุน/บัญชีธนาคาร |
| `btnSetting` | `FrmMasterData` | ⚙️ จัดการข้อมูลหลัก ประเภท/กองทุน/บัญชี และนำเข้าจังหวัด |
| `btnMultiImport` | `FrmMultiMachineImport` (**modal `ShowDialog`**) | นำเข้าข้อมูลหลายเครื่อง |
| `btnExportTransfer` | `FrmExportForTransfer` (**modal `ShowDialog`**) | ส่งออกข้อมูลไปเครื่องอื่น |

> Multi-machine import/export deliberately bypass `ShowFormInPanel` (modal dialogs) to prevent the form being disposed mid-merge/export.

**Other frmMain behaviors:** F1 → `HelpSystem.ShowManual("FrmMain")`; Ctrl+Shift+D or triple-click logo → developer credit; status-center double-click opens DB folder / right-click copies DB path; auto-backup on `FormClosing` via `DatabaseBackupHelper.BackupFullSystem(isAuto:=True)`; dashboard 4-card overview with dynamic metric resolution (`OverviewMetric` enum) against real DB.

### 2.2 Child Forms Design Standard

**Action button row (all entry forms — `FrmIncome`, `FrmExpense`):** four buttons, one horizontal row, same Y, `Anchor = Top Or Left` (locked, no stretching):

| Button | Text | Size | BackColor | Notes |
|---|---|---|---|---|
| `btnSave` | 💾 บันทึกรายการ | 240 × 56 | green `(22,163,74)` income / orange `(180,83,9)` expense | enters edit mode → "💾 บันทึกการแก้ไข" |
| `btnCancel` | ❌ เคลียร์ (ล้างข้อมูล) | 180 × 56 | orange `(180,83,9)` income / slate `(75,85,99)` expense | edit mode → "🔙 ย้อนกลับ" + return to Transactions |
| `btnImportExcel` | 📥 นำเข้าจาก Excel | 240 × 56 | blue `(37,99,235)` | hidden in edit mode |
| `btnClose` | ❌ ปิด | 140 × 56 | charcoal `(64,64,64)` | **`btnClose.Location.X = btnImportExcel.Right + 8`** (e.g. 750+240=990 → X=998) |

- Font: Tahoma 11 Bold (all four buttons); `FlatStyle.Flat`, `Cursor = Hand`, white text.
- **`btnClose` behavior (returns to dashboard):**
  ```vb
  Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
      Dim f = TryCast(Me.ParentForm, frmMain)
      If f IsNot Nothing Then
          f.CloseActiveForm()
          f.ShowDashboard()
      End If
  End Sub
  ```
- **FrmIncome vertical layout is code-locked** via `ApplyFixedVerticalLayout()` (`FrmIncome.vb`): remark height 55, 8px Y gaps between field rows, `btnSave.Top = txtReceipt.Bottom + 12`, action buttons keep `Anchor = Top Or Left` only. `FrmExpense` uses equivalent fixed coordinates in the Designer.
- Receipt row: `btnBrowseReceipt` (📂 เลือกรูปภาพ), `btnPasteReceipt` (📋 วางจาก LINE), `btnClearReceipt` (🗑️) — Clipboard-paste support; clears clipboard after save to avoid duplicate pastes.
- Enter-key navigation (`_enterFlow`), F1 help, `UiFitter.AutoFitFormButtons` + `DisableComboBoxWheel` on load.

---

## 3. Metric Strip & DataGridView Standards

### 3.1 Top Summary Strip (`lblSummary`)

One full-width, center-aligned strip docked to the top of `FrmIncome` / `FrmExpense` (replaces the legacy four separate metric labels):

| Property | Value |
|---|---|
| `Dock` | `DockStyle.Top` |
| Height | 36 px (`Size = New Size(1773, 36)` income / `(1639, 36)` expense) |
| BackColor | warm amber `(254, 240, 138)` |
| Font | Tahoma 11 Bold |
| ForeColor | dark brown `(69, 26, 3)` |
| TextAlign | `ContentAlignment.MiddleCenter` |

**Text pattern (vertical `|` separators, unit = "ครั้ง"):**

```
FrmIncome : รายรับเดือนนี้: 8 ครั้ง  |  สูงสุด: 2,500.00 บาท  |  เฉลี่ย: 887.50 บาท  |  รวมรายรับ: 7,100.00 บาท
FrmExpense: รายจ่ายเดือนนี้: X ครั้ง  |  สูงสุด: X.XX บาท  |  เฉลี่ย: X.XX บาท  |  รวมรายจ่าย: X.XX บาท
```

**Terminology standard (enforced project-wide):** use **"ครั้ง"** as the default transaction count unit (ใบ→ครั้ง for transaction counts). Money always `#,##0.00` (`:n2`).

### 3.2 Recent-Transactions Grid (`dgvRecentTransactions`)

**Fixed 10-row height standard (no vertical stretching):**

| Property | Value |
|---|---|
| `Anchor` | `AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right` (NO `Bottom`) |
| Height | **412 px** (both forms, V7) — header + exactly 10 rows, no empty space below |
| Location | `(766, 113)` income / `(766, 102)` expense — `Top = lblRecentHeader.Bottom` |
| `ScrollBars` | `Both` (rows overflow scrolls internally) |
| ReadOnly / no user add / no delete / no row resize | enforced |
| Column sizing (code, `ConfigureRecentGridColumns`) | Date 90 + centered, ประเภท 110, กองทุน 120, จำนวนเงิน 90 right-aligned (`DisplayedCells`), รายละเอียด `Fill` min 350 |

**Query (TOP 10):**
```sql
SELECT TOP 10 t.TranDate AS TransactionDate, c.CategoryName, f.FundName, t.Amount, t.[Detail] AS [Description]
FROM ((Transactions t
LEFT JOIN Categories c ON t.CategoryID = c.ID)
LEFT JOIN Funds f ON t.FundID = f.ID)
WHERE t.TranType = 'Income'  -- or 'Expense'
ORDER BY t.TranDate DESC, t.ID DESC
```

### 3.3 Gold Table Header (`lblRecentHeader`)

Standard yellow/gold header bar directly above the grid (same parent container, direct form children):

| Property | Value |
|---|---|
| Text | 📋 รายการ**รายรับ**ล่าสุด 10 รายการ / 📋 รายการ**รายจ่าย**ล่าสุด 10 รายการ |
| BackColor | Gold `(241, 196, 15)` = **#F1C40F** |
| Font | Tahoma 10 Bold |
| ForeColor | `Color.Black` |
| TextAlign | `ContentAlignment.MiddleCenter` |
| Height | 28 px; width = grid width (991 income / 857 expense) |
| Anchor | `Top Or Left Or Right` |
| Z-order | added after `dgvRecentTransactions` (sits above it); `Controls.Add` order: grid → header → summary |

Positioning rule: `lblRecentHeader.Bottom == dgvRecentTransactions.Top` (no overlap, no gap).

---

## 4. Database Schema & Business Logic

### 4.1 Database & Connectivity

- File: `TempleAccounting.accdb` (path resolved by `AppPaths.DatabaseFile`).
- Connection (Access): `Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;`
- Facade `Db` (module) delegates to `IDbProvider.Current`:
  - `OpenConn()`, `EnsureSchema()`, `ExecuteNonQuery(conn, sql, params)`, `DbScalar(...)`, `GetTable(...)`, `InsertAndGetId(...)` (via `SELECT @@IDENTITY`), `NormalizeGregorianDate`, `AccessDateLiteral`, `EscapeLikeText`, `ToDecimalOrZero`, `ToIntOrZero`, `BackupDatabase`.
- `SqlServerDbProvider` exists for a future SQL Server switch (`DbProviderFactory.ProviderType`).
- `EnsureSchema()` is idempotent: `TryCreateTable` (checks `GetSchema("Tables")`), then migrations (`MigratePositions`, `MigrateTempleSetting`, `MigratePersonnel`, `MigrateTransactions`), then seeders (Categories / Funds / BankAccounts / Positions / Personnel — only when empty).

### 4.2 Table Schema (Access DDL)

**Core ledger table — `Transactions`:**

| Column | Type | Notes |
|---|---|---|
| `ID` | COUNTER PK | auto-increment |
| `TranDate` | DATETIME NOT NULL | Gregorian-normalized |
| `TranType` | TEXT(10) NOT NULL | `Income` / `Expense` / `Transfer` |
| `CategoryID` | INTEGER | → `Categories.ID` |
| `FundID` | INTEGER | → `Funds.ID` |
| `BankID` | INTEGER | → `BankAccounts.ID` (nullable) |
| `Detail` | TEXT(255) | description |
| `Amount` | CURRENCY NOT NULL | money |
| `Note` | MEMO | remark |
| `CreateDate` | DATETIME DEFAULT Now() | |
| `ToFundID` | INTEGER | transfer target fund (migrated) |
| `ToBankID` | INTEGER | transfer target bank (migrated) |
| `ReceiptPath` | TEXT(255) | attached receipt image (migrated) |
| `TransactionGUID` | TEXT(36) | cross-machine dedup (migrated) |
| `ReceiveChannel` | TEXT(20) | Cash / BankTransfer / PromptPay / eDonation (migrated) |
| `ReferenceNo` | TEXT(50) | PromptPay/e-Donation ref (migrated) |

**Support tables:**

| Table | Columns |
|---|---|
| `Categories` | `ID` COUNTER PK, `CategoryName` TEXT(200) NOT NULL, `TranType` TEXT(10) NOT NULL |
| `Funds` | `ID` COUNTER PK, `FundName` TEXT(200) NOT NULL |
| `BankAccounts` | `ID` COUNTER PK, `BankName` TEXT(100) NOT NULL, `AccountNo` TEXT(50), `AccountName` TEXT(200) |
| `TempleSetting` | `ID` COUNTER PK, `TempleCode`, `TempleName`, `TempleAddress` MEMO, `Tambon`, `Amphoe`, `Province`, `PostCode`, `TemplePhone`, `AbbotPersonnelID`, `WaiyawatPersonnelID`, `BookkeeperPersonnelID`, `PromptPayName`, `PromptPayID` |
| `Personnel` | `PersonnelID` COUNTER PK, `Title`, `FirstName`, `LastName`, `FullName`, `PersonType` (Monk/Layperson), `PositionID`, `Phone` |
| `Positions` | `PositionID` COUNTER PK, `PositionName` TEXT(100), `PositionGroup` TEXT(20) (พระ/ฆราวาส/ทั่วไป) |
| `Province` / `District` / `SubDistrict` | address import (ID PK + name columns + `ZipCode`) |

**Seed data:** 10 Categories (5 Income / 5 Expense), 4 Funds (กองทุนทั่วไป / กฤติยาธรรม / กู้ภัยวัด / ซ่อมแซม), 1 BankAccount (ธนาคารออมสิน — วัดแหลมยาง), 19 Positions, 6 sample Personnel.

### 4.3 Monthly Summary Queries (Gregorian/Buddhist year + null-safety)

**Null-safe aggregates with `IIF(ISNULL(...))` fallback:**
```sql
SELECT COUNT(*) AS TotalCount,
       IIF(ISNULL(MAX(Amount)),0,MAX(Amount)) AS MaxAmount,
       IIF(ISNULL(AVG(Amount)),0,AVG(Amount)) AS AvgAmount,
       IIF(ISNULL(SUM(Amount)),0,SUM(Amount)) AS TotalAmount
FROM Transactions
WHERE (TranType LIKE '%รับ%' OR TranType = 'Income')        -- expense: '%จ่าย%' OR 'Expense'
  AND MONTH(TranDate) = MONTH(Date())
  AND (YEAR(TranDate) = YEAR(Date()) OR YEAR(TranDate) = 2026)   -- Buddhist year (2569) fallback
```

**Year compatibility hard rules:**
1. **Buddhist-year guard** `OR YEAR(TranDate) = 2026` — if the PC's Thai locale stored dates in Buddhist era (e.g., 2569), the month still matches; Gregorian-year rows (2026) also match.
2. **`Db.NormalizeGregorianDate`** — converts any `value.Year > 2400` by subtracting 543 (Buddhist → Gregorian) before display/use.
3. **`Db.AccessDateLiteral`** — emits `#yyyy-MM-dd HH:mm:ss#` literals (safe for Access).
4. Null-safe VB access via `Db.ToDecimalOrZero` / `Db.ToIntOrZero`.

**Other key SQL patterns:** balance = `SUM(IIF(TranType='Income',Amount,0)) - SUM(IIF(TranType='Expense',Amount,0))` (used by dashboard cards, report carry-forward, `GetBalanceBeforeDate` in `ReportEngine`); dashboard card metrics use `MONTH(TranDate) = MONTH(Date()) AND (YEAR(...) = YEAR(Date()) OR YEAR(...) = 2026)`.

### 4.4 Business Services

- `TransactionService` + `AccessTransactionRepository` (used by `FrmIncome`): `SaveIncome`, `UpdateIncome`, `UpdateReceiptPath` — INSERT/UPDATE of income records incl. channel/reference; Expense form uses direct `Db.ExecuteNonQuery`/`InsertAndGetId`.
- Every INSERT generates a `TransactionGUID` (`Guid.NewGuid().ToString("D")`) immediately — identity key for cross-machine export/import dedup (SHA-256 + `ImportHistory`).
- `ReportEngine.BuildReportDataset` — single source of truth for report data: opening balance (manual override or `GetBalanceBeforeDate`), income/expense row lists, "ยอดยกมา" carry-forward row 0, `GrandTotal` / `Balance`.

---

## 5. Change Log & V7 Migration Summary

### 5.1 V7 UI/UX Standardization Delta (this profile)

| # | Change | Files | Rationale |
|---|---|---|---|
| 1 | **Unified top summary strip** (`lblSummary`, amber, centered, `\|` separators, "ครั้ง") replacing 4 separate metric labels | `FrmIncome.vb` / `FrmIncome.Designer.vb`, `FrmExpense.vb` / `FrmExpense.Designer.vb` | Single clean horizontal metric bar consistent with `FrmTransactions`; `lblSummary.Text` recomputed with `{count} ครั้ง` + `{value:n2}` |
| 2 | **`btnClose` ("❌ ปิด")** added to child forms + return-to-dashboard logic | `FrmIncome`/`FrmExpense`/`FrmTransfer`/`FrmReports`/`FrmMasterData` (.vb + .Designer.vb) | Users can close current view and return to dashboard; charcoal `(64,64,64)` styling |
| 3 | **`btnClose` row alignment** — `Anchor = Top Or Left`, `Location.X = btnImportExcel.Right + 8` (= 998), same Y as `btnSave`/`btnImportExcel`, `Height`/`Font` matched (Tahoma 11 Bold) | `FrmIncome.Designer.vb`, `FrmExpense.Designer.vb` | 4 action buttons sit neatly in one row: บันทึกรายการ → เคลียร์ → นำเข้าจาก Excel → ❌ ปิด |
| 4 | **Fixed grid height (10 rows)** — `Anchor = Top Or Left Or Right` (Bottom removed), fixed `Height = 412` | `FrmIncome.Designer.vb`, `FrmExpense.Designer.vb` | Grid no longer stretches to form bottom; exactly 10 rows visible |
| 5 | **Gold table header restored** — `lblRecentHeader` (📋 รายการรับ/จ่ายล่าสุด 10 รายการ) above grid, `#F1C40F`, Tahoma 10 Bold, black, centered, 28 px, `Anchor Top/Left/Right`, `Controls.Add` order grid→header→summary | `FrmIncome.Designer.vb`, `FrmExpense.Designer.vb` | Identical yellow header bar on both forms; `Top = header.Bottom` guarantees no overlap |
| 6 | **FrmExpense fully mirrors FrmIncome layout** — metric strip, header, grid height, button alignment | `FrmExpense.Designer.vb`, `FrmExpense.vb` | 100% consistent design between the two entry forms |
| 7 | **Fix missing-total bug** — `lblSummary` text now appends the calculated sum (`รวมรายจ่าย: {total:n2} บาท`) with `AutoSize` | `FrmExpense.vb` / `FrmIncome.vb` | Total value no longer missing on the strip |

### 5.2 Legacy Context (from V6 baseline, carried forward)

- V6: `FrmChartPreview` full-screen chart (zoom/pan/toggle-totals/print/save), preview-first PDF export with toolbar "💾 บันทึกเป็น PDF", bottom-anchored signature block, dynamic page-row balancing, F1 manual sections 10–11, centralized `ReportEngine`.
- V5: `FrmMultiMachineImport` DPI rebuild, print hardening + `UseLandscape`/safe margins, `btnSavePdf`, multi-machine forms.
- V4: 8-button toolbar baseline, 12-item known-issue log.
- V3: UI auto-fit (`UiFitter`), personnel-ID migration.
- V2: post-refactor state, F1 Help System intro.

### 5.3 Outstanding Items (still open)

1. `FrmTransactions` in-grid edit path (`SaveSelectedRowEdits`) remains dead code — Transfer-row editing has no working save.
2. Two corrupted `�` glyphs remain on `FrmTransactions` receipt-retrofit buttons.
3. `FrmTransactions` debug-report harness (`http://127.0.0.1:7777/event`) still wired in.
4. `FrmLocationImport` CSV parser remains naive (no quoted-field handling).
5. `FrmPersonnelManagement` `fullName` title gluing (no space); Positions delete lacks friendly FK-in-use message.
6. `LoadMonkSample()` in frmMain still reads legacy TempleSetting columns (guarded only by `IsDBNull`).
7. `BaseReport` remains dead scaffolding; `AutoScaleMode` still mixed across forms.

### 5.4 Environment & Build

| Item | Value |
|---|---|
| OS | Windows (PowerShell; timezone Asia/Bangkok) |
| Build | `dotnet build -c Release` → 0 warnings / 0 errors |
| Runtime deps | .NET 10 preview SDK; 64-bit Microsoft Access Database Engine 2016 (OLEDB); `Microsoft Print to PDF` virtual printer |
| Installer | Inno Setup `TempleAccountingSetup.iss` (v1.0.0) |
| DB | `TempleAccounting.accdb`, schema self-healing via `Db.EnsureSchema()` |
| Help doc | `USER_MANUAL_TH.md` copied to output (see `.vbproj`) |

---

*End of PROJECT_PROFILE_V7.md — full current-state snapshot + V6→V7 delta: standardized amber metric strip ("ครั้ง" unit), gold `lblRecentHeader` table header, fixed 412 px 10-row grid height (no Bottom anchor), unified 4-button action row with aligned `btnClose` (right of `btnImportExcel` + 8 px), FrmIncome↔FrmExpense 100% design parity, and the complete Access schema / OLEDB helper / Buddhist-year SQL conventions. `PROJECT_PROFILE_V6.md` (and earlier profile documents) and all application source files remain unmodified.*
