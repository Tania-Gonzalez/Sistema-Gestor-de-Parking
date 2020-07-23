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
    public partial class PanelControl : Form
    {
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        private string Conexion,Usuario;
        public delegate void RF();
        public event RF Refresh_;

        private void btnAtras_Click(object sender, EventArgs e)
        {
           
            this.Close();
        }

        private void btnCorte_Click(object sender, EventArgs e)
        {
            using (var a = new Corte(Usuario))
            {
                a.ShowDialog();

            }
        }

        private void btnPension_Click(object sender, EventArgs e)
        {
            using (var a = new Pensiones(Conexion, Usuario))
            {
                a.Refresh_ += Refresh_Logo;
                a.ShowDialog();

            }
        }

        private void Refresh_Logo()
        {
        
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            using (var a = new Estadisticas(Conexion))
            {
                a.ShowDialog();

            }

        }

        private void btnConfiguraciones_Click(object sender, EventArgs e)
        {
            using (var a = new Configuraciones(Conexion, Usuario))
            {
                a.ShowDialog();

            }

        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            using (var a = new Usuarios(Conexion))
            {
                a.ShowDialog();

            }
        }

        public PanelControl(string Conexion, string Usuario)
        {
            this.Conexion = Conexion;
            this.Usuario = Usuario;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            CargarBotones();
        }

        private void PanelControl_Load(object sender, EventArgs e)
        {

        }

        private void PanelControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            Refresh_();
        }

        private void CargarBotones()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                
                var usuario = (from us in context.usuario where (us.Usuario1 == Usuario) select us.Privilegios).FirstOrDefault();
                if (usuario!= "Administrador")
                {
                    this.Size = new Size(341,248);
                   
                }
            }
           
        }
    }
}
