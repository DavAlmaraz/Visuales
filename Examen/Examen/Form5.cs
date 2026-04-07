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
            public decimal Weight { get; set; }
            public decimal Commission { get; set; }
            public bool ApplyCommission { get; set; }
            public decimal Subtotal => Price * Quantity;
            public decimal TotalCommission => ApplyCommission ? Commission * Quantity : 0m;
            public override string ToString()
            {
                string comTxt = ApplyCommission ? $" [Peso:{Weight}kg Com:${Commission:0.00}x{Quantity}]" : "";
                return $"{Name} x{Quantity} - ${Subtotal:0.00}{comTxt}";
            }
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
            public decimal TotalCommission { get; set; }
            public int ItemsWithCommission { get; set; }
            public int ItemsWithoutCommission { get; set; }
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
        // weight/commission controls
        private NumericUpDown numPesoLocal;
        private NumericUpDown numComisionLocal;
        private CheckBox chkAplicarComision;
        // shipping controls
        private Panel pnlEnvioGroup;
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
            this.BackColor = Color.FromArgb(245, 248, 255);
            this.Font = new Font("Segoe UI", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ── Top header (ML gold gradient) ──
            Panel topHeader = new Panel
            {
                Size = new Size(1020, 56),
                Location = new Point(0, 0),
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(255, 214, 0)
            };
            topHeader.Paint += (s, ev) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    topHeader.ClientRectangle,
                    Color.FromArgb(255, 193, 7),
                    Color.FromArgb(255, 224, 80),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    ev.Graphics.FillRectangle(brush, topHeader.ClientRectangle);
                }
            };
            Label lblStoreTitle = new Label
            {
                Text = "🛒  Mercado Libre - Tienda",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 40, 10),
                AutoSize = true,
                Location = new Point(18, 14),
                BackColor = Color.Transparent
            };
            btnBackStore = new Button
            {
                Text = "← Volver al menú",
                Location = new Point(850, 12),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(255, 255, 255, 120),
                ForeColor = Color.FromArgb(50, 40, 10),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBackStore.FlatAppearance.BorderColor = Color.FromArgb(200, 170, 0);
            btnBackStore.Click += (s, e) =>
            {
                SaveAccountPurchaseHistory();
                if (this.Owner != null) this.Owner.Show();
                this.Close();
            };
            topHeader.Controls.Add(lblStoreTitle);
            topHeader.Controls.Add(btnBackStore);

            // ── TabControl ──
            TabControl tabs = new TabControl
            {
                Location = new Point(10, 62),
                Size = new Size(984, 610),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            // ═══════════════════════════════════════
            //  TAB 1: Productos y Carrito
            // ═══════════════════════════════════════
            TabPage tabCompra = new TabPage("🛍  Productos y Carrito")
            {
                BackColor = Color.FromArgb(255, 253, 245),
                Padding = new Padding(10)
            };

            // Product selection row
            Panel pnlProductRow = new Panel
            {
                Location = new Point(14, 10),
                Size = new Size(940, 90),
                BackColor = Color.White
            };
            pnlProductRow.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 202, 40)), 0, 0, pnlProductRow.Width, 4);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 236, 179)), 0, 0, pnlProductRow.Width - 1, pnlProductRow.Height - 1);
            };
            Label lblProducto = new Label { Text = "Producto:", Location = new Point(14, 16), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            cmbProductosLocal = new ComboBox { Location = new Point(90, 12), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            Label lblCantidad = new Label { Text = "Cantidad:", Location = new Point(450, 16), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60) };
            numCantidadLocal = new NumericUpDown { Location = new Point(524, 12), Width = 80, Minimum = 1, Value = 1, Font = new Font("Segoe UI", 9.5f) };
            btnAgregarLocal = new Button { Text = "➕ Agregar", Location = new Point(624, 8), Size = new Size(140, 34), BackColor = Color.FromArgb(255, 202, 40), ForeColor = Color.FromArgb(50, 40, 10), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAgregarLocal.FlatAppearance.BorderSize = 0;
            btnVerCarritoLocal = new Button { Text = "✅ Confirmar compra", Location = new Point(780, 8), Size = new Size(150, 34), BackColor = Color.FromArgb(66, 165, 245), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnVerCarritoLocal.FlatAppearance.BorderSize = 0;
            // Weight/commission row
            Label lblPeso = new Label { Text = "Peso (kg):", Location = new Point(14, 56), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 9f) };
            numPesoLocal = new NumericUpDown { Location = new Point(90, 52), Width = 80, Minimum = 0, Maximum = 10000, DecimalPlaces = 2, Value = 0, Font = new Font("Segoe UI", 9f) };
            Label lblComision = new Label { Text = "Comisión ($):", Location = new Point(190, 56), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 9f) };
            numComisionLocal = new NumericUpDown { Location = new Point(280, 52), Width = 90, Minimum = 0, Maximum = 100000, DecimalPlaces = 2, Value = 0, Font = new Font("Segoe UI", 9f) };
            chkAplicarComision = new CheckBox { Text = "Aplicar comisión por peso/volumen", Location = new Point(390, 54), AutoSize = true, Font = new Font("Segoe UI", 9f), ForeColor = Color.FromArgb(90, 80, 60) };
            pnlProductRow.Controls.AddRange(new Control[] { lblProducto, cmbProductosLocal, lblCantidad, numCantidadLocal, btnAgregarLocal, btnVerCarritoLocal, lblPeso, numPesoLocal, lblComision, numComisionLocal, chkAplicarComision });

            // Left: Payment method panel
            Panel pnlPago = new Panel { Location = new Point(14, 112), Size = new Size(560, 240), BackColor = Color.White };
            pnlPago.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(66, 165, 245)), 0, 0, 5, pnlPago.Height);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(187, 222, 251)), 0, 0, pnlPago.Width - 1, pnlPago.Height - 1);
            };
            Label lblPagoTitle = new Label { Text = "💳  Método de pago", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.FromArgb(30, 136, 229), Location = new Point(16, 10), AutoSize = true, BackColor = Color.Transparent };
            grpPagoLocal = new GroupBox { Location = new Point(0, 0), Size = new Size(0, 0), Visible = false };
            rdoContadoLocal = new RadioButton { Text = "Contado", Location = new Point(16, 40), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            rdoTarjetaMSILocal = new RadioButton { Text = "Tarjeta crédito (MSI)", Location = new Point(160, 40), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            rdoMercadoPagoLocal = new RadioButton { Text = "Mercado Pago", Location = new Point(360, 40), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };

            // Separator line
            Label paymentSep = new Label { Size = new Size(520, 1), Location = new Point(16, 68), BackColor = Color.FromArgb(220, 230, 240) };

            cmbContadoOpcionesLocal = new ComboBox { Location = new Point(16, 80), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbContadoOpcionesLocal.Items.AddRange(new object[] { "Deposito Oxxo", "Deposito Seven Eleven", "Deposito bancario", "Transferencia", "Tarjeta débito", "Tarjeta crédito" });
            cmbContadoOpcionesLocal.SelectedIndex = 0;

            cmbMSIMesesLocal = new ComboBox { Location = new Point(16, 80), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbMSIMesesLocal.Items.AddRange(new object[] { "3", "6", "12" });
            cmbMSIMesesLocal.SelectedIndex = 0;

            cmbMPOpcionesLocal = new ComboBox { Location = new Point(16, 80), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbMPOpcionesLocal.Items.AddRange(new object[] { "Contado", "3 meses (10% mensual)", "6 meses (10% mensual)" });
            cmbMPOpcionesLocal.SelectedIndex = 0;

            // Shipping row - wrapped in separate Panel so radios don't conflict with payment radios
            Label lblEnvio = new Label { Text = "📦  Envío:", Location = new Point(16, 120), AutoSize = true, ForeColor = Color.FromArgb(90, 80, 60), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            pnlEnvioGroup = new Panel { Location = new Point(90, 114), Size = new Size(200, 28), BackColor = Color.Transparent };
            rdoEnvioNormal = new RadioButton { Text = "Normal", Location = new Point(0, 4), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            rdoEnvioFull = new RadioButton { Text = "Full", Location = new Point(100, 4), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            rdoEnvioNormal.Checked = true;
            pnlEnvioGroup.Controls.Add(rdoEnvioNormal);
            pnlEnvioGroup.Controls.Add(rdoEnvioFull);

            // Cancellation row
            Label lblCancelTitle = new Label { Text = "❌  Cancelación:", Location = new Point(16, 156), AutoSize = true, ForeColor = Color.FromArgb(198, 40, 40), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            cmbCancelReasons = new ComboBox { Location = new Point(150, 152), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            cmbCancelReasons.Items.AddRange(new object[] { "Derecho de compra", "Producto defectuoso", "Producto con demora" });
            cmbCancelReasons.SelectedIndex = 0;
            chkCancelarTodo = new CheckBox { Text = "Todo el carrito", Location = new Point(366, 154), AutoSize = true, Font = new Font("Segoe UI", 9f) };

            btnCancelarLocal = new Button { Text = "Cancelar compra", Location = new Point(16, 192), Size = new Size(170, 36), BackColor = Color.FromArgb(255, 235, 238), ForeColor = Color.FromArgb(198, 40, 40), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancelarLocal.FlatAppearance.BorderColor = Color.FromArgb(239, 154, 154);
            btnResumenLocal = new Button { Text = "📊 Ver resumen", Location = new Point(200, 192), Size = new Size(160, 36), BackColor = Color.FromArgb(232, 245, 233), ForeColor = Color.FromArgb(27, 94, 32), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnResumenLocal.FlatAppearance.BorderColor = Color.FromArgb(165, 214, 167);

            pnlPago.Controls.AddRange(new Control[] { lblPagoTitle, rdoContadoLocal, rdoTarjetaMSILocal, rdoMercadoPagoLocal, paymentSep, cmbContadoOpcionesLocal, cmbMSIMesesLocal, cmbMPOpcionesLocal, lblEnvio, pnlEnvioGroup, lblCancelTitle, cmbCancelReasons, chkCancelarTodo, btnCancelarLocal, btnResumenLocal });

            // Right: Cart panel
            Panel pnlCart = new Panel { Location = new Point(590, 112), Size = new Size(364, 240), BackColor = Color.White };
            pnlCart.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 166, 35)), 0, 0, pnlCart.Width, 4);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 236, 179)), 0, 0, pnlCart.Width - 1, pnlCart.Height - 1);
            };
            Label lblCarrito = new Label { Text = "🛒 Carrito de compras", Location = new Point(12, 12), AutoSize = true, BackColor = Color.Transparent, ForeColor = Color.FromArgb(245, 166, 35), Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            lstCarritoLocal = new ListBox { Location = new Point(6, 40), Size = new Size(352, 194), SelectionMode = SelectionMode.MultiExtended, BorderStyle = BorderStyle.None, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };
            pnlCart.Controls.Add(lblCarrito);
            pnlCart.Controls.Add(lstCarritoLocal);

            tabCompra.Controls.Add(pnlProductRow);
            tabCompra.Controls.Add(pnlPago);
            tabCompra.Controls.Add(pnlCart);

            // ═══════════════════════════════════════
            //  TAB 2: Historial de Ventas
            // ═══════════════════════════════════════
            TabPage tabVentas = new TabPage("📦  Historial de Ventas")
            {
                BackColor = Color.FromArgb(245, 248, 255),
                Padding = new Padding(10)
            };

            // Sales list (left)
            Label lblVentas = new Label { Text = "📦 Ventas registradas", Location = new Point(14, 10), AutoSize = true, ForeColor = Color.FromArgb(69, 90, 100), Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            lstVentas = new ListBox { Location = new Point(14, 38), Size = new Size(550, 420), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };

            // Sale items (right)
            Label lblVentaDetail = new Label { Text = "📋 Detalle de venta", Location = new Point(580, 10), AutoSize = true, ForeColor = Color.FromArgb(69, 90, 100), Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            lstVentaItems = new ListBox { Location = new Point(580, 38), Size = new Size(374, 420), SelectionMode = SelectionMode.MultiExtended, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Font = new Font("Segoe UI", 9f) };

            // Cancel sale controls at bottom
            Panel pnlCancelSale = new Panel { Location = new Point(14, 470), Size = new Size(940, 50), BackColor = Color.White };
            pnlCancelSale.Paint += (s, ev) =>
            {
                ev.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 152, 0)), 0, 0, pnlCancelSale.Width, 4);
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 224, 178)), 0, 0, pnlCancelSale.Width - 1, pnlCancelSale.Height - 1);
            };
            btnCancelarVenta = new Button { Text = "❌ Cancelar venta seleccionada", Location = new Point(14, 12), Size = new Size(260, 30), BackColor = Color.FromArgb(255, 243, 224), ForeColor = Color.FromArgb(230, 126, 34), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancelarVenta.FlatAppearance.BorderColor = Color.FromArgb(255, 224, 178);
            chkCancelarVentaTodo = new CheckBox { Text = "Cancelar toda la venta", Location = new Point(300, 14), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            pnlCancelSale.Controls.Add(btnCancelarVenta);
            pnlCancelSale.Controls.Add(chkCancelarVentaTodo);

            tabVentas.Controls.Add(lblVentas);
            tabVentas.Controls.Add(lstVentas);
            tabVentas.Controls.Add(lblVentaDetail);
            tabVentas.Controls.Add(lstVentaItems);
            tabVentas.Controls.Add(pnlCancelSale);

            tabs.TabPages.Add(tabCompra);
            tabs.TabPages.Add(tabVentas);

            this.Controls.Add(topHeader);
            this.Controls.Add(tabs);

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
            decimal weight = numPesoLocal.Value;
            decimal commission = numComisionLocal.Value;
            bool applyComm = chkAplicarComision.Checked;
            var item = new CartItem { Name = selected.Name, Price = selected.Price, Quantity = qty, Weight = weight, Commission = commission, ApplyCommission = applyComm };
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

            var a = AccountManager.CurrentAccount;
            if (a == null)
            {
                MessageBox.Show("Inicie sesión en una cuenta primero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = cart.Sum(i => i.Subtotal);
            decimal totalCommission = cart.Sum(i => i.TotalCommission);
            int itemsWithComm = cart.Count(i => i.ApplyCommission);
            int itemsWithoutComm = cart.Count(i => !i.ApplyCommission);

            // shipping cost - no extra charge per requirement
            decimal shippingCost = 0m;

            string paymentDetail = "";
            int months = 0;
            decimal totalToPay = total + shippingCost + totalCommission;
            List<(int Number, decimal Amount)> installments = null;

            if (rdoContadoLocal.Checked)
            {
                paymentDetail = $"Contado - {cmbContadoOpcionesLocal.SelectedItem}";
            }
            else if (rdoTarjetaMSILocal.Checked)
            {
                paymentDetail = "Tarjeta crédito (MSI)";
                months = int.Parse(cmbMSIMesesLocal.SelectedItem.ToString());
                decimal per = Math.Round(totalToPay / months, 2);
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
                    totalToPay = Math.Round(total + shippingCost + totalCommission + interestAmount, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
                else if (opt.StartsWith("6 meses"))
                {
                    months = 6;
                    decimal interestAmount = Math.Round(total * 0.10m * months, 2);
                    totalToPay = Math.Round(total + shippingCost + totalCommission + interestAmount, 2);
                    decimal per = Math.Round(totalToPay / months, 2);
                    installments = new List<(int, decimal)>();
                    for (int i = 1; i <= months; i++) installments.Add((i, per));
                }
            }

            // Check MP balance is sufficient
            if (a.Balance < totalToPay)
            {
                MessageBox.Show($"Saldo insuficiente en Mercado Pago.\nSaldo disponible: ${a.Balance:0.00}\nTotal a pagar: ${totalToPay:0.00}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Deduct from MP balance
            a.Balance -= totalToPay;

            var sb = new StringBuilder();
            sb.AppendLine("═══ RESUMEN DE COMPRA ═══");
            sb.AppendLine();
            sb.AppendLine($"Total productos: ${total:0.00}");
            sb.AppendLine($"Comisiones peso/volumen: ${totalCommission:0.00}");
            sb.AppendLine($"Productos con comisión: {itemsWithComm}");
            sb.AppendLine($"Productos sin comisión: {itemsWithoutComm}");
            sb.AppendLine($"Método: {paymentDetail}");
            sb.AppendLine($"Envío: {(rdoEnvioFull.Checked ? "Full" : "Normal")}");
            if (installments != null)
            {
                sb.AppendLine($"Total a pagar: ${totalToPay:0.00}");
                sb.AppendLine("Mensualidades:");
                foreach (var it in installments)
                    sb.AppendLine($"  {it.Number} -> ${it.Amount:0.00}");
                decimal interestAmount = totalToPay - total - totalCommission;
                if (interestAmount > 0)
                {
                    if (rdoMercadoPagoLocal.Checked && months > 0)
                    {
                        decimal monthlyIncrease = Math.Round(total * 0.10m, 2);
                        sb.AppendLine($"Aumento por mensualidad: ${monthlyIncrease:0.00} (10% mensual)");
                    }
                    sb.AppendLine($"Interés total: ${interestAmount:0.00}");
                }
            }
            else
            {
                sb.AppendLine($"Total a pagar: ${totalToPay:0.00}");
            }
            sb.AppendLine();
            sb.AppendLine("── Detalle de productos ──");
            foreach (var c in cart)
            {
                string commInfo = c.ApplyCommission ? $" | Com: ${c.Commission:0.00}x{c.Quantity}=${c.TotalCommission:0.00} (Peso:{c.Weight}kg)" : " | Sin comisión";
                sb.AppendLine($"  {c.Name} x{c.Quantity} = ${c.Subtotal:0.00}{commInfo}");
            }
            sb.AppendLine();
            sb.AppendLine($"Saldo MP restante: ${a.Balance:0.00}");

            ShowStyledReport("Compra realizada", sb.ToString(), Color.FromArgb(66, 165, 245));

            // Record the sale
            string category = "";
            if (rdoContadoLocal.Checked)
            {
                var opt = cmbContadoOpcionesLocal.SelectedItem.ToString();
                if (opt == "Deposito Oxxo") category = "Deposito Oxxo";
                else if (opt == "Deposito Seven Eleven") category = "Deposito Seven Eleven";
                else if (opt == "Deposito bancario") category = "Deposito bancario";
                else if (opt == "Transferencia") category = "Transferencia";
                else if (opt == "Tarjeta débito") category = "Tarjeta debito";
                else category = "Tarjeta credito";
            }
            else if (rdoTarjetaMSILocal.Checked)
            {
                category = "Tarjeta credito MSI";
            }
            else if (rdoMercadoPagoLocal.Checked)
            {
                var opt = cmbMPOpcionesLocal.SelectedItem.ToString();
                if (opt.StartsWith("Contado")) category = "MP Contado";
                else category = "MP Credito";
            }

            decimal interest = totalToPay - total - totalCommission;
            int itemsCount = cart.Sum(i => i.Quantity);
            sales.Add(new SaleRecord
            {
                Category = category,
                AmountBeforeInterest = total,
                TotalPaid = totalToPay,
                InterestAmount = interest,
                Months = months,
                ItemsCount = itemsCount,
                TotalCommission = totalCommission,
                ItemsWithCommission = itemsWithComm,
                ItemsWithoutCommission = itemsWithoutComm,
                Items = cart.Select(c => new CartItem { Name = c.Name, Price = c.Price, Quantity = c.Quantity, Weight = c.Weight, Commission = c.Commission, ApplyCommission = c.ApplyCommission }).ToList(),
                ShippingType = (rdoEnvioFull.Checked ? "Full" : "Normal"),
                ShippingCost = shippingCost
            });

            // Record purchase in account
            foreach (var c in cart)
            {
                string key = c.Name;
                if (!a.PurchaseCounts.ContainsKey(key)) a.PurchaseCounts[key] = 0;
                if (!a.PurchaseTotals.ContainsKey(key)) a.PurchaseTotals[key] = 0m;
                a.PurchaseCounts[key] += c.Quantity;
                a.PurchaseTotals[key] += c.Subtotal;
                a.TotalPurchaseItems += c.Quantity;
                if (c.ApplyCommission)
                    a.PurchaseItemsWithCommission += c.Quantity;
                else
                    a.PurchaseItemsWithoutCommission += c.Quantity;
                a.TotalCommissionsPaid += c.TotalCommission;
                a.PurchaseHistory.Add(new PurchaseRecord
                {
                    ProductName = c.Name,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price,
                    Weight = c.Weight,
                    Commission = c.Commission,
                    AppliedCommission = c.ApplyCommission,
                    TotalPaid = c.Subtotal + c.TotalCommission,
                    Date = DateTime.Now
                });
            }

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
            var a = AccountManager.CurrentAccount;
            if (sales.Count == 0 && (a == null || a.PurchaseHistory.Count == 0))
            {
                MessageBox.Show("No hay ventas registradas aún.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Counts
            int comprasDepositoOxxo = sales.Count(s => s.Category == "Deposito Oxxo");
            int comprasDepositoSeven = sales.Count(s => s.Category == "Deposito Seven Eleven");
            int comprasDepositoBancario = sales.Count(s => s.Category == "Deposito bancario");
            int comprasTransferencia = sales.Count(s => s.Category == "Transferencia");
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
            decimal ingresosContado = sales.Where(s => s.Category == "Deposito Oxxo" || s.Category == "Deposito Seven Eleven" || s.Category == "Deposito bancario" || s.Category == "Transferencia" || s.Category == "Tarjeta debito" || s.Category == "Tarjeta credito").Sum(s => s.TotalPaid);
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
            decimal totalDepositoOxxo = sales.Where(s => s.Category == "Deposito Oxxo").Sum(s => s.TotalPaid);
            decimal totalDepositoSeven = sales.Where(s => s.Category == "Deposito Seven Eleven").Sum(s => s.TotalPaid);
            decimal totalDepositoBancarioYTransferencia = sales.Where(s => s.Category == "Deposito bancario" || s.Category == "Transferencia").Sum(s => s.TotalPaid);
            decimal totalDebito = sales.Where(s => s.Category == "Tarjeta debito").Sum(s => s.TotalPaid);
            decimal totalCreditoContado = sales.Where(s => s.Category == "Tarjeta credito").Sum(s => s.TotalPaid);
            decimal totalCreditoMSI = sales.Where(s => s.Category == "Tarjeta credito MSI").Sum(s => s.TotalPaid);
            decimal totalMPContado = sales.Where(s => s.Category == "MP Contado").Sum(s => s.TotalPaid);
            decimal totalMPCredito = sales.Where(s => s.Category == "MP Credito").Sum(s => s.TotalPaid);

            decimal totalIntereses = sales.Sum(s => s.InterestAmount);
            decimal totalComisionesPesoVol = sales.Sum(s => s.TotalCommission);
            int totalConComision = sales.Sum(s => s.ItemsWithCommission);
            int totalSinComision = sales.Sum(s => s.ItemsWithoutCommission);

            // Cancellations summary
            int totalCancellations = cancellations.Count;
            int cancelDerecho = cancellations.Count(c => c.Reason == "Derecho de compra");
            int cancelDefectuoso = cancellations.Count(c => c.Reason == "Producto defectuoso");
            int cancelDemora = cancellations.Count(c => c.Reason == "Producto con demora");
            decimal refundDerecho = cancellations.Where(c => c.Reason == "Derecho de compra").Sum(c => c.RefundAmount);
            decimal refundDefectuoso = cancellations.Where(c => c.Reason == "Producto defectuoso").Sum(c => c.RefundAmount);
            decimal refundDemora = cancellations.Where(c => c.Reason == "Producto con demora").Sum(c => c.RefundAmount);

            var sb = new StringBuilder();
            sb.AppendLine("══════ RESUMEN DE MERCADO LIBRE ══════");
            sb.AppendLine();
            // MP balance section
            if (a != null)
            {
                sb.AppendLine($"Saldo MP disponible para compras: ${a.Balance:0.00}");
                sb.AppendLine($"Total gastado en compras ML: ${a.TotalPurchased + a.TotalCommissionsPaid:0.00}");
                sb.AppendLine();
            }
            sb.AppendLine($"Total productos vendidos: {totalProductosVendidos}");
            sb.AppendLine($"Total ventas (transacciones): {sales.Count}");
            sb.AppendLine();
            sb.AppendLine("── Comisiones por peso/volumen ──");
            sb.AppendLine($"Total comisiones cobradas: ${totalComisionesPesoVol:0.00}");
            sb.AppendLine($"Productos CON comisión: {totalConComision}");
            sb.AppendLine($"Productos SIN comisión: {totalSinComision}");
            sb.AppendLine();
            sb.AppendLine("── Compras por método ──");
            sb.AppendLine($"- Depósito Oxxo: {comprasDepositoOxxo}");
            sb.AppendLine($"- Depósito Seven Eleven: {comprasDepositoSeven}");
            sb.AppendLine($"- Depósito bancario: {comprasDepositoBancario}");
            sb.AppendLine($"- Transferencia: {comprasTransferencia}");
            sb.AppendLine($"- Tarjeta débito: {comprasDebito}");
            sb.AppendLine($"- Tarjeta crédito (contado): {comprasCreditoContado}");
            sb.AppendLine($"- Tarjeta crédito a MSI: {comprasCreditoMSI}");
            sb.AppendLine($"- Mercado Pago (contado): {comprasMPContado}");
            sb.AppendLine($"- Mercado Pago (crédito / meses): {comprasMPCredito}");
            sb.AppendLine();
            sb.AppendLine($"Total cancelaciones: {totalCancellations}");
            sb.AppendLine($"- Derecho de compra: {cancelDerecho} (Reembolso: ${refundDerecho:0.00})");
            sb.AppendLine($"- Producto defectuoso: {cancelDefectuoso} (Reembolso: ${refundDefectuoso:0.00})");
            sb.AppendLine($"- Producto con demora: {cancelDemora} (Reembolso: ${refundDemora:0.00})");
            sb.AppendLine();
            sb.AppendLine($"- Envíos Normal: {totalEnviosNormal}");
            sb.AppendLine($"- Envíos Full: {totalEnviosFull}");
            sb.AppendLine();
            sb.AppendLine("── Ventas por meses (MSI) ──");
            sb.AppendLine($"- 3 meses: {msi3}");
            sb.AppendLine($"- 6 meses: {msi6}");
            sb.AppendLine($"- 12 meses: {msi12}");
            sb.AppendLine();
            sb.AppendLine("── Ventas por meses (Mercado Pago) ──");
            sb.AppendLine($"- 3 meses: {mp3}");
            sb.AppendLine($"- 6 meses: {mp6}");
            sb.AppendLine();
            sb.AppendLine("── Ingresos totales ──");
            sb.AppendLine($"- Total ingresos: ${totalIngresos:0.00}");
            sb.AppendLine($"- Ingresos por contado: ${ingresosContado:0.00}");
            sb.AppendLine($"- Ingresos MSI 3 meses: ${ingresosMsi3:0.00}");
            sb.AppendLine($"- Ingresos MSI 6 meses: ${ingresosMsi6:0.00}");
            sb.AppendLine($"- Ingresos MSI 12 meses: ${ingresosMsi12:0.00}");
            sb.AppendLine($"- Ingresos MP 3 meses: ${ingresosMp3:0.00}");
            sb.AppendLine($"- Ingresos MP 6 meses: ${ingresosMp6:0.00}");
            sb.AppendLine($"- Ingresos por envíos (Normal): ${totalShippingNormal:0.00}");
            sb.AppendLine($"- Ingresos por envíos (Full): ${totalShippingFull:0.00}");
            sb.AppendLine($"- Depósitos Oxxo: ${totalDepositoOxxo:0.00}");
            sb.AppendLine($"- Depósitos Seven Eleven: ${totalDepositoSeven:0.00}");
            sb.AppendLine($"- Depósitos bancarios y Transferencias: ${totalDepositoBancarioYTransferencia:0.00}");
            sb.AppendLine($"- Tarjeta débito: ${totalDebito:0.00}");
            sb.AppendLine($"- Tarjeta crédito (contado): ${totalCreditoContado:0.00}");
            sb.AppendLine($"- Tarjeta crédito a MSI: ${totalCreditoMSI:0.00}");
            sb.AppendLine($"- Mercado Pago (contado): ${totalMPContado:0.00}");
            sb.AppendLine($"- Mercado Pago (crédito / meses): ${totalMPCredito:0.00}");
            sb.AppendLine();
            sb.AppendLine($"Ingresos por intereses: ${totalIntereses:0.00}");
            sb.AppendLine($"Ingresos por comisiones peso/volumen: ${totalComisionesPesoVol:0.00}");

            ShowStyledReport("Resumen de Mercado Libre", sb.ToString(), Color.FromArgb(255, 202, 40));
        }

        private void ShowStyledReport(string title, string content, Color accentColor)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.Size = new Size(560, 520);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = Color.White;
                dlg.Font = new Font("Segoe UI", 10f);

                Panel header = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = accentColor };
                Label lblTitle = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(16, 12),
                    BackColor = Color.Transparent
                };
                header.Controls.Add(lblTitle);

                TextBox txtReport = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Location = new Point(12, 62),
                    Size = new Size(520, 370),
                    Font = new Font("Consolas", 9.5f),
                    BackColor = Color.FromArgb(250, 252, 255),
                    ForeColor = Color.FromArgb(40, 40, 40),
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = content
                };

                Button btnClose = new Button
                {
                    Text = "Cerrar",
                    Size = new Size(120, 36),
                    Location = new Point(412, 440),
                    BackColor = accentColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    DialogResult = DialogResult.OK
                };
                btnClose.FlatAppearance.BorderSize = 0;

                dlg.Controls.Add(header);
                dlg.Controls.Add(txtReport);
                dlg.Controls.Add(btnClose);
                dlg.AcceptButton = btnClose;
                dlg.ShowDialog(this);
            }
        }

        private void SaveAccountPurchaseHistory()
        {
            // Account data is already saved in-memory via AccountManager.CurrentAccount
            // This method ensures the sidebar in Form8 refreshes when returning
        }
    }
}
