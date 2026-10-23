import pyodbc, os, io, sys, glob
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
cn = pyodbc.connect(r'DRIVER={Microsoft Access Driver (*.mdb, *.accdb)};DBQ=C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\_ProjectDocs_And_Archive\V9_survey\tmp\dbcopy.accdb;ReadOnly=True;', autocommit=True)
cur = cn.cursor()
cur.execute("SELECT ID, ReceiptPath FROM Transactions WHERE ReceiptPath IS NOT NULL AND ReceiptPath <> '' ORDER BY ID")
rows = [(r[0], r[1]) for r in cur.fetchall()]
print("DB rows with non-empty ReceiptPath:", len(rows))
for tid, rp in rows:
    print("  ID=%s ReceiptPath=%r" % (tid, rp))

disk = sorted(os.path.basename(p) for p in glob.glob(r'Receipts\*.jpg'))
print()
print("Disk Receipts/*.jpg:", len(disk))
print(disk)

db_names = set(os.path.basename(rp) for _, rp in rows)
disk_set = set(disk)

print()
print("== DB row WITHOUT file (missing file) ==")
missing = 0
for tid, rp in rows:
    if os.path.basename(rp) not in disk_set:
        missing += 1
        print("  ID=%s -> %r" % (tid, rp))
print("missing count:", missing)

print("== File WITHOUT DB row (orphan file) ==")
orphans = sorted(disk_set - db_names)
for f in orphans:
    print("  ", f)
print("orphan count:", len(orphans))

# also: transactions with image-type Note? no. Count of rows by has receipt
cur.execute("SELECT COUNT(*) FROM Transactions")
total = cur.fetchone()[0]
print("total transactions:", total)
