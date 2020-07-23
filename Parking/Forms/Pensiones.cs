using BarcodeLib;
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
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Parking.Forms
{
    public partial class Pensiones : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        string Conexion;
        string Usuario;
        int ID_Pension;
        public delegate void RF();
        public event RF Refresh_;
        int idUltimoCobro, idUltimoIngreso;
        #endregion
        public Pensiones(string conexion, string usuario)
        {
            Conexion = conexion;
            Usuario = usuario;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            radioTodas.Checked = true;
            CargarBitacora();
            CargarSalidos();

        }

        private void CargarDatosDerecha(int ID)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from bp in context.banco_pension
                                     join cl in context.cliente on bp.Id_cliente equals cl.Id_cliente
                                     join p in context.pensiones on bp.ID_Tipo_Pension equals p.ID_Tipo_Pension
                                     where bp.ID == ID
                                     select new Modelos.ModeloCargaDatosPensionDerecha
                                     {
                                         IDCliente = ID,
                                         Costo = bp.Costo.ToString(),
                                         Descripcion = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Descripcion).FirstOrDefault(),
                                         Direccion = cl.Direccion,
                                         FechaInscripcion = bp.FechaInscripcion.ToString(),
                                         Nombre = cl.Nombre + " " + cl.Apellido_paterno,
                                         Telefono = cl.Tel1,
                                         UltimoDeposito = bp.FechaUltimoPago.ToString()

                                     }).ToList();
                foreach (var item in listabusqueda)
                {

                    lblID.Text = item.IDCliente.ToString();
                    lblDireccion.Text = item.Direccion;
                    lblDescripcion.Text = item.Descripcion;
                    lblCosto.Text = item.Costo;
                    lblNombre.Text = item.Nombre;
                    lblTelefono.Text = item.Telefono;
                    lblInscripcion.Text = item.FechaInscripcion;
                    lblUltimoPago.Text = item.UltimoDeposito;

                }
            }
        }
        private string GetIDBitacora()
        {
            try
            {
                var a = dtPensiones.Rows[dtPensiones.CurrentRow.Index].Cells[0].Value.ToString();
                 return a;
            }
            catch
            {
                return null;

            }
        }
        private int? GetID()
        {
            try
            {
                var a = dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[0].Value.ToString();
                ID_Pension = Int32.Parse(a);
                return ID_Pension;
            }
            catch
            {
                return null;

            }
        }
        private void LlenarTabla(string Consulta)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from bp in context.banco_pension
                                     join cl in context.cliente on bp.Id_cliente equals cl.Id_cliente
                                     join p in context.pensiones on bp.ID_Tipo_Pension equals p.ID_Tipo_Pension
                                     select new Modelos.ModeloPensiones
                                     {
                                         ID = bp.ID,
                                         Folio = bp.Folio,
                                         Marca = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Marca).FirstOrDefault(),
                                         Color = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Color).FirstOrDefault(),
                                         Placa = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Placa).FirstOrDefault(),
                                         CobradoPor = bp.CobradoPor,
                                         CostoRegular = bp.Costo.ToString(),
                                         EdoPago = bp.StatusPago.ToString(),
                                         InicioPension = bp.Inicio_Pension.ToString(),
                                         TerminoPension = bp.Fin_Pension.ToString(),
                                         Pensn_Tipo = p.Pensn_Tipo,
                                     }).AsQueryable();

                switch (Consulta)
                {

                    case "Todas":
                        dataGridView.DataSource = listabusqueda.ToList();
                        break;
                    case "Pagadas":
                        listabusqueda = listabusqueda.Where(n => n.EdoPago.Contains("True"));
                        dataGridView.DataSource = listabusqueda.ToList();
                        break;
                    case "Pendientes":
                        listabusqueda = listabusqueda.Where(n => n.EdoPago.Contains("False"));
                        dataGridView.DataSource = listabusqueda.ToList();
                        break;
                    case "Inactivas":
                        listabusqueda = listabusqueda.Where(n => string.IsNullOrEmpty(n.EdoPago));
                        dataGridView.DataSource = listabusqueda.ToList();
                        break;
                }

            }

        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            Refresh_();
            this.Close();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            using (var a = new Clientes(Conexion, Usuario))
            {
                a.Refresh_ += A_Refresh_;
                a.ShowDialog();



            }
        }

        private void A_Refresh_()
        {
            if (radioTodas.Checked == true)
            {
                LlenarTabla("Todas");
                btnPagar.Visible = false;
                if (dataGridView.Rows.Count > 0)
                {
                    btnPagar.Visible = ((dataGridView.Rows[0].Cells[4].Value.ToString()) == "Pendiente") ? true : false;
                    btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;

                }
            }
            if (radioPagadas.Checked == true)
            {
                LlenarTabla("Pagadas");
                btnPagar.Visible = false;
                btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
            }
            if (radioPendientes.Checked == true)
            {
                LlenarTabla("Pendientes");
                btnPagar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
            }
            if (radioInactivas.Checked == true)
            {
                LlenarTabla("Inactivas");
                btnPagar.Visible = false;
                btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
            }
        }

        private void radioTodas_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTodas.Checked == true)
            {
                try
                {
                    LlenarTabla("Todas");
                    btnPagar.Visible = false;
                    if (dataGridView.Rows.Count > 0)
                    {
                        btnPagar.Visible = ((dataGridView.Rows[0].Cells[4].Value.ToString()) == "Pendiente") ? true : false;
                        btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                        int? id = GetID();
                        if (id != null)
                        {
                            CargarDatosDerecha(id.GetValueOrDefault());
                        }

                    }
                    if (dataGridView.Rows.Count == 0)
                    {
                        lblNombre.Text = "";
                        lblID.Text = "";
                        lblInscripcion.Text = "";
                        lblUltimoPago.Text = "";
                        lblDireccion.Text = "";
                        lblTelefono.Text = "";
                        lblDescripcion.Text = "";
                        lblCosto.Text = "";
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

        private void radioInactivas_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioInactivas.Checked == true)
                {
                    LlenarTabla("Inactivas");
                    btnPagar.Visible = false;
                    btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                    if (dataGridView.Rows.Count > 0)
                    {
                        int? id = GetID();
                        if (id != null)
                        {
                            CargarDatosDerecha(id.GetValueOrDefault());
                        }

                    }

                }
                if (dataGridView.Rows.Count == 0)
                {
                    lblNombre.Text = "";
                    lblID.Text = "";
                    lblInscripcion.Text = "";
                    lblUltimoPago.Text = "";
                    lblDireccion.Text = "";
                    lblTelefono.Text = "";
                    lblDescripcion.Text = "";
                    lblCosto.Text = "";
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

        private void radioPagadas_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioPagadas.Checked == true)
                {
                    LlenarTabla("Pagadas");
                    btnPagar.Visible = false;
                    btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                    if (dataGridView.Rows.Count > 0)
                    {
                        int? id = GetID();
                        if (id != null)
                        {
                            CargarDatosDerecha(id.GetValueOrDefault());
                        }

                    }
                }
                if (dataGridView.Rows.Count == 0)
                {
                    lblNombre.Text = "";
                    lblID.Text = "";
                    lblInscripcion.Text = "";
                    lblUltimoPago.Text = "";
                    lblDireccion.Text = "";
                    lblTelefono.Text = "";
                    lblDescripcion.Text = "";
                    lblCosto.Text = "";
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

        private void radioPendientes_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioPendientes.Checked == true)
                {
                    LlenarTabla("Pendientes");
                    btnPagar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                    btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                    if (dataGridView.Rows.Count > 0)
                    {
                        int? id = GetID();
                        if (id != null)
                        {
                            CargarDatosDerecha(id.GetValueOrDefault());
                        }

                    }

                }
                if (dataGridView.Rows.Count == 0)
                {
                    lblNombre.Text = "";
                    lblID.Text = "";
                    lblInscripcion.Text = "";
                    lblUltimoPago.Text = "";
                    lblDireccion.Text = "";
                    lblTelefono.Text = "";
                    lblDescripcion.Text = "";
                    lblCosto.Text = "";
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

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView.Rows.Count > 0)
            {
                var a = dataGridView.Rows[dataGridView.CurrentRow.Index].Cells[5].Value.ToString();
                btnPagar.Visible = (a == "Pendiente") ? true : false;
                btnEditar.Visible = (dataGridView.Rows.Count > 0) ? true : false;
                int? id = GetID();
                if (id != null)
                {
                    CargarDatosDerecha(id.GetValueOrDefault());
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                DialogResult resultado = MessageBox.Show("¿Desea eliminar la PENSION con el id:" + "\n" + id.ToString() + " ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var a = context.banco_pension.Where(n => n.ID == id).FirstOrDefault();
                        context.banco_pension.Remove(a);
                        context.SaveChanges();
                    }
                    radioTodas.Checked = true;
                    LlenarTabla("Todas");
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {

            int? id = GetID();
            if (id != null)
            {
                using (var a = new ModificarPension(Conexion, id.GetValueOrDefault(), Usuario))
                {
                    a.Refresh_ += A_Refresh_;
                    a.ShowDialog();


                }
            }
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            try
            {
            var pd = new PrintDocument();
            var ps = new PrinterSettings();
            pd.PrinterSettings = ps;
            pd.PrintPage += ImprimirReporte;
            pd.Print();

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

        private void ImprimirReporte(object sender, PrintPageEventArgs e)
        {
            bool? ancho = true;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                ancho = (from cf in context.config_tickets
                         where cf.ID_ticket == 1
                         select cf.Tamaño_papel).FirstOrDefault();

            }
            int espaciado = 10;
            //315 = 80mm
            int tamLetra = (ancho == true) ? 13 : 9;
            int tamPapel = (ancho == true) ? 305 : 218;
            Font fuenteNormal = new Font("Arial", tamLetra, FontStyle.Regular, GraphicsUnit.Point);
            Font fuenteNegrita = new Font("Arial", tamLetra, FontStyle.Bold, GraphicsUnit.Point);
            StringFormat LetrasCentradas = new StringFormat();
            LetrasCentradas.Alignment = StringAlignment.Center;
            LetrasCentradas.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("***REPORTE DE PENSIONES***", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado, tamPapel, 20), LetrasCentradas);
            e.Graphics.DrawString(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US")), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
            e.Graphics.DrawString("*****************************", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var totalPensiones = (from ps in context.banco_pension select ps.ID).Count();
                var totalActivas = (from ps in context.banco_pension where ps.StatusPago != null select ps.ID).Count();
                var totalPagadas = (from ps in context.banco_pension where ps.StatusPago == true select ps.ID).Count();
                var totalPendientes = (from ps in context.banco_pension where ps.StatusPago == false select ps.ID).Count();
                e.Graphics.DrawString("Pensiones Registradas: " + totalPensiones.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Pensiones Activas: " + totalActivas.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Pensiones Pagadas: " + totalPagadas.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Pensiones Pendientes: " + totalPendientes.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

            }
            e.Graphics.DrawString("*****************************", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);


        }
        private string Cultura()
        {
            var info = CultureInfo.CurrentCulture.NumberFormat;
            return info.NumberDecimalSeparator;
        }
        private void btnPagar_Click(object sender, EventArgs e)
        {
            try
            {
            string costoFormato =(Cultura()==",")? lblCosto.Text.Replace('.', ','): lblCosto.Text;
            int? id = GetID();
            int ultimoCorte;
            if (id != null)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var b = context.cortes.ToList();
                    var ultimo = b.LastOrDefault();
                    ultimoCorte = ultimo.Id_Corte;
                    var a = context.banco_pension.SingleOrDefault(n => n.ID == id);
                    if (a != null)
                    {
                        a.FechaUltimoPago = DateTime.Today;
                        a.StatusPago = true;
                        a.No_PagosAnticipados = 0;
                        a.Pagos = Convert.ToDecimal(costoFormato);
                        a.Pension_Corte = false;
                        a.Pension_Activa = true;
                        a.Id_corte = ultimoCorte;
                        context.SaveChanges();


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
            throw new NotImplementedException();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtFolio.Text) && string.IsNullOrEmpty(txtPlaca.Text))
            {
                SystemSounds.Hand.Play();
            }
                else
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var listabusqueda = (from bp in context.banco_pension
                                         join cl in context.cliente on bp.Id_cliente equals cl.Id_cliente
                                         join p in context.pensiones on bp.ID_Tipo_Pension equals p.ID_Tipo_Pension
                                         where bp.StatusPago == true
                                         select new Modelos.ModeloPensiones
                                         {
                                             ID = bp.ID,
                                             Folio = bp.Folio,
                                             Marca = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Marca).FirstOrDefault(),
                                             Color = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Color).FirstOrDefault(),
                                             Placa = (from vh in context.vehiculo where vh.Id_Vehiculo == cl.Id_Vehiculo select vh.Placa).FirstOrDefault(),
                                             CobradoPor = bp.CobradoPor,
                                             CostoRegular = bp.Costo.ToString(),
                                             EdoPago = bp.StatusPago.ToString(),
                                             InicioPension = bp.Inicio_Pension.ToString(),
                                             TerminoPension = bp.Fin_Pension.ToString(),
                                             Pensn_Tipo = p.Pensn_Tipo,
                                         }).AsQueryable();

                    if (!string.IsNullOrEmpty(txtFolio.Text))
                    {
                        listabusqueda = listabusqueda.Where(f => f.Folio.Contains(txtFolio.Text.Trim()));
                        var lista = listabusqueda.ToList();
                        dataGridView1.DataSource = (lista.Count() > 0) ? lista : null;
                        btnRegistroEntrada.Visible = (dataGridView1.Rows.Count > 0) ? true : false;
                    }
                    if (!string.IsNullOrEmpty(txtPlaca.Text))
                    {
                        listabusqueda = listabusqueda.Where(n => n.Placa.Contains(txtPlaca.Text.Trim()));
                        var lista = listabusqueda.ToList();
                        dataGridView1.DataSource = (lista.Count() > 0) ? lista : null;
                        btnRegistroEntrada.Visible = (dataGridView1.Rows.Count > 0) ? true : false;

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
        private void btnRegistrarSalida_Click(object sender, EventArgs e)
        {
            try
            {
            if (dtPensiones.Rows.Count > 0)
            {
                string id = GetIDBitacora();
                if (!string.IsNullOrEmpty(id))
                {
                    int idd = Convert.ToInt32(id);
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        DateTime fechahoy = DateTime.Today;
                        string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                        string salida2 = DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

                        DateTime sal2 = DateTime.Now;

                        var a = context.bitacora_pensiones.SingleOrDefault(n => n.Id_bitacora == idd);

                        string cierre = ("21:00:00");
                        TimeSpan sal = TimeSpan.Parse(salida);
                        TimeSpan horadecierre = TimeSpan.Parse(cierre);
                        var selecthistorial = (from bitacora in context.bitacora_pensiones
                                               where bitacora.Id_bitacora == idd
                                               select bitacora.Hora_Entrada).SingleOrDefault();
                        var selectfechaen = (from bitacora in context.bitacora_pensiones
                                             where bitacora.Id_bitacora == idd
                                             select bitacora.Fecha_Entrada).SingleOrDefault();

                        TimeSpan tiempototal = sal - selecthistorial;
                        // tiempototal  son las horas totales que ha estado en parking
                        double ttotal = tiempototal.TotalHours;

                        DateTime td = selectfechaen;
                        TimeSpan ts = selecthistorial;
                        td = td.Date + ts;
                        TimeSpan resultado = sal2.Subtract(td);
                        double horasdia = resultado.TotalHours;
                        string message = "Se registrara la salida a las : " + sal.ToString();

                        string caption = "Confirmar salida";
                        if (horasdia <= 12 && sal <= horadecierre)
                        {

                            var result = MessageBox.Show(message, caption,
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                a.Penalizacion = false;
                                a.Monto_Penalizacion = 0;
                                a.Hora_Salida = sal;
                                a.Fecha_salida = fechahoy;
                                a.Tiempo_Exedido = TimeSpan.Parse("00:00:00");
                                context.SaveChanges();
                                CargarSalidos();
                                CargarBitacora();
                                MessageBox.Show("Datos actualizados correctamente");
                            }
                            if (result == DialogResult.No)
                            {

                            }
                        }

                        if (horasdia > 12 && sal < horadecierre && fechahoy.Date == td.Date)
                        {

                            double fraccion = (ttotal - 12);
                            //fraccion = fraccion / 60;
                            int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                            fraccionados = fraccionados * 4;

                            if (((fraccion % 1) / 100 * 60) >= .01 & (((fraccion % 1) / 100 * 60) <= .15))
                            {
                                fraccionados = fraccionados + 1;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) <= .30))
                            {
                                fraccionados = fraccionados + 2;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) <= .45))
                            {
                                fraccionados = fraccionados + 3;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .46 & (((fraccion % 1) / 100 * 60) <= .59))
                            {
                                fraccionados = fraccionados + 4;

                            }
                            double costo2 = (fraccionados * 8);

                            var result = MessageBox.Show(message, caption,
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);
                            TimeSpan restante = tiempototal - TimeSpan.Parse("12:00:00");
                            if (result == DialogResult.Yes)
                            {
                                a.Penalizacion = true;
                                a.Monto_Penalizacion = Convert.ToDecimal(costo2);
                                a.Hora_Salida = sal;
                                a.Fecha_salida = fechahoy;
                                a.Tiempo_Exedido = restante;
                                context.SaveChanges(); 
                                CargarSalidos();
                                CargarBitacora();
                                MessageBox.Show("Hay un cargo extra por tiempo excedido\nCOBRAR: $" + costo2.ToString(), "Cargo extra", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                MessageBox.Show("Datos actualizados correctamente");


                            }
                            if (result == DialogResult.No)
                            {

                            }
                        }

                        if (DateTime.Now > (td.Date + horadecierre))
                        {
                            double precio;

                            TimeSpan hrsfinal = resultado - (horadecierre - selecthistorial);
                            double datohoras = hrsfinal.TotalHours;
                            //fraccion = fraccion / 60;;
                            if (datohoras % 1 > 0.01)
                            {
                                precio = (datohoras - (datohoras % 1)) + 1;
                            }
                            else { precio = (datohoras - (datohoras % 1)); }

                            double costo2 = (precio * 32);


                            var result = MessageBox.Show(message, caption,
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                a.Penalizacion = true;
                                a.Monto_Penalizacion = Convert.ToDecimal(costo2);
                                a.Hora_Salida = sal;
                                a.Fecha_salida = DateTime.Today;
                                a.Tiempo_Exedido = hrsfinal;
                                context.SaveChanges();
                                CargarSalidos();
                                CargarBitacora();

                                MessageBox.Show("Hay un cargo extra por tiempo excedido\nCOBRAR: $" + costo2.ToString(), "Cargo extra", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                MessageBox.Show("Datos actualizados correctamente");

                            }
                            if (result == DialogResult.No)
                            {
                            }
                        }
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

        /*  private void btnRegistrarSalida_Click(object sender, EventArgs e)
          {
              if (dtPensiones.Rows.Count>0)
              {
                  string id = GetIDBitacora();
                  if (!string.IsNullOrEmpty(id))
                  {
                      int idd = Convert.ToInt32(id);
                      using (var context = new PARKING_DBEntities(CadenaConexion))
                      {
                          //ya tengo el id 
                          DateTime fechahoy = DateTime.Now;
                          string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                          var a = context.bitacora_pensiones.SingleOrDefault(n => n.Id_bitacora == idd);

                          string cierre = ("21:00:00");
                          TimeSpan sal = TimeSpan.Parse(salida);
                          TimeSpan horadecierre = TimeSpan.Parse(cierre);
                          var selecthistorial = (from bitacora in context.bitacora_pensiones
                                                 where bitacora.Id_bitacora == idd
                                                 select bitacora.Hora_Entrada).SingleOrDefault();

                          TimeSpan tiempototal = sal - selecthistorial;
                          // tiempototal  son las horas totales que ha estado en parking
                          double ttotal = tiempototal.TotalHours;

                          string message = "Se registrara la salida a las : " + sal.ToString();

                          string caption = "Confirmar salida";

                          if (ttotal <= 12 && sal <= horadecierre)
                          {

                              var result = MessageBox.Show(message, caption,
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                              if (result == DialogResult.Yes)
                              {
                                  a.Penalizacion = false;
                                  a.Monto_Penalizacion = 0;
                                  a.Hora_Salida = sal;
                                  context.SaveChanges();
                                  //tus metodos para actualizar o no se que onda
                                  MessageBox.Show("Datos actualizados correctamente");
                              }
                              if (result == DialogResult.No)
                              {

                              }
                          }


                          if (ttotal > 12 && sal < horadecierre)
                          {

                              double fraccion = (ttotal - 12);
                              //fraccion = fraccion / 60;
                              MessageBox.Show(fraccion.ToString());
                              int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                              fraccionados = fraccionados * 4;

                              if (((fraccion % 1) / 100 * 60) >= .01 & (((fraccion % 1) / 100 * 60) <= .15))
                              {
                                  fraccionados = fraccionados + 1;

                              }
                              if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) <= .30))
                              {
                                  fraccionados = fraccionados + 2;

                              }
                              if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) <= .45))
                              {
                                  fraccionados = fraccionados + 3;

                              }
                              if (((fraccion % 1) / 100 * 60) >= .46 & (((fraccion % 1) / 100 * 60) <= .59))
                              {
                                  fraccionados = fraccionados + 4;

                              }
                              double costo2 = (fraccionados * 8);
                              MessageBox.Show("el costo es" + costo2.ToString());


                              var result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                              if (result == DialogResult.Yes)
                              {
                                  a.Penalizacion = true;
                                  a.Monto_Penalizacion = Convert.ToDecimal(costo2);
                                  a.Hora_Salida = sal;
                                  context.SaveChanges();
                                  //tus metodos para actualizar o no se que onda
                                  MessageBox.Show("Datos actualizados correctamente");
                              }
                              if (result == DialogResult.No)
                              {

                              }
                          }

                          if (sal > horadecierre)
                          {
                              double fraccion = (ttotal - 12);
                              //fraccion = fraccion / 60;
                              MessageBox.Show(fraccion.ToString());

                              double precio = (fraccion - (fraccion % 1)) + 1;

                              double costo2 = (precio * 32);
                              MessageBox.Show("el costo es" + costo2.ToString());


                              var result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                              if (result == DialogResult.Yes)
                              {
                                  a.Penalizacion = true;
                                  a.Monto_Penalizacion = Convert.ToDecimal(costo2);
                                  a.Hora_Salida = sal;
                                  a.Fecha_salida = DateTime.Today;
                                  context.SaveChanges();
                                  CargarSalidos();
                                  CargarBitacora();
                                  MessageBox.Show("Datos actualizados correctamente");

                              }
                              if (result == DialogResult.No)
                              {
                              }
                          }
                      }
                  }
              }
          }
          */
        private void btnRegistroEntrada_Click(object sender, EventArgs e)
        {
            try
            {
            string horaenpunto = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
            TimeSpan horanow = TimeSpan.Parse(horaenpunto);

            string id =GetEntrada();
            if (!string.IsNullOrEmpty(id))
            {
                int idint = Convert.ToInt32(id);
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var contador = context.bitacora_pensiones.Where(n => n.Id_Pension == idint&&n.Hora_Salida==null).Count();
                    if (contador==0)
                    {
                        var a = new bitacora_pensiones
                        {
                            Id_Pension = idint,
                            Fecha_Entrada=DateTime.Today,
                            Hora_Entrada=horanow,
                            Hora_Salida=null,
                            Monto_Penalizacion=null,
                            Penalizacion=false,
                            Tiempo_Exedido=null,
                        };
                        context.bitacora_pensiones.Add(a);
                        context.SaveChanges();
                        idUltimoIngreso = a.Id_bitacora;
                        CargarBitacora();
                        txtFolio.Text = "";
                        txtPlaca.Text = "";
                        dataGridView1.DataSource = null;
                        btnRegistroEntrada.Visible = false;
                       
                        var pd = new PrintDocument();
                        var ps = new PrinterSettings();
                        pd.PrinterSettings = ps;
                        pd.PrintPage += ImprimirEntrada;
                        pd.Print();

                    }
                    else
                    {
                        SystemSounds.Hand.Play();
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

        private void ImprimirEntrada(object sender, PrintPageEventArgs e)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var oBitacora = context.bitacora_pensiones.Where(n => n.Id_bitacora == idUltimoIngreso).FirstOrDefault();
                var oPension = context.banco_pension.Where(n => n.ID == oBitacora.Id_Pension).FirstOrDefault();
                var oCliente= context.cliente.Where(n => n.Id_cliente == oPension.Id_cliente).FirstOrDefault();
                var oVehiculo = context.vehiculo.Where(n => n.Id_Vehiculo == oCliente.Id_Vehiculo).FirstOrDefault();
                var confT = (context.config_tickets.Where(n => n.ID_ticket == 1)).FirstOrDefault();
                var nombreU = context.usuario.Where(n => n.Usuario1 == Usuario).Select(n => n.Nombre).FirstOrDefault();
                bool? ancho = confT.Tamaño_papel;
                //315 = 80mm
                int tamPapel = (ancho == true) ? 305 : 218;
                int tamLetra = (ancho == true) ? 10 : 8;
                int tamLetraPoliticas = (ancho == true) ? 6 : 4;
                float largoPoliticas = (ancho == true) ? 0.1491F : 0.096F;
                Font fuenteNormal = new Font("Arial", tamLetra, FontStyle.Bold, GraphicsUnit.Point);
                Font fuentePoliticas = new Font("Arial", tamLetraPoliticas, FontStyle.Bold, GraphicsUnit.Point);
                StringFormat LetrasCentradas = new StringFormat();
                LetrasCentradas.Alignment = StringAlignment.Center;
                LetrasCentradas.LineAlignment = StringAlignment.Center;
                Barcode CodigoBarras = new Barcode();
                var imgCodigoBarras = CodigoBarras.Encode(TYPE.CODE128, oPension.Folio, Color.Black, Color.White, 1300, 1000); // <---------
                int largoCodigoBarras = 80;
                int AnchoCodigoBarras = 150;

                string b = "";
                int primeraCordenada = 0;
                int tamRenglon;

                b = confT.Estacionamiento;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Direccion: " + confT.Direccion;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Razon Social: " + confT.Razon_Social;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "RFC: " + confT.RFC;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Placa: " + oVehiculo.Placa;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Marca: " + oVehiculo.Marca;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Color: " + oVehiculo.Color;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Atendido Por: " + nombreU;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Entrada: " + oBitacora.Fecha_Entrada.Day.ToString() + " " + oBitacora.Fecha_Entrada.ToString("MMMM", new CultureInfo("es-ES")) + " " + oBitacora.Fecha_Entrada.Year.ToString();
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = (string.Format("{0:hh\\:mm}", oBitacora.Hora_Entrada));
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                e.Graphics.DrawImage(imgCodigoBarras, new RectangleF(((tamPapel / 2) - (AnchoCodigoBarras / 2)), primeraCordenada += 5, AnchoCodigoBarras, largoCodigoBarras));
                primeraCordenada += largoCodigoBarras;

                b = oPension.Folio;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "******POLITICAS******";
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = confT.Desarrollador;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = confT.DatosEntrada;
                tamRenglon = Convert.ToInt32(largoPoliticas * b.Length);
                e.Graphics.DrawString(b, fuentePoliticas, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;





            }
        }

        private void CargarBitacora()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
               
                var listaBitacora = (from bit in context.bitacora_pensiones
                                     join pen in context.banco_pension on bit.Id_Pension equals pen.ID
                                     join cl in context.cliente on pen.Id_cliente equals cl.Id_cliente
                                     join vh in context.vehiculo on cl.Id_Vehiculo equals vh.Id_Vehiculo
                                     where bit.Hora_Salida == null
                                     select new Modelos.ModeloBitacoraPensiones
                                     {
                                         ID = bit.Id_bitacora,
                                         FechaEntrada = bit.Fecha_Entrada,
                                         FechaSalida=bit.Fecha_salida,
                                         Folio = pen.Folio,
                                         HoraEntrada = bit.Hora_Entrada,
                                         Placa = vh.Placa

                                     }).ToList();
                dtPensiones.DataSource = listaBitacora;
                btnRegistrarSalida.Visible = (dtPensiones.Rows.Count>0)?true:false;
            }
        }
        private void CargarSalidos()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listaBitacora = (from bit in context.bitacora_pensiones
                                     join pen in context.banco_pension on bit.Id_Pension equals pen.ID
                                     join cl in context.cliente on pen.Id_cliente equals cl.Id_cliente
                                     join vh in context.vehiculo on cl.Id_Vehiculo equals vh.Id_Vehiculo
                                     where bit.Hora_Salida != null
                                     select new Modelos.ModeloBitacoraPensiones
                                     {
                                         ID = bit.Id_bitacora,
                                         FechaEntrada = bit.Fecha_Entrada,
                                         FechaSalida=bit.Fecha_salida,
                                         Folio = pen.Folio,
                                         HoraEntrada = bit.Hora_Entrada,
                                         HoraSalida=bit.Hora_Salida,
                                         Placa = vh.Placa

                                     }).ToList();
                dataGridView2.DataSource = listaBitacora;
            }
        }
        private string GetEntrada()
        {
            try
            {
                var a = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                string placa = a;
                return placa;
            }
            catch
            {
                return null;


            }
        }
    }
    
}
