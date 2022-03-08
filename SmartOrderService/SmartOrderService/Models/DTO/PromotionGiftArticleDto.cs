using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionGiftArticleDto
    {
        public int promotion_catalogId { get; set; }
        public int article_promotionalId { get; set; }
        public int amount { get; set; }
    }
}