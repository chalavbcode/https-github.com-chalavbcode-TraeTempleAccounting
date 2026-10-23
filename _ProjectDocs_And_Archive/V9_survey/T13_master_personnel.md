# T13 — Master Data & Personnel (survey notes)

Evidence: HEAD = `0c274d5`; empirical tests on disposable DB copy (`tmp/dbcopy.accdb`). Line refs are HEAD.

## 1. Where master data lives

**Single form: `Forms/FrmMasterData.vb`** (246 code lines; Designer = 1043 — the largest designer file in the project). Tabbed (`tpCategory` / fund / bank tabs) with per-tab grid + add/edit/delete. There is NO per-entity form (V8-era `FrmCategory/FrmFund/FrmBankAccount` names do not exist at HEAD).

- Loads all three grids in `LoadAll()` (55-59): Categories ordered by TranType, Funds by name, Banks by name.
- Enter-key navigation wired per tab (`SetupEnterNavigation`/`HandleEnter`, 81-105).

### Category (107-181)
- Duplicate guard `CategoryExists` (name+type, exclude on edit) on **both Add and Edit** — parameterized, Thai warning.
- Edit supported (name + type switch).
- Delete: Yes/No confirm → `DELETE FROM Categories WHERE ID=@id`; generic catch shows **"ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่"** ("cannot delete — in use").

### Fund (183-210) / Bank (212-240)
- **No duplicate-name guard** (unlike Category) — same-name funds/banks can be created freely.
- **No edit buttons** — add/delete only (a mis-typed fund must be deleted, not corrected).
- Same delete + generic catch message.

## 2. Referential integrity — empirically proven ABSENT (key finding)

- **Zero FK constraints** in the accounting schema (`REFERENCES` appears only in `SaddhiviharikaDb.vb` — the separate monk/temple registry module). Confirmed on the live-DB copy with a disposable test copy (file removed afterwards):
  - `INSERT INTO Transactions … CategoryID=99999, FundID=99999` → **succeeded** (bogus FK accepted).
  - `DELETE FROM Categories WHERE ID=3` (ID actively referenced by real transactions) → **succeeded**.
- Consequence: the catch-block message "ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่" **can never fire for reference reasons** — it is misleading copy; in-use deletes go through silently.
- **Mitigating design (verified):** every consumer reads master data with `LEFT JOIN` + NULL fallbacks, so orphans degrade instead of vanishing:
  - Reports: `LEFT JOIN Categories … IIF(c.CategoryName IS NULL,'ไม่ระบุ',…)` (ReportEngine summary + detail, 464/476).
  - Grids: `FrmTransactions.vb:199-203` and FrmIncome/FrmExpense/export all `LEFT JOIN` with `IIF(IS NULL,'',…)` → **blank label, row preserved**.
- Residual risk: deleted category labels are silently lost (reports show 'ไม่ระบุ'); re-editing such a transaction in FrmIncome/Expense loads a non-existent `CategoryID` into the combo → a save then writes whatever is currently selected (silent category reassignment).

## 3. Personnel & Positions — `Forms/FrmPersonnelManagement.vb`

Two tabs (`tcMain`: Personnel / Positions); grids loaded by `LoadPersonnelData`/`LoadPositionData`; editor selection via `SelectionChanged`.

- **SavePersonnel** (148-185): validates **FirstName only**; `FullName` composed as `Title+First + LastName` at save; stores `PersonType` from combo text, optional `PositionID`, `Phone`. **No duplicate-name check**, no phone validation. Add/Edit both supported, parameterized SQL.
- **SavePosition** (188-210): name-only validation, add/edit, no duplicate check.
- **DeletePersonnel** (221-234): Yes/No confirm → plain `DELETE FROM Personnel`. **No guard** against being linked in `TempleSetting` (Abbot/Waiyawat/Bookkeeper) or `Positions`.
- **DeletePosition** (236-249): same — no guard for `Personnel.PositionID` references.
- Personnel grid joins `LEFT JOIN Positions` (`FrmPersonnelManagement.vb` join list).

### TempleSetting linkage (no FK, graceful read)
`TempleSetting` columns (live DB): `AbbotPersonnelID / WaiyawatPersonnelID / BookkeeperPersonnelID` INTEGER — **no constraints** (same proven absence of FKs).
- Signature-name resolution `ReportEngine.LoadTemplateInfoFromDatabase` (518-591) uses **triple `LEFT JOIN` Personnel** with `IsDBNull → ""`, waiyawat-first fallback to bookkeeper for the accountant name → deleting a linked person leaves a **blank signature line, not a crash**.
- Temple name/address defaults are hard-coded fallbacks in the same method: temple "วัดแหลมยาง", address "ต.ป่ามะคาบ อ.เมืองพิจิตร จ.พิจิตร" (518-521) — a real place name appears if the DB is empty.
- `FrmTempleSetting` role combos/dropdowns join Personnel (inner style, for combo fill only).

## 4. Seeds (from `AccessDbProvider`, EnsureSchema — see T04)

- Categories (name+type pairs), Funds (names), BankAccounts (name/account), `SeedPositions` (name + `PositionGroup` 'พระ'/'ฆราวาส' — drives wizard Step 2 combos), Province/District/SubDistrict location tables. All idempotent (existence-checked before insert).

## 5. Issues / debt found (T13)

1. **P1 — No referential integrity anywhere** (proven): deleting in-use Categories/Funds/Banks succeeds; UI message falsely claims protection. Labels lost silently ('ไม่ระบุ'/blank). No ON-CASCADE, no guard-count check before delete. *Recommendation: pre-delete `COUNT(*)` guard in FrmMasterData + Personnel (linked in TempleSetting / Positions in use).*
2. **P2 — Asymmetric validation:** duplicate-name check only for Categories; Fund/Bank allow dupes; no edit for Fund/Bank at all.
3. **P2 — Personnel delete has no link guard:** dangling `TempleSetting` role IDs (reads degrade to blank names via LEFT JOIN, so no crash — but signature block silently loses a name).
4. **P3 — Hard-coded fallback temple identity** ("วัดแหลมยาง" + full address) in ReportEngine can print a *wrong* temple on reports if TempleSetting is empty/deleted (rather than an obviously blank header).
5. **P3 — No duplicate-personnel check**; FullName recomputed only at save (renaming FirstName later doesn't touch other tables — acceptable since names, not IDs, are stored elsewhere… but TempleSetting keeps IDs).
