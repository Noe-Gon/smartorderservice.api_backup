using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    [Table("axrefsitios")]
    public partial class AxRefSitios
    {
        [Key]
        public string sitio { set; get; }

        public int idcedis { set; get; }
        
    }
}