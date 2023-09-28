using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    public class SaleTeamTransactionMessage
    {
        public bool? SmsDeliveryTicket { get; set; }
        public bool? EmailDeliveryTicket { get; set; }
        public string PaymentMethod { get; set; }

        public string Email { get; set; }

        public int UserId;

        public double TotalCash;

        public int SaleId;

        public double TotalCredit;

        public String CustomerTag;

        public int InventoryId;

        public string Date;

        public int CustomerId;

        public int DeliveryId;

        public List<SaleDetail> SaleDetails;

        public List<SaleReplacement> SaleReplacements;

        public List<SalePromotion> SalePromotions;

        public PromocionData PromocionData;

        public List<PromotionCatalog> PromotionCatalog;

        public List<PromotionProductDto> PromotionProductDto;

        public List<PromotionGiftProductDto> PromotionGiftProductDto;

        public List<PromotionGiftArticleDto> PromotionGiftArticleDto;

        public List<SaleDetailsArticles> SaleDetailsArticles;

        public SaleTeamTransactionMessage()
        {
            PromotionCatalog = new List<PromotionCatalog>();
            PromotionProductDto = new List<PromotionProductDto>();
            PromotionGiftProductDto = new List<PromotionGiftProductDto>();
            PromotionGiftArticleDto = new List<PromotionGiftArticleDto>();
            SaleDetailsArticles = new List<SaleDetailsArticles>();
            PromocionData = new PromocionData();
        }
    }
}