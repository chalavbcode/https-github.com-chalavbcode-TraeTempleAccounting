# PROJECT_PROFILE_V10 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V10 (incremental update over V9: re-verification of V9 claims at HEAD + V9→V10 delta + new findings)
> **Generated:** 2026-10-24
> **Evidence base:** HEAD = `45047aa` ("Refactor FrmCashCount for DPI-aware layouts & update docs"), branch `modify_backupoct`. Every factual statement cites `<file>:<line>` or a RUNTIME probe. Companion V9 survey notes remain in `_ProjectDocs_And_Archive/V9_survey/`.
> **Label legend:** VERIFIED (read in code at HEAD) / RUNTIME (observed by running a command or on disk) / DOC-ONLY (carried from V9 text, not re-confirmed) / CONTRADICTED (V9 is wrong at HEAD) / RESOLVED (V9 open item now closed, with evidence) / UNVERIFIED (cannot be confirmed without additional evidence).
> **Method:** read-only. All DB probes ran against disposable **copies** (`V9_survey/tmp/dbcopy.accdb`, `V9_survey/tmp/v10_liveprobe.accdb` — copy of `Database/TempleAccounting.accdb` made 2026-10-24); the live DB was never opened for writing. V9 sections not re-audited this session are carried forward and marked DOC-ONLY.

---

## 1. Executive Summary & Version History

### 1.1 Executive Summary

Temple Accounting System (lamYangI&E) is a VB.NET / WinForms desktop application for temple income–expense bookkeeping on **MS Access (OLEDB)**, .NET 10 (`net10.0-windows`), single-user and fully offline. Since V9 (2026-10-23), **6 commits** added a complete **Cash-on-Hand / cash-count subsystem** (dashboard card + `FrmCashCount` + pure-logic service + repository + `CashCounts` table + seed data) and committed the V9 survey artifacts themselves. No backup-system or transaction-layer code changed.

Headline findings:
- **Both V9 P0 defects are still OPEN at HEAD.** (1) The transaction-facade defect (`AccessDbProvider` never assigns `cmd.Transaction`) is untouched — the three DELETE+INSERT saves still throw every time; now at `AccessDbProvider.vb:332-375`. (2) `SeedPositions` still rewrites all 19 Positions rows on every boot (`AccessDbProvider.vb:244-264`), still invalidating backup hash-equality.
- **New P1 — cash-count seed writes real historical money into any empty DB**: `SeedCashCounts` (`AccessDbProvider.vb:295-307`) inserts 3 rows carrying this temple's actual cash counts (0 / 26,820 / 19,390 บาท) into *any* database where `CashCounts` is empty — including a future fresh install for a different temple. RUNTIME-confirmed present in the live DB (3 rows, `ExpectedBefore`/`Variance` all NULL).
- **New P1 — the test suite no longer runs.** `BackupTests/CashOnHandServiceTests.vb` starts with **four stacked UTF-8 BOMs** → `dotnet test` fails with BC30037/BC30627/BC30465 (RUNTIME). All 40 tests (14 engine + 12 legacy + 14 cash-on-hand) are un-compilable; and `BackupTests/` is still gitignored by the `Backup*/` rule, so the broken file is invisible to git.
- **New P2 — hard-coded `CashFundId = 1`** (`CashOnHandService.vb:35`) makes cash classification data-dependent: it is correct only because this DB's FundID 1 happens to be "เงินสด"; a fresh install seeds "กองทุนทั่วไป" as FundID 1 (`AccessDbProvider.vb:227-233`), which would silently misclassify transfers.
- **Encoding regression:** commit `09b36f3` **stripped the UTF-8 BOM from `Services/AccessDbProvider.vb`** (git-diff confirmed) — contradicting the project's "BOM enforced" intent. `frmMain.UiHelpers.vb` is also BOM-less.
- **Build now succeeds clean:** `dotnet build -c Release` → **0 Warning(s), 0 Error(s)** (RUNTIME, 2026-10-24, app not running) — V9's blocked build was environmental, now resolved; the compile itself was and remains clean.
- **V9 doc-integrity error:** V9's stated evidence base "HEAD = `0c274d5`" **does not exist in this repository** (`git cat-file -t 0c274d5` → fatal). V9's numbers (400 commits) match `fdeb598`, which is the true V9 baseline.
- **Three V9 UNVERIFIED items RESOLVED:** `SaveSelectedRowEdits` is wired (not dead code); `LoadMonkSample` uses the personnel-ID schema; the ACE OLEDB 12.0 provider is present and works on this machine.
- **Receipt reconciliation unchanged** (re-run on a fresh copy): 110 transactions, 20 receipt-linked rows, 22 files on disk, **0 missing, 2 orphans** (`Receipt_13.jpg`, `Receipt_56.jpg` — IDs confirmed absent from Transactions).
- **Privacy note:** `cash_dump.txt` (committed in `fb9378b`) contains the temple's **real ledger data** — donor names, amounts, bank names — now tracked in git, alongside survey scripts containing absolute machine paths.

