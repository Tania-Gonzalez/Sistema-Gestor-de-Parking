using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
     public class ModeloConfiguracionGeneral
    {
        public bool PeriodoCobro { get; set; }
        public int ToleranciaIngreso { get; set; }
        public int ToleranciaProxTarifa { get; set; }
        public bool TicketCancelado { get; set; }
        public int TCancel { get; set; }
        public bool TicketCortesia { get; set; }
        public int Tcortesia { get; set; }

        public bool TicketPerdido { get; set; }

        public int TPerdido { get; set; }
        public int Multa { get; set; }
        public int Capacidad{ get; set; }

        public bool BuscarVehiculo { get; set; }
        public bool ModoCobro { get; set; }

        public int Limpiar { get; set; }
        public string DireccionFactura{ get; set; }

    }
}
