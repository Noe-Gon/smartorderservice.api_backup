using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class SalePromotionResponse
    {
        public int PromotionId;
        public int Amount;
        public List<SalePromotionDetailProductResponse> DetailProduct;
    }
}