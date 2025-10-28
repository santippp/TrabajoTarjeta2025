using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarjetaSube
{
    public class Tiempo
    {
        public virtual DateTime Now()
        {
            return DateTime.Now;
        }
    }
}