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
        private RouteTeamTravelsService routeTeamTravelsService = new RouteTeamTravelsService();

        public bool CheckCurrentTravelState(int userId)
        {
            try
            {
                ERolTeam userRole = roleTeamService.GetUserRole(userId);
                if (userRole == ERolTeam.SinAsignar)
                {
                    return true;
                }
                

                int inventoryState = GetInventoryState(userId, DateTime.Today);

                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process

                if ((inventoryState == 0 && userRole == ERolTeam.Impulsor))
                {
                    CheckOpenedTravalers(userId);
                    return true;
                }
                if (((inventoryState == 1 || inventoryState == 2) && userRole == ERolTeam.Impulsor))
                {
                    throw new InventoryNotClosedByUserException("El viaje anterior no ha sido cerrado por otro usuario");
                }
                if (inventoryState == 1)
                {
                    CheckIfCurrentTravelsIsNewByUser(userId);

                    return true;
                }
                if (inventoryState == 0 && userRole == ERolTeam.Ayudante)
                    throw new InventoryNotOpenException();
                return false;
            }
            catch (InventoryInProgressException)
            {
                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process

                throw new InventoryInProgressException();
            }
            catch (InventoryEmptyException)
            {
                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process

                throw new InventoryEmptyException();
            }

        }

        public void CheckIfCurrentTravelsIsNewByUser(int userId)
        {
            InventoryService inventoryService = new InventoryService();

            var impulsorId = SearchDrivingId(userId);
            var inventory = inventoryService.GetCurrentInventory(impulsorId, DateTime.Today);
            var workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            //Verificar que en el inventario actual yo no este
            var travel = db.so_route_team_travels_employees
                .Where(x => x.inventoryId == inventory.inventoryId && x.work_dayId == workDay.work_dayId
                    && x.userId == userId)
                .FirstOrDefault();

            if (travel != null)
                throw new InventoryNotClosedByUserException("El viaje anterior no ha sido cerrado por otro usuario");
        }

        public void CheckOpenedTravalers(int impulsorId)
        {
            var workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            var traval = db.so_route_team_travels_employees
                .Where(x => x.work_dayId == workDay.work_dayId && x.active)
                .FirstOrDefault();

            if (traval == null)
                return;

            throw new InventoryNotClosedException();
        }
        public void CallLoadInventoryProcess(int userId)
        {
            var inventoryService = new InventoryService();
            int impulsorId = SearchDrivingId(userId);
            var routeTeam = db.so_route_team.Where(x => x.userId == impulsorId).First();
            var route = db.so_route.Where(x => x.routeId == routeTeam.routeId).First();

            inventoryService.CallLoadInventoryProcess(impulsorId, route.so_branch.code, route.code, null);
        }

        public List<int> GetTeamIds(int userId)
        {
            int routeId = searchRouteId(userId);
            return db.so_route_team
                .Where(s => s.routeId.Equals(routeId))
                .Select(s => s.userId).ToList();
        }

        public bool checkDriverWorkDay(int userId)
        {
            /*
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.Impulsor)
            {
                try
                {
                    var userWorkDay = GetWorkdayByUserAndDate(userId, DateTime.Today);
                }
                catch (WorkdayNotFoundException e)
                {
                    return true;
                }
                return false;
            }
            if (userRole == ERolTeam.Ayudante)
            {
                try
                {
                    int driverId = getDriverIdByAssistant(userId);
                    var userWorkDay = GetWorkdayByUserAndDate(driverId, DateTime.Today);
                }
                catch (WorkdayNotFoundException e)
                {
                    return false;
                }
                return true;
            }
            */
            return true;
        }

        public bool CheckTravelClosingStatus(int userId, int inventoryId)
        {
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }

            //Start Load Inventory Process OPCD
            var inventoryService = new InventoryService();
            int impulsorId = SearchDrivingId(userId);
            var routeTeam = db.so_route_team.Where(x => x.userId == impulsorId).First();
            var route = db.so_route.Where(x => x.routeId == routeTeam.routeId).First();
            var inventory = db.so_inventory.Where(x => x.inventoryId == inventoryId).FirstOrDefault();
            inventoryService.CallLoadInventoryProcess(impulsorId, route.so_branch.code, route.code, null);
            //End Load Inventory Process

            //Si es de Viaje no sincronizado devolver true
            var workDay = GetWorkdayByUserAndDate(impulsorId, inventory.date);
            var isInTravelsemployees = db.so_route_team_travels_employees
                .Where(x => x.userId == userId && x.inventoryId == inventoryId && x.work_dayId == workDay.work_dayId)
                .FirstOrDefault() != null;

            if (isInTravelsemployees)
                return true;
            //Fin de validación de viaje no sincronizado

            if (userRole == ERolTeam.Impulsor)
            {
                if (GetInventoryState(userId,DateTime.Today) == 1 && routeTeamTravelsService.getTravelStatusByInventoryId(inventoryId) == EInventoryTeamStatus.InventarioCerradoPorAsistente)
                {
                    return true;
                }
                return false;
            }
            if (GetInventoryState(userId, DateTime.Today) == 1 && routeTeamTravelsService.getTravelStatusByInventoryId(inventoryId) == EInventoryTeamStatus.InventarioAbiertoPorAyudante)
            {
                return true;
            }
            return false;
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

        public so_work_day GetWorkdayByUserAndDate(int userId, DateTime date)
        {
            so_work_day workday =  db.so_work_day.Where(
                i => i.userId == userId
                && DbFunctions.TruncateTime(i.date_start) == DbFunctions.TruncateTime(date)
                ).FirstOrDefault();
            if (workday == null)
            {
                throw new WorkdayNotFoundException("No se encontro la jornada para el usuario " + userId + " y el dia " + date);
            }
            return workday;
        }

        public int GetInventoryState(int userId, DateTime date)
        {
            InventoryService inventoryService = new InventoryService();
            if (date == null)
            {
                date = DateTime.Today;
            }
            userId = SearchDrivingId(userId);
            var inventory = inventoryService.GetCurrentInventory(userId,date);
            return inventory.state;
        }

        public bool CheckWorkDayClosingStatus(int userId)
        {
            
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }
            /*if (userRole == ERolTeam.Impulsor)
            {
                var userWorkDay = GetWorkdayByUserAndDate(userId, DateTime.Today);
                if (userWorkDay == null)
                {
                    throw new WorkdayNotFoundException();
                }
                if (userWorkDay.date_end == null && routeTeamTravelsService.CheckTravelsClosingStatus(userId,DateTime.Today))
                {
                    return true;
                }
                return false;
            }
            return true;
            */
            int impulsorId = SearchDrivingId(userId);
            var workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            int userTravel = db.so_route_team_travels_employees
                .Where(x => x.work_dayId == workDay.work_dayId && x.active)
                .Count();
            if (userTravel > 0)
            {
                return false;
            }
            return true;
        }

        public int getDriverIdByAssistant(int assistantId)
        {
            int assistantRouteId = searchRouteId(assistantId);
            int driverId = searchDriverByRouteId(assistantRouteId).userId;
            return driverId;
        }

        public bool IsImpulsor(int userId)
        {
            int actualUserId = userId;
            try
            {
                int driverId = SearchDrivingId(userId);
                if (actualUserId == driverId)
                {
                    return true;
                }
                return false;
            }
            catch (RelatedDriverNotFoundException e)
            {
                return false;
            }
        }

        public int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = db.so_route_team.Where(i => i.userId == actualUserId).ToList().FirstOrDefault();
            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = db.so_route_team.Where(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor).ToList().FirstOrDefault().userId;
            return DrivingId;
        }

        private so_route_team searchDriverByRouteId(int routeId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.routeId == routeId
                && i.roleTeamId == (int)ERolTeam.Impulsor
                ).FirstOrDefault();
            return routeTeam;
        }

    }
}