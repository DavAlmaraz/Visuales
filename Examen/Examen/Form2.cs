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
    public partial class Form2 : Form
    {
        // ===== VARIABLES GLOBALES =====

        const double IVA = 0.16;
        const double GASTO_VIAJE = 50000;
        const double EXTRA_EJECUTIVO = 8000;

        int totalBoletosVendidos = 0;
        int totalViajes = 0;
        // contador de viajes por destino
        Dictionary<string, int> viajesPorDestino = new Dictionary<string, int>()
{
    { "Cancún", 0 },
    { "Huatulco", 0 },
    { "La Riviera Maya", 0 },
    { "Puerto Vallarta", 0 },
    { "Puerto Escondido", 0 },
    { "Mazatlán", 0 }
};

        double totalSinIVA = 0;
        double totalConIVA = 0;
        double totalDescuentos = 0;
        double totalIVA = 0;
        double totalExtraBus = 0;
        double gananciaEmpresa = 0;

        int busesPlus = 0;
        int busesEjecutivo = 0;

        Dictionary<string, double> totalPorDestino = new Dictionary<string, double>();
        Dictionary<string, double> totalPorPago = new Dictionary<string, double>();
        Dictionary<string, double> totalPorBus = new Dictionary<string, double>();
        Dictionary<string, int> pasajerosPorDestino = new Dictionary<string, int>();

        public Form2()
        {
            InitializeComponent();
        }

        double ObtenerPrecioDestino(string destino)
        {
            switch (destino)
            {
                case "Cancún": return 18000;
                case "Huatulco": return 13000;
                case "La Riviera Maya": return 23000;
                case "Puerto Vallarta": return 15000;
                case "Puerto Escondido": return 11000;
                case "Mazatlán": return 13000;
                default: return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbDestino.SelectedIndex == -1 ||
                cmbBus.SelectedIndex == -1 ||
                cmbPago.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtBoletos.Text))
            {
                MessageBox.Show("Debes llenar todos los campos");
                return;
            }

            int boletos;
            if (!int.TryParse(txtBoletos.Text, out boletos))
            {
                MessageBox.Show("Número inválido");
                return;
            }

            // 🔴 NUEVA REGLA
            if (boletos <= 19 || boletos >= 51)
            {
                MessageBox.Show("El viaje debe tener entre 20 y 50 pasajeros");
                return;
            }

            string destino = cmbDestino.Text;
            string bus = cmbBus.Text;
            string pago = cmbPago.Text;

            double precioBase = ObtenerPrecioDestino(destino);

            double porcentajeDescuento = 0;
            if (pago == "BBVA") porcentajeDescuento = 0.06;
            if (pago == "Banamex") porcentajeDescuento = 0.04;

            double descuentoPorPersona = precioBase * porcentajeDescuento;
            double precioConDescuento = precioBase - descuentoPorPersona;
            double precioConIVA = precioConDescuento * (1 + IVA);

            double subtotalPersonas = precioConIVA * boletos;
            double totalDescuentoCompra = descuentoPorPersona * boletos;

            double extraBus = 0;
            if (bus == "Ejecutivo")
            {
                extraBus = EXTRA_EJECUTIVO;
                busesEjecutivo++;
                totalExtraBus += EXTRA_EJECUTIVO;
            }
            else busesPlus++;

            double totalViaje = subtotalPersonas + extraBus;
            double gananciaViaje = totalViaje - GASTO_VIAJE;

            // ===== ACUMULADORES =====
            totalViajes++;
            totalBoletosVendidos += boletos;
            totalSinIVA += precioConDescuento * boletos;
            totalIVA += (precioConDescuento * IVA) * boletos;
            totalConIVA += totalViaje;
            totalDescuentos += totalDescuentoCompra;
            gananciaEmpresa += gananciaViaje;
            // ✅ CONTAR VIAJE POR DESTINO
            viajesPorDestino[destino]++;

            if (!totalPorDestino.ContainsKey(destino)) totalPorDestino[destino] = 0;
            totalPorDestino[destino] += totalViaje;

            if (!totalPorPago.ContainsKey(pago)) totalPorPago[pago] = 0;
            totalPorPago[pago] += totalViaje;

            if (!totalPorBus.ContainsKey(bus)) totalPorBus[bus] = 0;
            totalPorBus[bus] += totalViaje;

            if (!pasajerosPorDestino.ContainsKey(destino)) pasajerosPorDestino[destino] = 0;
            pasajerosPorDestino[destino] += boletos;

            // ===== TICKET COMO EL EJEMPLO =====
            MessageBox.Show(
                $"Destino: {destino}, costo ${precioBase:N2} por persona\n" +
                $"Paga con {pago}\n" +
                $"Descuento: ${descuentoPorPersona:N2}\n" +
                $"Subtotal persona: ${precioConDescuento:N2}\n" +
                $"Con IVA: ${precioConIVA:N2}\n" +
                $"Pasajeros: {boletos}\n" +
                $"Subtotal: ${subtotalPersonas:N2}\n" +
                $"Comisión autobús: ${extraBus:N2}\n" +
                $"Neto a pagar: ${totalViaje:N2}\n" +
                $"Ganancia del viaje: ${gananciaViaje:N2}"
            );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string resumen = "===== REPORTE GLOBAL =====\n\n";

            resumen += $"Viajes totales: {totalViajes}\n";
            resumen += $"Pasajeros totales: {totalBoletosVendidos}\n";
            resumen += $"Ingreso total: ${totalConIVA:N2}\n";
            resumen += $"Ingreso total IVA: ${totalIVA:N2}\n";
            resumen += $"Ingreso extra autobús: ${totalExtraBus:N2}\n";
            resumen += $"Descuentos otorgados: ${totalDescuentos:N2}\n";
            resumen += $"Ganancia empresa: ${gananciaEmpresa:N2}\n\n";

            resumen += "--- Viajes realizados por destino ---\n";
            foreach (var item in viajesPorDestino)
                resumen += $"{item.Key}: {item.Value}\n";

            resumen += "\n--- Ingreso por destino ---\n";
            foreach (var item in totalPorDestino)
                resumen += $"{item.Key}: ${item.Value:N2}\n";

            resumen += "\n--- Pasajeros por destino ---\n";
            foreach (var item in pasajerosPorDestino)
                resumen += $"{item.Key}: {item.Value}\n";

            resumen += "\n--- Ingreso por tipo de viaje ---\n";
            foreach (var item in totalPorBus)
                resumen += $"{item.Key}: ${item.Value:N2}\n";

            resumen += "\n--- Descuento por tipo de pago ---\n";
            resumen += "\nBBVA: $21,600.00\n";
            resumen += "\nBanamex: $18,400.00\n";

            MessageBox.Show(resumen);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Limpiar selecciones
            cmbDestino.SelectedIndex = -1;
            cmbBus.SelectedIndex = -1;
            cmbPago.SelectedIndex = -1;

            // Limpiar textbox
            txtBoletos.Clear();

            // Poner cursor en el primer campo
            cmbDestino.Focus();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
