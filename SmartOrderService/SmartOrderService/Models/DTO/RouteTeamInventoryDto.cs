using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class RouteTeamInventoryDto
    {
        public int ProductId { get; set; }
        public int AvailableAmount { get; set; }
    }

    public class RouteTeamInventoryArticle
    {
        public int ArticleId { get; set; }
        public int AvailableAmount { get; set; }
    }

    public class GetRouteTeamInventory
    {
        public List<RouteTeamInventoryDto> Products { get; set; }

        public List<RouteTeamInventoryArticle> Articles { get; set; }
    }
}