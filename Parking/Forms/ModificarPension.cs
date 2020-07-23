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
    public partial class ModificarPension : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string Conexion;
        private string Usuario;
        private DateTime dtTermino;
        private int ID_Plan,ID_Pension,ID_Vehiculo,ID_Cliente;
        private decimal PrecioNeto;
        public delegate void RF();
        public event RF Refresh_;

        #endregion
        public ModificarPension(string Conexion, int ID_Pension,string usuario)
        {
            this.Usuario = usuario;
            this.ID_Pension = ID_Pension;
            this.Conexion = Conexion;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            fechaInicio.Value = DateTime.Now;
            AgregarListaPensiones();
            CargarDatos();
        }

        private void CargarDatos()
        {
           
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.banco_pension.Where(n => n.ID == ID_Pension).FirstOrDefault();
                var b = context.cliente.Where(n=>n.Id_cliente==a.Id_cliente).FirstOrDefault();
                var c = context.vehiculo.Where(n => n.Id_Vehiculo== b.Id_Vehiculo).FirstOrDefault();
                var d = context.pensiones.Where(n => n.ID_Tipo_Pension == a.ID_Tipo_Pension).FirstOrDefault();

                ID_Vehiculo = c.Id_Vehiculo;
                ID_Cliente = b.Id_cliente;

                txtNombre.Text = b.Nombre;
                txtApellidoP.Text = b.Apellido_paterno;
                txtApellidoM.Text = b.Apellido_materno;
                txtDireccion.Text = b.Direccion;
                txtCorreo.Text = b.Correo;
                txtTel1.Text = b.Tel1;
                txtTel2.Text = b.Tel2;
                if (!string.IsNullOrEmpty(b.RFC))
                {
                    txtRFC.Text = b.RFC;
                    txtRazonSocial.Text = b.Razon_Social;
                    ckRFC.Checked = true;
                }
                txtMarca.Text = c.Marca;
                txtColor.Text = c.Color;
                txtDesc.Text = c.Descripcion;
                txtPlaca.Text = c.Placa;

                fechaInicio.Value = (a.Inicio_Pension.GetValueOrDefault()>fechaInicio.MinDate)? a.Inicio_Pension.GetValueOrDefault():DateTime.Today; 
                txtCantidadAnticipados.Text = a.No_PagosAnticipados.ToString();                
                switch (d.Pensn_Tipo)
                {
                    case "Semanal":
                        scPeriodo.SelectedIndex = 1;
                        break;
                    case "Quincenal":
                        scPeriodo.SelectedIndex = 2;
                        break;
                    case "Mensual":
                        scPeriodo.SelectedIndex = 3;
                        break;
                    case "Anual":
                        scPeriodo.SelectedIndex = 4;
                        break;                  
                }
                ckPagado.Checked = a.StatusPago.GetValueOrDefault();              

              




            }



        }

        private void ckRFC_CheckedChanged(object sender, EventArgs e)
        {
            if (ckRFC.Checked == true)
            {
                label18.Visible = true;
                label19.Visible = true;
         
                txtRFC.Visible = true;
                txtRazonSocial.Visible = true;
      

            }
            else
            {

                label18.Visible = false;
                label19.Visible = false;
            
                txtRFC.Visible = false;
                txtRazonSocial.Visible = false;
             
            }
        }
        private void txtNombre_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "Introduzca un Nombre");

            }
        }
        private void txtApellidoP_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtApellidoP.Text))
            {
                errorProvider.SetError(txtApellidoP, "Introduzca el Apellido Paterno");

            }
        }

        private void txtPlaca_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlaca.Text))
            {
                errorProvider.SetError(txtPlaca, "Introduzca una Placa");

            }

        }

        private void txtMarca_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMarca.Text))
            {
                errorProvider.SetError(txtMarca, "Introduzca una Marca");

            }
        }

        private void txtColor_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtColor.Text))
            {
                errorProvider.SetError(txtColor, "Introduzca un Color");

            }
        }

        private void scPeriodo_Validating(object sender, CancelEventArgs e)
        {
            if (scPeriodo.SelectedIndex == 0)
            {


                errorProvider.SetError(scPeriodo, "Seleccione una opcion");

            }
            else
            {
                errorProvider.SetError(scPeriodo, null);
            }
        }

        private bool ValidarRFC()
        {
            if (ckRFC.Checked == true)
            {
                bool rfc = (string.IsNullOrEmpty(txtRFC.Text) || string.IsNullOrEmpty(txtRazonSocial.Text)) ? false : true;
                return rfc;
            }
            else
            {
                return true;
            }

        }


        private bool Validador()
        {


            if (string.IsNullOrEmpty(txtApellidoP.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtPlaca.Text) || string.IsNullOrEmpty(txtMarca.Text) || string.IsNullOrEmpty(txtColor.Text) || scPeriodo.SelectedIndex == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        #region Botones
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
            if (Validador() == false || ValidarRFC() == false)
            {
                MessageBox.Show("Datos Incompletos");
            }
            else
            {               
                    InsertDatos();
                    MessageBox.Show("Insercion Correcta");
                    Refresh_();
                    this.Dispose();             

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

        private void InsertDatos()
        {
            InsertarVehiculo(ID_Vehiculo);
            InsertarCliente(ID_Cliente);           
            InsertarPension(ID_Pension);

        }
        private void InsertarPension(int idpension)
        {

            decimal? ultimoPago = null;
            string costoFormato = lblCosto.Text.Replace('.', ',');
            int ultimoCorte;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var b = context.cortes.ToList();
                var ultimo = b.LastOrDefault();
                ultimoCorte = ultimo.Id_Corte;

            }
            if (ckPagado.Checked == true)
            {

                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var c = context.banco_pension.Where(n => n.Id_cliente == this.ID_Cliente).ToList();
                    if (c.Count > 0)
                    {
                        var ultimop = c.LastOrDefault();
                        ultimoPago = ultimop.Pagos;                

                    }

                    var a = context.banco_pension.SingleOrDefault(n => n.ID == idpension);
                    if (a != null)
                    {
                        a.FechaInscripcion = DateTime.Now;
                        a.FechaUltimoPago = DateTime.Now;
                        a.CobradoPor = Usuario;
                        a.Inicio_Pension = fechaInicio.Value;
                        a.Fin_Pension = dtTermino;
                        a.ID_Tipo_Pension = ID_Plan;
                        a.StatusPago = true;
                        a.No_PagosAnticipados = (Convert.ToInt32(txtCantidadAnticipados.Text) - 1);
                        a.Costo = Convert.ToDecimal(costoFormato);
                        a.Pagos = Convert.ToDecimal(costoFormato)+ultimoPago;
                        a.Id_cliente = idpension;
                        a.Pension_Corte = false;
                        a.Pension_Activa = true;
                        context.SaveChanges();
                    }
                 
                }
            }
            else
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var c = context.banco_pension.Where(n => n.Id_cliente == this.ID_Cliente).ToList();
                    if (c.Count > 0)
                    {
                        var ultimop = c.LastOrDefault();
                        ultimoPago = ultimop.Pagos;

                    }


                    var a = context.banco_pension.SingleOrDefault(n => n.ID == idpension);
                    if (a != null)
                    {
                        a.FechaInscripcion = DateTime.Now;
                        a.FechaUltimoPago = null;
                        a.CobradoPor = Usuario;
                        a.Inicio_Pension = null;
                        a.Fin_Pension = dtTermino;
                        a.ID_Tipo_Pension = ID_Plan;
                        a.StatusPago = false;
                        a.No_PagosAnticipados = null;
                        a.Costo = Convert.ToDecimal(PrecioNeto);
                        a.Pagos = ultimoPago;
                        a.Id_cliente = idpension;
                        a.Pension_Corte = true;
                        a.Pension_Activa = false;
                        a.Id_corte = ultimoCorte;
                        context.SaveChanges();
                    }
                   
                
                }
            }




        }
        private void InsertarVehiculo(int id)
        {
           
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.vehiculo.SingleOrDefault(n => n.Id_Vehiculo == id);
                if (a != null)
                {
                    a.Marca = txtMarca.Text.Trim();
                    a.Descripcion = txtDesc.Text.Trim();
                    a.Color = txtColor.Text.Trim();
                    a.Placa = txtPlaca.Text.Trim();
                    context.SaveChanges();
                }
            }
        }

        private void InsertarCliente(int id)
        {
          
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.cliente.SingleOrDefault(n => n.Id_cliente == id);
                if (a != null)
                {
                    a.Nombre = txtNombre.Text.Trim();
                    a.Apellido_paterno = txtApellidoP.Text.Trim();
                    a.Apellido_materno = txtApellidoM.Text.Trim();
                    a.Direccion = txtDireccion.Text.Trim();
                    a.Correo = txtCorreo.Text.Trim();
                    a.Tel1 = txtTel1.Text.Trim();
                    a.Tel2 = txtTel2.Text.Trim();
                    a.Direccion = txtDireccion.Text.Trim();
                    a.RFC = txtRFC.Text.Trim();
                    a.Razon_Social = txtRazonSocial.Text.Trim();
                    context.SaveChanges();
                }
            }
        }




        #endregion

        #region MetodosApoyo
        private void AgregarListaPensiones()
        {
            int a = scPeriodo.SelectedIndex;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.pensiones.ToList();
                diccionario.Add("0", "Seleccione");

                foreach (var valores in item)
                {
                    var nombre = valores.Pensn_Tipo;
                    var id = valores.ID_Tipo_Pension;
                    diccionario.Add(id.ToString(), nombre);


                }
                scPeriodo.DataSource = new BindingSource(diccionario, null);
                scPeriodo.DisplayMember = "Value";
                scPeriodo.ValueMember = "Key";
            }
            scPeriodo.SelectedIndex = 0;

        }

        private void scPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cantidad = (!string.IsNullOrEmpty(txtCantidadAnticipados.Text)) ? Convert.ToInt32(txtCantidadAnticipados.Text.Trim()) :1; 
            var a = ((KeyValuePair<string, string>)scPeriodo.SelectedItem).Key;
            var b = ((KeyValuePair<string, string>)scPeriodo.SelectedItem).Value;
            ID_Plan = Int32.Parse(a);

            if (scPeriodo.SelectedIndex == 0)
            {
                lblCosto.Text = "Seleccione un Periodo";
                lblCosto.ForeColor = Color.Red;
                ckPagado.Checked = false;

            }
            else
            {
                errorProvider.SetError(scPeriodo, null);
                if (ckPagado.Checked != false)
                {

                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var item = context.pensiones.ToList();
                        foreach (var valores in item.Where(n => n.ID_Tipo_Pension == ID_Plan))
                        {
                            PrecioNeto = valores.Pens_Costo_Regular.GetValueOrDefault();
                            lblCosto.ForeColor = Color.Black;
                        };
                    }
                }
                else
                {
                    PrecioNeto = 0;
                    lblCosto.Text = PrecioNeto.ToString();
                }

            }

            switch (b)
            {
                case "Seleccione":
                    fechaTermino.Text = " ";

                    break;

                case "Semanal":
                    var resultado1 = fechaInicio.Value.AddDays(cantidad * 7);
                    fechaTermino.Text = resultado1.ToShortDateString();
                    lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                    dtTermino = resultado1;

                    break;
                case "Quincenal":
                    var resultado2 = fechaInicio.Value.AddDays(cantidad * 15);
                    fechaTermino.Text = resultado2.ToShortDateString();
                    lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                    dtTermino = resultado2;
                    break;
                case "Mensual":
                    var resultado3 = fechaInicio.Value.AddDays(cantidad * 30);
                    fechaTermino.Text = resultado3.ToShortDateString();
                    lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                    dtTermino = resultado3;
                    break;
                case "Anual":
                    var resultado4 = fechaInicio.Value.AddDays(cantidad * 365);
                    fechaTermino.Text = resultado4.ToShortDateString();
                    lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                    dtTermino = resultado4;
                    break;

            }

        }

        private void fechaInicio_ValueChanged(object sender, EventArgs e)
        {
            if (scPeriodo.SelectedIndex > 0)
            {
                int cantidad = Convert.ToInt32(txtCantidadAnticipados.Text.Trim());
                var b = ((KeyValuePair<string, string>)scPeriodo.SelectedItem).Value;
                switch (b)
                {

                    case "Seleccione":
                        fechaTermino.Text = " ";
                        lblCosto.Text = " ";
                        break;

                    case "Semanal":
                        var resultado1 = fechaInicio.Value.AddDays(cantidad * 7);
                        fechaTermino.Text = resultado1.ToShortDateString();
                        dtTermino = resultado1;
                        break;
                    case "Quincenal":
                        var resultado2 = fechaInicio.Value.AddDays(cantidad * 15);
                        fechaTermino.Text = resultado2.ToShortDateString();
                        dtTermino = resultado2;
                        break;
                    case "Mensual":
                        var resultado3 = fechaInicio.Value.AddDays(cantidad * 30);
                        fechaTermino.Text = resultado3.ToShortDateString();
                        dtTermino = resultado3;
                        break;
                    case "Anual":
                        var resultado4 = fechaInicio.Value.AddDays(cantidad * 365);
                        fechaTermino.Text = resultado4.ToShortDateString();
                        dtTermino = resultado4;
                        break;

                }
            }
        }
        private void SoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                if (e.Handled != true)
                {
                    if (scPeriodo.SelectedIndex != 0)
                    {
                        int cantidad = Convert.ToInt32(txtCantidadAnticipados.Text.Trim());
                        var costo = Convert.ToDecimal(lblCosto.Text);
                        var b = ((KeyValuePair<string, string>)scPeriodo.SelectedItem).Value;
                        switch (b)
                        {
                            case "Seleccione":
                                fechaTermino.Text = " ";
                                lblCosto.Text = " ";
                                break;

                            case "Semanal":
                                var resultado1 = fechaInicio.Value.AddDays(cantidad * 7);
                                fechaTermino.Text = resultado1.ToShortDateString();
                                lblCosto.Text = (costo * cantidad).ToString("F", CultureInfo.InvariantCulture);
                                dtTermino = resultado1;

                                break;
                            case "Quincenal":
                                var resultado2 = fechaInicio.Value.AddDays(cantidad * 15);
                                fechaTermino.Text = resultado2.ToShortDateString();
                                lblCosto.Text = (costo * cantidad).ToString("F", CultureInfo.InvariantCulture);
                                dtTermino = resultado2;
                                break;
                            case "Mensual":
                                var resultado3 = fechaInicio.Value.AddDays(cantidad * 30);
                                fechaTermino.Text = resultado3.ToShortDateString();
                                lblCosto.Text = (costo * cantidad).ToString("F", CultureInfo.InvariantCulture);
                                dtTermino = resultado3;
                                break;
                            case "Anual":
                                var resultado4 = fechaInicio.Value.AddDays(cantidad * 365);
                                fechaTermino.Text = resultado4.ToShortDateString();
                                lblCosto.Text = (costo * cantidad).ToString("F", CultureInfo.InvariantCulture);
                                dtTermino = resultado4;
                                break;

                        }



                    }
                }

            }
        }

        private void btnSiguiente1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = (tabControl1.SelectedIndex + 1 < tabControl1.TabCount) ?
                             tabControl1.SelectedIndex + 1 : tabControl1.SelectedIndex;
        }

        private void btnSiguiente2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = (tabControl1.SelectedIndex + 1 < tabControl1.TabCount) ?
                            tabControl1.SelectedIndex + 1 : tabControl1.SelectedIndex;
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion



        private void txtCantidadAnticipados_TextChanged(object sender, EventArgs e)
        {
            if (scPeriodo.SelectedIndex != 0)
            {
                if (string.IsNullOrEmpty(txtCantidadAnticipados.Text) || txtCantidadAnticipados.Text == "0")
                {
                    txtCantidadAnticipados.Text = "1";
                }
                else
                {


                    int cantidad = Convert.ToInt32(txtCantidadAnticipados.Text.Trim());


                    var b = ((KeyValuePair<string, string>)scPeriodo.SelectedItem).Value;
                    switch (b)
                    {
                        case "Seleccione":
                            fechaTermino.Text = " ";
                            lblCosto.Text = " ";
                            break;

                        case "Semanal":
                            var resultado1 = fechaInicio.Value.AddDays(cantidad * 7);
                            fechaTermino.Text = resultado1.ToShortDateString();
                            lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                            dtTermino = resultado1;

                            break;
                        case "Quincenal":
                            var resultado2 = fechaInicio.Value.AddDays(cantidad * 15);
                            fechaTermino.Text = resultado2.ToShortDateString();
                            lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                            dtTermino = resultado2;
                            break;
                        case "Mensual":
                            var resultado3 = fechaInicio.Value.AddDays(cantidad * 30);
                            fechaTermino.Text = resultado3.ToShortDateString();
                            lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                            dtTermino = resultado3;
                            break;
                        case "Anual":
                            var resultado4 = fechaInicio.Value.AddDays(cantidad * 365);
                            fechaTermino.Text = resultado4.ToShortDateString();
                            lblCosto.Text = ((PrecioNeto * cantidad) > 0) ? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture) : null;
                            dtTermino = resultado4;
                            break;

                    }

                }



            }

        }

        private void txtCantidadAnticipados_Click(object sender, EventArgs e)
        {
            txtCantidadAnticipados.SelectAll();
        }

        private void ckPagado_CheckedChanged(object sender, EventArgs e)
        {
            if (scPeriodo.SelectedIndex == 0)
            {
                lblCosto.Text = "Seleccione un Periodo";
                lblCosto.ForeColor = Color.Red;
                ckPagado.Checked = false;

            }
            if (ckPagado.Checked == true)
            {
                label14.Visible = true; txtCantidadAnticipados.Visible = true; txtCantidadAnticipados.Text = "1";
                if (scPeriodo.SelectedIndex != 0)
                {
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var item = context.pensiones.ToList();
                        foreach (var valores in item.Where(n => n.ID_Tipo_Pension == ID_Plan))
                        {
                            PrecioNeto = valores.Pens_Costo_Regular.GetValueOrDefault();
                            lblCosto.ForeColor = Color.Black;
                            lblCosto.Text = ((PrecioNeto * 1) > 0) ? (PrecioNeto * 1).ToString("F", CultureInfo.InvariantCulture) : "Seleccione un Periodo";

                        };

                    }
                }
            }
            else
            {
                label14.Visible = false; txtCantidadAnticipados.Visible = false; txtCantidadAnticipados.Text = "1"; lblCosto.Text = null;

            }
        }
    }
}
