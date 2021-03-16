using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("biregistros")]
    public partial class Biregistros
    {
        [Key]
        public decimal idregistro { set; get; } 

        public string registro { set; get; }

        public string razonsoc { set; get; }

        public string direccion { set; get; }
    }
}