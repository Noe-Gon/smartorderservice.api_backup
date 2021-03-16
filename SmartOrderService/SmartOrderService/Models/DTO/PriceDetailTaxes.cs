using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class PriceDetailTaxes
    {
        public decimal base_price_no_tax { get; set; }
        public decimal discount_no_tax { get; set; }
        public decimal vat { get; set; }
        public decimal vat_total { get; set; }
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