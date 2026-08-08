# PROJECT_PROFILE_V4 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V4 (exhaustive audit)
> **Generated:** 2026-08-08
> **Scope:** Every subsystem, UI module, database table, data flow, helper module, business rule, and historical refactoring record in the repository `TempleAccounting_FullProject_20260726_090422`.
> **Companion documents (kept as historical references, NOT modified):** `PROJECT_PROFILE_V3.md`, `PROJECT_PROFILE_V2.md`, `Project_Profile_Document.md`.

---

## 1. Executive Summary & Architecture Overview

### 1.1 What This System Is

The **Temple Accounting System (ระบบบัญชีวัด — "TempleAccounting")** is a desktop accounting application built for Thai Buddhist temples. It records cash income (donations), cash expenses, internal fund/bank transfers, manages temple personnel (abbot, vice-abbot, lay treasurers, bookkeepers), holds the temple's profile and contact data (including PromptPay), produces official-format financial reports on A4 paper, exports data to CSV/Excel, imports donations from Excel, attaches receipt images, and performs full-system backup/restore.

The working UI language is **Thai**. The Buddhist calendar is handled everywhere: dates typed or stored in Buddhist year (CE + 543) are normalized to Gregorian before SQL work, and displayed back in Thai format.

### 1.2 Technology Stack

| Layer | Technology | Notes |
|---|---|---|
| Language | Visual Basic .NET (VB.NET), `Option Strict Off` / `Option Explicit On` on almost every file | `FrmPersonnelManagement.vb` has no Option statements at all |
| Runtime | .NET 10.0-windows (`net10.0-windows`), SDK-style project, preview SDK 10.0.400-preview.0.26322.102 | `StartupObject = TempleAccounting.Program` |
| UI | WinForms (`System.Windows.Forms`), `AutoScaleMode.Font` (default) on most forms; `FrmTempleSetting` uses `AutoScaleMode.Dpi`; `frmMain` and `FrmTransactions` use explicit `AutoScaleMode.Font` | Standard font Tahoma 10.5pt; toolbar buttons uniform via `UiFitter.UniformButtonGroup` |
| Database | Microsoft Access `.accdb` via OLEDB `Microsoft.ACE.OLEDB.12.0` (NuGet `System.Data.OleDb` 10.0.10) | `Database\TempleAccounting.accdb`; schema self-healed by `Db.EnsureSchema()` |
| Charts | `System.Windows.Forms.DataVisualization` (NuGet `WinForms.DataVisualization` 1.10.2) | `FrmReports.chartMonthly` bar chart |
| Printing | GDI+ `PrintDocument` subclass (`IncomeExpenseReport`) + `PrintPreviewDialog` | A4 landscape, pixel-exact |
| Help | Markdown-based `USER_MANUAL_TH.md` + custom `HelpSystem` / `FrmHelpDialog` | F1-key contextual help |
| Deployment | Inno Setup (`TempleAccountingSetup.iss`), `SimpleInstaller.ps1`, The Unlicense license | Version 1.0.0, publisher "วัดดอนเมือง" |
| Packaging | `EnableDefaultCompileItems=false` — all items explicit in `.vbproj` | `TempleAccounting.slnx` solution |

### 1.3 Physical Architecture (single-process WinForms)

```
Program.vb  ──►  Application.Run(New frmMain)      (multi-layered exception "Beacon")
                 │
                 ├── frmMain (shell, FormBorderStyle.None, custom chrome)
                 │     ├── pnlSidebar  → 10 nav buttons (dashboard/donation/expense/report/member/vip/activity/setting/backup/restore)
                 │     ├── pnlOverview → 4 KPI cards (lblCard1..4) refreshed per-screen from real DB SQL
                 │     ├── pnlFormHost └ pnlFormHostBody   ← child forms hosted here
                 │     │        ShowFormInPanel(childForm, title): TopLevel=False, Border=None, Dock=Fill
                 │     │        CloseActiveForm(): Remove+Close+Dispose
                 │     └── pnlStatus (status bar) + pnlHeader (custom title bar)
                 │
                 ├── Child forms (hosted inside frmMain): FrmIncome, FrmExpense, FrmTransfer,
                 │     FrmTransactions, FrmReports, FrmTempleSetting, FrmMasterData, FrmLocationImport
                 ├── Modal dialogs: FrmPersonnelManagement (ShowDialog), FrmHelpDialog (ShowDialog)
                 └── Shared modules: Db, AppPaths, UiFitter, HelpSystem, MessageBoxHelper,
                       ReceiptImageHelper, ExcelTransactionImporter, DatabaseBackupHelper
                       Reports: BaseReport (abstract scaffolding), IncomeExpenseReport, ReportEngine
```

### 1.4 Application Flow

1. **Bootstrap (`Program.vb`)** — layered Try/Catch "Beacon" handlers; `Application.SetHighDpiMode(HighDpiMode.SystemAware)`; creates main form; on catastrophic failure shows a message and writes `crash.log`.
2. **Shell (`frmMain`)** — `New()` ensures directories (`AppPaths.EnsureDirectoriesExist`), builds UI, installs emoji icons, event handlers, card hover effects, status clock. `FrmMain_Load` sets KeyPreview, calls `HelpSystem.SetupHelp(Me, "FrmMain")`, shows the 600×600 home logo, and opens the Dashboard.
3. **Navigation** — sidebar buttons call `NavMenu_Click` → `ShowFormInPanel(New FrmXxx(), title)`, hide the home logo, and refresh the 4 KPI cards from real SQL (with sample-data fallbacks on error).
4. **Data entry** — `FrmIncome`/`FrmExpense` insert `Transactions` rows (`TranType='Income'|'Expense'`); `FrmTransfer` inserts single rows with `TranType='Transfer'` plus nullable `ToFundID`/`ToBankID`.
5. **Review** — `FrmTransactions` lists/filters/searches/edits/deletes transactions and attaches/view/deletes receipt images.
6. **Reporting** — `FrmReports` provides grid summaries, official A4 printing (`IncomeExpenseReport`), a monthly bar chart, and CSV export.
7. **Settings** — `FrmTempleSetting` (temple info + personnel + PromptPay + location cascade) and `FrmMasterData` (categories/funds/bank accounts).
8. **Backup/Restore** — `DatabaseBackupHelper.BackupFullSystem` (full system: DB + receipt images) and `RestoreFullSystem`; automatic background backup on `FormClosing`.

### 1.5 Cross-Cutting Engineering Conventions

- **F1 contextual help:** every primary form sets `Me.KeyPreview = True`, calls `HelpSystem.SetupHelp(Me, "<FormName>")` in `Load`, and implements `Handles Me.KeyDown` → `HelpSystem.ShowManual("<FormName>", Me)` with `e.Handled = True` and `e.SuppressKeyPress = True`. `SetupHelp` appends the status-bar hint `💡 คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้าจอนี้`.
- **Button auto-fit:** every form Load calls `UiFitter.AutoFitFormButtons(Me)` to prevent Thai vowel/mark clipping.
- **ComboBox wheel protection:** `UiFitter.DisableComboBoxWheel(Me)` is wired in FrmIncome, FrmExpense, FrmTransfer, FrmTransactions, FrmTempleSetting, FrmMasterData, FrmReports (forms with ComboBoxes). FrmPersonnelManagement and FrmLocationImport do not call it (FrmLocationImport has no ComboBoxes).
- **Date normalization:** all SQL that compares dates uses `Db.AccessDateLiteral()`; all Thai Buddhist dates are normalized via `Db.NormalizeGregorianDate()` (subtracts 543 if `Year > 2400`). SQL-side idiom: `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)`.
- **DB access:** `Db.OpenConn()` per operation (`Using conn = ...`); no long-lived connections; `Db.EnsureSchema()` cached via `_schemaChecked`.
- **Error handling:** Try/Catch + `AppPaths.LogCrash(ex, tag)`; user-facing Thai `MessageBox` messages; `MessageBoxHelper` for a consistent Tahoma-8 message box font.
- **Host-panel z-order discipline:** `Controls.Add` puts a control on top; child forms are added after hiding the home logo to prevent overlay bugs.

---

## 2. Complete Form & UI Inventory

### 2.1 Shared UI Patterns (applies to every form)

| Pattern | Description |
|---|---|
| Header banner | Label with emoji + Thai title, `Dock=Top`, pastel BackColor (yellow `#FDE68A` for income/overview, red `#FECACA` for expense, purple `#E9D5FF` for transfer, blue `#BAE6FD` for location import), Tahoma 12–15pt bold |
| Form font | Tahoma 10.5pt (222 = Thai charset codepage) on nearly every form |
| Background | `Color.FromArgb(254, 249, 235)` (cream) on most forms |
| Flat buttons | `FlatStyle.Flat`, `FlatAppearance.BorderSize=0`, `Cursor=Hand`, rounded-ish modern colors (green `#16A34A`, blue `#2563EB`, red `#DC2626`, purple `#7E22CE`, gray `#4B5563`) |
| Label field convention | Left column labels `MiddleRight`, fields at x≈240, 33–40px tall |
| Enter navigation | `SetupEnterNavigation()` builds a `_enterFlow` List(Of Control); Enter advances focus (ComboBox opens dropdown, TextBox SelectAll), Enter on last control performs the primary save button |
| Load sequence | `Me.KeyPreview=True` → `HelpSystem.SetupHelp` → `Db.EnsureSchema()` → load data → `SetupToolTips` → `SetupEnterNavigation` → `UiFitter.AutoFitFormButtons(Me)` → `UiFitter.DisableComboBoxWheel(Me)` → focus first field |

---

### 2.2 frmMain — Main Shell (Dashboard / Navigation)

**Files:** `frmMain.vb`, `frmMain.Designer.vb`, `frmMain.UiHelpers.vb` (3 partials), `frmMain.resx`.

- Class `Partial Public Class frmMain : Inherits Form`; `FormBorderStyle=None` (custom chrome), `ClientSize 1440×840`, `MinimumSize 940×620`, `StartPosition=CenterScreen`, `WindowState=Maximized`, `AutoScaleMode.Font`.
- Fields: `_currentActiveButton As Button`, `_currentChildForm As Form`, `_compactOverviewMode As Boolean`, `picHomeLogo As PictureBox`.

**Layout regions (Designer):**

| Region | Control | Details |
|---|---|---|
| Header | `pnlHeader` (Dock=Top, 84px, `#78350F`) | Contains `pnlLogo` (60×60, `picLogo` gold circle + `picLogoBadge`), `lblTitle` "📿 ระบบบัญชีวัดฯ" (Tahoma 20 bold `#FFD700`), `lblSubtitle` "ระบบบัญชีวัด - Temple Accounting Software", `pnlHeaderRight` (Dock=Right, 370px) with `lblUserInfo` "👤 ผู้ดูแลระบบ", `btnMinimize` "─", `btnClose` "✕" |
| Sidebar | `pnlSidebar` (Dock=Left, 251px, `#58280C`) | 10 Dock=Top nav buttons + `btnLogout` "🚪 ออกจากระบบ" (Dock=Bottom, `#991B1B`) + `pnlSidebarSpacer`. Buttons: `btnDashboard` "🏠 หน้าหลัก" (12pt bold gold `#EAB308` active), `btnDonation` "💰 บันทึกรับเงิน", `btnExpense` "💸 บันทึกจ่ายเงิน", `btnReport` "🖨️ พิมพ์รายงาน", `btnMember` "📖 รายการทางบัญชี", `btnVip` "🥇 พระ / อาวาส", `btnActivity` "🎎 โอนเงินภายใน", `btnRestore` "🔄 คืนค่าข้อมูล", `btnBackup` "💾 สำรองข้อมูล", `btnSetting` "⚙️ ตั้งค่าระบบ" — each 227px wide, `Padding(14,0,8,0)`, ImageAlign MiddleLeft, `ilIcons` (22×22, Depth32Bit) image key |
| Overview strip | `pnlOverview` (Dock=Top, 121px) | `lblOverviewTitle` (13pt bold, `#78350F`) + `pnlCards` with 4 KPI cards `pnlCard1..4` (248×90, white, `Cursor=Hand`, Dock Left/Right alternating). Each card: `lblCardNTitle` (10pt bold `#785028`) + `lblCardNValue` (17pt bold, card-specific color: card1 amber `#A16207`, card2 green `#166534`, card3 red `#991B1B`, card4 brown `#9A3412`) |
| Form host | `pnlFormHost` (Dock=Fill) | `pnlFormHostHeader` (Dock=Top, 84px, `#FAF0D2`, `lblFormHostTitle` 14.5pt bold + `lblFormHostHint` + `pnlSeparator2` amber line) + `pnlFormHostBody` (Dock=Fill) — child forms and the home logo live here |
| Status bar | `pnlStatus` (Dock=Bottom, 30px, `#58280C`) | `lblStatusLeft` (8.5pt), `lblStatusCenter` (9.75pt, live clock + DB name, `#FEF9C3`), `lblStatusRight` "v1.0.0" |

