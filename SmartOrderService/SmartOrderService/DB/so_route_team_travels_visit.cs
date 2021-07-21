using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_route_team_travels_visit
    {
        public int binnacleId { get; set; }
        public so_binnacle_visit So_Binnacle_Visit { get; set; }

        #region route team travels ID
        public int inventoryId { get; set; }
        public so_inventory So_Inventory { get; set; }

        public int routeId { get; set; }
        public so_route So_Route { get; set; }

        public Guid workDayId { get; set; }
        #endregion

        public so_route_team_travels so_route_team_travels { get; set; }

    }
}