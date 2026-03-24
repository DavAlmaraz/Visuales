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
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
            InitializeMenuControls();
        }

        private Button btnOpenStore;
        private Button btnAccountSummaryMenu;
        private Button btnLogout;
        private Button btnAccountOps;

        private void InitializeMenuControls()
        {
            this.Text = "Menú principal";
            this.Size = new Size(460, 260);
            this.BackColor = Color.FromArgb(255, 230, 0);
            this.Font = new Font("Segoe UI", 10f);
            Label lbl = new Label { Text = "Menú principal", Location = new Point(12, 12), Font = new Font("Segoe UI", 14f, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            btnOpenStore = new Button { Text = "Abrir tienda (compras)", Location = new Point(10, 50), Size = new Size(380, 36) };
            btnOpenStore.BackColor = Color.White; btnOpenStore.FlatStyle = FlatStyle.Flat; btnOpenStore.ForeColor = Color.FromArgb(0,85,165);
            btnAccountSummaryMenu = new Button { Text = "Resumen de mi cuenta", Location = new Point(12, 100), Size = new Size(210, 40) };
            btnAccountOps = new Button { Text = "Operaciones de cuenta", Location = new Point(236, 100), Size = new Size(210, 40) };
            btnAccountSummaryMenu.BackColor = Color.White; btnAccountSummaryMenu.FlatStyle = FlatStyle.Flat; btnAccountSummaryMenu.ForeColor = Color.FromArgb(0,85,165);
            btnAccountOps.BackColor = Color.White; btnAccountOps.FlatStyle = FlatStyle.Flat; btnAccountOps.ForeColor = Color.FromArgb(0,85,165);
            btnLogout = new Button { Text = "Cerrar sesión", Location = new Point(12, 150), Size = new Size(434, 40) };
            btnLogout.BackColor = Color.FromArgb(0,85,165); btnLogout.FlatStyle = FlatStyle.Flat; btnLogout.ForeColor = Color.White;

            btnOpenStore.Click += BtnOpenStore_Click;
            btnAccountSummaryMenu.Click += BtnAccountSummaryMenu_Click;
            btnAccountOps.Click += BtnAccountOps_Click;
            btnLogout.Click += BtnLogout_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(btnOpenStore);
            this.Controls.Add(btnAccountSummaryMenu);
            this.Controls.Add(btnAccountOps);
            this.Controls.Add(btnLogout);
            AddBackButton();
        }

        private void BtnOpenStore_Click(object sender, EventArgs e)
        {
            var f = new Form5();
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void BtnAccountSummaryMenu_Click(object sender, EventArgs e)
        {
            // Global report across all accounts
            var accounts = AccountManager.Accounts;
            var sb = new StringBuilder();
            sb.AppendLine("INFORME GLOBAL DEL SISTEMA");
            sb.AppendLine();
            sb.AppendLine($"Total cuentas creadas: {accounts.Count}");
            sb.AppendLine($"Intentos fallidos de inicio de sesión (global): {AccountManager.TotalFailedLoginAttempts}");
            sb.AppendLine();

            // Aggregate deposits
            var depositCounts = new Dictionary<string, int>();
            var depositTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.DepositCounts.Keys)
                {
                    if (!depositCounts.ContainsKey(k)) depositCounts[k] = 0;
                    depositCounts[k] += ac.DepositCounts[k];
                }
                foreach (var k in ac.DepositTotals.Keys)
                {
                    if (!depositTotals.ContainsKey(k)) depositTotals[k] = 0m;
                    depositTotals[k] += ac.DepositTotals[k];
                }
            }

            sb.AppendLine("INGRESOS (por tipo):");
            // ensure common keys appear even if zero
            var depositKeys = new List<string> { "Oxxo/7Eleven", "Depósito bancario", "Transferencia", "Dinero desde el extranjero (USD)" };
            // include any bank-specific deposit keys
            depositKeys.AddRange(depositTotals.Keys.Where(k => k.StartsWith("Depósito bancario - ") && !depositKeys.Contains(k)));
            depositKeys = depositKeys.Distinct().ToList();
            foreach (var k in depositKeys)
            {
                int cnt = depositCounts.ContainsKey(k) ? depositCounts[k] : 0;
                decimal tot = depositTotals.ContainsKey(k) ? depositTotals[k] : 0m;
                sb.AppendLine($" - {k}: {cnt} operaciones - ${tot:0.00}");
            }

            sb.AppendLine();
            // Aggregate transfers
            var transferCounts = new Dictionary<string, int>();
            var transferTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.TransferCounts.Keys)
                {
                    if (!transferCounts.ContainsKey(k)) transferCounts[k] = 0;
                    transferCounts[k] += ac.TransferCounts[k];
                }
                foreach (var k in ac.TransferTotals.Keys)
                {
                    if (!transferTotals.ContainsKey(k)) transferTotals[k] = 0m;
                    transferTotals[k] += ac.TransferTotals[k];
                }
            }
            sb.AppendLine("TRANSFERENCIAS (por banco):");
            var banks = new List<string> { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" };
            banks.AddRange(transferTotals.Keys.Where(k => !banks.Contains(k)));
            foreach (var b in banks)
            {
                int cnt = transferCounts.ContainsKey(b) ? transferCounts[b] : 0;
                decimal tot = transferTotals.ContainsKey(b) ? transferTotals[b] : 0m;
                sb.AppendLine($" - {b}: {cnt} transferencias - ${tot:0.00}");
            }

            sb.AppendLine();
            // Aggregate withdrawals
            var withdrawCounts = new Dictionary<string, int>();
            var withdrawTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.WithdrawCounts.Keys)
                {
                    if (!withdrawCounts.ContainsKey(k)) withdrawCounts[k] = 0;
                    withdrawCounts[k] += ac.WithdrawCounts[k];
                }
                foreach (var k in ac.WithdrawTotals.Keys)
                {
                    if (!withdrawTotals.ContainsKey(k)) withdrawTotals[k] = 0m;
                    withdrawTotals[k] += ac.WithdrawTotals[k];
                }
            }
            sb.AppendLine("RETIROS (por proveedor):");
            var providers = new List<string> { "Mercado Pago Express", "7-Eleven", "Soriana", "Chedraui", "Aurrera", "Walmart" };
            foreach (var p in providers)
            {
                int cnt = withdrawCounts.ContainsKey(p) ? withdrawCounts[p] : 0;
                decimal tot = withdrawTotals.ContainsKey(p) ? withdrawTotals[p] : 0m;
                sb.AppendLine($" - {p}: {cnt} retiros - ${tot:0.00}");
            }

            sb.AppendLine();
            // Totals and commissions
            decimal totalIngresos = depositTotals.Values.Sum();
            decimal totalTransferencias = transferTotals.Values.Sum();
            decimal totalRetiros = withdrawTotals.Values.Sum();
            decimal totalComisiones = 0m;
            foreach (var p in providers)
            {
                int cnt = withdrawCounts.ContainsKey(p) ? withdrawCounts[p] : 0;
                decimal comm = 0m;
                switch (p)
                {
                    case "Chedraui": comm = 20m; break;
                    case "Aurrera": comm = 15m; break;
                    case "Walmart": comm = 15m; break;
                    default: comm = 0m; break;
                }
                totalComisiones += comm * cnt;
            }
            decimal totalSaldos = accounts.Sum(a => a.Balance);

            sb.AppendLine("TOTALES DEL SISTEMA:");
            sb.AppendLine($" - Ingresos totales: ${totalIngresos:0.00}");
            sb.AppendLine($" - Transferencias totales: ${totalTransferencias:0.00}");
            sb.AppendLine($" - Retiros totales: ${totalRetiros:0.00}");
            sb.AppendLine($" - Comisiones totales: ${totalComisiones:0.00}");
            sb.AppendLine($" - Suma de saldos (todos): ${totalSaldos:0.00}");

            MessageBox.Show(sb.ToString(), "Informe global", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            AccountManager.Logout();
            MessageBox.Show("Sesión cerrada.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (this.Owner != null) this.Owner.Show();
            this.Close();
        }

        private void BtnAccountOps_Click(object sender, EventArgs e)
        {
            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("No hay sesión iniciada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var f = new Form9();
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        // Back button to return to previous form
        private Button btnBackMenu;
        private void AddBackButton()
        {
            btnBackMenu = new Button { Text = "Atrás", Location = new Point(12, 200), Size = new Size(80, 28), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };
            btnBackMenu.Click += (s, e) => { if (this.Owner != null) this.Owner.Show(); this.Close(); };
            this.Controls.Add(btnBackMenu);
        }
    }
}
