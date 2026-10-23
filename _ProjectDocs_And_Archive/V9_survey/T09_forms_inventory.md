T09 — Forms inventory (UI) and navigation map (V9 survey)
Generated: 2026-10-23. All VERIFIED (scripted read of every Forms/*.vb + frmMain*; control counts = `Friend WithEvents` declarations in Designer files; handlers = `Handles` clauses; tables = name occurrence in form code).

## Catalog (17 forms + frmMain partial class)
| Form (file, LOC) | Base | Designer | Controls | Handlers | AutoScaleMode | F1 help | UiFitter | DB tables touched (direct) |
|---|---|---|---|---|---|---|---|---|
| frmMain.vb (1090) + frmMain.UiHelpers.vb (284) + frmMain.Designer.vb (957) | Form | Y | 53 created in Designer (16 btn declared; NOT WithEvents — wired manually UiHelpers.SetupEventHandlers) | 1 Handles + ~30 AddHandler | **Font** (Designer:919) | Y (frmMain.vb F1 KeyDown→ShowManual) | Y | Transactions, Categories, Funds, BankAccounts, TempleSetting, Personnel, Positions |
| Forms/FrmIncome.vb (666) | Form | Y | 32 | 11 | **unset** | Y | Y | Transactions, Categories, Funds, BankAccounts |
| Forms/FrmExpense.vb (622) | Form | Y | 27 | 10 | **unset** | Y | Y | Transactions, Categories, Funds, BankAccounts |
| Forms/FrmTransactions.vb (955) | Form | Y | 29 | 19 | Font (444) | Y | Y | Transactions, Categories, Funds, BankAccounts |
| Forms/FrmTransfer.vb (206) | Form | Y | 19 | 6 | **unset** | Y | Y | Transactions, Funds, BankAccounts |
| Forms/FrmReports.vb (825) | Form | Y | 31 | 16 | Font (544) | Y | Y | Transactions, Categories, Funds, BankAccounts (+ReportEngine cache) |
| Forms/FrmChartPreview.vb (509) | Form | **N (programmatic)** | 2 in-code | 5 | **Dpi** (in .vb:49) | Y (ShowManual) | - | via ReportEngine only |
| Forms/FrmTempleSetting.vb (395) | Form | Y | 50 | 10 | **Dpi** (838) | Y | Y | TempleSetting, Personnel, Positions, Province, District, SubDistrict |
| Forms/FrmPersonnelManagement.vb (255) | Form | Y | 32 | 9 | Font (505) | Y | Y | Personnel, Positions |
| Forms/FrmMasterData.vb (251) | Form | Y | 18 | 13 | **unset** | Y | Y | Categories, Funds, BankAccounts |
| Forms/FrmLocationImport.vb (249) | Form | Y | 28 | 8 | **unset** | Y | Y | Province, District, SubDistrict |
| Forms/FrmMultiMachineImport.vb (1355) | Form | Y | 30 | 8 | Dpi (454) | Y | Y | Transactions, Categories, Funds, BankAccounts, ImportHistory (+ALTER ImportBatchID/SourceMachineID) |
| Forms/FrmExportForTransfer.vb (612) | Form | Y | 23 | 6 | Dpi (371) | Y | Y | Transactions, Categories, Funds, BankAccounts, ExportHistory, LocalSettings |
| Forms/FrmOnboardingWizard.vb (819) | Form | Y | 76 | 19 | Dpi (1000, comment "ห้าม Font") | **-** | Y | BankAccounts, TempleSetting, Province, District, SubDistrict, Personnel, Positions, Funds, Transactions (via OnboardingRepository) |
| Forms/FrmHelpDialog.vb (108) | Form | Y | 4 | 1 | Font (84) | N/A (is the help) | Y | none (reads USER_MANUAL_TH.md via HelpSystem) |
| Forms/FrmReceiptViewer.vb (102) | Form | Y | 10 | 5 | Font (180) | **-** | - | none (image only) |
| Forms/FrmBackupVerify.vb (439) | Form | **N (programmatic)** | 9 in-code | 0 Handles (manual wiring) | **unset** | **-** | - | none directly (via DatabaseBackupHelper/registry) |
| Programmatic (no Designer) set = FrmChartPreview, FrmBackupVerify (NEW) | | | | | | | | |

**AutoScaleMode verdict**: V8 "still mixed" → **VERIFIED**: Dpi ×5, Font ×6, **unset ×6** (FrmIncome, FrmExpense, FrmTransfer, FrmMasterData, FrmLocationImport, FrmBackupVerify — no assignment anywhere in source; effective runtime default UNVERIFIED). Note the intentional comment in FrmOnboardingWizard.Designer.vb:995: "AutoScaleMode = Dpi (ห้าม Font — จะทำให้ขนาดเพี้ยน)".

**F1 contextual help (HelpSystem.SetupHelp)**: present in 12 forms + frmMain (list = grep): Expense, ExportForTransfer, Income, LocationImport, MasterData, MultiMachineImport, PersonnelManagement, Reports, TempleSetting, Transactions, Transfer, frmMain. **Missing: FrmReceiptViewer, FrmOnboardingWizard, FrmBackupVerify** (ChartPreview uses ShowManual directly).

**UiFitter** (AutoFitFormButtons/DisableComboBoxWheel): used by all forms except FrmChartPreview, FrmReceiptViewer, FrmBackupVerify (programmatic/new forms) — verified per-file grep.

## Navigation map (VERIFIED)
Panel-hosted (via `frmMain.ShowFormInPanel(child, title)`, frmMain.vb:406-433: TopLevel=False, FormBorderStyle.None, Dock=Fill, hides logo/overview; `CloseActiveForm` l.435-449 = Remove→Close→Dispose):
| Trigger (frmMain.vb Select Case NavMenu_Click l.228-315) | Opens |
|---|---|
| btnDashboard | ShowDashboard |
| btnDonation | **panel** FrmIncome |
| btnExpense | **panel** FrmExpense |
| btnReport | **panel** FrmReports |
| btnMember | **panel** FrmTransactions (compact overview) |
| btnMultiImport | **MODAL** FrmMultiMachineImport (comment: ห้ามผ่าน ShowFormInPanel — กันฟอร์มถูก Dispose กลางคัน l.272-277) |
| btnExportTransfer | **MODAL** FrmExportForTransfer (same rationale l.279-284) |
| btnVip | **panel** FrmTempleSetting |
| btnActivity | **panel** FrmTransfer |
| btnSetting | **panel** FrmMasterData |
| btnBackup / btnRestore / btnVerifyBackup | async backup / sync restore / **MODAL** FrmBackupVerify (l.1056) |
| btnClose / btnMinimize / btnLogout | Application.Exit confirm / minimize / demo logout msg (l.1006-1028) |
Child cross-links:
- FrmTransactions → panel FrmIncome/FrmExpense/FrmTransfer (l.497-511), edit-doubles (l.538-542), **modal** FrmReceiptViewer on receipt cell (l.809).
- FrmTempleSetting → **modal** FrmPersonnelManagement (l.370), FrmLocationImport — panel if hosted by frmMain else modal (l.385-387).
- FrmLocationImport → panel FrmTempleSetting on done (l.243).
- FrmReports → **modal** FrmChartPreview (l.368, Using).
- HelpSystem.SetupHelp (F1) → **modal** FrmHelpDialog (HelpSystem.vb:93).
- Program (onboarding gate) → **modal** FrmOnboardingWizard (Program.vb:157); dev sidebar button → modal wizard test mode (frmMain.UiHelpers.vb:76).

## Sidebar menu — every item (frmMain.Designer.vb declarations l.19-61 + UiHelpers wiring l.105-126)
16 buttons declared (Friend/Private, wired by AddHandler in SetupEventHandlers):
`btnDashboard, btnDonation, btnExpense, btnReport, btnMember, btnSetting` (nav top), `btnBackup, btnRestore, btnVerifyBackup` (data), `btnActivity, btnExportTransfer, btnMultiImport, btnVip` (nav lower), `btnLogout, btnClose, btnMinimize` (footer).
Plus status bar `lblStatusCenter` (dbl-click opens DB folder, right-click copies path, frmMain.vb:130-157), `picLogo` (triple-click developer credit easter egg l.110-128), `picLogoBadge`, metric cards pnlCard1-4 (click → nav, OverviewCard_Click l.317-330).
**Dev-only buttons** (created at runtime in `SetupDeveloperTools`, frmMain.UiHelpers.vb:14-65): "🧪 ทดลอง Onboarding" (btnTestWizard → wizard dry-run) and "🔄 รีเซ็ต Onboarding" (btnResetOnboarding → `OnboardingRepository.ResetOnboardingStatus()`), added into `pnlDev` panel docked Top in sidebar. **There is NO `#If DEBUG` guard anywhere in UiHelpers** — both dev buttons are VISIBLE AND FUNCTIONAL in Release builds (VERIFIED). V8 called these "dev tools" without noting Release visibility.
Other frmMain runtime features: 1-second status clock (ApplyInitialState, UiHelpers l.244-252), tool-tips for all buttons (SetupToolTips l.256-284), secret Ctrl+Shift+D credit (frmMain.vb:104-108), desktop shortcut auto-create on Load (l.76).

## Notable per-form external calls
- frmMain: Process.Start(explorer) status center (l.133), Clipboard.SetText (l.144), DesktopShortcutHelper (l.76), shell32 AppUserModelID (Program).
- FrmIncome/FrmExpense: OpenFileDialog w/ hard-coded `C:\LineDownloads` initial dir (FrmIncome.vb:617 — a hard-coded machine path in SOURCE; exception to the "no absolute paths" claim in T02 — path is guarded by Directory.Exists but still literal).
- FrmReports/FrmExportForTransfer/FrmMultiMachineImport: ClosedXML/OleDb Excel IO + SaveFileDialog; ReportEngine print/PDF.
- FrmReceiptViewer: loads image file only.
- FrmReceiptViewer/FrmTransactions: receipt path resolution (T10).
