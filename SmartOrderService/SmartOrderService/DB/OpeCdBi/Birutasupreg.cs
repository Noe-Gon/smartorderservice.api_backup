using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("birutasupreg")]
    public partial class Birutasupreg
    {
        [Key]
        public decimal idrutasupreg { set; get; }
        public decimal idcliente { set; get; }

        public decimal idregistro { set; get; }

        public decimal idrutacli { set; get; }

        public int idbiusocomprobante { set; get; }

        public int factwbc { set; get; }

        public int codope { set; get; }

        public decimal idsupervisorcli { set; get; }

        public bool activo { set; get; }

        public int idtipoprod { set; get; }
    }

   
}