namespace Parking
{
    partial class CrearBase
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
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Servidor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pass = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.TextBox();
            this.ComprobarServidor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.panelServidor = new System.Windows.Forms.Panel();
            this.panelSerial = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.ComprobarSerial = new System.Windows.Forms.Button();
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.panelPrincipal = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panelServidor.SuspendLayout();
            this.panelSerial.SuspendLayout();
            this.panelPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(276, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 35);
            this.label3.TabIndex = 7;
            this.label3.Text = "Bienvenido";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(109, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Servidor:";
            // 
            // Servidor
            // 
            this.Servidor.Location = new System.Drawing.Point(89, 33);
            this.Servidor.Name = "Servidor";
            this.Servidor.Size = new System.Drawing.Size(100, 20);
            this.Servidor.TabIndex = 13;
            this.Servidor.Text = "localhost";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(437, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Contraseña:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(274, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Usuario:";
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(423, 33);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(100, 20);
            this.pass.TabIndex = 10;
            // 
            // user
            // 
            this.user.Location = new System.Drawing.Point(254, 33);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(100, 20);
            this.user.TabIndex = 9;
            // 
            // ComprobarServidor
            // 
            this.ComprobarServidor.Location = new System.Drawing.Point(448, 86);
            this.ComprobarServidor.Name = "ComprobarServidor";
            this.ComprobarServidor.Size = new System.Drawing.Size(75, 23);
            this.ComprobarServidor.TabIndex = 8;
            this.ComprobarServidor.Text = "Comprobar";
            this.ComprobarServidor.UseVisualStyleBackColor = true;
            this.ComprobarServidor.Click += new System.EventHandler(this.ComprobarServidor_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(98, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(522, 24);
            this.label5.TabIndex = 15;
            this.label5.Text = "Coloque sus credenciales para la configuracion inicial";
            // 
            // panelServidor
            // 
            this.panelServidor.Controls.Add(this.label7);
            this.panelServidor.Controls.Add(this.label1);
            this.panelServidor.Controls.Add(this.user);
            this.panelServidor.Controls.Add(this.ComprobarServidor);
            this.panelServidor.Controls.Add(this.pass);
            this.panelServidor.Controls.Add(this.Servidor);
            this.panelServidor.Controls.Add(this.label4);
            this.panelServidor.Controls.Add(this.label2);
            this.panelServidor.Location = new System.Drawing.Point(0, 0);
            this.panelServidor.Name = "panelServidor";
            this.panelServidor.Size = new System.Drawing.Size(579, 122);
            this.panelServidor.TabIndex = 16;
            // 
            // panelSerial
            // 
            this.panelSerial.Controls.Add(this.label6);
            this.panelSerial.Controls.Add(this.ComprobarSerial);
            this.panelSerial.Controls.Add(this.txtSerial);
            this.panelSerial.Location = new System.Drawing.Point(0, 0);
            this.panelSerial.Name = "panelSerial";
            this.panelSerial.Size = new System.Drawing.Size(579, 122);
            this.panelSerial.TabIndex = 17;
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
            // panelPrincipal
            // 
            this.panelPrincipal.Controls.Add(this.panelSerial);
            this.panelPrincipal.Controls.Add(this.panelServidor);
            this.panelPrincipal.Location = new System.Drawing.Point(68, 104);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Size = new System.Drawing.Size(579, 122);
            this.panelPrincipal.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(354, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 24);
            this.label7.TabIndex = 16;
            this.label7.Text = "Espere...";
            this.label7.Visible = false;
            // 
            // CrearBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 265);
            this.Controls.Add(this.panelPrincipal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "CrearBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crear Base";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CrearBase_FormClosing);
            this.panelServidor.ResumeLayout(false);
            this.panelServidor.PerformLayout();
            this.panelSerial.ResumeLayout(false);
            this.panelSerial.PerformLayout();
            this.panelPrincipal.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Servidor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.TextBox user;
        private System.Windows.Forms.Button ComprobarServidor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelServidor;
        private System.Windows.Forms.Panel panelSerial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ComprobarSerial;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Panel panelPrincipal;
        private System.Windows.Forms.Label label7;
    }
}