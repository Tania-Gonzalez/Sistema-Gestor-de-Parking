using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloTicketActivadoYPagado
    {
        public DateTime? ultimaFecha{ get; set; }
        public string Importe{ get; set; }
        public string ID { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Color { get; set; }
        public string ProximoPago { get {return (ultimaFecha= ultimaFecha.Value.AddDays(1)).Value.ToString("hh:mm:ss tt", new System.Globalization.CultureInfo("en-US")); } }

    }
}
