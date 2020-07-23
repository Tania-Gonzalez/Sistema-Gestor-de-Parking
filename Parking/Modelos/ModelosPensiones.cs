using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
     public class ModeloPensiones
    {
       
        public int ID { get; set; }
        public string Folio { get; set; }
        [DisplayName("Placa")]
        public string Placa { get; set; }
        [DisplayName("Marca")]
        public string Marca { get; set; }
        [DisplayName("Color")]
        public string Color { get; set; }
        [DisplayName("Estado de Pago")]     

        public string EdoPago
        {
            get { return EdoPago_; }
            set
            {
                EdoPago_ = value;
                if (this.EdoPago_=="False") { this.EdoPago_ = "Pendiente"; }
                if (this.EdoPago_ == "True") { this.EdoPago_ = "Pagado"; }
                if (this.EdoPago_ == "") { this.EdoPago_ = "Inactiva"; }

            }
        }
        public string EdoPago_;

        [DisplayName("Plan")]
        public string Pensn_Tipo { get; set; }
        [DisplayName("Inicio Pension")]
        public string InicioPension { get; set; }
        [DisplayName("Termino Pension")]
        public string TerminoPension { get; set; }
        [DisplayName("Costo Regular")]
        public string CostoRegular { get; set; }
        [DisplayName("Cobrado Por")]
        public string CobradoPor { get; set; }
    }
    public class ModeloBitacoraPensiones
    {
        public int ID { get; set; }

        public string Folio { get; set; }

        public string Placa { get; set; }
        [DisplayName("Fecha Entrada")]
        public DateTime FechaEntrada { get; set; }
        [DisplayName("Hora Entrada")]
        public TimeSpan? HoraEntrada { get; set; }
        [DisplayName("Hora Salida")]
        public TimeSpan? HoraSalida { get; set; }
        [DisplayName("Fecha Salida")]
        public DateTime? FechaSalida { get; set; }


    }
}
