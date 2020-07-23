using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class Caducada : Form
    {
        string Serial, Conexion, Tipo;
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        ModeloDatos oSettings;
        string mensajeError = "Error Inesperado Contacte al Administrador!";


        private void Caducada_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        public Caducada()
        {
            InitializeComponent();
            oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));

        }

        private void ComprobarSerial_Click(object sender, EventArgs e)
        {

            try
            {
                oSettings.Serial = txtSerial.Text.Trim();
                oSettings.Tipo = "Enterprise";
                var hoy = DateTime.Today.ToString();
                oSettings.FechaInicio = hoy;
                var termino = (DateTime.Today.AddDays(7)).ToString();
                oSettings.FechaTermino = termino;
                var json = JsonConvert.SerializeObject(oSettings, Formatting.None);
                File.WriteAllText(rutaJson, json);
                var result = MessageBox.Show("Bienvenido", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = path + @"\Error.txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
                MessageBox.Show(mensajeError);

            }

        }
    }
}
