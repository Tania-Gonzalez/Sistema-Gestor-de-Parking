
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Parking.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking.Forms
{
    public partial class Corte : Form
    {
        #region Variables  
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        int CantidadCortes,IDCorte;
        string usuario;
        #endregion


        public Corte(string Usuario)
        {
            this.usuario = Usuario;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;

            CargarHistorialCortes();
         
        }
      
        #region HistorialCorte




  
        
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        #region Cortes
   

        private void CargarHistorialCortes()
        {
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                CantidadCortes = (from cs in context.cortes select cs.Id_Corte).Count();
                var listabusqueda = (from bp in context.cortes
                                     select new Modelos.ModeloCorteHistorial
                                     {
                                         ID = bp.Id_Corte,
                                         CorteNormal = bp.CorteNormal.ToString(),
                                         CortePension = bp.CortePension.ToString(),
                                         CorteTarifa2 = bp.CorteTarifa2.ToString(),
                                         FechaCorte = bp.FechaCorte.ToString(),
                                         ImporteCortes = bp.Importe_Cortes.ToString(),
                                         ImporteParqueo = bp.Importe_Parqueo.ToString(),
                                         ImporteReportado = bp.Importe_Reportado.ToString(),
                                         Usuario = bp.Usuario
                                     }).ToList();
                dtHistorialCorte.DataSource = listabusqueda;
                DataGridViewCheckBoxColumn btnCheck = new DataGridViewCheckBoxColumn();
                btnCheck.HeaderText = "";
                btnCheck.Name = "Reporte";
                btnCheck.FalseValue = false;
                btnCheck.TrueValue = true;
                      
                if (dtHistorialCorte.Columns["Reporte"] == null)
                {
                    dtHistorialCorte.Columns.Insert(0, btnCheck);
                }

                dmCorteResumido.Maximum= context.cortes.Max(x => x.Id_Corte);
            }
            dmCortes.Maximum = CantidadCortes;
            dmCortes.Value = CantidadCortes;


        }

        private void btnMostrarHistorialCortes_Click(object sender, EventArgs e)
        {
            dtHistorialCorte.DataSource = null;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                CantidadCortes = (from cs in context.cortes select cs.Id_Corte).Count();
                var can = Convert.ToInt32(dmCortes.Value);
                var listabusqueda = (from bp in context.cortes
                                     orderby bp.Id_Corte descending
                                     select new Modelos.ModeloCorteHistorial
                                     {
                                         ID = bp.Id_Corte,
                                         CorteNormal = bp.CorteNormal.ToString(),
                                         CortePension = bp.CortePension.ToString(),
                                         CorteTarifa2 = bp.CorteTarifa2.ToString(),
                                         FechaCorte = bp.FechaCorte.ToString(),
                                         ImporteCortes = bp.Importe_Cortes.ToString(),
                                         ImporteParqueo = bp.Importe_Parqueo.ToString(),
                                         ImporteReportado = bp.Importe_Reportado.ToString(),
                                         Usuario = bp.Usuario
                                     }).Take(can).ToList();
                dtHistorialCorte.DataSource = listabusqueda;
                DataGridViewCheckBoxColumn btnCheck = new DataGridViewCheckBoxColumn();
                btnCheck.HeaderText = "";
                btnCheck.Name = "Reporte";
                btnCheck.FalseValue = false;
                btnCheck.TrueValue = true;           
                if (dtHistorialCorte.Columns["Reporte"] == null)
                {
                    dtHistorialCorte.Columns.Insert(0, btnCheck);
                }

            }
        }

        private int? GetIDCorte()
        {
            try
            {
                var a = dtHistorialCorte.Rows[dtHistorialCorte.CurrentRow.Index].Cells[1].Value.ToString();
                IDCorte = Int32.Parse(a);
                return IDCorte;
            }
            catch
            {
                return null;

            }
        }

        private void dtHistorialCorte_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtHistorialCorte.Rows.Count > 0)
            {
                int? id = GetIDCorte();
                if (id != null)
                {
                    label4.Text = id.ToString();
                }
                if (e.ColumnIndex ==0)
                {
                    if (Convert.ToBoolean(dtHistorialCorte["Reporte", dtHistorialCorte.CurrentRow.Index].Value) == false)
                    {
                        dtHistorialCorte["Reporte", dtHistorialCorte.CurrentRow.Index].Value = true;
                    }
                    else
                    { 
                        dtHistorialCorte["Reporte", dtHistorialCorte.CurrentRow.Index].Value = false;
                    }
                    
                }




            }
        }

        private void btnImprimirCorte_Click(object sender, EventArgs e)
        {
            if (label4.Text!="0")
            {
                var pd = new PrintDocument();
                var ps = new PrinterSettings();
                pd.PrinterSettings = ps;
                pd.PrintPage += ImprimirReporte;
                pd.Print();

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


            e.Graphics.DrawString("***CORTE RESUMIDO***", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado, tamPapel, 20), LetrasCentradas);
            e.Graphics.DrawString(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US")), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 15, tamPapel, 20), LetrasCentradas);
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {

                var ultimoCorte = context.cortes.Where(n => n.Id_Corte == IDCorte-1).FirstOrDefault();
                var corte = context.cortes.Where(n => n.Id_Corte == IDCorte).FirstOrDefault();
                var park = context.banco_parking.Where(n=>n.Corte==1&&n.Id_corte==IDCorte).Select(n=>n.Folio).ToList();
                var counentradas = (from bp in context.banco_parking where bp.FechaEntrada == DateTime.Now select bp.FechaEntrada).Count();
                var capacidad = (from config in context.configgen1 select config.Capacidad).SingleOrDefault();
                var counparking = (from bp in context.banco_parking where bp.Estancia == "EN PROGRESO" select bp.Estancia).Count();
                var salidas = counentradas - counparking;
                var importeNormal = (from bp in context.banco_parking
                                     join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                     where cb.FormaCobro == true&& bp.Corte==1&&bp.Id_corte==IDCorte&& bp.Estancia != "EN PROGRESO"
                                     select bp.Importe).Sum();
                var importeConvenio = (from bp in context.banco_parking
                                     join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                     where cb.FormaCobro == true && bp.Corte == 0 && bp.Id_corte == IDCorte && bp.Estancia != "EN PROGRESO"
                                     select bp.Importe).Sum();
                string ultmCorte = (ultimoCorte.FechaCorte==null) ? "Sin Registro" : ultimoCorte.FechaCorte.Value.ToShortDateString();


                e.Graphics.DrawString("Corte: " + corte.Id_Corte, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Usuario: " + corte.Usuario, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Ultimo Corte: " + ultmCorte, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("*****************************", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Rango de Folios: " + park.FirstOrDefault().ToString()+" - "+park.LastOrDefault().ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Tickets Contabilizados: " + park.Count.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Entradas Registradas: " + counentradas.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Salidas Registradas: " + salidas.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Parqueado Actualmente: " + counparking.ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("*******RESUMEN COBRO******", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Importe Parqueo Normal: "+ importeNormal, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Importe Parqueo Convenio: "+ importeConvenio, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("*****************************", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Importe Total"+ (importeNormal+ importeConvenio).ToString(), fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("*****************************", fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Total del Corte: "+corte.Importe_Cortes, fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);
                e.Graphics.DrawString("Monto Ingresado: "+corte.Importe_Reportado,fuenteNormal, Brushes.Black, new RectangleF(0, espaciado += 20, tamPapel, 20), LetrasCentradas);

            }




        }

        #endregion

        private void btnBorrarSeleccion_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtHistorialCorte.Rows.Count; i++)
            {
                dtHistorialCorte["Reporte", i].Value = false;
            }
        }

        private void btnSeleccionarTodo_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtHistorialCorte.Rows.Count; i++)
            {
                dtHistorialCorte["Reporte", i].Value = true;
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            bool seleccionado = false;
            List<Modelos.ModeloCorteHistorial> lst = new List<Modelos.ModeloCorteHistorial>();
           
            for (int i = 0; i < dtHistorialCorte.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dtHistorialCorte["Reporte",i].Value) == true) 
                {
                    seleccionado = true;
                }
            }
            if (seleccionado==true)
            {
                for (int i = 0; i < dtHistorialCorte.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dtHistorialCorte["Reporte", i].Value) == true)
                    {
                        lst.Add(new Modelos.ModeloCorteHistorial
                        {
                            ID = Convert.ToInt32(dtHistorialCorte.Rows[i].Cells[1].Value.ToString()),
                            Usuario= dtHistorialCorte.Rows[i].Cells[2].Value.ToString(),
                            FechaCorte = dtHistorialCorte.Rows[i].Cells[2].Value.ToString(),
                            ImporteParqueo = dtHistorialCorte.Rows[i].Cells[3].Value.ToString(),
                            ImporteCortes= dtHistorialCorte.Rows[i].Cells[4].Value.ToString(),
                            ImporteReportado= dtHistorialCorte.Rows[i].Cells[5].Value.ToString(),
                            CorteNormal= dtHistorialCorte.Rows[i].Cells[6].Value.ToString(),
                            CorteTarifa2= dtHistorialCorte.Rows[i].Cells[7].Value.ToString(),
                            CortePension= dtHistorialCorte.Rows[i].Cells[8].Value.ToString()
                        });
                    }

                }
                
                ReportDataSource rs = new ReportDataSource("lista",lst);
                Forms.Reporte frm = new Reporte();

                frm.reportViewer1.LocalReport.DataSources.Clear();              
                frm.reportViewer1.LocalReport.DataSources.Add(rs);
                frm.reportViewer1.LocalReport.ReportEmbeddedResource = "Parking.Report1.rdlc";
                frm.reportViewer1.RefreshReport();



                frm.ShowDialog();
            }



        }

        #endregion
    
       #region HistorialTicket
        private void btnCorteResumido_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Desea realizar el corte resumido?", "Busqueda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                using (var a = new ImporteCorte(usuario))
                {
                    a.Refresh_ += CargarHistorialCortes;
                    a.ShowDialog();

                }
            }
        }

     

        private void btnSeleccionarTodosHistorialTickets_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtHistorialTickets.Rows.Count; i++)
            {
                dtHistorialTickets["Reporte", i].Value = true;
            }
        }

        private void btnBorrarTodosHistorialtickets_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtHistorialTickets.Rows.Count; i++)
            {
                dtHistorialTickets["Reporte", i].Value = false;
            }
        }

        private void dtHistorialTickets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtHistorialTickets.Rows.Count > 0)
            {
                int? id = GetIDTicket();
                if (id != null)
                {
                    label4.Text = id.ToString();
                }
                if (e.ColumnIndex == 0)
                {
                    if (Convert.ToBoolean(dtHistorialTickets["Reporte", dtHistorialTickets.CurrentRow.Index].Value) == false)
                    {
                        dtHistorialTickets["Reporte", dtHistorialTickets.CurrentRow.Index].Value = true;
                    }
                    else
                    {
                        dtHistorialTickets["Reporte", dtHistorialTickets.CurrentRow.Index].Value = false;
                    }

                }




            }
        }

        private int? GetIDTicket()
        {
            try
            {
                var a = dtHistorialTickets.Rows[dtHistorialTickets.CurrentRow.Index].Cells[1].Value.ToString();
                IDCorte = Int32.Parse(a);
                return IDCorte;
            }
            catch
            {
                return null;

            }
        }

        private void btnGenerarReporteTickets_Click(object sender, EventArgs e)
        {
            bool seleccionado = false;
            List<Modelos.ModeloHitorialTickets> lst = new List<Modelos.ModeloHitorialTickets>();

            for (int i = 0; i < dtHistorialTickets.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dtHistorialTickets["Reporte", i].Value) == true)
                {
                    seleccionado = true;
                }
            }
            if (seleccionado == true)
            {
                for (int i = 0; i < dtHistorialTickets.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dtHistorialTickets["Reporte", i].Value) == true)
                    {
                        lst.Add(new Modelos.ModeloHitorialTickets
                        {
                            Folio = dtHistorialTickets.Rows[i].Cells[1].Value.ToString(),
                            FechaEntrada = Convert.ToDateTime(dtHistorialTickets.Rows[i].Cells[2].Value.ToString()),
                            HoraEntrada = TimeSpan.Parse(dtHistorialTickets.Rows[i].Cells[3].Value.ToString()),
                            Placas = dtHistorialTickets.Rows[i].Cells[4].Value.ToString(),
                            TipoCobro = dtHistorialTickets.Rows[i].Cells[5].Value.ToString(),
                            Parqueo = dtHistorialTickets.Rows[i].Cells[6].Value.ToString(),
                            Importe = dtHistorialTickets.Rows[i].Cells[7].Value.ToString(),
                            Expedido = dtHistorialTickets.Rows[i].Cells[8].Value.ToString(),
                            PensionU = dtHistorialTickets.Rows[i].Cells[9].Value.ToString(),
                            ServicioAd = dtHistorialTickets.Rows[i].Cells[10].Value.ToString(),
                            ModoTicket = dtHistorialTickets.Rows[i].Cells[11].Value.ToString()
                        });
                    }

                }

                ReportDataSource rs = new ReportDataSource("lista", lst);
                Forms.Reporte frm = new Reporte();

                frm.reportViewer1.LocalReport.DataSources.Clear();
                frm.reportViewer1.LocalReport.DataSources.Add(rs);
                frm.reportViewer1.LocalReport.ReportEmbeddedResource = "Parking.Report2.rdlc";
                frm.reportViewer1.RefreshReport();



                frm.ShowDialog();
            }


        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            dtHistorialTickets.DataSource = null;
            using (var context = new PARKING_DBEntities(CadenaConexion))
            {
                string dmTI = dmCorteResumido.Value.ToString();
                int tolerancia = Convert.ToInt32(dmTI);
                List<Modelos.ModeloHitorialTickets> hcortes = new List<Modelos.ModeloHitorialTickets>();
                if (radioAparcados.Checked==true)
                {
                    hcortes = (from bp in context.banco_parking
                               join sa in context.serviciosadicionales on bp.ID_ServicioAd equals sa.ID_ServicioAd
                               join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                               join pu in context.pensiones_unicas on bp.ID_PensionU equals pu.ID_PensionU
                               where bp.Corte == 1 && bp.Id_corte == tolerancia
                               select new Modelos.ModeloHitorialTickets
                               {
                                   ID=bp.ID,
                                   Folio = bp.Folio,
                                   ServicioAd = sa.ServicioAdicional,
                                   Expedido = bp.ExpedidoPor,
                                   FechaEntrada = bp.FechaEntrada,
                                   HoraEntrada = bp.HoraEntrada,
                                   Importe = bp.Importe.ToString(),
                                   ModoTicket = bp.ModoTicket,
                                   Placas = bp.Placa,
                                   TipoCobro = cb.Nombre,
                                   Parqueo = bp.Estancia,
                                   PensionU = pu.Nombre_PensionU
                               }).ToList();
                    dtHistorialTickets.DataSource = hcortes;

                    DataGridViewCheckBoxColumn btnCheck = new DataGridViewCheckBoxColumn();
                    btnCheck.HeaderText = "";
                    btnCheck.Name = "Reporte";
                    btnCheck.FalseValue = false;
                    btnCheck.TrueValue = true;
                    if (dtHistorialTickets.Columns["Reporte"] == null)
                    {
                        dtHistorialTickets.Columns.Insert(0, btnCheck);
                    }
                }
                if (radioCorte.Checked == true)
                {                        

                         hcortes = (from bp in context.banco_parking
                                       join sa in context.serviciosadicionales on bp.ID_ServicioAd equals sa.ID_ServicioAd
                                       join cb in context.cobro on bp.ID_Cobro equals cb.ID_Tabulador
                                       join pu in context.pensiones_unicas on bp.ID_PensionU equals pu.ID_PensionU
                                       where bp.Corte == 0 && bp.Id_corte == tolerancia
                                       select new Modelos.ModeloHitorialTickets
                                       {
                                           ID = bp.ID,
                                           Folio = bp.Folio,
                                           ServicioAd = sa.ServicioAdicional,
                                           Expedido = bp.ExpedidoPor,
                                           FechaEntrada = bp.FechaEntrada,
                                           HoraEntrada = bp.HoraEntrada,
                                           Importe = bp.Importe.ToString(),
                                           ModoTicket = bp.ModoTicket,
                                           Placas = bp.Placa,
                                           TipoCobro = cb.Nombre,
                                           Parqueo = bp.Estancia,
                                           PensionU = pu.Nombre_PensionU
                                       }).ToList();
                        dtHistorialTickets.DataSource = hcortes;
                    DataGridViewCheckBoxColumn btnCheck = new DataGridViewCheckBoxColumn();
                    btnCheck.HeaderText = "";
                    btnCheck.Name = "Reporte";
                    btnCheck.FalseValue = false;
                    btnCheck.TrueValue = true;
                    if (dtHistorialTickets.Columns["Reporte"] == null)
                    {
                        dtHistorialTickets.Columns.Insert(0, btnCheck);
                    }
                }
                }


                
        }

        
        #endregion





    }

}
