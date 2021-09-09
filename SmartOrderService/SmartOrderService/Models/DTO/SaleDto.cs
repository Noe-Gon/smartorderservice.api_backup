using using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class SaleDto
    {
        public int UserId { get; set; }
        public double TotalCash { get; set; }
        public int SaleId { get; set; }
        public double TotalCredit { get; set; }
        public string CustomerTag { get; set; }
        public int InventoryId { get; set; }
        public string Date { get; set; }
        public int CustomerId { get; set; }
        public int DeliveryId { get; set; }
        public List<SaleDetail> saleDetails { get; set; }
        public List<SaleReplacement> saleReplacements { get; set; }
        public List<SalePromotion> salePromotion { get; set; }
    }
}