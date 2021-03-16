using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpeCDLib.Models
{
    public class Jornada
    {

        public Jornada() {
            Viajes = new List<Viaje>();
        }

        public int Ruta { get; set; }
        public bool Finalizada { get; set; }
        public List<Viaje> Viajes { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fin { get; set; }

    }
}
