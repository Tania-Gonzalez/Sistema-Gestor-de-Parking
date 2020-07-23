namespace Parking.Forms
{
    partial class Caducada
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
            this.panelSerial = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.ComprobarSerial = new System.Windows.Forms.Button();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.panelSerial.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(141, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(543, 55);
            this.label1.TabIndex = 2;
            this.label1.Text = "Su licencia ha expirado";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(137, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(556, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "Seleccione una licencia nueva para su sistema";
            // 
            // panelSerial
            // 
            this.panelSerial.Controls.Add(this.label6);
            this.panelSerial.Controls.Add(this.ComprobarSerial);
            this.panelSerial.Controls.Add(this.txtSerial);
            this.panelSerial.Location = new System.Drawing.Point(105, 160);
            this.panelSerial.Name = "panelSerial";
            this.panelSerial.Size = new System.Drawing.Size(579, 122);
            this.panelSerial.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(86, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Serial:";
            // 
            // ComprobarSerial
            // 
            this.ComprobarSerial.Location = new System.Drawing.Point(448, 86);
            this.ComprobarSerial.Name = "ComprobarSerial";
            this.ComprobarSerial.Size = new System.Drawing.Size(75, 23);
            this.ComprobarSerial.TabIndex = 8;
            this.ComprobarSerial.Text = "Comprobar";
            this.ComprobarSerial.UseVisualStyleBackColor = true;
            this.ComprobarSerial.Click += new System.EventHandler(this.ComprobarSerial_Click);
            // 
            // txtSerial
            // 
            this.txtSerial.Location = new System.Drawing.Point(128, 39);
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(395, 20);
            this.txtSerial.TabIndex = 13;
            // 
            // Caducada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 332);
            this.Controls.Add(this.panelSerial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Caducada";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Licencia Terminada";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Caducada_FormClosing);
            this.panelSerial.ResumeLayout(false);
            this.panelSerial.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelSerial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ComprobarSerial;
        private System.Windows.Forms.TextBox txtSerial;
    }
}