using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Modelos
{
    public class ModeloUsuarios
    {
        [DisplayName("ID")]
        public int IDU { get; set; }
        public string Privilegios { get; set; }
        public string Usuario { get; set; }
        [Browsable(false)]
        public string  Password { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        [DisplayName("Area")]
        public string AreaPersonal{ get; set; }
        public string Puesto { get; set; }
        [DisplayName("Email")]
        public string CorreoE { get; set; }
        [Browsable(false)]
        public string CorreoP { get; set; }
        [Browsable(false)]
        public string Dominio { get; set; }
        [Browsable(false)]
        public string SMTP { get; set; }
        [Browsable(false)]
        public int Puerto    { get; set; }

    }
}
