using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("birutastipos")]
    public partial class BiRutasTipos
    {
        [Key]
        public decimal idrutatipo { set; get; }
        public decimal idbiruta { set; get; }

        public int idtipoprod { set; get; }

        public bool activo { set; get; }
        
    }
}