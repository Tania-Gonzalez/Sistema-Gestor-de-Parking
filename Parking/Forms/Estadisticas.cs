
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
using System.Windows.Forms.DataVisualization.Charting;

namespace Parking.Forms
{
    public partial class Estadisticas : Form
    {
        #region Variables
        string rutaJson = Environment.CurrentDirectory + @"\data.json";
        string CadenaConexion;
        List<Modelos.ModeloEstadisticas> lista = new List<Modelos.ModeloEstadisticas>();
        List<Modelos.ModeloEstadisticasGraficaVehiculo> listagrafica = new List<Modelos.ModeloEstadisticasGraficaVehiculo>();
        List<Modelos.ModeloEstadisticasGraficaVentas> listagraficaventas = new List<Modelos.ModeloEstadisticasGraficaVentas>();
        string mensajeError = "Error Inesperado Contacte al Administrador!";
        string Conexion;
        int mes,IDPeriodo;
        #endregion
        public Estadisticas(string conexion)
        {
            this.Conexion = conexion;
            InitializeComponent();
            ModeloDatos oSettings = JsonConvert.DeserializeObject<ModeloDatos>(File.ReadAllText(rutaJson));
            CadenaConexion = oSettings.CadenaConexion;
            CargarMeses();
           

        }
        public  void CargarMeses()
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            diccionario.Add("1", "Enero");
            diccionario.Add("2", "Febrero");
            diccionario.Add("3", "Marzo");
            diccionario.Add("4", "Abril");
            diccionario.Add("5", "Mayo");
            diccionario.Add("6", "Junio");
            diccionario.Add("7", "Julio");
            diccionario.Add("8", "Agosto");
            diccionario.Add("9", "Septiembre");
            diccionario.Add("10", "Octubre");
            diccionario.Add("11", "Noviembre");
            diccionario.Add("12", "Diciembre");
            cbFecha.DataSource = new BindingSource(diccionario, null);
            cbFecha.DisplayMember = "Value";
            cbFecha.ValueMember = "Key";
            dmAño.Value = (Convert.ToDecimal(DateTime.Now.Year));
            var mes = DateTime.Now.Month;
            cbFecha.SelectedIndex = mes - 1;


        }
        private void CargarSemanas()
        {
            lista.Clear();

            var firstDayOfMonth = new DateTime(Convert.ToInt32(dmAño.Value), mes, 1);

            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int ultimo = lastDayOfMonth.Day;
            int primero = 1;
            var fechaInicioSemana = new DateTime(Convert.ToInt32(dmAño.Value), mes, primero);
            int ID = 0;
            while (primero <= ultimo)
            {
                Modelos.ModeloEstadisticas modelo = new Modelos.ModeloEstadisticas();
                DateTime ultimafecha;
                string nombreInicioSemana = fechaInicioSemana.ToString("dddd", new CultureInfo("es-ES"));
                switch (nombreInicioSemana) 
                {
                    case "lunes":
                        ultimafecha = fechaInicioSemana.AddDays(6);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID+=1;                 
                        fechaInicioSemana= fechaInicioSemana.AddDays(7);
                        primero +=7;
                        lista.Add(modelo);
                        break;
                    case "martes":
                        ultimafecha = fechaInicioSemana.AddDays(5);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1;
                        fechaInicioSemana = fechaInicioSemana.AddDays(6);
                        primero += 6;
                        lista.Add(modelo);
                        break;
                    case "miércoles":
                        ultimafecha = fechaInicioSemana.AddDays(4);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1; ;        
                        fechaInicioSemana = fechaInicioSemana.AddDays(5);
                        primero += 5;
                        lista.Add(modelo);
                        break;
                    case "jueves":
                        ultimafecha = fechaInicioSemana.AddDays(3);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1;
                        fechaInicioSemana = fechaInicioSemana.AddDays(4);
                        primero += 4;
                        lista.Add(modelo);
                        break;
                    case "viernes":
                        ultimafecha = fechaInicioSemana.AddDays(2);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1;
                        fechaInicioSemana = fechaInicioSemana.AddDays(3);
                        primero += 3;
                        lista.Add(modelo);
                        break;
                    case "sábado":
                        ultimafecha = fechaInicioSemana.AddDays(1);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1;
                        fechaInicioSemana = fechaInicioSemana.AddDays(2);
                        primero += 2;
                        lista.Add(modelo);
                        break;
                    case "domingo":
                        ultimafecha = fechaInicioSemana.AddDays(0);
                        modelo.ID = ID;
                        modelo.FechaInicio = fechaInicioSemana;
                        modelo.FechaFinal = ultimafecha;
                        ID += 1;
                        fechaInicioSemana = fechaInicioSemana.AddDays(1);
                        primero += 1;
                        lista.Add(modelo);
                        break;  
                }



            };
            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            foreach (var item in lista)
            {
                var nombre = item.Texto;
                var id = item.ID;
                diccionario.Add(id.ToString(), nombre);
                
            }
            cbSemana.DataSource = new BindingSource(diccionario, null);
            cbSemana.DisplayMember = "Value";
            cbSemana.ValueMember = "Key";
            cbSemana.SelectedIndex = 0;
            
        }


