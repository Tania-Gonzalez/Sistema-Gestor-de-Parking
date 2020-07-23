using BarcodeLib;
using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;

namespace Parking.Forms
{
    public partial class Principal : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string conexion;
        private string usuario;
        int parqueo = 0;
        int pensionu = 0;
        int servicioad = 0;
        int idUltimoCobro, idUltimoIngreso;
        int copiasEntrada, copiasSalida;
        bool formaBusqueda, activarBusqueda, activarbtnCortesia, activarbtnCancelar, ActivarbtnPerdido;
        banco_parking Busqueda = new banco_parking();

        #endregion
        public Principal()
        {
            InitializeComponent();

            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
           
            if (string.IsNullOrEmpty(oSettings.CadenaConexion)||string.IsNullOrEmpty(oSettings.Serial))
            {
                CrearBase crearBase = new CrearBase();
                crearBase.ShowDialog();
            }
            else
            {
                var valido = DateTime.Parse(oSettings.FechaTermino);
                if (valido<DateTime.Today)
                {
                    Caducada caducada = new Caducada();
                    caducada.ShowDialog();
                }
                else
                {
                    CadenaConexion = oSettings.CadenaConexion;
                    Login login = new Login();
                    login.Cadena += Login_Cadena;
                    login.ShowDialog();
                }
            
            }

           


        }
        private void Login_Cadena(string t, string u)
        {
            conexion = t;
            usuario = u;

        }

        private void Refresh_Logo()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var ruta = context.config_tickets.Where(n => n.ID_ticket == 1).FirstOrDefault();
                if (ruta.Imagen != null && ruta.Imagen.Length > 0)
                {
                    Bitmap bmp;
                    using (var ms = new MemoryStream(ruta.Imagen))
                    {
                        bmp = new Bitmap(ms);
                        pictureBox1.Image = bmp;
                    }
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.logo4parking;
                }



                var listabusqueda = (from bp in context.banco_pension
                                     join cl in context.cliente on bp.Id_cliente equals cl.Id_cliente
                                     join p in context.pensiones on bp.ID_Tipo_Pension equals p.ID_Tipo_Pension
                                     select new Modelos.ModeloPensiones
                                     {
                                         ID = bp.ID,
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
                listabusqueda = listabusqueda.Where(n => n.EdoPago.Contains("False"));
                dtPendientesPago.DataSource = listabusqueda.ToList();
                //1 placa 0 folio
                var busqueda = context.configgen1.Where(n => n.ID_config == 1).Select(n => n.Buscar_vehiculo).FirstOrDefault();
                lblFormaBusqueda.Text = (busqueda == false) ? "Folio" : "Placa";
                label11.Text = (busqueda == false) ? "Folio:" : "Placa:";
                formaBusqueda = busqueda;
                var prioridad = context.usuario.Where(n => n.Usuario1 == usuario).FirstOrDefault();
                var habilitarTicketPerdido = context.configgen1.Where(n => n.ID_config == 1).Select(n => n.Ticker_perdido).FirstOrDefault();
                ActivarbtnPerdido = (habilitarTicketPerdido == true&&(prioridad.Privilegios == "Administrador" || prioridad.Privilegios == "Supervisor")) ? true : false;
                var habilitarTicketCancelado = context.configgen1.Where(n => n.ID_config == 1).Select(n => n.Ticket_cancel).FirstOrDefault();
                activarbtnCancelar = (habilitarTicketCancelado == true && (prioridad.Privilegios == "Administrador" || prioridad.Privilegios == "Supervisor")) ? true : false;
                var habilitarTicketCortesia = context.configgen1.Where(n => n.ID_config == 1).Select(n => n.Ticket_cortesia).FirstOrDefault();
                activarbtnCortesia = (habilitarTicketCortesia == true && (prioridad.Privilegios == "Administrador" || prioridad.Privilegios == "Supervisor")) ? true : false;

                var copiaentrada = context.config_tickets.Where(n => n.ID_ticket == 1).Select(n => n.Cantidad_Copias_Entrada).FirstOrDefault();
                copiasEntrada = copiaentrada;
                var copiasalida = context.config_tickets.Where(n => n.ID_ticket == 1).Select(n => n.Cantidad_Copias_Salida).FirstOrDefault();
                copiasSalida = copiasalida;
                ObtenerServicioAD();

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            txtBoxFecha.Text = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")) + " " + DateTime.Now.Year.ToString();
            txtBoxHora.Text = DateTime.Now.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US"));


        }

        private void Principal_Load(object sender, EventArgs e)
        {

            // ObtenerCovenio();
            ObtenerServicioAD();
            // ObtenerParqueo();
            LlenarTablaParking();
            parking();
            marcas();
            colores();
            CheckarPensionEdit();
            ObtenerServicioE();
            Refresh_Logo();
            txtPlaca.Focus();
            ckPensionUnica.Checked = true;
            cbPensionU.SelectedIndex = 1;
            checkPensionEdit.Checked = true;
            cbPensionUedit.SelectedIndex = 1;
            rbTarifa.Checked = true;
            txtPlaca.Focus();
        }

        public void CheckarPensionEdit()
        {
            if (checkPensionEdit.Checked == true)
            {
                cbPensionUedit.Enabled = true;
                int a = cbPensionUedit.SelectedIndex;
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {

                    Dictionary<string, string> diccionario = new Dictionary<string, string>();

                    var item = context.pensiones_unicas.ToList();
                    diccionario.Add("0", "Seleccione");

                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre_PensionU;
                        var id = valores.ID_PensionU;
                        var activa = valores.PensionU_Activa;
                        if (activa == true)
                        {
                            diccionario.Add(id.ToString(), nombre);
                        }

                    }
                    cbPensionUedit.DataSource = new BindingSource(diccionario, null);
                    cbPensionUedit.DisplayMember = "Value";
                    cbPensionUedit.ValueMember = "Key";
                }
                cbPensionUedit.SelectedIndex = 0;
            }
            else
            {
                cbPensionUedit.Enabled = false;

            }
        }

