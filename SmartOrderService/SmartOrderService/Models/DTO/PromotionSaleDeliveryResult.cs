using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionSaleDeliveryResult
    {
        public List<SaleWithPromotion> SaleWithPromotion { get; set; }
        public List<RelSalePromotion> RelSalePromotion { get; set; }
        public List<PromotionSaleDetailPromotionProduct> PromotionSaleDetailPromotionProduct { get; set; }
        public List<PromotionSaleDetailGiftProduct> PromotionSaleDetailGiftProduct { get; set; }
        public List<PromotionSaleDetailGiftArticle> PromotionSaleDetailGiftArticle { get; set; }
    }
}