Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb



Namespace TempleAccounting
    <DesignerCategory("Form")>
    Partial Public Class FrmIncome
        ' ตัวแปรเก็บ Path รูปภาพต้นทางที่ผู้ใช้เลือก (เช่น จาก C:\LineDownloads)
        Private selectedSourceReceiptPath As String = ""
        Private isImageFromClipboard As Boolean = False
        Private clipboardImage As Image = Nothing
        Private _editId As Integer = 0

        Private ReadOnly _txnService As TransactionService = New TransactionService(New AccessTransactionRepository())

        Private ReadOnly _enterFlow As New List(Of Control)()

        Public Sub New()
            InitializeComponent()
        End Sub

        ''' <summary>
        ''' รหัสรายการที่ต้องการแก้ไข (ถ้าเป็น 0 หมายถึงเพิ่มใหม่)
        ''' </summary>
        Public Property EditID As Integer
            Get
                Return _editId
            End Get
            Set(value As Integer)
                _editId = value
                If _editId > 0 Then
                    LoadTransactionData(_editId)
                    ' ปรับเปลี่ยน UI สำหรับโหมดแก้ไข
                    btnCancel.Text = "🔙 ย้อนกลับ"
                    btnCancel.BackColor = Color.FromArgb(75, 85, 99) ' สีเทาเข้ม
                    btnImportExcel.Visible = False
                    ttMain.SetToolTip(btnCancel, "ยกเลิกการแก้ไขและย้อนกลับไปหน้าก่อนหน้า")
                Else
                    ' โหมดเพิ่มใหม่
                    btnCancel.Text = "❌ ล้างข้อมูล"
                    btnCancel.BackColor = Color.FromArgb(180, 83, 9) ' สีส้มอิฐเดิม
                    btnImportExcel.Visible = True
                    ttMain.SetToolTip(btnCancel, "ล้างข้อมูลที่กรอกไว้ทั้งหมดเพื่อเริ่มกรอกใหม่")
                End If
            End Set
        End Property

        Public Sub LoadTransactionData(id As Integer)
            Try
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, "SELECT * FROM Transactions WHERE ID = " & id)
                    If dt.Rows.Count > 0 Then
                        Dim dr = dt.Rows(0)
                        dtpDate.Value = Db.NormalizeGregorianDate(Convert.ToDateTime(dr("TranDate")))
                        cboCategory.SelectedValue = dr("CategoryID")
                        cboFund.SelectedValue = dr("FundID")
                        cboBank.SelectedValue = If(dr("BankID") Is DBNull.Value, DBNull.Value, dr("BankID"))
                        txtDescription.Text = Convert.ToString(dr("Detail"))
                        txtAmount.Text = Convert.ToDecimal(dr("Amount")).ToString("N2")
                        txtRemark.Text = Convert.ToString(dr("Note"))
                        txtReceipt.Text = Convert.ToString(dr("ReceiptPath"))
                        
                        ' Populate Income Channel fields
                        Dim channel = Convert.ToString(dr("ReceiveChannel"))
                        SetSelectedChannel(channel)
                        If channel = "PromptPay" OrElse channel = "eDonation" Then
                            txtReferenceNo.Text = Convert.ToString(dr("ReferenceNo"))
                        End If

                        btnSave.Text = "💾 บันทึกการแก้ไข"
                        btnImportExcel.Visible = False
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถโหลดข้อมูลรายการได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub FrmIncome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmIncome")
            Db.EnsureSchema()
            LoadMasters()
            LoadRecentTransactions()
            CalculateMonthlySummary()
            SetupToolTips()
            SetupEnterNavigation()
            UiFitter.AutoFitFormButtons(Me)
            ApplyFixedVerticalLayout()
            UiFitter.DisableComboBoxWheel(Me)
            ResetEntry(True)
            FocusStartField()
        End Sub

        Private Sub ApplyFixedVerticalLayout()
            ' บังคับ Anchor = Top, Left เท่านั้น (ห้ามใช้ Bottom เพื่อกันการยืด/เว้นช่องอัตโนมัติ)
            lbl7.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            txtRemark.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            lbl8.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            txtReceipt.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnBrowseReceipt.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnPasteReceipt.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnClearReceipt.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnSave.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnCancel.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnImportExcel.Anchor = AnchorStyles.Top Or AnchorStyles.Left

            ' ความสูงช่องหมายเหตุคงที่
            txtRemark.Height = 55

            ' จัดตำแหน่ง Y แบบเด็ดขาด 8px ตามสเปค
            lbl7.Top = txtAmount.Bottom + 8
            txtRemark.Top = lbl7.Top

            lbl8.Top = txtRemark.Bottom + 8
            txtReceipt.Top = lbl8.Top

            btnBrowseReceipt.Top = txtReceipt.Top
            btnPasteReceipt.Top = txtReceipt.Top
            btnClearReceipt.Top = txtReceipt.Top

            btnSave.Top = txtReceipt.Bottom + 12
            btnCancel.Top = btnSave.Top
            btnImportExcel.Top = btnSave.Top
        End Sub

        Private Sub FrmIncome_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmIncome", Me)
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSave, "บันทึกข้อมูลรายรับที่กรอกลงในฐานข้อมูล (Enter)")
            ttMain.SetToolTip(btnCancel, "ล้างข้อมูลที่กรอกไว้ทั้งหมดเพื่อเริ่มกรอกใหม่")
            ttMain.SetToolTip(btnImportExcel, "นำข้อมูลรายรับจำนวนมากเข้ามาจากไฟล์ Excel (.xlsx)")
            ttMain.SetToolTip(btnBrowseReceipt, "เลือกรูปภาพหลักฐาน/ใบเสร็จ จากเครื่องคอมพิวเตอร์")
            ttMain.SetToolTip(btnPasteReceipt, "วางรูปภาพหลักฐานที่คัดลอกมาจาก LINE หรือโปรแกรมอื่น (Ctrl+V)")
            ttMain.SetToolTip(btnClearReceipt, "ยกเลิกการเลือกรูปภาพ")

            ' ใช้ AddHandler แทน Handles เพื่อเลี่ยงปัญหา BC30506 ในบางสภาพแวดล้อม
            RemoveHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
            AddHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
        End Sub

        Private Sub LoadMasters()
            Using conn = Db.OpenConn()
                Dim cats = Db.GetTable(conn, "SELECT ID, CategoryName FROM Categories WHERE TranType='Income' ORDER BY CategoryName")
                cboCategory.DisplayMember = "CategoryName"
                cboCategory.ValueMember = "ID"
                cboCategory.DataSource = cats
                FillAutoCompleteSource(cboCategory, cats, "CategoryName")

                Dim funds = Db.GetTable(conn, "SELECT ID, FundName FROM Funds ORDER BY FundName")
                cboFund.DisplayMember = "FundName"
                cboFund.ValueMember = "ID"
                cboFund.DataSource = funds
                FillAutoCompleteSource(cboFund, funds, "FundName")

                Dim banks = Db.GetTable(conn, "SELECT ID, BankName & '  ' & IIF(AccountNo IS NULL,'',AccountNo) & '  (' & IIF(AccountName IS NULL,'',AccountName) & ')' AS Disp FROM BankAccounts ORDER BY BankName")
                Dim blankBank = banks.NewRow()
                blankBank("ID") = DBNull.Value
                blankBank("Disp") = ""
                banks.Rows.InsertAt(blankBank, 0)
                cboBank.DisplayMember = "Disp"
                cboBank.ValueMember = "ID"
                cboBank.DataSource = banks
            End Using
            RestoreLastSelections()
        End Sub

        ''' <summary>
        ''' โหลดรายการรายรับล่าสุด 10 รายการมาแสดงในแผงตัวอย่างด้านขวา
        ''' </summary>
        Private Sub LoadRecentTransactions()
            Try
                dgvRecentTransactions.DataSource = Nothing
                Using conn = Db.OpenConn()
                    Dim dtRecent = Db.GetTable(conn,
                        "SELECT TOP 10 t.TranDate AS TransactionDate, c.CategoryName, f.FundName, t.Amount, t.[Detail] AS [Description] " &
                        "FROM ((Transactions t " &
                        "LEFT JOIN Categories c ON t.CategoryID = c.ID) " &
                        "LEFT JOIN Funds f ON t.FundID = f.ID) " &
                        "WHERE t.TranType = 'Income' " &
                        "ORDER BY t.TranDate DESC, t.ID DESC")
                    dgvRecentTransactions.DataSource = dtRecent
                    ConfigureRecentGridColumns()
                End Using
            Catch ex As Exception
                dgvRecentTransactions.DataSource = Nothing
                AppPaths.LogCrash(ex, "FrmIncome.LoadRecentTransactions")
            End Try
        End Sub

        ''' <summary>ตั้งความกว้างคอลัมน์ตาราง: คอลัมน์สั้น DisplayedCells, Description Fill เต็มพื้นที่ที่เหลือ</summary>
        Private Sub ConfigureRecentGridColumns()
            If dgvRecentTransactions Is Nothing Then Return
            dgvRecentTransactions.ScrollBars = ScrollBars.Both

            If dgvRecentTransactions.Columns.Contains("TransactionDate") Then
                dgvRecentTransactions.Columns("TransactionDate").HeaderText = "วันที่ทำรายการ"
                dgvRecentTransactions.Columns("TransactionDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End If
            If dgvRecentTransactions.Columns.Contains("CategoryName") Then
                dgvRecentTransactions.Columns("CategoryName").HeaderText = "ประเภท"
            End If
            If dgvRecentTransactions.Columns.Contains("FundName") Then
                dgvRecentTransactions.Columns("FundName").HeaderText = "กองทุน"
            End If
            If dgvRecentTransactions.Columns.Contains("Amount") Then
                dgvRecentTransactions.Columns("Amount").HeaderText = "จำนวนเงิน"
                dgvRecentTransactions.Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvRecentTransactions.Columns.Contains("Description") Then
                dgvRecentTransactions.Columns("Description").HeaderText = "รายละเอียด"
            End If

            For Each col As DataGridViewColumn In dgvRecentTransactions.Columns
                Select Case col.Name
                    Case "TransactionDate"
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                        col.MinimumWidth = 90
                    Case "CategoryName"
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                        col.MinimumWidth = 110
                    Case "FundName"
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                        col.MinimumWidth = 120
                    Case "Amount"
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                        col.MinimumWidth = 90
                    Case "Description"
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                        col.MinimumWidth = 350
                End Select
            Next
        End Sub

        ''' <summary>
        ''' คำนวณยอดสรุปประจำเดือน (จำนวนใบ / ยอดสูงสุด / ยอดเฉลี่ย / ยอดรวม) สำหรับรายรับ
        ''' </summary>
        Private Sub CalculateMonthlySummary()
            Try
                Dim monthStart As New Date(Today.Year, Today.Month, 1)
                Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn,
                        "SELECT COUNT(*) AS TotalCount, " &
                        "IIF(ISNULL(MAX(Amount)),0,MAX(Amount)) AS MaxAmount, " &
                        "IIF(ISNULL(AVG(Amount)),0,AVG(Amount)) AS AvgAmount, " &
                        "IIF(ISNULL(SUM(Amount)),0,SUM(Amount)) AS TotalAmount " &
                        "FROM Transactions " &
                        "WHERE TranType='Income' AND TranDate BETWEEN @m1 AND @m2",
                        New Tuple(Of String, Object)("@m1", monthStart),
                        New Tuple(Of String, Object)("@m2", monthEnd))
                    If dt.Rows.Count > 0 Then
                        Dim dr = dt.Rows(0)
                        Dim totalCount = Db.ToIntOrZero(dr("TotalCount"))
                        Dim maxAmount = Db.ToDecimalOrZero(dr("MaxAmount"))
                        Dim avgAmount = Db.ToDecimalOrZero(dr("AvgAmount"))
                        Dim totalAmount = Db.ToDecimalOrZero(dr("TotalAmount"))

                        lblReceiptCount.Text = "จำนวน " & totalCount & " ใบ"
                        lblMaxAmount.Text = "สูงสุด " & Format(maxAmount, "#,##0.00")
                        lblAvgAmount.Text = "เฉลี่ย " & Format(avgAmount, "#,##0.00")
                        lblTotalMonthlyIncome.Text = "รวมรายรับ " & Format(totalAmount, "#,##0.00")
                    End If
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmIncome.CalculateMonthlySummary")
            End Try
        End Sub

        Private Sub ResetEntry(resetToToday As Boolean)
            If resetToToday Then
                dtpDate.Value = Today
            End If
            ' หมายเหตุ: ไม่ reset cboCategory / cboFund เพื่อคงค่า "ประเภท/กองทุน" ที่เลือกครั้งล่าสุดไว้
            cboBank.SelectedIndex = 0
            cboReceiveChannel.SelectedIndex = 0
            txtReferenceNo.Clear()
            txtDescription.Clear()
            txtAmount.Clear()
            txtRemark.Clear()
            txtReceipt.Clear()
        End Sub

        ''' <summary>คืนค่า "ประเภท/กองทุน" กลับเป็นค่าที่ผู้ใช้เลือกครั้งล่าสุด (ถ้าไม่มีให้เลือกตัวแรก)</summary>
        Private Sub RestoreLastSelections()
            SetComboValue(cboCategory, AppPreferences.LastIncomeCategoryID)
            SetComboValue(cboFund, AppPreferences.LastIncomeFundID)
        End Sub

        ''' <summary>จดจำค่า "ประเภท/กองทุน" ที่เลือกอยู่ปัจจุบันลงใน AppPreferences</summary>
        Private Sub RememberSelections()
            AppPreferences.LastIncomeCategoryID = GetSelectedId(cboCategory)
            AppPreferences.LastIncomeFundID = GetSelectedId(cboFund)
            AppPreferences.Save()
        End Sub

        Private Shared Sub SetComboValue(cbo As ComboBox, value As Integer)
            If cbo Is Nothing OrElse cbo.Items.Count = 0 Then Return
            If value > 0 Then
                Try
                    cbo.SelectedValue = value
                Catch
                End Try
            End If
            If cbo.SelectedIndex < 0 Then
                cbo.SelectedIndex = 0
            End If
        End Sub

        Private Shared Function GetSelectedId(cbo As ComboBox) As Integer
            If cbo Is Nothing OrElse cbo.SelectedValue Is Nothing OrElse cbo.SelectedValue Is DBNull.Value Then Return 0
            Dim id As Integer
            If Integer.TryParse(cbo.SelectedValue.ToString(), id) Then Return id
            Return 0
        End Function

        Private Function GetSelectedBankValue() As Object
            If cboBank.SelectedValue Is Nothing OrElse cboBank.SelectedValue Is DBNull.Value Then
                Return DBNull.Value
            End If
            Dim bankId As Integer
            If Integer.TryParse(cboBank.SelectedValue.ToString(), bankId) Then
                Return bankId
            End If
            Return DBNull.Value
        End Function

        Private Function GetSelectedBankIdOrNothing() As Integer?
            Dim bankValue = GetSelectedBankValue()
            If bankValue Is DBNull.Value Then Return Nothing
            Return CInt(bankValue)
        End Function

        Private Function GetSelectedBankValueNullable() As Integer?
            If cboBank.SelectedValue Is Nothing OrElse cboBank.SelectedValue Is DBNull.Value Then Return Nothing
            Dim bankId As Integer
            If Integer.TryParse(cboBank.SelectedValue.ToString(), bankId) Then Return bankId
            Return Nothing
        End Function

        ''' <summary>
        ''' แปลงค่าจาก ComboBox Thai → English channel key สำหรับบันทึกลง DB
        ''' </summary>
        Private Function GetSelectedChannel() As String
            If cboReceiveChannel.SelectedIndex < 0 Then Return ""
            Select Case cboReceiveChannel.SelectedItem.ToString()
                Case "เงินสด" : Return "Cash"
                Case "โอนธนาคาร" : Return "BankTransfer"
                Case "พร้อมเพย์" : Return "PromptPay"
                Case "e-Donation" : Return "eDonation"
                Case Else : Return ""
            End Select
        End Function

        ''' <summary>
        ''' Map English channel key → Thai ComboBox index
        ''' </summary>
        Private Sub SetSelectedChannel(channelKey As String)
            Select Case channelKey
                Case "Cash" : cboReceiveChannel.SelectedItem = "เงินสด"
                Case "BankTransfer" : cboReceiveChannel.SelectedItem = "โอนธนาคาร"
                Case "PromptPay" : cboReceiveChannel.SelectedItem = "พร้อมเพย์"
                Case "eDonation" : cboReceiveChannel.SelectedItem = "e-Donation"
                Case Else : cboReceiveChannel.SelectedIndex = 0
            End Select
        End Sub

        ''' <summary>
        ''' แสดง/ซ่อน ช่อง ReferenceNo เมื่อเลือก PromptPay หรือ e-Donation
        ''' </summary>
        Private Sub cboReceiveChannel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboReceiveChannel.SelectedIndexChanged
            Dim channel = GetSelectedChannel()
            Dim isEDonation = (channel = "PromptPay" OrElse channel = "eDonation")
            lblRefNo.Visible = isEDonation
            txtReferenceNo.Visible = isEDonation
        End Sub

        Private Sub SetupEnterNavigation()
            If _enterFlow.Count > 0 Then Return

            _enterFlow.AddRange({dtpDate, cboCategory, cboReceiveChannel, txtDescription, txtAmount, btnSave})

            For Each ctrl In _enterFlow
                AddHandler ctrl.KeyDown, AddressOf HandleEnterAdvance
            Next
        End Sub

        ''' <summary>
        ''' เติม AutoCompleteCustomSource จากตารางข้อมูลหลัก เพื่อให้พิมพ์ค้นหาแบบ SuggestAppend ทำงาน
        ''' </summary>
        Private Shared Sub FillAutoCompleteSource(cbo As ComboBox, dt As DataTable, colName As String)
            cbo.AutoCompleteCustomSource.Clear()
            For Each row As DataRow In dt.Rows
                Dim v = Convert.ToString(row(colName))
                If Not String.IsNullOrWhiteSpace(v) Then
                    cbo.AutoCompleteCustomSource.Add(v.Trim())
                End If
            Next
        End Sub

        Private Sub MoveNextFrom(current As Control)
            ' แก้ข้อความที่พิมพ์ใน ComboBox แบบ DropDown ให้ตรงกับรายการจริงก่อนเลื่อนฟิลด์ (Enter)
            Dim curCb = TryCast(current, ComboBox)
            If curCb IsNot Nothing AndAlso curCb.DropDownStyle = ComboBoxStyle.DropDown Then
                ResolveComboTextSelection(curCb)
            End If

            Dim idx = _enterFlow.IndexOf(current)
            If idx < 0 Then Return

            If idx = _enterFlow.Count - 1 Then
                btnSave.PerformClick()
                Return
            End If

            Dim nextCtrl = _enterFlow(idx + 1)
            nextCtrl.Focus()

            Dim cb = TryCast(nextCtrl, ComboBox)
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then
                cb.DroppedDown = True
            End If

            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then
                tb.SelectAll()
            End If
        End Sub

        ''' <summary>
        ''' เลือก item ที่ตรงกับข้อความที่พิมพ์ใน ComboBox (ให้ Enter เลือกรายการจริง ไม่ใช่แค่พิมพ์ข้อความ)
        ''' </summary>
        Private Sub ResolveComboTextSelection(cb As ComboBox)
            Dim typed = cb.Text.Trim()
            If typed = "" Then Return
            For Each item In cb.Items
                If String.Equals(cb.GetItemText(item).Trim(), typed, StringComparison.OrdinalIgnoreCase) Then
                    cb.SelectedItem = item
                    Return
                End If
            Next
        End Sub

        Private Sub HandleEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(DirectCast(sender, Control))
        End Sub

        Private Sub FocusStartField()
            If Not IsHandleCreated Then Return
            BeginInvoke(New Action(Sub()
                                       dtpDate.Focus()
                                   End Sub))
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            If _editId > 0 Then
                ' ถ้าเป็นโหมดแก้ไข ให้ย้อนกลับไปหน้า Transactions
                Dim f = TryCast(Me.ParentForm, frmMain)
                If f IsNot Nothing Then
                    f.btnMember.PerformClick()
                End If
            Else
                ' ถ้าเป็นโหมดเพิ่มใหม่ ให้ล้างข้อมูล
                ResetEntry(True)
                dtpDate.Focus()
            End If
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If cboCategory.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกประเภทรายรับ", "แจ้งเตือน") : cboCategory.Focus() : Return
            If cboFund.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกกองทุน", "แจ้งเตือน") : cboFund.Focus() : Return
            Dim amt As Decimal
            If Not Decimal.TryParse(txtAmount.Text, amt) OrElse amt <= 0 Then
                MessageBox.Show("กรุณาใส่จำนวนเงินที่ถูกต้อง", "แจ้งเตือน") : txtAmount.Focus() : Return
            End If
            Dim keepDate = dtpDate.Value.Date
            Dim bankValue = GetSelectedBankValue()
            Dim channel As String = GetSelectedChannel()
            Dim refNo As String = If(channel = "PromptPay" OrElse channel = "eDonation", txtReferenceNo.Text.Trim, "")
            Try
                Using conn = Db.OpenConn()
                    Dim targetID As Integer = _editId

                    If _editId > 0 Then
                        ' โหมดแก้ไข: UPDATE ผ่าน Service
                        _txnService.UpdateIncome(_editId, dtpDate.Value.Date, CInt(cboCategory.SelectedValue),
                            CInt(cboFund.SelectedValue), GetSelectedBankValueNullable(), txtDescription.Text.Trim,
                            amt, txtRemark.Text.Trim, channel, refNo)
                    Else
                        ' โหมดเพิ่มใหม่: INSERT ผ่าน Service
                        targetID = _txnService.SaveIncome(dtpDate.Value.Date, CInt(cboCategory.SelectedValue),
                            CInt(cboFund.SelectedValue), GetSelectedBankValueNullable(), txtDescription.Text.Trim,
                            amt, txtRemark.Text.Trim, channel, refNo)
                    End If

                    ' 2. จัดการรูปภาพใบเสร็จ (ถ้ามีการเลือกใหม่ หรือวางจากคลิปบอร์ด)
                    If Not String.IsNullOrEmpty(selectedSourceReceiptPath) Then
                        Dim savedFileName As String = SaveReceiptFile(targetID)
                        If Not String.IsNullOrEmpty(savedFileName) Then
                            _txnService.UpdateReceiptPath(targetID, savedFileName)
                        End If
                    End If

                    MessageBox.Show("✅ บันทึกข้อมูลเรียบร้อยแล้ว!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' จดจำประเภท/กองทุนที่เลือกครั้งล่าสุด เพื่อคืนค่าให้อัตโนมัติเมื่อเปิดฟอร์มใหม่
                    RememberSelections()

                    ' รีเฟรชยอดสรุปประจำเดือนทันที หลังบันทึก/แก้ไขรายการ
                    CalculateMonthlySummary()

                    If _editId > 0 Then
                        ' ถ้าเป็นการแก้ไข ให้ปิดหน้าจอนี้และกลับไปหน้า Transactions
                        Dim f = TryCast(Me.ParentForm, frmMain)
                        If f IsNot Nothing Then
                            f.btnMember.PerformClick()
                        End If
                    Else
                        ResetEntry(False)
                        ' ล้างค่าตัวแปรเก็บ Path รูปภาพที่เคยเลือกไว้
                        selectedSourceReceiptPath = ""
                        If txtReceipt IsNot Nothing Then txtReceipt.Clear()

                        ' ล้าง Clipboard หลังบันทึกรูปสำเร็จ
                        Try
                            Clipboard.Clear()
                        Catch ex As Exception
                            AppPaths.LogCrash(ex, "FrmIncome.ClearClipboard")
                        End Try

                        dtpDate.Value = keepDate
                        dtpDate.Focus()

                        ' รีเฟรชรายการล่าสุดทันที หลังบันทึกและเคลียร์ฟอร์มเรียบร้อยแล้ว
                        LoadRecentTransactions()
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' --- ฟังก์ชันช่วยก๊อบปี้รูปภาพใบเสร็จไปยัง AppPaths.ReceiptsDir โดยการย่อขนาดและบีบอัด ---
        Private Function SaveReceiptFile(transactionID As Integer) As String
            If isImageFromClipboard Then
                If clipboardImage Is Nothing Then Return ""
                Return ReceiptImageHelper.SaveOptimizedReceipt(clipboardImage, transactionID)
            End If

            If String.IsNullOrWhiteSpace(selectedSourceReceiptPath) OrElse Not IO.File.Exists(selectedSourceReceiptPath) Then
                Return ""
            End If

            Return ReceiptImageHelper.SaveOptimizedReceipt(selectedSourceReceiptPath, transactionID)
        End Function

        Private Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
            If cboCategory.SelectedValue Is Nothing OrElse cboFund.SelectedValue Is Nothing Then
                MessageBox.Show("กรุณาเลือกประเภท และกองทุนเริ่มต้นก่อนนำเข้า", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using ofd As New OpenFileDialog()
                ofd.Title = "เลือกไฟล์ Excel สำหรับนำเข้ารายรับ"
                ofd.Filter = "Excel Files|*.xlsx;*.xlsm;*.xlsb;*.xls|All Files|*.*"
                If ofd.ShowDialog(Me) <> DialogResult.OK Then Return

                Try
                    Dim defaultBankId = GetSelectedBankIdOrNothing()
                    Dim result = ImportTransactionsFromExcel(ofd.FileName, "Income", CInt(cboCategory.SelectedValue), CInt(cboFund.SelectedValue), defaultBankId)
                    LoadMasters()
                    CalculateMonthlySummary()
                    ResetEntry(False)
                    dtpDate.Focus()
                    MessageBox.Show(result.BuildSummaryMessage("รายรับ"), "นำเข้าข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("นำเข้าข้อมูลไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub

        ' ปุ่มกดเลือกรูปภาพ
        Private Sub btnBrowseReceipt_Click(sender As Object, e As EventArgs) Handles btnBrowseReceipt.Click
            Using ofd As New OpenFileDialog()
                ' กำหนดโฟลเดอร์เริ่มต้นไปที่จุดดาวน์โหลดของ LINE Desktop
                If IO.Directory.Exists("C:\LineDownloads") Then
                    ofd.InitialDirectory = "C:\LineDownloads"
                End If

                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
                ofd.Title = "เลือกรูปภาพใบเสร็จ"

                If ofd.ShowDialog() = DialogResult.OK Then
                    selectedSourceReceiptPath = ofd.FileName
                    txtReceipt.Text = IO.Path.GetFileName(selectedSourceReceiptPath)
                End If
            End Using
        End Sub

        Private Sub btnPasteReceipt_Click(sender As Object, e As EventArgs)
            If Clipboard.ContainsImage() Then
                clipboardImage = Clipboard.GetImage()
                isImageFromClipboard = True
                selectedSourceReceiptPath = "Clipboard_Image"
                txtReceipt.Text = "[รูปภาพจากคลิปบอร์ด/LINE]"
                btnSave.Focus()
            Else
                MessageBox.Show("ไม่พบรูปภาพใหม่ในคลิปบอร์ด กรุณาไปที่ LINE แล้วกดคัดลอกรูปภาพใบเสร็จใหม่ก่อนกดปุ่มนี้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End Sub

        Private Sub btnClearReceipt_Click(sender As Object, e As EventArgs) Handles btnClearReceipt.Click
            selectedSourceReceiptPath = ""
            isImageFromClipboard = False
            If clipboardImage IsNot Nothing Then clipboardImage.Dispose()
            clipboardImage = Nothing
            txtReceipt.Clear()

            ' ล้าง Clipboard เพื่อป้องกันการวางรูปเดิมซ้ำ
            Try
                Clipboard.Clear()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmIncome.ClearReceipt.ClearClipboard")
            End Try
        End Sub

        Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
            If Char.IsControl(e.KeyChar) Then Return
            Dim tb = DirectCast(sender, TextBox)
            If e.KeyChar = "."c AndAlso tb.Text.Contains(".") Then e.Handled = True : Return
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then e.Handled = True
        End Sub
    End Class
End Namespace
