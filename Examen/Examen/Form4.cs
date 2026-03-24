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
    public partial class Form4 : Form
    {
        // Product definition
        private readonly List<(string Name, decimal Price)> products = new List<(string, decimal)>();

        private class CartItem
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Subtotal => Price * Quantity;
            public override string ToString() => $"{Name} x{Quantity} - ${Subtotal:0.00}";
        }

        private readonly List<CartItem> cart = new List<CartItem>();

        // Dynamic controls for payment
        private GroupBox grpPago;
        private RadioButton rdoContado;
        private RadioButton rdoTarjetaMSI;
        private RadioButton rdoMercadoPago;
        private ComboBox cmbContadoOpciones;
        private ComboBox cmbMSIMeses;
        private ComboBox cmbMPOpciones;
        private Button btnResumen;

        public Form4()
        {
            InitializeComponent();
            InitializeProducts();
            InitializeDynamicControls();
            HookEvents();
        }

        private void InitializeProducts()
        {
            // 30 predetermined products with prices
            products.Add(("Camisa", 299.99m));
            products.Add(("Pantalon", 599.50m));
            products.Add(("Zapatos", 1200.00m));
            products.Add(("Calcetines (pack)", 99.90m));
            products.Add(("Gorra", 149.00m));
            products.Add(("Sudadera", 899.00m));
            products.Add(("Chaqueta", 1599.00m));
            products.Add(("Cinturon", 249.99m));
            products.Add(("Bolso", 1299.00m));
            products.Add(("Reloj", 2499.00m));
            products.Add(("Auriculares", 799.00m));
            products.Add(("Televisor 32\"", 4999.00m));
            products.Add(("Microondas", 1399.00m));
            products.Add(("Licudora", 699.00m));
            products.Add(("Telefono", 8999.00m));
            products.Add(("Tablet", 5999.00m));
            products.Add(("Teclado", 399.00m));
            products.Add(("Mouse", 199.00m));
            products.Add(("Monitor", 2499.00m));
            products.Add(("Impresora", 1899.00m));
            products.Add(("Silla", 1299.00m));
            products.Add(("Mesa", 2199.00m));
            products.Add(("Lampara", 249.00m));
            products.Add(("Cobija", 349.00m));
            products.Add(("Almohada", 199.00m));
            products.Add(("Juego de sartenes", 799.00m));
            products.Add(("Set de cuchillos", 499.00m));
            products.Add(("Colchon", 4599.00m));
            products.Add(("Estufa", 2699.00m));
            products.Add(("Refrigerador", 8999.00m));

            // populate combo box
            foreach (var p in products)
            {
                cmbProductos.Items.Add($"{p.Name} - ${p.Price:0.00}");
            }

            if (cmbProductos.Items.Count > 0)
                cmbProductos.SelectedIndex = 0;
        }

        private void InitializeDynamicControls()
        {
            // create payment group inside existing grpContado location
            grpPago = new GroupBox
            {
                Text = "Método de pago",
                Location = grpContado.Location,
                Size = grpContado.Size
            };

            int left = 10;
            int top = 20;

            rdoContado = new RadioButton { Text = "Contado", Location = new Point(left, top), AutoSize = true };
            rdoTarjetaMSI = new RadioButton { Text = "Tarjeta crédito (MSI)", Location = new Point(left + 120, top), AutoSize = true };
            rdoMercadoPago = new RadioButton { Text = "Mercado Pago", Location = new Point(left + 320, top), AutoSize = true };

            // Sub options
            cmbContadoOpciones = new ComboBox { Location = new Point(left, top + 30), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbContadoOpciones.Items.AddRange(new object[] { "Deposito", "Tarjeta débito", "Tarjeta crédito" });
            cmbContadoOpciones.SelectedIndex = 0;

            cmbMSIMeses = new ComboBox { Location = new Point(left + 220, top + 30), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMSIMeses.Items.AddRange(new object[] { "3", "6", "12" });
            cmbMSIMeses.SelectedIndex = 0;

            cmbMPOpciones = new ComboBox { Location = new Point(left + 360, top + 30), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMPOpciones.Items.AddRange(new object[] { "Contado", "Crédito (normal)", "3 meses (10% mensual)", "6 meses (10% mensual)" });
            cmbMPOpciones.SelectedIndex = 0;

            btnResumen = new Button { Text = "Ver resumen", Location = new Point(10, 60), Size = new Size(150, 30) };

            grpPago.Controls.Add(rdoContado);
            grpPago.Controls.Add(rdoTarjetaMSI);
            grpPago.Controls.Add(rdoMercadoPago);
            grpPago.Controls.Add(cmbContadoOpciones);
            grpPago.Controls.Add(cmbMSIMeses);
            grpPago.Controls.Add(cmbMPOpciones);
            grpPago.Controls.Add(btnResumen);

            this.Controls.Add(grpPago);

            // default visibility
            cmbContadoOpciones.Visible = true;
            cmbMSIMeses.Visible = false;
            cmbMPOpciones.Visible = false;
            rdoContado.Checked = true;
        }

        private void HookEvents()
        {
            btnAgregar.Click += BtnAgregar_Click;
            btnVerCarrito.Click += BtnVerCarrito_Click;
            btnResumen.Click += BtnResumen_Click;
            rdoContado.CheckedChanged += PaymentRadioChanged;
            rdoTarjetaMSI.CheckedChanged += PaymentRadioChanged;
            rdoMercadoPago.CheckedChanged += PaymentRadioChanged;
        }

        private void PaymentRadioChanged(object sender, EventArgs e)
        {
            cmbContadoOpciones.Visible = rdoContado.Checked;
            cmbMSIMeses.Visible = rdoTarjetaMSI.Checked;
            cmbMPOpciones.Visible = rdoMercadoPago.Checked;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int qty = (int)numCantidad.Value;
            if (qty <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor que 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selected = products[cmbProductos.SelectedIndex];
            var item = new CartItem { Name = selected.Name, Price = selected.Price, Quantity = qty };
            cart.Add(item);
            lstCarrito.Items.Add(item.ToString());
        }

        private void BtnVerCarrito_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = cart.Sum(i => i.Subtotal);

            // determine payment selection
            string paymentDetail = "";
            int months = 0;
            decimal totalToPay = total;
            List<(int Number, decimal Amount)> installments = null;

            if (rdoContado.Checked)
            {
                paymentDetail = $"Contado - {cmbContadoOpciones.SelectedItem}";
                // no installment, pay total
            }
            else if (rdoTarjetaMSI.Checked)
            {
                paymentDetail = "Tarjeta crédito (MSI)";
                months = int.Parse(cmbMSIMeses.SelectedItem.ToString());
                // no interest
                decimal per = Math.Round(total / months, 2);
                installments = new List<(int, decimal)>();
                for (int i = 1; i <= months; i++) installments.Add((i, per));
            }
            else if (rdoMercadoPago.Checked)
            {
                var opt = cmbMPOpciones.SelectedItem.ToString();
                paymentDetail = "Mercado Pago - " + opt;
                if (opt.StartsWith("3 meses"))
                {
                    months = 3;
                    // 10% mensual simple interest
                    decimal factor = 1 + 0.10m * months;
                    totalToPay = Math.Round(total * factor, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
                else if (opt.StartsWith("6 meses"))
                {
                    months = 6;
                    decimal factor = 1 + 0.10m * months;
                    totalToPay = Math.Round(total * factor, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
                else
                {
                    // Contado or Credito (normal) no extra interest and no installments
                }
            }

            // build message
            var sb = new StringBuilder();
            sb.AppendLine($"Total carrito: ${total:0.00}");
            sb.AppendLine($"Método: {paymentDetail}");
            if (installments != null)
            {
                sb.AppendLine($"Total a pagar: ${totalToPay:0.00}");
                sb.AppendLine("Mensualidades:");
                foreach (var it in installments)
                {
                    sb.AppendLine($"{it.Number} -> ${it.Amount:0.00}");
                }
            }
            else
            {
                // if MercadoPago with interest we already updated totalToPay
                if (totalToPay != total)
                    sb.AppendLine($"Total a pagar (con intereses): ${totalToPay:0.00}");
                else
                    sb.AppendLine($"Total a pagar: ${total:0.00}");
            }

            MessageBox.Show(sb.ToString(), "Resumen de pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnResumen_Click(object sender, EventArgs e)
        {
            // Build summary like image: totals and counts per payment option
            if (cart.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = cart.Sum(i => i.Subtotal);
            int totalProducts = cart.Sum(i => i.Quantity);

            string contadoType = rdoContado.Checked ? cmbContadoOpciones.SelectedItem.ToString() : "-";
            string msi = rdoTarjetaMSI.Checked ? cmbMSIMeses.SelectedItem.ToString() : "-";
            string mp = rdoMercadoPago.Checked ? cmbMPOpciones.SelectedItem.ToString() : "-";

            var sb = new StringBuilder();
            sb.AppendLine("RESUMEN");
            sb.AppendLine($"Total productos comprados: {totalProducts}");
            sb.AppendLine($"Total a pagar: ${total:0.00}");
            sb.AppendLine("\nDetalles de método seleccionado:");
            sb.AppendLine($"Contado opción: {contadoType}");
            sb.AppendLine($"Tarjeta MSI meses: {msi}");
            sb.AppendLine($"MercadoPago opción: {mp}");

            MessageBox.Show(sb.ToString(), "Resumen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