        private void cbFecha_SelectedIndexChanged(object sender, EventArgs e)
        {
           mes = Int32.Parse((((KeyValuePair<string, string>)cbFecha.SelectedItem).Key));
           CargarSemanas();

        }

        private void dmAño_ValueChanged(object sender, EventArgs e)
        {
            CargarSemanas();
        }
      

        private void btnGraficar_Click(object sender, EventArgs e)
        {
            try
            {
            btnCrearImagen.Visible = true;
            chGrafico.Visible = true;
            dtServicio.DataSource = null;
            listagraficaventas.Clear();
            listagrafica.Clear();
            chGrafico.Titles.Clear();
            chGrafico.Series.Clear();
            lblImporte.Visible = (radioImporte.Checked==true) ?true:false;
            lblVehiculos.Visible = (radioVehiculos.Checked == true) ? true : false;
            if (radioVehiculos.Checked == true)
            {
               
                var semana = lista.Where(n=>n.ID==IDPeriodo).FirstOrDefault();
                var dias =new List<DateTime>();
                int total=0;
                for (var date = semana.FechaInicio; date <= semana.FechaFinal; date = date.AddDays(1))
                {
                    dias.Add(date);                
                }
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    foreach (var item in dias)
                    {
                        var cantidad = (from bp in context.banco_parking
                                        where ((bp.FechaEntrada == item || bp.FechaSalida==item) && bp.Importe>0) 
                                        select bp.Folio).Count();
                        total = cantidad + total;
                        Modelos.ModeloEstadisticasGraficaVehiculo subquery = new Modelos.ModeloEstadisticasGraficaVehiculo
                        {
                            CantidadVehiculos = cantidad,
                            Dia = item,                           

                        };                      
                            listagrafica.Add(subquery);
                        
                    }
                }
                chGrafico.Titles.Add("Cantidad de vehiculos");
                foreach (var variables in listagrafica)
                {                   
                    
                    Series s = chGrafico.Series.Add(variables.Dia.ToShortDateString());
                    s.Label = variables.CantidadVehiculos.ToString();
                    s.Points.Add(variables.CantidadVehiculos);
                }
                lblVehiculos.Text = total.ToString();
                dtServicio.DataSource = listagrafica;
            }
            if (radioImporte.Checked == true)
            {
                var semana = lista.Where(n => n.ID == IDPeriodo).FirstOrDefault();
                var dias = new List<DateTime>();
                decimal? total = 0;
                for (var date = semana.FechaInicio; date <= semana.FechaFinal; date = date.AddDays(1))
                {
                    dias.Add(date);
                }
                using (var context = new PARKING_DBEntities(CadenaConexion))
                {
                    foreach (var item in dias)
                    {
                        var totalCobrado = (from bp in context.banco_parking
                                        where ((bp.FechaEntrada == item || bp.FechaSalida == item) && bp.Importe > 0)
                                        select bp.Importe).Sum();
                        total = totalCobrado + total;
                        Modelos.ModeloEstadisticasGraficaVentas subquery = new Modelos.ModeloEstadisticasGraficaVentas
                        {
                            Total = (totalCobrado==null) ?0: totalCobrado,
                            Dia = item,

                        };
                        listagraficaventas.Add(subquery);

                    }
                }
                total = (total==null) ?0:total;
                chGrafico.Titles.Add("Total Ingresos");
                foreach (var variables in listagraficaventas)
                {

                    Series s = chGrafico.Series.Add(variables.Dia.ToShortDateString());
                    s.Label = variables.Total.ToString();
                    s.Points.Add(Convert.ToInt32(variables.Total));
                }
                lblImporte.Text = total.ToString(); 
                dtServicio.DataSource = listagraficaventas;



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

        private void btnCrearImagen_Click(object sender, EventArgs e)
        {
            try
            {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Gráfico_" + DateTime.Now.ToString("dddd", new CultureInfo("es-ES")) + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            sfd.Filter = "Imagenes PNG |*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                chGrafico.SaveImage(sfd.FileName, ChartImageFormat.Png);

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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void cbSemana_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDPeriodo = Int32.Parse((((KeyValuePair<string, string>)cbSemana.SelectedItem).Key));
        }
    }
}