### 1.2 Version History (highlights)

| Version | Date | Key highlights |
|---|---|---|
| **V10** | 2026-10 | Cash-on-Hand subsystem (`CashCounts` table, `CashOnHandService`/`Repository`, `FrmCashCount`, dashboard card, 14 cash tests); V9 survey artifacts committed; re-audit: both P0s still open, new P1 seed-data leak + broken test project, BOM regression, build 0/0 confirmed, 3 V9 UNVERIFIEDs resolved |
| **V9** | 2026-10 | Async backup engine + SHA-256 verification + registry + `FrmBackupVerify`; P0 transaction defect + SeedPositions churn identified; 26 backup tests |
| **V8** | 2026-09 | 5-step Onboarding Wizard; tiered backup; centralized receipts sync; `ErrorLogHelper` |
| V7–V2 | 2026-07/08 | UI parity, charts/preview PDF, DPI rebuild, UiFitter, Help System | *(DOC-ONLY, see V8/V9)* |

---

## 2. V9 → V10 Delta

### 2.1 Git state (VERIFIED)

| Item | V9 | V10 (2026-10-24) |
|---|---|---|
| HEAD | claimed `0c274d5` (**does not exist** — CONTRADICTED; true baseline `fdeb598`) | `45047aa` |
| Total commits | 400 | **406** |
| Unpushed vs `origin/main` | 398 | **404** (branch still local-only) |
| Uncommitted changes | 0 | 0 (clean) |
| Tracked files | 150 | **183** (+3: cash source files; +29: V9 doc, 18 survey notes, 11 tmp scripts/dumps) |
| Tags | v1.0.0, v1.1.0-backup-complete | unchanged |

### 2.2 New commits (fdeb598 → HEAD, 38 files, +3381/−1)

| Commit | Date | Content |
|---|---|---|
| `38fd7e3` | 10-23 | V9 survey notes (T01–T18) + `registry_dump.md` committed |
| `2a301b8` | 10-23 | `PROJECT_PROFILE_V9.md` + `OleDbTxTest.cs/.ps1` + `reconcile.py` committed |
| `09b36f3` | 10-23 | **Cash count feature**: `Forms/FrmCashCount.vb` (+394), `Services/CashOnHandService.vb` (+121), `Services/CashOnHandRepository.vb` (+179), `CashCounts` table + `SeedCashCounts` in `AccessDbProvider.vb` (+39), vbproj (+5), `frmMain.vb` (+1). **Also removed the BOM from AccessDbProvider.vb** |
| `d8e0c7c` | 10-23 | "Cash on Hand" dashboard card (`frmMain.UiHelpers.vb` +143) |
| `fb9378b` | 10-23 | PowerShell cash-analysis scripts + `cash_dump.txt` committed to `V9_survey/tmp/` |
| `45047aa` | 10-24 | `FrmCashCount` DPI refactor (TableLayoutPanel + `K()` scaling, +314/−104), `USER_MANUAL_TH.md` §13 (+5), V9 doc formatting |

**Unchanged since V9:** backup engine family, `DatabaseBackupHelper`, `OnboardingRepository`, `FrmTempleSetting`, transaction layer, all other forms. The two P0 defects were therefore not touched.

### 2.3 Scale (VERIFIED, excl. bin/obj/tools/BackupTests)

