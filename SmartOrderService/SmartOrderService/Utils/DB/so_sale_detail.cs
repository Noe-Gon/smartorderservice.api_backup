namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_detail
    {
        [Key]
        public int detailId { get; set; }

        public int saleId { get; set; }

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

        public decimal? base_price_no_tax { get; set; }

        public decimal? discount_no_tax { get; set; }

        public decimal? vat { get; set; }

        public decimal? vat_total { get; set; }

        public decimal? stps { get; set; }

        public decimal? stps_fee { get; set; }

        public decimal? stps_snack { get; set; }

        public decimal? net_content { get; set; }

        public decimal? vat_rate { get; set; }

        public decimal? stps_rate { get; set; }

        public decimal? stps_fee_rate { get; set; }

        public decimal? stps_snack_rate { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_sale so_sale { get; set; }
    }
}
