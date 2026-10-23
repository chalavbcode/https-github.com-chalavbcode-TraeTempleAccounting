T10 — Income / Expense / Transfer logic, duplication, receipts reconciliation (V9 survey)
Generated: 2026-10-23. VERIFIED unless marked; reconciliation = RUNTIME on DB copy.

## 1. FrmIncome vs FrmExpense duplication (V8 "~80% shared" re-tested)
- Size: FrmIncome 666 lines vs FrmExpense 622. Method-name inventory: **29 names shared** (LoadMasters, ResetEntry, SaveReceiptFile, btnSave_Click, btnImportExcel_Click, btnPasteReceipt_Click, btnBrowseReceipt_Click, btnClearReceipt_Click, CalculateMonthlySummary, ConfigureRecentGridColumns, LoadTransactionData, RememberSelections, RestoreLastSelections, SetupEnterNavigation, MoveNextFrom, HandleEnterAdvance, ApplyFixedVerticalLayout, …) vs **3 Income-only**: `GetSelectedChannel`, `SetSelectedChannel`, `cboReceiveChannel_SelectedIndexChanged` (+ `GetSelectedBankValueNullable`), **0 Expense-only**.
- Numeric similarity: after normalizing the tokens Income/Expense→X, `diff` = **134 differing lines out of 666/622 ⇒ ≈79% identical** — V8's "~80%" claim **VERIFIED**.
- Real differences (all VERIFIED):
  1. **PromptPay/ReceiveChannel exists only in Income**: 16 `ReceiveChannel|PromptPay` refs in FrmIncome, **0 in FrmExpense** (correct business-wise: channels only apply to income).
  2. **Receipt-path write drift**: after save, FrmIncome calls `_txnService.UpdateReceiptPath(targetID, savedFileName)` (FrmIncome.vb:529-532 → AccessTransactionRepository.vb:47-53 UPDATE … WHERE ID=@id); **FrmExpense builds its own inline OleDb UPDATE with `cmd.Parameters.AddWithValue("@rp", savedFileName)` instead** (FrmExpense.vb ~479-485) — same effect, two implementations (drift risk the `form-duplicate-code` skill warns about).
  3. Labels/queries differ by TranType ('Income'/'Expense', Thai titles) and overview loaders in frmMain.
  4. Hard-coded `C:\LineDownloads` receipt-browse initial dir exists **in both** (FrmIncome.vb:617-619, FrmExpense.vb:574-576) — machine-specific literal in source.
- Both use the same receipt save flow: `SaveReceiptFile(targetID)` → `ReceiptImageHelper.SaveOptimizedReceipt` → `UpdateReceiptPath` only if non-empty (FrmIncome.vb:527-533; FrmExpense.vb:479-485).

## 2. FrmTransfer (206 lines) — V8 claim re-verified
- Single INSERT with To-columns: `INSERT INTO Transactions (TranDate,TranType,FundID,BankID,ToFundID,ToBankID,[Detail],Amount,[Note],TransactionGUID) VALUES (<AccessDateLiteral>,'Transfer',@ff,@fb,@tf,@tb,@de,@a,@n,@g)` + `Guid.NewGuid().ToString("D")` (FrmTransfer.vb:159,167). Date literal ✓, GUID ✓.

## 3. Receipt subsystem (ReceiptImageHelper.vb read in full, 136 lines)
- **Resize**: max dimension 1600 px keeping aspect (MaxSize=1600, l.9), HighQualityBicubic redraw (l.47-54), **JPEG quality 80** (JpegQuality=80, l.10; encoder l.57-61).
- **Naming**: `Receipt_{transactionID}.jpg` (l.65) saved into `AppPaths.ReceiptsDir` (= project-root `Receipts\`, l.66) — filename (not path) is what is stored in `Transactions.ReceiptPath`.
- **ResolveReceiptPath** fallback chain (l.88-106 + GetReceiptCandidateFolders l.115-137): 1) `AppPaths.MasterReceiptsDir` (project-root, survives Rebuild), 2) all legacy folders from `AppPaths.GetLegacyReceiptFolders()` (AppRoot\Receipts, parent-of-AppRoot\Receipts, ProjectRoot\Receipts, ProjectRoot\Images\Receipts, CurrentDir\Receipts, CurrentDir\Images\Receipts — AppPaths.vb:287-316), deduped; returns "" if not found.
- **MigrateReceiptsToMaster** (AppPaths.vb:254-284): one-time non-destructive *.jpg copy from legacy dirs into master at startup (Program.vb Step 4b); per-file failures silently skipped.
- Errors in helper → `AppPaths.LogCrash` + return "" (silent to caller; caller shows success message regardless — receipt failure does NOT block "✅ บันทึกข้อมูลเรียบร้อยแล้ว!", FrmIncome.vb:534).

### Orphan / missing mechanics (code paths that create each state)
- **Row without file**: UPDATE ReceiptPath only runs when `SaveReceiptFile` returned non-empty (guarded, FrmIncome.vb:527-533) — so new rows don't get paths on failure. A row CAN end up without its file if the file is later deleted/moved manually, or the DB is restored from an older backup than the receipts folder, or `ResolveReceiptPath` can't find it (deleted from all candidate dirs).
- **File without row (orphan)**: (i) FrmTransactions multi-delete deletes rows transactionally (l.456-472) then deletes files with **empty Catch** "ปล่อยผ่านถ้าไฟล์ถูกล็อค" (l.476-484) → locked file ⇒ orphan; file also skipped silently when ResolveReceiptPath returns "" (same block); (ii) `RestoreFullSystem` merges images unconditionally via `CopyImageFilesOnly` with overwrite, no DB cross-check (DatabaseBackupHelper.vb:565-597) ⇒ restoring an older DB over newer receipts ⇒ orphans (evaluator even counts them as `ReceiptsExtra`); (iii) UPDATE ReceiptPath failing after file written (separate connections, non-transactional save flow — T04/T05).
- The ONLY receipt File.Delete site in the whole app is FrmTransactions.l.476-484 (grep) — restore/backup never delete receipts (append-only).

## 4. RECONCILIATION RESULT (RUNTIME, read-only copy `tmp/dbcopy.accdb` vs workspace `Receipts\`)
| Metric | Value |
|---|---|
| Transactions total | 110 (Income 63 / Expense 46 / Transfer 1) |
| Rows with non-empty ReceiptPath | **20** (all filename-form `Receipt_{id}.jpg`, IDs 111…167) |
| Files in `Receipts\*.jpg` | **22** |
| **DB row without file (missing)** | **0** ✓ |
| **File without DB row (orphan)** | **2**: `Receipt_13.jpg` (mtime 2026-06-18), `Receipt_56.jpg` (mtime 2026-08-01) |
- Orphan classification **confirmed true orphans**: `SELECT ID FROM Transactions WHERE ID IN (13,56)` on the copy returns **0 rows** (RUNTIME) — the transactions were deleted but their image files remained, consistent with the empty-Catch file-delete at FrmTransactions.vb:476-484 (locked/failed delete skipped silently). Which historical event fired it (delete-lock vs restore-merge vs older state) remains UNVERIFIED; both files date 2026-06-18 / 2026-08-01.

## 5. V8 claims status
- Receipt helper resize/naming/fallback chain: VERIFIED as described in V8 §3.3.
- ReceiptPath + migration: VERIFIED (T03/T04).
- Income/Expense ~80% shared: VERIFIED (79% measured).
