namespace Parking.Forms
{
    partial class Estadisticas
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chGrafico = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCrearImagen = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dtServicio = new System.Windows.Forms.DataGridView();
            this.dmAño = new System.Windows.Forms.NumericUpDown();
            this.panel29 = new System.Windows.Forms.Panel();
            this.lblImporte = new System.Windows.Forms.Label();
            this.lblVehiculos = new System.Windows.Forms.Label();
            this.radioImporte = new System.Windows.Forms.RadioButton();
            this.radioVehiculos = new System.Windows.Forms.RadioButton();
            this.label71 = new System.Windows.Forms.Label();
            this.btnGraficar = new System.Windows.Forms.Button();
            this.cbSemana = new System.Windows.Forms.ComboBox();
            this.cbFecha = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chGrafico)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtServicio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dmAño)).BeginInit();
            this.panel29.SuspendLayout();
            this.SuspendLayout();
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.LightGray;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(1002, 14);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(110, 37);
            this.button4.TabIndex = 12;
            this.button4.Text = "ATRAS";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(478, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 31);
            this.label1.TabIndex = 11;
            this.label1.Text = "Estadisticas";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Parking.Properties.Resources.logo4parking;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(213, 81);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.chGrafico);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Location = new System.Drawing.Point(12, 99);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1100, 508);
            this.panel2.TabIndex = 13;
            // 
            // chGrafico
            // 
            chartArea1.Name = "ChartArea1";
            this.chGrafico.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chGrafico.Legends.Add(legend1);
            this.chGrafico.Location = new System.Drawing.Point(3, 3);
            this.chGrafico.Name = "chGrafico";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chGrafico.Series.Add(series1);
            this.chGrafico.Size = new System.Drawing.Size(757, 500);
            this.chGrafico.TabIndex = 17;
            this.chGrafico.Text = "chart1";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btnCrearImagen);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Location = new System.Drawing.Point(762, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(333, 500);
            this.panel3.TabIndex = 16;
            // 
            // btnCrearImagen
            // 
            this.btnCrearImagen.BackColor = System.Drawing.Color.LightGray;
            this.btnCrearImagen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearImagen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrearImagen.Location = new System.Drawing.Point(125, 442);
            this.btnCrearImagen.Name = "btnCrearImagen";
            this.btnCrearImagen.Size = new System.Drawing.Size(110, 37);
            this.btnCrearImagen.TabIndex = 13;
            this.btnCrearImagen.Text = "CREAR IMAGEN";
            this.btnCrearImagen.UseVisualStyleBackColor = false;
            this.btnCrearImagen.Visible = false;
            this.btnCrearImagen.Click += new System.EventHandler(this.btnCrearImagen_Click);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.dtServicio);
            this.panel4.Controls.Add(this.dmAño);
            this.panel4.Controls.Add(this.panel29);
            this.panel4.Controls.Add(this.btnGraficar);
            this.panel4.Controls.Add(this.cbSemana);
            this.panel4.Controls.Add(this.cbFecha);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(7, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(321, 431);
            this.panel4.TabIndex = 1;
            // 
            // dtServicio
            // 
            this.dtServicio.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dtServicio.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtServicio.BackgroundColor = System.Drawing.Color.White;
            this.dtServicio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtServicio.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dtServicio.ColumnHeadersHeight = 30;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.MenuHighlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtServicio.DefaultCellStyle = dataGridViewCellStyle2;
            this.dtServicio.Location = new System.Drawing.Point(32, 191);
            this.dtServicio.Name = "dtServicio";
            this.dtServicio.ReadOnly = true;
            this.dtServicio.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dtServicio.RowHeadersVisible = false;
            this.dtServicio.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtServicio.Size = new System.Drawing.Size(253, 218);
            this.dtServicio.TabIndex = 41;
            // 
            // dmAño
            // 
            this.dmAño.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dmAño.Location = new System.Drawing.Point(223, 25);
            this.dmAño.Maximum = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            this.dmAño.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.dmAño.Name = "dmAño";
            this.dmAño.Size = new System.Drawing.Size(55, 22);
            this.dmAño.TabIndex = 36;
            this.dmAño.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dmAño.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.dmAño.ValueChanged += new System.EventHandler(this.dmAño_ValueChanged);
            // 
            // panel29
            // 
            this.panel29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel29.Controls.Add(this.lblImporte);
            this.panel29.Controls.Add(this.lblVehiculos);
            this.panel29.Controls.Add(this.radioImporte);
            this.panel29.Controls.Add(this.radioVehiculos);
            this.panel29.Controls.Add(this.label71);
            this.panel29.Location = new System.Drawing.Point(27, 111);
            this.panel29.Name = "panel29";
            this.panel29.Size = new System.Drawing.Size(255, 75);
            this.panel29.TabIndex = 34;
            // 
            // lblImporte
            // 
            this.lblImporte.AutoSize = true;
            this.lblImporte.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblImporte.Location = new System.Drawing.Point(174, 40);
            this.lblImporte.Name = "lblImporte";
            this.lblImporte.Size = new System.Drawing.Size(76, 16);
            this.lblImporte.TabIndex = 5;
            this.lblImporte.Text = "999999.99";
            this.lblImporte.Visible = false;
            // 
            // lblVehiculos
            // 
            this.lblVehiculos.AutoSize = true;
            this.lblVehiculos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblVehiculos.Location = new System.Drawing.Point(174, 15);
            this.lblVehiculos.Name = "lblVehiculos";
            this.lblVehiculos.Size = new System.Drawing.Size(40, 16);
            this.lblVehiculos.TabIndex = 4;
            this.lblVehiculos.Text = "9999";
            this.lblVehiculos.Visible = false;
            // 
            // radioImporte
            // 
            this.radioImporte.AutoSize = true;
            this.radioImporte.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioImporte.Location = new System.Drawing.Point(7, 38);
            this.radioImporte.Name = "radioImporte";
            this.radioImporte.Size = new System.Drawing.Size(94, 20);
            this.radioImporte.TabIndex = 3;
            this.radioImporte.Text = "IMPORTE";
            this.radioImporte.UseVisualStyleBackColor = true;
            // 
            // radioVehiculos
            // 
            this.radioVehiculos.AutoSize = true;
            this.radioVehiculos.Checked = true;
            this.radioVehiculos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioVehiculos.Location = new System.Drawing.Point(5, 15);
            this.radioVehiculos.Name = "radioVehiculos";
            this.radioVehiculos.Size = new System.Drawing.Size(111, 20);
            this.radioVehiculos.TabIndex = 2;
            this.radioVehiculos.TabStop = true;
            this.radioVehiculos.Text = "VEHICULOS";
            this.radioVehiculos.UseVisualStyleBackColor = true;
            // 
            // label71
            // 
            this.label71.AutoSize = true;
            this.label71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label71.Location = new System.Drawing.Point(-2, -1);
            this.label71.Name = "label71";
            this.label71.Size = new System.Drawing.Size(77, 13);
            this.label71.TabIndex = 1;
            this.label71.Text = "VEHICULOS";
            // 
            // btnGraficar
            // 
            this.btnGraficar.BackColor = System.Drawing.Color.YellowGreen;
            this.btnGraficar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGraficar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGraficar.Location = new System.Drawing.Point(27, 81);
            this.btnGraficar.Name = "btnGraficar";
            this.btnGraficar.Size = new System.Drawing.Size(255, 24);
            this.btnGraficar.TabIndex = 32;
            this.btnGraficar.Text = "GRAFICAR";
            this.btnGraficar.UseVisualStyleBackColor = false;
            this.btnGraficar.Click += new System.EventHandler(this.btnGraficar_Click);
            // 
            // cbSemana
            // 
            this.cbSemana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSemana.FormattingEnabled = true;
            this.cbSemana.Location = new System.Drawing.Point(27, 54);
            this.cbSemana.Name = "cbSemana";
            this.cbSemana.Size = new System.Drawing.Size(255, 21);
            this.cbSemana.TabIndex = 13;
            this.cbSemana.SelectedIndexChanged += new System.EventHandler(this.cbSemana_SelectedIndexChanged);
            // 
            // cbFecha
            // 
            this.cbFecha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFecha.FormattingEnabled = true;
            this.cbFecha.Location = new System.Drawing.Point(76, 27);
            this.cbFecha.Name = "cbFecha";
            this.cbFecha.Size = new System.Drawing.Size(132, 21);
            this.cbFecha.TabIndex = 11;
            this.cbFecha.SelectedIndexChanged += new System.EventHandler(this.cbFecha_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(24, 27);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(46, 16);
            this.label16.TabIndex = 10;
            this.label16.Text = "Fecha";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mostrar Semana del Dia";
            // 
            // Estadisticas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1124, 619);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Estadisticas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Estadisticas";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chGrafico)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtServicio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dmAño)).EndInit();
            this.panel29.ResumeLayout(false);
            this.panel29.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbFecha;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cbSemana;
        private System.Windows.Forms.Button btnGraficar;
        private System.Windows.Forms.Panel panel29;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.RadioButton radioImporte;
        private System.Windows.Forms.RadioButton radioVehiculos;
        private System.Windows.Forms.Button btnCrearImagen;
        private System.Windows.Forms.NumericUpDown dmAño;
        private System.Windows.Forms.Label lblImporte;
        private System.Windows.Forms.Label lblVehiculos;
        private System.Windows.Forms.DataVisualization.Charting.Chart chGrafico;
        private System.Windows.Forms.DataGridView dtServicio;
    }
}