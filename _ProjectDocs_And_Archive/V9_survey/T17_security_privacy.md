# T17 — Security, Privacy, Data-Safety (survey notes)

Evidence: HEAD = `0c274d5` + live-DB copy probes + repo scan. Line refs are HEAD.

## 1. Threat-model context

Single-user, **fully offline** desktop app for a temple office (no auth layer by design — access = physical machine access). No network code at all: zero `HttpClient`/`WebClient`/socket usage; the only URL in sources is a download hint inside a MessageBox (`Program.vb:131`, ACE 2016 installer link). Runs `asInvoker` (app.manifest:7) — never requests elevation.

## 2. Secrets & credentials

- **No secrets in code/repo**: password/API-key/token scans return only `CancellationToken` matches; no `.config` connection-string credentials.
- ACE connection string: `Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;` (AccessDbProvider.vb:22) ✔.
- SQLite (Saddhiviharika): local file path only.

## 3. SQL injection posture (audit of all SQL-bearing lines)

- Full parameterization for all user-entered values (names, details, notes, amounts, IDs) — Tuple-style `@params` everywhere (T04 inventory).
- Only concatenations into SQL are: (a) `Db.AccessDateLiteral` — a **typed Date re-formatted invariantly** (T05-audited, cannot carry injection payload); (b) constant WHERE fragments passed alongside parameter arrays (FrmReports `fromWhere`/`sqlBase`); (c) `WHERE ID = " & id` where `id` is a grid-cell Integer (FrmExpense:56, FrmIncome:60) — type-safe, style-only nit.
- Worksheet names embedded in `SELECT * FROM [name]`: FrmMultiMachineImport:588 escapes `]`→`]]`; ExcelTransactionImporter:197 doesn't — but Excel forbids `]` in sheet names, so both are practically safe (minor inconsistency).
- **Verdict: no exploitable SQL injection surface for the single-user model.**

## 4. File-system & shell surface

- `Process.Start` sites (5): explorer with `AppPaths` folders ×3, open manual PDF, open receipt — all app-controlled paths.
- **P3 — path handling trusts DB content:** `ReceiptImageHelper.ResolveReceiptPath` does `Path.Combine(folder, fileName)` with no `Path.GetFileName` sanitization; its result feeds `File.Delete` (FrmTransactions:479/930) and `Process.Start(..., UseShellExecute)` (FrmReceiptViewer:74). A tampered `Transactions.ReceiptPath` containing `..\..` could delete/open outside the Receipts folder. Requires DB write access already (receipt paths in practice are `Receipt_{id}.jpg` or GUID names written by the app) → low severity, cheap hardening.
- Excel open path: ACE reads cell values; macros in `.xlsm` are not executed by OLEDB; **200,000-row cap** (`MaxRows`, FrmMultiMachineImport:597) guards OOM from hostile/huge files. ✔

## 5. Data at rest (the main exposure)

- **Access DB is unencrypted** — the survey copy opened passwordless; no `Database Password`/`Encode` anywhere. Whole ledger is readable by anyone with file access.
- **Sensitive columns present:** `TempleSetting.PromptPayID` (national-ID/phone equivalent) + `PromptPayName`; `Personnel` (full names, titles, phones, roles); `BankAccounts.AccountNo/AccountName`; transaction details/amounts; receipt images (photos of temple financial documents).
- **Backups are plain copies** (DB + Receipts) written to `D:\สำรองข้อมูลบัญชี` (or user-chosen drive, incl. USB) — unencrypted; transfer exports likewise plain `TransactionData.xlsx` + `Receipts\*.jpg` in `Export\`.
- `*.accdb` correctly gitignored (`.gitignore:370`) — DB never enters version control. But **2 real report CSVs were committed** (`Export/Report_2569*.csv`, 5 bytes each — effectively empty, no data leaked; still repo litter).
- **Environment-local logs are unbounded and unredacted**: `crash.log` (stack traces, file paths), `error.log` (message texts), `ImportHistory.ImportedBy = Environment.UserName` (audit-trail use — intended). Logs live in the app folder, same ACLs as the app.

## 6. Integrity controls (data-safety, not security)

- Pre-merge/pre-import automatic DB backups (T06/T12); SHA-256 file dedup for imports; single-transaction merge with staging (T12).
- **No FK enforcement** (T13) — integrity by convention only; silent label loss on in-use deletes.
- Receipt deletion swallows all errors (by design, logged nowhere) — T10: caused the 2 orphan files.
- SeedPositions rewrites 19 rows every boot (T06 P1) — no data loss, but constant DB churn defeats "unchanged file" heuristics (e.g., "backup differs from current" checks fire immediately after any boot — T06).

## 7. Issues ranked

| # | Sev | Finding |
|---|---|---|
| 1 | **P2** | DB + backups + exports all unencrypted at rest (PromptPay ID, personnel phones, financials). Single-user temple PC model makes this *acceptable-risk with disclosure*, but should be documented (and optionally protected via Access DB encryption or BitLocker guidance). |
| 2 | **P2** | Unbounded unredacted logs next to the app (privacy + disk growth; T14). |
| 3 | **P3** | `ReceiptPath` not sanitized before `File.Delete`/shell-open (needs pre-tampered DB). |
| 4 | **P3** | Exported CSV/XLSX/backup folders inherit folder ACLs — remind users that `D:\สำรองข้อมูลบัญชี`/USB copies are plain-text data. |
| 5 | Info | No network, no elevation, no secrets, parameterized SQL, no macro execution — attack surface is minimal for its class. |

## 8. Verified-good

- Parameterized SQL throughout; invariant date literals; `Persist Security Info=False`; ACE row cap; bracket-escaped sheet name in merge importer; `asInvoker`; gitignore covers `*.accdb`; import/merge backups + rollback discipline; audit trail (ImportHistory with user + hash + per-batch GUID).
