namespace SmartOrderService.Models.DTO
{
    public class PromotionSaleDetailGiftArticle
    {

        public int promotion_saleId { get; set; }
        public int article_promotionalId { get; set; }
        public int amount { get; set; }
        public string name_article { get; set; }
    }
}