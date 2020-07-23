using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloEstadisticas
    {
        public int ID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public string Texto{ get { return "Del " + FechaInicio.ToString("dddd", new CultureInfo("es-ES")) + " " + FechaInicio.ToString("dd/MM/yyyy") + "---Al---" + FechaFinal.ToString("dddd", new CultureInfo("es-ES")) + " " + FechaFinal.ToString("dd/MM/yyyy") ;}}
    }
    public class ModeloEstadisticasGraficaVehiculo
    {
        [Browsable(false)]
        public DateTime Dia { get; set; }
        [DisplayName("Día")]
        public string diaString { get { return Dia.ToString("dddd", new CultureInfo("es-ES"))+", "+Dia.Day.ToString() + " de "+ Dia.ToString("MMMM", new CultureInfo("es-ES")) +" de "+ Dia.Year; } }
        [DisplayName("Cantidad")]
        public int CantidadVehiculos { get; set; }

    }
    public class ModeloEstadisticasGraficaVentas
    {
        [Browsable(false)]
        public DateTime Dia { get; set; }
        [DisplayName("Día")]
        public string diaString { get { return Dia.ToString("dddd", new CultureInfo("es-ES")) + ", " + Dia.Day.ToString() + " de " + Dia.ToString("MMMM", new CultureInfo("es-ES")) + " de " + Dia.Year; } }
        [DisplayName("Total")]
        public decimal? Total { get; set; }

    }
}