**Event wiring (`frmMain.UiHelpers.vb`):**
- `SetupEventHandlers()` — all nav buttons → `NavMenu_Click`; `btnBackup`/`btnRestore` → dedicated handlers; cards → `OverviewCard_Click`; window chrome → `BtnClose_Click`/`BtnMinimize_Click`/`BtnLogout_Click`; status label → `StatusCenter_DoubleClick` / `StatusCenter_MouseClick`; `Load`/`FormClosing` handlers.
- `SetupCardHoverEffects()` / `ApplyCardHover(card, highlight)` — hover highlight colors (yellow `#FEF9C3`, green `#DCFCE7`, purple `#F3E8FF`, orange `#FFEDD5`); child labels share the hover + click bridge.
- `SetupIconsAndImages()` — `ilIcons` keys: home 🏠 `#451A03`, donation 💰 `#166534`, expense 💸 `#991B1B`, report 🖨️ `#1E40AF`, member 📖 `#7C2D12`, vip 🥇 `#A16207`, activity 🎎 `#831843`, setting ⚙️ `#4B5563`, backup 💾 `#059669`, restore 🔄 `#3B82F6`. `picLogo` 📿 and `picLogoBadge` 🏛️ rendered via `MakeIconBitmap`.
- `MakeIconBitmap(emoji, color, size, fontSize)` — renders emoji into a transparent 28×28 bitmap using "Segoe UI Emoji", anti-aliased, center-aligned with 8% inset.
- `ApplyInitialState()` — starts 1-second `WinTimer` for `UpdateStatusTime()`.
- `UpdateStatusTime()` — `🟢 สถานะระบบ: ปกติ | ฐานข้อมูล: <name> | dd/MM/yyyy HH:mm:ss`.
- `SetupToolTips()` — Thai tooltips for all nav buttons.

**Navigation & hosting logic (`frmMain.vb`):**
- `NavMenu_Click(sender, e)` — Select Case on `clickedBtn.Name`:
  - `btnDashboard` → `SetOverviewCompactMode(False)`, `ShowDashboard()`
  - `btnDonation` → `ShowFormInPanel(New FrmIncome(), "💰 บันทึกรายรับเงินบริจาค")` + `LoadActualDonationOverviewFromDb()` (fallback `LoadDonationSample`)
  - `btnExpense` → `ShowFormInPanel(New FrmExpense(), "💸 บันทึกรายจ่ายของวัด")` + expense overview
  - `btnReport` → `ShowFormInPanel(New FrmReports(), "📊 ศูนย์รายงานและส่งออก CSV/Excel")` + report overview
  - `btnMember` → `SetOverviewCompactMode(True)`, `ShowFormInPanel(New FrmTransactions(), "👥 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ)")` + `LoadTransactionOverview()`
  - `btnVip` → `ShowFormInPanel(New FrmTempleSetting(), "🥇 ข้อมูลวัด - พระ/อาวาส/ผู้ทำบัญชี/พร้อมเพย์")` + `LoadMonkSample()`
  - `btnActivity` → `ShowFormInPanel(New FrmTransfer(), "🎎 โอนเงินภายในระหว่างกองทุน/บัญชีธนาคาร")` + `LoadActivitySample()`
  - `btnSetting` → `ShowFormInPanel(New FrmMasterData(), "⚙️ จัดการข้อมูลหลัก ประเภท/กองทุน/บัญชี  และนำเข้าจังหวัด")` + `LoadSettingSample()`
- `ShowDashboard()` — `CloseActiveForm()`, **re-shows `picHomeLogo`**, `SetActiveButton(btnDashboard)`, sets header title "🪟 หน้าหลัก - ภาพรวมงานประจำวัน", shows hint, `LoadActualDashboardFromDb()` (fallback `LoadSampleDashboardData`).
- `ShowFormInPanel(childForm, titleOverride)` — `CloseActiveForm()`, **hides `picHomeLogo`** (prevents logo-overlay bug), then: `childForm.TopLevel=False`, `FormBorderStyle=None`, `Dock=Fill`, `BackColor=255,253,244`, `Font=Tahoma 10.5`, adds to `pnlFormHostBody` (fallback `pnlFormHost`), `pnlFormHost.Tag=childForm`, `childForm.Show()`.
- `CloseActiveForm()` — removes/disposes `_currentChildForm`; `pnlFormHost.Tag = Nothing`.
- `ShowPlaceholder(...)` — builds a "🚧 under-construction" panel with dev instructions (legacy helper; no current nav target uses it).
- `SetActiveButton(btn)` — gold active style (12pt bold `#451A03`, height 54) vs transparent inactive (11.25pt white, height 50).
- `OverviewCard_Click` — card1 → `btnMember`, card2 → `btnDonation`, card3 → `btnExpense`, card4 → `btnReport`.
- `LoadActualDashboardFromDb()` — month window (`monthStart` = first of month, `monthEnd` = +1 month −1s); SQL: `COUNT(*)` income this month, `SUM(Amount)` income/expense this month, global balance `SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0))`.
- `LoadActualDonationOverviewFromDb()` — count, total, max, avg income this month.
- `LoadActualExpenseOverviewFromDb()` — count, total, avg expense + net balance.
- `LoadActualReportOverviewFromDb()` — total count, **YTD net** (`SUM(...) WHERE TranDate >= yearStart`), month count, prior-year balance (`TranDate < yearStart`).
- `LoadTransactionOverview()` — total count, income sum, expense sum, transfer sum.
- `LoadMonkSample()` — reads `TempleSetting` TOP 1 (TempleName/AbbotName/WaiyawatName/BookkeeperName — **legacy columns still referenced here**; data-safe with `IsDBNull` guards).
- `LoadActivitySample()` / `LoadSettingSample()` — transfer counts + category/fund/bank counts.
- `BtnClose_Click` / `BtnMinimize_Click` / `BtnLogout_Click` — confirm + `Application.Exit()` / minimize / informational logout stub.
- `btnBackup_Click` — `FolderBrowserDialog` → `DatabaseBackupHelper.BackupFullSystem(path, False)`.
- `btnRestore_Click` — warning confirm → `OpenFileDialog` (`*.accdb;*.jpg;...`) → `DatabaseBackupHelper.RestoreFullSystem(backupFolder)` → `Application.Restart()`.
- `FrmMain_FormClosing` — background auto-backup `DatabaseBackupHelper.BackupFullSystem(isAuto:=True)` (silent).
- `StatusCenter_DoubleClick` — opens the DB folder in Explorer; `StatusCenter_MouseClick` (right-click) copies DB path to clipboard.

---

### 2.3 FrmIncome — บันทึกรายรับ (Record Income)

**Files:** `Forms\FrmIncome.vb` + `Forms\FrmIncome.Designer.vb` (+ `.resx`).

- `Partial Public Class FrmIncome : Inherits Form`; `ClientSize 1280×820`, `MinimumSize 1100×720`, `AutoScroll=True`, `BackColor 254,249,235`, Tahoma 10.5, `StartPosition=CenterScreen`, `WindowState=Maximized`, `Text="บันทึกรายรับ"`. AutoScaleMode default (Font).
- **Load flow:** `KeyPreview=True` → `HelpSystem.SetupHelp(Me,"FrmIncome")` → `Db.EnsureSchema()` → `LoadMasters()` → `SetupToolTips()` → `SetupEnterNavigation()` → `UiFitter.AutoFitFormButtons(Me)` → `UiFitter.DisableComboBoxWheel(Me)` → `ResetEntry(True)` → `FocusStartField()`.
- **F1:** `HelpSystem.ShowManual("FrmIncome", Me)` with Handled/SuppressKeyPress.

**Controls (Designer):** `lblHeader` "💰 บันทึกรายรับเงินเข้าวัด" (Dock=Top, 1280×70, `#FDE68A`, 15pt bold); field labels lbl1..lbl8 (40px column, MiddleRight); `dtpDate` (240,110, 520×33, Tahoma 10.5); `cboCategory` (DropDownList, 520×33); `cboFund` (DropDownList); `cboBank` (DropDownList); `txtDescription` (520×33); `txtAmount` (260×35, Tahoma 11.5 bold, green `#166534`, TextAlign Right); `txtRemark` (multiline, 520×100); receipt row: `txtReceipt` (ReadOnly, 300×33), `btnBrowseReceipt` "📂 เลือกรูปภาพ" (`#4F46E5`), `btnPasteReceipt` "📋 วางจาก LINE" (`#059669`), `btnClearReceipt` "🗑️" (`#DC2626`); action row: `btnSave` "💾 บันทึกรายการ" (`#16A34A`, 240×56), `btnCancel` "❌ เคลียร์" (`#B45309`, 180×56), `btnImportExcel` "📥 นำเข้าจาก Excel" (`#2563EB`, 240×56); `ttMain` ToolTip.

**Data & SQL:**
- `LoadMasters()`: categories `"SELECT ID, CategoryName FROM Categories WHERE TranType='Income' ORDER BY CategoryName"`; funds `"SELECT ID, FundName FROM Funds ORDER BY FundName"`; banks `"SELECT ID, BankName & '  ' & IIF(AccountNo IS NULL,'',AccountNo) & '  (' & IIF(AccountName IS NULL,'',AccountName) & ')' AS Disp FROM BankAccounts ORDER BY BankName"` with a blank row (`DBNull`) inserted at index 0 for "no bank".
- `btnSave_Click` validation: category selected, fund selected, `Decimal.TryParse(txtAmount.Text, amt)` with `amt > 0`; Thai error messages.
  - UPDATE (edit mode): `"UPDATE Transactions SET TranDate=" & Db.AccessDateLiteral(dtpDate.Value.Date) & ", CategoryID=@c, FundID=@f, BankID=@b, [Detail]=@de, Amount=@a, [Note]=@n WHERE ID=@id"`.
  - INSERT (new): `"INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(dtpDate.Value.Date) & ",'Income',@c,@f,@b,@de,@a,@n)"` via `Db.InsertAndGetId` (`SELECT @@IDENTITY`).
  - Receipt: `"UPDATE Transactions SET ReceiptPath = @rp WHERE ID = @id"` when a new source path was chosen.
- **Edit mode** via `EditID` property (`_editId > 0`): `LoadTransactionData(id)` fills fields (`Amount` with `ToString("N2")`), header becomes "📝 แก้ไขรายการรายรับ (ID: n)", `btnCancel` becomes "🔙 ย้อนกลับ" (gray), `btnImportExcel` hidden, save button "💾 บันทึกการแก้ไข". After save in edit mode → back via `frmMain.btnMember.PerformClick()`.
- `SaveReceiptFile(transactionID)` — clipboard image → `ReceiptImageHelper.SaveOptimizedReceipt(image, id)`; else file path version.
- `btnImportExcel_Click` — `OpenFileDialog` (xlsx/xlsm/xlsb/xls) → `ExcelTransactionImporter.ImportTransactionsFromExcel(path, "Income", categoryId, fundId, bankId)` → summary MessageBox + reload.
- Receipt browse: `InitialDirectory="C:\LineDownloads"` if exists, filter `*.jpg;*.jpeg;*.png;*.bmp`. Paste-from-LINE: `Clipboard.ContainsImage()`, shows "[รูปภาพจากคลิปบอร์ด/LINE]". Clear: disposes image + `Clipboard.Clear()` with LogCrash guard.
- `txtAmount_KeyPress` — digits + single "."; control chars pass.
- Enter flow: `{dtpDate, cboCategory, cboFund, cboBank, txtDescription, txtAmount, txtRemark, btnBrowseReceipt, btnPasteReceipt, btnSave}`.

---

### 2.4 FrmExpense — บันทึกรายจ่าย (Record Expense)

**Files:** `Forms\FrmExpense.vb` + `.Designer.vb` (+ `.resx`).

- Mirror of FrmIncome with: header `lblHeader.Text="💸 บันทึกรายจ่ายของวัด"` (`#FECACA`, `#7F1D1D`), `lbl2="ประเภทรายจ่าย:"`, `txtAmount.ForeColor=#991B1B` (red), `btnSave.BackColor=#B45309` (orange), `btnCancel` gray in designer (code overrides to orange in new mode). Same ClientSize/min sizes, same control coordinates (minor: `btnBrowseReceipt` 157×40, `btnPasteReceipt` 169×40).
- Identical Load flow with `HelpSystem.SetupHelp(Me,"FrmExpense")`; identical Enter flow; identical F1 pattern.
- SQL: category load uses `TranType='Expense'`; INSERT uses `'Expense'`; validation messages "กรุณาเลือกประเภทรายจ่าย" / "กรุณาเลือกกองทุน" / "กรุณาใส่จำนวนเงินที่ถูกต้อง". Import title "เลือกไฟล์ Excel สำหรับนำเข้ารายจ่าย", summary label "รายจ่าย".

---

### 2.5 FrmTransfer — โอนเงินภายใน (Internal Fund/Bank Transfer)

