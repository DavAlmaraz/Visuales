namespace Examen
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbProductos = new System.Windows.Forms.ComboBox();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.lstCarrito = new System.Windows.Forms.ListBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnVerCarrito = new System.Windows.Forms.Button();
            this.grpContado = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(238, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(488, 51);
            this.label1.TabIndex = 2;
            this.label1.Text = "SISTEMA DE VENTAS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(501, 51);
            this.label2.TabIndex = 3;
            this.label2.Text = "PRODUCTOS                 ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(252, 51);
            this.label3.TabIndex = 4;
            this.label3.Text = "Producto:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 269);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(244, 51);
            this.label4.TabIndex = 5;
            this.label4.Text = "Cantidad:";
            // 
            // cmbProductos
            // 
            this.cmbProductos.FormattingEnabled = true;
            this.cmbProductos.Location = new System.Drawing.Point(285, 238);
            this.cmbProductos.Name = "cmbProductos";
            this.cmbProductos.Size = new System.Drawing.Size(121, 24);
            this.cmbProductos.TabIndex = 6;
            // 
            // numCantidad
            // 
            this.numCantidad.Location = new System.Drawing.Point(286, 290);
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Size = new System.Drawing.Size(120, 22);
            this.numCantidad.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(720, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(398, 51);
            this.label5.TabIndex = 8;
            this.label5.Text = "CARRITO              ";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // lstCarrito
            // 
            this.lstCarrito.FormattingEnabled = true;
            this.lstCarrito.ItemHeight = 16;
            this.lstCarrito.Location = new System.Drawing.Point(695, 176);
            this.lstCarrito.Name = "lstCarrito";
            this.lstCarrito.Size = new System.Drawing.Size(275, 212);
            this.lstCarrito.TabIndex = 9;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Font = new System.Drawing.Font("Impact", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregar.Location = new System.Drawing.Point(21, 341);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(294, 47);
            this.btnAgregar.TabIndex = 46;
            this.btnAgregar.Text = "Agregar al carrito";
            this.btnAgregar.UseVisualStyleBackColor = true;
            // 
            // btnVerCarrito
            // 
            this.btnVerCarrito.Font = new System.Drawing.Font("Impact", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerCarrito.Location = new System.Drawing.Point(321, 341);
            this.btnVerCarrito.Name = "btnVerCarrito";
            this.btnVerCarrito.Size = new System.Drawing.Size(294, 47);
            this.btnVerCarrito.TabIndex = 47;
            this.btnVerCarrito.Text = "Ver carrito";
            this.btnVerCarrito.UseVisualStyleBackColor = true;
            // 
            // grpContado
            // 
            this.grpContado.Location = new System.Drawing.Point(21, 457);
            this.grpContado.Name = "grpContado";
            this.grpContado.Size = new System.Drawing.Size(949, 100);
            this.grpContado.TabIndex = 48;
            this.grpContado.TabStop = false;
            this.grpContado.Text = "groupBox1";
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 634);
            this.Controls.Add(this.grpContado);
            this.Controls.Add(this.btnVerCarrito);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.lstCarrito);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numCantidad);
            this.Controls.Add(this.cmbProductos);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form4";
            this.Text = "Form4";
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbProductos;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lstCarrito;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnVerCarrito;
        private System.Windows.Forms.GroupBox grpContado;
    }
}