- Main project: **27,060 VB lines across 67 files** (V9: 26,077 / 66). New largest entries: `FrmCashCount.vb` 502, `frmMain.UiHelpers.vb` 427 (was 284), `CashOnHandRepository.vb` 179, `CashOnHandService.vb` 121.
- Form classes: **18** (17 in `Forms/` + frmMain partial). F1 coverage gaps now **4 forms**: `FrmReceiptViewer`, `FrmOnboardingWizard`, `FrmBackupVerify`, and the new **`FrmCashCount`** (no `Keys.F1`/`ProcessCmdKey` — VERIFIED by search).
- `AutoScaleMode` tally: **Dpi 6** (…+FrmCashCount, which implements the intended Dpi-only policy with `K()` device-DPI scaling — first form to do it properly), **Font 6**, **unset 6** (`FrmBackupVerify`, `FrmExpense`, `FrmIncome`, `FrmLocationImport`, `FrmMasterData`, `FrmTransfer` — unchanged from V9).

---

## 3. Re-verification of V9 Claims at HEAD

### 3.1 P0 #1 — Transaction-facade defect: **STILL OPEN** (VERIFIED + V9 RUNTIME carried)

`AccessDbProvider.ExecuteNonQuery/DbScalar/GetTable/InsertAndGetId` (now `AccessDbProvider.vb:332-375`; V9 cited 305-345 — lines shifted by the cash-count additions) still create commands via `conn.CreateCommand()` and **never assign `cmd.Transaction`**. The three broken call sites are byte-for-byte unchanged:

| Site | Evidence at HEAD |
|---|---|
| `OnboardingRepository.SaveTempleSetting` | DELETE with `cmd.Transaction` (l.36) then `Db.InsertAndGetId` without (l.41) — `OnboardingRepository.vb:31-57` |
| `OnboardingRepository.SaveOpeningBalance` | DELETE with tx (l.189) then 2× `InsertAndGetId` without (l.196,203) — `OnboardingRepository.vb:184-216` |
| `FrmTempleSetting.btnSave_Click` | DELETE with tx (l.319) then `InsertAndGetId` without (l.325) — `FrmTempleSetting.vb:315-344` |

OleDb enforces assignment: a command executed while a transaction is pending on the connection throws "The Transaction property of the command has not been initialized." (V9 RUNTIME-proven with a disposable DB; the test scripts are now committed at `V9_survey/tmp/OleDbTxTest.cs`/`.ps1`.) Consequence unchanged: **wizard Step 1 / Step 4 saves and FrmTempleSetting save throw and fail every time**; masked on this machine only by the onboarding re-entry guard and existing TempleSetting row.

### 3.2 P0 #2 — SeedPositions rewrites 19 rows every boot: **STILL OPEN** (VERIFIED)

`AccessDbProvider.vb:244-264`: for each of the 19 known positions, the code checks existence and then — in the `Else` branch (l.259-262) — runs `UPDATE Positions SET PositionGroup = @g WHERE PositionName = @n` **unconditionally**, even when the value already matches. Every `EnsureSchema()` (every boot) therefore writes 19 rows, changing the DB file's hash and making complete backups report "ฐานข้อมูลถูกแก้ไขหลังการสำรอง" without any user edit. Fix direction (from V9, still apt): skip the UPDATE when `PositionGroup` already equals the target.

### 3.3 Other V9 claims checked at HEAD

