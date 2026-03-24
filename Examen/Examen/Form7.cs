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
            this.Text = "Inicio de sesión";
            this.Size = new Size(420, 200);
            this.BackColor = Color.FromArgb(255, 230, 0);
            this.Font = new Font("Segoe UI", 10f);
            Label lbl = new Label { Text = "Email:", Location = new Point(12, 12), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtEmailLogin = new TextBox { Location = new Point(12, 36), Width = 300 };
            Label lbl2 = new Label { Text = "Contraseña:", Location = new Point(12, 66), AutoSize = true, ForeColor = Color.FromArgb(0,85,165) };
            txtPasswordLogin = new TextBox { Location = new Point(12, 90), Width = 300, UseSystemPasswordChar = true };
            btnLoginLocal = new Button { Text = "Iniciar sesión", Location = new Point(320, 86), Size = new Size(80, 28), BackColor = Color.FromArgb(0,85,165), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCreateLocal = new Button { Text = "Crear cuenta", Location = new Point(12, 130), Size = new Size(120, 28), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };
            btnBackLogin = new Button { Text = "Atrás", Location = new Point(140, 130), Size = new Size(80, 28), BackColor = Color.White, ForeColor = Color.FromArgb(0,85,165), FlatStyle = FlatStyle.Flat };

            btnLoginLocal.Click += BtnLoginLocal_Click;
            btnCreateLocal.Click += BtnCreateLocal_Click;
            btnBackLogin.Click += BtnBackLogin_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(txtEmailLogin);
            this.Controls.Add(lbl2);
            this.Controls.Add(txtPasswordLogin);
            this.Controls.Add(btnLoginLocal);
            this.Controls.Add(btnCreateLocal);
            this.Controls.Add(btnBackLogin);
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
