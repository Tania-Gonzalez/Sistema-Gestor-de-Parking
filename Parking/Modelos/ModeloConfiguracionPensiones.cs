using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloDtPensiones
    {
        public int ID { get; set; }
        [DisplayName("Nombre")]
        public string Tipo { get; set; }
        public decimal? Bonificacion { get; set; }
        [DisplayName("Costo Regular")]
        public decimal? CostoRegular { get; set; }
        public int? Tolerancia { get; set; }
        public decimal? Recargos { get; set; }
        [DisplayName("Día Cobro 1")]
        public int? Cobro1 { get; set; }
        [DisplayName("Día Cobro 2")]
        public int? Cobro2 { get; set; }
        [DisplayName("Días Inactivo")]
        public int? DiasInactivo { get; set; }

    }

  
}
