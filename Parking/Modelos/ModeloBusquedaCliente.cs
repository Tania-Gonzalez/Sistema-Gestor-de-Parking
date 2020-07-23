using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
     public class ModeloBusquedaCliente
    {
        public int ID { get; set; }
        [DisplayName("Nombre")]
        public string Nombre { get; set; }
        [DisplayName("Direccion")]
        public string Direccion{ get; set; }

        [DisplayName("Telefono")]
        public string Tel1 { get; set; }

        [DisplayName("RFC")]
        public string RFC { get; set; }

        [DisplayName("Placa")]
        public string Placa { get; set; }
        [DisplayName("Cantidad de Pensiones Registradas")]
        public int CantidadPensiones { get; set; }

    }
}
