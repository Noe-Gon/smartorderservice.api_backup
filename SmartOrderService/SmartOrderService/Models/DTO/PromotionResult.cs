using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionResult
    {
        public List<PromotionGiftArticleCatalogDto> PromotionGiftArticleCatalogDto { get; set; }
        public List<PromotionCatalogDto> PromotionCatalogDto { get; set; }
        public List<PromotionProductDto> PromotionProductDto { get; set; }
        public List<PromotionGiftProductDto> PromotionGiftProductDto { get; set; }
        public List<PromotionGiftArticleDto> PromotionGiftArticleDto { get; set; }
    }
}