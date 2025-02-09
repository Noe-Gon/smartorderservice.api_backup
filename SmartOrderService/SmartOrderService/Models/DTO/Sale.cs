﻿using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;

namespace SmartOrderService.Models
{
    public class Sale
    {

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

        public IList<SaleCombo> SaleCombos { get; set; }

    }

    public class SaleAdjusmentResult
    {
        public Sale NewSale { get; set; }
        public Sale DeletedSale { get; set; }
    }

    public class SaleTeam
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

        public SaleTeam()
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