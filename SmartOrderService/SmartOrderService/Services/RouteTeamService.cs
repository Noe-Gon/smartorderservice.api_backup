using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class RouteTeamService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private InventoryService inventoryService = new InventoryService();

        public bool checkCurrentTravelState(int userId)
        {
            int inventoryState = getInventoryState(userId);
            if (inventoryState == InventoryService.INVENTORY_OPEN)
            {
                return true;
            }
            if (inventoryState == InventoryService.INVENTORY_AVAILABLE || inventoryState == InventoryService.INVENTORY_CLOSED)
            {
                return false;
            }
            throw new Exception();
        }

        public bool checkDriverWorkDay(int userId)
        {
            int routeId = searchRouteId(userId);
            so_route_team driver = searchDriverId(routeId);
            if (driver.userId == userId)
            {
                throw new NotSupportedException();
            }
            if (GetWorkdayByUserAndDate(driver.userId, DateTime.Today) == null)
            {
                return false;
            }
            return true;
        }

        private int getInventoryState(int userId)
        {
            DateTime today = DateTime.Today;
            int inventoryState = inventoryService.getInventoryState(userId,today);
            return inventoryState;
        }

        private int searchRouteId(int userId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.userId == userId
                ).FirstOrDefault();
            if (routeTeam == null)
            {
                throw new RelatedDriverNotFoundException(userId);
            }
            return routeTeam.routeId;
        }

        private so_route_team searchDriverId(int routeId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.routeId == routeId
                && i.roleTeamId == (int)ERolTeam.Impulsor
                ).FirstOrDefault();
            return routeTeam;
        }

        private so_work_day GetWorkdayByUserAndDate(int userId, DateTime date)
        {
            return db.so_work_day.Where(
                i => i.userId == userId
                && DbFunctions.TruncateTime(i.date_start) == DbFunctions.TruncateTime(date)
                ).FirstOrDefault();
        }

    }
}