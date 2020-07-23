using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class Configuraciones : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;

        string mensajeError = "Error Inesperado Contacte al Administrador!";
        private string Conexion, Usuario;
        private int IDCobro, IDPensionUnica,IDPension,IDServicioAdicional,ID_ServidorSMTP;
        char separadorDecimal = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        #endregion

        #region ConfiguracionGeneral
        private void CargarConfiguracionGeneral()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tConfGen = (from cnfg in context.configgen1
                                where cnfg.ID_config == 1
                                select new Modelos.ModeloConfiguracionGeneral
                                {
                                    Capacidad = cnfg.Capacidad,
                                    Multa = cnfg.Multa,
                                    BuscarVehiculo = cnfg.Buscar_vehiculo,
                                    Limpiar = cnfg.Limpiar,
                                    ModoCobro = cnfg.Modo_cobro,
                                    PeriodoCobro = cnfg.PeriodoCobro,
                                    TCancel = cnfg.TCancel,
                                    Tcortesia = cnfg.Tcortesia,
                                    TicketCancelado = cnfg.Ticket_cancel,
                                    TicketCortesia = cnfg.Ticket_cortesia,
                                    TicketPerdido = cnfg.Ticker_perdido,
                                    ToleranciaIngreso = cnfg.Tolerancia_ingreso,
                                    ToleranciaProxTarifa = cnfg.Tolerancia_proxtarifa,
                                    TPerdido = cnfg.Tperdido,
                                    DireccionFactura=cnfg.DireccionFactura


                                }).ToList();
                foreach (var item in tConfGen)
                {
                    dmCapacidad.Value = item.Capacidad;
                    dmMulta.Value = item.Multa;

                    dmToleranciaTicketCancelado.Value = item.TCancel;
                    dmToleranciaTicketCortesia.Value = item.Tcortesia;

                    dmToleranciaIngreso.Value = item.ToleranciaIngreso;
                    dmToleranciaProxTarifa.Value = item.ToleranciaProxTarifa;
                    dmToleranciaTicketPerdido.Value = item.TPerdido;

                    radio1Hora.Checked = (item.PeriodoCobro == true) ? true : false;
                    radioMediaHora.Checked = (item.PeriodoCobro == false) ? true : false;
                    ckTicketCancelado.Checked = (item.TicketCancelado==true) ? true : false;
                    ckTicketCortesia.Checked = (item.TicketCortesia==true) ? true : false;
                    ckTicketPerdido.Checked = (item.TicketPerdido==true) ? true : false;
                    radioFolio.Checked = (item.BuscarVehiculo == false) ? true : false;
                    radioPlaca.Checked = (item.BuscarVehiculo == true) ? true : false;
                    txtUrlFactura.Text = item.DireccionFactura;

                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    diccionario.Add("0", "Lector");
                    diccionario.Add("1", "Manual y Lector");
                    scModoCobro.DataSource = new BindingSource(diccionario, null);
                    scModoCobro.DisplayMember = "Value";
                    scModoCobro.ValueMember = "Key";                
                    scModoCobro.SelectedIndex = (item.ModoCobro==true) ?1:0;
                    Dictionary<string, string> diccionario2 = new Dictionary<string, string>();
                    diccionario2.Add("0", "Nunca");
                    diccionario2.Add("1", "Semanal");
                    diccionario2.Add("2", "Quincenal");
                    diccionario2.Add("3", "Mensual");
                    diccionario2.Add("4", "2 Meses");
                    diccionario2.Add("5", "3 Meses");
                    scLimpiarHistorial.DataSource = new BindingSource(diccionario2, null);
                    scLimpiarHistorial.DisplayMember = "Value";
                    scLimpiarHistorial.ValueMember = "Key";
                    switch (item.Limpiar)
                    {
                        case 0:
                            scLimpiarHistorial.SelectedIndex = 0;
                            break;
                        case 1:
                            scLimpiarHistorial.SelectedIndex = 1;
                            break;
                        case 2:
                            scLimpiarHistorial.SelectedIndex = 2;
                            break;
                        case 3:
                            scLimpiarHistorial.SelectedIndex = 3;
                            break;
                        case 4:
                            scLimpiarHistorial.SelectedIndex = 4;
                            break;
                        case 5:
                            scLimpiarHistorial.SelectedIndex = 5;
                            break;
                    }
                }

            }


        }
        private void btnGuardarCambiosGenerales_Click(object sender, EventArgs e)
        {
            ActualizarCambiosGenerales();
        }

        private void ActualizarCambiosGenerales()
        {
            try
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.configgen1.SingleOrDefault(n => n.ID_config == 1);
                    if (a != null)
                    {

                        a.Capacidad = Convert.ToInt32(dmCapacidad.Value);
                        a.Multa = Convert.ToInt32(dmMulta.Value);
                        a.TCancel = Convert.ToInt32(dmToleranciaTicketCancelado.Value);
                        a.Tcortesia = Convert.ToInt32(dmToleranciaTicketCortesia.Value);
                        a.Tolerancia_ingreso = Convert.ToInt32(dmToleranciaIngreso.Value);
                        a.Tolerancia_proxtarifa = Convert.ToInt32(dmToleranciaProxTarifa.Value);
                        a.Tperdido = Convert.ToInt32(dmToleranciaTicketPerdido.Value);
                        a.PeriodoCobro = (radio1Hora.Checked == true) ? true : false;
                        a.Ticket_cancel = (ckTicketCancelado.Checked == true) ? true : false;
                        a.Ticket_cortesia = (ckTicketCortesia.Checked == true) ? true : false;
                        a.Ticker_perdido = (ckTicketPerdido.Checked == true) ? true : false;
                        a.Buscar_vehiculo = (radioPlaca.Checked == true) ? true : false;
                        a.Modo_cobro = (scModoCobro.SelectedIndex == 1) ? true : false;
                        a.DireccionFactura = txtUrlFactura.Text.Trim();
                        switch (scLimpiarHistorial.SelectedIndex)
                        {
                            case 0:
                                a.Limpiar = 0;
                                break;
                            case 1:
                                a.Limpiar = 1;
                                break;
                            case 2:
                                a.Limpiar = 2;
                                break;
                            case 3:
                                a.Limpiar = 3;
                                break;
                            case 4:
                                a.Limpiar = 4;
                                break;
                            case 5:
                                a.Limpiar = 5;
                                break;

                        }



                        context.SaveChanges();
                        CargarConfiguracionGeneral();
                        MessageBox.Show("Configuracion General Actualizada");
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

        #endregion

        #region ConfiguracionTicket

        private void CargarConfiguracionTicket()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = (from conftick in context.config_tickets
                         where conftick.ID_ticket == 1
                         select new Modelos.ModeloConfiguracionTicket
                         {
                             DatosCancelado = conftick.DatosCancelado,
                             DatosConvenio = conftick.DatosConvenio,
                             DatosCortesia = conftick.DatosCortesia,
                             DatosEntrada = conftick.DatosEntrada,
                             DatosPension = conftick.DatosPension,
                             DatosPerdido = conftick.DatosPerdido,
                             DatosSalida = conftick.DatosSalida,
                             Direccion = conftick.Direccion,
                             Estacionamiento = conftick.Estacionamiento,
                             Telefono = conftick.Telefono,
                             RFC = conftick.RFC,
                             RazonSocial = conftick.Razon_Social,
                             IncluirLogo = conftick.Incluir_Logo,
                             TamañoPapel = conftick.Tamaño_papel,                        
                             CantidadTicketEntrada = conftick.Cantidad_Copias_Entrada,
                             CantidadTicketSalida = conftick.Cantidad_Copias_Salida,
                             Imagen=conftick.Imagen,
                             Desarrollador=conftick.Desarrollador
                             
                         }).ToList();

                foreach (var item in a)
                {
                    if (item.Imagen != null && item.Imagen.Length > 0)
                    {
                        Bitmap bmp;
                        using (var ms = new MemoryStream(item.Imagen))
                        {
                            bmp = new Bitmap(ms);
                            pbLogo.Image = bmp;
                        }
                    }
                    else
                    {
                        pbLogo.Image = Properties.Resources.logo4parking;
                    }
                    txtDesarrollador.Text = item.Desarrollador;
                    txtEstacionamiento.Text = item.Estacionamiento;
                    txtDireccion.Text = item.Direccion;
                    txtRazonSocial.Text = item.RazonSocial;
                    txtTelefono.Text = item.Telefono;
                    txtRFC.Text = item.RFC;
                    txtTicket1.Text = item.DatosEntrada;
                    txtTicket2.Text = item.DatosSalida;
                    txtTicket3.Text = item.DatosPerdido;
                    txtTicket4.Text = item.DatosCortesia;
                    txtTicket5.Text = item.DatosConvenio;
                    txtTicket6.Text = item.DatosCancelado;
                    txtTicket7.Text = item.DatosPension;
                    radio80.Checked = (item.TamañoPapel == true) ? true : false;
                    radio58.Checked = (item.TamañoPapel == false) ? true : false;
                    dmComprobaneEntrada.Value = item.CantidadTicketEntrada;
                    dmComprobanteSalida.Value = item.CantidadTicketSalida;
                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    diccionario.Add("0", "Ticket Entrada");
                    diccionario.Add("1", "Ticket Salida");
                    diccionario.Add("2", "Ticket Perdido");
                    diccionario.Add("3", "Ticket Cortesia");
                    diccionario.Add("4", "Ticket Convenio");
                    diccionario.Add("5", "Ticket Cancelado");
                    diccionario.Add("6", "Ticket Pension");
                    scTicket.DataSource = new BindingSource(diccionario, null);
                    scTicket.DisplayMember = "Value";
                    scTicket.ValueMember = "Key";
                    scTicket.SelectedIndex = 0;


                }


            }


        }


        private void scTicket_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTicket1.Visible = (scTicket.SelectedIndex == 0) ? true : false;
            txtTicket2.Visible = (scTicket.SelectedIndex == 1) ? true : false;
            txtTicket3.Visible = (scTicket.SelectedIndex == 2) ? true : false;
            txtTicket4.Visible = (scTicket.SelectedIndex == 3) ? true : false;
            txtTicket5.Visible = (scTicket.SelectedIndex == 4) ? true : false;
            txtTicket6.Visible = (scTicket.SelectedIndex == 5) ? true : false;
            txtTicket7.Visible = (scTicket.SelectedIndex == 6) ? true : false;
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            try {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var img = new Bitmap(openFileDialog1.FileName);
                    ImageConverter converter = new ImageConverter();
                    var byt = (byte[])converter.ConvertTo(img, typeof(byte[]));
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var a = context.config_tickets.SingleOrDefault(n => n.ID_ticket == 1);
                        if (a != null)
                        {
                            a.Imagen = byt;
                            context.SaveChanges();

                            pbLogo.Image = new Bitmap(img);

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
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.config_tickets.SingleOrDefault(n => n.ID_ticket == 1);
                    if (a != null)
                    {
                        a.Imagen = null;
                        context.SaveChanges();
                        pbLogo.Image = Properties.Resources.logo4parking;
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
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.config_tickets.SingleOrDefault(n => n.ID_ticket == 1);
                if (a != null)
                {

                    a.Desarrollador = txtDesarrollador.Text.Trim();
                    a.Estacionamiento=txtEstacionamiento.Text;
                    a.Direccion = txtDireccion.Text;
                    a.Razon_Social = txtRazonSocial.Text;
                    a.Telefono = txtTelefono.Text;
                    a.RFC= txtRFC.Text;
                    a.DatosEntrada=txtTicket1.Text;
                    a.DatosSalida =txtTicket2.Text;
                    a.DatosPerdido=txtTicket3.Text;
                    a.DatosCortesia= txtTicket4.Text;
                    a.DatosConvenio= txtTicket5.Text;
                    a.DatosCancelado= txtTicket6.Text;
                    a.DatosPension= txtTicket7.Text;
                    a.Tamaño_papel = (radio80.Checked == true) ? true : false;
                    a.Cantidad_Copias_Entrada= Convert.ToInt32(dmComprobaneEntrada.Value);
                    a.Cantidad_Copias_Salida= Convert.ToInt32(dmComprobanteSalida.Value);

                    context.SaveChanges();
                    CargarConfiguracionTicket();
                    MessageBox.Show("Configuracion Tickets Actualizada");
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


        #endregion

        #region ConfiguracionCobro
        private void CargarConfiguracionCobro()
        {
            CargarPanelRojo();
        }
        private void CargarTodo(int id)
        {


            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.cobro.Where(n => n.ID_Tabulador == id).ToList();
                foreach (var item in a)
                {
                    radioSeleccionadoLineal.Checked = (item.FormaCobro == false) ? true : false;
                    radioSeleccionadoTabulador.Checked = (item.FormaCobro == true) ? true : false;
                    ckMetodoCobro.Checked = (item.Tarifa_Habilitada == true) ? true : false;
                    txtImporteDefault.Text = item.CobroLineal_ImporteDefault.ToString();
                    txtImporteLineal.Text = item.CobroLineal_Importe.ToString();
                    dmTiempoDefault.Value = Convert.ToDecimal(item.CobroLineal_minDefault);
                 
                    txt30min.Text = item.min_30.ToString();
                    txt1hr.Text = item.min_60.ToString();
                    txt1h30m.Text = item.min_90.ToString();
                    txt2h.Text = item.min_120.ToString();
                    txt2h30m.Text = item.min_150.ToString();
                    txt3h.Text = item.min_180.ToString();
                    txt3h30m.Text = item.min_210.ToString();
                    txt4h.Text = item.min_240.ToString();
                    txt4h30m.Text = item.min_270.ToString();
                    txt5h.Text = item.min_300.ToString();
                    txt5h30m.Text = item.min_330.ToString();
                    txt6h.Text = item.min_360.ToString();
                    txt6h30m.Text = item.min_390.ToString();
                    txt7h.Text = item.min_420.ToString();
                    txt7h30m.Text = item.min_450.ToString();
                    txt8h.Text = item.min_480.ToString();
                    txt8h30m.Text = item.min_510.ToString();
                    txt9h.Text = item.min_540.ToString();
                    txt9h30m.Text = item.min_570.ToString();
                    txt10h.Text = item.min_600.ToString();
                    txt10h30m.Text = item.min_630.ToString();
                    txt11h.Text = item.min_660.ToString();
                    txt11h30m.Text = item.min_690.ToString();
                    txt12h.Text = item.min_720.ToString();
                    txt12h30m.Text = item.min_750.ToString();
                    txt13h.Text = item.min_780.ToString();
                    txt13h30m.Text = item.min_810.ToString();
                    txt14h.Text = item.min_840.ToString();
                    txt14h30m.Text = item.min_870.ToString();
                    txt15h.Text = item.min_900.ToString();
                    txt15h30m.Text = item.min_930.ToString();
                    txt16h.Text = item.min_960.ToString();
                    txt16h30m.Text = item.min_990.ToString();
                    txt17h.Text = item.min_1020.ToString();
                    txt17h30m.Text = item.min_1050.ToString();
                    txt18h.Text = item.min_1080.ToString();
                    txt18h30m.Text = item.min_1110.ToString();
                    txt19h.Text = item.min_1140.ToString();
                    txt19h30m.Text = item.min_1170.ToString();
                    txt20h.Text = item.min_1200.ToString();
                    txt20h30m.Text = item.min_1230.ToString();
                    txt21h.Text = item.min_1260.ToString();
                    txt21h30m.Text = item.min_1290.ToString();
                    txt22h.Text = item.min_1320.ToString();
                    txt22h30m.Text = item.min_1350.ToString();
                    txt23h.Text = item.min_1380.ToString();
                    txt23h30m.Text = item.min_1410.ToString();
                    txt24h.Text = item.min_1440.ToString();



                }

            }



        }

        private void CargarPanelRojo()
        {
            if (scTarifas.Items.Count > 0)
            {
                scTarifas.DataSource = null;
                scTarifas.Items.Clear();


            }


            if (radioOpcionTarifas.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    var item = context.cobro.Where(n => n.TipoCobro == true).ToList();
                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre;
                        var id = valores.ID_Tabulador;
                        diccionario.Add(id.ToString(), nombre);
                    }
                    scTarifas.DataSource = new BindingSource(diccionario, null);
                    scTarifas.DisplayMember = "Value";
                    scTarifas.ValueMember = "Key";
                }
                if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }
            }
            if (radioOpcionConvenios.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    var item = context.cobro.Where(n => n.TipoCobro == false).ToList();
                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre;
                        var id = valores.ID_Tabulador;
                        diccionario.Add(id.ToString(), nombre);
                    }
                    scTarifas.DataSource = new BindingSource(diccionario, null);
                    scTarifas.DisplayMember = "Value";
                    scTarifas.ValueMember = "Key";
                }
                if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }
            }
        }



        private void radioOpcionTarifas_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                Dictionary<string, string> diccionario = new Dictionary<string, string>();
                var item = context.cobro.Where(n => n.TipoCobro == true).ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Nombre;
                    var id = valores.ID_Tabulador;
                    diccionario.Add(id.ToString(), nombre);
                }
                scTarifas.DataSource = new BindingSource(diccionario, null);
                scTarifas.DisplayMember = "Value";
                scTarifas.ValueMember = "Key";
            }
            if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }
        }

        private void radioOpcionConvenios_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                Dictionary<string, string> diccionario = new Dictionary<string, string>();
                var item = context.cobro.Where(n => n.TipoCobro == false).ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Nombre;
                    var id = valores.ID_Tabulador;
                    diccionario.Add(id.ToString(), nombre);
                }
                scTarifas.DataSource = new BindingSource(diccionario, null);
                scTarifas.DisplayMember = "Value";
                scTarifas.ValueMember = "Key";
            }
            if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }
        }

        private void scTarifas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var a = ((KeyValuePair<string, string>)scTarifas.SelectedItem).Key;
            CargarTodo(Convert.ToInt32(a));
            IDCobro = Int32.Parse((((KeyValuePair<string, string>)scTarifas.SelectedItem).Key));
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
            if (string.IsNullOrEmpty(txtNombreTarifa.Text))
            {
                MessageBox.Show("Coloque un Nombre");

            }
            else
            {
                CrearNuevoPlan();
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

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.cobro.SingleOrDefault(n => n.ID_Tabulador == IDCobro);
                    if (a != null)
                    {
                        a.Tarifa_Habilitada = (ckMetodoCobro.Checked == true) ? true : false;
                        a.FormaCobro = (radioSeleccionadoTabulador.Checked == true) ? true : false;
                        a.CobroLineal_Importe = (Cultura() == ",") ? Convert.ToDecimal(txtImporteLineal.Text.Replace('.', ',')) : Convert.ToDecimal(txtImporteLineal.Text);
                        a.CobroLineal_minDefault = Convert.ToInt32(dmTiempoDefault.Value);
                        a.CobroLineal_ImporteDefault = (Cultura() == ",") ? Convert.ToDecimal(txtImporteDefault.Text.Replace('.', ',')) : Convert.ToDecimal(txtImporteDefault.Text);
                        a.min_30 =(Cultura() == ",") ? Convert.ToDecimal(txt30min.Text.Replace('.', ',')): Convert.ToDecimal(txt30min.Text);
                        a.min_60 = (Cultura() == ",") ? Convert.ToDecimal(txt1hr.Text.Replace('.', ',')): Convert.ToDecimal(txt1hr.Text);
                        a.min_90 = (Cultura() == ",") ? Convert.ToDecimal(txt1h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt1h30m.Text);
                        a.min_120 = (Cultura() == ",") ? Convert.ToDecimal(txt2h.Text.Replace('.', ',')): Convert.ToDecimal(txt2h.Text);
                        a.min_150 = (Cultura() == ",") ? Convert.ToDecimal(txt2h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt2h30m.Text);
                        a.min_180 = (Cultura() == ",") ? Convert.ToDecimal(txt3h.Text.Replace('.', ',')): Convert.ToDecimal(txt3h.Text);
                        a.min_210 = (Cultura() == ",") ? Convert.ToDecimal(txt3h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt3h30m.Text);
                        a.min_240 = (Cultura() == ",") ? Convert.ToDecimal(txt4h.Text.Replace('.', ',')): Convert.ToDecimal(txt4h.Text);
                        a.min_270 = (Cultura() == ",") ? Convert.ToDecimal(txt4h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt4h30m.Text);
                        a.min_300 = (Cultura() == ",") ? Convert.ToDecimal(txt5h.Text.Replace('.', ',')): Convert.ToDecimal(txt5h.Text);
                        a.min_330 = (Cultura() == ",") ? Convert.ToDecimal(txt5h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt5h30m.Text);
                        a.min_360 = (Cultura() == ",") ? Convert.ToDecimal(txt6h.Text.Replace('.', ',')): Convert.ToDecimal(txt6h.Text);
                        a.min_390 = (Cultura() == ",") ? Convert.ToDecimal(txt6h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt6h30m.Text);
                        a.min_420 = (Cultura() == ",") ? Convert.ToDecimal(txt7h.Text.Replace('.', ',')): Convert.ToDecimal(txt7h.Text);
                        a.min_450 = (Cultura() == ",") ? Convert.ToDecimal(txt7h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt7h30m.Text);
                        a.min_480 = (Cultura() == ",") ? Convert.ToDecimal(txt8h.Text.Replace('.', ',')): Convert.ToDecimal(txt8h.Text);
                        a.min_510 = (Cultura() == ",") ? Convert.ToDecimal(txt8h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt8h30m.Text);
                        a.min_540 = (Cultura() == ",") ? Convert.ToDecimal(txt9h.Text.Replace('.', ',')): Convert.ToDecimal(txt9h.Text);
                        a.min_570 = (Cultura() == ",") ? Convert.ToDecimal(txt9h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt9h30m.Text);
                        a.min_600 = (Cultura() == ",") ? Convert.ToDecimal(txt10h.Text.Replace('.', ',')): Convert.ToDecimal(txt10h.Text);
                        a.min_630 = (Cultura() == ",") ? Convert.ToDecimal(txt10h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt10h30m.Text);
                        a.min_660 = (Cultura() == ",") ? Convert.ToDecimal(txt11h.Text.Replace('.', ',')): Convert.ToDecimal(txt11h.Text);
                        a.min_690 = (Cultura() == ",") ? Convert.ToDecimal(txt11h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt11h30m.Text);
                        a.min_720 = (Cultura() == ",") ? Convert.ToDecimal(txt12h.Text.Replace('.', ',')): Convert.ToDecimal(txt12h.Text);
                        a.min_750 = (Cultura() == ",") ? Convert.ToDecimal(txt12h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt12h30m.Text);
                        a.min_780 = (Cultura() == ",") ? Convert.ToDecimal(txt13h.Text.Replace('.', ',')): Convert.ToDecimal(txt13h.Text);
                        a.min_810 = (Cultura() == ",") ? Convert.ToDecimal(txt13h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt13h30m.Text);
                        a.min_840 = (Cultura() == ",") ? Convert.ToDecimal(txt14h.Text.Replace('.', ',')): Convert.ToDecimal(txt14h.Text);
                        a.min_870 = (Cultura() == ",") ? Convert.ToDecimal(txt14h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt14h30m.Text);
                        a.min_900 = (Cultura() == ",") ? Convert.ToDecimal(txt15h.Text.Replace('.', ',')): Convert.ToDecimal(txt15h.Text);
                        a.min_930 = (Cultura() == ",") ? Convert.ToDecimal(txt15h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt15h30m.Text);
                        a.min_960 = (Cultura() == ",") ? Convert.ToDecimal(txt16h.Text.Replace('.', ',')): Convert.ToDecimal(txt16h.Text);
                        a.min_990 = (Cultura() == ",") ? Convert.ToDecimal(txt16h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt16h30m.Text);
                        a.min_1020 = (Cultura() == ",") ? Convert.ToDecimal(txt17h.Text.Replace('.', ',')): Convert.ToDecimal(txt17h.Text);
                        a.min_1050 = (Cultura() == ",") ? Convert.ToDecimal(txt17h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt17h30m.Text);
                        a.min_1080 = (Cultura() == ",") ? Convert.ToDecimal(txt18h.Text.Replace('.', ',')): Convert.ToDecimal(txt18h.Text);
                        a.min_1110 = (Cultura() == ",") ? Convert.ToDecimal(txt18h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt18h30m.Text);
                        a.min_1140 = (Cultura() == ",") ? Convert.ToDecimal(txt19h.Text.Replace('.', ',')): Convert.ToDecimal(txt19h.Text);
                        a.min_1170 = (Cultura() == ",") ? Convert.ToDecimal(txt19h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt19h30m.Text);
                        a.min_1200 = (Cultura() == ",") ? Convert.ToDecimal(txt20h.Text.Replace('.', ',')): Convert.ToDecimal(txt20h.Text);
                        a.min_1230 = (Cultura() == ",") ? Convert.ToDecimal(txt20h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt20h30m.Text);
                        a.min_1260 = (Cultura() == ",") ? Convert.ToDecimal(txt21h.Text.Replace('.', ',')): Convert.ToDecimal(txt21h.Text);
                        a.min_1290 = (Cultura() == ",") ? Convert.ToDecimal(txt21h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt21h30m.Text);
                        a.min_1320 = (Cultura() == ",") ? Convert.ToDecimal(txt22h.Text.Replace('.', ',')): Convert.ToDecimal(txt22h.Text);
                        a.min_1350 = (Cultura() == ",") ? Convert.ToDecimal(txt22h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt22h30m.Text);
                        a.min_1380 = (Cultura() == ",") ? Convert.ToDecimal(txt23h.Text.Replace('.', ',')): Convert.ToDecimal(txt23h.Text);
                        a.min_1410 = (Cultura() == ",") ? Convert.ToDecimal(txt23h30m.Text.Replace('.', ',')): Convert.ToDecimal(txt23h30m.Text);
                        a.min_1440 = (Cultura() == ",") ? Convert.ToDecimal(txt24h.Text.Replace('.', ',')): Convert.ToDecimal(txt24h.Text);

                        context.SaveChanges();
                        MessageBox.Show("El cobro de " + a.Nombre + " ha sido actualizado");
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

        private void CrearNuevoPlan()
        {
            decimal p = 0M;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = new cobro()
                {
                    Nombre = txtNombreTarifa.Text,
                    TipoCobro = (radioCrearTarifa.Checked == true) ? true : false,
                    FormaCobro = true,
                    Tarifa_Habilitada = false,
                    CobroLineal_ImporteDefault = p,
                    CobroLineal_Importe = p,
                    CobroLineal_minDefault = 0,
                    CobroLineal_minFrecuencia = 0,
                    min_30 = p,
                    min_60 = p,
                    min_90 = p,
                    min_120 = p,
                    min_150 = p,
                    min_180 = p,
                    min_210 = p,
                    min_240 = p,
                    min_270 = p,
                    min_300 = p,
                    min_330 = p,
                    min_360 = p,
                    min_390 = p,
                    min_420 = p,
                    min_450 = p,
                    min_480 = p,
                    min_510 = p,
                    min_540 = p,
                    min_570 = p,
                    min_600 = p,
                    min_630 = p,
                    min_660 = p,
                    min_690 = p,
                    min_720 = p,
                    min_750 = p,
                    min_780 = p,
                    min_810 = p,
                    min_840 = p,
                    min_870 = p,
                    min_900 = p,
                    min_930 = p,
                    min_960 = p,
                    min_990 = p,
                    min_1020 = p,
                    min_1050 = p,
                    min_1080 = p,
                    min_1110 = p,
                    min_1140 = p,
                    min_1170 = p,
                    min_1200 = p,
                    min_1230 = p,
                    min_1260 = p,
                    min_1290 = p,
                    min_1320 = p,
                    min_1350 = p,
                    min_1380 = p,
                    min_1410 = p,
                    min_1440 = p,


                };
                context.cobro.Add(a);
                context.SaveChanges();
                MessageBox.Show("Insercion Correcta!");
            }
            if (radioOpcionTarifas.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    var item = context.cobro.Where(n => n.TipoCobro == true).ToList();
                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre;
                        var id = valores.ID_Tabulador;
                        diccionario.Add(id.ToString(), nombre);
                    }
                    scTarifas.DataSource = new BindingSource(diccionario, null);
                    scTarifas.DisplayMember = "Value";
                    scTarifas.ValueMember = "Key";
                }
                if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }
            }
            if (radioOpcionConvenios.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    Dictionary<string, string> diccionario = new Dictionary<string, string>();
                    var item = context.cobro.Where(n => n.TipoCobro == false).ToList();
                    foreach (var valores in item)
                    {
                        var nombre = valores.Nombre;
                        var id = valores.ID_Tabulador;
                        diccionario.Add(id.ToString(), nombre);
                    }
                    scTarifas.DataSource = new BindingSource(diccionario, null);
                    scTarifas.DisplayMember = "Value";
                    scTarifas.ValueMember = "Key";
                }
                if (scTarifas.Items.Count > 0) { scTarifas.SelectedIndex = 0; }

            }


        }



        #endregion

        #region ConfiguracionPensionUnica
        private void CargarConfiguracionPensionUnica()
        {
            CargarDtPensionUnica();
            CargarPanelPensionUnica();

        }

        private void CargarDtPensionUnica()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbPensionesUnicas = (from pu in context.pensiones_unicas
                                         join tp in context.tipos_pensionesu on pu.ID_Tipo_PensionU equals tp.ID_Tipo_PensionU
                                         select new Modelos.ModeloDtPensionUnica
                                         {
                                             ID = pu.ID_PensionU,
                                             Nombre = pu.Nombre_PensionU,
                                             Edo = pu.PensionU_Activa.ToString(),
                                             Precio = pu.Precio_PensionU,
                                             Tipo = tp.Tipo_PensionU,
                                             HoraF = tp.HoraFin,
                                             HoraI = tp.HoraInicio,
                                             Tin = tp.Tolerancia_IN.ToString(),
                                             Tout = tp.Tolerancia_OUT.ToString()

                                         }).AsQueryable().OrderBy(n => n.Nombre);
                dtPensionUnica.DataSource = tbPensionesUnicas.ToList();


            }
        }

        private void CargarPanelPensionUnica()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.pensiones_unicas.ToList();


                foreach (var valores in item)
                {
                    var nombre = valores.Nombre_PensionU;
                    var id = valores.ID_PensionU;
                    diccionario.Add(id.ToString(), nombre);
                }
                scPensionUnica.DataSource = new BindingSource(diccionario, null);
                scPensionUnica.DisplayMember = "Value";
                scPensionUnica.ValueMember = "Key";
            }
            if (scPensionUnica.Items.Count > 0) { scPensionUnica.SelectedIndex = 0; }
            if (radioTipoPensionUnica.Checked == false && radioTipoPensionNocturna.Checked == false) { radioTipoPensionUnica.Checked = true; }


        }
        private void scPensionUnica_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDPensionUnica = Int32.Parse((((KeyValuePair<string, string>)scPensionUnica.SelectedItem).Key));
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbPensionesUnicas = (from pu in context.pensiones_unicas
                                         join tp in context.tipos_pensionesu on pu.ID_Tipo_PensionU equals tp.ID_Tipo_PensionU
                                         where (pu.ID_PensionU == IDPensionUnica)
                                         select new Modelos.ModeloConfiguracionPensionUnica
                                         {
                                             Nombre = pu.Nombre_PensionU,
                                             PensionActiva = pu.PensionU_Activa,
                                             Precio = pu.Precio_PensionU,
                                             Tarifa = pu.Precio_PensionU.ToString(),
                                             ToleranciaEntrada = tp.Tolerancia_IN.ToString(),
                                             ToleranciaSalida = tp.Tolerancia_OUT.ToString(),
                                             HoraInicio = tp.HoraInicio,
                                             HoraFin = tp.HoraFin,
                                             ID_Tipo_PensionU = tp.ID_Tipo_PensionU



                                         }).ToList();
                foreach (var item in tbPensionesUnicas)
                {
                    txtNombrePensionUnica.Text = item.Nombre;
                    txtTarifa.Text = item.Tarifa;
                    lblHorario.Text = item.Horario;
                    lblToleranciaEntrada.Text = item.ToleranciaEntrada;
                    lblToleranciaSalida.Text = item.ToleranciaSalida;
                    ckEstadoPensionUnica.Checked = (item.PensionActiva == true) ? true : false;
                    scTipoPensionUnica.SelectedIndex = (item.ID_Tipo_PensionU == 1) ? 0 : 1;

                }




            }




        }

        private void radioTipoPensionUnica_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTipoPensionUnica.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.tipos_pensionesu.Where(n => n.ID_Tipo_PensionU == 1).ToList();
                    foreach (var item in a)
                    {

                        dmHoraInicial.Value = Convert.ToDecimal(string.Format("{0:hh}", item.HoraInicio));
                        dmHoraFinal.Value = Convert.ToDecimal(string.Format("{0:hh}", item.HoraFin));
                        dmMinInicial.Value = Convert.ToDecimal(string.Format("{0:mm}", item.HoraInicio));
                        dmMinFinal.Value = Convert.ToDecimal(string.Format("{0:mm}", item.HoraFin));
                        dmToleranciaEntrada.Value = Convert.ToDecimal(item.Tolerancia_IN);
                        dmToleranciaSalida.Value = Convert.ToDecimal(item.Tolerancia_OUT);

                    }

                }
            }


        }

        private void radioTipoPensionNocturna_CheckedChanged(object sender, EventArgs e)
        {

            if (radioTipoPensionNocturna.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.tipos_pensionesu.Where(n => n.ID_Tipo_PensionU == 2).ToList();
                    foreach (var item in a)
                    {
                        dmHoraInicial.Value = Convert.ToDecimal(string.Format("{0:hh}", item.HoraInicio));
                        dmHoraFinal.Value = Convert.ToDecimal(string.Format("{0:hh}", item.HoraFin));
                        dmMinInicial.Value = Convert.ToDecimal(string.Format("{0:mm}", item.HoraInicio));
                        dmMinFinal.Value = Convert.ToDecimal(string.Format("{0:mm}", item.HoraFin));
                        dmToleranciaEntrada.Value = Convert.ToDecimal(item.Tolerancia_IN);
                        dmToleranciaSalida.Value = Convert.ToDecimal(item.Tolerancia_OUT);


                    }


                }
            }
        }
        private void btnActualizarTiposPensionesUnicas_Click(object sender, EventArgs e)
        {
            try
            {

            if (radioTipoPensionUnica.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.tipos_pensionesu.SingleOrDefault(n => n.ID_Tipo_PensionU == 1);
                    if (a != null)
                    {

                        a.HoraInicio = TimeSpan.Parse(dmHoraInicial.Value.ToString() + ":" + dmMinInicial.Value.ToString());
                        a.HoraFin = TimeSpan.Parse(dmHoraFinal.Value.ToString() + ":" + dmMinFinal.Value.ToString());
                        a.Tolerancia_IN = Convert.ToInt32(dmToleranciaEntrada.Value);
                        a.Tolerancia_OUT = Convert.ToInt32(dmToleranciaSalida.Value);

                        context.SaveChanges();
                        CargarDtPensionUnica();
                        CargarPanelPensionU();
                        MessageBox.Show("Turno Diurno Actualizado");
                    }

                }

            }
            if (radioTipoPensionNocturna.Checked == true)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {

                    var a = context.tipos_pensionesu.SingleOrDefault(n => n.ID_Tipo_PensionU == 2);
                    if (a != null)
                    {

                        a.HoraInicio = TimeSpan.Parse(dmHoraInicial.Value.ToString() + ":" + dmMinInicial.Value.ToString());
                        a.HoraFin = TimeSpan.Parse(dmHoraFinal.Value.ToString() + ":" + dmMinFinal.Value.ToString());
                        a.Tolerancia_IN = Convert.ToInt32(dmToleranciaEntrada.Value);
                        a.Tolerancia_OUT = Convert.ToInt32(dmToleranciaSalida.Value);

                        context.SaveChanges();
                        CargarDtPensionUnica();
                        CargarPanelPensionU();
                        MessageBox.Show("Turno Nocturno Actualizado");
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

        private void CargarPanelPensionU()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbPensionesUnicas = (from pu in context.pensiones_unicas
                                         join tp in context.tipos_pensionesu on pu.ID_Tipo_PensionU equals tp.ID_Tipo_PensionU
                                         where (pu.ID_PensionU == IDPensionUnica)
                                         select new Modelos.ModeloConfiguracionPensionUnica
                                         {
                                             Nombre = pu.Nombre_PensionU,
                                             PensionActiva = pu.PensionU_Activa,
                                             Precio = pu.Precio_PensionU,
                                             Tarifa = pu.Precio_PensionU.ToString(),
                                             ToleranciaEntrada = tp.Tolerancia_IN.ToString(),
                                             ToleranciaSalida = tp.Tolerancia_OUT.ToString(),
                                             HoraInicio = tp.HoraInicio,
                                             HoraFin = tp.HoraFin,
                                             ID_Tipo_PensionU = tp.ID_Tipo_PensionU
                                         }).ToList();
                foreach (var item in tbPensionesUnicas)
                {
                    txtNombrePensionUnica.Text = item.Nombre;
                    txtTarifa.Text = item.Tarifa;
                    lblHorario.Text = item.Horario;
                    lblToleranciaEntrada.Text = item.ToleranciaEntrada;
                    lblToleranciaSalida.Text = item.ToleranciaSalida;
                    ckEstadoPensionUnica.Checked = (item.PensionActiva == true) ? true : false;
                    scTipoPensionUnica.SelectedIndex = (item.ID_Tipo_PensionU == 1) ? 0 : 1;

                }




            }
        }

        private void btnActualizarPensionUnicas_Click(object sender, EventArgs e)
        {
            try
            {
            var selc = scPensionUnica.SelectedIndex;
            if (!string.IsNullOrEmpty(txtNombrePensionUnica.Text) || !string.IsNullOrEmpty(txtTarifa.Text))
            {
                string costoFormato = txtTarifa.Text.Trim();
                costoFormato =(Cultura()==",")? costoFormato.Replace('.', ','):costoFormato;
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.pensiones_unicas.SingleOrDefault(n => n.ID_PensionU == IDPensionUnica);
                    if (a != null)
                    {

                        a.Nombre_PensionU = txtNombrePensionUnica.Text.Trim();
                        a.Precio_PensionU = Convert.ToDecimal(costoFormato);
                        a.PensionU_Activa = (ckEstadoPensionUnica.Checked == true) ? true : false;
                        a.ID_Tipo_PensionU = (scTipoPensionUnica.SelectedIndex == 0) ? 1 : 2;

                        context.SaveChanges();
                        CargarDtPensionUnica();
                        CargarPanelPensionUnica();
                        scPensionUnica.SelectedIndex = selc;
                        MessageBox.Show("Pension Unica Actualizada");
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








        #endregion

        #region ConfiguracionPensiones
        private void CargarConfiguracionPensiones ()
        {
            CargarDtPension();
            CargarPensiones();

        }

        private void CargarDtPension()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbPensionesUnicas = (from pe in context.pensiones
                                         select new Modelos.ModeloDtPensiones
                                         {
                                             ID = pe.ID_Tipo_Pension,
                                             Tipo = pe.Pensn_Tipo,
                                             Bonificacion = pe.Pens_Bonificacion,
                                             CostoRegular = pe.Pens_Costo_Regular,
                                             Cobro1=pe.Pens_Cobro_1,
                                             Cobro2=pe.Pens_Cobro_2,
                                             DiasInactivo=pe.Pens_DiasInactivo,
                                             Recargos=pe.Pens_Recargos,
                                             Tolerancia=pe.Pens_Tolerancia_dias

                                         }).AsQueryable();
                dtPensiones.DataSource = tbPensionesUnicas.ToList();


            }
        }

        private void CargarPensiones()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                Dictionary<string, string> diccionario = new Dictionary<string, string>();

                var item = context.pensiones.ToList();
                foreach (var valores in item)
                {
                    var nombre = valores.Pensn_Tipo;
                    var id = valores.ID_Tipo_Pension;
                    diccionario.Add(id.ToString(), nombre);
                }
                scTipoPension.DataSource = new BindingSource(diccionario, null);
                scTipoPension.DisplayMember = "Value";
                scTipoPension.ValueMember = "Key";
            }
           
        }

        private void scTipoPension_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDPension = Int32.Parse((((KeyValuePair<string, string>)scTipoPension.SelectedItem).Key));
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbPensiones = (from pe in context.pensiones
                                   where (pe.ID_Tipo_Pension == IDPension)                                        
                                   select new Modelos.ModeloDtPensiones
                                   {
                                     ID=pe.ID_Tipo_Pension,
                                     Bonificacion=pe.Pens_Bonificacion,
                                     Cobro1=pe.Pens_Cobro_1,
                                     Cobro2=pe.Pens_Cobro_2,
                                     CostoRegular=pe.Pens_Costo_Regular,
                                     DiasInactivo=pe.Pens_DiasInactivo,
                                     Recargos=pe.Pens_Recargos,
                                     Tipo=pe.Pensn_Tipo,
                                     Tolerancia=pe.Pens_Tolerancia_dias
                                   }).ToList();
                foreach (var item in tbPensiones)
                {
                    txtPagoAnticipado.Text = item.Bonificacion.ToString();
                    txtCostoRegular.Text = item.CostoRegular.ToString();
                    dmTolerancia.Value = Convert.ToDecimal(item.Tolerancia);
                    txtRecargos.Text = item.Recargos.ToString();
                    dmCancelar.Value = Convert.ToDecimal(item.DiasInactivo);
                    dmCobro1.Value = Convert.ToDecimal(item.Cobro1);
                    dmCobro2.Value = Convert.ToDecimal(item.Cobro2);
                }
            }
        }
      
        
        
        
        private void btnActualizarPension_Click(object sender, EventArgs e)
        {
            try
            {
            using (var context = new PARKING_DBEntities(CadenaConexion))            
                {
                var a = context.pensiones.SingleOrDefault(n => n.ID_Tipo_Pension == IDPension);
                if (a != null)
                {

                    a.Pens_Bonificacion =(Cultura() == ",") ? Convert.ToDecimal(txtPagoAnticipado.Text.Replace('.', ',')): Convert.ToDecimal(txtPagoAnticipado.Text);
                    a.Pens_Costo_Regular= (Cultura() == ",") ? Convert.ToDecimal(txtCostoRegular.Text.Replace('.', ',')): Convert.ToDecimal(txtCostoRegular.Text);
                    a.Pens_Recargos= (Cultura()==",") ? Convert.ToDecimal(txtRecargos.Text.Replace('.', ',')): Convert.ToDecimal(txtRecargos.Text);
                    a.Pens_Tolerancia_dias = Convert.ToInt32(dmTolerancia.Value);
                    a.Pens_DiasInactivo = Convert.ToInt32(dmCancelar.Value);
                    a.Pens_Cobro_1 = Convert.ToInt32(dmCobro1.Value);
                    a.Pens_Cobro_2 = Convert.ToInt32(dmCobro2.Value);                   
                    context.SaveChanges();
                    CargarConfiguracionPensiones();
                    MessageBox.Show("Pension Actualizada");
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

        #endregion

        #region ConfiguracionServicioAdicional
        private void CargarConfiguracionServicioAdicional()
        {
            CargarDtServicoAdicional();
            if (dtServicio.Rows.Count>0)
            {
                int? id = GetID();
                if (id != null)
                {
                    using (var context = new PARKING_DBEntities(CadenaConexion))
                    {
                        var a = context.serviciosadicionales.SingleOrDefault(n => n.ID_ServicioAd == id);
                        lblServicioAdicional.Text = a.ServicioAdicional;
                        lblImporteServicioAdicional.Text = a.Precio_ServiciosAd.ToString();
                        lblTiempoGracia.Text = a.Tiempo_Gracia.ToString();
                        ckEstado.Checked = a.ServicioAd_Activo.GetValueOrDefault();

                    }
                }
            }

        }

        private void CargarDtServicoAdicional()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var tbServicios = (from sa in context.serviciosadicionales
                                         select new Modelos.ModeloConfiguracionServiciosAdicionales
                                         {
                                             ID = sa.ID_ServicioAd,
                                             ServicioAdicional = sa.ServicioAdicional,
                                             Estado = sa.ServicioAd_Activo.ToString(),
                                             Costo = sa.Precio_ServiciosAd,
                                             Tiempo=sa.Tiempo_Gracia
                                            

                                         }).AsQueryable();
                dtServicio.DataSource = tbServicios.ToList();


            }
        }

      
    
        private void btnCrearServicio_Click(object sender, EventArgs e)
        {
            using (var a = new NuevoServicioAdicional())
            {
                a.Refresh_ += A_Refresh_; ;
                a.ShowDialog();

            }
        }

        private void A_Refresh_()
        {
            CargarDtServicoAdicional();

        }


        private void btnActualizarServicio_Click(object sender, EventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                using (var a = new ActualizarServicioAdicional (id.GetValueOrDefault()))
                {
                    a.Refresh_ += A_Refresh_;
                    a.ShowDialog();


                }

            }
        }

        private int? GetID()
        {
            try
            {
                var a = dtServicio.Rows[dtServicio.CurrentRow.Index].Cells[0].Value.ToString();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var query = (from sa in context.serviciosadicionales where sa.ServicioAdicional == a select sa.ID_ServicioAd).FirstOrDefault();

                    return query; 
                }
                
               
            }
            catch
            {
                return null;

            }
        }

        #endregion













        #region ConfiguracionEmail
        private void CargarConfiguracionEmail()
        {
            int index = 0;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var confEmail = context.email.Where(n => n.ID_Email == 1).ToList();

                foreach (var item in confEmail)
                {
                    if (item.Url_Encabezado != null && item.Url_Encabezado.Length > 0)
                    {
                        Bitmap bmp;
                        using (var ms = new MemoryStream(item.Url_Encabezado))
                        {
                            bmp = new Bitmap(ms);
                            pbEncabezado.Image = bmp;
                        }
                    }
                    else
                    {
                        pbEncabezado.Image = Properties.Resources.logo4parking;
                    }
                    
                    txtDireccionCorreoE.Text = item.Direccion_Email;
                    txtContraseña.Text = item.Password;
                    txtAsunto.Text = item.Asunto;
                    txtCuerpo.Text = item.Cuerpo;
                    txtNombreEmail.Text = item.Nombre;
                    txtDireccionEmail.Text = item.Direccion;
                    txtEmailEmail.Text = item.Direccion_Email;
                    txtTelEmail.Text = item.Telefono;
                    txtWebSiteEmail.Text = item.WebSite;
                    index = item.ID_ProveedoresCorreo.GetValueOrDefault();
                }


            }
            CargarServidoresSMTP();
            scServidor.SelectedIndex = index-1;

        }

        private void ckPasswordEmail_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseña.PasswordChar = (ckPasswordEmail.Checked == false) ? '*' : Convert.ToChar(0);
        }

        private void CargarServidoresSMTP()
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
        private void btnCargarEmail_Click(object sender, EventArgs e)
        {
            try
            {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var img = new Bitmap(openFileDialog1.FileName);
                ImageConverter converter = new ImageConverter();
                var byt = (byte[])converter.ConvertTo(img, typeof(byte[]));
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.email.SingleOrDefault(n => n.ID_Email == 1);
                    if (a != null)
                    {
                        a.Url_Encabezado = byt;
                        context.SaveChanges();
                        pbEncabezado.Image = new Bitmap(img);
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

        private void btnEliminarEmail_Click(object sender, EventArgs e)
        {
            try
            {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.email.SingleOrDefault(n => n.ID_Email == 1);
                if (a != null)
                {
                    a.Url_Encabezado = null;
                    context.SaveChanges();
                    pbEncabezado.Image = Properties.Resources.logo4parking;

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
            ID_ServidorSMTP = Int32.Parse((((KeyValuePair<string, string>)scServidor.SelectedItem).Key));

        }

        private void btnActualizarEmail_Click(object sender, EventArgs e)
        {
            try
            {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                var a = context.email.SingleOrDefault(n => n.ID_Email == 1);
                if (a != null)
                {

                    a.Direccion_Email = txtDireccionCorreoE.Text.Trim();
                    a.Password = txtContraseña.Text.Trim();
                    a.Asunto = txtAsunto.Text.Trim();
                    a.Cuerpo = txtCuerpo.Text.Trim();
                    a.Nombre = txtNombreEmail.Text.Trim();
                    a.Direccion = txtDireccionEmail.Text.Trim();
                    a.Telefono = txtTelEmail.Text.Trim();
                    a.WebSite = txtWebSiteEmail.Text.Trim();
                    a.ID_ProveedoresCorreo = ID_ServidorSMTP;
                    context.SaveChanges();
                    MessageBox.Show("Datos del Email Actualizados");
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

        private void btnPruebaEnvio_Click(object sender, EventArgs e)
        {
            try
            {

            if (string.IsNullOrEmpty(txtDireccionCorreoE.Text)|| string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Coloque el correo y contraseña");
            }
            else
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(txtDireccionCorreoE.Text.Trim());
                msg.Subject = "Correo Prueba";
                msg.SubjectEncoding = Encoding.UTF8;

                msg.Body="Correo Prueba";
                msg.BodyEncoding= Encoding.UTF8;
                msg.From = new MailAddress(txtDireccionCorreoE.Text.Trim());

                SmtpClient cliente = new SmtpClient();
                cliente.Credentials = new NetworkCredential(txtDireccionCorreoE.Text.Trim(),txtContraseña.Text.Trim());

                var provedor = new proveedorescorreo();
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    provedor = context.proveedorescorreo.Where(n => n.ID_ProveedoresCorreo == ID_ServidorSMTP).FirstOrDefault();
                }
                cliente.Port = provedor.Puerto.GetValueOrDefault();
                cliente.EnableSsl = true;
                cliente.Host = provedor.Servidor_SMTP;
                try
                {
                    cliente.Send(msg);
                    MessageBox.Show("Mensaje Enviado Correctamente!");

                }
                catch (Exception )
                {
                    MessageBox.Show("Error al enviar\n"+
                        "Cuentas de Gmail deben garantizar el permiso a aplicaciones menos seguras\n"
                        +"Cuentas de Outlook deben estar verificadas con numero de telefono");

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

        private void btnServidoresSMTP_Click(object sender, EventArgs e)
        {
            using (var a = new ServidoresSMTP())
            {
                a.ShowDialog();

            }
        }

        #endregion

        #region Constructor
        public Configuraciones(string cadena, string usuario)
        {
            this.Conexion = cadena;
            this.Usuario = usuario;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            CargarConfiguracionGeneral();
            CargarConfiguracionTicket();
            CargarConfiguracionCobro();
            CargarConfiguracionPensionUnica();
            CargarConfiguracionPensiones();
            CargarConfiguracionServicioAdicional();
            CargarConfiguracionEmail();
        }

      

        private void txtTarifa_Click(object sender, EventArgs e)
        {
            txtTarifa.SelectAll();
        }

        private void dtServicio_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int? id = GetID();
            if (id != null)
            {
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    var a = context.serviciosadicionales.SingleOrDefault(n => n.ID_ServicioAd == id);
                    lblServicioAdicional.Text = a.ServicioAdicional;
                    lblImporteServicioAdicional.Text = a.Precio_ServiciosAd.ToString();
                    lblTiempoGracia.Text = a.Tiempo_Gracia.ToString();
                    ckEstado.Checked = a.ServicioAd_Activo.GetValueOrDefault();

                }
            }
        }











        #endregion

        #region MetodosApoyo

        private string Cultura()
        {
            var info = CultureInfo.CurrentCulture.NumberFormat;
            return info.NumberDecimalSeparator;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Sololetras(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&
        (e.KeyChar != ' '))

            {
                e.Handled = true;
            }
        }

      

        private void TextoVacio(object sender, EventArgs e)
        {
            txt30min.Text=(string.IsNullOrEmpty(txt30min.Text))?"0":txt30min.Text;
            txt1hr.Text = (string.IsNullOrEmpty(txt1hr.Text)) ? "0" : txt1hr.Text;
            txt1h30m.Text = (string.IsNullOrEmpty(txt1h30m.Text)) ? "0" : txt1h30m.Text;
            txt2h.Text = (string.IsNullOrEmpty(txt2h.Text)) ? "0" : txt2h.Text;
            txt2h30m.Text = (string.IsNullOrEmpty(txt2h30m.Text)) ? "0" : txt2h30m.Text;
            txt3h.Text = (string.IsNullOrEmpty(txt3h.Text)) ? "0" : txt3h.Text;
            txt3h30m.Text = (string.IsNullOrEmpty(txt3h30m.Text)) ? "0" : txt3h30m.Text;
            txt4h.Text = (string.IsNullOrEmpty(txt4h.Text)) ? "0" : txt4h.Text;
            txt4h30m.Text = (string.IsNullOrEmpty(txt4h30m.Text)) ? "0" : txt4h30m.Text;
            txt5h.Text = (string.IsNullOrEmpty(txt5h.Text)) ? "0" : txt5h.Text;
            txt5h30m.Text = (string.IsNullOrEmpty(txt5h30m.Text)) ? "0" : txt5h30m.Text;
            txt6h.Text = (string.IsNullOrEmpty(txt6h.Text)) ? "0" : txt6h.Text;
            txt6h30m.Text = (string.IsNullOrEmpty(txt6h30m.Text)) ? "0" : txt6h30m.Text;
            txt7h.Text = (string.IsNullOrEmpty(txt7h.Text)) ? "0" : txt7h.Text;
            txt7h30m.Text = (string.IsNullOrEmpty(txt7h30m.Text)) ? "0" : txt7h30m.Text;
            txt8h.Text = (string.IsNullOrEmpty(txt8h.Text)) ? "0" : txt8h.Text;
            txt8h30m.Text = (string.IsNullOrEmpty(txt8h30m.Text)) ? "0" : txt8h30m.Text;
            txt9h.Text = (string.IsNullOrEmpty(txt9h.Text)) ? "0" : txt9h.Text;
            txt9h30m.Text = (string.IsNullOrEmpty(txt9h30m.Text)) ? "0" : txt9h30m.Text;
            txt10h.Text = (string.IsNullOrEmpty(txt10h.Text)) ? "0" : txt10h.Text;
            txt10h30m.Text = (string.IsNullOrEmpty(txt10h30m.Text)) ? "0" : txt10h30m.Text;
            txt11h.Text = (string.IsNullOrEmpty(txt11h.Text)) ? "0" : txt11h.Text;
            txt11h30m.Text = (string.IsNullOrEmpty(txt11h30m.Text)) ? "0" : txt11h30m.Text;
            txt12h.Text = (string.IsNullOrEmpty(txt12h.Text)) ? "0" : txt12h.Text;
            txt12h30m.Text = (string.IsNullOrEmpty(txt12h30m.Text)) ? "0" : txt12h30m.Text;
            txt13h.Text = (string.IsNullOrEmpty(txt13h.Text)) ? "0" : txt13h.Text;
            txt13h30m.Text = (string.IsNullOrEmpty(txt13h30m.Text)) ? "0" : txt13h30m.Text;
            txt14h.Text = (string.IsNullOrEmpty(txt14h.Text)) ? "0" : txt14h.Text;
            txt14h30m.Text = (string.IsNullOrEmpty(txt14h30m.Text)) ? "0" : txt14h30m.Text;
            txt15h.Text = (string.IsNullOrEmpty(txt15h.Text)) ? "0" : txt15h.Text;
            txt15h30m.Text = (string.IsNullOrEmpty(txt15h30m.Text)) ? "0" : txt15h30m.Text;
            txt16h.Text = (string.IsNullOrEmpty(txt16h.Text)) ? "0" : txt16h.Text;
            txt16h30m.Text = (string.IsNullOrEmpty(txt16h30m.Text)) ? "0" : txt16h30m.Text;
            txt17h.Text = (string.IsNullOrEmpty(txt17h.Text)) ? "0" : txt17h.Text;
            txt17h30m.Text = (string.IsNullOrEmpty(txt17h30m.Text)) ? "0" : txt17h30m.Text;
            txt18h.Text = (string.IsNullOrEmpty(txt18h.Text)) ? "0" : txt18h.Text;
            txt18h30m.Text = (string.IsNullOrEmpty(txt18h30m.Text)) ? "0" : txt18h30m.Text;
            txt19h.Text = (string.IsNullOrEmpty(txt19h.Text)) ? "0" : txt19h.Text;
            txt19h30m.Text = (string.IsNullOrEmpty(txt19h30m.Text)) ? "0" : txt19h30m.Text;
            txt20h.Text = (string.IsNullOrEmpty(txt20h.Text)) ? "0" : txt20h.Text;
            txt20h30m.Text = (string.IsNullOrEmpty(txt20h30m.Text)) ? "0" : txt20h30m.Text;
            txt21h.Text = (string.IsNullOrEmpty(txt21h.Text)) ? "0" : txt21h.Text;
            txt21h30m.Text = (string.IsNullOrEmpty(txt21h30m.Text)) ? "0" : txt21h30m.Text;
            txt22h.Text = (string.IsNullOrEmpty(txt22h.Text)) ? "0" : txt22h.Text;
            txt22h30m.Text = (string.IsNullOrEmpty(txt22h30m.Text)) ? "0" : txt22h30m.Text;
            txt23h.Text = (string.IsNullOrEmpty(txt23h.Text)) ? "0" : txt23h.Text;
            txt23h30m.Text = (string.IsNullOrEmpty(txt23h30m.Text)) ? "0" : txt23h30m.Text;
            txt24h.Text = (string.IsNullOrEmpty(txt24h.Text)) ? "0" : txt24h.Text;
            txtImporteLineal.Text = (string.IsNullOrEmpty(txtImporteLineal.Text)) ? "0" : txtImporteLineal.Text;
            txtImporteDefault.Text = (string.IsNullOrEmpty(txtImporteDefault.Text)) ? "0" : txtImporteDefault.Text;

            txtTarifa.Text = (string.IsNullOrEmpty(txtTarifa.Text)) ? "0" : txtTarifa.Text;
           
            txtPagoAnticipado.Text = (string.IsNullOrEmpty(txtPagoAnticipado.Text)) ? "0" : txtPagoAnticipado.Text;
            txtCostoRegular.Text = (string.IsNullOrEmpty(txtCostoRegular.Text)) ? "0" : txtCostoRegular.Text;
            txtRecargos.Text = (string.IsNullOrEmpty(txtRecargos.Text)) ? "0" : txtRecargos.Text;

           


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


        #endregion



      
    }
}