| V9 claim | V10 status | Evidence |
|---|---|---|
| Scan depth ≤3 hides local backups | **STILL OPEN** | `DatabaseBackupHelper.vb:304,336` pass `3` to `BackupEngine.ScanForBackupFolders` (`BackupEngine.vb:436-478`) |
| Registry `AddEntry` own-run path never observed at runtime | **STILL UNVERIFIED** | Code registers post-success at `DatabaseBackupHelper.vb:74-80`; registry file unchanged since 2026-10-23 05:39 (RUNTIME: mtime, 5 entries); `error.log` shows no "สำรอง ->" entry after 05:39 despite ~9 app boots through 2569-10-24 09:05 (`crash.log` tail) |
| Manifest `Created` Buddhist text (BackupEngine.vb:623,658 culture-sensitive) | code defect **STILL PRESENT**; file contents UNVERIFIED | V9 §12.2 carried; D:\ sets not opened this session |
| ~19 culture-sensitive `ToString("yyyy…")` sites | **STILL PRESENT** (representative: `AppPaths.vb:38,48`, `Program.vb:43`, `ErrorLogHelper.vb:27,49`, `BackupEngine.vb:623,658`, `DatabaseBackupHelper.vb:140`, `FrmExportForTransfer.vb:360`) | code search |
| Logs: Buddhist-era timestamps, no rotation | **STILL PRESENT** (RUNTIME: `bin/Release/.../Logs/crash.log` = 77,501 bytes, headers `2569-10-24 …`; `error.log` = 1,566 bytes) | disk |
| `WizardText.Step3Title` mismatch | **STILL PRESENT** (`WizardText.vb:23` — "ตั้งค่าปีงบประมาณ" but Step 3 is bank/funds setup, `FrmOnboardingWizard.vb:676 LoadStep3BankAndFunds`) | code |
| Dev buttons lack `#If DEBUG` guard | **STILL PRESENT** (`frmMain.UiHelpers.vb:21-71` — created in `SetupEventHandlers` → constructor `frmMain.vb:56`) | code |
| No FK constraints; misleading "in use" delete message | **STILL PRESENT** (V9 RUNTIME-proven; schema unchanged) | DOC-ONLY carried |
| Backup freeze fixed at `f943ecb` | **STILL FIXED** (`.GetAwaiter().GetResult()`/`.Wait()` = 0 hits; `Task.Run` only in `BackupEngine.vb` + `FrmBackupVerify.vb`; single `Thread.Sleep` at `BackupEngine.vb:588` in non-cancellable retry) | code search |

### 3.4 V9 UNVERIFIED items — resolutions

| # | V9 UNVERIFIED item | V10 resolution |
|---|---|---|
| 4 | `LoadMonkSample` reads legacy TempleSetting columns | **RESOLVED — migrated.** `frmMain.vb:783-795` JOINs `Personnel` via `ts.AbbotPersonnelID/WaiyawatPersonnelID/BookkeeperPersonnelID` |
| 8 | `FrmTransactions.SaveSelectedRowEdits` dead-code status | **RESOLVED — wired, not dead.** `FrmTransactions.vb:515-518`: `btnEdit_Click` branches `If _isEditing Then SaveSelectedRowEdits()` (fix documented in `FIX_REPORT.md`, commit `b5b8b31`) |
| 9 | ACE OLEDB 12.0 provider present on this machine | **RESOLVED — present.** RUNTIME 2026-10-24: `OleDbConnection` opened `dbcopy.accdb` and `v10_liveprobe.accdb`, rows readable |
| 1,2,3,5,6,7 | fresh-install wizard E2E; user save attempts since `05d5f18`; engine AddEntry at runtime; unset-AutoScaleMode effective default; orphan-receipt forensics; manifest `.Created` contents | **remain UNVERIFIED** (no new evidence; see §11) |

---

## 4. New Subsystem: Cash on Hand / การนับเงินสด (V9→V10 feature)

### 4.1 Architecture (VERIFIED)

