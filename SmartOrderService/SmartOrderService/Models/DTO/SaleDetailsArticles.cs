using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class SaleDetailsArticles
    {
        public int article_promotionalId { get; set; }
        public int amount { get; set; }
        public decimal import { get; set; }
        public decimal priceValue { get; set; }
    }
}