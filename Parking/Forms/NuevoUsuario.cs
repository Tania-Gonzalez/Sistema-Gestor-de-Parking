using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class NuevoUsuario : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;

        string mensajeError = "Error Inesperado Contacte al Administrador!";
        public delegate void RF();
        public event RF Refresh_;
        private string Conexion;
        private int IDAreaPersonal;
        private int? IDServidor;
        #endregion
        public NuevoUsuario(string conexion)
        {
            this.Conexion = conexion;
            InitializeComponent(); 
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            CargarAreas();
            CargarProvedores();
            scPrioridad.SelectedIndex = 0;
        }
    
        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txtUsuario.Text)|| string.IsNullOrEmpty(txtContra1.Text)|| string.IsNullOrEmpty(txtContra2.Text)|| string.IsNullOrEmpty(txtNombre.Text)|| string.IsNullOrEmpty(txtTelefono.Text)|| string.IsNullOrEmpty(txtPuesto.Text)|| string.IsNullOrEmpty(txtCorreo.Text))
            {
                MessageBox.Show("Faltan campos por llenar"); return false;
            }
            else
            {
                if (txtContra1.Text!=txtContra2.Text) 
                { 
                    MessageBox.Show("Las contraseñas no coinciden"); return false; 
                }
                else
                {   using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var us = txtUsuario.Text.Trim();
                        var a = context.usuario.Where(n => n.Usuario1 == us).Count();
                        if (a>0) { MessageBox.Show("El usuario ya existe seleccione otro nombre"); return false; }
                        else { return true; }
                     }
                }

            }

        }
        private bool ValidarCheck()
        {
            if (ckCorreo.Checked==true)
            {
                if (string.IsNullOrEmpty(txtContraseñaServidor.Text)) 
                {

                    MessageBox.Show("Faltan campos por llenar"); return false;

                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }
        }

        private void CargarProvedores()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.proveedorescorreo.ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Dominio;
                    var id = valores.ID_ProveedoresCorreo;
                    diccionario.Add(id.ToString(), nombre);


                }
                scServidor.DataSource = new BindingSource(diccionario, null);
                scServidor.DisplayMember = "Value";
                scServidor.ValueMember = "Key";
            }
            scServidor.SelectedIndex = 0;
        }
        private void CargarAreas()
        {
           
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.areas_personal.ToList();
             
                foreach (var valores in item)
                {
                    var nombre = valores.AreaPersonal;
                    var id = valores.ID_AreaPersonal;
                    diccionario.Add(id.ToString(), nombre);


                }
                scArea.DataSource = new BindingSource(diccionario, null);
                scArea.DisplayMember = "Value";
                scArea.ValueMember = "Key";
            }
            scArea.SelectedIndex = 0;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
            if (ValidarCheck() == true && ValidarCampos() == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = new usuario()
                    {
                        Privilegios = scPrioridad.Text,
                        Usuario1 = txtUsuario.Text.Trim(),
                        Password = txtContra1.Text,
                        Nombre = txtNombre.Text,
                        Telefono = txtTelefono.Text,
                        ID_AreaPersonal = IDAreaPersonal,
                        Puesto = txtPuesto.Text.Trim(),
                        mail_Correo = txtCorreo.Text,
                        mail_Password = (ckCorreo.Checked==true) ?txtContraseñaServidor.Text:null,
                        CorreoConfirmado=false,
                        ID_ProveedoresCorreo = (ckCorreo.Checked == true) ? IDServidor : null,
                        


                    };

                    context.usuario.Add(a);
                    context.SaveChanges();
                    MessageBox.Show("Insercion Correcta");
                
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

        private void scArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDAreaPersonal = Int32.Parse((((KeyValuePair<string, string>)scArea.SelectedItem).Key));
        }

        private void scServidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDServidor = Int32.Parse((((KeyValuePair<string, string>)scServidor.SelectedItem).Key));

        }

        private void ckCorreo_CheckedChanged(object sender, EventArgs e)
        {
            label13.Enabled = (ckCorreo.Checked==true) ?true:false;
            label11.Enabled = (ckCorreo.Checked == true) ? true : false;
            scServidor.Enabled = (ckCorreo.Checked == true) ? true : false;
            txtContraseñaServidor.Enabled = (ckCorreo.Checked == true) ? true : false;
            checkBox2.Enabled = (ckCorreo.Checked == true) ? true : false;
            btnConfirmarCorreo.Enabled = (ckCorreo.Checked == true) ? true : false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseñaServidor.PasswordChar = (checkBox2.Checked==false) ?'*':Convert.ToChar(0);
        }

        private void btnConfirmarCorreo_Click(object sender, EventArgs e)
        {
            try
            {
            if (string.IsNullOrEmpty(txtCorreo.Text) || string.IsNullOrEmpty(txtContraseñaServidor.Text))
            {
                MessageBox.Show("Coloque el correo y contraseña");
            }
            else
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(txtCorreo.Text.Trim());
                msg.Subject = "Correo Prueba";
                msg.SubjectEncoding = Encoding.UTF8;

                msg.Body = "Correo Prueba";
                msg.BodyEncoding = Encoding.UTF8;
                msg.From = new MailAddress(txtCorreo.Text.Trim());

                SmtpClient cliente = new SmtpClient();
                cliente.Credentials = new NetworkCredential(txtCorreo.Text.Trim(), txtContraseñaServidor.Text.Trim());

                var provedor = new proveedorescorreo();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    provedor = context.proveedorescorreo.Where(n => n.ID_ProveedoresCorreo == IDServidor).FirstOrDefault();
                }
                cliente.Port = provedor.Puerto.GetValueOrDefault();
                cliente.EnableSsl = true;
                cliente.Host = provedor.Servidor_SMTP;
                try
                {
                    cliente.Send(msg);
                    MessageBox.Show("Mensaje Enviado Correctamente!");

                }
                catch (Exception)
                {
                    MessageBox.Show("Error al enviar\n" +
                        "Cuentas de Gmail deben garantizar el permiso a aplicaciones menos seguras\n"
                        + "Cuentas de Outlook deben estar verificadas con numero de telefono");

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

        private void btnSalir_Click(object sender, EventArgs e)
        {
          
            this.Close();
        }

        private void NuevoUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Refresh_();
        }

        private void NuevoUsuario_Load(object sender, EventArgs e)
        {

        }
    }
}
