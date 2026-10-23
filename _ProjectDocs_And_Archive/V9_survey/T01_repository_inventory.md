T01 — Repository inventory (V9 survey)
Generated: 2026-10-23. All statements VERIFIED by reading disk/git unless marked.

## Git state (VERIFIED)
| Item | Value | Evidence |
|---|---|---|
| Current branch | `modify_backupoct` (NOT `modify_incomeform` — user's premise CONTRADICTED) | `git branch --show-current` |
| Uncommitted changes | 0 (clean tree) | `git status --porcelain` → empty, count 0 |
| Upstream | none configured for this branch (`fatal: no upstream`); remote `origin` has 8 branches, none named `modify_backupoct` | `git rev-parse @{u}`, `git ls-remote --heads origin` |
| Unpushed commits | 398 (HEAD vs origin/main; entire branch exists only locally) | `git rev-list --count origin/main..HEAD` = 398 |
| Total commits on branch | 400 | `git rev-list --count HEAD` |
| Merge-base with origin/main | 4b4b2e5 2026-07-27 "Add project files." | `git merge-base HEAD origin/main` |
| Tags | v1.0.0 (23adbc0, 2026-08-03), v1.1.0-backup-complete (720dc07, 2026-08-04) | `git tag -l`, `git log -1` |
| V8 snapshot point | PROJECT_PROFILE_V8.md last committed at 91d2a5e (2026-10-23 04:51 "Major audit, docs, and backup/reporting updates"); 2 commits after it: f943ecb, fdeb598 | `git log -1 -- PROJECT_PROFILE_V8.md` |
| Files changed since V8 commit | BackupEngine.vb (+105), BackupLegacyEvaluator.vb (new, 777 lines), BackupRegistry.vb (+39), DatabaseBackupHelper.vb (+426/-), Forms/FrmBackupVerify.vb (+360/-), TempleAccounting.vbproj (+1), frmMain.UiHelpers.vb (+3), frmMain.vb (+157) — 8 files, 1583 insertions, 285 deletions | `git diff --stat 91d2a5e..HEAD` |

Last 30 commit messages: see `git log --format='%h %ad %s' --date=short -30` output captured in survey session; newest 6:
- fdeb598 2026-10-23 Refactor backup status logic into new module
- f943ecb 2026-10-23 Enhance backup system: legacy support, UI, robustness
- 91d2a5e 2026-10-23 Major audit, docs, and backup/reporting updates (V8 doc)
- 9d011b1 2026-10-22 Enhance backup system with registry, async, and UI verify
- 921c119 2026-10-22 Enforce Option Explicit, add docs, clean up scripts/files
- 3cf6363 2026-10-22 Refactor and enhance backup system; code hygiene updates
(then aae3cc3 2026-09-02 … back to d08c64a 2026-08-18; full list in git.)

## Top-level folder & file inventory (VERIFIED by ls/find/git ls-files)
150 tracked files. 384 files on disk excluding .git. Key folders:

| Path | Role | Tracked? | Notes |
|---|---|---|---|
| TempleAccounting.slnx | Solution (SDK-style) | yes | |
| TempleAccounting.vbproj (+ .user) | Main project | yes (.user ignored) | |
| *.vb at root (24 files) | Core modules (AppPaths, Db, backup, helpers) | yes | |
| Forms/ | All forms incl. FrmBackupVerify.vb (NEW, has no .Designer.vb) | yes (.bak ignored) | FrmTempleSetting.vb.bak + Designer.vb.bak present, ignored |
| Services/ | Db providers + transaction repo/service | yes | |
| Reports/ | ReportEngine.vb, IncomeExpenseReport.vb | yes | |
| My Project/ | Application.myapp, PublishProfiles/ | partially | |
| .agents/skills/ | 9 SKILL.md files | yes | |
| .trae/documents/ | 3 plan md files | yes | |
| @Deployment/ | UserManual_TempleAccounting.md (43 KB) | yes | |
| Database/ | 2 .sql scripts + TempleAccounting.accdb + pre_P6 copy (accdb ignored) | sql tracked | |
| Docs/ | 3 md + Specs/ (Prerelease checklist, Thai checklist result) | yes | |
| Import/ | province/amphoe/tambon CSV + Excel template | yes | |
| Export/ | 2 sample CSV exports | yes | |
| Receipts/ | 22 receipt JPGs (Receipt_*.jpg) | yes | |
| Logs/ | crash.log (0 bytes), LastLocationImport.txt | ignored | |
| Backup/, Backup1/, Backups/ | manual/db backups; Backup1 holds a stray .slnx; Backups empty | ignored (Backup*/ rule) | Backup/ has 2 manual .accdb copies |
| bin/, obj/, .vs/ | build artifacts (Release+Debug net10.0-windows) | ignored | bin contains a full deployed app incl. Database/*.accdb and Backup/Weekly, Monthly, Backup_2569* trees |
| BackupTests/ | Separate test project (BackupTests.vbproj, 685 LOC, 2 test files, TestResults/) | ignored (Backup*/ rule!) | net8.0 |
| tools/BackupScanDiag/ | Standalone diag console app (Program.vb 185 LOC, own .vbproj) | ignored | |
| altbin/Release | old build output | ignored | |
| Mahachat/, Models/, Resources/, _quarantine_review/, .cursor/ | empty dirs | n/a | |
| _ProjectDocs_And_Archive/V9_survey/ | THIS survey's output folder | new | |

