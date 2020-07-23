using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloHitorialTickets
    {
        public int ID { get; set; }
        public string Folio { get; set; }
        public DateTime? FechaEntrada { get; set; }
        public TimeSpan? HoraEntrada { get; set; }

        public string Placas { get; set; }
        public string TipoCobro { get; set; }
        public string Parqueo { get; set; }
        public string Importe { get; set; }
        public string Expedido { get; set; }
        public string PensionU { get; set; }
        public string ServicioAd { get; set; }
        public string ModoTicket { get; set; }
    }
}
