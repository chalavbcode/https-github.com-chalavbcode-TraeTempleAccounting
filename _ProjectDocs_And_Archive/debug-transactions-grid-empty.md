[OPEN]

# Debug Session: transactions-grid-empty

## Symptom
- Run via Visual Studio (F5) → FrmTransactions grid shows headers only, no rows.

## Expected
- Should list all income/expense/transfer rows from `Database\TempleAccounting.accdb`.

## Environment
- App: TempleAccounting (VB.NET WinForms, net10.0-windows)
- DB: Microsoft Access (.accdb)

## Hypotheses
- A: App is opening the wrong database file (different `.accdb` path than expected).
- B: Query date filtering returns zero rows due to BE/AD year conversion mismatch.
- C: Query returns rows but DataGridView binding fails (dt empty vs binding issue).
- D: Exception happens during LoadData/OpenConn but is swallowed/logged elsewhere, leaving grid empty.

## Plan
1. Start debug server and collect runtime logs (pre-fix run).
2. Instrument key points: DB path, file exists, record count, min/max date, query params, returned rows, exception.
3. Analyze evidence and implement minimal fix.

