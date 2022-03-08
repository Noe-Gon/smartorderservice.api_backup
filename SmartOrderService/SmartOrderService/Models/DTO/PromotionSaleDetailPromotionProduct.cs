namespace SmartOrderService.Models.DTO
{
    public class PromotionSaleDetailPromotionProduct
    {

        public int promotion_saleId { get; set; }
        public int productId { get; set; }
        public int amount { get; set; }
        public string name_product { get; set; }
    }
}