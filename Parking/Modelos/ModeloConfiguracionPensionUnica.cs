using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloConfiguracionPensionUnica
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        
        public int ID_Tipo_PensionU { get; set; }
        public string Tarifa{ get; set; }
        public string ToleranciaEntrada { get; set; }
        public string ToleranciaSalida { get; set; }
        public TimeSpan? HoraInicio { set { horainicio_ = value; } }
        private TimeSpan? horainicio_;
        public TimeSpan? HoraFin {  set { horafin_ = value; } }
        private TimeSpan? horafin_;
        public string Horario { get {return string.Format("{0:hh\\:mm}", horainicio_)+" - "+ string.Format("{0:hh\\:mm}", horafin_); } }
        public bool? PensionActiva { get; set; }
        public decimal? Precio { get; set; }

    }
    public class ModeloConfiguracionTipoPensionUnica
    {
        public  int ID { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin{ get; set; }
        public int ToleranciaIn { get; set; }
        public int ToleranciaOut { get; set; }

    }
    public class ModeloDtPensionUnica
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        [DisplayName("Tipo")]
        public string Tipo{ get; set; }




        [DisplayName("Hora Inicio")]
        public string  HoraInicio{ get { return string.Format("{0:hh\\:mm}", HoraI); }}
        
        [Browsable(false)]
        public TimeSpan? HoraI { get; set; }

        [DisplayName("Hora Final")]
        public string HoraFinal{ get { return string.Format("{0:hh\\:mm}", HoraF); }}

        [Browsable(false)]
        public TimeSpan? HoraF { get; set; }







        [DisplayName("Toleracia Llegada")]
        public string Tin{ get; set; }
        [DisplayName("Tolerancia Fuera")]
        public string Tout { get; set; }

        [DisplayName("Estado")]
        public string Edo
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
        public decimal? Precio { get; set; }

    }
}
