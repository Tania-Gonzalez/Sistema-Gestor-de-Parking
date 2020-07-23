using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    public partial class PARKING_DBEntities : DbContext
    {
        public PARKING_DBEntities(string CadenaConexion):base(CadenaConexion)
        {

        }
    }
}
