using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionProductDto
    {
        public int promotion_catalogId { get; set; }
        public int productId { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
    }
}