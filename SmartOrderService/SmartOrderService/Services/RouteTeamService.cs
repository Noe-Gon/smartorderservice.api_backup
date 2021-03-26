﻿using SmartOrderService.CustomExceptions;
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
        private RoleTeamService roleTeamService = new RoleTeamService();

        public bool checkCurrentTravelState(int userId)
        {
            ERolTeam userRole = roleTeamService.getUserRole(userId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }
            int inventoryState = getInventoryState(userId);
            if ((inventoryState == 0 && userRole == ERolTeam.Impulsor))
            {
                return true;
            }
            if (((inventoryState == 1 || inventoryState == 2) && userRole == ERolTeam.Impulsor))
            {
                return false;
            }
            if (inventoryState == 1)
            {
                return true;
            }
            return false;
        }

        public bool checkDriverWorkDay(int userId)
        {
            int routeId = searchRouteId(userId);
            so_route_team driver = searchDriverByRouteId(routeId);
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


        public int searchRouteId(int userId)
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

        private int getDriverIdByAssistant(int assistantId)
        {
            int assistantRouteId = searchRouteId(assistantId);
            int driverId = searchDriverByRouteId(assistantRouteId).userId;
            return driverId;
        }

        private int getInventoryState(int userId)
        {
            DateTime today = DateTime.Today;
            int inventoryState = getInventoryState(userId,today);
            return inventoryState;
        }

        private so_route_team searchDriverByRouteId(int routeId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.routeId == routeId
                && i.roleTeamId == (int)ERolTeam.Impulsor
                ).FirstOrDefault();
            return routeTeam;
        }

        public so_work_day GetWorkdayByUserAndDate(int userId, DateTime date)
        {
            return db.so_work_day.Where(
                i => i.userId == userId
                && DbFunctions.TruncateTime(i.date_start) == DbFunctions.TruncateTime(date)
                ).FirstOrDefault();
        }

        public int getInventoryState(int userId, DateTime date)
        {
            if (date == null)
            {
                date = DateTime.Today;
            }
            date = Convert.ToDateTime("31/01/2020");
            userId = SearchDrivingId(userId);
            var inventory = db.so_inventory.Where(i => i.userId == userId
            && DbFunctions.TruncateTime(i.date) == DbFunctions.TruncateTime(date)
            ).ToList();
            if (!inventory.Any())
            {
                throw new InventoryEmptyException();
            }
            return inventory.FirstOrDefault().state;
        }

        private int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = db.so_route_team.Where(i => i.userId == actualUserId).ToList().FirstOrDefault();
            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = db.so_route_team.Where(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor).ToList().FirstOrDefault().userId;
            return DrivingId;
        }
    }
}