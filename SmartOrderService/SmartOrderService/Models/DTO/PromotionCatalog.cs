using System;
using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionCatalog
    {
        public int promotion_catalogId { get; set; }
        public decimal additional_cost { get; set; }
        public decimal amountSale { get; set; }
    }
}