using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class RelSalePromotion
    {

        public int promotion_saleId { get; set; }
        public int saleId { get; set; }
        public int amountSale { get; set; }
        public int promotion_catalogId { get; set; }
        public decimal additional_cost { get; set; }
        public string name_promotion { get; set; }
        public string name_type_promotion { get; set; }
    }
}