**Files:** `Forms\FrmTransfer.vb` + `.Designer.vb` (+ `.resx`).

- `Partial Public Class FrmTransfer : Inherits Form`; `Text="โอนเงินภายใน"`, `ClientSize 1280×820`, `MinimumSize 1100×720`, `FormBorderStyle=Sizable`, `AutoScroll=True`, `StartPosition=CenterScreen`, `WindowState=Maximized`, `BackColor 254,249,235`, Tahoma 10.5. AutoScaleMode default.
- **Load flow:** KeyPreview → `SetupHelp(Me,"FrmTransfer")` → `Db.EnsureSchema()` → `SetupToolTips()` → `UiFitter.AutoFitFormButtons(Me)` → `DisableComboBoxWheel(Me)` → load funds/banks inline (`AddBlankOption` uses `EmptySelectionId = 0` blank rows, `.Copy()` for the "to" combo) → `SetupEnterNavigation()` → `ResetEntry(True)` → `FocusStartField()`.

**Controls:** header "🔁 โอนเงินภายในระหว่างกองทุน/บัญชี" (14pt bold `#581C87`, bg `#E9D5FF`, Dock=Top 70px); `dtpDate` "วันที่โอน:"; `cboFromFund` "จากกองทุน:"; `cboFromBank` "จากบัญชีธนาคาร:"; `cboToFund` "ไปยังกองทุน:"; `cboToBank` "ไปยังบัญชี:"; `txtAmount` "จำนวนเงินที่โอน:" (11.5 bold `#581C87`, right-aligned); `txtRemark` "เหตุผลการโอน:" (multiline 520×100); `btnSave` "💾 บันทึกการโอน" (`#7E22CE`, 260×56); `btnCancel` "❌ เคลียร์" (`#4B5563`, 180×56). No DataGridView, no TabIndex assignments.

**Business logic (`btnSave_Click`):**
- `fromFundId/fromBankId/toFundId/toBankId = SelectedIdOrZero(cbo)` (blank = 0).
- Validation: source empty → "กรุณาเลือกต้นทางอย่างน้อย 1 ช่อง เช่น กองทุนหรือบัญชีธนาคาร"; dest empty → "กรุณาเลือกปลายทางอย่างน้อย 1 ช่อง เช่น กองทุนหรือบัญชีธนาคาร"; `fromFundId = toFundId AndAlso fromBankId = toBankId` → "ต้นทางและปลายทางต้องแตกต่างกัน"; bad amount → "กรุณาใส่จำนวนเงิน".
- INSERT (single-row, nullable To-columns): `"INSERT INTO Transactions (TranDate, TranType, FundID, BankID, ToFundID, ToBankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(...) & ",'Transfer',@ff,@fb,@tf,@tb,@de,@a,@n)"` with `@ff/@fb/@tf/@tb = ToDbNullableId(...)` (0 → DBNull).
- `BuildTransferDetail(...)`: `"โอนเงินภายใน: " & Join(sourceParts, " / ") & " -> " & Join(destParts, " / ")` where parts are "กองทุน <name>" and/or "บัญชี <bank display>".
- Success: "✅ โอนเงินภายในสำเร็จ!"; keeps the chosen date after reset.
- **No edit mode, no Excel import, no receipt attachment.**

---

### 2.6 FrmTransactions — รายการรับ-จ่ายทั้งหมด (Transaction Ledger)

**Files:** `Forms\FrmTransactions.vb` + `.Designer.vb` (+ `.resx`).

- `Partial Public Class FrmTransactions : Inherits Form`; `ClientSize 1250×800`, `MinimumSize 1180×760`, `AutoScaleMode=Font`, `AutoScaleDimensions=12×25`, `BackColor 254,249,235`, Tahoma 10.5, `StartPosition=CenterScreen`, `WindowState=Maximized`, `Text="รายการรับ-จ่ายทั้งหมด"`. Fields: `_searchFlow`, `_isEditing`, `_editingTransactionId`.
- **Load flow:** KeyPreview → `SetupToolTips()` → `SetupHelp(Me,"FrmTransactions")` → `UiFitter.AutoFitFormButtons(Me)` → `DisableComboBoxWheel(Me)` → **debug instrumentation** (`DebugReport` points A–D; `DebugSessionId="transactions-grid-empty"`, `DebugRunId="post-fix"`, POST JSON to `http://127.0.0.1:7777/event` with 500 ms timeout; env file `.dbg\transactions-grid-empty.env` with `DEBUG_SERVER_URL=`; walks up 12 dir levels) → `Db.EnsureSchema()` → `LoadFilters()` → `SetupSearchEnterNavigation()` → `LoadData()`.

**Controls:**
- `lblSummary` (Dock=Top, 1250×50, `#FEF08A`, 11pt bold) — default text `"รายรับ: 0.00 บาท | รายจ่าย: 0.00 บาท | คงเหลือ: 0.00 บาท | โอน: 0.00 บาท"`; runtime totals.
- `pFilter` (Dock=Top, 1250×110, white): `cboCategory` "ประเภท:" (200×33), `cboType` "ชนิด:" (180×33, items `{"ทั้งหมด","รายรับ","รายจ่าย","โอนภายใน"}`), `dtpFrom` "ตั้งแต่:" (Short format), `dtpTo` "ถึง:", `txtSearch` "ค้นหา:" (210×33), `btnSearch` "🔍 ค้นหา" (`#2563EB`), `btnRefresh` "🔄 รีเฟรช" (`#059669`).
- `dgvTransactions` (Dock=Fill, ReadOnly, FullRowSelect, no designer columns — runtime-bound; ColumnHeadersHeight 40, RowHeadersWidth 50, RowTemplate.Height 34, alternating `#FFFBE`).
- `pActions` (Dock=Bottom, 1250×140, `#F5F0DC`): `btnAddInc` "💰 บันทึกรายรับ" (`#16A34A`), `btnAddExp` "💸 บันทึกรายจ่าย" (`#B45309`), `btnAddTrans` "🔁 โอนเงิน" (`#7E22CE`), `btnEdit` "✏️ แก้ไข" (`#2563EB`), `btnDelete` "🗑️ ลบ" (`#DC2626`), `btnClose` "ปิด" (`#4B5563`) — row 1; `btnViewReceipt` "🔍 ดูใบเสร็จ" (`#F59E0B`), `btnPasteReceipt` "� วางรูปย้อนหลัง" (`#059669`) **[contains U+FFFD corrupted glyph — known issue]**, `btnBrowseReceipt` "� เลือกรูปย้อนหลัง" (`#4F46E5`) **[corrupted glyph]**, `btnDeleteReceipt` "❌ ลบรูปใบเสร็จ" (`#991B1B`) — row 2.

**Filter SQL (`LoadFilters`):** category list `"SELECT ID, CategoryName FROM Categories ORDER BY CategoryName"` + synthetic row ID=0 "ทุกประเภท"; date bounds via MIN/MAX with `IIF(Year(TranDate)>2400, DateAdd('yyyy',-543,TranDate), TranDate)`; fallback `dtpFrom = Jan 1`, `dtpTo = today`.

**Main grid SQL (`LoadData`):** `tranDateExpr = "IIF(Year(t.TranDate)>2400, DateAdd('yyyy',-543,t.TranDate), t.TranDate)"`; d1 = `dtpFrom`, d2 = `dtpTo.AddDays(1).AddSeconds(-1)`:
```sql
SELECT t.ID, t.TranDate, t.TranType,
  IIF(t.TranType='Income','รายรับ',IIF(t.TranType='Expense','รายจ่าย','โอนภายใน')) AS TranTypeDisplay,
  t.CategoryID, IIF(c.CategoryName IS NULL,'',c.CategoryName) AS CategoryName,
  t.FundID, IIF(f.FundName IS NULL,'',f.FundName) AS FundName,
  t.BankID, IIF(b.BankName IS NULL,'',b.BankName & IIF(b.AccountNo IS NULL,'',' ' & b.AccountNo)) AS BankName,
  t.Detail, t.Amount, t.Note, t.CreateDate,
  t.ToFundID, IIF(f2.FundName IS NULL,'',f2.FundName) AS ToFundName,
  t.ToBankID, IIF(b2.BankName IS NULL,'',b2.BankName & IIF(b2.AccountNo IS NULL,'',' ' & b2.AccountNo)) AS ToBankName,
  t.ReceiptPath, IIf(t.ReceiptPath IS NOT NULL AND t.ReceiptPath <> '', '📷 มีรูป', '-') AS HasReceiptDisplay
FROM ((((Transactions t
  LEFT JOIN Categories c  ON t.CategoryID=c.ID)
  LEFT JOIN Funds f       ON t.FundID=f.ID)
  LEFT JOIN BankAccounts b ON t.BankID=b.ID)
  LEFT JOIN Funds f2       ON t.ToFundID=f2.ID)
  LEFT JOIN BankAccounts b2 ON t.ToBankID=b2.ID
WHERE <tranDateExpr> BETWEEN <d1Literal> AND <d2Literal>
  [AND t.CategoryID=@cat]           -- when SelectedValue <> 0
  [AND t.TranType='Income'|'Expense'|'Transfer']  -- per cboType index 1..3
  [AND (t.Detail LIKE @s1 OR t.Note LIKE @s2 OR c.CategoryName LIKE @s3 OR
       f.FundName LIKE @s4 OR b.BankName LIKE @s5 OR f2.FundName LIKE @s6 OR
       b2.BankName LIKE @s7 OR IIF(f.FundName IS NULL,'',f.FundName & ' ' &
       IIF(b.BankName IS NULL,'',b.BankName)) LIKE @s8 OR IIF(f2.FundName IS NULL,'',
       f2.FundName & ' ' & IIF(b2.BankName IS NULL,'',b2.BankName)) LIKE @s9)]
ORDER BY <tranDateExpr> DESC, t.ID DESC
```
- LIKE escaping: `searchText.Replace("[","[[]").Replace("?","[?]").Replace("#","[#]")`; pattern `"%" & safe & "%"` (Access uses `%`, not `*`). Search wrapped in Try/Catch + `LogCrash("FrmTransactions.Search")`.
- Summary: `$"รายรับ: {sumInc:n2} บาท  |  รายจ่าย: {sumExp:n2} บาท  |  คงเหลือ: {(sumInc-sumExp):n2} บาท  |  โอนภายใน: {sumTrf:n2} บาท"` (totals computed in VB from the DataTable).
- `ConfigureGridColumns()` — hides `{TranType, CategoryID, FundID, BankID, ToFundID, ToBankID, ReceiptPath}`; Thai headers (HasReceiptDisplay→"ใบเสร็จ", ID→"ID", TranDate→"วันที่", TranTypeDisplay→"ชนิด", CategoryName→"ประเภท", FundName→"กองทุน", BankName→"ธนาคาร", Detail→"รายละเอียด", Amount→"จำนวนเงิน", Note→"หมายเหตุ", CreateDate→"วันที่บันทึก", ToFundName→"ไปยังกองทุน", ToBankName→"ไปยังธนาคาร"); formats `TranDate/CreateDate` `dd/MM/yyyy[ HH:mm:ss]`, `Amount "N2"` MiddleRight; display-only columns grey `#F5F5F5`; explicit widths (90..240px), `AutoSizeMode=None`.

**Events:** `txtSearch_KeyDown` (Enter → search), `txtSearch_TextChanged` (auto-reload when empty), `btnRefresh_Click` (also search; exits edit mode if refreshing), `btnDelete_Click` (multi-delete within one transaction; deletes receipt files under `AppPaths.ReceiptsDir`; `LogCrash("MultiDeleteTransactions")`), `btnAddInc/AddExp/AddTrans` (navigate to entry forms via `frmMain.ShowFormInPanel`), `btnEdit_Click` (routes by TranType to FrmIncome/FrmExpense EditID, or grid edit mode for Transfer), `btnClose_Click` (`CloseActiveForm(); ShowDashboard()`), `btnViewReceipt_Click` (shell-open `ReceiptPath` under ReceiptsDir), `btnPasteReceipt_Click` / `btnBrowseReceipt_Click` (`SaveOptimizedReceipt` + `UPDATE Transactions SET ReceiptPath=@p WHERE ID=@id` + `Clipboard.Clear()`), `btnDeleteReceipt_Click` (delete file + `UPDATE ... SET ReceiptPath=NULL`), `dgvTransactions_CellDoubleClick` (view receipt).

**Known dead/inert code:** `SaveSelectedRowEdits()` (line ~737) is never called — the in-grid "💾 บันทึกแก้ไข" edit affordance cannot persist edits; `btnEdit_Click` never checks `_isEditing`. Designer `lblHeader` is declared but never instantiated; `dgvTransactions_CellContentClick` and `lblSearch_Click` are empty handlers.

---

### 2.7 FrmReports — ศูนย์รายงาน (Report Center)

**Files:** `Forms\FrmReports.vb` + `.Designer.vb` (+ `.resx`). See Section 5 for the full reporting subsystem.

