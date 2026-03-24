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
    public partial class Form1 : Form
    {
        int totalClientes = 0;
        int totalAprobados = 0;
        int totalRechazados = 0;
        int totalErrores = 0;

        int autorizadosCDMX = 0;
        int autorizadosForaneo = 0;

        int aprobadasOro = 0;
        int aprobadasPlatinum = 0;
        int aprobadasAmex = 0;

        // Línea de crédito (ingreso del banco)
        double lineaOro = 30000;
        double lineaPlatinum = 50000;
        double lineaAmex = 100000;

        // Anualidad
        double anualOro = 500;
        double anualPlatinum = 1500;
        double anualAmex = 3000;

        // Recuperación anual
        double recuperacionOro = 75000;
        double recuperacionPlatinum = 105000;
        double recuperacionAmex = 210000;


        double ingresoTotalOro = 0;
        double ingresoTotalPlatinum = 0;
        double ingresoTotalAmex = 0;

        double anualTotalOro = 0;
        double anualTotalPlatinum = 0;
        double anualTotalAmex = 0;

        double recuperacionTotalOro = 0;
        double recuperacionTotalPlatinum = 0;
        double recuperacionTotalAmex = 0;

        double gananciaOro = 0;
        double gananciaPlatinum = 0;
        double gananciaAmex = 0;

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            totalClientes++;
            bool aprobado = false;
            List<string> errores = new List<string>();

            // ==============================
            // 🔹 VALIDAR TEXTBOX VACÍOS
            // ==============================
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                errores.Add("Ingrese el nombre");

            if (string.IsNullOrWhiteSpace(textBox2.Text))
                errores.Add("Ingrese los apellidos");

            if (string.IsNullOrWhiteSpace(textBox3.Text))
                errores.Add("Ingrese la dirección");

            if (string.IsNullOrWhiteSpace(textBox4.Text))
                errores.Add("Ingrese la edad");

            if (string.IsNullOrWhiteSpace(textBox5.Text))
                errores.Add("Ingrese el salario");

            if (string.IsNullOrWhiteSpace(textBox6.Text))
                errores.Add("Ingrese los adeudos");

            // ==============================
            // 🔹 VALIDAR COMBOBOX
            // ==============================
            if (comboBox1.SelectedIndex == -1)
                errores.Add("Seleccione tipo de cliente");

            if (comboBox2.SelectedIndex == -1)
                errores.Add("Seleccione tarjeta");

            if (comboBox3.SelectedIndex == -1)
                errores.Add("Seleccione sexo");

            if (comboBox4.SelectedIndex == -1)
                errores.Add("Seleccione tipo de empresa");

            if (comboBox5.SelectedIndex == -1)
                errores.Add("Seleccione nivel de estudios");

            if (comboBox6.SelectedIndex == -1)
                errores.Add("Seleccione estado civil");

            if (comboBox7.SelectedIndex == -1)
                errores.Add("Seleccione número de hijos");

            if (comboBox8.SelectedIndex == -1)
                errores.Add("Seleccione casa propia");

            if (comboBox9.SelectedIndex == -1)
                errores.Add("Seleccione número de vehículos");

            if (comboBox10.SelectedIndex == -1)
                errores.Add("Seleccione modelo de vehículo");

            if (comboBox11.SelectedIndex == -1)
                errores.Add("Seleccione antigüedad laboral");

            if (comboBox12.SelectedIndex == -1)
                errores.Add("Seleccione infonavit");

            if (comboBox13.SelectedIndex == -1)
                errores.Add("Seleccione referencias");

            // ==============================
            // 🔹 VALIDAR FORMATOS NUMÉRICOS
            // ==============================
            int edad;
            double salario;
            double adeudos;
            double valorCasa;

            if (!int.TryParse(textBox4.Text, out edad))
                errores.Add("Edad inválida");

            if (!double.TryParse(textBox5.Text, out salario))
                errores.Add("Salario inválido");

            if (!double.TryParse(textBox6.Text, out adeudos))
                errores.Add("Adeudos inválidos");

            if (!double.TryParse(textBox7.Text, out valorCasa))
                errores.Add("Valor de casa inválido");

            // 👉 Si hay errores de captura, se detiene aquí
            if (errores.Count > 0)
            {
                totalErrores += errores.Count;
                totalRechazados++;

                MessageBox.Show("❌ Errores encontrados:\n\n" + string.Join("\n", errores));
                return;
            }

            // ==============================
            // 🔹 OBTENER DATOS
            // ==============================
            string cliente = comboBox1.Text;
            string tarjeta = comboBox2.Text;
            string sexo = comboBox3.Text;
            string empresa = comboBox4.Text;
            string estudios = comboBox5.Text;
            string estadoCivil = comboBox6.Text;
            int hijos = int.Parse(comboBox7.Text);
            int antiguedad = int.Parse(comboBox11.Text);
            string casa = comboBox8.Text;
            int auto = int.Parse(comboBox9.Text);
            int modeloAuto = int.Parse(comboBox10.Text);
            string infonavit = comboBox12.Text;
            int referencias = int.Parse(comboBox13.Text);


            // ==============================
            // 🔹 VALIDACIÓN DE REQUISITOS
            // EJEMPLO → CDMX ORO
            // ==============================
            // ==============================
            // 🔹 CDMX ORO
            // ==============================
            if (cliente == "CDMX" && tarjeta == "Oro")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 25 && edad <= 50))
                    erroresTarjeta.Add("Edad fuera de rango (CDMX Oro)");

                if (salario < 12000)
                    erroresTarjeta.Add("Salario mínimo requerido 12,000");

                if (adeudos >= 5000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 5,000");

                if (hijos != 1)
                    erroresTarjeta.Add("Debe tener exactamente 1 hijo");

                if (antiguedad < 3)
                    erroresTarjeta.Add("Antigüedad mínima 3 años");

                if (estudios != "Carrera trunca")
                    erroresTarjeta.Add("Nivel requerido: Carrera trunca");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");

                if (auto != 0)
                    erroresTarjeta.Add("No debe tener vehículo");

                if (modeloAuto != 0)
                    erroresTarjeta.Add("No debe tener vehículo");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasOro++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }
            else if (cliente == "CDMX" && tarjeta == "Platinum")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 30 && edad <= 50))
                    erroresTarjeta.Add("Edad fuera de rango (CDMX Platinum)");

                if (salario < 20000)
                    erroresTarjeta.Add("Salario mínimo requerido 20,000");

                if (adeudos >= 10000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 3,000");

                if (hijos > 2 && hijos < 1)
                    erroresTarjeta.Add("Debe tener 1 o 2 hijos");

                if (antiguedad < 3)
                    erroresTarjeta.Add("Antigüedad mínima 3 años");

                if (estudios != "Profesional")
                    erroresTarjeta.Add("Nivel requerido Profesional");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");


                if (auto < 1 && auto > 2)
                    erroresTarjeta.Add("Debe tener 1 o 2 vehículos");

                if (modeloAuto < 2014)
                    erroresTarjeta.Add("Debe tener vehículo modelo 2014 o mayor");

                if (referencias < 1)
                    erroresTarjeta.Add("Debe tener al menos 1 referencia");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasPlatinum++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }
            else if (cliente == "CDMX" && tarjeta == "AmericanExpress")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 35 && edad <= 50))
                    erroresTarjeta.Add("Edad fuera de rango (CDMX Amex)");

                if (salario < 30000)
                    erroresTarjeta.Add("Salario mínimo requerido 30,000");

                if (adeudos >= 20000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 20,000");

                if (hijos > 4)
                    erroresTarjeta.Add("Máximo 4 hijos");

                if (antiguedad < 8)
                    erroresTarjeta.Add("Antigüedad mínima 8 años");

                if (estudios != "Profesional")
                    erroresTarjeta.Add("Nivel requerido Profesional");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");

                if (valorCasa < 2000000)
                    erroresTarjeta.Add("Valor de casa mínimo 2,000,000");

                if (auto < 4)
                    erroresTarjeta.Add("Debe tener 4 vehículos");

                if (modeloAuto < 2015)
                    erroresTarjeta.Add("Debe tener vehículo modelo 2015 o mayor");

                if (referencias < 2)
                    erroresTarjeta.Add("Debe tener al menos 2 referencias");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasAmex++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }
            else if (cliente == "Foráneo" && tarjeta == "Oro")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 25 && edad <= 45))
                    erroresTarjeta.Add("Edad fuera de rango (Foráneo Oro)");

                if (salario < 13000)
                    erroresTarjeta.Add("Salario mínimo requerido 13,000");

                if (adeudos >= 4000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 4,000");

                if (hijos != 1)
                    erroresTarjeta.Add("Máximo 1 hijo");

                if (antiguedad < 5)
                    erroresTarjeta.Add("Antigüedad mínima 5 años");

                if (estudios != "Carrera trunca")
                    erroresTarjeta.Add("Nivel requerido: Carrera trunca");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");

                if (auto != 1)
                    erroresTarjeta.Add("Debe tener 1 vehículo");

                if (modeloAuto < 2013)
                    erroresTarjeta.Add("Debe tener vehículo modelo 2013 o mayor");

                if (referencias < 1)
                    erroresTarjeta.Add("Debe tener al menos 1 referencia");

                if (empresa != "Gobierno")
                    erroresTarjeta.Add("Debe trabajar en empresa de gobierno");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasOro++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }
            else if (cliente == "Foráneo" && tarjeta == "Platinum")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 25 && edad <= 45))
                    erroresTarjeta.Add("Edad fuera de rango (Foráneo Platinum)");

                if (salario < 15000)
                    erroresTarjeta.Add("Salario mínimo requerido 15,000");

                if (adeudos >= 3000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 3,000");

                if (hijos < 1 && hijos > 2)
                    erroresTarjeta.Add("Debe tener 1 o 2 hijos");

                if (antiguedad < 5)
                    erroresTarjeta.Add("Antigüedad mínima 5 años");

                if (estudios != "Profesional")
                    erroresTarjeta.Add("Nivel requerido Profesional");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");

                if (valorCasa < 1000000)
                    erroresTarjeta.Add("Valor de casa mínimo 1,000,000");

                if (auto != 1)
                    erroresTarjeta.Add("Debe tener 1 vehículo");

                if (modeloAuto < 2013)
                    erroresTarjeta.Add("Debe tener vehículo modelo 2015 o mayor");

                if (referencias < 1)
                    erroresTarjeta.Add("Debe tener al menos 2 referencias");

                if (empresa != "Gobierno")
                    erroresTarjeta.Add("Debe trabajar en empresa de gobierno");

                if (infonavit != "Sí")
                    erroresTarjeta.Add("Debe tener infonavit");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasPlatinum++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }
            else if (cliente == "Foráneo" && tarjeta == "AmericanExpress")
            {
                List<string> erroresTarjeta = new List<string>();

                if (!(edad >= 25 && edad <= 55))
                    erroresTarjeta.Add("Edad fuera de rango (Foráneo Amex)");

                if (salario < 25000)
                    erroresTarjeta.Add("Salario mínimo requerido 25,000");

                if (adeudos >= 5000)
                    erroresTarjeta.Add("Adeudos deben ser menores a 5,000");

                if (hijos > 4)
                    erroresTarjeta.Add("Máximo 4 hijos");

                if (antiguedad < 10)
                    erroresTarjeta.Add("Antigüedad mínima 10 años");

                if (estudios != "Profesional")
                    erroresTarjeta.Add("Nivel requerido Profesional");

                if (casa != "Sí")
                    erroresTarjeta.Add("Debe tener casa propia");

                if (valorCasa < 3000000)
                    erroresTarjeta.Add("Valor de casa mínimo 3,000,000");

                if (auto < 1 && auto > 2)
                    erroresTarjeta.Add("Debe tener 1 o 2 vehículos");

                if (modeloAuto < 2015)
                    erroresTarjeta.Add("Debe tener vehículo modelo 2015 o mayor");

                if (referencias < 3)
                    erroresTarjeta.Add("Debe tener al menos 3 referencias");

                if (infonavit != "Sí")
                    erroresTarjeta.Add("Debe tener infonavit");

                if (erroresTarjeta.Count == 0)
                {
                    aprobado = true;
                    aprobadasAmex++;
                }
                else
                    errores.AddRange(erroresTarjeta);
            }

            // ==============================
            // 🔹 FORÁNEO ORO
            // ==============================


            // ==============================
            // 🔹 RESULTADO FINAL
            // ==============================
            if (aprobado)
            {
                totalAprobados++;

                if (cliente == "CDMX")
                    autorizadosCDMX++;
                else
                    autorizadosForaneo++;

                string datosCliente =
                    "✅ SOLICITUD APROBADA\n\n" +
                    "Nombre: " + textBox1.Text + " " + textBox2.Text +
                    "\nDirección: " + textBox3.Text +
                    "\nEdad: " + edad +
                    "\nSexo: " + sexo +
                    "\nTipo cliente: " + cliente +
                    "\nTarjeta: " + tarjeta +
                    "\nSalario: $" + salario.ToString("N2") +
                    "\nAdeudos: $" + adeudos.ToString("N2") +
                    "\nEstado civil: " + estadoCivil +
                    "\nHijos: " + hijos +
                    "\nAntigüedad laboral: " + antiguedad + " años";

                MessageBox.Show(datosCliente, "Resultado");
            }
            else
            {
                totalRechazados++;

                if (errores.Count > 0)
                    totalErrores += errores.Count;

                string mensajeError =
                    "❌ SOLICITUD RECHAZADA\n\n" +
                    "Motivos:\n" +
                    string.Join("\n", errores);

                MessageBox.Show(mensajeError, "Resultado");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ==============================
            // 🔹 CÁLCULOS POR TARJETA
            // ==============================

            // INGRESOS (línea de crédito * aprobadas)
            double ingresoOroTotal = aprobadasOro * lineaOro;
            double ingresoPlatinumTotal = aprobadasPlatinum * lineaPlatinum;
            double ingresoAmexTotal = aprobadasAmex * lineaAmex;

            // ANUALIDADES
            double anualOroTotal = aprobadasOro * anualOro;
            double anualPlatinumTotal = aprobadasPlatinum * anualPlatinum;
            double anualAmexTotal = aprobadasAmex * anualAmex;

            // RECUPERACIÓN (recuperación + anualidad) * total
            double recuperacionOroTotal = aprobadasOro * (recuperacionOro + anualOro);
            double recuperacionPlatinumTotal = aprobadasPlatinum * (recuperacionPlatinum + anualPlatinum);
            double recuperacionAmexTotal = aprobadasAmex * (recuperacionAmex + anualAmex);

            // GANANCIA (recuperación − ingreso)
            double gananciaOroTotal = recuperacionOroTotal - ingresoOroTotal;
            double gananciaPlatinumTotal = recuperacionPlatinumTotal - ingresoPlatinumTotal;
            double gananciaAmexTotal = recuperacionAmexTotal - ingresoAmexTotal;

            // ==============================
            // 🔹 TOTALES GENERALES
            // ==============================

            double ingresoTotal = ingresoOroTotal + ingresoPlatinumTotal + ingresoAmexTotal;
            double anualTotal = anualOroTotal + anualPlatinumTotal + anualAmexTotal;
            double recuperacionTotal = recuperacionOroTotal + recuperacionPlatinumTotal + recuperacionAmexTotal;
            double gananciaTotal = gananciaOroTotal + gananciaPlatinumTotal + gananciaAmexTotal;

            // ==============================
            // 🔹 MENSAJE REPORTE
            // ==============================

            string reporte =
    "📊 REPORTE GENERAL DEL SISTEMA\n\n" +

    "👥 CLIENTES ATENDIDOS\n" +
    "Total evaluados: " + totalClientes +
    "\nAutorizados: " + totalAprobados +
    "\nNo autorizados: " + totalRechazados +

    "\n\n🌎 APROBADOS POR ZONA" +
    "\nCDMX: " + autorizadosCDMX +
    "\nForáneo: " + autorizadosForaneo +

    "\n\n💳 TARJETAS APROBADAS" +
    "\nOro: " + aprobadasOro +
    "\nPlatinum: " + aprobadasPlatinum +
    "\nAmerican Express: " + aprobadasAmex +

    "\n\n💰 INGRESOS POR LÍNEA" +
    "\nOro: $" + ingresoOroTotal.ToString("N2") +
    "\nPlatinum: $" + ingresoPlatinumTotal.ToString("N2") +
    "\nAmerican Express: $" + ingresoAmexTotal.ToString("N2") +

    "\n\n🧾 ANUALIDADES" +
    "\nOro: $" + anualOroTotal.ToString("N2") +
    "\nPlatinum: $" + anualPlatinumTotal.ToString("N2") +
    "\nAmerican Express: $" + anualAmexTotal.ToString("N2") +

    "\n\n📈 RECUPERACIÓN" +
    "\nOro: $" + recuperacionOroTotal.ToString("N2") +
    "\nPlatinum: $" + recuperacionPlatinumTotal.ToString("N2") +
    "\nAmerican Express: $" + recuperacionAmexTotal.ToString("N2") +

    "\n\n🏆 GANANCIA" +
    "\nOro: $" + gananciaOroTotal.ToString("N2") +
    "\nPlatinum: $" + gananciaPlatinumTotal.ToString("N2") +
    "\nAmerican Express: $" + gananciaAmexTotal.ToString("N2") +

    "\n\n📊 RESUMEN GENERAL" +
    "\nTotal ingresos: $" + ingresoTotal.ToString("N2") +
    "\nTotal anualidades: $" + anualTotal.ToString("N2") +
    "\nTotal recuperación: $" + recuperacionTotal.ToString("N2") +
    "\nGANANCIA TOTAL: $" + gananciaTotal.ToString("N2") +

    "\n\n⚠ CONTROL DE ERRORES" +
    "\nErrores detectados: " + totalErrores;

            MessageBox.Show(reporte, "Reporte Final");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
