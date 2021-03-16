using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("biusocomprobantes")]
    public partial class Biusocomprobantes
    {
        [Key]
        public int idbiusocomprobante { set; get; }

        public string idusocomprobante { set; get; }

        public string descripcion { set; get; }
    }
}