using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class Configuracion_WorkByCloud
    {
        [Key]
        public int wbcConfId { get; set; }
        [StringLength(100)]
        public string BillPocket_TokenUsuario { get; set; }
        public int branchId { get; set; }
        public DateTime? createdon { get; set; }
        public DateTime? modifiedon { get; set; }
        public virtual so_branch so_user { get; set; }
    }
}