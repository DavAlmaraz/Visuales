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
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
            InitializeLoginControls();
        }

        private TextBox txtEmailLogin;
        private TextBox txtPasswordLogin;
        private Button btnLoginLocal;
        private Button btnCreateLocal;
        private Button btnBackLogin;

        private void InitializeLoginControls()
        {
            this.Text = "Mercado Pago - Iniciar sesión";
            this.Size = new Size(780, 500);
            this.BackColor = Color.FromArgb(245, 248, 255);
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Left brand panel (MP blue gradient) ──
            Panel pnlBrand = new Panel
            {
                Size = new Size(320, 462),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, 136, 229)
            };
            pnlBrand.Paint += (s, ev) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    pnlBrand.ClientRectangle,
                    Color.FromArgb(21, 101, 192),
                    Color.FromArgb(66, 165, 245),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    ev.Graphics.FillRectangle(brush, pnlBrand.ClientRectangle);
                }
            };

            Label lblBrandIcon = new Label
            {
                Text = "💳",
                Font = new Font("Segoe UI", 48f),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(110, 80),
                BackColor = Color.Transparent
            };
            Label lblBrandName = new Label
            {
                Text = "Mercado Pago",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(52, 170),
                BackColor = Color.Transparent
            };
            Label lblBrandDesc = new Label
            {
                Text = "Tu dinero rinde más.\nPagá, cobrá e invertí\ndesde un solo lugar.",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(200, 227, 255),
                Size = new Size(260, 70),
                Location = new Point(52, 215),
                BackColor = Color.Transparent
            };
            Label lblBrandFooter = new Label
            {
                Text = "Seguro • Rápido • Simple",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(160, 210, 255),
                AutoSize = true,
                Location = new Point(70, 410),
                BackColor = Color.Transparent
            };
            pnlBrand.Controls.Add(lblBrandIcon);
            pnlBrand.Controls.Add(lblBrandName);
            pnlBrand.Controls.Add(lblBrandDesc);
            pnlBrand.Controls.Add(lblBrandFooter);

            // ── Right login panel ──
            Panel pnlLogin = new Panel
            {
                Size = new Size(444, 462),
                Location = new Point(320, 0),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 136, 229),
                AutoSize = true,
                Location = new Point(40, 60)
            };
            Label lblSub = new Label
            {
                Text = "Ingresa tus datos para continuar",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(120, 144, 156),
                AutoSize = true,
                Location = new Point(40, 98)
            };

            // Email
            Label lblEmail = new Label
            {
                Text = "✉  Correo electrónico",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 90, 100),
                AutoSize = true,
                Location = new Point(40, 150)
            };
            txtEmailLogin = new TextBox
            {
                Location = new Point(40, 174),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 11.5f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 248, 255)
            };

            // Password
            Label lblPass = new Label
            {
                Text = "🔒  Contraseña",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 90, 100),
                AutoSize = true,
                Location = new Point(40, 220)
            };
            txtPasswordLogin = new TextBox
            {
                Location = new Point(40, 244),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 11.5f),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 248, 255)
            };

            // Login button
            btnLoginLocal = new Button
            {
                Text = "Iniciar sesión",
                Location = new Point(40, 300),
                Size = new Size(360, 46),
                BackColor = Color.FromArgb(66, 165, 245),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLoginLocal.FlatAppearance.BorderSize = 0;

            // Separator
            Label sep = new Label
            {
                Text = "─────────  o  ─────────",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(180, 190, 200),
                Size = new Size(360, 20),
                Location = new Point(40, 360),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create + Back buttons
            btnCreateLocal = new Button
            {
                Text = "Crear cuenta nueva",
                Location = new Point(40, 390),
                Size = new Size(175, 40),
                BackColor = Color.FromArgb(232, 245, 233),
                ForeColor = Color.FromArgb(56, 142, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreateLocal.FlatAppearance.BorderColor = Color.FromArgb(165, 214, 167);

            btnBackLogin = new Button
            {
                Text = "Salir",
                Location = new Point(225, 390),
                Size = new Size(175, 40),
                BackColor = Color.FromArgb(255, 243, 224),
                ForeColor = Color.FromArgb(230, 126, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackLogin.FlatAppearance.BorderColor = Color.FromArgb(255, 224, 178);

            btnLoginLocal.Click += BtnLoginLocal_Click;
            btnCreateLocal.Click += BtnCreateLocal_Click;
            btnBackLogin.Click += BtnBackLogin_Click;

            pnlLogin.Controls.Add(lblTitle);
            pnlLogin.Controls.Add(lblSub);
            pnlLogin.Controls.Add(lblEmail);
            pnlLogin.Controls.Add(txtEmailLogin);
            pnlLogin.Controls.Add(lblPass);
            pnlLogin.Controls.Add(txtPasswordLogin);
            pnlLogin.Controls.Add(btnLoginLocal);
            pnlLogin.Controls.Add(sep);
            pnlLogin.Controls.Add(btnCreateLocal);
            pnlLogin.Controls.Add(btnBackLogin);

            this.Controls.Add(pnlBrand);
            this.Controls.Add(pnlLogin);
        }

        private void BtnCreateLocal_Click(object sender, EventArgs e)
        {
            var f = new Form6();
            // when Form6 closes, return to this login form
            f.FormClosed += (s, ev) => { try { this.Show(); } catch { } };
            f.Show();
            this.Hide();
        }

        private void BtnLoginLocal_Click(object sender, EventArgs e)
        {
            var email = txtEmailLogin.Text?.Trim();
            var password = txtPasswordLogin.Text;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Ingrese email y contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var acc = AccountManager.FindByEmailAndPassword(email, password);
            if (acc == null)
            {
                AccountManager.TotalFailedLoginAttempts++;
                // if an account with the email exists, increment its failed attempts
                var maybe = AccountManager.Accounts.FirstOrDefault(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase));
                if (maybe != null) maybe.FailedLoginAttempts++;
                MessageBox.Show("Credenciales inválidas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AccountManager.CurrentAccount = acc;
            // open menu
            var f = new Form8();
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void BtnBackLogin_Click(object sender, EventArgs e)
        {
            // if there is an owner, show it, otherwise just close
            if (this.Owner != null) this.Owner.Show();
            this.Close();
        }
    }
}
