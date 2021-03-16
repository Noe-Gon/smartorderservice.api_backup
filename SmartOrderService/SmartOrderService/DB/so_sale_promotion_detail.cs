namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_promotion_detail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sale_promotionId { get; set; }

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
        public decimal? vat_tax { get; set; }

        public Single? vat_tax_rate { get; set; }
        public Single? sale_price { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_sale_promotion so_sale_promotion { get; set; }

        #region New price fields
        /// <summary>
        /// Precio base
        /// </summary>
        public decimal? base_price { get; set; }

        /// <summary>
        /// Descuento final aplicado, sea por un importe directo o por porcentaje
        /// </summary>
        public Single? discount { get; set; }

        /// <summary>
        /// Importe de descuento (puede ser cero y tener en su lugar porcentaje de descuento)
        /// </summary>
        public decimal? discount_amount { get; set; }

        /// <summary>
        /// Porcentaje de descuento
        /// </summary>
        public decimal? discount_percent { get; set; }

        /// <summary>
        /// Precio neto
        /// </summary>
        public decimal? net_price { get; set; }

        /// <summary>
        /// Precios sin impuestos
        /// </summary>
        public decimal? price_without_taxes { get; set; }

        /// <summary>
        /// Descuento sin impuestos
        /// </summary>
        public decimal? discount_without_taxes { get; set; }

        public decimal? ieps { get; set; }

        /// <summary>
        /// IEPS cuota
        /// </summary>
        public decimal? ieps_fee { get; set; }

        /// <summary>
        /// IEPS botana
        /// </summary>
        public decimal? ieps_snack { get; set; }

        /// <summary>
        /// Tasa IEPS
        /// </summary>
        public decimal? ieps_rate { get; set; }

        /// <summary>
        /// Tasa IEPS cuota
        /// </summary>
        public decimal? ieps_fee_rate { get; set; }

        /// <summary>
        /// Tasa IEPS botana
        /// </summary>
        public decimal? ieps_snack_rate { get; set; }

        /// <summary>
        /// Litros
        /// </summary>
        public decimal? liters { get; set; }
        #endregion New price fields
    }
}
