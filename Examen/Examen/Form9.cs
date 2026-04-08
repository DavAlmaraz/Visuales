using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Examen
{
    public partial class Form9 : Form
    {
        public Form9()
        {
            InitializeComponent();
            InitializeAccountOperations();
        }

        private ComboBox cmbDepositType;
        private ComboBox cmbTransferBank;
        private ComboBox cmbWithdrawProvider;
        private NumericUpDown numDepositAmount;
        private NumericUpDown numTransferAmount;
        private NumericUpDown numWithdrawAmount;
        private Button btnDeposit;
        private Button btnTransfer;
        private Button btnWithdraw;
        private Button btnSummary;
        private Label lblAcct;
        private ComboBox cmbDepositBank;
        private Button btnBackOps;
        private DataGridView historyGrid;
        private ComboBox cmbRechargeCompany;
        private NumericUpDown numRechargeAmount;
        private Button btnRecharge;
        private ComboBox cmbServiceType;
        private NumericUpDown numServiceAmount;
        private Button btnServicePayment;
        private ComboBox cmbTagProvider;
        private NumericUpDown numTagAmount;
        private Button btnTagRecharge;

        private void InitializeAccountOperations()
        {
            this.Text = "Mercado Pago - Operaciones";
            this.Size = new Size(860, 640);
            this.BackColor = Color.FromArgb(245, 248, 255);
            this.Font = new Font("Segoe UI", 9.5f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Top banner (MP blue gradient) ──
            Panel topBanner = new Panel
            {
                Size = new Size(860, 60),
                Location = new Point(0, 0),
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(66, 165, 245)
            };
            topBanner.Paint += (s, ev) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    topBanner.ClientRectangle,
                    Color.FromArgb(21, 101, 192),
                    Color.FromArgb(66, 165, 245),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    ev.Graphics.FillRectangle(brush, topBanner.ClientRectangle);
                }
            };
            lblAcct = new Label
            {
                Text = "Cuenta: (sin sesión)",
                Location = new Point(18, 8),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            Label lblAcctSub = new Label
            {
                Text = "💳 Mercado Pago - Operaciones",
                Location = new Point(18, 34),
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 227, 255),
                Font = new Font("Segoe UI", 9f),
                BackColor = Color.Transparent
            };
            btnBackOps = new Button
            {
                Text = "← Volver",
                Location = new Point(720, 14),
                Size = new Size(110, 32),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackOps.FlatAppearance.BorderColor = Color.FromArgb(100, 180, 255);
            btnBackOps.Click += (s, e) => { if (this.Owner != null) { this.Owner.Show(); } this.Close(); };
            topBanner.Controls.Add(lblAcct);
            topBanner.Controls.Add(lblAcctSub);
            topBanner.Controls.Add(btnBackOps);

            // ── TabControl ──
            TabControl tabs = new TabControl
            {
                Location = new Point(10, 66),
                Size = new Size(824, 530),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            // ═══════════════════════════════════════
            //  TAB 1: Operaciones Bancarias
            // ═══════════════════════════════════════
            TabPage tabBanking = new TabPage("🏦 Operaciones Bancarias")
            {
                BackColor = Color.FromArgb(250, 252, 255),
                Padding = new Padding(8)
            };

            // ── Deposit panel ──
            Panel pnlDeposit = new Panel { Location = new Point(10, 10), Size = new Size(390, 220), BackColor = Color.White };
            pnlDeposit.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 202, 40)), 0, 0, pnlDeposit.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 236, 179)), 0, 0, pnlDeposit.Width - 1, pnlDeposit.Height - 1);
            };
            Label lblDepTitle = new Label { Text = "💰 Ingresos", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(245, 166, 35), Location = new Point(16, 14), AutoSize = true };
            Label lblDepType = new Label { Text = "Tipo:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbDepositType = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbDepositType.Items.AddRange(new object[] { "Oxxo", "Seven Eleven", "Depósito bancario", "Transferencia", "Dinero desde el extranjero (USD)", "Dinero desde el extranjero (EUR)" });
            cmbDepositType.SelectedIndex = 0;
            Label lblDepAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numDepositAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 1, Maximum = 100000, Value = 100, Font = new Font("Segoe UI", 9.5f) };
            Label lblBank = new Label { Text = "Banco:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 102), AutoSize = true };
            cmbDepositBank = new ComboBox { Location = new Point(16, 122), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            var banks = new object[] { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" };
            cmbDepositBank.Items.AddRange(banks);
            if (cmbDepositBank.Items.Count > 0) cmbDepositBank.SelectedIndex = 0;
            btnDeposit = new Button { Text = "Ingresar", Location = new Point(16, 168), Size = new Size(360, 36), BackColor = Color.FromArgb(255, 202, 40), ForeColor = Color.FromArgb(50, 40, 10), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnDeposit.FlatAppearance.BorderSize = 0;
            btnDeposit.Click += BtnDeposit_Click;
            pnlDeposit.Controls.AddRange(new Control[] { lblDepTitle, lblDepType, cmbDepositType, lblDepAmt, numDepositAmount, lblBank, cmbDepositBank, btnDeposit });

            cmbDepositType.SelectedIndexChanged += (s, e) =>
            {
                var isBank = cmbDepositType.SelectedItem.ToString().Contains("Depósito bancario");
                cmbDepositBank.Enabled = isBank;
            };

            // ── Transfer panel ──
            Panel pnlTransfer = new Panel { Location = new Point(414, 10), Size = new Size(390, 220), BackColor = Color.White };
            pnlTransfer.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(66, 165, 245)), 0, 0, pnlTransfer.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(187, 222, 251)), 0, 0, pnlTransfer.Width - 1, pnlTransfer.Height - 1);
            };
            Label lblTrTitle = new Label { Text = "🔄 Transferencias", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 136, 229), Location = new Point(16, 14), AutoSize = true };
            Label lblTrBank = new Label { Text = "Banco destino:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbTransferBank = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbTransferBank.Items.AddRange(new object[] { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" });
            cmbTransferBank.SelectedIndex = 0;
            Label lblTrAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numTransferAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 1, Maximum = 100000, Value = 100, Font = new Font("Segoe UI", 9.5f) };
            btnTransfer = new Button { Text = "Transferir", Location = new Point(16, 120), Size = new Size(360, 36), BackColor = Color.FromArgb(66, 165, 245), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnTransfer.FlatAppearance.BorderSize = 0;
            btnTransfer.Click += (s, e) => { numTransferAmount_ValueChanged(s, e); BtnTransfer_Click(s, e); };
            pnlTransfer.Controls.AddRange(new Control[] { lblTrTitle, lblTrBank, cmbTransferBank, lblTrAmt, numTransferAmount, btnTransfer });

            // ── Withdraw panel ──
            Panel pnlWithdraw = new Panel { Location = new Point(10, 244), Size = new Size(390, 220), BackColor = Color.White };
            pnlWithdraw.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(102, 187, 106)), 0, 0, pnlWithdraw.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 230, 201)), 0, 0, pnlWithdraw.Width - 1, pnlWithdraw.Height - 1);
            };
            Label lblWdTitle = new Label { Text = "💸 Retiros", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(46, 125, 50), Location = new Point(16, 14), AutoSize = true };
            Label lblWdProv = new Label { Text = "Proveedor:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbWithdrawProvider = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbWithdrawProvider.Items.AddRange(new object[] { "Mercado Pago Express", "7-Eleven", "Soriana", "Chedraui", "Aurrera", "Walmart" });
            cmbWithdrawProvider.SelectedIndex = 0;
            Label lblWdAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numWithdrawAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 1, Maximum = 100000, Value = 100, Font = new Font("Segoe UI", 9.5f) };
            btnWithdraw = new Button { Text = "Retirar", Location = new Point(16, 120), Size = new Size(360, 36), BackColor = Color.FromArgb(102, 187, 106), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnWithdraw.FlatAppearance.BorderSize = 0;
            btnWithdraw.Click += (s, e) => { numWithdrawAmount_ValueChanged(s, e); BtnWithdraw_Click(s, e); };
            pnlWithdraw.Controls.AddRange(new Control[] { lblWdTitle, lblWdProv, cmbWithdrawProvider, lblWdAmt, numWithdrawAmount, btnWithdraw });

            // Summary button in banking tab
            btnSummary = new Button
            {
                Text = "📊 Informe de cuenta",
                Location = new Point(414, 244),
                Size = new Size(390, 46),
                BackColor = Color.FromArgb(232, 245, 233),
                ForeColor = Color.FromArgb(27, 94, 32),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSummary.FlatAppearance.BorderColor = Color.FromArgb(165, 214, 167);
            btnSummary.Click += BtnSummary_Click;

            tabBanking.Controls.Add(pnlDeposit);
            tabBanking.Controls.Add(pnlTransfer);
            tabBanking.Controls.Add(pnlWithdraw);
            tabBanking.Controls.Add(btnSummary);

            // ═══════════════════════════════════════
            //  TAB 2: Servicios y Recargas
            // ═══════════════════════════════════════
            TabPage tabServices = new TabPage("📱 Servicios y Recargas")
            {
                BackColor = Color.FromArgb(250, 252, 255),
                Padding = new Padding(8)
            };

            // ── Recarga Celular panel ──
            Panel pnlRecharge = new Panel { Location = new Point(10, 10), Size = new Size(390, 220), BackColor = Color.White };
            pnlRecharge.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(186, 104, 200)), 0, 0, pnlRecharge.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(225, 190, 231)), 0, 0, pnlRecharge.Width - 1, pnlRecharge.Height - 1);
            };
            Label lblRchTitle = new Label { Text = "📱 Recarga Celular", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(123, 31, 162), Location = new Point(16, 14), AutoSize = true };
            Label lblRchComp = new Label { Text = "Compañía:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbRechargeCompany = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbRechargeCompany.Items.AddRange(new object[] { "AT&T", "Unefón", "Telcel", "Movistar", "Bait" });
            cmbRechargeCompany.SelectedIndex = 0;
            Label lblRchAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numRechargeAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 10, Maximum = 5000, Value = 100, Font = new Font("Segoe UI", 9.5f) };
            btnRecharge = new Button { Text = "Recargar", Location = new Point(16, 120), Size = new Size(360, 36), BackColor = Color.FromArgb(186, 104, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnRecharge.FlatAppearance.BorderSize = 0;
            btnRecharge.Click += BtnRecharge_Click;
            pnlRecharge.Controls.AddRange(new Control[] { lblRchTitle, lblRchComp, cmbRechargeCompany, lblRchAmt, numRechargeAmount, btnRecharge });

            // ── Pago de Servicios panel ──
            Panel pnlService = new Panel { Location = new Point(414, 10), Size = new Size(390, 220), BackColor = Color.White };
            pnlService.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 150, 136)), 0, 0, pnlService.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(178, 223, 219)), 0, 0, pnlService.Width - 1, pnlService.Height - 1);
            };
            Label lblSvcTitle = new Label { Text = "🏠 Pago de Servicios", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 105, 92), Location = new Point(16, 14), AutoSize = true };
            Label lblSvcType = new Label { Text = "Servicio:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbServiceType = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbServiceType.Items.AddRange(new object[] { "Otros servicios", "Internet", "Agua", "Luz" });
            cmbServiceType.SelectedIndex = 0;
            Label lblSvcAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numServiceAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 1, Maximum = 50000, Value = 200, Font = new Font("Segoe UI", 9.5f) };
            btnServicePayment = new Button { Text = "Pagar servicio", Location = new Point(16, 120), Size = new Size(360, 36), BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnServicePayment.FlatAppearance.BorderSize = 0;
            btnServicePayment.Click += BtnServicePayment_Click;
            pnlService.Controls.AddRange(new Control[] { lblSvcTitle, lblSvcType, cmbServiceType, lblSvcAmt, numServiceAmount, btnServicePayment });

            // ── Recarga de Tag panel ──
            Panel pnlTag = new Panel { Location = new Point(10, 244), Size = new Size(390, 220), BackColor = Color.White };
            pnlTag.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 152, 0)), 0, 0, pnlTag.Width, 5);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 224, 178)), 0, 0, pnlTag.Width - 1, pnlTag.Height - 1);
            };
            Label lblTagTitle = new Label { Text = "🏷 Recarga de Tag", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Color.FromArgb(230, 126, 34), Location = new Point(16, 14), AutoSize = true };
            Label lblTagProv = new Label { Text = "Proveedor:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(16, 48), AutoSize = true };
            cmbTagProvider = new ComboBox { Location = new Point(16, 68), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbTagProvider.Items.AddRange(new object[] { "Pase", "Televia" });
            cmbTagProvider.SelectedIndex = 0;
            Label lblTagAmt = new Label { Text = "Monto:", Font = new Font("Segoe UI", 9f), ForeColor = Color.Gray, Location = new Point(280, 48), AutoSize = true };
            numTagAmount = new NumericUpDown { Location = new Point(280, 68), Width = 96, Minimum = 50, Maximum = 10000, Value = 200, Font = new Font("Segoe UI", 9.5f) };
            btnTagRecharge = new Button { Text = "Recargar Tag", Location = new Point(16, 120), Size = new Size(360, 36), BackColor = Color.FromArgb(255, 152, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnTagRecharge.FlatAppearance.BorderSize = 0;
            btnTagRecharge.Click += BtnTagRecharge_Click;
            pnlTag.Controls.AddRange(new Control[] { lblTagTitle, lblTagProv, cmbTagProvider, lblTagAmt, numTagAmount, btnTagRecharge });

            tabServices.Controls.Add(pnlRecharge);
            tabServices.Controls.Add(pnlService);
            tabServices.Controls.Add(pnlTag);

            // ═══════════════════════════════════════
            //  TAB 3: Historial
            // ═══════════════════════════════════════
            TabPage tabHistory = new TabPage("📋 Historial")
            {
                BackColor = Color.FromArgb(250, 252, 255),
                Padding = new Padding(8)
            };

            Label lblHist = new Label
            {
                Text = "📋 Historial de operaciones recientes",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 90, 100)
            };
            historyGrid = new DataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(790, 440),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(224, 224, 224),
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9f),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            historyGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(66, 165, 245);
            historyGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            historyGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            historyGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            historyGrid.EnableHeadersVisualStyles = false;
            historyGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(227, 242, 253);
            historyGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);
            historyGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            historyGrid.Columns.Add("Hora", "Hora");
            historyGrid.Columns.Add("Tipo", "Tipo");
            historyGrid.Columns.Add("Monto", "Monto");
            historyGrid.Columns.Add("Detalles", "Detalles");
            historyGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            tabHistory.Controls.Add(lblHist);
            tabHistory.Controls.Add(historyGrid);

            tabs.TabPages.Add(tabBanking);
            tabs.TabPages.Add(tabServices);
            tabs.TabPages.Add(tabHistory);

            this.Controls.Add(topBanner);
            this.Controls.Add(tabs);

            UpdateAccountLabel();
            LoadPurchaseHistory();
        }

        private void LoadPurchaseHistory()
        {
            var a = AccountManager.CurrentAccount;
            if (a == null) return;
            foreach (var p in a.PurchaseHistory)
            {
                historyGrid.Rows.Add(p.Date.ToString("HH:mm"), "Compra ML", $"${p.TotalPaid:0.00}", $"{p.ProductName} x{p.Quantity}");
            }
            foreach (var r in a.ReturnHistory)
            {
                string cond = r.IsGoodCondition ? "Buen estado" : $"Dañado (-{r.ChargePercent:0}%)";
                historyGrid.Rows.Add(r.Date.ToString("HH:mm"), "Devolución ML", $"${r.RefundAmount:0.00}", $"{r.ProductName} x{r.Quantity} ({cond})");
            }
        }

        private void UpdateAccountLabel()
        {
            var a = AccountManager.CurrentAccount;
            if (a == null) lblAcct.Text = "Cuenta: (sin sesión)";
            else lblAcct.Text = $"Cuenta: {a.Owner} - {a.CardNumber} - Saldo: ${a.Balance:0.00}";
        }

        private void BtnDeposit_Click(object sender, EventArgs e)
        {
            try
            {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string type = cmbDepositType.SelectedItem.ToString();
            decimal amount = numDepositAmount.Value;
            decimal amountToAdd = amount;
            // convert foreign currencies to MXN: USD -> 18, EUR -> 20
            if (type.Contains("USD")) amountToAdd = Math.Round(amount * 18m, 2);
            else if (type.Contains("EUR")) amountToAdd = Math.Round(amount * 20m, 2);
            // determine deposit bank name from combo if enabled
            string bankName = string.Empty;
            if (cmbDepositBank != null && cmbDepositBank.Enabled && cmbDepositBank.SelectedItem != null)
                bankName = cmbDepositBank.SelectedItem.ToString();
            a.Balance += amountToAdd;
            string key = type;
            if (!a.DepositCounts.ContainsKey(key)) a.DepositCounts[key] = 0;
            if (!a.DepositTotals.ContainsKey(key)) a.DepositTotals[key] = 0m;
            a.DepositCounts[key]++;
            a.DepositTotals[key] += amountToAdd;
            UpdateAccountLabel();
            // record history entry
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Ingreso", $"${amountToAdd:0.00}", key);
            MessageBox.Show($"Ingreso realizado.\nCuenta: {a.Owner} - {a.CardNumber}\nTipo: {key}\nMonto ingresado: ${amountToAdd:0.00}\nSaldo actual: ${a.Balance:0.00}", "Ingreso registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el depósito: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string bank = cmbTransferBank.SelectedItem.ToString();
            decimal amount = numTransferAmount.Value;
            if (amount <= 0) { MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (a.Balance < amount) { MessageBox.Show("Saldo insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            a.Balance -= amount;
            if (!a.TransferCounts.ContainsKey(bank)) a.TransferCounts[bank] = 0;
            if (!a.TransferTotals.ContainsKey(bank)) a.TransferTotals[bank] = 0m;
            a.TransferCounts[bank]++;
            a.TransferTotals[bank] += amount;
            UpdateAccountLabel();
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Transferencia", $"${amount:0.00}", bank);
            MessageBox.Show($"Transferencia realizada.\nCuenta origen: {a.Owner} - {a.CardNumber}\nBanco destino: {bank}\nMonto transferido: ${amount:0.00}\nSaldo actual: ${a.Balance:0.00}", "Transferencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnWithdraw_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string prov = cmbWithdrawProvider.SelectedItem.ToString();
            decimal amount = numWithdrawAmount.Value;
            decimal limit = 0m, commission = 0m;
            switch (prov)
            {
                case "Mercado Pago Express": limit = 2000m; commission = 0m; break;
                case "7-Eleven": limit = 3000m; commission = 0m; break;
                case "Soriana": limit = 2000m; commission = 0m; break;
                case "Chedraui": limit = 9000m; commission = 20m; break;
                case "Aurrera": limit = 3000m; commission = 15m; break;
                case "Walmart": limit = 3000m; commission = 15m; break;
            }
            if (amount > limit) { MessageBox.Show($"El monto excede el límite para {prov} (${limit:0.00}).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            decimal totalDeduct = amount + commission;
            if (a.Balance < totalDeduct) { MessageBox.Show("Saldo insuficiente para retiro y comisión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            a.Balance -= totalDeduct;
            if (!a.WithdrawCounts.ContainsKey(prov)) a.WithdrawCounts[prov] = 0;
            if (!a.WithdrawTotals.ContainsKey(prov)) a.WithdrawTotals[prov] = 0m;
            a.WithdrawCounts[prov]++;
            a.WithdrawTotals[prov] += amount;
            UpdateAccountLabel();
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Retiro", $"${amount:0.00}", $"{prov} (Com: ${commission:0.00})");
            MessageBox.Show($"Retiro realizado.\nCuenta: {a.Owner} - {a.CardNumber}\nProveedor: {prov}\nMonto retirado: ${amount:0.00}\nComisión: ${commission:0.00}\nSaldo actual: ${a.Balance:0.00}", "Retiro registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSummary_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null) { MessageBox.Show("Inicie sesión para ver el resumen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            var sb = new StringBuilder();
            sb.AppendLine($"  Titular:       {a.Owner}");
            sb.AppendLine($"  Tarjeta:       {a.CardNumber}");
            sb.AppendLine($"  Email:         {a.Email}");
            sb.AppendLine($"  Teléfono:      {a.Phone}");
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════");
            sb.AppendLine("  TOTALES");
            sb.AppendLine("══════════════════════════════════════");
            decimal totalCommissions = 0m;
            foreach (var prov in a.WithdrawCounts.Keys)
            {
                decimal comm = 0m;
                switch (prov)
                {
                    case "Chedraui": comm = 20m; break;
                    case "Aurrera": comm = 15m; break;
                    case "Walmart": comm = 15m; break;
                    default: comm = 0m; break;
                }
                totalCommissions += comm * a.WithdrawCounts[prov];
            }
            sb.AppendLine($"  Comisiones totales:       ${totalCommissions:0.00}");
            sb.AppendLine($"  Ingresos totales:         ${a.TotalDeposited:0.00}");
            sb.AppendLine($"  Transferencias totales:   ${a.TotalTransferred:0.00}");
            sb.AppendLine($"  Retiros totales:          ${a.TotalWithdrawn:0.00}");
            sb.AppendLine($"  Recargas celular totales: ${a.TotalRecharged:0.00}");
            sb.AppendLine($"  Pagos servicios totales:  ${a.TotalServicesPaid:0.00}");
            sb.AppendLine($"  Recargas Tag totales:     ${a.TotalTagRecharged:0.00}");
            sb.AppendLine($"  Compras ML totales:       ${a.TotalPurchased:0.00}");
            sb.AppendLine($"  Saldo actual:             ${a.Balance:0.00}");
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE INGRESOS");
            sb.AppendLine("──────────────────────────────────────");
            int combinedBancTransOps = 0;
            decimal combinedBancTransVal = 0m;
            foreach (var k in a.DepositCounts.Keys)
            {
                decimal val = a.DepositTotals.ContainsKey(k) ? a.DepositTotals[k] : 0m;
                if (k.StartsWith("Depósito bancario") || k == "Transferencia")
                {
                    combinedBancTransOps += a.DepositCounts[k];
                    combinedBancTransVal += val;
                }
                else
                {
                    sb.AppendLine($"   • {k}: {a.DepositCounts[k]} ops — ${val:0.00}");
                }
            }
            if (combinedBancTransOps > 0)
                sb.AppendLine($"   • Depósitos bancarios y Transferencias: {combinedBancTransOps} ops — ${combinedBancTransVal:0.00}");
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE TRANSFERENCIAS");
            sb.AppendLine("──────────────────────────────────────");
            foreach (var k in a.TransferCounts.Keys) { decimal val = a.TransferTotals.ContainsKey(k) ? a.TransferTotals[k] : 0m; sb.AppendLine($"   • {k}: {a.TransferCounts[k]} ops — ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE RETIROS");
            sb.AppendLine("──────────────────────────────────────");
            foreach (var k in a.WithdrawCounts.Keys) { decimal val = a.WithdrawTotals.ContainsKey(k) ? a.WithdrawTotals[k] : 0m; sb.AppendLine($"   • {k}: {a.WithdrawCounts[k]} retiros — ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE RECARGAS CELULAR");
            sb.AppendLine("──────────────────────────────────────");
            foreach (var k in a.RechargeCounts.Keys) { decimal val = a.RechargeTotals.ContainsKey(k) ? a.RechargeTotals[k] : 0m; sb.AppendLine($"   • {k}: {a.RechargeCounts[k]} recargas — ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE PAGOS DE SERVICIOS");
            sb.AppendLine("──────────────────────────────────────");
            foreach (var k in a.ServicePaymentCounts.Keys) { decimal val = a.ServicePaymentTotals.ContainsKey(k) ? a.ServicePaymentTotals[k] : 0m; sb.AppendLine($"   • {k}: {a.ServicePaymentCounts[k]} pagos — ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DETALLE DE RECARGAS TAG");
            sb.AppendLine("──────────────────────────────────────");
            foreach (var k in a.TagRechargeCounts.Keys) { decimal val = a.TagRechargeTotals.ContainsKey(k) ? a.TagRechargeTotals[k] : 0m; sb.AppendLine($"   • {k}: {a.TagRechargeCounts[k]} recargas — ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  COMPRAS EN MERCADO LIBRE");
            sb.AppendLine("──────────────────────────────────────");
            if (a.PurchaseHistory.Count == 0)
            {
                sb.AppendLine("   (Sin compras realizadas)");
            }
            else
            {
                foreach (var k in a.PurchaseCounts.Keys)
                {
                    decimal val = a.PurchaseTotals.ContainsKey(k) ? a.PurchaseTotals[k] : 0m;
                    sb.AppendLine($"   • {k}: {a.PurchaseCounts[k]} unidades — ${val:0.00}");
                }
                sb.AppendLine();
                sb.AppendLine($"   Total productos comprados:     {a.TotalPurchaseItems}");
                sb.AppendLine($"   Total gastado en productos:    ${a.TotalPurchased:0.00}");
            }
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  DEVOLUCIONES");
            sb.AppendLine("──────────────────────────────────────");
            if (a.ReturnHistory.Count == 0)
            {
                sb.AppendLine("   (Sin devoluciones realizadas)");
            }
            else
            {
                foreach (var r in a.ReturnHistory)
                {
                    string cond = r.IsGoodCondition ? "Buen estado" : "Dañado (-20%)";
                    sb.AppendLine($"   • {r.ProductName} x{r.Quantity} — Costo:${r.OriginalCost:0.00} — {cond} — Reembolso:${r.RefundAmount:0.00}");
                }
                sb.AppendLine();
                int goodReturns = a.ReturnHistory.Count(r => r.IsGoodCondition);
                int damagedReturns = a.ReturnHistory.Count(r => !r.IsGoodCondition);
                sb.AppendLine($"   Total devoluciones:            {a.ReturnHistory.Count}");
                sb.AppendLine($"   Devoluciones en buen estado:   {goodReturns}");
                sb.AppendLine($"   Devoluciones con daño:         {damagedReturns}");
                sb.AppendLine();
                decimal totalCompraAcc = a.ReturnHistory.Sum(r => r.OriginalCost);
                decimal totalDevueltoAcc = a.TotalRefunded;
                decimal totalDescuentoAcc = a.TotalDamageCharges;
                decimal pctDescuentoAcc = totalCompraAcc > 0 ? Math.Round((totalDescuentoAcc / totalCompraAcc) * 100m, 2) : 0m;
                sb.AppendLine($"   Compra: ${totalCompraAcc:0.00}");
                sb.AppendLine($"   Devolver: ${totalDevueltoAcc:0.00}");
                sb.AppendLine($"   Descuento: ${totalDescuentoAcc:0.00} ({pctDescuentoAcc:0.##}%)");
            }
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════");
            sb.AppendLine($"  Cuentas creadas: {AccountManager.Accounts.Count}");
            sb.AppendLine($"  Intentos fallidos (global): {AccountManager.TotalFailedLoginAttempts}");
            ShowStyledReport("Informe de Cuenta", sb.ToString(), Color.FromArgb(66, 165, 245));
        }

        private void ShowStyledReport(string title, string content, Color accentColor)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.Size = new Size(520, 480);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = Color.White;
                dlg.Font = new Font("Segoe UI", 10f);

                Panel header = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    BackColor = accentColor
                };
                Label lblTitle = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(16, 12),
                    BackColor = Color.Transparent
                };
                header.Controls.Add(lblTitle);

                TextBox txtReport = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Location = new Point(12, 62),
                    Size = new Size(480, 340),
                    Font = new Font("Consolas", 9.5f),
                    BackColor = Color.FromArgb(250, 252, 255),
                    ForeColor = Color.FromArgb(40, 40, 40),
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = content
                };

                Button btnClose = new Button
                {
                    Text = "Cerrar",
                    Size = new Size(120, 36),
                    Location = new Point(372, 408),
                    BackColor = accentColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = DialogResult.OK
                };
                btnClose.FlatAppearance.BorderSize = 0;

                dlg.Controls.Add(header);
                dlg.Controls.Add(txtReport);
                dlg.Controls.Add(btnClose);
                dlg.AcceptButton = btnClose;
                dlg.ShowDialog(this);
            }
        }

        private void BtnRecharge_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string company = cmbRechargeCompany.SelectedItem.ToString();
            decimal amount = numRechargeAmount.Value;
            if (amount <= 0) { MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (a.Balance < amount) { MessageBox.Show("Saldo insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            a.Balance -= amount;
            if (!a.RechargeCounts.ContainsKey(company)) a.RechargeCounts[company] = 0;
            if (!a.RechargeTotals.ContainsKey(company)) a.RechargeTotals[company] = 0m;
            a.RechargeCounts[company]++;
            a.RechargeTotals[company] += amount;
            UpdateAccountLabel();
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Recarga Cel", $"${amount:0.00}", company);
            MessageBox.Show($"Recarga realizada.\nCompañía: {company}\nMonto: ${amount:0.00}\nSaldo actual: ${a.Balance:0.00}", "Recarga Celular", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnServicePayment_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string service = cmbServiceType.SelectedItem.ToString();
            decimal amount = numServiceAmount.Value;
            if (amount <= 0) { MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (a.Balance < amount) { MessageBox.Show("Saldo insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            a.Balance -= amount;
            if (!a.ServicePaymentCounts.ContainsKey(service)) a.ServicePaymentCounts[service] = 0;
            if (!a.ServicePaymentTotals.ContainsKey(service)) a.ServicePaymentTotals[service] = 0m;
            a.ServicePaymentCounts[service]++;
            a.ServicePaymentTotals[service] += amount;
            UpdateAccountLabel();
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Pago Servicio", $"${amount:0.00}", service);
            MessageBox.Show($"Pago de servicio realizado.\nServicio: {service}\nMonto: ${amount:0.00}\nSaldo actual: ${a.Balance:0.00}", "Pago de Servicio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnTagRecharge_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string provider = cmbTagProvider.SelectedItem.ToString();
            decimal amount = numTagAmount.Value;
            if (amount <= 0) { MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (a.Balance < amount) { MessageBox.Show("Saldo insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            a.Balance -= amount;
            if (!a.TagRechargeCounts.ContainsKey(provider)) a.TagRechargeCounts[provider] = 0;
            if (!a.TagRechargeTotals.ContainsKey(provider)) a.TagRechargeTotals[provider] = 0m;
            a.TagRechargeCounts[provider]++;
            a.TagRechargeTotals[provider] += amount;
            UpdateAccountLabel();
            historyGrid.Rows.Insert(0, DateTime.Now.ToString("HH:mm"), "Recarga Tag", $"${amount:0.00}", provider);
            MessageBox.Show($"Recarga de Tag realizada.\nProveedor: {provider}\nMonto: ${amount:0.00}\nSaldo actual: ${a.Balance:0.00}", "Recarga de Tag", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void numTransferAmount_ValueChanged(object s, EventArgs e)
        {
            numDepositAmount.Value = numTransferAmount.Value;
        }

        private void numWithdrawAmount_ValueChanged(object s, EventArgs e)
        {
            numDepositAmount.Value = numWithdrawAmount.Value;
        }
    }
}