| Layer | File | Role |
|---|---|---|
| Schema + seed | `Services/AccessDbProvider.vb:51` (`CashCounts` table, 17 cols), `:295-307` (`SeedCashCounts`, runs inside `EnsureSchema`, **guarded by `COUNT(*) > 0`** — seeds only when empty) | table created + seeded on first boot after upgrade |
| Pure logic | `Services/CashOnHandService.vb` (121 lines) | `ClassifyCashAmount` (l.52-86), `ComputeCashOnHand` (l.91-119), `NormalizeDate` (l.40), `CashFundId = 1` (l.35) — no DB access, unit-testable |
| Data access | `Services/CashOnHandRepository.vb` (179) | `LoadCounts` (l.40), `LoadTransactions` (l.70 — **full-table SELECT, no date filter**), `ComputeCashOnHand` (l.93), `GetLatestCount` (l.98), `SaveCount` (l.108 — INSERT-only; computes `TotalCounted/ExpectedBefore/Variance` before insert) |
| UI (dialog) | `Forms/FrmCashCount.vb` (502) | 9 denomination value-boxes (D1…D1000, storing *values* not counts), auto-total, expected/variance preview (`RecalcExpected` l.402), history grid (`LoadHistory` l.421), confirm-then-save (`BtnSave_Click` l.459). **Dpi + `K()` scaling + TableLayoutPanel + AutoSize labels** (l.41-61) — the reference implementation of the intended DPI policy |
| UI (dashboard) | `frmMain.UiHelpers.vb:169-247` (`SetupCashOnHandCard`, second card row, `pnlOverview.Height = 211` fixed at l.173), `:249-279` (`RefreshCashOnHandCard` — stale-count warning > 30 days), `:282-296` (open dialog); called from `frmMain.vb:393` | wired in `SetupEventHandlers` (`UiHelpers.vb:113`) |
| Docs | `USER_MANUAL_TH.md` §13 (l.148-151) | user-facing description committed in `45047aa` |
| Tests | `BackupTests/CashOnHandServiceTests.vb` — **14 TestMethods** (classification, date normalization, compute scenarios) | **cannot compile — see §6** |

### 4.2 Cash-classification semantics (VERIFIED — read these before trusting the number)

`ClassifyCashAmount` (`CashOnHandService.vb:52-86`):
- **INCOME** → +Amount **iff `ReceiveChannel` is empty or "Cash"** (l.63-67). Legacy rows with NULL channel are therefore treated as cash (matches this DB: many pre-channel rows are cash donations — `cash_dump.txt` Q1).
- **EXPENSE** → −Amount **iff `BankID IS NULL`** (l.69-71). An expense paid from a bank but recorded without `BankID` would be wrongly counted as cash-out — heuristic quality, acceptable for this temple's data but not self-enforcing.
- **TRANSFER** → +Amount when `BankID` present and `ToFundID = CashFundId` (l.72-74); −Amount when `FundID = CashFundId` and `ToBankID` present (l.75-77).
- **Everything else (incl. `OpeningBalance`) → 0.** Opening balances seeded by wizard Step 4 do **not** feed the cash figure; the cash baseline is the first physical count.
- `CashOnHand = last count ≤ asOf + Σ signed movements (countDate, asOf]` (l.91-119).

### 4.3 New findings in this subsystem

1. **P1 — seed leaks real financial history into any empty DB** (`AccessDbProvider.vb:295-307`): the three seeded rows carry actual amounts — 2026-02-20 total 0 ("ยอดตั้งต้น"), 2026-04-30 total **26,820**, 2026-06-09 total **19,390** — with Thai notes claiming "การนับจริงย้อนหลัง". RUNTIME: the live DB contains exactly these 3 rows (`ExpectedBefore`/`Variance` NULL ×3 — seeds skip the computation that `SaveCount` always performs). On a *fresh install* (new temple), the user would inherit this temple's cash history as if it were their own, and the dashboard would show a fabricated "เงินสดในมือ". Fix direction: seed only a zero-baseline row (or nothing), and compute/stamp `ExpectedBefore` like `SaveCount` does.
2. **P2 — hard-coded `CashFundId = 1`** (`CashOnHandService.vb:35`): correct today only because this DB's FundID 1 is "เงินสด" (`cash_dump.txt` Funds). `SeedFunds` (`AccessDbProvider.vb:227-233`) creates "กองทุนทั่วไป" **first** on a fresh DB → FundID 1 = กองทุนทั่วไป → bank↔general-fund transfers would be miscounted as cash movement. Fix direction: resolve the cash fund by name/flag at startup, not by magic ID.
3. **P2 — full-table loads on the UI thread at dashboard refresh** (`CashOnHandRepository.vb:70-92` SELECTs *all* CashCounts + *all* Transactions; called from `RefreshCashOnHandCard`, `frmMain.vb:393`, and again on every `dtpDate` change via `FrmCashCount.RecalcExpected` l.402-407). Harmless at 110 rows; unbounded growth risk — same class as V9's P1 UI-thread findings (Excel import, merge, location rebuild still open).
4. **P3 — `SaveCount` duplicates the compute path** (`CashOnHandRepository.vb:118` reloads counts+transactions to compute `ExpectedBefore` in the same UI flow that just computed it for the preview) — two full scans per save; cosmetic at current scale.
5. **Positive notes:** the service/repository split is the codebase's **first** pure-logic/data-access separation and shipped with 14 unit tests; `SeedCashCounts` correctly uses the seed-only-when-empty guard (unlike `SeedPositions`); `FrmCashCount` is the first form to implement the intended Dpi-only policy; manual documents were updated in the same feature (§13).

