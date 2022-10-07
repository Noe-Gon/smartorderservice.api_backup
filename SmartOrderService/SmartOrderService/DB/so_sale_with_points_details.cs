using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_sale_with_points_details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int saleWithPointDetailId { get; set; }
        //[ForeignKey("saleWithPointsId")]
        public int saleWithPointsId { get; set; }
        //[ForeignKey("productId")]
        public int productId { get; set; }
        public int Amount { get; set; }
        public int pointsPerUnit { get; set; }
        public int totalPoints { get; set; }
        public virtual so_sale_with_points so_sale_with_points { get; set; }
        public virtual so_product so_product { get; set; }
        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

    }
}