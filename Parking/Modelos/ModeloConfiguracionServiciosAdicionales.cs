using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    class ModeloConfiguracionServiciosAdicionales
    {
        [Browsable(false)]
        public int ID { get; set; }
        [DisplayName("Servicio Adicional")]
        public string ServicioAdicional { get; set; }

        [DisplayName("Estado")]
        public string Estado
        {
            get { return Edo_; }
            set
            {
                Edo_ = value;
                if (this.Edo_ == "False") { this.Edo_ = "Inactivo"; }
                if (this.Edo_ == "True") { this.Edo_ = "Activo"; }

            }
        }
        public string Edo_;
        public decimal? Costo { get; set; }
        public int? Tiempo { get; set; }
    }
}
