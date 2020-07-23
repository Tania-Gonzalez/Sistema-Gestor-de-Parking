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
    public partial class ActualizarUsuario : Form
    {
        #region Variables
        string mensajeError = "Error Inesperado Contacte al Administrador!";

        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        public delegate void RF();
        public event RF Refresh_;
        private string Conexion;
        private int IDAreaPersonal,IDUsuario;
        private int IDServidor;
        #endregion
        public ActualizarUsuario(string conexion,int idUsuario)
        {
            this.Conexion = conexion;
            this.IDUsuario = idUsuario;
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            InitializeComponent();
            CargarAreas();
            CargarProvedores();
            CargarDatos();
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

        private void ckCorreo_CheckedChanged(object sender, EventArgs e)
        {
            label13.Enabled = (ckCorreo.Checked == true) ? true : false;
            label11.Enabled = (ckCorreo.Checked == true) ? true : false;
            scServidor.Enabled = (ckCorreo.Checked == true) ? true : false;
            txtContraseñaServidor.Enabled = (ckCorreo.Checked == true) ? true : false;
            checkBox2.Enabled = (ckCorreo.Checked == true) ? true : false;
            btnConfirmarCorreo.Enabled = (ckCorreo.Checked == true) ? true : false;
        }

        private void CargarDatos()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                var tUsuario = (from us in context.usuario
                                join ap in context.areas_personal on us.ID_AreaPersonal equals ap.ID_AreaPersonal
                                where us.ID_Usuario == IDUsuario
                                select new Modelos.ModeloUsuarios
                                {
                                  IDU=us.ID_Usuario,
                                  Privilegios=us.Privilegios,
                                  Usuario=us.Usuario1,
                                  Nombre=us.Nombre,
                                  Telefono=us.Telefono,
                                  AreaPersonal=ap.AreaPersonal,
                                  Puesto=us.Puesto,
                                  CorreoE=us.mail_Correo                                  
                                }).ToList();
                foreach (var item in tUsuario)
                {
                    scPrioridad.SelectedIndex = (item.Privilegios== "Empleador") ?0:1;
                    txtUsuario.Text = item.Usuario;
                    txtContra1.Text = item.Password;
                    txtNombre.Text = item.Nombre;
                    txtTelefono.Text = item.Telefono;
                    txtCorreo.Text = item.CorreoE;
                    txtPuesto.Text = item.Puesto;
                    switch (item.AreaPersonal)
                    {
                        case "Administracion":
                            scArea.SelectedIndex = 0;
                            break;
                        case "Almacen":
                            scArea.SelectedIndex = 1;
                            break;
                        case "Facturacion":
                            scArea.SelectedIndex = 2;
                            break;
                        case "Otros":
                            scArea.SelectedIndex = 3;
                            break;
                        case "Ventas":
                            scArea.SelectedIndex = 4;
                            break;

                    }

                }

            }
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.usuario.Where(n => n.ID_Usuario == IDUsuario).FirstOrDefault();
                if (a.proveedorescorreo!=null) 
                {
                    ckCorreo.Checked = true;
                    IDServidor = a.ID_ProveedoresCorreo.GetValueOrDefault();
                    var b = context.proveedorescorreo.Where(n => n.ID_ProveedoresCorreo == IDServidor).FirstOrDefault();
                    switch(b.Dominio)
                    {
                        case "@gmail.com":
                            scServidor.SelectedIndex = 0;
                            break;
                        case "@yahoo.com":
                            scServidor.SelectedIndex = 1;
                            break;
                        case "@hotmail.com":
                            scServidor.SelectedIndex = 2;
                            break;
                        case "@outlook.com":
                            scServidor.SelectedIndex = 3;
                            break;
                        case "@1and1.com":
                            scServidor.SelectedIndex = 4;
                            break;
                  

                    }
                    
                }
            
            }
            
        }
        private bool ValidarCheck()
        {
            if (ckCorreo.Checked == true)
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

            if (ValidarCheck() == true && ValidarCampos() == true)
            {

                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.usuario.SingleOrDefault(n => n.ID_Usuario == IDUsuario);
                    if (a != null)
                    {
                        a.Privilegios = scPrioridad.Text;
                        a.Usuario1 = txtUsuario.Text.Trim();
                        a.Password = txtContra1.Text;
                        a.Nombre = txtNombre.Text;
                        a.Telefono = txtTelefono.Text;
                        a.ID_AreaPersonal = IDAreaPersonal;
                        a.Puesto = txtPuesto.Text.Trim();
                        a.mail_Correo = txtCorreo.Text;
                        a.mail_Password = (ckCorreo.Checked == true) ? txtContraseñaServidor.Text : null;
                        a.CorreoConfirmado = false;
                        if (ckCorreo.Checked == true) { a.ID_ProveedoresCorreo = IDServidor; }
                        else { a.ID_ProveedoresCorreo = null; }
                        context.SaveChanges();
                        MessageBox.Show("Actualizacion Correcta");
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

        private void scServidor_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDServidor = Int32.Parse((((KeyValuePair<string, string>)scServidor.SelectedItem).Key));

        }

        private void scArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDAreaPersonal = Int32.Parse((((KeyValuePair<string, string>)scArea.SelectedItem).Key));
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseñaServidor.PasswordChar = (checkBox2.Checked == false) ? '*' : Convert.ToChar(0);

        }

        private void btnConfirmarCorreo_Click(object sender, EventArgs e)
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void ActualizarUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Refresh_();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContra1.Text) || string.IsNullOrEmpty(txtContra2.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtTelefono.Text) || string.IsNullOrEmpty(txtPuesto.Text) || string.IsNullOrEmpty(txtCorreo.Text))
            {
                MessageBox.Show("Faltan campos por llenar"); return false;
            }
            else
            {
                if (txtContra1.Text != txtContra2.Text)
                {
                    MessageBox.Show("Las contraseñas no coinciden"); return false;
                }
                else
                {
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var us = txtUsuario.Text.Trim();
                        var a = context.usuario.Where(n => n.Usuario1 == us).Count();
                        if (a > 0) { MessageBox.Show("El usuario ya existe seleccione otro nombre"); return false; }
                        else { return true; }
                    }
                }

            }

        }

    }
}
