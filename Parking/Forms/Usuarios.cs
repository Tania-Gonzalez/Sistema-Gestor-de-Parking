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
    public partial class Usuarios : Form
    {
        #region Variables
        ModeloDatos oSettings;

        private string Conexion;
        private int IDUsuario;
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        #endregion
        public Usuarios(string conexion)
        {
            this.Conexion = conexion;
            InitializeComponent();
            oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;
            lblFechaInstalacion.Text = oSettings.FechaInicio;
            lblFechaTermino.Text = oSettings.FechaTermino;
            CargarTabla();
        }
        public void CargarTabla()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from us in context.usuario
                                     join ap in context.areas_personal on us.ID_AreaPersonal equals ap.ID_AreaPersonal                                   
                                     select new Modelos.ModeloUsuarios
                                     {
                                        IDU = us.ID_Usuario,
                                        Privilegios = us.Privilegios,
                                        Usuario=us.Usuario1,
                                        Nombre = us.Nombre,
                                        Telefono=us.Telefono,
                                        AreaPersonal=ap.AreaPersonal,
                                        Puesto=us.Puesto,
                                        CorreoE=us.mail_Correo

                                     }).AsQueryable();

                dtUsuarios.DataSource = listabusqueda.ToList();
                Console.WriteLine(listabusqueda.Count());
            }

            
        }
        private int? GetID()
        {
            try
            {
                var a = dtUsuarios.Rows[dtUsuarios.CurrentRow.Index].Cells[0].Value.ToString();
                IDUsuario = Int32.Parse(a);
                return IDUsuario;
            }
            catch
            {
                return null;

            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            using (var a = new NuevoUsuario(Conexion))
            {
                a.Refresh_ += CargarTabla;
                a.ShowDialog();
                

            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                using (var a = new Forms.ActualizarUsuario(Conexion,id.GetValueOrDefault()))
                {
                    a.Refresh_ += CargarTabla;
                    a.ShowDialog();

                }
            }
            
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Usuarios_Load(object sender, EventArgs e)
        {

        }

        private void btnLicencia_Click(object sender, EventArgs e)
        {
            this.Size = new Size(829,642);
            this.CenterToScreen();
        }

        private void ComprobarSerial_Click(object sender, EventArgs e)
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
    }
}
