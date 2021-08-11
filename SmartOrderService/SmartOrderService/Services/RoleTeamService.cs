using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class RoleTeamService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public ERolTeam GetUserRole(int userId)
        {
            so_route_team userRoleTeam = db.so_route_team.Where(i => i.userId == userId).FirstOrDefault();
            if (userRoleTeam == null)
            {
                return ERolTeam.SinAsignar;
            }
            return (ERolTeam)userRoleTeam.roleTeamId;
        }
    }
}