---

## 5. Backup & Registry State (re-check)

Unchanged code; refreshed runtime picture (RUNTIME, 2026-10-24):

- `%APPDATA%\TempleAccounting\backup_registry.json`: **5 entries, 24,099 bytes, UTF-8 no-BOM, mtime 2026-10-23 05:39** — identical to V9's dump (`V9_survey/tmp/registry_dump.md`, now committed). Entries 1–2 (complete, `Backup_2026*`) still carry "(เพิ่มจากการสแกนไดรฟ์)"; entries 3–5 legacy-incomplete.
- **No backup run has occurred since V9** despite ~9 boots (`crash.log` tail shows boots through `2569-10-24 09:05`; `error.log` unchanged since `05:39:47`). V9's UNVERIFIED #3 (engine's own `AddEntry` writing a registry entry) therefore **remains unverified** — the next manual 💾 backup should be watched for a new registry entry.
- `error.log` also preserves the V9 observation that the 04:55 scan *rejected* all 5 sets ("ตรวจสอบไม่ผ่าน ไม่เพิ่มเข้าสู่ registry") while the 05:39 `ScanAndRegisterBackups` added all 5 ("พบ=5 เพิ่ม=5 ข้าม=0") — two scan paths with different verification gates coexist.
- Disk inventory, retention gaps (Weekly≤8/Monthly≤15/Yearly≤5 with no completeness interlock), engine sets append-only, scan-depth-3 blindness: **all still as V9 §7.3–7.4** (DOC-ONLY carried; D:\ not re-inventoried this session).

---

## 6. Build, Tests, Encoding (RUNTIME, 2026-10-24)

