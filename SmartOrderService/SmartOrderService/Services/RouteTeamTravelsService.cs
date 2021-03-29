using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class RouteTeamTravelsService
    {
        private RoleTeamService roleTeamService = new RoleTeamService();

        private SmartOrderModel db = new SmartOrderModel();

        public int getTravelStatusByInventoryId(int inventoryId)
        {
            var userRouteTeamTravel = db.so_route_team_travels.Where(i => i.inventoryId == inventoryId).FirstOrDefault();
            if (userRouteTeamTravel == null)
            {
                throw new InventoryNotFoundException();
            }
            return userRouteTeamTravel.travelStatus;
        }
    }
}