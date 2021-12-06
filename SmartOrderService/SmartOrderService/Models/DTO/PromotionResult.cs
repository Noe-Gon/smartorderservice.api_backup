using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionResult
    {
        public int SaleProductId { get; set; }
        public int AmountSaleProduct { get; set; }
        public int GiftProductId { get; set; }
        public int AmountGiftProduct { get; set; }
        public int GiftArticleId { get; set; }
        public int AmountGiftArticle { get; set; }
        public int AvailableGiftArticle { get; set; }
        public string NamePromotion { get; set; }
        public string Validity { get; set; }
        public string TypePromotion { get; set; }
        public decimal AmountMaxCustomer { get; set; }
        public int TotalAvailablePromotions { get; set; }
    }
}