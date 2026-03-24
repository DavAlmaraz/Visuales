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
            this.Text = "Crear cuenta";
            this.Size = new Size(460, 300);
            this.BackColor = Color.FromArgb(255, 230, 0);
            this.Font = new Font("Segoe UI", 10f);
            Label lbl = new Label { Text = "Nombre titular:", Location = new Point(12, 12), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtOwnerCreate = new TextBox { Location = new Point(12, 36), Width = 360 };
            // Phone
            Label lblPhone = new Label { Text = "Teléfono:", Location = new Point(12, 56), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtPhone = new TextBox { Location = new Point(12, 76), Width = 200 };
            // Address
            Label lblAddress = new Label { Text = "Dirección:", Location = new Point(12, 102), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtAddress = new TextBox { Location = new Point(12, 122), Width = 420 };
            // Email and Password on same row
            Label lblEmail = new Label { Text = "Email:", Location = new Point(12, 152), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtEmailCreate = new TextBox { Location = new Point(12, 172), Width = 260 };
            Label lblPassword = new Label { Text = "Contraseña:", Location = new Point(300, 152), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtPasswordCreate = new TextBox { Location = new Point(300, 172), Width = 180, UseSystemPasswordChar = true };
            // buttons placed at bottom
            btnCreate = new Button { Text = "Crear cuenta", Location = new Point(12, 220), Size = new Size(140, 34), BackColor = Color.FromArgb(0,85,165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGoLogin = new Button { Text = "Volver al inicio", Location = new Point(168, 220), Size = new Size(140, 34), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };
            btnBackCreate = new Button { Text = "Atrás", Location = new Point(324, 220), Size = new Size(120, 34), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };

            btnCreate.Click += BtnCreate_Click;
            btnGoLogin.Click += BtnGoLogin_Click;
            btnBackCreate.Click += BtnBackCreate_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(txtOwnerCreate);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhone);
            this.Controls.Add(lblAddress);
            this.Controls.Add(txtAddress);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmailCreate);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPasswordCreate);
            this.Controls.Add(btnCreate);
            this.Controls.Add(btnGoLogin);
            this.Controls.Add(btnBackCreate);
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
