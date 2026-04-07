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
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    var acct = AccountManager.CurrentAccount;
                    if (acct != null && lblUserBalance != null)
                        lblUserBalance.Text = $"Saldo: ${acct.Balance:0.00}";
                }
            };
        }

        private Button btnOpenStore;
        private Button btnAccountSummaryMenu;
        private Button btnLogout;
        private Button btnAccountOps;
        private Label lblUserBalance;

        private void InitializeMenuControls()
        {
            this.Text = "Panel Principal";
            this.Size = new Size(820, 540);
            this.BackColor = Color.FromArgb(245, 248, 255);
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Left sidebar ──
            Panel sidebar = new Panel
            {
                Size = new Size(220, 502),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(21, 101, 192)
            };
            sidebar.Paint += (s, ev) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    sidebar.ClientRectangle,
                    Color.FromArgb(13, 71, 161),
                    Color.FromArgb(30, 136, 229),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    ev.Graphics.FillRectangle(brush, sidebar.ClientRectangle);
                }
            };

            // User avatar area in sidebar
            Label lblAvatar = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 36f),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(75, 24),
                BackColor = Color.Transparent
            };
            Label lblUserName = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(200, 22),
                Location = new Point(10, 90),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            Label lblUserCard = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(180, 210, 255),
                Size = new Size(200, 18),
                Location = new Point(10, 114),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            lblUserBalance = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 255, 130),
                Size = new Size(200, 22),
                Location = new Point(10, 134),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var acct = AccountManager.CurrentAccount;
            if (acct != null)
            {
                lblUserName.Text = acct.Owner;
                lblUserCard.Text = acct.CardNumber;
                lblUserBalance.Text = $"Saldo: ${acct.Balance:0.00}";
            }

            // Separator in sidebar
            Label sidebarSep = new Label
            {
                Size = new Size(180, 1),
                Location = new Point(20, 168),
                BackColor = Color.FromArgb(60, 130, 220)
            };

            // Sidebar nav label
            Label lblNav = new Label
            {
                Text = "NAVEGACIÓN",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 190, 255),
                AutoSize = true,
                Location = new Point(20, 180),
                BackColor = Color.Transparent
            };

            sidebar.Controls.Add(lblAvatar);
            sidebar.Controls.Add(lblUserName);
            sidebar.Controls.Add(lblUserCard);
            sidebar.Controls.Add(lblUserBalance);
            sidebar.Controls.Add(sidebarSep);
            sidebar.Controls.Add(lblNav);

            // Back button at bottom of sidebar
            btnBackMenu = new Button
            {
                Text = "← Volver",
                Location = new Point(20, 440),
                Size = new Size(180, 36),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(180, 210, 255),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackMenu.FlatAppearance.BorderSize = 0;
            btnBackMenu.Click += (s, e) => { if (this.Owner != null) this.Owner.Show(); this.Close(); };
            sidebar.Controls.Add(btnBackMenu);

            // ── Main content area ──
            Panel mainArea = new Panel
            {
                Size = new Size(584, 502),
                Location = new Point(220, 0),
                BackColor = Color.FromArgb(245, 248, 255)
            };

            // Welcome header in main area
            Label lblWelcome = new Label
            {
                Text = acct != null ? $"¡Hola, {acct.Owner}!" : "Bienvenido",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 80),
                AutoSize = true,
                Location = new Point(30, 24)
            };
            Label lblWelcomeSub = new Label
            {
                Text = "¿Qué deseas hacer hoy?",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(120, 140, 160),
                AutoSize = true,
                Location = new Point(30, 60)
            };
            mainArea.Controls.Add(lblWelcome);
            mainArea.Controls.Add(lblWelcomeSub);

            // 2x2 card grid
            int cardW = 250;
            int cardH = 160;
            int gap = 22;
            int leftX = 30;
            int rightX = leftX + cardW + gap;
            int topY = 100;
            int bottomY = topY + cardH + gap;

            // Card 1: Tienda (ML yellow)
            Panel cardStore = CreateMenuCard(leftX, topY, cardW, cardH,
                "🛒", "Tienda", "Explora productos,\nagrega al carrito y compra",
                Color.FromArgb(255, 249, 196), Color.FromArgb(255, 202, 40), Color.FromArgb(90, 70, 10));
            cardStore.Click += (s, e) => BtnOpenStore_Click(s, e);
            foreach (Control c in cardStore.Controls) c.Click += (s, e) => BtnOpenStore_Click(s, e);

            // Card 2: Operaciones (MP blue)
            Panel cardOps = CreateMenuCard(rightX, topY, cardW, cardH,
                "💳", "Operaciones", "Ingresos, transferencias,\nretiros y recargas",
                Color.FromArgb(227, 242, 253), Color.FromArgb(66, 165, 245), Color.FromArgb(21, 101, 192));
            cardOps.Click += (s, e) => BtnAccountOps_Click(s, e);
            foreach (Control c in cardOps.Controls) c.Click += (s, e) => BtnAccountOps_Click(s, e);

            // Card 3: Informe global (green pastel)
            Panel cardReport = CreateMenuCard(leftX, bottomY, cardW, cardH,
                "📊", "Informe global", "Resumen de todas las\ncuentas del sistema",
                Color.FromArgb(232, 245, 233), Color.FromArgb(102, 187, 106), Color.FromArgb(27, 94, 32));
            cardReport.Click += (s, e) => BtnAccountSummaryMenu_Click(s, e);
            foreach (Control c in cardReport.Controls) c.Click += (s, e) => BtnAccountSummaryMenu_Click(s, e);

            // Card 4: Cerrar sesión (warm coral)
            Panel cardLogout = CreateMenuCard(rightX, bottomY, cardW, cardH,
                "🚪", "Cerrar sesión", "Salir de forma segura\nde tu cuenta",
                Color.FromArgb(255, 235, 238), Color.FromArgb(229, 115, 115), Color.FromArgb(183, 28, 28));
            cardLogout.Click += (s, e) => BtnLogout_Click(s, e);
            foreach (Control c in cardLogout.Controls) c.Click += (s, e) => BtnLogout_Click(s, e);

            mainArea.Controls.Add(cardStore);
            mainArea.Controls.Add(cardOps);
            mainArea.Controls.Add(cardReport);
            mainArea.Controls.Add(cardLogout);

            // Hidden buttons (for event handler references)
            btnOpenStore = new Button { Visible = false };
            btnAccountSummaryMenu = new Button { Visible = false };
            btnAccountOps = new Button { Visible = false };
            btnLogout = new Button { Visible = false };

            btnOpenStore.Click += BtnOpenStore_Click;
            btnAccountSummaryMenu.Click += BtnAccountSummaryMenu_Click;
            btnAccountOps.Click += BtnAccountOps_Click;
            btnLogout.Click += BtnLogout_Click;

            this.Controls.Add(sidebar);
            this.Controls.Add(mainArea);
        }

        private Panel CreateMenuCard(int x, int y, int w, int h, string icon, string title, string desc, Color bgColor, Color accentColor, Color textColor)
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
                // top accent bar
                ev.Graphics.FillRectangle(new SolidBrush(accentColor), 0, 0, w, 5);
                // border
                ev.Graphics.DrawRectangle(new Pen(accentColor, 1), 0, 0, w - 1, h - 1);
            };
            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 28f),
                AutoSize = true,
                Location = new Point(20, 18),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = textColor,
                AutoSize = true,
                Location = new Point(20, 70),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Label lblD = new Label
            {
                Text = desc,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 100, 100),
                Size = new Size(w - 40, 46),
                Location = new Point(20, 100),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            card.Controls.Add(lblIcon);
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
            var depositKeys = new List<string> { "Oxxo", "Seven Eleven", "Dinero desde el extranjero (USD)", "Dinero desde el extranjero (EUR)" };
            depositKeys.AddRange(depositTotals.Keys.Where(k => !depositKeys.Contains(k) && !k.StartsWith("Depósito bancario") && k != "Transferencia"));
            depositKeys = depositKeys.Distinct().ToList();
            int combinedBancTransOps = 0;
            decimal combinedBancTransVal = 0m;
            foreach (var k in depositCounts.Keys)
            {
                if (k.StartsWith("Depósito bancario") || k == "Transferencia")
                {
                    combinedBancTransOps += depositCounts[k];
                    combinedBancTransVal += depositTotals.ContainsKey(k) ? depositTotals[k] : 0m;
                }
            }
            foreach (var k in depositKeys)
            {
                int cnt = depositCounts.ContainsKey(k) ? depositCounts[k] : 0;
                decimal tot = depositTotals.ContainsKey(k) ? depositTotals[k] : 0m;
                sb.AppendLine($"   • {k}: {cnt} ops — ${tot:0.00}");
            }
            if (combinedBancTransOps > 0)
                sb.AppendLine($"   • Depósitos bancarios y Transferencias: {combinedBancTransOps} ops — ${combinedBancTransVal:0.00}");

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
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  RECARGAS CELULAR (por compañía)");
            sb.AppendLine("──────────────────────────────────────");
            var rechargeCounts = new Dictionary<string, int>();
            var rechargeTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.RechargeCounts.Keys)
                {
                    if (!rechargeCounts.ContainsKey(k)) rechargeCounts[k] = 0;
                    rechargeCounts[k] += ac.RechargeCounts[k];
                }
                foreach (var k in ac.RechargeTotals.Keys)
                {
                    if (!rechargeTotals.ContainsKey(k)) rechargeTotals[k] = 0m;
                    rechargeTotals[k] += ac.RechargeTotals[k];
                }
            }
            var companies = new List<string> { "AT&T", "Unefón", "Telcel", "Movistar", "Bait" };
            foreach (var c in companies)
            {
                int cnt = rechargeCounts.ContainsKey(c) ? rechargeCounts[c] : 0;
                decimal tot = rechargeTotals.ContainsKey(c) ? rechargeTotals[c] : 0m;
                sb.AppendLine($"   • {c}: {cnt} recargas — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  PAGOS DE SERVICIOS (por tipo)");
            sb.AppendLine("──────────────────────────────────────");
            var serviceCounts = new Dictionary<string, int>();
            var serviceTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.ServicePaymentCounts.Keys)
                {
                    if (!serviceCounts.ContainsKey(k)) serviceCounts[k] = 0;
                    serviceCounts[k] += ac.ServicePaymentCounts[k];
                }
                foreach (var k in ac.ServicePaymentTotals.Keys)
                {
                    if (!serviceTotals.ContainsKey(k)) serviceTotals[k] = 0m;
                    serviceTotals[k] += ac.ServicePaymentTotals[k];
                }
            }
            var services = new List<string> { "Otros servicios", "Internet", "Agua", "Luz" };
            foreach (var svc in services)
            {
                int cnt = serviceCounts.ContainsKey(svc) ? serviceCounts[svc] : 0;
                decimal tot = serviceTotals.ContainsKey(svc) ? serviceTotals[svc] : 0m;
                sb.AppendLine($"   • {svc}: {cnt} pagos — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  RECARGAS DE TAG (por proveedor)");
            sb.AppendLine("──────────────────────────────────────");
            var tagCounts = new Dictionary<string, int>();
            var tagTotals = new Dictionary<string, decimal>();
            foreach (var ac in accounts)
            {
                foreach (var k in ac.TagRechargeCounts.Keys)
                {
                    if (!tagCounts.ContainsKey(k)) tagCounts[k] = 0;
                    tagCounts[k] += ac.TagRechargeCounts[k];
                }
                foreach (var k in ac.TagRechargeTotals.Keys)
                {
                    if (!tagTotals.ContainsKey(k)) tagTotals[k] = 0m;
                    tagTotals[k] += ac.TagRechargeTotals[k];
                }
            }
            var tagProviders = new List<string> { "Pase", "Televia" };
            foreach (var tp in tagProviders)
            {
                int cnt = tagCounts.ContainsKey(tp) ? tagCounts[tp] : 0;
                decimal tot = tagTotals.ContainsKey(tp) ? tagTotals[tp] : 0m;
                sb.AppendLine($"   • {tp}: {cnt} recargas — ${tot:0.00}");
            }

            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════");
            sb.AppendLine("  TOTALES DEL SISTEMA");
            sb.AppendLine("══════════════════════════════════════");
            decimal totalIngresos = depositTotals.Values.Sum();
            decimal totalTransferencias = transferTotals.Values.Sum();
            decimal totalRetiros = withdrawTotals.Values.Sum();
            decimal totalRecargas = rechargeTotals.Values.Sum();
            decimal totalServicios = serviceTotals.Values.Sum();
            decimal totalTagRecargas = tagTotals.Values.Sum();
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
            decimal totalComprasML = accounts.Sum(a => a.TotalPurchased);
            decimal totalComisionesML = accounts.Sum(a => a.TotalCommissionsPaid);
            int totalProductosComprados = accounts.Sum(a => a.TotalPurchaseItems);
            int totalConComision = accounts.Sum(a => a.PurchaseItemsWithCommission);
            int totalSinComision = accounts.Sum(a => a.PurchaseItemsWithoutCommission);

            sb.AppendLine($"  Ingresos totales:         ${totalIngresos:0.00}");
            sb.AppendLine($"  Transferencias totales:   ${totalTransferencias:0.00}");
            sb.AppendLine($"  Retiros totales:          ${totalRetiros:0.00}");
            sb.AppendLine($"  Recargas celular totales: ${totalRecargas:0.00}");
            sb.AppendLine($"  Pagos servicios totales:  ${totalServicios:0.00}");
            sb.AppendLine($"  Recargas Tag totales:     ${totalTagRecargas:0.00}");
            sb.AppendLine($"  Comisiones retiro totales:${totalComisiones:0.00}");
            sb.AppendLine($"  Suma de saldos (todos):   ${totalSaldos:0.00}");
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine("  COMPRAS EN MERCADO LIBRE (GLOBAL)");
            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine($"  Total productos comprados:     {totalProductosComprados}");
            sb.AppendLine($"  Productos con comisión:        {totalConComision}");
            sb.AppendLine($"  Productos sin comisión:        {totalSinComision}");
            sb.AppendLine($"  Total gastado en productos:    ${totalComprasML:0.00}");
            sb.AppendLine($"  Total comisiones peso/volumen: ${totalComisionesML:0.00}");
            sb.AppendLine($"  Total gastado (con comisiones):${(totalComprasML + totalComisionesML):0.00}");

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
