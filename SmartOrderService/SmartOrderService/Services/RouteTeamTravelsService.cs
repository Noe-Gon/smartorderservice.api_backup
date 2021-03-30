using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public void SetClosingStatusRoutTeamTravels(Guid workDayId)
        {
            var routeTeamTravels = db.so_route_team_travels.Where(e => e.work_dayId.Equals(workDayId));
            foreach (var routeTravel in routeTeamTravels)
            {
                routeTravel.travelStatus = 4;
            }
            db.SaveChanges();
        }

        public bool CheckTravelsClosingStatus(int userId, DateTime date)
        {
            // Join entre la columna inventory y route team travels


            var travelStates = db.so_inventory.Join(
                db.so_route_team_travels,
                inventario => inventario.inventoryId,
                routeTeam => routeTeam.inventoryId,
                (inventario, routeTeam) => new
                {
                    state = routeTeam.travelStatus,
                    userId = inventario.userId,
                    date = inventario.date
                }
                ).Where(s => s.userId.Equals(userId) 
                && s.state < 3 
                && DbFunctions.TruncateTime(s.date) == DbFunctions.TruncateTime(date));
            //si existe algun registro con el id del usuario y un codio menor que 3 (no acabado)
            //se envia un false para no permitir el cierre del viaje
            if (travelStates.Any())
            {
                return false;
            }
            return true;
        }

    }
}