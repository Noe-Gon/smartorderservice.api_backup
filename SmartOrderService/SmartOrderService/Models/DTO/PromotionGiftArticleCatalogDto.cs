using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionGiftArticleCatalogDto
    {
        public int article_promotionalId { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int amount { get; set; }
        public bool sell_allowed { get; set; }
        public string barcode { get; set; }
    }
}