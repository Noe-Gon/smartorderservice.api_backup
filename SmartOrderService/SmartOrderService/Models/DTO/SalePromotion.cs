using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SalePromotion
    {
        public int PromotionId;
        public int Amount;
        public bool Status;
        public List<SalePromotionDetailProduct> DetailProduct;
    }
}