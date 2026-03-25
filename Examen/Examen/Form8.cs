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
            this.Text = "Panel Principal";
            this.Size = new Size(600, 480);
            this.BackColor = Color.FromArgb(245, 248, 255); // very light blue-white
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Top header panel with MP blue gradient look
            Panel headerPanel = new Panel
            {
                Size = new Size(600, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(66, 165, 245),
                Dock = DockStyle.Top
            };
            Label lblWelcome = new Label
            {
                Text = "Bienvenido",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 12)
            };
            Label lblUser = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(227, 242, 253),
                AutoSize = true,
                Location = new Point(24, 48)
            };
            // Show current user info
            var acct = AccountManager.CurrentAccount;
            if (acct != null)
            {
                lblWelcome.Text = $"Hola, {acct.Owner}";
                lblUser.Text = $"Tarjeta: {acct.CardNumber}  |  Saldo: ${acct.Balance:0.00}";
            }
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblUser);

            // Navigation cards area
            int cardY = 100;
            int cardW = 250;
            int cardH = 110;
            int gap = 20;
            int leftX = 36;
            int rightX = leftX + cardW + gap;

            // Card 1: Tienda (ML yellow)
            Panel cardStore = CreateMenuCard(leftX, cardY, cardW, cardH,
                "🛒  Tienda", "Explora productos y compra",
                Color.FromArgb(255, 249, 196), Color.FromArgb(255, 202, 40), Color.FromArgb(90, 70, 10));
            cardStore.Click += (s, e) => BtnOpenStore_Click(s, e);
            foreach (Control c in cardStore.Controls) c.Click += (s, e) => BtnOpenStore_Click(s, e);

            // Card 2: Operaciones (MP blue)
            Panel cardOps = CreateMenuCard(rightX, cardY, cardW, cardH,
                "💳  Operaciones", "Ingresos, transferencias y retiros",
                Color.FromArgb(227, 242, 253), Color.FromArgb(66, 165, 245), Color.FromArgb(21, 101, 192));
            cardOps.Click += (s, e) => BtnAccountOps_Click(s, e);
            foreach (Control c in cardOps.Controls) c.Click += (s, e) => BtnAccountOps_Click(s, e);

            // Card 3: Informe global (green pastel)
            Panel cardReport = CreateMenuCard(leftX, cardY + cardH + gap, cardW, cardH,
                "📊  Informe global", "Resumen de todas las cuentas",
                Color.FromArgb(232, 245, 233), Color.FromArgb(102, 187, 106), Color.FromArgb(27, 94, 32));
            cardReport.Click += (s, e) => BtnAccountSummaryMenu_Click(s, e);
            foreach (Control c in cardReport.Controls) c.Click += (s, e) => BtnAccountSummaryMenu_Click(s, e);

            // Card 4: Cerrar sesión (warm coral)
            Panel cardLogout = CreateMenuCard(rightX, cardY + cardH + gap, cardW, cardH,
                "🚪  Cerrar sesión", "Salir de tu cuenta",
                Color.FromArgb(255, 235, 238), Color.FromArgb(229, 115, 115), Color.FromArgb(183, 28, 28));
            cardLogout.Click += (s, e) => BtnLogout_Click(s, e);
            foreach (Control c in cardLogout.Controls) c.Click += (s, e) => BtnLogout_Click(s, e);

            // Hidden buttons (keep for event handlers)
            btnOpenStore = new Button { Visible = false };
            btnAccountSummaryMenu = new Button { Visible = false };
            btnAccountOps = new Button { Visible = false };
            btnLogout = new Button { Visible = false };

            btnOpenStore.Click += BtnOpenStore_Click;
            btnAccountSummaryMenu.Click += BtnAccountSummaryMenu_Click;
            btnAccountOps.Click += BtnAccountOps_Click;
            btnLogout.Click += BtnLogout_Click;

            // Back link at bottom
            btnBackMenu = new Button
            {
                Text = "← Volver",
                Location = new Point(24, 390),
                Size = new Size(120, 34),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(66, 165, 245),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackMenu.FlatAppearance.BorderSize = 0;
            btnBackMenu.Click += (s, e) => { if (this.Owner != null) this.Owner.Show(); this.Close(); };

            this.Controls.Add(headerPanel);
            this.Controls.Add(cardStore);
            this.Controls.Add(cardOps);
            this.Controls.Add(cardReport);
            this.Controls.Add(cardLogout);
            this.Controls.Add(btnBackMenu);
        }

        private Panel CreateMenuCard(int x, int y, int w, int h, string title, string desc, Color bgColor, Color accentColor, Color textColor)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bgColor,
                Cursor = Cursors.Hand
            };
            card.Paint += (s, ev) =>
            {
                // left accent bar
                ev.Graphics.FillRectangle(new SolidBrush(accentColor), 0, 0, 5, h);
                // border
                ev.Graphics.DrawRectangle(new Pen(accentColor, 1), 0, 0, w - 1, h - 1);
            };
            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = textColor,
                AutoSize = true,
                Location = new Point(16, 20),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Label lblD = new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(16, 56),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            card.Controls.Add(lblT);
            card.Controls.Add(lblD);
            return card;
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
            sb.AppendLine($"  Cuentas creadas: {accounts.Count}");
            sb.AppendLine($"  Intentos fallidos (global): {AccountManager.TotalFailedLoginAttempts}");
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════");
            sb.AppendLine("  INGRESOS (por tipo)");
            sb.AppendLine("══════════════════════════════════════");

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
            var depositKeys = new List<string> { "Oxxo/7Eleven", "Depósito bancario", "Transferencia", "Dinero desde el extranjero (USD)" };
            depositKeys.AddRange(depositTotals.Keys.Where(k => k.StartsWith("Depósito bancario - ") && !depositKeys.Contains(k)));
            depositKeys = depositKeys.Distinct().ToList();
            foreach (var k in depositKeys)
            {
                int cnt = depositCounts.ContainsKey(k) ? depositCounts[k] : 0;
                decimal tot = depositTotals.ContainsKey(k) ? depositTotals[k] : 0m;
                sb.AppendLine($"   • {k}: {cnt} ops — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  TRANSFERENCIAS (por banco)");
            sb.AppendLine("──────────────────────────────────────");
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
            var banks = new List<string> { "BBVA", "Santander", "Banorte", "HSBC", "Banamex" };
            banks.AddRange(transferTotals.Keys.Where(k => !banks.Contains(k)));
            foreach (var b in banks)
            {
                int cnt = transferCounts.ContainsKey(b) ? transferCounts[b] : 0;
                decimal tot = transferTotals.ContainsKey(b) ? transferTotals[b] : 0m;
                sb.AppendLine($"   • {b}: {cnt} transferencias — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  RETIROS (por proveedor)");
            sb.AppendLine("──────────────────────────────────────");
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
            var providers = new List<string> { "Mercado Pago Express", "7-Eleven", "Soriana", "Chedraui", "Aurrera", "Walmart" };
            foreach (var p in providers)
            {
                int cnt = withdrawCounts.ContainsKey(p) ? withdrawCounts[p] : 0;
                decimal tot = withdrawTotals.ContainsKey(p) ? withdrawTotals[p] : 0m;
                sb.AppendLine($"   • {p}: {cnt} retiros — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════");
            sb.AppendLine("  TOTALES DEL SISTEMA");
            sb.AppendLine("══════════════════════════════════════");
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

            sb.AppendLine($"  Ingresos totales:         ${totalIngresos:0.00}");
            sb.AppendLine($"  Transferencias totales:   ${totalTransferencias:0.00}");
            sb.AppendLine($"  Retiros totales:          ${totalRetiros:0.00}");
            sb.AppendLine($"  Comisiones totales:       ${totalComisiones:0.00}");
            sb.AppendLine($"  Suma de saldos (todos):   ${totalSaldos:0.00}");

            ShowStyledReport("Informe Global del Sistema", sb.ToString(), Color.FromArgb(102, 187, 106));
        }

        private void ShowStyledReport(string title, string content, Color accentColor)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.Size = new Size(540, 500);
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
                    Size = new Size(500, 350),
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
                    Location = new Point(392, 420),
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

        // Back button field (used in InitializeMenuControls)
        private Button btnBackMenu;
    }
}
