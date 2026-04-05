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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            InitializeAccountCreateControls();
        }

        private TextBox txtOwnerCreate;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtEmailCreate;
        private TextBox txtPasswordCreate;
        private Button btnCreate;
        private Button btnGoLogin;
        private Button btnBackCreate;

        private void InitializeAccountCreateControls()
        {
            this.Text = "Mercado Libre - Crear cuenta";
            this.Size = new Size(820, 520);
            this.BackColor = Color.FromArgb(255, 253, 240);
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Left brand panel (ML gold gradient) ──
            Panel pnlBrand = new Panel
            {
                Size = new Size(300, 482),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(255, 193, 7)
            };
            pnlBrand.Paint += (s, ev) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    pnlBrand.ClientRectangle,
                    Color.FromArgb(255, 179, 0),
                    Color.FromArgb(255, 214, 0),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    ev.Graphics.FillRectangle(brush, pnlBrand.ClientRectangle);
                }
            };

            Label lblBrandIcon = new Label
            {
                Text = "🛒",
                Font = new Font("Segoe UI", 48f),
                ForeColor = Color.FromArgb(50, 40, 10),
                AutoSize = true,
                Location = new Point(100, 60),
                BackColor = Color.Transparent
            };
            Label lblBrandName = new Label
            {
                Text = "Mercado Libre",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 40, 10),
                AutoSize = true,
                Location = new Point(46, 150),
                BackColor = Color.Transparent
            };
            Label lblBrandDesc = new Label
            {
                Text = "Crea tu cuenta y empieza\na comprar, vender y\nadministrar tu dinero.",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(80, 60, 10),
                Size = new Size(240, 60),
                Location = new Point(46, 195),
                BackColor = Color.Transparent
            };

            // Step indicators
            Label lblStep1 = new Label { Text = "① Completa tus datos", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(60, 50, 10), AutoSize = true, Location = new Point(46, 290), BackColor = Color.Transparent };
            Label lblStep2 = new Label { Text = "② Verifica tu email", Font = new Font("Segoe UI", 9f), ForeColor = Color.FromArgb(100, 80, 30), AutoSize = true, Location = new Point(46, 315), BackColor = Color.Transparent };
            Label lblStep3 = new Label { Text = "③ ¡Listo para comprar!", Font = new Font("Segoe UI", 9f), ForeColor = Color.FromArgb(100, 80, 30), AutoSize = true, Location = new Point(46, 340), BackColor = Color.Transparent };

            Label lblBrandFooter = new Label
            {
                Text = "Millones de productos te esperan",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 100, 30),
                AutoSize = true,
                Location = new Point(40, 430),
                BackColor = Color.Transparent
            };

            pnlBrand.Controls.Add(lblBrandIcon);
            pnlBrand.Controls.Add(lblBrandName);
            pnlBrand.Controls.Add(lblBrandDesc);
            pnlBrand.Controls.Add(lblStep1);
            pnlBrand.Controls.Add(lblStep2);
            pnlBrand.Controls.Add(lblStep3);
            pnlBrand.Controls.Add(lblBrandFooter);

            // ── Right form panel ──
            Panel pnlForm = new Panel
            {
                Size = new Size(504, 482),
                Location = new Point(300, 0),
                BackColor = Color.White
            };

            Label lblTitle = new Label
            {
                Text = "Crea tu cuenta",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 166, 35),
                AutoSize = true,
                Location = new Point(36, 30)
            };
            Label lblSub = new Label
            {
                Text = "Completa los siguientes campos",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Location = new Point(36, 65)
            };

            int y = 100;
            int fieldW = 420;
            int halfW = 200;

            // Name (full width)
            Label lblName = new Label { Text = "👤  Nombre completo", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(36, y) };
            txtOwnerCreate = new TextBox { Location = new Point(36, y + 22), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10.5f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 58;

            // Phone + Address (side by side)
            Label lblPhone = new Label { Text = "📞  Teléfono", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(36, y) };
            txtPhone = new TextBox { Location = new Point(36, y + 22), Size = new Size(halfW, 28), Font = new Font("Segoe UI", 10.5f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };

            Label lblAddress = new Label { Text = "📍  Dirección", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(256, y) };
            txtAddress = new TextBox { Location = new Point(256, y + 22), Size = new Size(halfW, 28), Font = new Font("Segoe UI", 10.5f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 58;

            // Email (full width)
            Label lblEmail = new Label { Text = "✉  Correo electrónico", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(36, y) };
            txtEmailCreate = new TextBox { Location = new Point(36, y + 22), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10.5f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 58;

            // Password (full width)
            Label lblPassword = new Label { Text = "🔒  Contraseña", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(36, y) };
            txtPasswordCreate = new TextBox { Location = new Point(36, y + 22), Size = new Size(fieldW, 28), Font = new Font("Segoe UI", 10.5f), UseSystemPasswordChar = true, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 62;

            // Create button (full width, prominent)
            btnCreate = new Button
            {
                Text = "Crear cuenta",
                Location = new Point(36, y),
                Size = new Size(fieldW, 44),
                BackColor = Color.FromArgb(255, 202, 40),
                ForeColor = Color.FromArgb(50, 40, 10),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            y += 52;

            // Bottom buttons row
            btnGoLogin = new Button
            {
                Text = "Ya tengo cuenta",
                Location = new Point(36, y),
                Size = new Size(205, 38),
                BackColor = Color.FromArgb(227, 242, 253),
                ForeColor = Color.FromArgb(30, 136, 229),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGoLogin.FlatAppearance.BorderColor = Color.FromArgb(187, 222, 251);

            btnBackCreate = new Button
            {
                Text = "Cancelar",
                Location = new Point(251, y),
                Size = new Size(205, 38),
                BackColor = Color.FromArgb(255, 238, 238),
                ForeColor = Color.FromArgb(198, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackCreate.FlatAppearance.BorderColor = Color.FromArgb(239, 154, 154);

            btnCreate.Click += BtnCreate_Click;
            btnGoLogin.Click += BtnGoLogin_Click;
            btnBackCreate.Click += BtnBackCreate_Click;

            pnlForm.Controls.Add(lblTitle);
            pnlForm.Controls.Add(lblSub);
            pnlForm.Controls.Add(lblName);
            pnlForm.Controls.Add(txtOwnerCreate);
            pnlForm.Controls.Add(lblPhone);
            pnlForm.Controls.Add(txtPhone);
            pnlForm.Controls.Add(lblAddress);
            pnlForm.Controls.Add(txtAddress);
            pnlForm.Controls.Add(lblEmail);
            pnlForm.Controls.Add(txtEmailCreate);
            pnlForm.Controls.Add(lblPassword);
            pnlForm.Controls.Add(txtPasswordCreate);
            pnlForm.Controls.Add(btnCreate);
            pnlForm.Controls.Add(btnGoLogin);
            pnlForm.Controls.Add(btnBackCreate);

            this.Controls.Add(pnlBrand);
            this.Controls.Add(pnlForm);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var owner = txtOwnerCreate.Text?.Trim();
            if (string.IsNullOrEmpty(owner))
            {
                MessageBox.Show("Ingrese el nombre del titular.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var phone = txtPhone.Text?.Trim();
            var address = txtAddress.Text?.Trim();
            var email = txtEmailCreate.Text?.Trim();
            var password = txtPasswordCreate.Text;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email y contraseña son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var acc = AccountManager.CreateAccount(owner, phone, address, email, password);
            if (acc == null)
            {
                MessageBox.Show("Ya existe una cuenta con ese email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show($"Cuenta creada:\nTitular: {acc.Owner}\nTarjeta: {acc.CardNumber}\nCuenta: {acc.AccountNumber}\nEmail: {acc.Email}", "Cuenta creada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtOwnerCreate.Clear();
            txtPhone.Clear(); txtAddress.Clear(); txtEmailCreate.Clear(); txtPasswordCreate.Clear();
        }

        private void BtnGoLogin_Click(object sender, EventArgs e)
        {
            // Close this form; Form7 (login) will be shown by the caller's FormClosed handler
            if (this.Owner != null)
            {
                this.Owner.Show();
            }
            this.Close();
        }

        private void BtnBackCreate_Click(object sender, EventArgs e)
        {
            if (this.Owner != null) this.Owner.Show();
            this.Close();
        }
    }
}
