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
            this.Size = new Size(520, 480);
            this.BackColor = Color.FromArgb(227, 242, 253); // pastel MP blue
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Card panel centered
            Panel card = new Panel
            {
                Size = new Size(400, 360),
                Location = new Point(50, 50),
                BackColor = Color.White,
                Padding = new Padding(30)
            };
            card.Paint += (s, ev) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(187, 222, 251), 2), rect);
            };

            // Header stripe
            Panel stripe = new Panel
            {
                Size = new Size(400, 6),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(66, 165, 245) // MP accent blue
            };
            card.Controls.Add(stripe);

            // Logo / Title
            Label lblTitle = new Label
            {
                Text = "Mercado Pago",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 136, 229),
                AutoSize = true,
                Location = new Point(30, 24)
            };

            Label lblSub = new Label
            {
                Text = "Ingresa a tu cuenta",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(120, 144, 156),
                AutoSize = true,
                Location = new Point(30, 60)
            };

            // Email field
            Label lblEmail = new Label
            {
                Text = "Correo electrónico",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 90, 100),
                AutoSize = true,
                Location = new Point(30, 100)
            };
            txtEmailLogin = new TextBox
            {
                Location = new Point(30, 122),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 11f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Password field
            Label lblPass = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(69, 90, 100),
                AutoSize = true,
                Location = new Point(30, 162)
            };
            txtPasswordLogin = new TextBox
            {
                Location = new Point(30, 184),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 11f),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Login button - full width accent
            btnLoginLocal = new Button
            {
                Text = "Iniciar sesión",
                Location = new Point(30, 230),
                Size = new Size(340, 42),
                BackColor = Color.FromArgb(66, 165, 245),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLoginLocal.FlatAppearance.BorderSize = 0;

            // Separator line
            Label sep = new Label
            {
                Size = new Size(340, 1),
                Location = new Point(30, 286),
                BackColor = Color.FromArgb(207, 216, 220)
            };

            // Create account and back as link-style
            btnCreateLocal = new Button
            {
                Text = "Crear cuenta nueva",
                Location = new Point(30, 298),
                Size = new Size(165, 36),
                BackColor = Color.FromArgb(232, 245, 233),
                ForeColor = Color.FromArgb(56, 142, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreateLocal.FlatAppearance.BorderColor = Color.FromArgb(165, 214, 167);

            btnBackLogin = new Button
            {
                Text = "Salir",
                Location = new Point(205, 298),
                Size = new Size(165, 36),
                BackColor = Color.FromArgb(255, 243, 224),
                ForeColor = Color.FromArgb(230, 126, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackLogin.FlatAppearance.BorderColor = Color.FromArgb(255, 224, 178);

            btnLoginLocal.Click += BtnLoginLocal_Click;
            btnCreateLocal.Click += BtnCreateLocal_Click;
            btnBackLogin.Click += BtnBackLogin_Click;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(lblEmail);
            card.Controls.Add(txtEmailLogin);
            card.Controls.Add(lblPass);
            card.Controls.Add(txtPasswordLogin);
            card.Controls.Add(btnLoginLocal);
            card.Controls.Add(sep);
            card.Controls.Add(btnCreateLocal);
            card.Controls.Add(btnBackLogin);
            this.Controls.Add(card);
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
