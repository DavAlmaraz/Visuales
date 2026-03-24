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
        private ListBox lstAcctHistory;

        private void InitializeAccountOperations()
        {
            // Mercado Libre inspired colors
            this.Text = "Operaciones de cuenta";
            this.Size = new Size(740, 520);
            this.BackColor = Color.FromArgb(255, 230, 0); // ML yellow
            this.Font = new Font("Segoe UI", 9f);

            lblAcct = new Label { Text = "Cuenta: (sin sesión)", Location = new Point(12, 12), AutoSize = true, ForeColor = Color.FromArgb(0, 85, 165) };

            // Deposit group
            var grpDeposit = new GroupBox { Text = "Ingresar dinero", Location = new Point(12, 40), Size = new Size(340, 160), BackColor = Color.White };
            cmbDepositType = new ComboBox { Location = new Point(12, 22), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDepositType.Items.AddRange(new object[] { "Oxxo/7Eleven", "Depósito bancario", "Transferencia", "Dinero desde el extranjero (USD)", "Dinero desde el extranjero (EUR)" });
            cmbDepositType.SelectedIndex = 0;
            var lblBank = new Label { Text = "Banco (si depósito bancario):", Location = new Point(12, 86), AutoSize = true };
            cmbDepositBank = new ComboBox { Location = new Point(12, 104), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            // populate with same banks as transfer
            var banks = new object[] { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" };
            cmbDepositBank.Items.AddRange(banks);
            if (cmbDepositBank.Items.Count > 0) cmbDepositBank.SelectedIndex = 0;
            var lblDepAmount = new Label { Text = "Monto:", Location = new Point(12, 54), AutoSize = true };
            numDepositAmount = new NumericUpDown { Location = new Point(70, 52), Width = 120, Minimum = 1, Maximum = 100000, Value = 100 };
            btnDeposit = new Button { Text = "Ingresar", Location = new Point(12, 130), Size = new Size(250, 30), BackColor = Color.FromArgb(0, 85, 165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDeposit.Click += BtnDeposit_Click;
            grpDeposit.Controls.Add(cmbDepositType);
            grpDeposit.Controls.Add(lblDepAmount);
            grpDeposit.Controls.Add(numDepositAmount);
            grpDeposit.Controls.Add(lblBank);
            grpDeposit.Controls.Add(cmbDepositBank);
            grpDeposit.Controls.Add(btnDeposit);

            cmbDepositType.SelectedIndexChanged += (s, e) =>
            {
                // enable bank combo only for bank deposit
                var isBank = cmbDepositType.SelectedItem.ToString().Contains("Depósito bancario");
                cmbDepositBank.Enabled = isBank;
            };

            // Transfer group
            var grpTransfer = new GroupBox { Text = "Transferir a banco", Location = new Point(372, 40), Size = new Size(340, 160), BackColor = Color.White };
            cmbTransferBank = new ComboBox { Location = new Point(12, 22), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTransferBank.Items.AddRange(new object[] { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" });
            cmbTransferBank.SelectedIndex = 0;
            var lblTrAmount = new Label { Text = "Monto:", Location = new Point(12, 54), AutoSize = true };
            numTransferAmount = new NumericUpDown { Location = new Point(70, 52), Width = 120, Minimum = 1, Maximum = 100000, Value = 100 };
            btnTransfer = new Button { Text = "Transferir", Location = new Point(12, 86), Size = new Size(250, 30), BackColor = Color.FromArgb(0, 85, 165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            // Hook transfer, using numTransferAmount value
            btnTransfer.Click += (s, e) => { numTransferAmount_ValueChanged(s, e); BtnTransfer_Click(s, e); };
            grpTransfer.Controls.Add(cmbTransferBank);
            grpTransfer.Controls.Add(lblTrAmount);
            grpTransfer.Controls.Add(numTransferAmount);
            grpTransfer.Controls.Add(btnTransfer);

            // Withdraw group
            var grpWithdraw = new GroupBox { Text = "Retirar efectivo", Location = new Point(12, 210), Size = new Size(340, 200), BackColor = Color.White };
            cmbWithdrawProvider = new ComboBox { Location = new Point(12, 48), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbWithdrawProvider.Items.AddRange(new object[] { "Mercado Pago Express", "7-Eleven", "Soriana", "Chedraui", "Aurrera", "Walmart" });
            cmbWithdrawProvider.SelectedIndex = 0;
            var lblWAmount = new Label { Text = "Monto:", Location = new Point(12, 88), AutoSize = true };
            numWithdrawAmount = new NumericUpDown { Location = new Point(70, 86), Width = 120, Minimum = 1, Maximum = 100000, Value = 100 };
            btnWithdraw = new Button { Text = "Retirar", Location = new Point(210, 82), Size = new Size(120, 30), BackColor = Color.FromArgb(0, 85, 165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnWithdraw.Click += (s, e) => { numWithdrawAmount_ValueChanged(s, e); BtnWithdraw_Click(s, e); };
            grpWithdraw.Controls.Add(new Label { Text = "Proveedor:", Location = new Point(12, 28), AutoSize = true });
            grpWithdraw.Controls.Add(cmbWithdrawProvider);
            grpWithdraw.Controls.Add(lblWAmount);
            grpWithdraw.Controls.Add(numWithdrawAmount);
            grpWithdraw.Controls.Add(btnWithdraw);

            // Summary button
            btnSummary = new Button { Text = "Ver informe de cuenta", Location = new Point(12, 430), Size = new Size(200, 34), BackColor = Color.FromArgb(0, 85, 165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSummary.Click += BtnSummary_Click;

            btnBackOps = new Button { Text = "Atrás", Location = new Point(232, 430), Size = new Size(100, 34), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };
            btnBackOps.Click += (s,e) => { if (this.Owner!=null) { this.Owner.Show(); } this.Close(); };

            // History list (ensure initialized)
            lstAcctHistory = new ListBox { Location = new Point(372, 210), Size = new Size(340, 200), BorderStyle = BorderStyle.FixedSingle };
            Label lblHist = new Label { Text = "Historial reciente", Location = new Point(372, 188), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };

            this.Controls.Add(lblAcct);
            this.Controls.Add(grpDeposit);
            this.Controls.Add(grpTransfer);
            this.Controls.Add(grpWithdraw);
            this.Controls.Add(btnSummary);
            this.Controls.Add(btnBackOps);
            this.Controls.Add(lblHist);
            this.Controls.Add(lstAcctHistory);

            UpdateAccountLabel();
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
            if (type.Contains("Depósito bancario") && !string.IsNullOrEmpty(bankName)) key = $"Depósito bancario - {bankName}";
            if (!a.DepositCounts.ContainsKey(key)) a.DepositCounts[key] = 0;
            if (!a.DepositTotals.ContainsKey(key)) a.DepositTotals[key] = 0m;
            a.DepositCounts[key]++;
            a.DepositTotals[key] += amountToAdd;
            UpdateAccountLabel();
            // record history entry
            lstAcctHistory.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm} - Ingreso {key} ${amountToAdd:0.00}");
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
            lstAcctHistory.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm} - Transferencia a {bank} ${amount:0.00}");
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
            lstAcctHistory.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm} - Retiro {prov} ${amount:0.00} (Com: ${commission:0.00})");
            MessageBox.Show($"Retiro realizado.\nCuenta: {a.Owner} - {a.CardNumber}\nProveedor: {prov}\nMonto retirado: ${amount:0.00}\nComisión: ${commission:0.00}\nSaldo actual: ${a.Balance:0.00}", "Retiro registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSummary_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null) { MessageBox.Show("Inicie sesión para ver el resumen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            var sb = new StringBuilder();
            sb.AppendLine("INFORME DE CUENTA");
            sb.AppendLine();
            sb.AppendLine($"Titular: {a.Owner}");
            sb.AppendLine($"Tarjeta: {a.CardNumber}");
            sb.AppendLine($"Email: {a.Email}");
            sb.AppendLine($"Teléfono: {a.Phone}");
            sb.AppendLine();
            sb.AppendLine("TOTALES:");
            // commissions total
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
            sb.AppendLine($"- Comisiones totales: ${totalCommissions:0.00}");
            sb.AppendLine($"- Ingresos totales: ${a.TotalDeposited:0.00}");
            sb.AppendLine($"- Transferencias totales: ${a.TotalTransferred:0.00}");
            sb.AppendLine($"- Retiros totales: ${a.TotalWithdrawn:0.00}");
            sb.AppendLine($"- Saldo actual: ${a.Balance:0.00}");
            sb.AppendLine();
            sb.AppendLine("DETALLE DE INGRESOS:");
            foreach (var k in a.DepositCounts.Keys) { decimal val = a.DepositTotals.ContainsKey(k) ? a.DepositTotals[k] : 0m; sb.AppendLine($" - {k}: {a.DepositCounts[k]} operaciones - ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("DETALLE DE TRANSFERENCIAS:");
            foreach (var k in a.TransferCounts.Keys) { decimal val = a.TransferTotals.ContainsKey(k) ? a.TransferTotals[k] : 0m; sb.AppendLine($" - {k}: {a.TransferCounts[k]} operaciones - ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine("DETALLE DE RETIROS:");
            foreach (var k in a.WithdrawCounts.Keys) { decimal val = a.WithdrawTotals.ContainsKey(k) ? a.WithdrawTotals[k] : 0m; sb.AppendLine($" - {k}: {a.WithdrawCounts[k]} retiros - ${val:0.00}"); }
            sb.AppendLine();
            sb.AppendLine($"Total cuentas creadas: {AccountManager.Accounts.Count}");
            sb.AppendLine($"Intentos fallidos de inicio de sesión (global): {AccountManager.TotalFailedLoginAttempts}");
            MessageBox.Show(sb.ToString(), "Informe de cuenta", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
