using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryDetailDto
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int CreditAmount { get; set; }
        public bool Status { get; set; }
        public Single? price { get; set; }
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
    }
}