**Controls (Designer):** filter row — `dtpFrom` "ตั้งแต่:", `dtpTo` "ถึง:", `cboType` ("ทั้งหมด/รายรับ/รายจ่าย/โอนภายใน"), `cboFund` (all funds + blank), `cboBank` (all banks + blank), `txtBalance` (read-only computed), `btnCalcBalance` "คำนวณยอดคงเหลือ", `btnRefresh` "รีเฟรช"; 8-button uniform report toolbar: `btnPrintDetail` "📜 รายงานละเอียด", `btnPrintSummary` "📚 รายงานย่อ", `btnSummaryIncome` "💵 สรุปรายรับ", `btnSummaryExpense` "💸 สรุปรายจ่าย", `btnShowChart` "📊 กราฟสรุปรายเดือน", `btnMonthly` "📈 รายเดือน", `btnLedger` "📒 สมุดรายวัน", `btnPrint` "📊 Excel"; `lblSummary` (yellow strip); `dgvReport` (read-only grid); `chartMonthly` (WinForms chart, hidden by default).

**Load flow:** KeyPreview → `SetupHelp(Me,"FrmReports")` → `Db.EnsureSchema()` → `LoadFilters()` → `LoadData()` → `UiFitter.AutoFitFormButtons(Me)` + `UiFitter.UniformButtonGroup(<8 report buttons>)` + `DisableComboBoxWheel(Me)`.

---

### 2.8 FrmTempleSetting — ตั้งค่าข้อมูลวัด (Temple Settings)

**Files:** `Forms\FrmTempleSetting.vb` + `.Designer.vb` (+ `.resx`).

- `<DesignerCategory("Form")>`, `Partial Public Class FrmTempleSetting : Inherits Form`; `ClientSize 1202×817`, `MinimumSize 800×600`, **`AutoScaleMode=Dpi`** (the only Dpi-scaled form), `AutoScaleDimensions 144×144`, Tahoma 10, BackColor 254,249,235, `Text="ตั้งค่าข้อมูลวัด"`.
- **Load flow** (whole body in Try/Catch with Thai error MessageBox + `Debug.WriteLine("[FrmTempleSetting] Load Error: ...")`): KeyPreview → `SetupHelp(Me,"FrmTempleSetting")` → `Db.EnsureSchema()` → `LoadLocations()` → `LoadPersonnel()` → `LoadTempleData()` → `SetupToolTips()` → `SetupEnterNavigation()` → `pBottom.BringToFront()` → `AutoFitFormButtons` → `DisableComboBoxWheel` → focus `txtTempleCode`.

**Control tree:** `pMainContainer` (Dock=Fill, Padding 10) → `pContent` (Dock=Fill, AutoScroll, white) + `pBottom` (Dock=Bottom, 1182×100, `#F1F5F9`). In `pContent` (top-docked): 
- `gbTempleInfo` "🏛️ ข้อมูลพื้นฐานของวัด" (`#1E40AF`) → `tlpTempleInfo` (2-col TableLayout): `txtTempleCode` "รหัสวัด:", `txtTempleName` "ชื่อวัด:", `txtTempleAddress` "ที่อยู่วัด:" (multiline).
- `gbContactInfo` "📞 ข้อมูลติดต่อ" → `tlpContactInfo` (2 cols × 5 rows): `cboProvince` "จังหวัด:", `cboAmphoe` "อำเภอ:", `cboTambon` "ตำบล:", `txtPostCode` "รหัสไปรษณีย์:", `txtTemplePhone` "เบอร์ติดต่อวัด:" (all DropDownList combos except text boxes).
- `gbPromptPay` "💳 พร้อมเพย์" → `tlpPromptPay` (3 rows): `chkUsePromptPay` "เปิดใช้งานพร้อมเพย์" (bold `#0F766E`), `txtPromptPayName` "ชื่อบัญชีพร้อมเพย์:", `txtPromptPayID` "เลขพร้อมเพย์/เลขบัญชี:".
- `gbPersonnel` "👤 ผู้ดำรงตำแหน่งในวัด" → `tlpPersonnel` (3 rows): `cboAbbotName` "ชื่อเจ้าอาวาส:", `cboWaiyawatName` "ชื่อไวยาวัจกร:", `cboBookkeeperName` "ชื่อผู้ทำบัญชี:".
- `gbPersonnelList` "📋 รายชื่อและบทบาทบุคลากร" (height 100) → `dgvPersonnel` (Dock=Fill, ReadOnly, FullRowSelect, `RowHeadersVisible=False`, AutoSizeColumnsMode=Fill, no designer columns).
- `pBottom` → `flpButtons` (RightToLeft): `btnClose` "❌ ปิด" (`#4B5563`), `btnManagePersonnel` "👤 จัดการรายชื่อ..." (`#9333EA`), `btnLocationImport` "📍 นำเข้าที่อยู่" (`#2563EB`), `btnCancel` "🔄 โหลดใหม่" (`#D97706`), `btnSave` "💾 บันทึก" (`#16A34A`).

**Dead designer members:** `pHeader` and `lblHeader` are configured but never added to any parent — never displayed.

**Personnel SQL:**
- Abbot list: `"SELECT p.PersonnelID, p.FullName FROM Personnel p INNER JOIN Positions pos ON p.PositionID = pos.PositionID WHERE pos.PositionGroup = 'พระ' ORDER BY p.FullName"`.
- Lay list (waiyawat + bookkeeper): same with `'ฆราวาส'`, both bind `layTable.Copy()`.
- Roster grid (`LoadPersonnelGrid`, UNION ALL to avoid Access JOIN-OR limitation):
```sql
SELECT 'เจ้าอาวาส' AS [บทบาทในวัด], p.Title & ' ' & p.FirstName & ' ' & p.LastName AS [ชื่อ-นามสกุล],
       pos.PositionName AS [ตำแหน่ง], p.PersonType AS [ประเภท], p.Phone AS [เบอร์โทร]
FROM ((TempleSetting t INNER JOIN Personnel p ON t.AbbotPersonnelID = p.PersonnelID)
      LEFT JOIN Positions pos ON p.PositionID = pos.PositionID)
UNION ALL SELECT 'ไวยาวัจกร', ..., t.WaiyawatPersonnelID ...
UNION ALL SELECT 'ผู้ทำบัญชี', ..., t.BookkeeperPersonnelID ...
```

**Location cascade:** `LoadLocations()` → `"SELECT ProvinceID, ProvinceName FROM Province ORDER BY ProvinceName"`; `cboProvince_SelectedIndexChanged` → `"SELECT DistrictID, DistrictName FROM District WHERE ProvinceID=@p ORDER BY DistrictName"`; `cboAmphoe_SelectedIndexChanged` → `"SELECT SubDistrictID, SubDistrictName, ZipCode FROM SubDistrict WHERE DistrictID=@d ORDER BY DistrictName"`; `cboTambon_SelectedIndexChanged` → `"SELECT ZipCode FROM SubDistrict WHERE SubDistrictID=@s"` → autofill `txtPostCode`.

**Temple data load/save:**
- Load: `"SELECT TOP 1 * FROM TempleSetting ORDER BY ID DESC"`; `TrySetComboText` matches stored name strings against combo DataRowView columns; personnel combos set by `SelectedValue = r("AbbotPersonnelID")` etc. (guarded by `Columns.Contains` + `IsDBNull`); `chkUsePromptPay.Checked = (Not IsNullOrWhiteSpace(PromptPayName) OrElse Not IsNullOrWhiteSpace(PromptPayID))`.
- Save (`btnSave_Click`, requires TempleName): extracts combo text via DataRowView (`"ProvinceName"`/`"DistrictName"`/`"SubDistrictName"`) else `.Text`; blanks PromptPay unless checked; then:
```sql
DELETE FROM TempleSetting
```
```sql
INSERT INTO TempleSetting (TempleCode,TempleName,TempleAddress,Tambon,Amphoe,Province,PostCode,TemplePhone,
  PromptPayName,PromptPayID,AbbotPersonnelID,WaiyawatPersonnelID,BookkeeperPersonnelID)
VALUES (@a1,@a2,@a3,@a4,@a5,@a6,@a7,@a8,@a9,@a10,@a11,@a12,@a13)
```
Personnel IDs `DBNull.Value` when not selected. After save: `ReportEngine.ClearTemplateInfoCache()`, reload roster grid, focus `txtTempleCode`.

**Other events:** `btnManagePersonnel_Click` → `New FrmPersonnelManagement().ShowDialog(Me)` then reload personnel combos; `btnCancel_Click` → `LoadTempleData()`; `btnLocationImport_Click` → host `FrmLocationImport` in panel (or `ShowDialog` when standalone); `btnClose_Click` → `CloseActiveForm(); ShowDashboard()`.

**Enter flow:** `{txtTempleCode, txtTempleName, txtTempleAddress, cboProvince, cboAmphoe, cboTambon, txtPostCode, txtTemplePhone, cboAbbotName, cboWaiyawatName, cboBookkeeperName, chkUsePromptPay, txtPromptPayName, txtPromptPayID, btnSave}`.

---

### 2.9 FrmPersonnelManagement — จัดการรายชื่อบุคลากร (Personnel & Positions, modal dialog)

**Files:** `Forms\FrmPersonnelManagement.vb` + `.Designer.vb` (+ `.resx`).

- `Public Class FrmPersonnelManagement : Inherits Form`; `ClientSize 900×620`, `AutoScaleMode=Font` (8×16), `FormBorderStyle=FixedDialog`, `MaximizeBox=False`, `MinimizeBox=False`, `StartPosition=CenterParent`, `Text="จัดการรายชื่อบุคลากรและตำแหน่ง"`. Fields `_selectedPersonnelID`, `_selectedPositionID`.
- **Load flow:** KeyPreview → `SetupHelp(Me,"FrmPersonnelManagement")` → `LoadPositionCombo()` → `LoadPersonnelData()` → `LoadPositionData()` → `ClearPersonnelEditor()` → `ClearPositionEditor()` → `UiFitter.AutoFitFormButtons(Me)` (no DisableComboBoxWheel). F1 → `ShowManual("FrmPersonnelManagement", Me)`.

**Controls:** `pnlHeader` (Dock=Top, 900×42, `#FDE68A`) → `lblHeader` "👤 จัดการรายชื่อบุคลากรและตำแหน่ง" (12pt bold `#451A03`); `pnlMain` (Dock=Fill) → `tcMain` (TabControl, Tahoma 10, SelectedIndex 0):
- `tpPersonnel` "บุคลากร": `pnlEditor` (Dock=Top 220px, white, Padding 20) → `tlpEditor` (4 cols: 120/50%/120/50%, 3 rows): `txtTitle` "คำนำหน้า:", `cboPositionID` "ตำแหน่ง:", `txtFirstName` "ชื่อ:", `txtLastName` "นามสกุล:", `cboPersonType` "ประเภท:" (DropDownList, items `{"Monk","Layperson"}`), `txtPhone` "เบอร์โทร:"; `dgvPersonnel` (Dock=Fill, ReadOnly, FullRowSelect, Fill columns, no designer columns).
- `tpPositions` "ตำแหน่งหน้าที่": `pnlPosEditor` (Dock=Top 100px) → `txtPositionName` "ชื่อตำแหน่ง:"; `dgvPositions` (Dock=Fill, same style).
- `pnlButtons` (Dock=Bottom, 900×70, `#F5F5F0`) → `flpButtons` (RightToLeft): `btnClose` "ปิด", `btnDelete` "🗑️ ลบ" (`#991B1B`), `btnSave` "💾 บันทึก" (`#16A34A`), `btnNew` "➕ เพิ่มใหม่" (`#2563EB`).

**CRUD SQL:**
- `LoadPersonnelData()`: `"SELECT p.PersonnelID, p.Title, p.FirstName, p.LastName, p.FullName, p.PersonType, p.Phone, p.PositionID, pos.PositionName FROM Personnel p LEFT JOIN Positions pos ON p.PositionID = pos.PositionID ORDER BY p.FullName"` (hide PersonnelID/PositionID; Thai headers).
- `LoadPositionData()`: `"SELECT PositionID, PositionName FROM Positions ORDER BY PositionName"`.
- `SavePersonnel()` (requires FirstName): `fullName = $"{Title}{FirstName} {LastName}"` (title glued with **no space**); Insert `"INSERT INTO Personnel (Title, FirstName, LastName, FullName, PersonType, PositionID, Phone) VALUES (@t, @f, @l, @fn, @pt, @pos, @ph)"`; Update `"UPDATE Personnel SET Title=@t, FirstName=@f, LastName=@l, FullName=@fn, PersonType=@pt, PositionID=@pos, Phone=@ph WHERE PersonnelID=@id"`.
- `SavePosition()`: Insert `"INSERT INTO Positions (PositionName) VALUES (@n)"`; Update `"UPDATE Positions SET PositionName=@n WHERE PositionID=@id"`.
- `DeletePersonnel()` / `DeletePosition()` — YesNo confirm + `DELETE ... WHERE ...=@id`; no FK-cascade guard (deleting a referenced position raises a DB error caught by the catch MessageBox).

