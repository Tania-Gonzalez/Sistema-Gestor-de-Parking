using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Sql;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class Login : Form
    {

        #region Variables
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        private string servidor = "";
        private string cd;
        bool? validador =null;
        bool cerrar = true;

        public delegate void CD(string t,string u);
        public event CD Cadena;
        #endregion

        #region Constructor
        public Login()
        {
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

        }

        #endregion

        #region Metodos



        public async Task<int> Validar()
        {
            string nom = UsuarioText.Text.Trim();
            string pass = ContraseniaText.Text.Trim();
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = (from us in context.usuario where us.Usuario1 == nom && us.Password == pass select us.ID_Usuario).Count();
                var retorno = (a > 0) ? 1 : 0;
                return retorno; 
            }
                 
        }

        #endregion


        #region MetodosForm
      
        private void Conectar_Click(object sender, EventArgs e)
        {
            try 
            {
                       Task.Run(async () =>
                {
                    Task<int> resultado = Validar();
                    if (await resultado == 1) { validador = true; }                    

                    if (await resultado == 0) { validador = false;}
                    
                }).GetAwaiter().GetResult();


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
        


        private void Login_Load(object sender, EventArgs e)
        {
            
        }

        private void Cerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void minimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        private void Servidor_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (validador==false) { validador = null; MessageBox.Show("Usuario o Contraseña Incorrectos"); }
            if(validador==true)
            {
                
                    Cadena(cd, UsuarioText.Text);
                    cerrar = false;
                    this.Close();
             }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrar==true) {
                Environment.Exit(0);
            }
        }
    }
}
