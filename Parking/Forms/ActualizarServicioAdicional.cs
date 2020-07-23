using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class ActualizarServicioAdicional : Form
    {
        int ID;
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";

        public delegate void RF();
        public event RF Refresh_;
        public ActualizarServicioAdicional(int id)
        {
            ID = id;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;
            Cargar();
        }
        public void SoloNumerosDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }


            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        public bool ComprobarNombre()
        {
            var nombreServicio = txtNombreServicio.Text.Trim();

            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var query = (from sa in context.serviciosadicionales where sa.ServicioAdicional == nombreServicio && sa.ID_ServicioAd!=ID  select sa.ID_ServicioAd).Count();

                if (query > 0)
                {
                    MessageBox.Show("El nombre que intenta registrar ya existe");
                    return false;
                }


            }
            return true;
        }
        private void txtImporte_TextChanged(object sender, EventArgs e)
        {
            txtImporte.Text = (string.IsNullOrEmpty(txtImporte.Text.Trim())) ? "0" : txtImporte.Text;
        }
        private string Cultura()
        {
            var info = CultureInfo.CurrentCulture.NumberFormat;
            return info.NumberDecimalSeparator;
        }
        public bool ComprobarCampos()
        {
            if (string.IsNullOrEmpty(txtImporte.Text.Trim()) || string.IsNullOrEmpty(txtNombreServicio.Text.Trim()))
            {
                MessageBox.Show("Llene todos los campos");
                return false;
            }

            return true;
        }
        public void Cargar()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.serviciosadicionales.SingleOrDefault(n => n.ID_ServicioAd == ID);
                txtNombreServicio.Text = a.ServicioAdicional;
                txtImporte.Text = a.Precio_ServiciosAd.ToString();
                dmTiempo.Value = Convert.ToDecimal(a.Tiempo_Gracia);
                ckEstado.Checked = a.ServicioAd_Activo.GetValueOrDefault();

            }
        }

        private void ActualizarServicioAdicional_FormClosing(object sender, FormClosingEventArgs e)
        {
            Refresh_();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ComprobarNombre() == true && ComprobarCampos() == true)
                {
                    string costoFormato = txtImporte.Text.Trim();
                    costoFormato = (Cultura() == ",") ? costoFormato.Replace('.', ',') : costoFormato;
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var a = context.serviciosadicionales.SingleOrDefault(n => n.ID_ServicioAd == ID);
                        if (a != null)
                        {
                            a.ServicioAdicional = txtNombreServicio.Text.Trim();
                            a.ServicioAd_Activo = ckEstado.Checked;
                            a.Precio_ServiciosAd = Convert.ToDecimal(costoFormato);
                            a.Tiempo_Gracia = Convert.ToInt32(dmTiempo.Value);
                            context.SaveChanges();
                            MessageBox.Show("Actualizacion Correcta");
                            this.Dispose();


                        }
                    }
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
