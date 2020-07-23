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
    public partial class AgregarPension : Form
    {
        #region Variables  
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string Conexion;
        private string Usuario;
        private DateTime dtTermino;
        private int ID_Plan,ID_Cliente,ID_Vehiculo;
        private decimal PrecioNeto;
        #endregion
        public AgregarPension(string Conexion, string Usuario, int IDCliente)
        {
            this.ID_Cliente = IDCliente;
            this.Usuario = Usuario;
            this.Conexion = Conexion;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;
            CargarVehiculo(IDCliente);
            AgregarListaPensiones();
            fechaInicio.Value = DateTime.Now;

        }


        #region Validadores
       
           
     

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

     


        private bool Validador()
        {


            if (string.IsNullOrEmpty(txtPlaca.Text) || string.IsNullOrEmpty(txtMarca.Text) || string.IsNullOrEmpty(txtColor.Text) || scPeriodo.SelectedIndex == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
        #region Botones
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Validador() == false )
            {
                MessageBox.Show("Datos Incompletos");
            }
            else
            {
                try
                {
                    InsertDatos();
                    MessageBox.Show("Insercion Correcta");
                    this.Dispose();                
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

        private void InsertDatos()
        {
            InsertarVehiculo(ID_Vehiculo);            
            InsertarPension(this.ID_Cliente);

        }
        private void InsertarPension(int IdCliente)
        {

            string costoFormato = lblCosto.Text.Replace('.', ',');
            int ultimoCorte;
            decimal? ultimoPago=0;
            DateTime? fechaUltimoPAgo=null;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var b = context.cortes.ToList();
                var ultimo = b.LastOrDefault();
                ultimoCorte = ultimo.Id_Corte;

                var c = context.banco_pension.Where(n=>n.Id_cliente==IdCliente).ToList();
                if (c.Count>0)
                {
                    var ultimop = c.LastOrDefault();
                    ultimoPago = ultimop.Pagos;
                    fechaUltimoPAgo = ultimop.FechaUltimoPago;

                }
                

            }

            if (ckPagado.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {

                    string identificador = "PE";
                    var ultimoID = context.banco_pension.ToList().LastOrDefault();
                    var folio = (ultimoID == null) ? "1" : (ultimoID.ID + 1).ToString();
                    var tam = folio.Length;
                    switch (tam)
                    {
                        case 1:
                            folio = "00000" + folio;
                            break;
                        case 2:
                            folio = "0000" + folio;
                            break;
                        case 3:
                            folio = "000" + folio;
                            break;
                        case 4:
                            folio = "00" + folio;
                            break;
                        case 5:
                            folio = "0" + folio;
                            break;

                    }
                    identificador = identificador + folio;



                    var a = new banco_pension()
                    {
                        FechaInscripcion = DateTime.Now,
                        Folio = identificador,
                        FechaUltimoPago = DateTime.Now,
                        CobradoPor = Usuario,
                        Inicio_Pension = fechaInicio.Value,
                        Fin_Pension = dtTermino,
                        ID_Tipo_Pension = ID_Plan,
                        StatusPago = true,
                        No_PagosAnticipados = (Convert.ToInt32(txtCantidadAnticipados.Text) - 1),
                        Costo = Convert.ToDecimal(costoFormato),
                        Pagos = ultimoPago+Convert.ToDecimal(costoFormato),
                        Id_cliente = IdCliente,                 
                        Pension_Corte = false,
                        Pension_Activa = true,
                        Id_corte = ultimoCorte,
                    };
                    context.banco_pension.Add(a);
                    context.SaveChanges();
                }
            }
            else
            {

                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    string identificador = "PE";
                    var ultimoID = context.banco_pension.ToList().LastOrDefault();
                    var folio = (ultimoID == null) ? "1" : (ultimoID.ID + 1).ToString();
                    var tam = folio.Length;
                    switch (tam)
                    {
                        case 1:
                            folio = "00000" + folio;
                            break;
                        case 2:
                            folio = "0000" + folio;
                            break;
                        case 3:
                            folio = "000" + folio;
                            break;
                        case 4:
                            folio = "00" + folio;
                            break;
                        case 5:
                            folio = "0" + folio;
                            break;

                    }
                    identificador = identificador + folio;

                    var a = new banco_pension()
                    {
                        FechaInscripcion = DateTime.Now,
                        Folio = identificador,
                        FechaUltimoPago = fechaUltimoPAgo,
                        CobradoPor = Usuario,
                        Inicio_Pension = DateTime.Now,
                        Fin_Pension = dtTermino,
                        ID_Tipo_Pension = ID_Plan,
                        StatusPago = false,
                        No_PagosAnticipados = null,
                        Costo = (from tp in context.pensiones where tp.ID_Tipo_Pension == ID_Plan select tp.Pens_Costo_Regular).FirstOrDefault(),
                        Pagos = ultimoPago,
                        Id_cliente = IdCliente,
                        Pension_Corte = true,
                        Pension_Activa = false,
                        Id_corte = null,
                    };
                    context.banco_pension.Add(a);
                    context.SaveChanges();
                }
            }




        }
        private void InsertarVehiculo(int IDvehiculo)
        {
          
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.vehiculo.SingleOrDefault(n => n.Id_Vehiculo == IDvehiculo);
                if (a != null)
                {
                    a.Marca= txtMarca.Text.Trim();
                    a.Descripcion=txtDesc.Text.Trim();
                    a.Color=txtColor.Text.Trim();
                    a.Placa=txtPlaca.Text.Trim();
                    context.SaveChanges();
                }                   
            }

         
        }
        private void CargarVehiculo(int id_cliente)
        {
            int IDVehiculo = 0;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                var tVehiculo =(from cl in context.cliente
                                 join vh in context.vehiculo on cl.Id_Vehiculo equals vh.Id_Vehiculo
                                 where cl.Id_cliente==id_cliente
                                 select new Modelos.ModeloCargarVehiculo 
                                 {
                                     ID=vh.Id_Vehiculo,
                                     Placa=vh.Placa,
                                     Marca=vh.Marca,
                                     Color=vh.Color,
                                     Descripcion=vh.Descripcion
                                 }).ToList();
                foreach (var item in tVehiculo) 
                {
                    IDVehiculo = item.ID;
                    txtMarca.Text = item.Marca;
                    txtDesc.Text = item.Descripcion;
                    txtColor.Text = item.Color;
                    txtPlaca.Text = item.Placa;

                }
                this.ID_Vehiculo = IDVehiculo;
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
            int cantidad = Convert.ToInt32(txtCantidadAnticipados.Text.Trim());
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