        public void ObtenerServicioE()
        {
            int b = cbServicioEdit.SelectedIndex;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.serviciosadicionales.ToList();
                diccionario.Add("0", "Seleccione");

                foreach (var valores in item)
                {
                    var nombre = valores.ServicioAdicional;
                    var id = valores.ID_ServicioAd;
                    var activa = valores.ServicioAd_Activo;
                    if (activa == true)
                    {
                        diccionario.Add(id.ToString(), nombre);
                    }

                }
                cbServicioEdit.DataSource = new BindingSource(diccionario, null);
                cbServicioEdit.DisplayMember = "Value";
                cbServicioEdit.ValueMember = "Key";
            }
            cbServicioEdit.SelectedIndex = 0;
        }
        private void LlenarTablaParking()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var listabusqueda = (from bpark in context.banco_parking
                                     where bpark.Estancia == "EN PROGRESO"
                                     select new Modelos.ModeloParking
                                     {

                                         Hora = bpark.HoraEntrada,
                                         Placa = bpark.Placa.ToString()
                                         ,
                                         Folio = bpark.Folio


                                     }).AsQueryable();

                dtParking.DataSource = listabusqueda.ToList();
            }

        }

        public void ObtenerPensionU()
        {
            if (ckPensionUnica.Checked == true)
            {
                cbPensionU.Enabled = true;
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {

                    Dictionary<string, string> diccionario = new Dictionary<string, string>();

                    var item = context.pensiones_unicas.ToList();
                    diccionario.Add("0", "Seleccione");

                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre_PensionU;
                        var id = valores.ID_PensionU;
                        var activa = valores.PensionU_Activa;
                        if (activa == true)
                        {
                            diccionario.Add(id.ToString(), nombre);
                        }

                    }
                    cbPensionU.DataSource = new BindingSource(diccionario, null);
                    cbPensionU.DisplayMember = "Value";
                    cbPensionU.ValueMember = "Key";
                }
               // cbPensionU.SelectedIndex = 0;

            }
            else
            {
                cbPensionU.Enabled = false;
            }
        }

        public void ObtenerParqueo()
        {

            int a = cbParqueo.SelectedIndex;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.cobro.ToList();
                diccionario.Add("0", "Seleccione");

                foreach (var valores in item)
                {
                    var nombre = valores.Nombre;
                    var id = valores.ID_Tabulador;
                    var activa = valores.Tarifa_Habilitada;
                    var tipo = valores.TipoCobro;
                    if (activa == true & tipo == true)
                    {
                        diccionario.Add(id.ToString(), nombre);
                    }

                }
                cbParqueo.DataSource = new BindingSource(diccionario, null);
                cbParqueo.DisplayMember = "Value";
                cbParqueo.ValueMember = "Key";
            }
            cbParqueo.SelectedIndex = 0;

        }
        public void ObtenerServicioAD()
        {

            int b = cbServicioAD.SelectedIndex;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.serviciosadicionales.ToList();
                diccionario.Add("0", "Seleccione");

                foreach (var valores in item)
                {
                    var nombre = valores.ServicioAdicional;
                    var id = valores.ID_ServicioAd;
                    var activa = valores.ServicioAd_Activo;
                    if (activa == true)
                    {
                        diccionario.Add(id.ToString(), nombre);
                    }

                }
                cbServicioAD.DataSource = new BindingSource(diccionario, null);
                cbServicioAD.DisplayMember = "Value";
                cbServicioAD.ValueMember = "Key";
            }
            cbServicioAD.SelectedIndex = 0;

        }
        public void SelectDatos()
        {
            panelGenerales.BringToFront();

            string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
            //corregir single
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var selectcolor = (from bpark in context.banco_parking
                                   where bpark.Placa == placa && bpark.Estancia == "EN PROGRESO"
                                   select bpark.Color).SingleOrDefault();

                var selectmarca = (from bpark in context.banco_parking
                                   where bpark.Placa == placa && bpark.Estancia == "EN PROGRESO"
                                   select bpark.Marca).SingleOrDefault();

                var selectentrada = (from bp in context.banco_parking
                                     where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                     select bp.HoraEntrada).SingleOrDefault();

                var selectfecha = (from bp in context.banco_parking
                                   where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                   select bp.FechaEntrada).SingleOrDefault();

                var selectestancia = (from bp in context.banco_parking
                                      where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                      select bp.Estancia).SingleOrDefault();

                var selecttarifaconvenio = (from bp in context.banco_parking
                                            from tc in context.cobro
                                            where bp.Placa == placa && bp.Estancia == "EN PROGRESO" && tc.ID_Tabulador == bp.ID_Cobro
                                            select tc.Nombre).SingleOrDefault();

                var selecttc = (from bp in context.banco_parking
                                from tc in context.cobro
                                where bp.Placa == placa && bp.Estancia == "EN PROGRESO" && tc.ID_Tabulador == bp.ID_Cobro
                                select tc.TipoCobro).SingleOrDefault();

                var color = selectcolor;
                var marca = selectmarca;
                var hora = selectentrada;
                var fecha = selectfecha;
                var estancia = selectestancia;

                txtColorG.Text = color;
                txtMarcaG.Text = marca;
                txtPlacaG.Text = placa;
                txtHoraRegistro.Text = hora.ToString();
                txtFechaRegistro.Text = fecha.Value.ToShortDateString();
                txtEstancia.Text = estancia;
                txtTarConv.Text = selecttarifaconvenio.ToString();

                if (selecttc == true) { txtTC.Text = "PARQUEO"; } else { txtTC.Text = "CONVENIO"; }

            }
        }

        public void parking()
        {

            string fechahoy = DateTime.Today.ToString("d");
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                DateTime hoy = DateTime.Parse(fechahoy);
                var counparking = (from bp in context.banco_parking
                                   where bp.Estancia == "EN PROGRESO"
                                   select bp.Estancia).Count();

                var counentradas = (from bp in context.banco_parking
                                    where bp.FechaEntrada == hoy
                                    select bp.FechaEntrada).Count();

                var capacidad = (from config in context.configgen1
                                 select config.Capacidad).SingleOrDefault();

                var totalPagadas = (from ps in context.banco_pension where ps.StatusPago == true select ps.ID).Count();

                lblNumPension.Text = totalPagadas.ToString();

                var disponibles = capacidad - counparking;


                var counparkinghoy = (from bp in context.banco_parking
                                      where bp.Estancia != "EN PROGRESO" && bp.FechaEntrada == hoy
                                      select bp.Estancia).Count();

                var countsalidas = (from bp in context.banco_parking
                                      where bp.Estancia == "EN PROGRESO" && bp.FechaEntrada == hoy
                                      select bp.Estancia).Count();

                var salidas = counentradas - countsalidas;



                txtEntradas.Text = counentradas.ToString();
                txtActivos.Text = counparking.ToString();
                txtSalidas.Text = salidas.ToString();
                lblDisponibles.Text = disponibles.ToString();


            }

        }
        public void borrado()
        {
            lblTiempoParqueo.Text = "";
            lblTiempoParqueo2.Text = "";
            lblParqueo.Text = "";
            lblServicios.Text = "";
            lblTiempoLibre.Text = "";
            lblTotal.Text = "";
            txtEstancia.Text = "";
            txtHoraRegistro.Text = "";
            txtFechaRegistro.Text = "";
            btnCancelado.Visible = false;
            btCortesia.Visible = false;
            btTicketPerdido.Visible = false;

        }

        public void colores()
        {
            var source = new AutoCompleteStringCollection();
            source.AddRange(new string[]

            { "AMARILLO",
            "ÁMBAR",
            "AÑIL",
            "AZUL",
            "AZUL CLARO",
            "AZUL ELÉCTRICO",
            "AZUL MARINO",
            "BEIGE",
            "BLANCO",
            "CAFÉ",
            "CAQUI",
            "CELESTE",
            "CEREZA",
            "CIAN",
            "COBRE",
            "CORAL",
            "FUCSIA",
            "GRIS",
            "LILA",
            "MAGENTA",
            "MARRÓN",
            "MORADO",
            "NARANJA",
            "NEGRO",
            "OCRE",
            "PARDO",
            "PLATA",
            "PÚRPURA",
            "ROJO",
            "ROSA",
            "VERDE",
            "VIOLETA",
            "VINO"
            });
            txtColor.AutoCompleteCustomSource = source;
            txtColor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtColor.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }
        public void marcas()
        {

            var source = new AutoCompleteStringCollection();
            source.AddRange(new string[]
                            { "Acura",
                              "Alfa",
                              "Audi",
                              "BAIC",
                              "Bentley",
                                "BMW",
                                "Chrysler" ,
                                "Fiat" ,
                                "Ford" ,
                                "General" ,
                                "Honda" ,
                                "Hyundai" ,
                                "Infiniti" ,
                                "Isuzu" ,
                                "JAC" ,
                                "Jaguar" ,
                                "KIA" ,
                                "Land" ,
                                "Lincoln" ,
                                "Mazda" ,
                                "Mercedes" ,
                                "Mini" ,
                                "Mitsubishi" ,
                                "Nissan" ,
                                "Peugeot" ,
                                "Porsche" ,
                                "Renault" ,
                                "SEAT" ,
                                "Smart" ,
                                "Subaru" ,
                                "Suzuki" ,
                                "Toyota" ,
                                "Volkswagen" ,
                                "Volvo"});
            txtMarca.AutoCompleteCustomSource = source;
            txtMarca.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtMarca.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }

        public void Datos()
        {



            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            int mes = myCal.GetMonth(DateTime.Now);
            int semana = myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);
            string fechahoy = DateTime.Today.ToString("d");
            string horaenpunto = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

            DateTime hoy = DateTime.Parse(fechahoy);
            TimeSpan horanow = TimeSpan.Parse(horaenpunto);

            string placa = txtPlaca.Text;



            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                string identificador = "";
                var idcobro = (from cb in context.cobro
                               where cb.ID_Tabulador == parqueo
                               select cb.TipoCobro).SingleOrDefault();
                if (idcobro == false) { identificador = "CV"; }
                if (idcobro == true) { identificador = "TF"; }

                if (pensionu != 9)
                {
                    identificador = "TL";
                }

                var ultimoID = context.banco_parking.ToList().LastOrDefault();

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



                var selectestancia = (from bp in context.banco_parking
                                      where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                      select bp.Estancia).Count();
                var a = selectestancia;
                if (a >= 1)
                {

                    MessageBox.Show("PLACA REPETIDA EN PARKING");

                }

                else
                {
                    var ban_park = new banco_parking()
                    {
                        Folio = identificador,
                        FechaEntrada = hoy,
                        HoraEntrada = horanow,
                        ExpedidoPor = usuario,
                        Placa = txtPlaca.Text,
                        Marca = txtMarca.Text,
                        Color = txtColor.Text,
                        ID_PensionU = pensionu,
                        ID_Cobro = parqueo,
                        ID_ServicioAd = servicioad,
                        ID_Penalizacion = 1,
                        FechaSalida = null,
                        HoraSalida = null,
                        CobradoPor = null,
                        T_Parqueo = null,
                        Estancia = "EN PROGRESO",
                        Importe = null,
                        Pago = null,
                        ModoCobro = null,
                        ModoTicket = null,
                        Justificacion = null,
                        Corte = 0,
                        Semana = semana,
                        Mes = mes

                    };
                    context.banco_parking.Add(ban_park);
                    context.SaveChanges();
                    idUltimoIngreso = ban_park.ID;
                    lblUltimoIngreso.Text = string.Format("{0:hh\\:mm}", horanow);

                    SystemSounds.Asterisk.Play();
                    if (copiasEntrada > 0)
                    {
                        var pd = new PrintDocument();
                        var ps = new PrinterSettings();
                        pd.PrinterSettings = ps;
                        pd.PrintPage += ImprimirEntrada;

                        for (int i = 0; i < copiasEntrada; i++)
                        {
                            pd.Print();

                        }
                    }

                }

            }

        }

        public void Cobrar()
        {
            string fechahoy = DateTime.Today.ToString("d");
            DateTime hoy = DateTime.Parse(fechahoy);

            string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");
                string salida = DateTime.Now.ToString("hh:mm:ss", new System.Globalization.CultureInfo("en-US"));
                TimeSpan salid = TimeSpan.Parse(a.HoraEntrada.ToString());

                var selectcobro = (from cb in context.configgen1
                                   select cb.Modo_cobro).SingleOrDefault();

                var modo = selectcobro;

                if (string.IsNullOrWhiteSpace(lblTiempoParqueo.Text))
                {
                    MessageBox.Show("Seleccione una placa para efectuar un cobro");
                }
                else
                {
                    if (a != null)
                    {

                        TimeSpan horasalida = salid + TimeSpan.Parse(lblTiempoParqueo.Text);

                        a.FechaSalida = hoy;
                        a.HoraSalida = horasalida;
                        a.CobradoPor = usuario;
                        a.T_Parqueo = TimeSpan.Parse(lblTiempoParqueo.Text);
                        a.Estancia = "FINALIZADA";
                        a.Importe = Convert.ToDecimal(lblTotal.Text);
                        a.Pago = 1;
                        a.ModoCobro = modo.ToString();
                        a.ModoTicket = "NORMAL";
                        a.Justificacion = "-";
                        context.SaveChanges();
                        idUltimoCobro = a.ID;
                        SystemSounds.Asterisk.Play();
                        borrado();

                        if (copiasSalida > 0)
                        {
                            var pd = new PrintDocument();
                            var ps = new PrinterSettings();
                            pd.PrinterSettings = ps;
                            pd.PrintPage += ImprimirSalida;

                            for (int i = 0; i < copiasSalida; i++)
                            {
                                pd.Print();

                            }
                        }




                    }
                }
            }
        }
        private void btnPensiones_Click(object sender, EventArgs e)
        {
            using (var a = new Pensiones(conexion, usuario))
            {
                a.Refresh_ += Refresh_Logo;
                a.ShowDialog();

            }
        }

        private void btnPanelControl_Click(object sender, EventArgs e)
        {
            using (var a = new PanelControl(conexion, usuario))
            {
                a.Refresh_ += Refresh_Logo;
                a.ShowDialog();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {

            if (string.IsNullOrEmpty(txtMarca.Text) || string.IsNullOrEmpty(txtPlaca.Text) || string.IsNullOrEmpty(txtColor.Text))
            {
                MessageBox.Show("Faltan Campos Por Llenar");
            }
            else
            {
                string fechahoy = DateTime.Today.ToString("d");
                string horaenpunto = DateTime.Now.ToString("hh:mm:ss", new System.Globalization.CultureInfo("en-US"));


                pensionu = Convert.ToInt32(((KeyValuePair<string, string>)cbPensionU.SelectedItem).Key);
                if (pensionu == 0) { pensionu = 9; }

                parqueo = Convert.ToInt32(((KeyValuePair<string, string>)cbParqueo.SelectedItem).Key);

                servicioad = Convert.ToInt32(((KeyValuePair<string, string>)cbServicioAD.SelectedItem).Key);
                if (servicioad == 0) { servicioad = 1; }

                if (parqueo == 0) { MessageBox.Show("Por favor elija el tipo de vehiculo que desea ingresar"); }
                if (rbConvenio.Checked == true && pensionu < 9)
                {
                    MessageBox.Show("No se puede ingresar un convenio con pension unica");
                }
                else
                {
                    Datos();
                    LlenarTablaParking();
                    txtPlaca.Text = "";
                    txtMarca.Text = "";
                    txtColor.Text = "";
                    parking();




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
                var confT = (context.config_tickets.Where(n => n.ID_ticket == 1)).FirstOrDefault();
                var oParkin = (context.banco_parking.Where(n => n.ID == idUltimoIngreso)).FirstOrDefault();
                var nombreU = context.usuario.Where(n => n.Usuario1 == usuario).Select(n => n.Nombre).FirstOrDefault();
                var concepto = (from bp in context.banco_parking
                                join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                where bp.ID == idUltimoIngreso
                                select cb.Nombre).FirstOrDefault();
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
                var imgCodigoBarras = CodigoBarras.Encode(TYPE.CODE128, oParkin.Folio, Color.Black, Color.White, 1300, 1000); // <---------
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

                b = "Placa: " + oParkin.Placa;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Marca: " + oParkin.Marca;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Color: " + oParkin.Color;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Concepto: " + concepto;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Atendido Por: " + nombreU;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Entrada: " + oParkin.FechaEntrada.Value.Day.ToString() + " " + oParkin.FechaEntrada.Value.ToString("MMMM", new CultureInfo("es-ES")) + " " + oParkin.FechaEntrada.Value.Year.ToString();
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = (string.Format("{0:hh\\:mm\\:ss}", oParkin.HoraEntrada));
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                e.Graphics.DrawImage(imgCodigoBarras, new RectangleF(((tamPapel / 2) - (AnchoCodigoBarras / 2)), primeraCordenada += 5, AnchoCodigoBarras, largoCodigoBarras));
                primeraCordenada += largoCodigoBarras;

                b = oParkin.Folio;
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

        private string GetPlaca()
        {
            try
            {
                var a = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
                string placa = a;
                return placa;
            }
            catch
            {
                return null;

            }
        }
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            try
            {
                string placa = GetPlaca();
                decimal total = (string.IsNullOrEmpty(lblTotal.Text)) ? 0 : Convert.ToDecimal(lblTotal.Text);
                decimal efectivo = Convert.ToDecimal(txtEfectivo.Text);
                if (!string.IsNullOrEmpty(placa) && (!string.IsNullOrEmpty(txtFechaRegistro.Text)) && efectivo >= total)
                {
                    Cobrolineal();
                    Cobrar();
                    LlenarTablaParking();
                    parking();
                    panelAgregarVehiculo.BringToFront();
                    txtEfectivo.Text = "";
                }
                else
                {

                    MessageBox.Show("¡Coloque un importe igual o mayor al costo!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void ImprimirSalida(object sender, PrintPageEventArgs e)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var confT = (context.config_tickets.Where(n => n.ID_ticket == 1)).FirstOrDefault();
                var oParkin = (context.banco_parking.Where(n => n.ID == idUltimoCobro)).FirstOrDefault();
                var nombreU = context.usuario.Where(n => n.Usuario1 == usuario).Select(n => n.Nombre).FirstOrDefault();
                var concepto = (from bp in context.banco_parking
                                join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                where bp.ID == idUltimoCobro
                                select cb.Nombre).FirstOrDefault();
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

                b = "Folio: " + oParkin.Folio;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Placa: " + oParkin.Placa;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Marca: " + oParkin.Marca;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Color: " + oParkin.Color;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "-" + oParkin.ModoTicket + "-";
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Concepto: " + concepto;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Cobrado por: " + nombreU;
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Entrada: " + oParkin.FechaEntrada.Value.Day.ToString() + " " + oParkin.FechaEntrada.Value.ToString("MMMM", new CultureInfo("es-ES")) + " " + oParkin.FechaEntrada.Value.Year.ToString();
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = string.Format("{0:hh\\:mm\\:ss}", oParkin.HoraEntrada);
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;


                b = "Salida: " + oParkin.FechaSalida.Value.Day.ToString() + " " + oParkin.FechaEntrada.Value.ToString("MMMM", new CultureInfo("es-ES")) + " " + oParkin.FechaEntrada.Value.Year.ToString();
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = string.Format("{0:hh\\:mm\\:ss}", oParkin.HoraSalida);
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Estancia: " + string.Format("{0:hh}", oParkin.T_Parqueo) + "Hr " + string.Format("{0:mm}", oParkin.T_Parqueo) + "Min " + string.Format("{0:ss}", oParkin.T_Parqueo) + "Seg";
                tamRenglon = (ancho == true) ? (b.Length < 31) ? 15 : (b.Length < 62) ? 30 : 45 : (b.Length < 28) ? 10 : (b.Length < 56) ? 20 : 30;
                e.Graphics.DrawString(b, fuenteNormal, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

                b = "Importe: $" + oParkin.Importe;
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

                b = confT.DatosSalida;
                tamRenglon = Convert.ToInt32(largoPoliticas * b.Length);
                e.Graphics.DrawString(b, fuentePoliticas, Brushes.Black, new RectangleF(0, primeraCordenada += 5, tamPapel, tamRenglon), LetrasCentradas);
                primeraCordenada += tamRenglon;

            }
        }

        public void SoloNumerosDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }


            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public void Cobrolineal()
        {
            string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
            using (var context = new PARKING_DBEntities())
            {
                var selectplaca = (from bp in context.banco_parking
                                   where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                   select bp.Placa).SingleOrDefault();
                var placaso = selectplaca;
                var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");

                var selectpension = (from pensionunica in context.pensiones_unicas
                                     from bp in context.banco_parking
                                     where pensionunica.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                     select pensionunica.ID_Tipo_PensionU).SingleOrDefault();
                var pension = selectpension;

                var selectforma = (from cobro in context.cobro
                                   from bp in context.banco_parking
                                   where bp.ID_Cobro == cobro.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                   select cobro.FormaCobro).SingleOrDefault();

                var selecttipocobro = (from cobro in context.cobro
                                       from bp in context.banco_parking
                                       where bp.ID_Cobro == cobro.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                       select cobro.TipoCobro).SingleOrDefault();

                var tipocobroconvenio = selecttipocobro;

                var mincobro = (from cb in context.cobro
                                from bp in context.banco_parking
                                where bp.ID_Cobro == cb.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                select cb.CobroLineal_minDefault).SingleOrDefault();

                var cobro_lineal = (from cb in context.cobro
                                    from bp in context.banco_parking
                                    where bp.ID_Cobro == cb.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                    select cb.CobroLineal_Importe).SingleOrDefault();

                #region if pension ninguna
                if (pension == 3 && placaso == placa)
                {
                    //lineal ninguna
                    if (a != null)
                    {
                        //forma de cobro lineal 
                        #region if lineal 
                        if (selectforma == false)
                        {

                            var selectfechaen = (from hour in context.banco_parking
                                                 where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                 select hour.FechaEntrada).SingleOrDefault();
                            var selecthoraen = (from hour in context.banco_parking
                                                where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                select hour.HoraEntrada).SingleOrDefault();

                            DateTime dt = DateTime.Parse(selectfechaen.ToString());
                            TimeSpan ts = TimeSpan.Parse(selecthoraen.ToString());
                            dt = dt.Date + ts;
                            DateTime sali = DateTime.Now;

                            TimeSpan result = sali.Subtract(dt);

                            double horasTotales = result.TotalHours;

                          

                            lblTiempoParqueo.Text = result.ToString("c");


                            var selectcobro = (from cobrando in context.cobro
                                               from bp in context.banco_parking
                                               where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                               select cobrando.CobroLineal_ImporteDefault).FirstOrDefault();

                            var importe = selectcobro;
                            var importe_lineal = cobro_lineal;
                            //costo para una hora con servicio adicional o tiempo libre
                            double costo = Convert.ToInt32(importe);
                            double costo_lineal = Convert.ToInt32(importe_lineal);
                            #region if
                            if ((horasTotales * 60) < (mincobro))
                            {
                                lblParqueo.Text = importe.ToString();

                                string placas = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();

                                using (var contexts = new PARKING_DBEntities())
                                {
                                    var selectservicio = (from servi in context.serviciosadicionales
                                                          from bp in context.banco_parking
                                                          where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                          select servi.Precio_ServiciosAd).FirstOrDefault();

                                    var selectpensionu = (from pensionu in context.pensiones_unicas
                                                          from bp in context.banco_parking
                                                          where pensionu.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                          select pensionu.Precio_PensionU).FirstOrDefault();
                                    var servicio = selectservicio;
                                    lblServicios.Text = servicio.ToString();
                                    costo += Convert.ToInt32(servicio);

                                    var pensi = selectpensionu;
                                    lblTiempoLibre.Text = pensi.ToString();
                                    lblTotal.Text = costo.ToString();
                                }
                            }
                            #endregion

                            #region else
                            else
                            {
                                double fraccion = (horasTotales * 60) - Convert.ToDouble(mincobro);
                                fraccion = fraccion / 60;
                                int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                                fraccionados = fraccionados * 4;

                                if (((fraccion % 1) / 100 * 60) > .00 & (((fraccion % 1) / 100 * 60) < .16))
                                {
                                    fraccionados = fraccionados + 1;

                                }
                                if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) < .31))
                                {
                                    fraccionados = fraccionados + 2;

                                }
                                if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) < .46))
                                {
                                    fraccionados = fraccionados + 3;

                                }
                                if (((fraccion % 1) / 100 * 60) >= .46)
                                {
                                    fraccionados = fraccionados + 4;

                                }
                                double costo2 = costo + ((fraccionados) * costo_lineal);
                                costo = costo + ((fraccionados) * costo_lineal);

                                lblParqueo.Text = costo.ToString();
                                string placas = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();

                                using (var contexts = new PARKING_DBEntities())
                                {
                                    var selectservicio = (from servi in context.serviciosadicionales
                                                          from bp in context.banco_parking
                                                          where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placas && bp.Estancia == "EN PROGRESO"
                                                          select servi.Precio_ServiciosAd).FirstOrDefault();

                                    var selectpensionu = (from pensionu in context.pensiones_unicas
                                                          from bp in context.banco_parking
                                                          where pensionu.ID_PensionU == bp.ID_PensionU && bp.Placa == placas && bp.Estancia == "EN PROGRESO"
                                                          select pensionu.Precio_PensionU).FirstOrDefault();
                                    var servicio = selectservicio;

                                    lblServicios.Text = servicio.ToString();
                                    costo += (Convert.ToDouble(servicio));

                                    var pensi = selectpensionu;
                                    costo += Convert.ToDouble(pensi);
                                    lblTiempoLibre.Text = pensi.ToString();
                                    lblTotal.Text = costo.ToString();

                                }
                            }
                        }
                        #endregion
                        #endregion
                        //forma de cobro tabulador 
                        #region if tabulador
                        if (selectforma == true)
                        {
                            var selecthoraen = (from hour in context.banco_parking
                                                where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                select hour.HoraEntrada).SingleOrDefault();

                            var selectfechaen = (from hour in context.banco_parking
                                                 where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                 select hour.FechaEntrada).SingleOrDefault();

                            var selectid = (from id in context.cobro
                                            from bp in context.banco_parking
                                            where id.ID_Tabulador == bp.ID_Cobro && bp.Estancia == "EN PROGRESO" && bp.Placa == placa
                                            select id.ID_Tabulador).SingleOrDefault();

                            TimeSpan diferenciaHoras = new TimeSpan();

                            DateTime dt = DateTime.Parse(selectfechaen.ToString());
                            TimeSpan ts = TimeSpan.Parse(selecthoraen.ToString());

                            DateTime hoy = DateTime.Now;

                            dt = dt.Date + ts;

                            TimeSpan result = hoy.Subtract(dt);

                            lblTiempoParqueo.Text = result.ToString("c");

                            int horas = diferenciaHoras.Hours;
                            double horasTotales = result.TotalHours;

                            if (horasTotales <= 24)
                            {
                                int fraccionados = Convert.ToInt32(horasTotales - (horasTotales % 1));
                                fraccionados = fraccionados * 2;
                                if (((horasTotales % 1) / 100 * 60) > 0 & (((horasTotales % 1) / 100 * 60) <= .299))
                                {
                                    fraccionados = fraccionados + 1;

                                }
                                if (((horasTotales % 1) / 100 * 60) >= .30 & (((horasTotales % 1) / 100 * 60) <= .59))
                                {
                                    fraccionados = fraccionados + 2;

                                }
                                string valorcolumna = ("min_" + ((fraccionados * 30).ToString()));

                                var query = context.cobro.ToList();

                                var resultado = query
                                    .Where(p => p.ID_Tabulador == selectid)
                                    .Select("(" + valorcolumna + ")");
                                double precioparqueo = 0;
                                foreach (var cbcb in resultado)
                                {
                                    lblTotal.Text = cbcb.ToString();
                                    lblParqueo.Text = cbcb.ToString();
                                    precioparqueo = Convert.ToDouble(cbcb);
                                }
                                var selectservicio = (from servi in context.serviciosadicionales
                                                      from bp in context.banco_parking
                                                      where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                      select servi.Precio_ServiciosAd).FirstOrDefault();
                                var servicio = selectservicio;

                                lblServicios.Text = servicio.ToString();


                                precioparqueo = precioparqueo + Convert.ToDouble(servicio);
                                lblTotal.Text = precioparqueo.ToString();
                            }
                            else
                            {
                              

                                int auxiliar = (int)((horasTotales / 24));
                               
                                horasTotales = horasTotales - (auxiliar * 24);

                                int fraccionados = Convert.ToInt32(horasTotales - (horasTotales % 1));
                                fraccionados = fraccionados * 2;
                                if (((horasTotales % 1) / 100 * 60) > 0 & (((horasTotales % 1) / 100 * 60) <= .299))
                                {
                                    fraccionados = fraccionados + 1;

                                }
                                if (((horasTotales % 1) / 100 * 60) >= .30 & (((horasTotales % 1) / 100 * 60) <= .59))
                                {
                                    fraccionados = fraccionados + 2;

                                }
                                
                                string valorcolumna = ("min_" + ((fraccionados * 30).ToString()));

                                var query = context.cobro.ToList();
                                var totaldias = (from cb in context.cobro
                                                 where cb.ID_Tabulador == selectid
                                                 select cb.min_1440).SingleOrDefault();

                                var resultado = query
                                    .Where(p => p.ID_Tabulador == selectid)
                                    .Select("(" + valorcolumna + ")");
                                double precioparqueo = 0;

                                totaldias *= auxiliar;

                                foreach (var cbcb in resultado)
                                {
                                    lblTotal.Text = cbcb.ToString();
                                    precioparqueo = Convert.ToDouble(cbcb);
                                }
                                var selectservicio = (from servi in context.serviciosadicionales
                                                      from bp in context.banco_parking
                                                      where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                      select servi.Precio_ServiciosAd).FirstOrDefault();
                                var servicio = selectservicio;
                                precioparqueo = Convert.ToDouble(totaldias) + precioparqueo;
                                lblParqueo.Text = (precioparqueo).ToString();

                                lblServicios.Text = servicio.ToString();


                                precioparqueo = precioparqueo + Convert.ToDouble(servicio);
                                lblTotal.Text = precioparqueo.ToString();

                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region if pension 1
                if (pension == 1 && a.Placa == placa && tipocobroconvenio == true)
                {
                    var Selectnombre = (from tipop in context.pensiones_unicas
                                        from bp in context.banco_parking
                                        where tipop.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                        select tipop.Nombre_PensionU).SingleOrDefault();

                    var nombre = Selectnombre;

                    var selectprecio = (from preciop in context.pensiones_unicas
                                        where preciop.Nombre_PensionU == nombre
                                        select preciop.Precio_PensionU).SingleOrDefault();

                    var iniciopension = (from iniciop in context.tipos_pensionesu
                                         where iniciop.ID_Tipo_PensionU == pension
                                         select iniciop.HoraInicio).SingleOrDefault();

                    var finpension = (from finp in context.tipos_pensionesu
                                      where finp.ID_Tipo_PensionU == pension
                                      select finp.HoraFin).SingleOrDefault();

                    var tiempo = finpension - iniciopension;

                    var precio = selectprecio;

                    var horaentrada = (from entra in context.banco_parking
                                       where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                       select entra.HoraEntrada).SingleOrDefault();

                    var fechaentrada = (from entra in context.banco_parking
                                        where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                        select entra.FechaEntrada).SingleOrDefault();

                    var selecthorafin = (from tp in context.tipos_pensionesu
                                         where tp.ID_Tipo_PensionU == 1
                                         select tp.HoraFin).SingleOrDefault();

                    DateTime dt = DateTime.Parse(fechaentrada.ToString());
                    TimeSpan ts = TimeSpan.Parse(horaentrada.ToString());
                    dt = dt.Date + ts;

                    DateTime salida = DateTime.Now;

                    TimeSpan horasTs = salida.Subtract(dt);
                    double horasTotales = horasTs.TotalHours;

                    lblTiempoParqueo.Text = horasTs.ToString("c");

                    lblTiempoLibre.Text = precio.ToString();


                    #region pension dentro del tiempo

                    var selectservicio = (from servi in context.serviciosadicionales
                                          from bp in context.banco_parking
                                          where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                          select servi.Precio_ServiciosAd).FirstOrDefault();
                    var servicio = selectservicio;

                    if (horasTs <= tiempo && salida.TimeOfDay < selecthorafin)
                    {

                        lblParqueo.Text = (0.00).ToString();
                        lblServicios.Text = servicio.ToString();
                        lblTotal.Text = (Convert.ToDouble(precio) + Convert.ToDouble(servicio)).ToString();

                    }
                    #endregion

                    #region fuera de tiempo
                    else
                    {
                        var selectcobro = (from cobrando in context.cobro
                                           from bp in context.banco_parking
                                           where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa
                                           select cobrando.CobroLineal_Importe).FirstOrDefault();

                        var importe = selectcobro;
                        //costo para una hora con servicio adicional o tiempo libre
                        double costo = Convert.ToDouble(importe);
                        TimeSpan resultadohoras = TimeSpan.Parse(selecthorafin.ToString()) - TimeSpan.Parse(horaentrada.ToString());
                        double horaslibre = resultadohoras.TotalHours;
                        double fraccion = horasTotales - horaslibre;
                        if (fraccion <= 24)
                        {
                            int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                            fraccionados = fraccionados * 4;

                            if (((fraccion % 1) / 100 * 60) > .00 & (((fraccion % 1) / 100 * 60) < .16))
                            {
                                fraccionados = fraccionados + 1;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) < .31))
                            {
                                fraccionados = fraccionados + 2;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) < .46))
                            {
                                fraccionados = fraccionados + 3;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .46)
                            {
                                fraccionados = fraccionados + 4;

                            }
                            double totalprice = ((fraccionados) * costo);

                            lblServicios.Text = servicio.ToString();
                            lblParqueo.Text = totalprice.ToString();
                            lblTotal.Text = (totalprice + Convert.ToDouble(servicio) + Convert.ToDouble(precio)).ToString();
                        }
                        else
                        {

                            int auxiliar = Convert.ToInt32(fraccion / 24);
                            fraccion = fraccion - auxiliar * 24;

                            int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                            fraccionados = fraccionados * 4;

                            if (((fraccion % 1) / 100 * 60) > .00 & (((fraccion % 1) / 100 * 60) < .16))
                            {
                                fraccionados = fraccionados + 1;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) < .31))
                            {
                                fraccionados = fraccionados + 2;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) < .46))
                            {
                                fraccionados = fraccionados + 3;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .46)
                            {
                                fraccionados = fraccionados + 4;

                            }
                            double totalprice = ((fraccionados) * costo) + (costo * auxiliar * 24 * 4);

                            lblServicios.Text = servicio.ToString();
                            lblParqueo.Text = totalprice.ToString();
                            lblTotal.Text = (totalprice + Convert.ToDouble(servicio) + Convert.ToDouble(precio)).ToString();

                        }
                    }
                    #endregion
                }
                #endregion

                #region if pension = 2
                if (pension == 2 && a.Placa == placa && tipocobroconvenio == true)
                {
                    var Selectnombre = (from tipop in context.pensiones_unicas
                                        from bp in context.banco_parking
                                        where tipop.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                        select tipop.Nombre_PensionU).SingleOrDefault();

                    var nombre = Selectnombre;

                    var selectprecio = (from preciop in context.pensiones_unicas
                                        where preciop.Nombre_PensionU == nombre
                                        select preciop.Precio_PensionU).SingleOrDefault();

                    var iniciopension = (from iniciop in context.tipos_pensionesu
                                         where iniciop.ID_Tipo_PensionU == pension
                                         select iniciop.HoraInicio).SingleOrDefault();

                    var finpension = (from finp in context.tipos_pensionesu
                                      where finp.ID_Tipo_PensionU == pension
                                      select finp.HoraFin).SingleOrDefault();

                    var tiempo = finpension - iniciopension;

                    var precio = selectprecio;

                    var horaentrada = (from entra in context.banco_parking
                                       where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                       select entra.HoraEntrada).SingleOrDefault();

                    var fechaentrada = (from entra in context.banco_parking
                                        where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                        select entra.FechaEntrada).SingleOrDefault();

                    var selecthorafin = (from tp in context.tipos_pensionesu
                                         where tp.ID_Tipo_PensionU == 2
                                         select tp.HoraFin).SingleOrDefault();

                    DateTime dt = DateTime.Parse(fechaentrada.ToString());
                    TimeSpan ts = TimeSpan.Parse(horaentrada.ToString());
                    dt = dt.Date + ts;

                    DateTime salida = DateTime.Now;

                    TimeSpan horasTs = salida.Subtract(dt);
                    double horasTotales = horasTs.TotalHours;

                    lblTiempoParqueo.Text = horasTs.ToString("c");

                    lblTiempoLibre.Text = precio.ToString();


                    #region pension dentro del tiempo

                    var selectservicio = (from servi in context.serviciosadicionales
                                          from bp in context.banco_parking
                                          where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                          select servi.Precio_ServiciosAd).FirstOrDefault();
                    var servicio = selectservicio;

                    if (horasTs <= tiempo && salida.TimeOfDay < selecthorafin)
                    {

                        lblParqueo.Text = (0.00).ToString();
                        lblServicios.Text = servicio.ToString();
                        lblTotal.Text = (Convert.ToDouble(precio) + Convert.ToDouble(servicio)).ToString();

                    }
                    #endregion

                    #region fuera de tiempo
                    else
                    {
                        var selectcobro = (from cobrando in context.cobro
                                           from bp in context.banco_parking
                                           where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa
                                           select cobrando.CobroLineal_Importe).FirstOrDefault();

                        var importe = selectcobro;
                        //costo para una hora con servicio adicional o tiempo libre
                        double costo = Convert.ToDouble(importe);
                        TimeSpan resultadohoras = TimeSpan.Parse(selecthorafin.ToString()) - TimeSpan.Parse(horaentrada.ToString());
                        double horaslibre = resultadohoras.TotalHours;
                        double fraccion = horasTotales - horaslibre;
                        if (fraccion <= 24)
                        {
                            int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                            fraccionados = fraccionados * 4;

                            if (((fraccion % 1) / 100 * 60) > .00 & (((fraccion % 1) / 100 * 60) < .16))
                            {
                                fraccionados = fraccionados + 1;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) < .31))
                            {
                                fraccionados = fraccionados + 2;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) < .46))
                            {
                                fraccionados = fraccionados + 3;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .46)
                            {
                                fraccionados = fraccionados + 4;

                            }
                            double totalprice = ((fraccionados) * costo);

                            lblServicios.Text = servicio.ToString();
                            lblParqueo.Text = totalprice.ToString();
                            lblTotal.Text = (totalprice + Convert.ToDouble(servicio) + Convert.ToDouble(precio)).ToString();
                        }
                        else
                        {

                            int auxiliar = Convert.ToInt32(fraccion / 24);
                            fraccion = fraccion - auxiliar * 24;

                            int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                            fraccionados = fraccionados * 4;

                            if (((fraccion % 1) / 100 * 60) > .00 & (((fraccion % 1) / 100 * 60) < .16))
                            {
                                fraccionados = fraccionados + 1;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .16 & (((fraccion % 1) / 100 * 60) < .31))
                            {
                                fraccionados = fraccionados + 2;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .31 & (((fraccion % 1) / 100 * 60) < .46))
                            {
                                fraccionados = fraccionados + 3;

                            }
                            if (((fraccion % 1) / 100 * 60) >= .46)
                            {
                                fraccionados = fraccionados + 4;

                            }
                            double totalprice = ((fraccionados) * costo) + (costo * auxiliar * 24 * 4);

                            lblServicios.Text = servicio.ToString();
                            lblParqueo.Text = totalprice.ToString();
                            lblTotal.Text = (totalprice + Convert.ToDouble(servicio) + Convert.ToDouble(precio)).ToString();

                        }
                    }
                    #endregion
                }
                #endregion
            }

        }
        private void dtParking_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                borrado();
                SelectDatos();
                Cobrolineal();
                parking();
                btnCancelado.Visible = (activarbtnCancelar == true) ? true : false;
                btCortesia.Visible = (activarbtnCortesia == true) ? true : false;
                btTicketPerdido.Visible = (ActivarbtnPerdido == true) ? true : false;
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            panelAgregarVehiculo.BringToFront();
            borrado();

        }

        private void cbPensionU_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObtenerPensionU();
        }

        private void cbServicioAD_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObtenerServicioAD();
        }

        private void rbTarifa_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                Dictionary<string, string> diccionario = new Dictionary<string, string>();
                var item = context.cobro.Where(n => n.TipoCobro == true && n.Tarifa_Habilitada == true).ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Nombre;
                    var id = valores.ID_Tabulador;
                    diccionario.Add(id.ToString(), nombre);
                }
                cbParqueo.DataSource = new BindingSource(diccionario, null);
                cbParqueo.DisplayMember = "Value";
                cbParqueo.ValueMember = "Key";
            }
            if (cbParqueo.Items.Count > 0) { cbParqueo.SelectedIndex = 0; }
        }

        private void rbConvenio_CheckedChanged(object sender, EventArgs e)
        {

            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                Dictionary<string, string> diccionario = new Dictionary<string, string>();
                var item = context.cobro.Where(n => n.TipoCobro == false && n.Tarifa_Habilitada == true).ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Nombre;
                    var id = valores.ID_Tabulador;
                    diccionario.Add(id.ToString(), nombre);
                }
                cbParqueo.DataSource = new BindingSource(diccionario, null);
                cbParqueo.DisplayMember = "Value";
                cbParqueo.ValueMember = "Key";
            }
            if (cbParqueo.Items.Count > 0) { cbParqueo.SelectedIndex = 0; }
        }

        private void btTicketPerdido_Click(object sender, EventArgs e)
        {
            try
            {
            string test = GetPlaca();
            if (!string.IsNullOrEmpty(test) && (!string.IsNullOrEmpty(txtFechaRegistro.Text)))
            {
                string fechahoy = DateTime.Today.ToString("d");
                DateTime hoy = DateTime.Parse(fechahoy);

                string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {

                    var selectmulta = (from multa in context.configgen1
                                       select multa.Multa).SingleOrDefault();
                    int totalmasperdido = Convert.ToInt32(lblTotal.Text);

                    totalmasperdido += 50;

                    var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");
                    string salida = DateTime.Now.ToString("hh:mm:ss", new System.Globalization.CultureInfo("en-US"));
                    TimeSpan salid = TimeSpan.Parse(a.HoraEntrada.ToString());

                    var selectcobro = (from cb in context.configgen1
                                       select cb.Modo_cobro).SingleOrDefault();

                    var modo = selectcobro;

                    string message = "Desea generar un ticket perdido para el vehiculo con placa: " + placa +
                                          "\n MULTA POR TICKET PERDIDO: $" + selectmulta +
                                          "\n PRECIO FINAL : $" + totalmasperdido.ToString();
                    string caption = "IMPORTE POR TICKET PERDIDO";

                    var result = MessageBox.Show(message, caption,
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

                    if (string.IsNullOrWhiteSpace(lblTiempoParqueo.Text))
                    {
                        MessageBox.Show("Seleccione una placa para efectuar un cobro");
                    }
                    else
                    {
                        if (a != null)
                        {
                            if (result == DialogResult.Yes)
                            {
                                lblTotal.Text = totalmasperdido.ToString();

                                TimeSpan horasalida = salid + TimeSpan.Parse(lblTiempoParqueo.Text);
                                a.FechaSalida = hoy;
                                a.HoraSalida = horasalida;
                                a.CobradoPor = usuario;
                                a.T_Parqueo = TimeSpan.Parse(lblTiempoParqueo.Text);
                                a.Estancia = "FINALIZADA";
                                a.Importe = totalmasperdido;
                                a.Pago = 1;
                                a.ModoCobro = modo.ToString();
                                a.ModoTicket = "PERDIDO";
                                a.Justificacion = "-";
                                context.SaveChanges();
                                borrado();
                                LlenarTablaParking();
                                parking();
                                MessageBox.Show("Cobrado con cargo por ticket perdido");
                                panelAgregarVehiculo.BringToFront();
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

        private void btCortesia_Click(object sender, EventArgs e)
        {
            try
            {
            string test = GetPlaca();
            if (!string.IsNullOrEmpty(test) && (!string.IsNullOrEmpty(txtFechaRegistro.Text)))
            {
                string fechahoy = DateTime.Today.ToString("d");
                DateTime hoy = DateTime.Parse(fechahoy);

                string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");
                    string salida = DateTime.Now.ToString("hh:mm:ss", new CultureInfo("en-US"));
                    TimeSpan salid = TimeSpan.Parse(a.HoraEntrada.ToString());

                    var selectcobro = (from cb in context.configgen1
                                       select cb.Modo_cobro).SingleOrDefault();

                    var modo = selectcobro;

                    string message = "Desea generar una cortesia para el vehiculo con placa: " + placa;

                    string caption = "CORTESIA";

                    var result = MessageBox.Show(message, caption,
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);
                    if (string.IsNullOrWhiteSpace(lblTiempoParqueo.Text))
                    {
                        MessageBox.Show("Seleccione una placa para efectuar un cobro");
                    }
                    else
                    {
                        if (a != null)
                        {
                            if (result == DialogResult.Yes)
                            {
                                TimeSpan horasalida = salid + TimeSpan.Parse(lblTiempoParqueo.Text);
                                a.FechaSalida = hoy;
                                a.HoraSalida = horasalida;
                                a.CobradoPor = usuario;
                                a.T_Parqueo = TimeSpan.Parse(lblTiempoParqueo.Text);
                                a.Estancia = "FINALIZADA";
                                a.Importe = 0;
                                a.Pago = 1;
                                a.ModoCobro = modo.ToString();
                                a.ModoTicket = "CORTESIA";
                                a.Justificacion = "-";
                                context.SaveChanges();
                                borrado();
                                LlenarTablaParking();
                                parking();
                                MessageBox.Show("Cortesia generada correctamente");
                                panelAgregarVehiculo.BringToFront();
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

        private void ckPensionUnica_CheckedChanged(object sender, EventArgs e)
        {

            ObtenerPensionU();
        }

        private void checkPensionEdit_CheckedChanged(object sender, EventArgs e)
        {

            CheckarPensionEdit();
        }

        private void cbServicioEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObtenerServicioAD();
        }

        private void cbPensionUedit_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckarPensionEdit();
        }

        private void cbServicioAD_Click(object sender, EventArgs e)
        {
            ObtenerServicioAD();
        }

        private void cbPensionU_Click(object sender, EventArgs e)
        {
            try
            {
                ObtenerPensionU();

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

        private void cbPensionUedit_Click(object sender, EventArgs e)
        {
            CheckarPensionEdit();
        }

        private void cbServicioEdit_Click(object sender, EventArgs e)
        {
            ObtenerServicioE();
        }

        private void UPDATE_Click(object sender, EventArgs e)
        {

            string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");

                var selectcobro = (from cb in context.configgen1
                                   select cb.Modo_cobro).SingleOrDefault();

                var modo = selectcobro;

                var selecttipocobro = (from cobro in context.cobro
                                       from bp in context.banco_parking
                                       where bp.ID_Cobro == cobro.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                       select cobro.TipoCobro).SingleOrDefault();

                var tipocobroconvenio = selecttipocobro;

                string message = "Se actualizaran los datos del vehiculo con placa: " + placa;

                string caption = "";

                if (tipocobroconvenio == false)
                {
                    MessageBox.Show("IMPOSIBLE AGREGAR UNA PENSION, YA SE CUENTA CON UN CONVENIO");

                }
                else
                {
                    pensionu = Convert.ToInt32(((KeyValuePair<string, string>)cbPensionUedit.SelectedItem).Key);
                    if (pensionu == 0) { pensionu = 9; }

                    servicioad = Convert.ToInt32(((KeyValuePair<string, string>)cbServicioEdit.SelectedItem).Key);
                    if (servicioad == 0) { servicioad = 1; }

                    var result = MessageBox.Show(message, caption,
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);
                    if (a != null)
                    {
                        if (result == DialogResult.Yes)
                        {
                            a.ID_PensionU = pensionu;
                            a.ID_ServicioAd = servicioad;
                            context.SaveChanges();
                            borrado();
                            LlenarTablaParking();
                            parking();
                            MessageBox.Show("Datos actualizados correctamente");
                        }
                        if (result == DialogResult.No)
                        {

                        }
                    }
                }
            }
        }

        private void cbServicioAD_Click_1(object sender, EventArgs e)
        {
            try
            {
                ObtenerServicioAD();
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

        private void btnCancelado_Click(object sender, EventArgs e)
        {
            try
            {
            string pl = GetPlaca();
            if (!string.IsNullOrEmpty(pl) && (!string.IsNullOrEmpty(txtFechaRegistro.Text)))
            {
                string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");
                    string message = "Desea cancelar la estancia del vehiculo con placa: " + placa;

                    string caption = "CANCELACION";

                    var result = MessageBox.Show(message, caption,
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

                    var deletev = (from bp in context.banco_parking
                                   where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                   select bp).SingleOrDefault();

                    if (a != null)
                    {
                        if (result == DialogResult.Yes)
                        {
                            context.banco_parking.Remove(deletev);
                            context.SaveChanges();
                            MessageBox.Show("Cancelado correctamente");
                            borrado();
                            LlenarTablaParking();
                            parking();
                            panelAgregarVehiculo.BringToFront();
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

        private void lbTeclasRapidas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Teclas de acceso rapido:\n\n" +
                              "'F1' -> Ingresar con Tarifa Auto\n\n" +
                              "'F5' -> Mover Cursor a Nueva Placa\n\n" +
                              "'F8' -> Cobrar Placa Seleccionada"
                );
        }

        private void txtEfectivo_TextChanged(object sender, EventArgs e)
        {
            txtEfectivo.Text = (string.IsNullOrEmpty(txtEfectivo.Text)) ? "0" : txtEfectivo.Text;
        }

        private void txtPlaca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            { txtMarca.Focus(); }
        }

        private void txtMarca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            { txtColor.Focus(); }
        }

        private void txtColor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            { cbPensionU.Focus(); }
        }

        private void cbPensionU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            { cbServicioAD.Focus(); }
        }

        private void cbPensionU_Click_1(object sender, EventArgs e)
        {

        }

        private void cbServicioAD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            { btnIngresar.PerformClick(); }
        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var pagina = context.configgen1.Where(n => n.ID_config == 1).Select(n => n.DireccionFactura).FirstOrDefault();
                    ProcessStartInfo sInfo = new ProcessStartInfo(pagina);
                    Process.Start(pagina);
                  

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
                MessageBox.Show("No se pudo abrir la pagina, configure correctamente la pagina que desea abrir en las configuraciones \n");


            }



        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                btnIngresar_Click(sender, e);

            }
            if (e.KeyData == Keys.F5)
            {
                txtPlaca.Focus();

            }
            if (e.KeyData == Keys.F8)
            {
                btnCobrar_Click(sender, e);

            }
        }

        private void txtFolio_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (!string.IsNullOrEmpty(txtFolio.Text))
            {
                if (e.KeyData == Keys.Enter)
                {

                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        if (formaBusqueda == true)
                        {
                            var busqueda = context.banco_parking.Where(n => n.Estancia == "EN PROGRESO" && n.Placa == txtFolio.Text.Trim()).FirstOrDefault();
                            Busqueda = busqueda;

                            btnReimpresion.Text = (busqueda != null) ? Busqueda.Placa.ToString() : "Sin Resultado";
                            activarBusqueda = (busqueda != null) ? true : false;
                        }
                        if (formaBusqueda == false)
                        {
                            var busqueda = context.banco_parking.Where(n => n.Estancia == "EN PROGRESO" && n.Folio == txtFolio.Text.Trim()).FirstOrDefault();
                            Busqueda = busqueda;
                            btnReimpresion.Text = (busqueda != null) ? Busqueda.Folio.ToString() : "Sin Resultado";
                            activarBusqueda = (busqueda != null) ? true : false;


                           

                            string placa = txtFolio.Text;
                         
                            var selectcount = (from bpark in context.banco_parking
                                               where bpark.Folio == placa && bpark.Estancia == "EN PROGRESO"
                                               select bpark.Color).Count();
                            if (selectcount == 1)
                            {
                                panelGenerales.BringToFront();
                                var selectcolor = (from bpark in context.banco_parking
                                                   where bpark.Folio == placa && bpark.Estancia == "EN PROGRESO"
                                                   select bpark.Color).SingleOrDefault();

                                var selectmarca = (from bpark in context.banco_parking
                                                   where bpark.Folio == placa && bpark.Estancia == "EN PROGRESO"
                                                   select bpark.Marca).SingleOrDefault();

                                var selectentrada = (from bp in context.banco_parking
                                                     where bp.Folio == placa && bp.Estancia == "EN PROGRESO"
                                                     select bp.HoraEntrada).SingleOrDefault();

                                var selectfecha = (from bp in context.banco_parking
                                                   where bp.Folio == placa && bp.Estancia == "EN PROGRESO"
                                                   select bp.FechaEntrada).SingleOrDefault();

                                var selectestancia = (from bp in context.banco_parking
                                                      where bp.Folio == placa && bp.Estancia == "EN PROGRESO"
                                                      select bp.Estancia).SingleOrDefault();

                                var selecttarifaconvenio = (from bp in context.banco_parking
                                                            from tc in context.cobro
                                                            where bp.Folio == placa && bp.Estancia == "EN PROGRESO" && tc.ID_Tabulador == bp.ID_Cobro
                                                            select tc.Nombre).SingleOrDefault();

                                var selecttc = (from bp in context.banco_parking
                                                from tc in context.cobro
                                                where bp.Folio == placa && bp.Estancia == "EN PROGRESO" && tc.ID_Tabulador == bp.ID_Cobro
                                                select tc.TipoCobro).SingleOrDefault();

                                var selectplaca = (from bp in context.banco_parking
                                                   from tc in context.cobro
                                                   where bp.Folio == placa && bp.Estancia == "EN PROGRESO" && tc.ID_Tabulador == bp.ID_Cobro
                                                   select bp.Placa).SingleOrDefault();

                                var color = selectcolor;
                                var marca = selectmarca;
                                var hora = selectentrada;
                                var fecha = selectfecha;
                                var estancia = selectestancia;

                                txtColorG.Text = color;
                                txtMarcaG.Text = marca;
                                txtPlacaG.Text = selectplaca;
                                txtHoraRegistro.Text = hora.ToString();
                                txtFechaRegistro.Text = fecha.Value.ToShortDateString();
                                txtEstancia.Text = estancia;
                                txtTarConv.Text = selecttarifaconvenio.ToString();

                                if (selecttc == true) { txtTC.Text = "PARQUEO"; } else { txtTC.Text = "CONVENIO"; }
                            }
                            else { }

                        }


                    }
                }


            }


        }

        private void btnReimpresion_Click(object sender, EventArgs e)
        {
            try
            {
            if (activarBusqueda == true)
            {
                if (formaBusqueda == true)
                {
                    DialogResult resultado = MessageBox.Show("Desea reimprimir el ticket de entrada para la placa: " + Busqueda.Placa + "?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resultado == DialogResult.Yes)
                    {

                        var pd = new PrintDocument();
                        var ps = new PrinterSettings();
                        pd.PrinterSettings = ps;
                        pd.PrintPage += ReImprimirEntrada;
                        pd.Print();
                        //

                    }
                }
                if (formaBusqueda == false)
                {
                    DialogResult resultado = MessageBox.Show("Desea reimprimir el ticket de entrada para el folio: " + Busqueda.Folio + "?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resultado == DialogResult.Yes)
                    {
                        var pd = new PrintDocument();
                        var ps = new PrinterSettings();
                        pd.PrinterSettings = ps;
                        pd.PrintPage += ReImprimirEntrada;
                        pd.Print();
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

        private void ReImprimirEntrada(object sender, PrintPageEventArgs e)
        {
            bool bsqda = formaBusqueda;

            bool? ancho = true;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                ancho = (from cf in context.config_tickets
                         where cf.ID_ticket == 1
                         select cf.Tamaño_papel).FirstOrDefault();

            }

            int espaciado = 10;
            //315 = 80mm
            int largoCodigoBarras = 80;
            int AnchoCodigoBarras = 150;
            int largoPoliticas = (ancho == true) ? 170 : 110;
            int tamID = (ancho == true) ? 12 : 10;
            int tamLetra = (ancho == true) ? 10 : 8;
            int tamLetraPoliticas = (ancho == true) ? 6 : 4;
            int tamPapel = (ancho == true) ? 305 : 218;
            Font fuenteNormal = new Font("Arial", tamLetra, FontStyle.Regular, GraphicsUnit.Point);
            Font fuenteID = new Font("Arial", tamID, FontStyle.Bold, GraphicsUnit.Point);
            Font fuenteNegrita = new Font("Arial", tamLetra, FontStyle.Bold, GraphicsUnit.Point);
            Font fuentePoliticas = new Font("Arial", tamLetraPoliticas, FontStyle.Bold, GraphicsUnit.Point);
            StringFormat LetrasCentradas = new StringFormat();
            LetrasCentradas.Alignment = StringAlignment.Center;
            LetrasCentradas.LineAlignment = StringAlignment.Center;

            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var ultimoID = Busqueda;
                var folio = ultimoID.Folio.ToString();
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
                Barcode CodigoBarras = new Barcode();
                var imgCodigoBarras = CodigoBarras.Encode(TYPE.CODE128, folio, Color.Black, Color.White, 1300, 1000); // <---------

                var confTicket = context.config_tickets.Where(n => n.ID_ticket == 1);
                var nombreU = context.usuario.Where(n => n.Usuario1 == usuario).Select(n => n.Nombre).FirstOrDefault();
                var concepto = (from bp in context.banco_parking
                                join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                where bp.Folio == ultimoID.Folio
                                select cb.Nombre).FirstOrDefault();
                foreach (var item in confTicket)
                {
                    e.Graphics.DrawString(item.Estacionamiento.ToString(), fuenteID, Brushes.Black, new RectangleF(0, espaciado, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Direccion: " + item.Direccion, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Razon Social: " + item.Razon_Social, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("RFC: " + item.RFC, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Placa: " + ultimoID.Placa, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Marca: " + ultimoID.Marca, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Color: " + ultimoID.Color, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Concepto: " + concepto, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Atendido Por: " + nombreU, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Entrada: " + ultimoID.FechaEntrada.Value.Day.ToString() + " " + ultimoID.FechaEntrada.Value.ToString("MMMM", new CultureInfo("es-ES")) + " " + ultimoID.FechaEntrada.Value.Year, fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString(string.Format("{0:hh\\:mm}", ultimoID.HoraEntrada), fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawImage(imgCodigoBarras, new RectangleF(((tamPapel / 2) - (AnchoCodigoBarras / 2)), espaciado += 20, AnchoCodigoBarras, largoCodigoBarras));
                    e.Graphics.DrawString("A -" + "* " + folio + " *", fuenteID, Brushes.Black, new RectangleF(0, espaciado += 80, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("******POLITICAS******", fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString("Desarrollado por: IDEASTRO", fuenteNegrita, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                    e.Graphics.DrawString(item.DatosEntrada, fuentePoliticas, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, largoPoliticas), LetrasCentradas);
                }

            }

        }



        /*  public void Cobrolineal2()
           {
               string placa = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();
               using (var context = new PARKING_DBEntities(CadenaConexion))
               {
                   var selectplaca = (from bp in context.banco_parking
                                      where bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                      select bp.Placa).SingleOrDefault();
                   var placaso = selectplaca;
                   var a = context.banco_parking.SingleOrDefault(n => n.Placa == placa && n.Estancia == "EN PROGRESO");

                   var selectpension = (from pensionunica in context.pensiones_unicas
                                        from bp in context.banco_parking
                                        where pensionunica.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                        select pensionunica.ID_Tipo_PensionU).SingleOrDefault();
                   var pension = selectpension;

                   var selectforma = (from cobro in context.cobro
                                      from bp in context.banco_parking
                                      where bp.ID_Cobro == cobro.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                      select cobro.FormaCobro).SingleOrDefault();

                   var selecttipocobro = (from cobro in context.cobro
                                          from bp in context.banco_parking
                                          where bp.ID_Cobro == cobro.ID_Tabulador && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                          select cobro.TipoCobro).SingleOrDefault();

                   var tipocobroconvenio = selecttipocobro;

                   #region if pension ninguna
                   if (pension == 3 && placaso == placa)
                   {
                       //lineal ninguna
                       if (a != null)
                       {
                           //forma de cobro lineal 
                           #region if lineal 
                           if (selectforma == false)
                           {
                               var selecthoraen = (from hour in context.banco_parking
                                                   where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                   select hour.HoraEntrada).SingleOrDefault();
                               var horaen = selecthoraen;
                               TimeSpan horaentra = new TimeSpan();
                               horaentra = TimeSpan.Parse(horaentra.ToString());
                               TimeSpan diferenciaHoras = new TimeSpan();
                               string inicio = string.Format("{0:hh\\:mm\\:ss}", a.HoraEntrada);
                               string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

                               TimeSpan ini = TimeSpan.Parse(inicio);
                               TimeSpan sal = TimeSpan.Parse(salida);


                               diferenciaHoras = sal - ini;
                               lblTiempoParqueo.Text = diferenciaHoras.ToString();
                               int horas = diferenciaHoras.Hours;
                               double horasTotales = diferenciaHoras.TotalHours;

                               var selectcobro = (from cobrando in context.cobro
                                                  from bp in context.banco_parking
                                                  where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                  select cobrando.CobroLineal_Importe).FirstOrDefault();

                               var importe = selectcobro;
                               //costo para una hora con servicio adicional o tiempo libre
                               int costo = Convert.ToInt32(importe);
                               #region if
                               if (horasTotales < 1)
                               {
                                   lblParqueo.Text = importe.ToString();

                                   string placas = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();

                                   using (var contexts = new PARKING_DBEntities(CadenaConexion))
                                   {
                                       var selectservicio = (from servi in context.serviciosadicionales
                                                             from bp in context.banco_parking
                                                             where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                             select servi.Precio_ServiciosAd).FirstOrDefault();

                                       var selectpensionu = (from pensionu in context.pensiones_unicas
                                                             from bp in context.banco_parking
                                                             where pensionu.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                             select pensionu.Precio_PensionU).FirstOrDefault();
                                       var servicio = selectservicio;
                                       lblServicios.Text = servicio.ToString();
                                       costo += Convert.ToInt32(servicio);

                                       var pensi = selectpensionu;
                                       lblTiempoLibre.Text = pensi.ToString();
                                       lblTotal.Text = costo.ToString();
                                   }
                               }
                               #endregion

                               #region else
                               else
                               {
                                   double fraccion = horasTotales - 1;
                                   int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                                   fraccionados = fraccionados * 4;

                                   if (((fraccion % 1) / 100 * 60) >= .1 & (((fraccion % 1) / 100 * 60) <= .15))
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
                                   costo = costo + ((fraccionados) * costo / 4);

                                   lblParqueo.Text = costo.ToString();

                                   string placas = dtParking.Rows[dtParking.CurrentRow.Index].Cells[1].Value.ToString();

                                   using (var contexts = new PARKING_DBEntities(CadenaConexion))
                                   {
                                       var selectservicio = (from servi in context.serviciosadicionales
                                                             from bp in context.banco_parking
                                                             where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placas && bp.Estancia == "EN PROGRESO"
                                                             select servi.Precio_ServiciosAd).FirstOrDefault();

                                       var selectpensionu = (from pensionu in context.pensiones_unicas
                                                             from bp in context.banco_parking
                                                             where pensionu.ID_PensionU == bp.ID_PensionU && bp.Placa == placas && bp.Estancia == "EN PROGRESO"
                                                             select pensionu.Precio_PensionU).FirstOrDefault();
                                       var servicio = selectservicio;

                                       lblServicios.Text = servicio.ToString();
                                       costo += Convert.ToInt32(servicio);

                                       var pensi = selectpensionu;
                                       costo += Convert.ToInt32(pensi);
                                       lblTiempoLibre.Text = pensi.ToString();
                                       lblTotal.Text = costo.ToString();

                                   }
                               }
                           }
                           #endregion
                           #endregion
                           //forma de cobro tabulador 
                           #region if tabulador
                           if (selectforma == true)
                           {
                               var selecthoraen = (from hour in context.banco_parking
                                                   where hour.Placa == placa && hour.Estancia == "EN PROGRESO"
                                                   select hour.HoraEntrada).SingleOrDefault();
                               var horaen = selecthoraen;

                               var selectid = (from id in context.cobro
                                               from bp in context.banco_parking
                                               where id.ID_Tabulador == bp.ID_Cobro && bp.Estancia == "EN PROGRESO" && bp.Placa == placa
                                               select id.ID_Tabulador).SingleOrDefault();

                               TimeSpan horaentra = new TimeSpan();
                               horaentra = TimeSpan.Parse(horaentra.ToString());
                               TimeSpan diferenciaHoras = new TimeSpan();
                               string inicio = string.Format("{0:hh\\:mm\\:ss}", a.HoraEntrada);
                               string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

                               TimeSpan ini = TimeSpan.Parse(inicio);
                               TimeSpan sal = TimeSpan.Parse(salida);


                               diferenciaHoras = sal - ini;
                               lblTiempoParqueo.Text = diferenciaHoras.ToString();
                               int horas = diferenciaHoras.Hours;
                               double horasTotales = diferenciaHoras.TotalHours;


                               int fraccionados = Convert.ToInt32(horasTotales - (horasTotales % 1));
                               fraccionados = fraccionados * 2;
                               if (((horasTotales % 1) / 100 * 60) > 0 & (((horasTotales % 1) / 100 * 60) <= .299))
                               {
                                   fraccionados = fraccionados + 1;

                               }
                               if (((horasTotales % 1) / 100 * 60) >= .30 & (((horasTotales % 1) / 100 * 60) <= .59))
                               {
                                   fraccionados = fraccionados + 2;

                               }
                               string valorcolumna = ("min_" + ((fraccionados * 30).ToString()));

                               var query = context.cobro.ToList();

                               var resultado = query
                                   .Where(p => p.ID_Tabulador == selectid)
                                   .Select("(" + valorcolumna + ")");
                               int precioparqueo = 0;
                               foreach (var cbcb in resultado)
                               {
                                   lblTotal.Text = cbcb.ToString();
                                   lblParqueo.Text = cbcb.ToString();
                                   precioparqueo = Convert.ToInt32(cbcb);
                               }
                               var selectservicio = (from servi in context.serviciosadicionales
                                                     from bp in context.banco_parking
                                                     where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                                     select servi.Precio_ServiciosAd).FirstOrDefault();
                               var servicio = selectservicio;

                               lblServicios.Text = servicio.ToString();


                               precioparqueo = precioparqueo + Convert.ToInt32(servicio);
                               lblTotal.Text = precioparqueo.ToString();

                           }
                           #endregion
                       }
                   }
                   #endregion

                   #region if pension 1
                   if (pension == 1 && a.Placa == placa && tipocobroconvenio == true)
                   {
                       var Selectnombre = (from tipop in context.pensiones_unicas
                                           from bp in context.banco_parking
                                           where tipop.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                           select tipop.Nombre_PensionU).SingleOrDefault();

                       var nombre = Selectnombre;

                       var selectprecio = (from preciop in context.pensiones_unicas
                                           where preciop.Nombre_PensionU == nombre
                                           select preciop.Precio_PensionU).SingleOrDefault();

                       var iniciopension = (from iniciop in context.tipos_pensionesu
                                            where iniciop.ID_Tipo_PensionU == pension
                                            select iniciop.HoraInicio).SingleOrDefault();

                       var finpension = (from finp in context.tipos_pensionesu
                                         where finp.ID_Tipo_PensionU == pension
                                         select finp.HoraFin).SingleOrDefault();

                       var tiempo = finpension - iniciopension;

                       var precio = selectprecio;

                       var entrada = (from entra in context.banco_parking
                                      where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                      select entra.HoraEntrada).SingleOrDefault();

                       string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

                       TimeSpan sal = TimeSpan.Parse(salida);

                       var tiempoparking = sal - entrada;
                       lblTiempoParqueo.Text = tiempoparking.ToString();
                       lblTiempoLibre.Text = precio.ToString();
                       #region pension dentro del tiempo

                       var selectservicio = (from servi in context.serviciosadicionales
                                             from bp in context.banco_parking
                                             where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                             select servi.Precio_ServiciosAd).FirstOrDefault();
                       var servicio = selectservicio;

                       if (tiempoparking <= tiempo)
                       {

                           lblParqueo.Text = (0.00).ToString();
                           lblServicios.Text = servicio.ToString();
                           lblTotal.Text = (Convert.ToInt32(precio) + Convert.ToInt32(servicio)).ToString();

                       }
                       #endregion

                       #region fuera de tiempo
                       else
                       {
                           var tfuera = tiempoparking - tiempo;
                           TimeSpan ts = TimeSpan.Parse(string.Format("{0:hh\\:mm\\:ss}", tfuera));
                           int dif = ts.Hours;
                           double fraccion = ts.TotalHours;

                           var selectcobro = (from cobrando in context.cobro
                                              from bp in context.banco_parking
                                              where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa
                                              select cobrando.CobroLineal_Importe).FirstOrDefault();

                           var importe = selectcobro;
                           //costo para una hora con servicio adicional o tiempo libre
                           int costo = Convert.ToInt32(importe);



                           int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                           fraccionados = fraccionados * 4;

                           if (((fraccion % 1) / 100 * 60) >= .1 & (((fraccion % 1) / 100 * 60) <= .15))
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
                           int totalprice = ((fraccionados) * costo / 4);

                           lblServicios.Text = servicio.ToString();
                           lblParqueo.Text = totalprice.ToString();
                           lblTotal.Text = (totalprice + Convert.ToInt32(servicio) + Convert.ToInt32(precio)).ToString();
                       }
                       #endregion
                   }
                   #endregion

                   #region if pension = 2
                   if (pension == 2 && a.Placa == placa && tipocobroconvenio == true)
                   {

                       var Selectnombre = (from tipop in context.pensiones_unicas
                                           from bp in context.banco_parking
                                           where tipop.ID_PensionU == bp.ID_PensionU && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                           select tipop.Nombre_PensionU).SingleOrDefault();

                       var nombre = Selectnombre;

                       var selectprecio = (from preciop in context.pensiones_unicas
                                           where preciop.Nombre_PensionU == nombre
                                           select preciop.Precio_PensionU).SingleOrDefault();

                       var iniciopension = (from iniciop in context.tipos_pensionesu
                                            where iniciop.ID_Tipo_PensionU == pension
                                            select iniciop.HoraInicio).SingleOrDefault();

                       var finpension = (from finp in context.tipos_pensionesu
                                         where finp.ID_Tipo_PensionU == pension
                                         select finp.HoraFin).SingleOrDefault();

                       var tiempo = finpension - iniciopension;

                       var precio = selectprecio;

                       var entrada = (from entra in context.banco_parking
                                      where entra.Placa == placa && entra.Estancia == "EN PROGRESO"
                                      select entra.HoraEntrada).SingleOrDefault();

                       string salida = DateTime.Now.ToString("HH:mm:ss", new System.Globalization.CultureInfo("en-US"));

                       TimeSpan sal = TimeSpan.Parse(salida);

                       var tiempoparking = sal - entrada;
                       lblTiempoParqueo.Text = tiempoparking.ToString();
                       lblTiempoLibre.Text = precio.ToString();

                       var selectservicio = (from servi in context.serviciosadicionales
                                             from bp in context.banco_parking
                                             where servi.ID_ServicioAd == bp.ID_ServicioAd && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                             select servi.Precio_ServiciosAd).FirstOrDefault();

                       var servicio = selectservicio;


                       if (tiempoparking <= tiempo)
                       {
                           //dentro de tiempo
                           lblParqueo.Text = (0.00).ToString();
                           lblTiempoLibre.Text = precio.ToString();
                           lblServicios.Text = servicio.ToString();
                           lblTotal.Text = (Convert.ToInt32(precio) + Convert.ToInt32(servicio)).ToString();
                       }
                       else
                       {
                           var tfuera = tiempoparking - tiempo;
                           TimeSpan ts = TimeSpan.Parse(string.Format("{0:hh\\:mm\\:ss}", tfuera));
                           int dif = ts.Hours;
                           double fraccion = ts.TotalHours;

                           var selectcobro = (from cobrando in context.cobro
                                              from bp in context.banco_parking
                                              where cobrando.ID_Tabulador == a.ID_Cobro && bp.Placa == placa && bp.Estancia == "EN PROGRESO"
                                              select cobrando.CobroLineal_Importe).FirstOrDefault();

                           var importe = selectcobro;
                           //costo para una hora con servicio adicional o tiempo libre
                           int costo = Convert.ToInt32(importe);

                           int fraccionados = Convert.ToInt32(fraccion - (fraccion % 1));
                           fraccionados = fraccionados * 4;

                           if (((fraccion % 1) / 100 * 60) >= .1 & (((fraccion % 1) / 100 * 60) <= .15))
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
                           int totalprice = ((fraccionados) * costo / 4);

                           lblParqueo.Text = totalprice.ToString();
                           lblServicios.Text = servicio.ToString();
                           lblTiempoLibre.Text = precio.ToString();
                           lblTotal.Text = (Convert.ToInt32(servicio) + Convert.ToInt32(precio) + totalprice).ToString();
                       }

                   }
                   #endregion
               }
           }
           */









    }
}
