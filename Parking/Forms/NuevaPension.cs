using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class NuevaPension : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;

        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string Conexion;
        private string Usuario;
        private DateTime dtTermino;
        private int ID_Plan, UltimoID;
        private decimal PrecioNeto;
        private bool? ancho = true;
        public List<Modelos.ModeloTicketActivadoYPagado> lista = new List<Modelos.ModeloTicketActivadoYPagado>();

        #endregion
        #region Constructor
        public NuevaPension(string conexion, string usuario)
        {
            Usuario = usuario;
            Conexion = conexion;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            fechaInicio.Value = DateTime.Now;
            AgregarListaPensiones();
            
        }

        #endregion

        #region Validadores
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
            if (ckRFC.Checked==true)
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

        #endregion

        #region Botones
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
            if (Validador()==false||ValidarRFC()==false)
            {
                MessageBox.Show("Datos Incompletos");
            }
            else
                {
                    InsertDatos();
                    CargarTicket();
                    Imprimir();
                    MessageBox.Show("Insercion Correcta");

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

        private void CargarTicket()
        {
            lista.Clear();
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                ancho = (from cf in context.config_tickets
                         where cf.ID_ticket == 1
                         select cf.Tamaño_papel).FirstOrDefault();
                Console.WriteLine(ancho);
               
                var a = (from bp in context.banco_pension
                         join cl in context.cliente on bp.Id_cliente equals cl.Id_cliente
                         join vh in context.vehiculo on cl.Id_Vehiculo equals vh.Id_Vehiculo
                         where bp.ID == UltimoID
                         select new Modelos.ModeloTicketActivadoYPagado
                         {
                             ID = bp.ID.ToString(),
                             Color = vh.Color,
                             Direccion = cl.Direccion,
                             Importe = bp.Costo.ToString(),
                             Marca = vh.Marca,
                             Placa = vh.Placa,
                             Nombre = cl.Nombre + " " + cl.Apellido_paterno + " " + cl.Apellido_materno,
                             Telefono = cl.Tel1,
                             ultimaFecha = bp.Fin_Pension

                         }).ToList();               
                lista = a;
            }      
        }

        private void Imprimir()
        {

            var pd = new PrintDocument();
            var ps = new PrinterSettings();
            pd.PrinterSettings = ps;
            pd.PrintPage += ImprimirTicket;
            pd.Print();
        }

        private void ImprimirTicket(object sender, PrintPageEventArgs e)
        {
            if (ckPagado.Checked==true)
            {

                int espaciado = 10;
                //315 = 80mm
                int tamLetra = (ancho == true) ? 13 : 9;
                int tamPapel = (ancho == true) ? 305 : 218;
                Font fuenteNormal = new Font("Arial", tamLetra, FontStyle.Regular, GraphicsUnit.Point);
                Font fuenteNegrita = new Font("Arial", tamLetra, FontStyle.Bold, GraphicsUnit.Point);
                StringFormat LetrasCentradas = new StringFormat();
                LetrasCentradas.Alignment = StringAlignment.Center;
                LetrasCentradas.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString("***ACTIVACION Y PAGO***", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("DE PENSION", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US")), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Cobrado por: " + Usuario, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Tipo de pension: " + scPeriodo.Text, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

                foreach (var item in lista)
                {
                    e.Graphics.DrawString("Proximo Pago: " + item.ProximoPago, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Importe Regular: $" + item.Importe, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("__________________________", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("ID: " + item.ID, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Nombre: " + item.Nombre, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 30, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Direccion: " + item.Direccion, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Telefono: " + item.Telefono, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Placa: " + item.Placa, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Marca: " + item.Marca, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Color: " + item.Color, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Importe Proporcional: $" + item.Importe, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

                }
                e.Graphics.DrawString("__________________________", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

            }
            if (ckPagado.Checked == false)
            {
                int espaciado = 10;
                //315 = 80mm
                int tamLetra = (ancho == true) ? 13 : 9;
                int tamPapel = (ancho == true) ? 305 : 218;
                Font fuenteNormal = new Font("Arial", tamLetra, FontStyle.Regular, GraphicsUnit.Point);
                Font fuenteNegrita = new Font("Arial", tamLetra, FontStyle.Bold, GraphicsUnit.Point);
                StringFormat LetrasCentradas = new StringFormat();
                LetrasCentradas.Alignment = StringAlignment.Center;
                LetrasCentradas.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString("***ALTA DE PENSION***", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US")), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Usuario: " + Usuario, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("__________________________", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

                foreach (var item in lista)
                {
                    e.Graphics.DrawString("ID: " + item.ID, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Nombre: " + item.Nombre, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 30, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Direccion: " + item.Direccion, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Telefono: " + item.Telefono, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Placa: " + item.Placa, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Marca: " + item.Marca, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Color: " + item.Color, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Tipo de Pension: " + scPeriodo.Text, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Importe Regular: $" + item.Importe, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

                }
                e.Graphics.DrawString("__________________________", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);


            }


        }

        private void InsertDatos()
        {  
            int IDVehiculo= Insertarvehiculo();
           int IDCliente= InsertarCliente(IDVehiculo);
         
           InsertarPension(IDCliente);

        }
        private void InsertarPension(int IdCliente)
        {

            string costoFormato = lblCosto.Text.Replace('.',',');
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
                        Folio=identificador,
                        FechaUltimoPago = DateTime.Now,
                        CobradoPor = Usuario,
                        Inicio_Pension = fechaInicio.Value,
                        Fin_Pension = dtTermino,
                        ID_Tipo_Pension = ID_Plan,
                        StatusPago = true,
                        No_PagosAnticipados = (Convert.ToInt32(txtCantidadAnticipados.Text) - 1),
                        Costo = Convert.ToDecimal(costoFormato),
                        Pagos = Convert.ToDecimal(costoFormato),
                        Id_cliente = IdCliente,                   
                        Pension_Corte = false,
                        Pension_Activa = true,
                        Id_corte = ultimoCorte,
                    };
                    context.banco_pension.Add(a);
                    context.SaveChanges();
                    UltimoID = a.ID;
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
                        Folio=identificador,
                        FechaUltimoPago = null,
                        CobradoPor = Usuario,
                        Inicio_Pension = DateTime.Now,
                        Fin_Pension = dtTermino,
                        ID_Tipo_Pension = ID_Plan,
                        StatusPago = false,
                        No_PagosAnticipados = null,
                        Costo = (from tp in context.pensiones where tp.ID_Tipo_Pension==ID_Plan select tp.Pens_Costo_Regular).FirstOrDefault(),
                        Pagos = null,
                        Id_cliente = IdCliente,
                        Pension_Corte = true,
                        Pension_Activa = false,
                        Id_corte = null,
                    };
                    context.banco_pension.Add(a);
                    context.SaveChanges();
                    UltimoID = a.ID; 
                }
             

            }
            



        }
        private int Insertarvehiculo()
        {
            int IDVehiculo = 0;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = new vehiculo()
                {
                   Marca=txtMarca.Text.Trim(),
                   Placa=txtPlaca.Text.Trim(),
                   Color=txtColor.Text.Trim(),
                   Descripcion=txtDesc.Text.Trim()
                };
                context.vehiculo.Add(a);
                context.SaveChanges();
                IDVehiculo = a.Id_Vehiculo;
            }

            return IDVehiculo;
        }

        private int InsertarCliente(int idVehiculo)
        {
            int IDCliente = 0;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = new cliente()
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido_paterno = txtApellidoP.Text.Trim(),
                    Apellido_materno = txtApellidoM.Text.Trim(),
                    Tel1 = txtTel1.Text.Trim(),
                    Tel2=txtTel2.Text.Trim(),
                    Direccion=txtDireccion.Text.Trim(),
                    Correo=txtCorreo.Text.Trim(),
                    Razon_Social=txtRazonSocial.Text.Trim(),        
                    RFC=txtRFC.Text.Trim(),
                    Id_Vehiculo = idVehiculo,
                };
                context.cliente.Add(a);
                context.SaveChanges();
                IDCliente = a.Id_cliente;
            }

                return IDCliente;
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
                if (ckPagado.Checked!=false)
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
                    var resultado1 = fechaInicio.Value.AddDays(cantidad*7);
                    fechaTermino.Text = resultado1.ToShortDateString();
                       lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                    dtTermino = resultado1;

                    break;
                case "Quincenal":
                    var resultado2 = fechaInicio.Value.AddDays(cantidad*15);
                    fechaTermino.Text = resultado2.ToShortDateString();
                       lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                    dtTermino = resultado2;
                    break;
                case "Mensual":
                    var resultado3 = fechaInicio.Value.AddDays(cantidad*30);
                    fechaTermino.Text = resultado3.ToShortDateString();
                       lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                    dtTermino = resultado3;
                    break;
                case "Anual":
                    var resultado4 = fechaInicio.Value.AddDays(cantidad*365);
                    fechaTermino.Text = resultado4.ToShortDateString();
                       lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
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
                        var resultado1 = fechaInicio.Value.AddDays(cantidad*7);
                        fechaTermino.Text = resultado1.ToShortDateString();
                        dtTermino = resultado1;
                        break;
                    case "Quincenal":
                        var resultado2 = fechaInicio.Value.AddDays(cantidad*15);
                        fechaTermino.Text = resultado2.ToShortDateString();
                        dtTermino = resultado2;
                        break;
                    case "Mensual":
                        var resultado3 = fechaInicio.Value.AddDays(cantidad*30);
                        fechaTermino.Text = resultado3.ToShortDateString();
                        dtTermino = resultado3;
                        break;
                    case "Anual":
                        var resultado4 = fechaInicio.Value.AddDays(cantidad*365);
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
                if (string.IsNullOrEmpty(txtCantidadAnticipados.Text)||txtCantidadAnticipados.Text=="0")
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
                            lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                            dtTermino = resultado1;

                            break;
                        case "Quincenal":
                            var resultado2 = fechaInicio.Value.AddDays(cantidad * 15);
                            fechaTermino.Text = resultado2.ToShortDateString();
                               lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                            dtTermino = resultado2;
                            break;
                        case "Mensual":
                            var resultado3 = fechaInicio.Value.AddDays(cantidad * 30);
                            fechaTermino.Text = resultado3.ToShortDateString();
                               lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
                            dtTermino = resultado3;
                            break;
                        case "Anual":
                            var resultado4 = fechaInicio.Value.AddDays(cantidad * 365);
                            fechaTermino.Text = resultado4.ToShortDateString();
                               lblCosto.Text = ((PrecioNeto * cantidad)>0)? (PrecioNeto * cantidad).ToString("F", CultureInfo.InvariantCulture):null;
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
            if (ckPagado.Checked==true)
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
                label14.Visible = false; txtCantidadAnticipados.Visible = false; txtCantidadAnticipados.Text = "1";lblCosto.Text = null;
               
            }
        }
    }
}
