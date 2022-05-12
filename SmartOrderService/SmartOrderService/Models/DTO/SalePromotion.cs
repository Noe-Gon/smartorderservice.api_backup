using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SalePromotion
    {
        public SalePromotion()
        {

        }

        public SalePromotion(SalePromotion sale)
        {
            this.PromotionId = sale.PromotionId;
            this.Amount = sale.Amount;
            this.Status = sale.Status;
            this.DetailProduct = sale.DetailProduct;
        }

        public int PromotionId;
        public int Amount;
        public bool Status;
        public List<SalePromotionDetailProduct> DetailProduct;
    }
}