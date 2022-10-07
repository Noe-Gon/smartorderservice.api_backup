using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public partial class so_sale_with_points
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int saleWithPointsId { get; set; }
        //[ForeignKey("saleId")]
        public int saleId { get; set; }
        public DateTime? date { get; set; }
        //[ForeignKey("userId")]
        public int userId { get; set; }
        //[ForeignKey("customerId")]
        public int customerId { get; set; }
        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public virtual so_sale so_sale { get; set; }
        public virtual so_user so_user { get; set; }
        public virtual so_customer so_customer { get; set; }

    }
}