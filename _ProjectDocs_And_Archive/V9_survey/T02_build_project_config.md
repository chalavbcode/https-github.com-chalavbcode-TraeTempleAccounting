T02 — Build & project configuration (V9 survey)
Generated: 2026-10-23.

## Project file (TempleAccounting.vbproj) — VERIFIED by reading
| Property | Value | Evidence (TempleAccounting.vbproj) |
|---|---|---|
| SDK | Microsoft.NET.Sdk | line 1 |
| OutputType | WinExe | line 4 |
| TargetFramework | net10.0-windows | line 5 |
| Nullable | disable | line 6 |
| UseWindowsForms | true | line 7 |
| StartupObject | TempleAccounting.Program | line 8 |
| RootNamespace / AssemblyName | TempleAccounting / `lamYangI&E` | lines 9-10 |
| OptionExplicit / OptionStrict / OptionCompare / OptionInfer | On / **Off** / Binary / On | lines 16-19 |
| EnableDefaultCompileItems / EmbeddedResource / Page | false / false / false (explicit items only) | lines 20-22 |
| NoWarn | WFO1000 (WinForms preview API warning suppressed) | line 23 |
| Copyright | © 2026 พระบุญส่ง โสภณจิตฺโต | line 12 |
| ApplicationManifest | app.manifest (asInvoker, Win10 compat GUID) | app.manifest lines 5, 11 |

NuGet packages (vbproj PackageReference block):
- Microsoft.Data.SqlClient 6.0.2
- Microsoft.Data.Sqlite 10.0.11
- System.Data.OleDb 10.0.10
- WinForms.DataVisualization 1.10.2
- ClosedXML 0.104.2

Content items copied to output: `Database\**\*`, `Import\**\*`, `USER_MANUAL_TH.md`, `Lamyang I&Emain.png`, `lamYangI&E.ico` (all PreserveNewest).

## Solution (TempleAccounting.slnx) — VERIFIED
Contains ONLY `TempleAccounting.vbproj` (1 project). **BackupTests/BackupTests.vbproj and tools/BackupScanDiag/BackupScanDiag.vbproj are NOT in the solution** — they are orphans built separately (BackupTests targets net8.0 per its obj folder; BackupScanDiag targets net10.0).

## Disk-vs-project completeness check (VERIFIED by comm of sorted lists)
- .vb files in main scope (excl. bin/obj/tools/BackupTests): 64 on disk, 64 `<Compile>` items → **0 missing, 0 extra**. New files BackupConfig.vb, BackupEngine.vb, BackupRegistry.vb, BackupLegacyEvaluator.vb, Forms/FrmBackupVerify.vb ARE all included.
- .resx: 14 on disk, 14 `<EmbeddedResource>` items → 0 missing, 0 extra.
- Notes: FrmBackupVerify.vb has NO Designer.vb file at all (built programmatically — verify in T09); FrmChartPreview.vb likewise has no Designer.vb on disk.
- Untracked leftovers on disk not in project: Forms/FrmTempleSetting.vb.bak, Forms/FrmTempleSetting.Designer.vb.bak.

## Build result (RUNTIME — executed this session, `dotnet build -c Release`)
- SDK: 10.0.401 (`dotnet --version`).
- Result: **Build FAILED (exit 1)** — but NOT from code:
  - Compiler stage: **0 `warning BC*`, 0 `error BC*`** (grep count = 0) → VB compilation itself is clean, consistent with V8's 0/0 claim.
  - Failure is purely the file-copy stage: 10× `warning MSB3026` + `error MSB3027`/`error MSB3021` — "Could not copy obj\...\apphost.exe to bin\Release\net10.0-windows\lamYangI&E.exe … The file is locked by: 'lamYangI&E (11140)'".
  - Cause: **the application is currently RUNNING** on this machine (live process lamYangI&E, PID 11140, from the project output folder). RUNTIME observation; the app was deliberately NOT terminated (survey is read-only, killing the user's running bookkeeping app would be destructive).
- VERDICT vs V8 claim "Build succeeded — 0 Warning(s), 0 Error(s)": PARTIALLY CONTRADICTED as an environment statement — with the app running, the build fails at copy time (10 warnings/2 errors). The compile itself remains 0/0. A clean 0/0 build requires closing the app first (not done here to avoid disturbing the user).

## Project location on this machine (from build paths)
`C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\` — the repo root; the running exe lives in `<root>\bin\Release\net10.0-windows\`.

## Hard-coded machine paths in source (VERIFIED by code_search)
- Search for `C:\`, `D:\`, `C:/`, drive-letter strings in all .vb: **0 matches** — no absolute drive paths hard-coded in source.
- Path resolution goes through `Environment.GetFolderPath` (DesktopDirectory: DesktopShortcutHelper.vb:33,54; ApplicationData fallback: AppPaths.vb:174). Full path model documented in T03.
- The Thai-named external backup root (D:\สำรองข้อมูลบัญชี) does NOT appear in source — it is chosen at runtime via FolderBrowserDialog (frmMain.vb:899) and persisted in user_preferences.ini (T03/T06 to confirm).
- Machine-specific artifacts (not in code): project root under C:\Users\fantasy\Documents; running instance locks build output.

## Runtime dependencies (from vbproj + V8, partially verified)
- .NET 10 Windows Desktop Runtime (net10.0-windows).
- Microsoft ACE OLEDB 12.0 provider (external install — UNVERIFIED here whether present on this machine; required at runtime for .accdb).
- "Microsoft Print to PDF" virtual printer (used by report printing — V8 claim, printing path to verify in T11).
- NuGet runtime assemblies: System.Data.OleDb, Microsoft.Data.Sqlite (native e_sqlite3.dll present in bin/runtimes), Microsoft.Data.SqlClient, WinForms.DataVisualization, ClosedXML (bin/Release/DocumentFormat.OpenXml.dll present).
