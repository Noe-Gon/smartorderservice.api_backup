using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryPromotionDetailArticleDto
    {
        public int ArticleId { get; set; }

        public bool Status { get; set; }

        public double Amount { get; set; }
    }
}