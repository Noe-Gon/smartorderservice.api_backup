namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_order_detail
    {
        [Key]
        public int detailId { get; set; }

        public int orderId { get; set; }

        public int productId { get; set; }

        public int amount { get; set; }

        public double price { get; set; }

        public double import { get; set; }

        public int credit_amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_order so_order { get; set; }
    }
}
