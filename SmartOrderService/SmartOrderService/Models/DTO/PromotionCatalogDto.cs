using System;
using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class PromotionCatalogDto
    {
        public int promotion_catalogId { get; set; }
        public int amount_workday { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string name_promotion { get; set; }
        public decimal additional_cost { get; set; }
        public decimal amount_max_customer { get; set; }
        public string name_type_promotion { get; set; }
        public int? type_promotionId { get; set; }
    }
}