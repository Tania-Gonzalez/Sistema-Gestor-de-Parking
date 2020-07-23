using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloParking
    {

        [Browsable(false)]
        public TimeSpan? Hora { get; set; }
        [DisplayName("Hora Entrada")]
        public string HoraEntrada  { get { return string.Format("{0:hh\\:mm\\:ss}", Hora); } }
        [DisplayName("Placa")]
        public string Placa { get; set; }
        [DisplayName("Folio")]
        public string Folio { get; set; }

    }
}
