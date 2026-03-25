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
    public partial class Form5 : Form
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

        // (Account functionality moved to AccountManager.cs)

        private void PopulateSalesList()
        {
            lstVentas.Items.Clear();
            visibleSaleIndexes.Clear();
            // Only show active sales (with items)
            int displayIndex = 1;
            for (int i = 0; i < sales.Count; i++)
            {
                var s = sales[i];
                if (s.ItemsCount <= 0) continue;
                lstVentas.Items.Add($"Venta #{displayIndex} - {s.Category} - Productos:{s.ItemsCount} - Envío:{s.ShippingType}");
                visibleSaleIndexes.Add(i);
                displayIndex++;
            }
            // clear selected items view when rebuilding list
            lstVentaItems.Items.Clear();
        }

        private void LstVentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstVentaItems.Items.Clear();
            int idx = lstVentas.SelectedIndex;
            if (idx < 0 || idx >= visibleSaleIndexes.Count) return;
            int saleIndex = visibleSaleIndexes[idx];
            var s = sales[saleIndex];
            foreach (var it in s.Items)
                lstVentaItems.Items.Add($"{it.Name} x{it.Quantity} - ${it.Subtotal:0.00}");
        }

        private void BtnCancelarVenta_Click(object sender, EventArgs e)
        {
            int idx = lstVentas.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("Seleccione una venta para cancelar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (idx < 0 || idx >= visibleSaleIndexes.Count)
            {
                MessageBox.Show("Selección inválida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int saleIndex = visibleSaleIndexes[idx];
            var sale = sales[saleIndex];
            string reason = cmbCancelReasons.SelectedItem?.ToString() ?? "Sin motivo";

            // record cancellation list will be updated below

            if (chkCancelarVentaTodo.Checked)
            {
                // refund full sale
                decimal refund;
                // If sale had interest (credits), refund only product amount (bank keeps interests)
                if (sale.InterestAmount > 0)
                    refund = sale.AmountBeforeInterest;
                else
                    refund = sale.TotalPaid;
                var namesAll = sale.Items.Select(i => i.Name + " x" + i.Quantity).ToList();
                // no preserved interest accounting for cancelled sale
                // add cancellation record
                cancellations.Add(new CancellationRecord { Reason = reason, ItemsCount = sale.ItemsCount, RefundAmount = refund, IsPostSale = true, Items = namesAll });
                sales.RemoveAt(saleIndex);
                PopulateSalesList();
                MessageBox.Show($"Venta cancelada completamente.\nProductos devueltos:\n- {string.Join("\n- ", namesAll)}\nReembolso: ${refund:0.00}.\nMotivo: {reason}", "Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (lstVentaItems.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Seleccione uno o más productos de la venta para cancelar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // compute removed subtotal and quantities (ask user quantity per item)
            decimal removedSubtotal = 0m;
            int removedQty = 0;
            var indices = lstVentaItems.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
            var removedNames = new List<string>();
            foreach (int i in indices)
            {
                var it = sale.Items[i];
                int cancelQty = 0;
                if (it.Quantity > 1)
                {
                    cancelQty = PromptCancelQuantity(it.Name, it.Quantity);
                    if (cancelQty <= 0) continue; // user cancelled prompt
                }
                else
                {
                    cancelQty = 1;
                }

                removedSubtotal += it.Price * cancelQty;
                removedQty += cancelQty;
                removedNames.Add(it.Name + " x" + cancelQty);
            }

            decimal originalProductTotal = sale.AmountBeforeInterest;
            decimal ratio = originalProductTotal > 0 ? (removedSubtotal / originalProductTotal) : 0m;
            decimal removedInterest = Math.Round(sale.InterestAmount * ratio, 2);
            decimal removedShipping = Math.Round(sale.ShippingCost * ratio, 2);
            decimal removedTotalPaid = Math.Round(removedSubtotal + removedInterest + removedShipping, 2);
            // determine refund amount: refund full removed amount (including interests and shipping)
            decimal refundAmount = removedTotalPaid;

            // remove or reduce selected items from sale, based on chosen quantities
            foreach (int i in indices)
            {
                var it = sale.Items[i];
                // find how many were requested to cancel for this item in removedNames - match by name
                // we used the order and names collected above; assume mapping by sequence
                // simpler: if the entire quantity was requested (there is an entry matching full qty), remove; otherwise reduce
                // Determine cancelQty by parsing removedNames entries for this product
                int cancelQty = 0;
                for (int k = 0; k < removedNames.Count; k++)
                {
                    if (removedNames[k].StartsWith(it.Name + " x"))
                    {
                        var parts = removedNames[k].Split('x');
                        int q;
                        if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out q))
                        {
                            cancelQty = q;
                            // consume this entry so next same-name item won't match again
                            removedNames.RemoveAt(k);
                        }
                        break;
                    }
                }

                if (cancelQty >= it.Quantity)
                {
                    sale.Items.RemoveAt(i);
                }
                else if (cancelQty > 0)
                {
                    it.Quantity -= cancelQty;
                }
            }

            sale.AmountBeforeInterest = Math.Round(sale.AmountBeforeInterest - removedSubtotal, 2);
            sale.InterestAmount = Math.Round(sale.InterestAmount - removedInterest, 2);
            sale.ShippingCost = Math.Round(sale.ShippingCost - removedShipping, 2);
            // recompute total paid (what customer still owes/has paid for remaining items)
            sale.TotalPaid = Math.Round(sale.AmountBeforeInterest + sale.InterestAmount + sale.ShippingCost, 2);
            sale.ItemsCount = Math.Max(0, sale.ItemsCount - removedQty);
            sale.CancellationReason = reason;

            // record cancellation (refund amount considers interest policy)
            cancellations.Add(new CancellationRecord { Reason = reason, ItemsCount = removedQty, RefundAmount = refundAmount, IsPostSale = true, Items = removedNames });
            // no preserved interest accounting for partial cancellation

            // if no items left, remove sale
            if (sale.ItemsCount == 0)
            {
                // remove sale from list
                sales.RemoveAt(saleIndex);
                MessageBox.Show($"Venta cancelada completamente (por eliminación de todos los productos).\nProductos devueltos:\n- {string.Join("\n- ", removedNames)}\nReembolso: ${refundAmount:0.00}. Motivo: {reason}", "Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // refresh UI lists
                PopulateSalesList();
                lstVentaItems.Items.Clear();
                lstVentas.ClearSelected();
                return;
            }
            else
            {
                MessageBox.Show($"Productos cancelados:\n- {string.Join("\n- ", removedNames)}\nReembolso: ${refundAmount:0.00}. Motivo: {reason}", "Cancelación parcial", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            PopulateSalesList();
            lstVentaItems.Items.Clear();
            lstVentas.ClearSelected();
        }

        // Prompt a small dialog asking how many units to cancel for a given product
        private int PromptCancelQuantity(string productName, int maxQuantity)
        {
            using (Form f = new Form())
            {
                f.Text = "Cantidad a cancelar";
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.StartPosition = FormStartPosition.CenterParent;
                f.ClientSize = new Size(300, 110);
                f.MinimizeBox = false;
                f.MaximizeBox = false;

                Label lbl = new Label { Text = $"Producto: {productName}", Location = new Point(10, 10), AutoSize = true };
                Label lbl2 = new Label { Text = $"Cantidad (max {maxQuantity}):", Location = new Point(10, 35), AutoSize = true };
                NumericUpDown num = new NumericUpDown { Location = new Point(150, 33), Width = 120, Minimum = 1, Maximum = maxQuantity, Value = maxQuantity };
                Button btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(60, 70), Size = new Size(80, 28) };
                Button btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(160, 70), Size = new Size(80, 28) };

                f.Controls.Add(lbl);
                f.Controls.Add(lbl2);
                f.Controls.Add(num);
                f.Controls.Add(btnOk);
                f.Controls.Add(btnCancel);

                f.AcceptButton = btnOk;
                f.CancelButton = btnCancel;

                var dr = f.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    return (int)num.Value;
                }
                return 0;
            }
        }

        private readonly List<CartItem> cart = new List<CartItem>();
        // Account management moved to AccountManager
        // Sales history
        private class SaleRecord
        {
            public string Category { get; set; }
            public decimal AmountBeforeInterest { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal InterestAmount { get; set; }
            public int ItemsCount { get; set; }
            public int Months { get; set; }
            public string ShippingType { get; set; }
            public decimal ShippingCost { get; set; }
            public List<CartItem> Items { get; set; }
            public string CancellationReason { get; set; }
        }

        private readonly List<SaleRecord> sales = new List<SaleRecord>();
        // Map of visible sale indices in the ListBox to actual sales list indexes
        private readonly List<int> visibleSaleIndexes = new List<int>();
        // Cancellations history (pre and post sale)
        private class CancellationRecord
        {
            public string Reason { get; set; }
            public int ItemsCount { get; set; }
            public decimal RefundAmount { get; set; }
            public bool IsPostSale { get; set; }
            public List<string> Items { get; set; }
        }

        private readonly List<CancellationRecord> cancellations = new List<CancellationRecord>();

        // UI controls
        private ComboBox cmbProductosLocal;
        private NumericUpDown numCantidadLocal;
        private ListBox lstCarritoLocal;
        private Button btnAgregarLocal;
        private Button btnVerCarritoLocal;
        private GroupBox grpPagoLocal;
        private RadioButton rdoContadoLocal;
        private RadioButton rdoTarjetaMSILocal;
        private RadioButton rdoMercadoPagoLocal;
        private ComboBox cmbContadoOpcionesLocal;
        private ComboBox cmbMSIMesesLocal;
        private ComboBox cmbMPOpcionesLocal;
        private Button btnResumenLocal;
        // account controls moved to separate forms
        // shipping controls
        private RadioButton rdoEnvioNormal;
        private RadioButton rdoEnvioFull;
        // post-sale controls
        private ListBox lstVentas;
        private ListBox lstVentaItems;
        private Button btnCancelarVenta;
        private CheckBox chkCancelarVentaTodo;
        // cancellation controls
        private ComboBox cmbCancelReasons;
        private CheckBox chkCancelarTodo;
        private Button btnCancelarLocal;
        private Button btnBackStore;

        public Form5()
        {
            InitializeComponent();
            InitializeFormControls();
            InitializeProducts();
            HookEventsLocal();
        }

        private void InitializeFormControls()
        {
            this.Text = "Mercado Libre - Tienda";
            this.Size = new Size(1020, 720);
            this.BackColor = Color.FromArgb(255, 253, 240); // ML pastel cream
            this.Font = new Font("Segoe UI", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Top header panel (ML gold)
            Panel topHeader = new Panel
            {
                Size = new Size(1020, 50),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(255, 214, 0),
                Dock = DockStyle.Top
            };
            Label lblStoreTitle = new Label
            {
                Text = "Mercado Libre - Tienda",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 40, 10),
                AutoSize = true,
                Location = new Point(18, 12),
                BackColor = Color.Transparent
            };
            topHeader.Controls.Add(lblStoreTitle);

            // Product selection area
            Label lblProducto = new Label { Text = "Producto:", Location = new Point(18, 66), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            cmbProductosLocal = new ComboBox { Location = new Point(100, 62), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            Label lblCantidad = new Label { Text = "Cantidad:", Location = new Point(440, 66), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60) };
            numCantidadLocal = new NumericUpDown { Location = new Point(510, 62), Width = 80, Minimum = 1, Value = 1, Font = new Font("Segoe UI", 9.5f) };

            btnAgregarLocal = new Button { Text = "Agregar al carrito", Location = new Point(610, 58), Size = new Size(170, 34), BackColor = Color.FromArgb(255, 202, 40), ForeColor = Color.FromArgb(50, 40, 10), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAgregarLocal.FlatAppearance.BorderSize = 0;
            btnVerCarritoLocal = new Button { Text = "Confirmar compra", Location = new Point(790, 58), Size = new Size(160, 34), BackColor = Color.FromArgb(66, 165, 245), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnVerCarritoLocal.FlatAppearance.BorderSize = 0;

            // Cart panel (right side)
            Panel pnlCart = new Panel { Location = new Point(640, 108), Size = new Size(340, 240), BackColor = Color.White };
            pnlCart.Paint += (s, ev) => { ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 236, 179)), 0, 0, pnlCart.Width - 1, pnlCart.Height - 1); };
            Label lblCarrito = new Label { Text = "🛒 Carrito", Location = new Point(10, 6), AutoSize = true, BackColor = Color.Transparent, ForeColor = Color.FromArgb(245, 166, 35), Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            lstCarritoLocal = new ListBox { Location = new Point(4, 30), Size = new Size(332, 206), SelectionMode = SelectionMode.MultiExtended, BorderStyle = BorderStyle.None, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };
            pnlCart.Controls.Add(lblCarrito);
            pnlCart.Controls.Add(lstCarritoLocal);

            // Payment panel (left side)
            Panel pnlPago = new Panel { Location = new Point(18, 108), Size = new Size(600, 150), BackColor = Color.White };
            pnlPago.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(66, 165, 245)), 0, 0, 5, pnlPago.Height);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(187, 222, 251)), 0, 0, pnlPago.Width - 1, pnlPago.Height - 1);
            };
            Label lblPagoTitle = new Label { Text = "Método de pago", Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 136, 229), Location = new Point(16, 6), AutoSize = true, BackColor = Color.Transparent };
            grpPagoLocal = new GroupBox { Location = new Point(0, 0), Size = new Size(0, 0), Visible = false };
            rdoContadoLocal = new RadioButton { Text = "Contado", Location = new Point(16, 30), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            rdoTarjetaMSILocal = new RadioButton { Text = "Tarjeta crédito (MSI)", Location = new Point(140, 30), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            rdoMercadoPagoLocal = new RadioButton { Text = "Mercado Pago", Location = new Point(340, 30), AutoSize = true, Font = new Font("Segoe UI", 9f) };

            cmbContadoOpcionesLocal = new ComboBox { Location = new Point(16, 58), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbContadoOpcionesLocal.Items.AddRange(new object[] { "Deposito", "Tarjeta débito", "Tarjeta crédito" });
            cmbContadoOpcionesLocal.SelectedIndex = 0;

            cmbMSIMesesLocal = new ComboBox { Location = new Point(210, 58), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbMSIMesesLocal.Items.AddRange(new object[] { "3", "6", "12" });
            cmbMSIMesesLocal.SelectedIndex = 0;

            cmbMPOpcionesLocal = new ComboBox { Location = new Point(340, 58), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbMPOpcionesLocal.Items.AddRange(new object[] { "Contado", "3 meses (10% mensual)", "6 meses (10% mensual)" });
            cmbMPOpcionesLocal.SelectedIndex = 0;

            btnResumenLocal = new Button { Text = "Ver resumen", Location = new Point(16, 100), Size = new Size(150, 34), BackColor = Color.FromArgb(232, 245, 233), ForeColor = Color.FromArgb(27, 94, 32), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnResumenLocal.FlatAppearance.BorderColor = Color.FromArgb(165, 214, 167);

            pnlPago.Controls.Add(lblPagoTitle);
            pnlPago.Controls.Add(rdoContadoLocal);
            pnlPago.Controls.Add(rdoTarjetaMSILocal);
            pnlPago.Controls.Add(rdoMercadoPagoLocal);
            pnlPago.Controls.Add(cmbContadoOpcionesLocal);
            pnlPago.Controls.Add(cmbMSIMesesLocal);
            pnlPago.Controls.Add(cmbMPOpcionesLocal);
            pnlPago.Controls.Add(btnResumenLocal);

            // Shipping & cancellation row
            Label lblEnvio = new Label { Text = "Envío:", Location = new Point(18, 272), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            rdoEnvioNormal = new RadioButton { Text = "Normal", Location = new Point(80, 270), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            rdoEnvioFull = new RadioButton { Text = "Full", Location = new Point(170, 270), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            rdoEnvioNormal.Checked = true;

            cmbCancelReasons = new ComboBox { Location = new Point(260, 268), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbCancelReasons.Items.AddRange(new object[] { "Derecho de compra", "Producto defectuoso", "Producto con demora" });
            cmbCancelReasons.SelectedIndex = 0;
            chkCancelarTodo = new CheckBox { Text = "Cancelar todo", Location = new Point(500, 270), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            btnCancelarLocal = new Button { Text = "Cancelar compra", Location = new Point(640, 264), Size = new Size(160, 34), BackColor = Color.FromArgb(255, 235, 238), ForeColor = Color.FromArgb(198, 40, 40), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancelarLocal.FlatAppearance.BorderColor = Color.FromArgb(239, 154, 154);

            this.Controls.Add(topHeader);
            this.Controls.Add(lblEnvio);
            this.Controls.Add(rdoEnvioNormal);
            this.Controls.Add(rdoEnvioFull);
            this.Controls.Add(cmbCancelReasons);
            this.Controls.Add(chkCancelarTodo);
            this.Controls.Add(btnCancelarLocal);

            // Sales history section
            Label lblVentas = new Label { Text = "📦 Ventas registradas", Location = new Point(18, 310), AutoSize = true, BackColor = Color.Transparent, ForeColor = Color.FromArgb(69, 90, 100), Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            lstVentas = new ListBox { Location = new Point(18, 334), Size = new Size(560, 150), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };
            lstVentaItems = new ListBox { Location = new Point(590, 334), Size = new Size(390, 150), SelectionMode = SelectionMode.MultiExtended, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };
            btnCancelarVenta = new Button { Text = "Cancelar venta seleccionada", Location = new Point(18, 492), Size = new Size(240, 34), BackColor = Color.FromArgb(255, 243, 224), ForeColor = Color.FromArgb(230, 126, 34), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancelarVenta.FlatAppearance.BorderColor = Color.FromArgb(255, 224, 178);
            chkCancelarVentaTodo = new CheckBox { Text = "Cancelar toda la venta", Location = new Point(280, 498), AutoSize = true, Font = new Font("Segoe UI", 9f) };

            this.Controls.Add(lblVentas);
            this.Controls.Add(lstVentas);
            this.Controls.Add(lstVentaItems);
            this.Controls.Add(btnCancelarVenta);
            this.Controls.Add(chkCancelarVentaTodo);

            this.Controls.Add(lblProducto);
            this.Controls.Add(cmbProductosLocal);
            this.Controls.Add(lblCantidad);
            this.Controls.Add(numCantidadLocal);
            this.Controls.Add(btnAgregarLocal);
            this.Controls.Add(btnVerCarritoLocal);
            this.Controls.Add(pnlCart);
            this.Controls.Add(pnlPago);

            // Back button
            btnBackStore = new Button { Text = "← Volver", Location = new Point(820, 264), Size = new Size(120, 34), BackColor = Color.Transparent, ForeColor = Color.FromArgb(66, 165, 245), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnBackStore.FlatAppearance.BorderSize = 0;
            btnBackStore.Click += (s, e) => { if (this.Owner != null) this.Owner.Show(); this.Close(); };
            this.Controls.Add(btnBackStore);

            // defaults
            rdoContadoLocal.Checked = true;
            cmbContadoOpcionesLocal.Visible = true;
            cmbMSIMesesLocal.Visible = false;
            cmbMPOpcionesLocal.Visible = false;
        }

        private void InitializeProducts()
        {
            products.Add(("Camisa", 300m));
            products.Add(("Pantalon", 600m));
            products.Add(("Zapatos", 1200m));
            products.Add(("Calcetines (pack)", 100m));
            products.Add(("Gorra", 150m));
            products.Add(("Sudadera", 900m));
            products.Add(("Chaqueta", 1600m));
            products.Add(("Cinturon", 250m));
            products.Add(("Bolso", 1300m));
            products.Add(("Reloj", 2500m));
            products.Add(("Auriculares", 800m));
            products.Add(("Televisor 32\"", 5000m));
            products.Add(("Microondas", 1400m));
            products.Add(("Licuadora", 700m));
            products.Add(("Telefono", 9000m));
            // Producto de tecnología solicitado
            products.Add(("Dispositivo de masaje", 2000m));
            products.Add(("Tablet", 6000m));
            products.Add(("Teclado", 400m));
            products.Add(("Mouse", 200m));
            products.Add(("Monitor", 2500m));
            products.Add(("Impresora", 1900m));
            products.Add(("Silla", 1300m));
            products.Add(("Mesa", 2200m));
            products.Add(("Lampara", 250m));
            products.Add(("Cobija", 350m));
            products.Add(("Almohada", 200m));
            products.Add(("Juego de sartenes", 800m));
            products.Add(("Set de cuchillos", 500m));
            products.Add(("Colchon", 4600m));
            products.Add(("Estufa", 2700m));
            products.Add(("Refrigerador", 9000m));
            // 10 productos adicionales
            products.Add(("Cafetera", 800m));
            products.Add(("Aspiradora", 1200m));
            products.Add(("Ventilador", 400m));
            products.Add(("Horno", 3500m));
            products.Add(("Bicicleta", 3000m));
            products.Add(("Patineta", 700m));
            products.Add(("Cámara", 3500m));
            products.Add(("Altavoz Bluetooth", 600m));
            products.Add(("Router", 800m));
            products.Add(("Smartwatch", 1800m));
            // Producto adicional solicitado
            products.Add(("Cargador de telefono", 1000m));
            // Nuevo producto solicitado
            products.Add(("Lapiz digital", 1500m));

            foreach (var p in products)
                cmbProductosLocal.Items.Add($"{p.Name} - ${p.Price:0.00}");

            if (cmbProductosLocal.Items.Count > 0)
                cmbProductosLocal.SelectedIndex = 0;
        }

        private void HookEventsLocal()
        {
            btnAgregarLocal.Click += BtnAgregarLocal_Click;
            btnVerCarritoLocal.Click += BtnVerCarritoLocal_Click;
            btnResumenLocal.Click += BtnResumenLocal_Click;
            btnCancelarLocal.Click += BtnCancelarLocal_Click;
            btnCancelarVenta.Click += BtnCancelarVenta_Click;
            lstVentas.SelectedIndexChanged += LstVentas_SelectedIndexChanged;
            // account events moved to separate forms
            rdoContadoLocal.CheckedChanged += PaymentRadioChangedLocal;
            rdoTarjetaMSILocal.CheckedChanged += PaymentRadioChangedLocal;
            rdoMercadoPagoLocal.CheckedChanged += PaymentRadioChangedLocal;
        }

        private void PaymentRadioChangedLocal(object sender, EventArgs e)
        {
            cmbContadoOpcionesLocal.Visible = rdoContadoLocal.Checked;
            cmbMSIMesesLocal.Visible = rdoTarjetaMSILocal.Checked;
            cmbMPOpcionesLocal.Visible = rdoMercadoPagoLocal.Checked;
        }

        private void BtnAgregarLocal_Click(object sender, EventArgs e)
        {
            if (cmbProductosLocal.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int qty = (int)numCantidadLocal.Value;
            if (qty <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor que 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selected = products[cmbProductosLocal.SelectedIndex];
            var item = new CartItem { Name = selected.Name, Price = selected.Price, Quantity = qty };
            cart.Add(item);
            lstCarritoLocal.Items.Add(item.ToString());
        }

        private void BtnVerCarritoLocal_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = cart.Sum(i => i.Subtotal);

            // shipping cost - no extra charge per requirement
            decimal shippingCost = 0m;

            string paymentDetail = "";
            int months = 0;
            decimal totalToPay = total + shippingCost;
            List<(int Number, decimal Amount)> installments = null;

            if (rdoContadoLocal.Checked)
            {
                paymentDetail = $"Contado - {cmbContadoOpcionesLocal.SelectedItem}";
            }
            else if (rdoTarjetaMSILocal.Checked)
            {
                paymentDetail = "Tarjeta crédito (MSI)";
                months = int.Parse(cmbMSIMesesLocal.SelectedItem.ToString());
                decimal per = Math.Round(total / months, 2);
                installments = new List<(int, decimal)>();
                for (int i = 1; i <= months; i++) installments.Add((i, per));
            }
            else if (rdoMercadoPagoLocal.Checked)
            {
                var opt = cmbMPOpcionesLocal.SelectedItem.ToString();
                paymentDetail = "Mercado Pago - " + opt;
                if (opt.StartsWith("3 meses"))
                {
                    months = 3;
                    decimal interestAmount = Math.Round(total * 0.10m * months, 2);
                    totalToPay = Math.Round(total + shippingCost + interestAmount, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
                else if (opt.StartsWith("6 meses"))
                {
                    months = 6;
                    decimal interestAmount = Math.Round(total * 0.10m * months, 2);
                    totalToPay = Math.Round(total + shippingCost + interestAmount, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Total carrito: ${total:0.00}");
            sb.AppendLine($"Método: {paymentDetail}");
            if (installments != null)
            {
                sb.AppendLine($"Total a pagar: ${totalToPay:0.00}");
                sb.AppendLine($"Envío: {(rdoEnvioFull.Checked ? "Full" : "Normal")} ");
                sb.AppendLine("Mensualidades:");
                foreach (var it in installments)
                    sb.AppendLine($"{it.Number} -> ${it.Amount:0.00}");
                // show interest amount if any
                decimal interestAmount = totalToPay - total;
                if (interestAmount > 0)
                {
                    // If Mercado Pago cuotas, show aumento por mensualidad (10% del total cada mes)
                    if (rdoMercadoPagoLocal.Checked && months > 0)
                    {
                        decimal monthlyIncrease = Math.Round(total * 0.10m, 2);
                        sb.AppendLine($"Aumento por mensualidad: ${monthlyIncrease:0.00} (10% mensual)");
                    }
                    sb.AppendLine($"Interés total aplicado (Mercado Pago): ${interestAmount:0.00}");
                }
            }
            else
            {
                if (totalToPay != total)
                    sb.AppendLine($"Total a pagar (con intereses): ${totalToPay:0.00}");
                else
                    sb.AppendLine($"Total a pagar: ${total:0.00}");
            }

            MessageBox.Show(sb.ToString(), "Resumen de pago", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Record the sale
            string category = "";
            if (rdoContadoLocal.Checked)
            {
                var opt = cmbContadoOpcionesLocal.SelectedItem.ToString();
                if (opt == "Deposito") category = "Deposito";
                else if (opt == "Tarjeta débito") category = "Tarjeta debito";
                else category = "Tarjeta credito"; // contado con tarjeta credito
            }
            else if (rdoTarjetaMSILocal.Checked)
            {
                category = "Tarjeta credito MSI";
            }
            else if (rdoMercadoPagoLocal.Checked)
            {
                var opt = cmbMPOpcionesLocal.SelectedItem.ToString();
                if (opt.StartsWith("Contado")) category = "MP Contado";
                else category = "MP Credito"; // includes 3/6 meses y credito normal
            }

            decimal interest = totalToPay - total;
            int itemsCount = cart.Sum(i => i.Quantity);
            sales.Add(new SaleRecord
            {
                Category = category,
                AmountBeforeInterest = total,
                TotalPaid = totalToPay,
                InterestAmount = interest,
                Months = months,
                ItemsCount = itemsCount,
                Items = cart.Select(c => new CartItem { Name = c.Name, Price = c.Price, Quantity = c.Quantity }).ToList(),
                ShippingType = (rdoEnvioFull.Checked ? "Full" : "Normal"),
                ShippingCost = shippingCost
            });

            // update sales list UI
            PopulateSalesList();

            // Clear cart after completing the sale
            cart.Clear();
            lstCarritoLocal.Items.Clear();
        }

        // Account handlers moved to dedicated forms and AccountManager

        private void BtnCancelarLocal_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string reason = cmbCancelReasons.SelectedItem?.ToString() ?? "Sin motivo";

            if (chkCancelarTodo.Checked)
            {
                var names = cart.Select(c => c.Name + " x" + c.Quantity).ToList();
                cart.Clear();
                lstCarritoLocal.Items.Clear();
                // record pre-sale cancellation (no refund)
                cancellations.Add(new CancellationRecord { Reason = reason, ItemsCount = names.Count, RefundAmount = 0m, IsPostSale = false, Items = names });
                MessageBox.Show($"Compra cancelada (todo el carrito).\nProductos cancelados:\n- {string.Join("\n- ", names)}\nMotivo: {reason}\nReembolso: No aplica (compra no confirmada)", "Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIndices = lstCarritoLocal.SelectedIndices.Cast<int>().OrderByDescending(i => i).ToList();
            if (selectedIndices.Count == 0)
            {
                MessageBox.Show("Seleccione uno o más productos del carrito para cancelar o marque 'Cancelar todo el carrito'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var removedNames = new List<string>();
            // For each selected item, ask quantity to cancel if quantity > 1
            foreach (int idx in selectedIndices)
            {
                var removed = cart[idx];
                int cancelQty = 0;
                if (removed.Quantity > 1)
                {
                    cancelQty = PromptCancelQuantity(removed.Name, removed.Quantity);
                    if (cancelQty <= 0) // user cancelled the prompt
                        continue;
                }
                else
                {
                    cancelQty = 1;
                }

                // apply cancellation quantity
                if (cancelQty >= removed.Quantity)
                {
                    // remove entire item
                    removedNames.Add(removed.Name + " x" + removed.Quantity);
                    cart.RemoveAt(idx);
                    lstCarritoLocal.Items.RemoveAt(idx);
                }
                else
                {
                    // reduce quantity
                    removedNames.Add(removed.Name + " x" + cancelQty);
                    removed.Quantity -= cancelQty;
                    lstCarritoLocal.Items[idx] = removed.ToString();
                }
            }

            // record pre-sale partial cancellation
            if (removedNames.Count > 0)
            {
                cancellations.Add(new CancellationRecord { Reason = reason, ItemsCount = removedNames.Count, RefundAmount = 0m, IsPostSale = false, Items = removedNames });
                MessageBox.Show($"Productos cancelados:\n- {string.Join("\n- ", removedNames)}\nMotivo: {reason}\nReembolso: No aplica (compra no confirmada)", "Cancelación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnResumenLocal_Click(object sender, EventArgs e)
        {
            if (sales.Count == 0)
            {
                MessageBox.Show("No hay ventas registradas aún.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Counts
            int comprasDeposito = sales.Count(s => s.Category == "Deposito");
            int comprasDebito = sales.Count(s => s.Category == "Tarjeta debito");
            int comprasCreditoContado = sales.Count(s => s.Category == "Tarjeta credito");
            int comprasCreditoMSI = sales.Count(s => s.Category == "Tarjeta credito MSI");
            int comprasMPContado = sales.Count(s => s.Category == "MP Contado");
            int comprasMPCredito = sales.Count(s => s.Category == "MP Credito");

            // Counts by months for MSI and Mercado Pago
            int msi3 = sales.Count(s => s.Category == "Tarjeta credito MSI" && s.Months == 3);
            int msi6 = sales.Count(s => s.Category == "Tarjeta credito MSI" && s.Months == 6);
            int msi12 = sales.Count(s => s.Category == "Tarjeta credito MSI" && s.Months == 12);

            int mp3 = sales.Count(s => s.Category == "MP Credito" && s.Months == 3);
            int mp6 = sales.Count(s => s.Category == "MP Credito" && s.Months == 6);

            // Ingresos por tipo/meses
            decimal ingresosContado = sales.Where(s => s.Category == "Deposito" || s.Category == "Tarjeta debito" || s.Category == "Tarjeta credito").Sum(s => s.TotalPaid);
            decimal ingresosMsi3 = sales.Where(s => s.Category == "Tarjeta credito MSI" && s.Months == 3).Sum(s => s.TotalPaid);
            decimal ingresosMsi6 = sales.Where(s => s.Category == "Tarjeta credito MSI" && s.Months == 6).Sum(s => s.TotalPaid);
            decimal ingresosMsi12 = sales.Where(s => s.Category == "Tarjeta credito MSI" && s.Months == 12).Sum(s => s.TotalPaid);
            decimal ingresosMp3 = sales.Where(s => s.Category == "MP Credito" && s.Months == 3).Sum(s => s.TotalPaid);
            decimal ingresosMp6 = sales.Where(s => s.Category == "MP Credito" && s.Months == 6).Sum(s => s.TotalPaid);

            // Totals
            decimal totalIngresos = sales.Sum(s => s.TotalPaid);
            int totalProductosVendidos = sales.Sum(s => s.ItemsCount);
            int totalEnviosNormal = sales.Count(s => s.ShippingType == "Normal");
            int totalEnviosFull = sales.Count(s => s.ShippingType == "Full");
            decimal totalShippingNormal = sales.Where(s => s.ShippingType == "Normal").Sum(s => s.ShippingCost);
            decimal totalShippingFull = sales.Where(s => s.ShippingType == "Full").Sum(s => s.ShippingCost);
            decimal totalDeposito = sales.Where(s => s.Category == "Deposito").Sum(s => s.TotalPaid);
            decimal totalDebito = sales.Where(s => s.Category == "Tarjeta debito").Sum(s => s.TotalPaid);
            decimal totalCreditoContado = sales.Where(s => s.Category == "Tarjeta credito").Sum(s => s.TotalPaid);
            decimal totalCreditoMSI = sales.Where(s => s.Category == "Tarjeta credito MSI").Sum(s => s.TotalPaid);
            decimal totalMPContado = sales.Where(s => s.Category == "MP Contado").Sum(s => s.TotalPaid);
            decimal totalMPCredito = sales.Where(s => s.Category == "MP Credito").Sum(s => s.TotalPaid);

            decimal totalIntereses = sales.Sum(s => s.InterestAmount);

            // Cancellations summary
            int totalCancellations = cancellations.Count;
            int cancelDerecho = cancellations.Count(c => c.Reason == "Derecho de compra");
            int cancelDefectuoso = cancellations.Count(c => c.Reason == "Producto defectuoso");
            int cancelDemora = cancellations.Count(c => c.Reason == "Producto con demora");
            decimal refundDerecho = cancellations.Where(c => c.Reason == "Derecho de compra").Sum(c => c.RefundAmount);
            decimal refundDefectuoso = cancellations.Where(c => c.Reason == "Producto defectuoso").Sum(c => c.RefundAmount);
            decimal refundDemora = cancellations.Where(c => c.Reason == "Producto con demora").Sum(c => c.RefundAmount);

            var sb = new StringBuilder();
            sb.AppendLine("RESUMEN DE VENTAS");
            sb.AppendLine();
            sb.AppendLine($"Total productos vendidos: {totalProductosVendidos}");
            sb.AppendLine();
            sb.AppendLine($"Total ventas (transacciones): {sales.Count}");
            sb.AppendLine();
            sb.AppendLine("Número de compras por método:");
            sb.AppendLine();
            sb.AppendLine($"Total cancelaciones: {totalCancellations}");
            sb.AppendLine($"- Derecho de compra: {cancelDerecho} (Reembolso: ${refundDerecho:0.00})");
            sb.AppendLine($"- Producto defectuoso: {cancelDefectuoso} (Reembolso: ${refundDefectuoso:0.00})");
            sb.AppendLine($"- Producto con demora: {cancelDemora} (Reembolso: ${refundDemora:0.00})");
            sb.AppendLine();
            sb.AppendLine($"- Envíos Normal: {totalEnviosNormal}");
            sb.AppendLine($"- Envíos Full: {totalEnviosFull}");
            sb.AppendLine();
            sb.AppendLine("Ventas por meses (MSI):");
            sb.AppendLine($"- 3 meses: {msi3}");
            sb.AppendLine($"- 6 meses: {msi6}");
            sb.AppendLine($"- 12 meses: {msi12}");
            sb.AppendLine();
            sb.AppendLine("Ventas por meses (Mercado Pago):");
            sb.AppendLine($"- 3 meses: {mp3}");
            sb.AppendLine($"- 6 meses: {mp6}");
            sb.AppendLine($"- Depósito: {comprasDeposito}");
            sb.AppendLine($"- Tarjeta débito: {comprasDebito}");
            sb.AppendLine($"- Tarjeta crédito (contado): {comprasCreditoContado}");
            sb.AppendLine($"- Tarjeta crédito a MSI: {comprasCreditoMSI}");
            sb.AppendLine($"- Mercado Pago (contado): {comprasMPContado}");
            sb.AppendLine($"- Mercado Pago (crédito / meses): {comprasMPCredito}");
            sb.AppendLine();
            sb.AppendLine("Ingresos totales:");
            sb.AppendLine($"- Total ingresos: ${totalIngresos:0.00}");
            sb.AppendLine($"- Ingresos por contado: ${ingresosContado:0.00}");
            sb.AppendLine($"- Ingresos MSI 3 meses: ${ingresosMsi3:0.00}");
            sb.AppendLine($"- Ingresos MSI 6 meses: ${ingresosMsi6:0.00}");
            sb.AppendLine($"- Ingresos MSI 12 meses: ${ingresosMsi12:0.00}");
            sb.AppendLine($"- Ingresos MP 3 meses: ${ingresosMp3:0.00}");
            sb.AppendLine($"- Ingresos MP 6 meses: ${ingresosMp6:0.00}");
            sb.AppendLine($"- Ingresos por envíos (Normal): ${totalShippingNormal:0.00}");
            sb.AppendLine($"- Ingresos por envíos (Full): ${totalShippingFull:0.00}");
            sb.AppendLine($"- Depósitos: ${totalDeposito:0.00}");
            sb.AppendLine($"- Tarjeta débito: ${totalDebito:0.00}");
            sb.AppendLine($"- Tarjeta crédito (contado): ${totalCreditoContado:0.00}");
            sb.AppendLine($"- Tarjeta crédito a MSI: ${totalCreditoMSI:0.00}");
            sb.AppendLine($"- Mercado Pago (contado): ${totalMPContado:0.00}");
            sb.AppendLine($"- Mercado Pago (crédito / meses): ${totalMPCredito:0.00}");
            sb.AppendLine();
            sb.AppendLine($"Ingresos por intereses: ${totalIntereses:0.00}");

            MessageBox.Show(sb.ToString(), "Resumen de ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
