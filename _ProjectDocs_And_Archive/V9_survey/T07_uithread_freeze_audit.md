T07 — Application freeze / UI-thread audit (V9 survey)
Generated: 2026-10-23. VERIFIED unless marked.

## Global threading facts (grep across main project, excl. bin/obj/tests/tools)
- `Thread.Sleep`: **1 site** — BackupEngine.vb:588 (retry delay; only when token NOT cancelable, l.584-589; async path uses `token.WaitHandle.WaitOne(500)` which is interruptible).
- `.Result` / `.Wait()` / `GetAwaiter().GetResult()`: **0 sites at HEAD**. (V8-era code HAD one: `git show 91d2a5e:DatabaseBackupHelper.vb` l.23-24 `Return BackupFullSystemAsync(...).GetAwaiter().GetResult()` — the sync wrapper removed by f943ecb.)
- `Task.Run`: **only 2 files** — BackupEngine.vb:122 (ExecuteBackupAsync) and Forms/FrmBackupVerify.vb (5×). **Every other long operation in the app runs on the UI thread**: Async/Await count = **0** in FrmMultiMachineImport, FrmLocationImport, FrmIncome, FrmExpense, FrmReports, FrmExportForTransfer, ExcelTransactionImporter (grep -c = 0 each).
- Cross-thread modal dialogs: none found — engine/scan code is pure (no MessageBox), backup dialogs run after `Await` (UI context), other forms never leave the UI thread.
- `BeginInvoke` sites: FrmIncome.vb:473, FrmExpense.vb:415, FrmTransfer.vb:118 = focus-posting (harmless); FrmLocationImport.vb:24,34 and FrmMultiMachineImport.vb:1319,1329 = log/progress re-posts to the same UI queue (no threading benefit).
- Running app (RUNTIME): `lamYangI&E.exe` PID 11140, started 2026-10-23 06:42:52; exe/dll mtime **2026-10-23 05:39** ⇒ the running build INCLUDES f943ecb (05:37) + fdeb598 (05:38) = async backup era.

## frmMain.FormClosing (frmMain.vb:1066-1090) — detailed
1. `If _isBackingUp Then` → MessageBox prompt "กำลังสำรองข้อมูลอยู่ / ยกเลิกแล้วออก หรือ รอให้เสร็จ?"
   - **Yes** → `_backupCts.Cancel()` → close proceeds; engine checks token per file (BackupEngine.vb:165-170, 485-487) and in retry WaitOne (l.585) → backup stops promptly; queued Progress posts hit `If … Me.IsDisposed Then Return` guard (frmMain.vb:948-951); RunBackupAsync Finally guards `If Not Me.IsDisposed` (l.931-935).
   - **No** → `e.Cancel = True` → **app does NOT close** (flag can block closing — by design; user must retry).
2. Then `Try DatabaseBackupHelper.RunTieredBackup() Catch → LogSilentError` — runs **synchronously on the UI thread** during close: `GC.Collect+WaitForPendingFinalizers` + 3× `File.Copy` of the 1.8 MB DB (DatabaseBackupHelper.vb:716-727) + `SyncReceiptsToBackup` (reads/MD5s/copies receipts, l.733-781).
   - **Can an exception leave the app unable to close? NO** — the entire `RunTieredBackup` body is wrapped in one Try/Catch→LogCrash (DatabaseBackupHelper.vb:656-671), and the FormClosing caller has its own Try/Catch (frmMain.vb:1086-1088). Failure ⇒ delayed close (seconds), never a blocked close. Locked `.accdb` copy fails fast (single attempt, catch, log). Close latency normally <1–2 s (local AppRoot target).
   - No backup runs at *startup*; FormClosing is the only automatic trigger.

## btnBackup 💾 (frmMain.vb:886-936) — "freezes after choosing destination folder"
- **Current code: NOT a UI-thread freeze candidate.** Flow: FolderBrowserDialog → `Await RunBackupAsync` (async Sub, frmMain.vb:909) → disable button + show progress panel + cancel button → `Await DatabaseBackupHelper.BackupFullSystemAsync` → `BackupEngine.ExecuteBackupAsync` = `Task.Run` (BackupEngine.vb:122-124) → UI stays responsive with live progress (`OnBackupProgress` frmMain.vb:938-966) and working cancel (CTS, l.968-976).
- Minor UI-thread work that remains: `GC.Collect + WaitForPendingFinalizers` before starting (DatabaseBackupHelper.vb:46-47) ≈ tens–hundreds of ms; the folder dialog itself.
- **Historical freeze (matches the reported symptom), VERIFIED from git**:
  - ≤3cf6363 (through 2026-10-22 morning): `frmMain.btnBackup_Click` called **synchronous** `BackupFullSystem` (git show 91d2a5e:frmMain.vb l.886-894) — raw File.Copy of DB + MD5 receipt sync on the UI thread; frozen window from the moment the folder was chosen until all copies finished (slow D:/USB ⇒ long freeze), success MessageBox only at the end.
  - 9d011b1..91d2a5e (10-22 21:49 → 10-23 04:51): still called the sync wrapper, now `BackupFullSystemAsync(...).GetAwaiter().GetResult()` (91d2a5e DatabaseBackupHelper.vb l.23-24) — **UI message pump blocked for the whole engine run**; progress `Report` callbacks queue to the blocked UI thread ⇒ progress bar/labels never repaint (appears hung); DB-locked Retry dialog still works (modal nested pump).
  - Fixed in f943ecb (2026-10-23 05:37) — frmMain switched to `Await` (current code). The 05:39 build running now includes the fix. RUNTIME re-test of 💾 not performed (would require clicking through the app; left to the user).

