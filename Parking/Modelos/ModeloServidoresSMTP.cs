using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloServidoresSMTP
    {

        public int ID { get; set; }
        public string Dominio { get; set; }
        [DisplayName("Servidor SMTP")]
        public string ServidorSMTP { get; set; }
        public int? Puerto { get; set; }
    }
}