1. **Build: SUCCESS, 0 Warnings, 0 Errors** (`dotnet build TempleAccounting.vbproj -c Release`, SDK 10.0.401, ~2.4 s). V9's T02 failure was purely the running-app file lock; the app was not running, and the copy stage completed. (Note: this build overwrote `bin/Release/.../lamYangI&E.exe`, mtime now 10-24 09:49.)
2. **Tests: BROKEN — 0 of 40 runnable** (P1, new). `dotnet test BackupTests/BackupTests.vbproj` fails at compile: `CashOnHandServiceTests.vb(1,1-3): error BC30037: Character is not valid`, cascading BC30627/BC30465. Root cause (RUNTIME, byte-level): the file begins with **four consecutive UTF-8 BOMs** (`EF BB BF ×4`) before `Option Strict` — evidently three extra BOMs accumulated while saving. The VB compiler treats the stray BOM bytes as invalid characters. The project previously ran 26 tests (V9); the 14 new cash tests plus the existing 26 are all blocked. Fix: strip bytes 4–12 of the file (or re-save once). Compounding risk: `BackupTests/` remains **untracked and gitignored** (`Backup*/` rule at `.gitignore:262`), so neither the new tests nor this breakage are visible in git history.
3. **Encoding (full-tree scan, excl. bin/obj/.git):** text files — **68 with BOM / 89 without**; `.vb`-only — **52 BOM / 15 no-BOM** (V9: 63/68 across its file set; not directly comparable, but the direction is clear). **Regression:** commit `09b36f3` removed the BOM from `Services/AccessDbProvider.vb` (git diff shows `-﻿Option Strict Off` → `+Option Strict Off`); `frmMain.UiHelpers.vb` is also BOM-less. V9's "0 mojibake / compiler-safe" verdict still holds — this is policy drift and VS-noise, not a functional break — but it directly preceded the quadruple-BOM test-file breakage, indicating saving hygiene needs a guard (e.g., `.gitattributes` `*.vb text working-tree-encoding=UTF-8-BOM` or a lint step).
4. **Empty catches:** 80 one-line `Catch` statements of 260 `Catch` occurrences (counts by this session's regex; V9 reported ~54/255 by a different method — treat as "same order, slightly up"; new adds include `FrmCashCount.LoadHistory` l.455 and the double-empty-catch icon setup at `frmMain.UiHelpers.vb:353-361`).

---

## 7. Receipts & Data Reconciliation (RUNTIME, re-run 2026-10-24 on fresh copy)

| Metric | Value (V9 → V10) |
|---|---|
| Transactions | 110 → **110** (unchanged) |
| Rows with non-empty `ReceiptPath` | 20 → **20** |
| Files `Receipts\*.jpg` (project root) | 22 → **22** |
| DB row without file (missing) | 0 → **0** |
| File without DB row (orphan) | 2 → **2** (`Receipt_13.jpg`, `Receipt_56.jpg`; IDs 13/56 re-confirmed absent from Transactions) |
| `CashCounts` rows | — → **3** (all seeded; `ExpectedBefore`/`Variance` NULL) |
| Funds / Positions / TempleSetting | 6 / 19 / 1 (unchanged) |

Observation (P3): `bin/Release/net10.0-windows/Receipts/` holds only 9 jpgs — a stale legacy copy; the app resolves receipts via `AppPaths` master-dir fallback to the project-root `Receipts/` (22 files), so behavior is correct, but the stale copy invites confusion during manual restore drills.

---

## 8. New Risk & Action List (delta-oriented; V9 items still open are carried compactly)

### P0 — critical (unchanged from V9, still open)

1. **Transaction-facade defect** — `AccessDbProvider.vb:332-375` never assigns `cmd.Transaction`; TempleSetting save + wizard Step 1/4 saves throw every time (`OnboardingRepository.vb:31-57,184-216`, `FrmTempleSetting.vb:315-344`). *Fix: thread the ambient transaction through the facade, or stop mixing facade `InsertAndGetId` into explicit-transaction scopes.*
2. **`SeedPositions` rewrites 19 rows per boot** (`AccessDbProvider.vb:244-264`) — invalidates backup hash-equality. *Fix: skip the UPDATE when `PositionGroup` already matches.*

### P1 — high (new in V10)

3. **`SeedCashCounts` writes this temple's real cash history into any empty database** (`AccessDbProvider.vb:295-307`; live-DB-confirmed 3 rows). *Fix: seed only a zero baseline (or nothing), compute `ExpectedBefore`/`Variance` like `SaveCount`.*
4. **Test project does not compile** — quadruple BOM in `BackupTests/CashOnHandServiceTests.vb` blocks all 40 tests (RUNTIME); `BackupTests/` still gitignored, so the breakage is invisible to git. *Fix: strip the extra BOMs; un-ignore `BackupTests/` (`.gitignore:262`) so tests are versioned.*

### P1 — high (carried from V9, still open)

5. UI-thread freeze risks outside backup: Excel import, multi-machine merge, location rebuild (+ new: cash-card full-table loads, §4.3.3).
6. No referential integrity; misleading "in use" delete message.
7. Non-transactional single-file Excel import; non-transactional location rebuild.
8. Registry own-`AddEntry` never observed; scan depth ≤3 hides local backups (`DatabaseBackupHelper.vb:304,336`).
9. Tier retention deletes by count with no completeness interlock.

### P2 — medium

10. **Hard-coded `CashFundId = 1`** (`CashOnHandService.vb:35`) — breaks on any DB whose FundID 1 isn't "เงินสด" (incl. fresh installs).
11. **BOM regression / save hygiene** — `AccessDbProvider.vb` BOM stripped in `09b36f3`; root cause class of the test-file breakage.
12. **Real ledger data committed to git** — `V9_survey/tmp/cash_dump.txt` (donor names, amounts, banks) + scripts with absolute `C:\Users\fantasy\...` paths. *Consider squashing out or accepting with a privacy note.*
13. Data at rest unencrypted (now also: seed rows + git-tracked dump).
14. Unbounded, unredacted, Buddhist-timestamped logs (`crash.log` 77.5 KB and growing).
15. ~19 culture-sensitive date-format sites.
16. Mixed AutoScaleMode (6 forms still unset); restore + FormClosing tiered backup still on UI thread.
17. `Step3Title` mismatch + wizard Step 1 writes `txtTempleCode` into the `PostCode` column and passes an empty `TempleCode` (`FrmOnboardingWizard.vb:482-490`) — V9's postcode round-trip bug, now precisely located.

### P3 — low (carried + new)

Empty catches (~80); two dialog/log systems; README still 46 bytes; no CI; dead code (`_testReportViewed`, empty `dgvReport_CellContentClick`, `BackupConfig.BackupRootPaths`); hard-coded `C:\LineDownloads`; stale `bin\Release\...\Receipts` copy (9 files); `SaveCount` double-scan; `FrmCashCount` lacks F1 (4th F1-gap form); dev buttons visible in Release.

---

## 9. V9 → V10 Delta Summary

**Added:** cash-count subsystem (3 new source files + `CashCounts` schema/seed + dashboard card + manual §13 + 14 tests); V9 document + 18 survey notes + 11 analysis/test scripts committed to git.

**Changed:** `FrmCashCount` rebuilt DPI-aware; `frmMain.UiHelpers.vb` +143 (cash card); `USER_MANUAL_TH.md` +5; tracked files 150 → 183; commits 400 → 406.

**Removed:** UTF-8 BOM of `Services/AccessDbProvider.vb` (regression, §6.3).

**Contradicted (V9 wrong/outdated at HEAD):**
1. V9's evidence-base hash `0c274d5` does not exist in this repository; the true V9 baseline is `fdeb598` (400 commits — matching V9's own count).
2. V9's "26 passing tests" is no longer true in effect: the test project cannot compile at HEAD (0/40 runnable).

