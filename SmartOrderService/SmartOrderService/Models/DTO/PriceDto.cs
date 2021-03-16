using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class PriceDto
    {

        public PriceDto() {

            HasDiscount = false;

        }
        public int ProductId { get; set; }
        public decimal PriceValue { get; set; }
        public bool HasDiscount { get; set; }
        public bool Status { get; set; }
        public decimal PriceBaseValue { get; set; }
        public decimal DiscountPercentValue { get; set; }
        public decimal base_price_no_tax { get; set; }
        public decimal discount_no_tax { get; set; }
        public decimal vat { get; set; }
        public decimal stps { get; set; }
        public decimal stps_fee { get; set; }
        public decimal stps_snack { get; set; }
        public decimal net_content { get; set; }
        public decimal vat_rate { get; set; }
        public decimal stps_rate { get; set; }
        public decimal stps_fee_rate { get; set; }
        public decimal stps_snack_rate { get; set; }
    }

   
}