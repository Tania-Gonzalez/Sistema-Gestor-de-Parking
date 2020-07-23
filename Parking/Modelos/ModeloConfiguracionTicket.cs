using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloConfiguracionTicket
    {
        public bool? IncluirLogo { get; set; }
        public bool? TamañoPapel { get; set; }
        public string Estacionamiento { get; set; }
        public string Direccion { get; set; }
        public string RazonSocial { get; set; }
        public string Telefono { get; set; }
        public string RFC { get; set; }
        public string DatosEntrada { get; set; }
        public string DatosSalida { get; set; }
        public string DatosPerdido { get; set; }
        public string DatosCortesia { get; set; }
        public string DatosConvenio { get; set; }
        public string DatosCancelado { get; set; }
        public string DatosPension { get; set; }
        public int CantidadTicketEntrada { get; set; }
        public int CantidadTicketSalida { get; set; }
        public byte[] Imagen{ get; set; }
        public string Desarrollador { get; set; }
    }
}
