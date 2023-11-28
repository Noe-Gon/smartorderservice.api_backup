using System;
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
        public List<PromotionTypeCatalog> PromotionTypeCatalogDto { get; set; }
    }

    public class PromotionTypeCatalog
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }

    }
}