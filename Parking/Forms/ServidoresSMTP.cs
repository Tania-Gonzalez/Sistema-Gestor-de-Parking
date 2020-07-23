using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class ServidoresSMTP : Form
    {
        #region Variables
        int ID;
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        #endregion
        public ServidoresSMTP()
        {
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            CargarTabla();

        }

        private void CargarTabla()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from pc in context.proveedorescorreo
                                     select new Modelos.ModeloServidoresSMTP
                                     {
                                         ID = pc.ID_ProveedoresCorreo,
                                         Dominio = pc.Dominio,
                                         Puerto = pc.Puerto,
                                         ServidorSMTP = pc.Servidor_SMTP,

                                     }).AsQueryable();
                dtServidores.DataSource = listabusqueda.ToList();
                if (dtServidores.Rows.Count > 0)
                {
                    dtServidores.Rows[0].Selected = true;

                }
                int? id = GetID();
                if (id != null)
                {
                    var datos = context.proveedorescorreo.Where(n => n.ID_ProveedoresCorreo == id).FirstOrDefault();
                    lblID.Text = datos.ID_ProveedoresCorreo.ToString();
                    txtServidorSMTP.Text = datos.Servidor_SMTP;
                    txtPuerto.Text = datos.Puerto.ToString();
                
                }
        
            }
        }
      
        private int? GetID()
        {
            try
            {
                var a = dtServidores.Rows[dtServidores.CurrentRow.Index].Cells[0].Value.ToString();
                ID = Int32.Parse(a);
                return ID;
            }
            catch
            {
                return null;

            }
        }

        private void dtServidores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var datos = context.proveedorescorreo.Where(n => n.ID_ProveedoresCorreo == id).FirstOrDefault();
                    lblID.Text = datos.ID_ProveedoresCorreo.ToString();
                    txtServidorSMTP.Text = datos.Servidor_SMTP;
                    txtPuerto.Text = datos.Puerto.ToString();
                }
                    

            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDominioCrear.Text))
            {
                MessageBox.Show("Ingrese un Dominio");

            }
            else
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = new proveedorescorreo()
                    {
                        Dominio = txtDominioCrear.Text.Trim(),
                    };
                    context.proveedorescorreo.Add(a);
                    context.SaveChanges();
                    CargarTabla();
                }

                
            }
        }
        public void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }
    }
}
