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
    public partial class ImporteCorte : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string Usuario;
        DateTime hoy;
        public delegate void RF();
        public event RF Refresh_;
        #endregion
        public ImporteCorte(string usu)
        {
            this.Usuario = usu;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

        }

        private void Cerrar_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            txtImporte.Text = (string.IsNullOrEmpty(txtImporte.Text)) ?"0":txtImporte.Text;

        }
        private string Cultura()
        {
            var info = CultureInfo.CurrentCulture.NumberFormat;
            return info.NumberDecimalSeparator;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {


                string fechahoy = DateTime.Today.ToString("d");
                string horaenpunto = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                hoy = DateTime.Parse(fechahoy);

                var cobroparqueo = (from bp in context.banco_parking
                                    where bp.FechaEntrada == hoy && bp.Estancia != "EN PROGRESO" && bp.Corte == 0 && bp.ID_PensionU == 9
                                    select bp.Importe).Sum();
                cobroparqueo = (cobroparqueo == null) ? 0 : cobroparqueo;
                var cobropu = (from bp in context.banco_parking
                               where bp.FechaEntrada == hoy && bp.Estancia != "EN PROGRESO" && bp.Corte == 0 && bp.ID_PensionU != 9
                               select bp.Importe).Sum();
                cobropu = (cobropu == null) ? 0 : cobropu;
                var cobropension = (from bps in context.banco_pension
                                    where bps.StatusPago == true && bps.Pension_Corte == false && bps.FechaUltimoPago == hoy
                                    select bps.Pagos).Sum();
                cobropension = (cobropension == null) ? 0 : cobropension;
                var nombreU = context.usuario.Where(n => n.Usuario1 == Usuario).Select(n => n.Nombre).FirstOrDefault();

                var penal = (from bit in context.bitacora_pensiones
                             where bit.Fecha_salida == hoy && bit.Penalizacion == true
                             select bit.Monto_Penalizacion).Sum();
                penal = (penal==null) ?0:penal;

                var reportado = (Cultura() == ",") ? Convert.ToDecimal(txtImporte.Text.Replace('.', ',')) : Convert.ToDecimal(txtImporte.Text);

                var cortesin = new cortes()
                {
                    Usuario = Usuario,
                    FechaCorte = hoy,
                    Importe_Parqueo = cobroparqueo + cobropu,
                    Importe_Cortes = cobropu + cobroparqueo + cobropension,
                    Importe_Reportado = reportado, 
                    StatusCorte = false,
                    CorteNormal = cobroparqueo,
                    CorteTarifa2 = cobropu,
                    CortePension = cobropension,
                    CortePenalizacion = penal

                };
                context.cortes.Add(cortesin);
                context.SaveChanges();

            };
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                var maxidcorte = context.cortes.Max(x => x.Id_Corte);

                var parqueoupd = (from bp in context.banco_parking
                                  where bp.FechaEntrada == hoy && bp.Corte == 0 && bp.Estancia != "EN PROGRESO"
                                  select bp).ToList();
                var pensionupd = (from bps in context.banco_pension
                                  where bps.StatusPago == true && bps.Pension_Corte == false && bps.FechaUltimoPago == hoy
                                  select bps).ToList();
                foreach (var par in parqueoupd)
                {
                    par.Corte = 1;
                    par.Id_corte = maxidcorte;

                }
                foreach (var pen in pensionupd)
                {
                    pen.Pension_Corte = true;
                    pen.Id_corte = maxidcorte;
                }
                MessageBox.Show("Corte Realizado Correctamente");
                context.SaveChanges();
                Refresh_();
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
