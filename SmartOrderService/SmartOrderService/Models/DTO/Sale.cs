using SmartOrderService.Models.DTO;
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

    }

    public class SaleTeam
    {
        public bool? SmsDeliveryTicket { get; set; }
        public bool? EmailDeliveryTicket { get; set; }
        public string PaymentMethod { get; set; }
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

    }

}