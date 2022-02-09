using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SaleDetail
    {
        public decimal PriceValue;
        public int Amount;
        public int CreditAmount;
        public decimal Import;
        public int ProductId;

        #region New price fields
        /// <summary>
        /// Precio
        /// </summary>
        public Single? price { get; set; }

        /// <summary>
        /// IVA
        /// </summary>
        public decimal? vat { get; set; }

        /// <summary>
        /// Tasa IVA
        /// </summary>
        public Single? vat_rate { get; set; }

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

    public class SaleDetailLoyalty 
    {
        public string code { get; set; }
        public string name { get; set; }
        public string points { get; set; }
    }
}