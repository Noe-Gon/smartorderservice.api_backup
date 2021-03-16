namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_product_tax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productId { get; set; }

        public int vat_apply { get; set; }

        public int stps_apply { get; set; }

        public int stps_fee_apply { get; set; }

        public int stps_snack_apply { get; set; }

        public decimal trade_volume { get; set; }

        public decimal pieces { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [StringLength(250)]
        public string description_tax { get; set; }

        [StringLength(250)]
        public string unit { get; set; }

        public virtual so_product so_product { get; set; }
    }
}