**Resolved (V9 open items closed this session):**
1. `SaveSelectedRowEdits` — wired, not dead (`FrmTransactions.vb:515-518`).
2. `LoadMonkSample` — migrated to personnel-ID JOINs (`frmMain.vb:783-795`).
3. ACE OLEDB 12.0 provider — confirmed present (RUNTIME open of DB copies).
4. V9's blocked Release build — now succeeds 0/0 (environmental cause removed).

**Still open (re-confirmed, not merely carried):** both P0s; scan depth ≤3; registry own-`AddEntry` unverified at runtime; `Step3Title` mismatch; culture-sensitive formats; no log rotation; dev buttons in Release.

---

## 10. Appendix — Key Files at HEAD

| File | LOC | Role / note |
|---|---|---|
| Forms/FrmMultiMachineImport.vb | 1355 | cross-machine merge |
| Reports/ReportEngine.vb | 1141 | shared report engine |
| frmMain.vb / .Designer / .UiHelpers | 1091 / 957 / **427** | shell + cash card |
| Reports/IncomeExpenseReport.vb | 990 | GDI+ print/PDF |
| Forms/FrmTransactions.vb | 955 | ledger CRUD (edit path wired) |
| DatabaseBackupHelper.vb | 857 | tiered backup/restore/scan |
| Forms/FrmOnboardingWizard.vb / .Designer | 819 / 1052 | 5-step wizard |
| BackupLegacyEvaluator.vb | 777 | backup evaluation |
| BackupEngine.vb | 686 | async copy + SHA-256 |
| Forms/FrmIncome.vb / FrmExpense.vb | 666 / 622 | entry forms (~79% duplicated) |
| **Forms/FrmCashCount.vb** | **502** | cash-count dialog (DPI reference impl.) |
| **Services/CashOnHandRepository.vb** | **179** | CashCounts/Transactions reads + save |
| **Services/CashOnHandService.vb** | **121** | pure cash logic (`CashFundId=1` ⚠) |
| Services/AccessDbProvider.vb | 403 | schema/seeds/facade (both P0s live here ⚠) |

**UNVERIFIED at V10 (carry-forward):** fresh-install wizard E2E save failure (mechanism proven, flow untested); whether the engine's own `AddEntry` writes a registry entry on a manual 💾 backup; legacy Buddhist folder parse/sort at runtime; effective default of unset `AutoScaleMode` on high-DPI; historical cause of the 2 orphan receipts; actual `manifest.json` `Created` text on D:\ sets; whether a TempleSetting save was attempted since `05d5f18` (needs user evidence).
