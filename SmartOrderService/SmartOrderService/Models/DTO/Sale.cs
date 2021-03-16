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



}