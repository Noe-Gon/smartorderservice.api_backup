using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionGiftProductDto
    {
        public int promotion_catalogId { get; set; }
        public int productId { get; set; }
        public int amount { get; set; }
    }
}