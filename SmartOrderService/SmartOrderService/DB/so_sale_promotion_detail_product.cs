using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_sale_promotion_detail_product
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sale_promotion_detail_productId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productId { get; set; }

        public int amount { get; set; }

        public double price { get; set; }

        public double import { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_sale_promotion so_sale_promotion { get; set; }
    }
}