Root doc files (tracked): PROJECT_PROFILE_V4–V8.md, AUDIT_REPORT.md, FIX_REPORT.md, FIX_BRIEF_EMPTY_CATCH_PHASE1.md, FIX_BRIEF_RESTORE_COMPATIBILITY_1.md, PROJECT_HEALTH_REPORT.md, System_Stability_and_Robustness_Audit.md, USER_MANUAL_TH.md, README.md (0 bytes — empty!), LICENSE.txt, UpgradeLog.htm (ignored), SimpleInstaller.ps1, TempleAccountingSetup.iss, app.manifest, app_icon.ico, lamYangI&E.ico, "Lamyang I&E.png", "Lamyang I&Emain.png".

## Line counts (wc -l, VERIFIED)
- Main project .vb total (excl. bin/obj/tools/BackupTests): **26,077 lines** across 66 files.
- tools/BackupScanDiag: 185; BackupTests: 685 (incl. obj-generated AssemblyInfo).
- 20 largest .vb files:
  1. Forms/FrmMultiMachineImport.vb 1355
  2. Reports/ReportEngine.vb 1141
  3. frmMain.vb 1090
  4. Forms/FrmOnboardingWizard.Designer.vb 1052
  5. Reports/IncomeExpenseReport.vb 990
  6. frmMain.Designer.vb 957
  7. Forms/FrmTransactions.vb 955
  8. Forms/FrmTempleSetting.Designer.vb 873
  9. DatabaseBackupHelper.vb 857
  10. Forms/FrmReports.vb 825
  11. Forms/FrmOnboardingWizard.vb 819
  12. BackupLegacyEvaluator.vb 777 (NEW since V8)
  13. BackupEngine.vb 686
  14. Forms/FrmIncome.vb 666
  15. Forms/FrmExpense.vb 622
  16. Forms/FrmExportForTransfer.vb 612
  17. Forms/FrmReports.Designer.vb 568
  18. Forms/FrmPersonnelManagement.Designer.vb 534
  19. Forms/FrmMasterData.Designer.vb 511
  20. Forms/FrmChartPreview.vb 509

## Possibly dead / unreferenced (grep -rlw, VERIFIED at file level)
- tools/BackupScanDiag/Program.vb — references only itself; standalone tool, not in solution (check T02 for slnx contents).
- Forms .bak files (FrmTempleSetting.vb.bak, FrmTempleSetting.Designer.vb.bak) — leftover backups, ignored by git.
- Backup1/ (stray TempleAccounting.slnx, 71 bytes), altbin/, empty dirs (Mahachat/, Models/, Resources/, _quarantine_review/).
- README.md is empty (0 bytes) — contradicts expectation of a real readme.
- BackupLegacyEvaluator.vb: not referenced by name anywhere except itself — but it likely holds Module-level members used elsewhere; MUST check member-level usage in T16 (file-level grep can't see Module member calls).

## Notes / open items for later tasks
- bin/Release contains a full runtime tree (Database/TempleAccounting.accdb dated 2026-10-23, Backup/Weekly+Monthly, Backup_2569081x_* trees) — evidence that the deployed app writes DB+backups under the EXE folder, not the repo root. T03/T06 to resolve.
- The .gitignore `Backup*/` rule means ALL backup folders AND the BackupTests project are git-ignored (BackupTests source is untracked — risk of losing test code).
