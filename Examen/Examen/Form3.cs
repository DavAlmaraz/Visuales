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
    public partial class Form3 : Form
    {
        // ================================
        // CLASE TRANSACCION
        // ================================
        public class Transaccion
        {
            public int NumeroCaja { get; set; }
            public double Monto { get; set; }
            public DateTime Fecha { get; set; }
        }

        List<Transaccion> lista = new List<Transaccion>();

        public Form3()
        {
            InitializeComponent();
        }

        // ================================
        // BUSCAR TEXTBOX SEGURO
        // ================================
        private TextBox ObtenerTextBox(string nombre)
        {
            return this.Controls.Find(nombre, true).FirstOrDefault() as TextBox;
        }

        // ================================
        // LEER DATOS
        // ================================
        private bool LeerDatos()
        {
            lista.Clear();

            bool valido = true;

            // BLOQUE SUPERIOR
            for (int i = 0; i < 3; i++)
            {
                if (!AgregarTransaccion(1 + i, 4 + i, 7 + i, 10 + i, 13 + i))
                    valido = false;
            }

            // BLOQUE INFERIOR
            for (int i = 0; i < 3; i++)
            {
                if (!AgregarTransaccion(16 + i, 19 + i, 22 + i, 25 + i, 28 + i))
                    valido = false;
            }

            return valido;
        }

        // ================================
        // VALIDAR Y AGREGAR
        // ================================
        private bool AgregarTransaccion(int caja, int monto, int hora, int minuto, int segundo)
        {
            TextBox txtCaja = ObtenerTextBox("textBox" + caja);
            TextBox txtMonto = ObtenerTextBox("textBox" + monto);
            TextBox txtHora = ObtenerTextBox("textBox" + hora);
            TextBox txtMin = ObtenerTextBox("textBox" + minuto);
            TextBox txtSeg = ObtenerTextBox("textBox" + segundo);

            if (txtCaja == null || txtMonto == null || txtHora == null || txtMin == null || txtSeg == null)
            {
                MessageBox.Show("Error: No se encontraron los TextBox.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCaja.Text) &&
                string.IsNullOrWhiteSpace(txtMonto.Text) &&
                string.IsNullOrWhiteSpace(txtHora.Text) &&
                string.IsNullOrWhiteSpace(txtMin.Text) &&
                string.IsNullOrWhiteSpace(txtSeg.Text))
                return true;

            int numeroCaja;
            double montoTransaccion;
            int h, m, s;

            if (!int.TryParse(txtCaja.Text, out numeroCaja))
            {
                MessageBox.Show("Número de caja inválido.");
                return false;
            }

            if (!double.TryParse(txtMonto.Text, out montoTransaccion))
            {
                MessageBox.Show("Monto inválido.");
                return false;
            }

            if (!int.TryParse(txtHora.Text, out h) || h < 0 || h > 23)
            {
                MessageBox.Show("Hora inválida (0-23).");
                return false;
            }

            if (!int.TryParse(txtMin.Text, out m) || m < 0 || m > 59)
            {
                MessageBox.Show("Minuto inválido (0-59).");
                return false;
            }

            if (!int.TryParse(txtSeg.Text, out s) || s < 0 || s > 59)
            {
                MessageBox.Show("Segundo inválido (0-59).");
                return false;
            }

            lista.Add(new Transaccion
            {
                NumeroCaja = numeroCaja,
                Monto = montoTransaccion,
                Fecha = new DateTime(2024, 1, 1, h, m, s)
            });

            return true;
        }

        // ================================
        // MOSTRAR DATOS
        // ================================
        private void MostrarDatos()
        {
            if (lista.Count == 0)
            {
                MessageBox.Show("No hay datos para ordenar.");
                return;
            }

            int index = 0;

            void Escribir(int caja, int monto, int hora, int minuto, int segundo)
            {
                if (index >= lista.Count) return;

                ObtenerTextBox("textBox" + caja).Text = lista[index].NumeroCaja.ToString();
                ObtenerTextBox("textBox" + monto).Text = lista[index].Monto.ToString();
                ObtenerTextBox("textBox" + hora).Text = lista[index].Fecha.Hour.ToString();
                ObtenerTextBox("textBox" + minuto).Text = lista[index].Fecha.Minute.ToString();
                ObtenerTextBox("textBox" + segundo).Text = lista[index].Fecha.Second.ToString();

                index++;
            }

            for (int i = 0; i < 3; i++)
                Escribir(1 + i, 4 + i, 7 + i, 10 + i, 13 + i);

            for (int i = 0; i < 3; i++)
                Escribir(16 + i, 19 + i, 22 + i, 25 + i, 28 + i);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!LeerDatos()) return;

            lista = lista.OrderBy(x => x.NumeroCaja).ToList();
            MostrarDatos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!LeerDatos()) return;

            lista = lista.OrderBy(x => x.Monto).ToList();
            MostrarDatos();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!LeerDatos()) return;

            lista = lista.OrderBy(x => x.Fecha).ToList();
            MostrarDatos();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
