using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloCorteHistorial
    {
        public int ID { get; set; }
        public string Usuario { get; set; }
        [DisplayName("Fecha del Corte")]
        public string FechaCorte { get; set; }
        [DisplayName("Importe Parking")]
        public string ImporteParqueo { get; set; }
        [DisplayName("Importe Total")]
        public string ImporteCortes { get; set; }
        [DisplayName("Importe Reportado")]
        public string ImporteReportado { get; set; }
        [DisplayName("Normal")]
        public string CorteNormal { get; set; }
        [DisplayName("Pensiones Unicas")]
        public string CorteTarifa2 { get; set; }
        [DisplayName("Pensiones")]
        public string CortePension { get; set; }
    }
}
