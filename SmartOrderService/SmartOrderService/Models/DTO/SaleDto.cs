using Newtonsoft.Json;
using System.Collections.Generic;

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
        public int State { get; set; }
        public bool Status { get; set; }
        public string PaymentMethod { get; set; }
        public string CreateDate { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("IsSignature")]
        public bool IsSignature { get; set; }

        [JsonProperty("cause_no_signature")]
        public string CauseNoSignature { get; set; }

        public List<SaleDetailResponse> SaleDetails { get; set; }
        public List<SaleDetailsArticles> SaleDetailsArticles { get; set; }
        public List<SaleReplacement> SaleReplacements { get; set; }
        public List<SalePromotionResponse> SalePromotion { get; set; }
        public List<SalePromotionCatalog> SalePromotionCatalog { get; set; }
        public List<SaleDetailsLoyalty> SaleDetailsLoyalty { get; set; }
    }

    public class SalePromotionCatalog
    {
        [JsonProperty("promotion_catalogId")]
        public int PromotionCatalogId { get; set; }

        [JsonProperty("amountSale")]
        public int AmountSale { get; set; }

    }

    public class SaleDetailsLoyalty
    {
        [JsonProperty("code")]
        public string Code;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("Amount")]
        public int Amount;

        [JsonProperty("points")]
        public int Points;
    }
}