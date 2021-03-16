namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_inventory_detail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int inventoryId { get; set; }

        public int amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }
        public Single price { get; set; }
        public decimal? base_price { get; set; }
        public Single? discount { get; set; }
        public decimal? discount_amount { get; set; }
        public decimal? discount_percent { get; set; }
        public decimal? net_price { get; set; }
        public decimal? price_without_taxes { get; set; }
        public decimal? discount_without_taxes { get; set; }
        public decimal? vat { get; set; }
        public decimal? ieps { get; set; }
        public decimal? ieps_fee { get; set; }
        public decimal? ieps_snack { get; set; }
        public Single? vat_rate { get; set; }
        public decimal? ieps_rate { get; set; }
        public decimal? ieps_fee_rate { get; set; }
        public decimal? ieps_snack_rate { get; set; }
        public decimal? liters { get; set; }

        public virtual so_inventory so_inventory { get; set; }

        public virtual so_product so_product { get; set; }
    }
}
