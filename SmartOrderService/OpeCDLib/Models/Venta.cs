using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpeCDLib.Models
{
    public class Venta
    {
        public Venta() {
            Detalles = new List<Detalle>();
            Promociones = new List<Promocion>();
        }

        public int CUC { get; set; }

        public String Code { get; set; }
        public int CodOpe { get; set; }
        public bool Facturado { get; set; }
        public int WBCSaleId { get; set; }
        public List<Detalle> Detalles { get; set; }
        public List<Promocion> Promociones { get; set; }
        public int RazonDev { get; set; }

    }
}
