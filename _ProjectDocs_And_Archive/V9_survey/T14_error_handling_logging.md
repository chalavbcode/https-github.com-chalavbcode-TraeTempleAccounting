# T14 — Error Handling, Logging, Diagnostics (survey notes)

Evidence: HEAD = `0c274d5` + on-disk logs. Line refs are HEAD.

## 1. Global exception architecture (`Program.vb`)

- **Handlers installed first thing in `Main()`** (59-91), each wrapped in its own Try:
  - `AppDomain.CurrentDomain.UnhandledException` → `BootCrash` + fatal Thai MessageBox with crash-log path and "send this file to the developer".
  - `Application.ThreadException` → `BootCrash` + warning MessageBox; **app keeps running** (default `UseCatchException` mode — no `SetUnhandledExceptionMode` call).
  - Non-Exception crash objects logged as-is.
- **Staged boot** with per-stage `BootCrash(ex, "Step N - …")`: WinForms init (visual styles, `HighDpiMode.PerMonitorV2`), AppUserModelID (`LamyangIE.TempleAccounting.App.v1`), AppPaths (DB-missing check logs `!! DATABASE FILE NOT FOUND`), `Db.EnsureSchema` (failure shows the **ACE 2016 32/64-bit mismatch guidance + download link**, then continues to main form anyway), `SaddhiviharikaDb.EnsureSchema`, `MigrateReceiptsToMaster`, onboarding wizard gate, `frmMain` constructor (ExitCode=1 + return), `Application.Run` (ExitCode=1 + return).
- Boot is defensive: every step Try/Catch'd with double-wrapping in the critical steps (inner failure → BootCrash + user dialog; outer catch → BootCrash) — **no exception path skips logging**.

## 2. Logging (two files, both in `AppPaths.LogsFolder`)

| File | Writers | Contents |
|---|---|---|
| `crash.log` | `Program.BootLog` (startup `[BOOT]` lines), `Program.BootCrash`, `AppPaths.LogCrash` (55 call sites) | boot trace, unhandled UI/appdomain exceptions with location, type, message, inner, stack |
| `error.log` | `ErrorLogHelper.LogSilentError` (~20 sites), `ErrorLogHelper.LogInfo` | deliberately-caught silent errors with context string; operational INFO (backup scan) |

- `AppPaths.LogCrash` (43-60): append-only UTF-8, includes 📍 location + stack; outer silent catch (log failure never crashes the app — documented policy in ErrorLogHelper: "ยอมให้เงียบเฉพาะจุดนี้จุดเดียว").
- **No rotation, no size cap, no cleanup** — `bin/Release/…/Logs/crash.log` is 75 KB and growing since install.
- **Culture bug (same family as T05):** all three writers use `DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")` without InvariantCulture → **log timestamps are Buddhist-era**. On-disk evidence: `crash.log` lines read `2569-10-23 06:42:53` and `error.log` lines read `[2569-10-23 04:55:45]` (th-TH `yyyy` → 2569). Correlating logs with file mtimes (Gregorian) requires +543 mental math.
- Four log copies exist in repo/build dirs: root `Logs/crash.log`, `bin/Debug/…/Logs/crash.log`, `bin/Release/…/Logs/{crash,error}.log`.

## 3. Catch inventory (whole `.vb` tree)

- **255 `Catch` keywords** total.
- **~54 catches immediately followed by `End Try`** (empty — no logging, no rethrow).
- 80 single-line catches (`Catch` + statement on next line pattern).
- Logging discipline is partial and **asymmetric**: 55 `LogCrash` + ~20 `LogSilentError` + `LogInfo` cover key paths (backup engine, preferences, wizard loads, dashboard nav, backup verify), but empty catches remain in UI/flow code (e.g., `ShowPreviewWithPdfExport` toolbar injection, `Handles` paths, `Program` handlers themselves).
- `ErrorLogHelper`'s own doc comment states its purpose is to *replace* empty Catch blocks — the migration is incomplete (~31 log call sites vs ~54 empty catches).

## 4. Diagnostics & user-facing error UI

- `Debug.WriteLine` ×49, `Console.WriteLine` ×42 (the latter is a no-op in a GUI Release build — WinExe has no console; includes `Program.C()` boot trace which only reaches console when launched from a terminal).
- `MessageBoxHelper.vb`: custom large-messagebox family (`Show`/`ShowInfo`/`ShowError`/…) rendered with Tahoma 8 — used widely alongside **raw `MessageBox.Show` calls** (two parallel dialog styles coexist; raw calls pass raw `ex.Message` to users in many forms — see T17).
- Boot messages include actionable Thai guidance (ACE download link, PDF-printer enable path in T11, DB-missing warning).

## 5. On-disk log evidence (current builds)

- `error.log` (Release): records the 04:55/05:39 backup scans from T06 — `ScanDrivesForBackups: สแกน 3 ไดรฟ์ พบ 5 ชุดสำรอง`, then **`AddScannedBackupToRegistry: ตรวจสอบไม่ผ่าน ไม่เพิ่มเข้าสู่ registry`** for all 5 discovered sets (4 old-format `Backup_2569*/Backup_2026*` folders lack the new manifest), and `ScanAndRegisterBackups: พบ=5 เพิ่ม=5 ข้าม=0` at 05:39 — corroborates T06's registry/manifest findings.
- `crash.log` (Release): pure `[BOOT]` lines, no unhandled exceptions recorded in recent sessions (AppUserModelID + SaddhiviharikaDb init per run) — the app's boot path is stable; no crash-loop evidence.

## 6. Issues / debt found (T14)

1. **P2 — Buddhist-year log timestamps** (culture-sensitive `ToString` in all three log writers) — breaks time correlation with file names/mtimes; same root cause family as T05.
2. **P2 — No log rotation/retention**: `crash.log`/`error.log` grow forever (append-only); privacy consideration too (T17): stack traces + user names in a world-readable app folder.
3. **P3 — Empty-catch residue (~54)**: documented policy says ErrorLogHelper should replace them; incomplete migration means some silent failures still leave zero trace (e.g., PDF toolbar injection, dialog helpers).
4. **P3 — Two parallel dialog systems** (MessageBoxHelper + raw MessageBox) and two log files with overlapping purpose (`crash.log` vs `error.log`) — discoverability cost for support ("send us the log": which one?).
5. **Info — boot resilience is good**: staged Try/Double-Catch + ExitCode discipline + Thai actionable guidance; recent boots clean.

## 7. Verified-good design points

- Handlers registered before anything else can fail; every boot step logs via BootCrash; fatal vs recoverable dialogs clearly distinguished (⛔/⚠️/❌).
- `EnsureSchema` failure doesn't brick startup (warning + continue), with the most common real cause (ACE bitness) called out explicitly.
- Log-write failures never propagate (single documented silent point).
