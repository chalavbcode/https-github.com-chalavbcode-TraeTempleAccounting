# T18 — Agent Skills, Docs, Process Files (survey notes)

Evidence: HEAD = `0c274d5`, branch `modify_backupoct`.

## 1. Agent skill system — `.agents/skills/` (9 skills, all `SKILL.md`)

| Skill | Last touched | Purpose (from V8 table + files) |
|---|---|---|
| `access-date-safe` | Aug 20 | Thai BE vs Gregorian, `AccessDateLiteral`/`NormalizeGregorianDate` invariants, date-range query patterns |
| `childform-mdi-nav` | Aug 20 | Panel-hosted child-form lifecycle, frmMain↔child communication |
| `datagridview-column-setup` | Aug 20 | Grid column consistency (prevents grid-tweak churn) |
| `db-connection-safety` | Aug 20 | OleDb lifecycle, parameterized SQL, tx rules (no bare DELETE+INSERT) |
| `form-duplicate-code` | Aug 20 | FrmIncome/FrmExpense ~80% shared — sync discipline |
| `receipt-clipboard-workflow` | Aug 20 | Clipboard/browse → optimize → Receipts lifecycle |
| `report-rendering-layout` | Aug 20 | GDI+ signature block, page balancing, margin constants |
| `vb-audit-checker` | Aug 20 | VB/WinForms/Access bug-audit patterns |
| `winforms-ui-align` | Aug 24 | Coordinates/alignment/visual standards |

- Frontmatter is **minimal** (`name` + `description: <name> skill` — generic descriptions, Thai bodies with "ปัญหาที่เคยเจอ" known-issue sections).
- **No root `AGENTS.md` / `CLAUDE.md`**, no `.claude/`, no `.github/` (→ **no CI**), no skills beyond these 9; all last written during the V8 era (Aug 20-24) — untouched since.
- V8 profile documents these in its skills table (line ~231) — V9 should restate + note staleness (e.g., `form-duplicate-code` needs the new T10 ~79% metric; `access-date-safe` should absorb the log/filename culture findings).

## 2. Root documentation set (12 files, ~440 KB)

| File | Size | Role |
|---|---|---|
| `PROJECT_PROFILE_V4.md` … `V8.md` | 90/112/30/22/25 KB | Successive full-system profiles (**V8 = immediate predecessor of this deliverable**) |
| `AUDIT_REPORT.md` | 38 KB | Bug audit (source of B8/A6/A1 fix wave) |
| `System_Stability_and_Robustness_Audit.md` | 27 KB | 2026-08-09 stability audit |
| `PROJECT_HEALTH_REPORT.md` | 46 KB | Health report (source of empty-catch Phase 1 brief) |
| `FIX_REPORT.md` | 58 KB | Fix log ("แก้ 3 บั๊กอันดับแรก" + later waves), each with build re-verification |
| `FIX_BRIEF_EMPTY_CATCH_PHASE1.md`, `FIX_BRIEF_RESTORE_COMPATIBILITY_1.md` | 13/10 KB | Scoped agent work orders |
| `USER_MANUAL_TH.md` | 22 KB | Thai user manual |
| `README.md` | **46 bytes** | single line: project export name — effectively **empty** |

- `Docs/`: `Fix_Priority_Instructions_for_Agent.md` ("คำสั่งสำหรับ AI Agent: แผนแก้ไขตามลำดับความเสี่ยง" — risk-ordered fix plan, "ปิดจบในตัวเอง" no-new-debt rule), `Temple_Accounting_Prerelease_Checklist.md` + its results file (Thai), `Specs/` (**empty**).

## 3. Documented process (the agent-driven fix loop)

Observed pattern across docs: **Audit → priority plan → FIX_BRIEF (scope + risk order) → implement → `dotnet build -c Release` re-verified 0W/0E → FIX_REPORT** with a manual DB backup noted first (`Backup\TempleAccounting_before_*.accdb`). Briefs reference absolute original path `C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422` (= README export name; repo folder renamed since).

## 4. Installer artifacts

- `TempleAccountingSetup.iss` (Inno Setup, 1.9 KB) + `SimpleInstaller.ps1` (1 KB), both Aug 17 — present at root; V8 lists them as the distribution path (v1.0.0).

## 5. Survey workspace (this V9 effort)

- `_ProjectDocs_And_Archive/V9_survey/` — only content of the otherwise-empty `_ProjectDocs_And_Archive/` (**untracked** in git): `T01…T17` notes, `PROJECT_PROFILE_V9.md` (final deliverable), `tmp/` (DB copy, reconcile script, registry dump, OleDb tx test).
- Git state at survey end: branch `modify_backupoct`; clean except the untracked `_ProjectDocs_And_Archive/`.

## 6. Issues / recommendations (T18)

1. **P3 — README is 46 bytes**: no build/run/dependency instructions at repo root (ACE 64-bit, .NET 10, printer feature are only in PROJECT_PROFILE/build docs).
2. **P3 — skills stale since Aug 24**: descriptions generic; content predates backup rewrite (V9 T06), TransferFormat, ErrorLogHelper, and the T01-T17 findings.
3. **P3 — no CI**: build/test verification is manual (`dotnet build -c Release`, `dotnet test BackupTests`); the 26 passing tests never run automatically.
4. **Info — doc sprawl at root**: 12 overlapping markdown files across audits/profiles/fixes; `Docs/Specs/` empty; consider archiving V4-V7 profiles + old audits into `_ProjectDocs_And_Archive/` (V9 profile + V8 remain current).
5. **Info — process is otherwise solid**: brief→fix→re-verify-build→report loop with pre-fix DB backups is documented and followed (FIX_REPORT re-states 0W/0E per wave).
