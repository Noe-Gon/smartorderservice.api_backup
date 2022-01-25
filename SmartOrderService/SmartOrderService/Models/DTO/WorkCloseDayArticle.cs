using System.Collections.Generic;

namespace SmartOrderService.Models.DTO
{
    public class WorkCloseDayArticle
    {
        public int routeId { get; set; }
        public int branchId { get; set; }
        public int article_promotionalId { get; set; }
        public int amount { get; set; }
    }
}