## Ranked probable root causes of freeze complaints (evidence first)
| Rank | Root cause | Status | Evidence |
|---|---|---|---|
| **1** | Manual backup ran synchronously on UI thread (raw sync copy ≤3cf6363; `.GetAwaiter().GetResult()` blocking wrapper 9d011b1–91d2a5e) — freeze starts exactly after the destination folder is chosen | **FIXED at HEAD** (f943ecb, `Await` + Task.Run + progress); historical symptom fully explained | git show 91d2a5e:DatabaseBackupHelper.vb:23-24; 91d2a5e:frmMain.vb:886-894; current frmMain.vb:909,924 |
| **2** | **Excel import on UI thread**: FrmIncome.btnImportExcel_Click → `ImportTransactionsFromExcel` synchronous (ClosedXML open + per-row INSERT + pre-import `Db.BackupDatabase` File.Copy + full GC) — freeze ∝ rows | ACTIVE P1 | Forms/FrmIncome.vb:589-611 (calls at 602); ExcelTransactionImporter.vb:87,143-153; 0 async in ExcelTransactionImporter.vb |
| **3** | **Multi-machine merge on UI thread**: btnConfirmMerge_Click → `RunMerge` sync (one big OleDbTransaction l.978-1081, per-row INSERT l.1026+, per-row receipt File.Copy staging l.1020-1037, pre-merge `Db.BackupDatabase` l.940) — `btnConfirmMerge.Enabled=False` cannot repaint while blocked; freeze ∝ rows/files | ACTIVE P1 | Forms/FrmMultiMachineImport.vb:172+ (RunMerge at ~236); 0 Async/Task in file |
| **4** | **FrmLocationImport rebuild on UI thread**: `DELETE FROM SubDistrict/District/Province` + ~8,459 INSERTs in one loop with per-row progress (also non-transactional — T04) | ACTIVE P1 (long, data-unsafe on failure) | Forms/FrmLocationImport.vb:136-248 (DELETEs 148-150; loops 155+) |
| **5** | **Restore on UI thread**: btnRestore_Click → `RestoreFullSystem` sync — safety staging copies of current DB+Receipts + copy DB + merge receipts from chosen (often external) drive | ACTIVE P2 | frmMain.vb:1030-1056; DatabaseBackupHelper.vb:484-560 |
| **6** | **FormClosing tiered backup on UI thread** (blocking close for the copy duration; silent failure) + blocking `GC.Collect` in several backup paths | ACTIVE P2 (bounded seconds; cannot wedge close) | frmMain.vb:1081-1088; DatabaseBackupHelper.vb:46-47,656-671,716-727 |
| **7** | Blocking `GC.Collect()+WaitForPendingFinalizers()` on UI before backup/restore (also in CopyDatabaseToTierFolder every close) | ACTIVE P3 | DatabaseBackupHelper.vb:46-47, 481-482, 717-718 |
| **8** | Engine `Thread.Sleep` retry — only sync/test path (async uses cancellable WaitOne) | MINOR P3 | BackupEngine.vb:581-590 |

## Verdicts for the specific questions asked
- Does closing the app run backup synchronously? **YES** — `RunTieredBackup()` runs sync on the UI thread inside FormClosing (frmMain.vb:1083-1087). Normally fast (local copies); failures logged silently.
- Is there a flag that can block closing? **YES** — `_isBackingUp` (frmMain.vb:1067-1080): choosing "No" at the prompt sets `e.Cancel=True`; the app stays open until the user cancels the backup or it finishes.
- Can an exception leave the app unable to close? **NO** (double-wrapped Try/Catch, cited above); worst case = delayed close. UNVERIFIED edge: an exception thrown by the prompt MessageBox itself would escape FormClosing (no Try/Catch around l.1069-1076) — theoretical only.
- Backup button freeze after destination chosen: **was real (rank 1), fixed in the 05:39 build running now**; remaining sync-heavy UI paths are ranks 2–6.