---

### 2.10 FrmLocationImport — นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล (Location CSV Import)

**Files:** `Forms\FrmLocationImport.vb` + `.Designer.vb` (+ `.resx`).

- `<DesignerCategory("Form")>`, `Partial Public Class FrmLocationImport : Inherits Form`; `ClientSize 1591×781`, `MinimumSize 1180×760`, **AutoScaleMode None (default)**, Tahoma 10.5, BackColor 254,249,235, `AutoScroll=True`, `StartPosition=CenterScreen`, `Text="นำเข้าข้อมูลจังหวัด"`. Field `isImporting`.
- **Load flow:** KeyPreview → `SetupHelp(Me,"FrmLocationImport")` → `Db.EnsureSchema()` → `CheckFiles()` → `LoadLastImport()` → `SetupToolTips()` → `AutoFitFormButtons` → `btnCheck_Click(Nothing, EventArgs.Empty)` (programmatic). No ComboBoxes → no wheel handler.

**Controls:** `lblHeader` "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล" (Dock=Top 42px, `#BAE6FD`, `#0C4A6E`, 14pt bold); `grp1` "ไฟล์ CSV ต้นฉบับ" (965×130 @20,60): `lblProvinceFile`/`lblDistrictFile`/`Label2` ("province.csv"/"amphoe.csv"/"tambon.csv") + `lblProvinceCount`/`lblDistrictCount`/`lblSubDistrictCount` (file size or "❌ ไม่พบไฟล์", `#166534`) + `lnkOpenImportFolder` "📂 เปิดโฟลเดอร์ Import"; `grp2` "ขั้นตอนการนำเข้า" (965×90 @20,200): `chkClearBeforeImport` "ล้างข้อมูลเก่าก่อนนำเข้า (แนะนำครั้งแรก)", `btnImport` "1⃣ นำเข้าใหม่" (`#2563EB`), `btnUpdate` "2⃣ อัปเดตเพิ่ม" (`#059669`), `btnRebuild` "3⃣ สร้างใหม่ทั้งหมด" (`#B45309`); `grp3` "ตรวจสอบ" (588×230 @991,60): `btnCheck` "🔍 นับจำนวนในฐานข้อมูล" (`#4F46E5`), `Label3`/`Label4` ("รายการในตาราง:" / "Province / District / SubDistrict"), `lblStatus`; `grp4` "Progress" (1559×90 @20,300): `prgImport` (ProgressBar), `lblProgress`; `grp5` "ประวัติการทำงานล่าสุด" (1559×280 @20,400): `rtbLog` (RichTextBox, `Consolas 9`, dark `#0F172A` bg, `#E2E8F0` text, ReadOnly), `lblLastImportTitle`/`lblLastImport`; `btnClose` "ปิดหน้านี้" (150×50 @1429,686, `#4B5563`).

**Import logic (`RunImport(clearBefore, rebuild)`):**
- `Imports System.Data.OleDb` exists but **no Excel/OleDb used** — plain UTF-8 CSV parsing: `File.ReadAllLines(path, Encoding.UTF8)` + naive `ln.Split(","c)` (no header skipping, no quoted-field handling).
- Files: `AppPaths.ProvinceCsv()` / `DistrictCsv()` / `SubDistrictCsv()` in `AppPaths.ImportFolder`. Missing file → warning + abort.
- Optional clear: `DELETE FROM SubDistrict` → `DELETE FROM District` → `DELETE FROM Province` (child-first).
- Province (cols 0=ID,1=Name): rebuild → unconditional INSERT; else `"SELECT 1 FROM Province WHERE ProvinceID=@i"` → INSERT or `"UPDATE Province SET ProvinceName=@n WHERE ProvinceID=@i"`.
- Amphoe (cols 0=DistrictID,1=ProvinceID,2=DistrictName): INSERT `"INSERT INTO District (DistrictID, ProvinceID, DistrictName) VALUES (@d,@p,@n)"` or UPDATE `"UPDATE District SET ProvinceID=@p, DistrictName=@n WHERE DistrictID=@d"`.
- Tambon (cols 0=SubDistrictID,1=DistrictID,2=SubDistrictName,3=ZipCode): INSERT `"INSERT INTO SubDistrict (SubDistrictID, DistrictID, SubDistrictName, ZipCode) VALUES (@s,@d,@n,@z)"` or UPDATE.
- Progress via `Log(msg)`/`SetProgress(cur,tot,msg)` (thread-safe BeginInvoke + `Application.DoEvents()`); last import saved to `Logs\LastLocationImport.txt` via `SaveLastImport`.
- Re-entrancy guarded by `isImporting` flag; success MessageBox "นำเข้าข้อมูลจังหวัดสำเร็จ!".
- `btnClose_Click` → `ShowFormInPanel(New FrmTempleSetting(), "🏛️ ตั้งค่าข้อมูลวัด")`.

---

### 2.11 FrmMasterData — จัดการข้อมูลหลัก (Categories / Funds / Bank Accounts)

**Files:** `Forms\FrmMasterData.vb` + `.Designer.vb` (+ `.resx`).

- `Partial Public Class FrmMasterData : Inherits Form`; `ClientSize 1178×672`, `MinimumSize 1000×700`, `FormBorderStyle=Sizable`, `StartPosition=CenterScreen`, `WindowState=Maximized`, `BackColor 255,253,244`, Tahoma 10.5, `Text="จัดการข้อมูลหลัก"`. Fields: `_categoryFlow`, `_fundFlow`, `_bankFlow`, `selCatId/selFundId/selBankId`.
- **Load flow:** KeyPreview → `SetupHelp(Me,"FrmMasterData")` → `Db.EnsureSchema()` → `SetupToolTips()` → `LoadAll()` → `SetupEnterNavigation()` → `AutoFitFormButtons` → `DisableComboBoxWheel`.

**Controls:** `lblHeader` "🗂️ จัดการข้อมูลหลัก ประเภทรายการ / กองทุน / บัญชีธนาคาร" (Dock=Top 42px, `#FDE68A`, `#451A03`, 12.5pt bold); `TabControl1` (Dock=Fill, Tahoma 10 bold) with 3 tabs:
- `tpCategory` "ประเภทรายการ (Category)": `pCatTop` (Dock=Top 90px) with `txtCatName` "ชื่อประเภท:" (385×33), `cboCatType` "ชนิด:" (DropDownList, items `{"Income (รายรับ)","Expense (รายจ่าย)"}`), `btnCatAdd` "➕ เพิ่ม" (`#16A34A`), `btnCatEdit` "📝 แก้ไข" (`#D97706`), `btnCatDel` "🗑️ ลบที่เลือก" (`#991B1B`); `dgvCategory` (Dock=Fill, ReadOnly, FullRowSelect, `AutoSizeColumnsMode=Fill`, RowTemplate 32, ColumnHeaders 34, alternating `#FFFBE`).
- `tpFund` "กองทุน (Funds)": `txtFundName` "ชื่อกองทุน:", `btnFundAdd` "➕ เพิ่ม", `btnFundDel` "🗑️ ลบที่เลือก"; `dgvFund`.
- `tpBank` "บัญชีธนาคาร (Bank Accounts)": `txtBankName` "ชื่อธนาคาร:", `txtBankAccountNo` "เลขบัญชี:", `txtBankAccountName` "ชื่อบัญชี:", `btnBankAdd`, `btnBankDel`; `dgvBank`.

**Grid SQL (runtime columns, no designer columns):**
- `dgvCategory`: `"SELECT ID, CategoryName, TranType, IIF(TranType='Income', 'รายรับ', 'รายจ่าย') as TranTypeDisplay FROM Categories ORDER BY TranType, CategoryName"` (hide ID/TranType; headers "ชื่อประเภท"/"ชนิด").
- `dgvFund`: `"SELECT ID, FundName FROM Funds ORDER BY FundName"` (header "ชื่อกองทุน").
- `dgvBank`: `"SELECT ID, BankName, AccountNo, AccountName FROM BankAccounts ORDER BY BankName"` (headers "ชื่อธนาคาร"/"เลขบัญชี"/"ชื่อบัญชี").

**CRUD + duplicate guards:**
- Category: `CategoryExists(conn, name, type, excludeId)` → `"SELECT COUNT(*) FROM Categories WHERE CategoryName = @name AND TranType = @type"` (+ `AND ID <> @id`); INSERT/UPDATE/DELETE with duplicate MessageBox warnings; DELETE catch → "ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่".
- Fund: INSERT `"INSERT INTO Funds (FundName) VALUES (@n)"`; DELETE with confirm.
- Bank: INSERT `"INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n, @no, @a)"`; DELETE with confirm.
- Cell click → loads selected row into the editor fields.
- **Enter flows:** `_categoryFlow={txtCatName,cboCatType,btnCatAdd}`, `_fundFlow={txtFundName,btnFundAdd}`, `_bankFlow={txtBankName,txtBankAccountNo,txtBankAccountName,btnBankAdd}`; last control triggers its Add button.

---

### 2.12 FrmHelpDialog — ช่วยเหลือ (Contextual Help Dialog, modal)

**Files:** `Forms\FrmHelpDialog.vb` + `.Designer.vb` (+ `.resx`). See Section 6.2 for details.

- Constructor `New(formName As String)`; form 684×418, header `#2D3E50`, close button `#E74C3C`; `rtbHelp` RichTextBox renders the extracted markdown section; `btnClose` is the only handler.

---

## 3. Database Schema & Data Dictionary (`Database\TempleAccounting.accdb`)

### 3.1 Connection & Bootstrap

- **Connection string:** `Provider=Microsoft.ACE.OLEDB.12.0;Data Source=<AppPaths.DatabaseFile>;Persist Security Info=False;` (`Db.ConnectionString`).
- **DB discovery:** `AppPaths.FindPreferredDatabaseFile()` walks up 12 directory levels, preferring `<project root>\Database\TempleAccounting.accdb`; production fallback `<AppRoot>\Database\TempleAccounting.accdb`.
- **Schema self-healing:** `Db.EnsureSchema()` (cached by `_schemaChecked`) creates missing tables via `TryCreateTable` and runs idempotent column migrations, then seeds defaults. Throws `FileNotFoundException("ไม่พบไฟล์ฐานข้อมูล TempleAccounting.accdb")` if the file is missing.
- **Naming:** tables/columns are PascalCase; reserved words escaped with `[]` in SQL (`[Detail]`, `[Note]`).

### 3.2 Tables, Types, Keys & Relationships

