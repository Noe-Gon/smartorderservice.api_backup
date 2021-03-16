using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("birutas")]
    public partial class Birutas
    {
        [Key]
        public decimal idbiruta { set; get; }

        public decimal idruta { set; get; }

        public int idcedis { set; get; }

    }
}