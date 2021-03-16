using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("birutascli")]
    public class Birutascli
    {
        [Key]
        public decimal idrutacli { set; get; }

        public decimal idcliente { set; get; }

        public decimal idbiruta { set; get; }

        public bool credito { set; get; }
    }
}