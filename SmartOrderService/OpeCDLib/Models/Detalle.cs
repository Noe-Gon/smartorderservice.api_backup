using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpeCDLib.Models
{
    public class Detalle
    {
        public int Producto { get; set; }
        public int Cantidad { get; set; }
        public int CantidadCredito { get; set; }
        public Single Precio { get; set; }
        public decimal PrecioBase { get; set; }
        public Single Descuento { get; set; }
        public decimal ImporteDesc { get; set; }
        public decimal PorcentDesc { get; set; }
        public decimal PrecioNeto { get; set; }
        public decimal PrecioSinImpuesto { get; set; }
        public decimal DesctoSinImpuesto { get; set; }
        public decimal Iva { get; set; }
        public decimal Ieps { get; set; }
        public decimal IepsCuota { get; set; }
        public decimal IepsBotana { get; set; }
        public Single TasaIva { get; set; }
        public decimal TasaIeps { get; set; }
        public decimal TasaIepsCuota { get; set; }
        public decimal TasaIepsBotana { get; set; }
        public int Litros { get; set; }

    }
}