| Table | Created By (DDL) | Primary Key | Columns (type / nullable) | Relationships / Indexes |
|---|---|---|---|---|
| **Province** | `CREATE TABLE Province (ProvinceID INTEGER PRIMARY KEY, ProvinceName TEXT(100) NOT NULL)` | `ProvinceID` INTEGER | ProvinceName TEXT(100) NOT NULL | PK on ProvinceID; referenced by District.ProvinceID |
| **District** | `CREATE TABLE District (DistrictID INTEGER PRIMARY KEY, ProvinceID INTEGER NOT NULL, DistrictName TEXT(100) NOT NULL)` | `DistrictID` INTEGER | ProvinceID INTEGER NOT NULL (FK→Province), DistrictName TEXT(100) NOT NULL | FK-ish: District.ProvinceID → Province.ProvinceID (logical, no enforced FK object); referenced by SubDistrict.DistrictID |
| **SubDistrict** | `CREATE TABLE SubDistrict (SubDistrictID INTEGER PRIMARY KEY, DistrictID INTEGER NOT NULL, SubDistrictName TEXT(100) NOT NULL, ZipCode TEXT(10))` | `SubDistrictID` INTEGER | DistrictID INTEGER NOT NULL (FK→District), SubDistrictName TEXT(100) NOT NULL, ZipCode TEXT(10) | FK-ish: SubDistrict.DistrictID → District.DistrictID; used for address cascade + PostCode autofill |
| **TempleSetting** | `CREATE TABLE TempleSetting (ID COUNTER PRIMARY KEY, TempleCode TEXT(20), TempleName TEXT(200), TempleAddress MEMO, Tambon TEXT(100), Amphoe TEXT(100), Province TEXT(100), PostCode TEXT(10), TemplePhone TEXT(30), AbbotPersonnelID INTEGER, WaiyawatPersonnelID INTEGER, BookkeeperPersonnelID INTEGER, PromptPayName TEXT(100), PromptPayID TEXT(50))` | `ID` COUNTER (AutoNumber) | see columns; address MEMO; personnel IDs nullable | Personnel FK-ish via AbbotPersonnelID/WaiyawatPersonnelID/BookkeeperPersonnelID → Personnel.PersonnelID; **single-row pattern** (code deletes all + inserts one); `ORDER BY ID DESC TOP 1` reads latest |
| **Categories** | `CREATE TABLE Categories (ID COUNTER PRIMARY KEY, CategoryName TEXT(200) NOT NULL, TranType TEXT(10) NOT NULL)` | `ID` COUNTER | CategoryName TEXT(200) NOT NULL, TranType TEXT(10) NOT NULL | TranType in ('Income','Expense') — used heavily for filtering; uniqueness enforced at app level (duplicate-name check) |
| **Funds** | `CREATE TABLE Funds (ID COUNTER PRIMARY KEY, FundName TEXT(200) NOT NULL)` | `ID` COUNTER | FundName TEXT(200) NOT NULL | Referenced by Transactions.FundID / ToFundID |
| **BankAccounts** | `CREATE TABLE BankAccounts (ID COUNTER PRIMARY KEY, BankName TEXT(100) NOT NULL, AccountNo TEXT(50), AccountName TEXT(200))` | `ID` COUNTER | BankName TEXT(100) NOT NULL, AccountNo TEXT(50), AccountName TEXT(200) | Referenced by Transactions.BankID / ToBankID; display string `BankName & '  ' & AccountNo & '  (' & AccountName & ')'` used in combos |
| **Transactions** | `CREATE TABLE Transactions (ID COUNTER PRIMARY KEY, TranDate DATETIME NOT NULL, TranType TEXT(10) NOT NULL, CategoryID INTEGER, FundID INTEGER, BankID INTEGER, Detail TEXT(255), Amount CURRENCY NOT NULL, Note MEMO, CreateDate DATETIME DEFAULT Now(), ToFundID INTEGER, ToBankID INTEGER, ReceiptPath TEXT(255))` | `ID` COUNTER | TranDate DATETIME NOT NULL; TranType TEXT(10) NOT NULL ('Income'/'Expense'/'Transfer'); CategoryID INTEGER (nullable — **must be NULL for Transfer**); FundID INTEGER; BankID INTEGER; Detail TEXT(255); Amount CURRENCY NOT NULL; Note MEMO; CreateDate DATETIME DEFAULT Now(); ToFundID INTEGER (transfer destination fund); ToBankID INTEGER (transfer destination bank); ReceiptPath TEXT(255) (relative file name under `Receipts\`) | TranType is the primary discriminator; date filtering uses `TranDate` with Buddhist-year normalization; `ToFundID`/`ToBankID` are the transfer source-destination extension; LEFT JOINs to Categories/Funds/BankAccounts (×2) in every ledger/report query |
| **Personnel** | `CREATE TABLE Personnel (PersonnelID COUNTER PRIMARY KEY, Title TEXT(50), FirstName TEXT(100), LastName TEXT(100), FullName TEXT(255), PersonType TEXT(50), PositionID INTEGER, Phone TEXT(50))` | `PersonnelID` COUNTER | Title TEXT(50); FirstName TEXT(100); LastName TEXT(100); FullName TEXT(255) (built as `Title&FirstName[ LastName]`); PersonType TEXT(50) ('Monk'/'Layperson'); PositionID INTEGER (FK→Positions); Phone TEXT(50) | `FullName` is the join key used by legacy TempleSetting migrations; `PositionID` added by migration for FK to Positions |
| **Positions** | `CREATE TABLE Positions (PositionID COUNTER PRIMARY KEY, PositionName TEXT(100), PositionGroup TEXT(20))` | `PositionID` COUNTER | PositionName TEXT(100); PositionGroup TEXT(20) — seeded values `'พระ'` (monk) / `'ฆราวาส'` (layperson) / `'ทั่วไป'` (general) | `PositionGroup` added by migration (see 3.3); drives the abbot-vs-lay ComboBox split in FrmTempleSetting via `WHERE pos.PositionGroup = 'พระ'` / `'ฆราวาส'`; PositionName unique-ish (seed checks `WHERE PositionName = @n`) |

### 3.3 Schema Migrations (run inside `Db.EnsureSchema`, all idempotent)

1. **Positions.PositionGroup** — `conn.GetSchema("Columns", ...)` check; if missing → `ALTER TABLE Positions ADD COLUMN PositionGroup TEXT(20)`.
2. **TempleSetting personnel ID columns** — adds `AbbotPersonnelID`, `WaiyawatPersonnelID`, `BookkeeperPersonnelID` (INTEGER) if missing.
   - **Data migration from legacy names:** if old column `AbbotName` exists → `UPDATE TempleSetting INNER JOIN Personnel ON TempleSetting.AbbotName = Personnel.FullName SET TempleSetting.AbbotPersonnelID = Personnel.PersonnelID WHERE TempleSetting.AbbotPersonnelID IS NULL`; else if old `AbbotID` exists → copy `AbbotPersonnelID = AbbotID`. Same pattern for Waiyawat / Bookkeeper.
   - **Drops legacy columns** (best-effort, ignores errors): `{ABBOTNAME, ABBOTOFFICESTATUS, WAIYAWATNAME, WAIYAWATOFFICESTATUS, BOOKKEEPERNAME, BOOKKEEPERTYPE, ABBOTID, WAIYAWATID, BOOKKEEPERID}` via `ALTER TABLE TempleSetting DROP COLUMN <lc>`.
3. **Personnel.PositionID** — if missing → `ALTER TABLE Personnel ADD COLUMN PositionID INTEGER`; then seeds defaults `UPDATE Personnel SET PositionID = 1 WHERE PersonType = 'Monk'` (เจ้าอาวาส) and `UPDATE Personnel SET PositionID = 2 WHERE PersonType = 'Layperson'` (ไวยาวัจกร/ผู้ทำบัญชี).
4. **Transactions.ToFundID / ToBankID / ReceiptPath** — if missing → `ALTER TABLE Transactions ADD COLUMN ... INTEGER/INTEGER/TEXT(255)`.

### 3.4 Seed Data (only when tables are empty)

- **Categories (10):** Income — เงินบริจาคทั่วไป, เงินทอดพระเนตร, ดอกเบี้ยเงินฝาก, รายได้อื่นๆ; Expense — ค่าอาหารและข้าวสาร, ค่าน้ำประปา, ค่าไฟฟ้า, ค่าซ่อมบำรุง, ค่าอุปกรณ์วัด, รายจ่ายอื่นๆ.
- **Funds (4):** กองทุนทั่วไป, กองทุนกฤติยาธรรม, กองทุนกู้ภัยวัด, กองทุนซ่อมแซม.
- **BankAccounts (1):** ธนาคารออมสิน / "-" / วัดแหลมยาง.
- **Positions (19):** พระ group — เจ้าอาวาส, รองเจ้าอาวาส, ผู้ช่วยเจ้าอาวาส, เลขานุการเจ้าอาวาส, พระภิกษุ, พระลูกวัด, พระอาจารย์, สามเณร; ฆราวาส group — ไวยาวัจกร, รองไวยาวัจกร, ผู้ช่วยไวยาวัจกร, เหรัญญิก, ผู้ทำบัญชี, เจ้าหน้าที่การเงิน, กรรมการวัด, ผู้ดูแลทรัพย์สิน, เจ้าหน้าที่สำนักงานวัด; ทั่วไป group — อาสาสมัคร, อื่น ๆ. (Upsert: INSERT if absent, else UPDATE PositionGroup.)
- **Personnel (6 samples):** พระครูสมุห์ สมชาย (เจ้าอาวาส/Monk), พระมหา วิทยา (พระภิกษุ/Monk), นาย สมศักดิ์ ใจดี (ไวยาวัจกร), นาย ประเสริฐ บุญมี (เหรัญญิก), นางสาว พรทิพย์ สุขใจ (ผู้ทำบัญชี), นาย อนันต์ แสงทอง (กรรมการวัด). Dedupe on `FullName`.

### 3.5 The `Db` Module API (Database.vb)

| Member | Signature | Purpose |
|---|---|---|
| `ConnectionString` | `ReadOnly Property` | ACE OLEDB 12.0 provider string |
| `OpenConn` | `Function ... As OleDbConnection` | Opens & returns a new connection |
| `EnsureSchema` | `Sub` | Create tables, run migrations, seed defaults (cached) |
| `ExecuteNonQuery` | `Function(conn, sql, ParamArray Tuple(Of String,Object)())` | Parametrized command; `Nothing` → `DBNull.Value` |
| `DbScalar` | `Function(conn, sql, ParamArray params) As Object` | Parametrized scalar; DBNull → Nothing |
| `ToDecimalOrZero` / `ToIntOrZero` | `Function(value As Object)` | Null-safe converters |
| `GetTable` | `Function(conn, sql, ParamArray params) As DataTable` | `OleDbDataAdapter.Fill` wrapper |
| `InsertAndGetId` | `Function(conn, sql, ParamArray params) As Integer` | ExecuteNonQuery + `SELECT @@IDENTITY` |
| `NormalizeGregorianDate` | `Function(value As Date) As Date` | If `Year > 2400` subtract 543 (Buddhist → Gregorian) |
| `AccessDateLiteral` | `Function(value As Date) As String` | `"#yyyy-MM-dd HH:mm:ss#"` invariant, after normalization |
| `EscapeLikeText` | `Function(value) As String` | Escapes `[ * ? #` for Access LIKE |
| `BackupDatabase` | `Sub(Optional targetDir)` | Copies accdb to `Backup\TempleAccounting_yyyyMMdd_HHmmss.accdb` |

### 3.6 Data Dictionary Conventions

- **Amount** is `CURRENCY` (Access Currency = 4-decimal scaled decimal; all code displays `N2`).
- **Dates:** `TranDate`/`CreateDate` stored as DATETIME; **Buddhist-era ambiguity resolved at every read/write** — writes normalize via `NormalizeGregorianDate`, reads convert via `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)`.
- **Receipt files:** only the relative file name is stored (`Receipt_{ID}.jpg`); full path = `AppPaths.ReceiptsDir\fileName`.
- **Transfer modeling:** a transfer is one row with `TranType='Transfer'`, source in `FundID`/`BankID`, destination in `ToFundID`/`ToBankID`, `CategoryID` NULL. No double-entry ledger rows.

---

## 4. Core Business Logic & Calculations

### 4.1 Transaction Lifecycle

- **Income / Expense** (`FrmIncome`, `FrmExpense`): single INSERT with `TranType='Income'|'Expense'`, `CategoryID` + `FundID` required (validated), `BankID` optional (blank row = DBNull). Edit path updates the same row in place. After edit-save, the form navigates back to the ledger via `frmMain.btnMember.PerformClick()`.
- **Transfer** (`FrmTransfer`): single INSERT with `TranType='Transfer'`, `CategoryID` NULL, source `FundID`/`BankID`, destination `ToFundID`/`ToBankID`, at least one source and one destination, source ≠ destination. `Detail` auto-built: `โอนเงินภายใน: กองทุน X / บัญชี Y -> กองทุน X / บัญชี Y`.
- **Ledger editing** (`FrmTransactions`): row-level route — Income → `FrmIncome.EditID`, Expense → `FrmExpense.EditID`, Transfer → in-grid edit mode (intended but currently non-functional, see §7).

### 4.2 Balance & YTD Calculations

| Calculation | Where | SQL / Formula |
|---|---|---|
| Global net balance (คงเหลือ) | Dashboard, Expense overview, reports | `SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions` |
| Month income count/sum | Dashboard / Donation overview | `COUNT(*)` / `SUM(Amount)` `WHERE TranType='Income' AND TranDate BETWEEN monthStart AND monthEnd` (monthEnd = `AddMonths(1).AddSeconds(-1)`) |
| Month expense count/sum/avg | Expense overview | same pattern with `TranType='Expense'`; avg = total/count in VB |
| YTD net income (รายได้สุทธิ YTD) | Report overview | `SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions WHERE TranDate >= yearStart` (yearStart = Jan 1 of current year) |
| Prior-year balance (ยอดยกมาต้นปี) | Report overview | same SUM minus, `WHERE TranDate < yearStart` |
| Ledger totals (รายรับ/รายจ่าย/โอน/คงเหลือ) | `FrmTransactions.lblSummary` | sums computed **in VB** from the filtered DataTable: `sumInc`, `sumExp`, `sumInc-sumExp`, `sumTrf` |
| Report filter balance (คำนวณยอดคงเหลือ) | `FrmReports.btnCalcBalance` | balance **before** `dtpFrom` + movements inside range (see §5.3) |
| Transfer stats | Activity overview | count/sum of `TranType='Transfer'` month + all-time |

### 4.3 Thai/Buddhist Date Handling (hard rule)

- **Write path:** `Db.AccessDateLiteral(dateValue)` → `NormalizeGregorianDate` (if `Year > 2400` → `Year - 543`) → `"#yyyy-MM-dd HH:mm:ss#"` (InvariantCulture).
- **Read path:** SQL expression `IIF(Year(t.TranDate)>2400, DateAdd('yyyy',-543,t.TranDate), t.TranDate)` so the app receives Gregorian dates.
- **Display:** `ToString("dd/MM/yyyy")`; Thai month names (in reports) via `th-TH` culture + `ConvertToThaiDigits` (๐-๙).
- **Literals:** `dtpTo` end-of-day = `dtpTo.Value.Date.AddDays(1).AddSeconds(-1)` to include the whole day.

### 4.4 Money Formatting

- Input: `txtAmount` KeyPress allows only digits and a single `.`.
- Display: `Convert.ToDecimal(x).ToString("N2")` everywhere (thousand separators, 2 decimals); report values also `N2`; large card values `"#,##0"` or `"#,##0.00"`.
- Excel import amount parsing strips `","` and `"บาท"` before `Decimal.TryParse`.

### 4.5 Excel Import Rules (`ExcelTransactionImporter`)

- Uses OLE DB `Microsoft.ACE.OLEDB.12.0` with **Excel 12.0 Xml** (`*.xlsx`) / **Excel 8.0** (`*.xls`) providers, `HDR=YES;IMEX=1`.
- Sheet preference: Income → `รายรับ$/income$/receipts$/incomes$`; Expense → `รายจ่าย$/expense$/expenses$/payments$`; header aliases (Thai + English) for date/detail/amount/note/fund/category/bank columns (7 alias arrays).
- **Pre-import safety:** `Db.BackupDatabase()` (timestamped copy to `Backup\`).
- **Duplicate detection** by full-column match: `"SELECT COUNT(*) FROM Transactions WHERE TranType=@t AND DateValue(TranDate)=... AND CategoryID=@c AND FundID=@f AND IIF(BankID IS NULL,0,BankID)=@b AND [Detail]=@d AND Amount=@a AND IIF([Note] IS NULL,'',[Note])=@n"`.
- **Auto-create master data:** missing Categories (`INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)`), Funds (`INSERT INTO Funds (FundName) VALUES (@n)`), Banks (`INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n, @a, @nm)` with NULLs).
- Result summarized by `TransactionImportResult.BuildSummaryMessage(label)` (counts imported/skipped/duplicated, failures).

### 4.6 Receipt Image Handling (`ReceiptImageHelper`)

- `MaxSize = 1600` px (longest edge); resized preserving aspect; JPEG quality `80L`.
- Output: `Receipts\Receipt_{transactionID}.jpg`; `AppPaths.EnsureDirectoriesExist()` before save.
- Attach flows: new-record (browse file / paste from LINE clipboard), retrofit (FrmTransactions วางรูปย้อนหลัง / เลือกรูปย้อนหลัง), view (shell open), delete (file + NULL column), multi-delete (files cleaned after DB transaction commit).

### 4.7 Personnel / Temple-Info Business Rules

- A person has one `PositionID`; `Positions.PositionGroup` (`'พระ'`/`'ฆราวาส'`) decides which role ComboBox they appear in (abbot = พระ group; waiyawat & bookkeeper = ฆราวาส group).
- `TempleSetting` is a **single-row snapshot** — save = `DELETE FROM TempleSetting` + INSERT (not UPDATE).
- Legacy column migration reconstructs `AbbotPersonnelID` etc. from old `AbbotName` (join on `Personnel.FullName`) or old `AbbotID`.
- Report headers pull the temple name + personnel from `TemplateInfo` (cached by `ReportEngine`; cleared by `ClearTemplateInfoCache()` after temple save).

### 4.8 Backup / Restore Business Rules (`DatabaseBackupHelper`)

- `BackupFullSystem(targetDir, isAuto)` — copies the accdb + `Receipts\` images into `targetDir\Backup_yyyyMMdd_HHmmss\`; when `isAuto=True` uses `AppPaths.BackupFolder`. Forces `GC.Collect()` before file copy to release OLEDB locks.
- `RestoreFullSystem(backupFolder)` — creates a **safety backup** of the current DB first, then restores DB + images; on failure rolls back to the safety copy; returns Boolean success → `Application.Restart()`.
- Auto-backup on `FrmMain_FormClosing` (silent Try/Catch).

---

## 5. Reporting & Visualization Subsystem

### 5.1 `FrmReports` — Report Center (dual-mode)

**Files:** `Forms\FrmReports.vb` + `.Designer.vb`. Form size 1280×820, maximized, `AutoScroll=True`, cream background, Tahoma 10.5.

**Layout:** filter bar (top) → 8-button report toolbar → `lblSummary` (yellow strip) → `dgvReport` (fill) → `chartMonthly` (overlaid, hidden by default).

**Filter row controls:** `dtpFrom` "ตั้งแต่:", `dtpTo` "ถึง:", `cboType` "ประเภทรายการ:" (`ทั้งหมด/รายรับ/รายจ่าย/โอนภายใน`), `cboFund` "กองทุน:" (all funds + blank "ทุกกองทุน"), `cboBank` "บัญชีธนาคาร:" (all banks + blank), `txtBalance` (read-only), `btnCalcBalance` "คำนวณยอดคงเหลือ", `btnRefresh` "รีเฟรช".

**8 uniform toolbar buttons (same width/height via `UiFitter.UniformButtonGroup`):**
| Button | Text | Purpose |
|---|---|---|
| `btnPrintDetail` | 📜 รายงานละเอียด | A4 detailed report (all columns) |
| `btnPrintSummary` | 📚 รายงานย่อ | A4 summary report (category totals) |
| `btnSummaryIncome` | 💵 สรุปรายรับ | Income-only category summary |
| `btnSummaryExpense` | 💸 สรุปรายจ่าย | Expense-only category summary |
| `btnShowChart` | 📊 กราฟสรุปรายเดือน | Toggle monthly bar chart view |
| `btnMonthly` | 📈 รายเดือน | Monthly aggregated grid |
| `btnLedger` | 📒 สมุดรายวัน | Daily ledger grid |
| `btnPrint` | 📊 Excel | Export grid to CSV |

**Event handlers (14 total):** `FrmReports_Load`, `FrmReports_KeyDown` (F1), `btnRefresh_Click`, `btnCalcBalance_Click`, `btnPrintDetail_Click`, `btnPrintSummary_Click`, `btnSummaryIncome_Click`, `btnSummaryExpense_Click`, `btnShowChart_Click`, `btnMonthly_Click`, `btnLedger_Click`, `btnPrint_Click` (CSV export), `chartMonthly_MouseWheel` (zoom), `chartMonthly_MouseDoubleClick` (reset zoom).

### 5.2 Grid SQL (query builder shared by grid + print)

- **Ledger select** (รายละเอียด / สมุดรายวัน): same 5-table LEFT JOIN SELECT as `FrmTransactions` plus `cboType`/`cboFund`/`cboBank` filters and date-range `BETWEEN`; `ORDER BY` TranDate DESC, ID DESC.
- **Category summary** (รายงานย่อ / สรุปรายรับ / สรุปรายจ่าย):
```sql
SELECT c.CategoryName AS [หมวด], SUM(t.Amount) AS [รวม], COUNT(*) AS [จำนวนรายการ]
FROM Transactions t LEFT JOIN Categories c ON t.CategoryID = c.ID
WHERE <TranDate BETWEEN range> [AND t.TranType='Income'|'Expense'] [AND t.FundID=@f] [AND t.BankID=@b]
GROUP BY c.CategoryName ORDER BY SUM(t.Amount) DESC
```
- **Monthly summary** (`GetMonthlySummary`): groups by `FORMAT(t.TranDate,'yyyy-mm')` (after Buddhist normalization) with `MonthLabelThai` producing labels like "ม.ค. 68"; columns = month label, income, expense, net.
- **Balance before date** (`btnCalcBalance`): `SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions WHERE TranDate < d1` → shows "ยอดคงเหลือก่อนวันเริ่มต้น + ยอดในรอบ" in `txtBalance`.

### 5.3 CSV Export (`btnPrint` — "📊 Excel")

- Writes a **UTF-8 CSV with fully-quoted cells** to `AppPaths.ExportFolder` as `Report_yyyyMMdd_HHmmss.csv` (Buddhist-year timestamp in sample exports `Report_25690726_...`).
- Copies the exported path to the Clipboard and shows a Thai success MessageBox.

### 5.4 Monthly Bar Chart (`chartMonthly` — full config)

- Control: `System.Windows.Forms.DataVisualization.Charting.Chart`, `Dock=Fill`, hidden (`Visible=False`) until `btnShowChart` toggles grid↔chart.
- **Data binding pattern:** NOT `DataSource` binding — manual per-point `Series.Points.AddXY(monthIndex, value)` with separate `AxisLabel` (avoids the shared-axis label issue).
- **Series (3 Column series):**
  1. รายรับ (income) — `Color.MediumSeaGreen`
  2. รายจ่าย (expense) — `Color.IndianRed`
  3. เงินคงเหลือสุทธิ (net) — `Color.SteelBlue`
- `PointWidth = 0.7`; series appended with **"รวม" total bars** in bold colors (ForestGreen / Firebrick / RoyalBlue).
- **Axes:** `AxisY.ScaleView.Zoomable = True` + `ScrollBar.Enabled = True`; custom mouse-wheel zoom via `chartMonthly_MouseWheel` (zoom factor 0.9 / 1.1); `AxisY` label format `N2`.
- **Reset:** `chartMonthly_MouseDoubleClick` → `AxisY.ScaleView.ZoomReset()`.
- Month labels from `MonthLabelThai` (th-TH culture, `ConvertToThaiDigits` ๐-๙).

### 5.5 A4 GDI+ Printing Pipeline

**Files:** `Reports\BaseReport.vb` (abstract scaffolding, unused), `Reports\IncomeExpenseReport.vb` (the real engine), `Reports\ReportEngine.vb` (facade).

- `IncomeExpenseReport` **directly `Inherits PrintDocument`** (BaseReport is left as abstract scaffolding).
- **Paper:** A4 **landscape** at 96 DPI → `1169×827` px; margins 32/32/28/28.
- **Font hierarchy (Tahoma):** page title 16pt bold → subtitle 12pt → column headers 10pt bold → row body 9.5pt → footer 10pt; temple header via `TemplateInfo` (name/address/phone + personnel, cached by `ReportEngine.GetTemplateInfo`).
- **`DrawHeader`** — temple name banner + report title + date range + "รายงานรับ-จ่ายเงินของวัด".
- **`DrawSummaries`** — official 3-row summary block:
  1. รวมรายรับ (total income) / รวมรายจ่าย (total expense)
  2. รวมทั้งสิ้น (grand total) + **ยอดยกไป (carry-forward) in RED**
  3. Signature lines (บรรทัดเซ็น) — "ลงชื่อ............ผู้ทำบัญชี / เจ้าอาวาส"
- **`DrawSignatureBlock`** / `BuildCarryForwardLabel` — sign-off area and carry-forward text.
- **Report modes:** `ReportModes.Detailed` (every transaction row) vs `ReportModes.Summary` (per-category totals); `GetTemplateInfo` resolves temple settings.
- **Preview:** `ShowPreview()` opens a **`PrintPreviewDialog`** ONLY (no direct `PrintDialog`; user prints from the preview window).
- **Row-height/dynamic-layout lessons:** footer cut-off avoided by computing page height budget; dynamic table height to eliminate gaps; A4 page-1 enforcement for single-page summaries (see §7).

---

## 6. Help & System Utilities

### 6.1 `HelpSystem` (HelpSystem.vb — central F1 engine)

| Member | Signature | Behavior |
|---|---|---|
| `SetupHelp` | `Sub(frm As Form, sectionName As String)` | Sets `frm.KeyPreview = True`; appends (or creates) a StatusStrip with a right-aligned hint label `"💡 คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้าจอนี้"` |
| `ShowManual` | `Sub(formName As String, owner As Form)` | Opens modal `FrmHelpDialog(formName)` centered on `owner` (dialog centering fix passed the owner form); reads `USER_MANUAL_TH.md` |
| `AddStatusBarHint` | `Sub(frm As Form, Optional text As String)` | Appends the F1-hint label to a StatusStrip (used by `SetupHelp`) |

**Integration contract (hard convention):**
1. Every primary form sets `Me.KeyPreview = True` (via `SetupHelp`).
2. Load event calls `HelpSystem.SetupHelp(Me, "<FormName>")`.
3. Each form implements its own `Handles Me.KeyDown` (NOT anonymous AddHandler in a central module — a lesson learned to avoid event conflicts):
```vb
Private Sub FrmXxx_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
    If e.KeyCode = Keys.F1 Then
        e.Handled = True
        e.SuppressKeyPress = True
        HelpSystem.ShowManual("FrmXxx", Me)
    End If
End Sub
```
4. `ShowManual("FrmXxx", Me)` always passes the owner form so `FrmHelpDialog` centers on it.

### 6.2 `FrmHelpDialog` (Forms\FrmHelpDialog.vb) — Markdown renderer

- Constructor `New(formName As String)`; form 684×418, header color `#2D3E50`, close button `#E74C3C`, Tahoma content font.
- `LoadHelpContent()` — reads `USER_MANUAL_TH.md` (app-local), extracts the section matching the requested form, renders markdown into `rtbHelp` (RichTextBox, Segoe UI 11pt content).
- `ExtractSection(formName)` regex: `"## (.*?)\(" & Regex.Escape(formName) & "\)([\s\S]*?)(?=---|\n##|$)"` — matches `## N. ชื่อ (FrmXxx)` headers up to the next `---` or `##`.
- `RenderMarkdownAsRichText(...)` — bolds `**…**` segments (blue `#0066CC`), preserves paragraphs; falls back to a "not found" message with the section list when the form has no manual section.
- Only event handler: `btnClose_Click` → `Me.Close()` (modal).

### 6.3 `USER_MANUAL_TH.md` (manual source)

- Thai-language markdown with one section per form: `## 1. หน้าจอหลัก (FrmMain)`, `## 2. ... (FrmIncome)`, `(FrmExpense)`, `(FrmTransfer)`, `(FrmTransactions)`, `(FrmReports)`, `(FrmTempleSetting)`, `(FrmPersonnelManagement)`, `(FrmMasterData)`, `(FrmLocationImport)`, `(FrmHelpDialog)` — each `---`-terminated. Formatting follows the segment rule so the dialog can extract per-form guidance.

### 6.4 `AppPaths` (AppPaths.vb) — app-local path catalog

- **All paths are app-local under `AppDomain.CurrentDomain.BaseDirectory`** (never `%AppData%`): `Database`, `Import`, `Backup`, `Export`, `Logs`, `Receipts` folders; properties `DatabaseFile`, `BackupFolder`, `ImportFolder`, `ExportFolder`, `LogsFolder`, `ReceiptsDir`, `ProvinceCsv()`/`DistrictCsv()`/`SubDistrictCsv()`.
- `FindPreferredDatabaseFile()` — walks **up 12 directory levels** from the app base, preferring `<project root>\Database\TempleAccounting.accdb`; falls back to `<AppRoot>\Database\TempleAccounting.accdb`.
- `EnsureDirectoriesExist()` — creates all folders.
- `LogCrash(ex, tag)` — appends `Timestamp | tag | exception text` to `Logs\crash.log`.
- `ReadLastErrorLog()` — reads last crash-log entry for the diagnostics status label.

### 6.5 `Program.vb` — Bootstrap & error "Beacon"

1. `Application.SetHighDpiMode(HighDpiMode.SystemAware)`.
2. `ApplicationConfiguration.Initialize()` (default WinForms startup).
3. `AppPaths.EnsureDirectoriesExist()`; DB file check.
4. **Layered Try/Catch "Beacon" handlers**: form-creation errors, top-level unhandled exceptions, `ThreadException` — each logs `crash.log` (`AppPaths.LogCrash`) and shows a Thai error dialog with a hint to install **Microsoft Access Database Engine 2016 Redistributable** when OLEDB fails (with download URL).
5. `Application.Run(New frmMain)`.
- **No single-instance mutex**; each run can start independently.

### 6.6 `MessageBoxHelper` (MessageBoxHelper.vb)

- Global helper that shows MessageBoxes with a consistent **Tahoma 8pt** font by creating a temporary owner form (avoids Thai text clipping in the default Segoe UI dialog font).
- `ShowInfo(title, message)` / `ShowError(...)` Thai defaults; used by backup/restore, import, and error paths.

### 6.7 `UiFitter` (UiFitter.vb) — UI auto-fit toolkit (full public API)

| Member | Purpose |
|---|---|
| `AutoFitButtonText(btn, minFontSize=9, maxFontSize=11)` | Shrinks font 0.5pt at a time until `TextRenderer.MeasureText` (NoPadding/NoClipping) fits inside the button + 8px padding; wraps to 2 lines at the best split point; grows the button as a last resort |
| `AutoFitFormButtons(frm, ...)` | Recursively fits every Button / ToolStripButton in the control tree (incl. ToolStrip/StatusStrip) |
| `UniformButtonGroup(ParamArray btns)` | Makes all buttons in a group the same size (widest needed + 3px), used for the 8-button FrmReports toolbar |
| `ButtonTextNeed(btn)` | Computes the exact size a button needs for its text |
| `DisableComboBoxWheel(parent)` | Recursively attaches `MouseWheel` to every ComboBox; `ComboBox_PreventWheelChange` sets `HandledMouseEventArgs.Handled=True` and forwards `e.Delta` to the nearest `ScrollableControl` with `AutoScroll` (manual `AutoScrollPosition` shift) so wheel-scroll never changes ComboBox values but still scrolls the form |

**Measured button-fit strategy:** pad X/Y 8px each side, min font 9pt, max 11pt, 0.5pt steps, 2-line `vbCrLf` wrap fallback (split at space minimizing the widest line), then grow button.

### 6.8 Supporting helpers

- `ExcelTransactionImporter` — see §4.5 (OLE DB Excel reader, alias maps, dedupe, auto-create masters, `TransactionImportResult.BuildSummaryMessage`).
- `ReceiptImageHelper` — see §4.6 (`SaveOptimizedReceipt`, MaxSize 1600, JPEG 80).
- `DatabaseBackupHelper` — see §4.8 (`BackupFullSystem`, `RestoreFullSystem`, `CopyDirectory`, `CopyImageFilesOnly`).
- `ReportEngine` — facade for report template info (`GetTemplateInfo`, `ClearTemplateInfoCache`).

---

## 7. Known Issues & Recent Refactoring History

### 7.1 Currently Known Issues (unfixed as of V4)

1. **FrmTransactions in-grid edit is non-functional.** `SaveSelectedRowEdits()` (line ~737) is never called anywhere; `btnEdit_Click` never branches on `_isEditing`, so the "💾 บันทึกแก้ไข" affordance (`ApplyEditModeToGrid`) cannot persist edits. Transfer-row editing therefore has no working save path (Income/Expense rows work because they route to the dedicated edit forms).
2. **Corrupted emoji glyphs in `FrmTransactions` Designer:** `btnPasteReceipt.Text = "� วางรูปย้อนหลัง"` and `btnBrowseReceipt.Text = "� เลือกรูปย้อนหลัง"` contain U+FFFD replacement characters (encoding corruption, same class of bug as the frmMain nav icons — fixed for frmMain in Phase D but these two buttons remain). **Recommend:** replace with proper emoji (e.g. 📋 / 📂).
3. **Debug harness residue in FrmTransactions:** an HTTP debug-report client (`DebugSessionId="transactions-grid-empty"`, `DebugRunId="post-fix"`, POST JSON to `http://127.0.0.1:7777/event`, 500 ms timeout) is still wired into Load/LoadData debug points A–D. This is leftover instrumentation from the "transactions-grid-empty" debugging session and should be removed in a cleanup pass.
4. **Dead designer members:** `FrmTransactions.Designer.lblHeader` (declared, never instantiated) and `FrmTempleSetting.Designer.pHeader`/`lblHeader` (configured, never added) — invisible headers; harmless but confusing.
5. **FrmLocationImport CSV parser is naive:** plain UTF-8 `ReadAllLines` + `Split(",")` with no header-skip, no quoted-field handling, no delimiter escaping. Province/amphoe/tambon names containing commas or quotes will import incorrectly.
6. **FrmPersonnelManagement fullName concatenation:** `fullName = $"{Title}{FirstName} {LastName}"` — the title is glued to the first name with no space (e.g. "นายสมศักดิ์ ใจดี").
7. **Delete position without FK guard:** deleting a `Positions` row still referenced by `Personnel.PositionID` throws a DB error surfaced only via the catch MessageBox (no friendly "in use" message as in Categories).
8. **`LoadMonkSample()` in frmMain** references **legacy TempleSetting columns** (TempleName/AbbotName/WaiyawatName/BookkeeperName) that the migration drops — currently safe only because of `IsDBNull` guards; should be migrated to the new personnel-ID model.
9. **Access Engine dependency:** the app requires the 64-bit **Microsoft Access Database Engine 2016 Redistributable**; on machines without it, OLEDB fails and only the Beacon error dialog (with URL) guides the user.
10. **`BaseReport` is dead scaffolding** — `IncomeExpenseReport` inherits `PrintDocument` directly; the abstract base is unused.
11. **Two empty event handlers** (`dgvTransactions_CellContentClick`, `lblSearch_Click`) — no-op code.
12. **`AutoScaleMode` inconsistency:** most forms default to Font; `FrmTempleSetting` uses Dpi; `FrmLocationImport` has no AutoScaleDimensions/AutoScaleMode (None). Mixed scaling may cause minor layout drift on high-DPI.

### 7.2 Resolved Issues & Refactoring History (recent work log)

| Phase | Issue | Resolution |
|---|---|---|
| A | Thai button text truncation (vowel/mark clipping on 68 buttons) | `UiFitter.AutoFitButtonText` with 0.5pt steps + 2-line wrap + button growth; `UniformButtonGroup` for the 8-button report toolbar; wired into 12 form Load events |
| B | Home logo overlay on top of the พระ/อาวาส form (`FrmTempleSetting`), covering bottom action buttons | Root cause = WinForms z-order: `Controls.Add` re-adds to top; guarded `picHomeLogo.Visible=False` in `ShowFormInPanel()`/`ShowPlaceholder()`, re-shown only in `ShowDashboard()`/`ShowHomeLogo()`; single guard covers all 8 hosted forms + the `FrmLocationImport` re-swap path at `FrmLocationImport.vb:242` |
| C | ComboBox values changed by mouse-wheel hover (dropdown closed) | `UiFitter.DisableComboBoxWheel(parent)` + `ComboBox_PreventWheelChange` (`Handled=True` + `AutoScrollPosition` passthrough); wired recursively in FrmIncome/Expense/Transfer/Transactions/TempleSetting/MasterData/Reports Load events; first build error (`AutoScroll` not on `Control`) fixed via `TypeOf ... ScrollableControl` cast |
| D | Broken `"�"` placeholder icons on frmMain sidebar (พิมพ์รายงาน / รายการทางบัญชี) | Corrupted `"�"` chars in `frmMain.Designer.vb` Text properties replaced with `🖨️ พิมพ์รายงาน` / `📖 รายการทางบัญชี`; ImageList keys `report` 🖨️ `#1E40AF` and `member` 📖 `#7C2D12`; Grep confirmed zero `�` remaining; build 0/0 |
| — | F1 help dialog not centering on owner | `HelpSystem.ShowManual` now always receives `Me` as owner; `FrmHelpDialog` centers on it |
| — | Chart label overlap in `chartMonthly` | Per-point `AddXY(monthIndex, value)` with separate `AxisLabel` instead of DataSource binding; 3 column series + "รวม" totals |
| — | Date-range filtering wrong on the last day | `dtpTo.Value.Date.AddDays(1).AddSeconds(-1)` inclusive range + `BETWEEN` |
| — | Report footer cut off on A4 | Dynamic page-height budget; footer drawn within remaining space; single-page summary enforced to page 1 |
| — | Grand-total grid row alignment | `fix_grand_total_grid_plan.md` / `fix_footer_summary_plan.md` / `dynamic_row_height_plan.md` design docs (see `.trae\documents\`) |
| — | Transactions grid sometimes empty after load | Debugged via the `transactions-grid-empty` debug server session (see §7.1 #3); `LoadData()` + `RestoreSelectionById` hardened |
| — | Personnel model refactor | Legacy `TempleSetting` text columns (AbbotName etc.) migrated to FK-style `PersonnelID` columns; `Positions.PositionGroup` introduced for monk/layperson split |

### 7.3 Future Development Roadmap

1. **Fix the FrmTransactions in-grid save path** (wire `SaveSelectedRowEdits` to `btnEdit` when `_isEditing`, or route Transfer edits to a proper edit form).
2. **Repair the two corrupted receipt-button glyphs** in FrmTransactions.
3. **Remove the debug-report harness** from FrmTransactions (debug points A–D + `HttpClient` POST).
4. **Harden the CSV import parser** (quoted-field handling, header row detection, delimiter escaping).
5. **Add friendly FK-in-use messages** for Positions/Personnel deletes.
6. **Introduce an application-wide single-instance guard** and installer-first-time DB provisioning (move the Access Engine check into the installer).
7. **Uniform `AutoScaleMode`** policy across all forms (prefer `AutoScaleMode.Dpi` per the user's stated preference).
8. **Consider double-entry transfer rows** (source −, destination +) to make fund/bank balances independently verifiable per account, instead of single `TranType='Transfer'` rows.
9. **Periodic auto-backup scheduling** (daily/weekly configurable) in addition to close-time auto backup.
10. **PrintDialog option** alongside PrintPreviewDialog for direct printing without preview.
11. **English UI toggle** for non-Thai users (all strings currently hard-coded Thai in code/designers).

---

*End of PROJECT_PROFILE_V4.md — exhaustive audit of the TempleAccounting codebase. All 7 required sections covered at maximum token depth. `PROJECT_PROFILE_V3.md` (and earlier profile documents) and all application source files remain unmodified.*