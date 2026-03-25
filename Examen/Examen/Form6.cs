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
            this.Size = new Size(540, 520);
            this.BackColor = Color.FromArgb(255, 253, 231); // pastel ML yellow
            this.Font = new Font("Segoe UI", 10f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Card panel
            Panel card = new Panel
            {
                Size = new Size(460, 420),
                Location = new Point(32, 40),
                BackColor = Color.White,
                Padding = new Padding(24)
            };
            card.Paint += (s, ev) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 236, 179), 2), rect);
            };

            // Gold stripe
            Panel stripe = new Panel
            {
                Size = new Size(460, 6),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(255, 202, 40) // ML gold
            };
            card.Controls.Add(stripe);

            // Title
            Label lblTitle = new Label
            {
                Text = "Mercado Libre",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(245, 166, 35),
                AutoSize = true,
                Location = new Point(24, 20)
            };
            Label lblSub = new Label
            {
                Text = "Crea tu cuenta",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Location = new Point(24, 50)
            };

            int y = 80;
            int fieldW = 400;

            // Name
            Label lblName = new Label { Text = "Nombre completo", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(24, y) };
            txtOwnerCreate = new TextBox { Location = new Point(24, y + 20), Size = new Size(fieldW, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 52;

            // Phone
            Label lblPhone = new Label { Text = "Teléfono", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(24, y) };
            txtPhone = new TextBox { Location = new Point(24, y + 20), Size = new Size(180, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };

            // Address (same row)
            Label lblAddress = new Label { Text = "Dirección", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(220, y) };
            txtAddress = new TextBox { Location = new Point(220, y + 20), Size = new Size(204, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 52;

            // Email
            Label lblEmail = new Label { Text = "Correo electrónico", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(24, y) };
            txtEmailCreate = new TextBox { Location = new Point(24, y + 20), Size = new Size(fieldW, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 52;

            // Password
            Label lblPassword = new Label { Text = "Contraseña", Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(90, 80, 60), AutoSize = true, Location = new Point(24, y) };
            txtPasswordCreate = new TextBox { Location = new Point(24, y + 20), Size = new Size(fieldW, 26), Font = new Font("Segoe UI", 10f), UseSystemPasswordChar = true, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(255, 253, 245) };
            y += 56;

            // Buttons
            btnCreate = new Button
            {
                Text = "Crear cuenta",
                Location = new Point(24, y),
                Size = new Size(fieldW, 40),
                BackColor = Color.FromArgb(255, 202, 40),
                ForeColor = Color.FromArgb(50, 40, 10),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            y += 48;

            btnGoLogin = new Button
            {
                Text = "Ya tengo cuenta",
                Location = new Point(24, y),
                Size = new Size(193, 34),
                BackColor = Color.FromArgb(227, 242, 253),
                ForeColor = Color.FromArgb(30, 136, 229),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGoLogin.FlatAppearance.BorderColor = Color.FromArgb(187, 222, 251);

            btnBackCreate = new Button
            {
                Text = "Cancelar",
                Location = new Point(231, y),
                Size = new Size(193, 34),
                BackColor = Color.FromArgb(255, 238, 238),
                ForeColor = Color.FromArgb(198, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackCreate.FlatAppearance.BorderColor = Color.FromArgb(239, 154, 154);

            btnCreate.Click += BtnCreate_Click;
            btnGoLogin.Click += BtnGoLogin_Click;
            btnBackCreate.Click += BtnBackCreate_Click;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(lblName);
            card.Controls.Add(txtOwnerCreate);
            card.Controls.Add(lblPhone);
            card.Controls.Add(txtPhone);
            card.Controls.Add(lblAddress);
            card.Controls.Add(txtAddress);
            card.Controls.Add(lblEmail);
            card.Controls.Add(txtEmailCreate);
            card.Controls.Add(lblPassword);
            card.Controls.Add(txtPasswordCreate);
            card.Controls.Add(btnCreate);
            card.Controls.Add(btnGoLogin);
            card.Controls.Add(btnBackCreate);
            this.Controls.Add(card);
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
