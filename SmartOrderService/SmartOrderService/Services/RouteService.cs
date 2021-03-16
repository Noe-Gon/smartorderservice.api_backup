using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;

namespace SmartOrderService.Services
{
    public class RouteService
    {
        SmartOrderModel db = new SmartOrderModel();
        public List<RouteDto> GetByBranch(int branchId)
        {
            List<RouteDto> routesDto = new List<RouteDto>();
            var routes = db.so_route.Where(r => r.branchId == branchId && r.status);
            if(routes.Any())
                routesDto = Mapper.Map<List<RouteDto>>(routes);
            else
                throw new KeyNotFoundException("No se encontraron rutas para el cedisId=" + branchId);

            return routesDto;
        }

        public List<RouteDto> GetByUserNoticeRecharge(int userNoticeRechargeId)
        {
            List<RouteDto> routesDto = new List<RouteDto>();
            var routes = db.so_user_notice_recharge_route.
                Join(db.so_route, unrr => unrr.routeId, r => r.routeId, (unrr, r) => new { unrr, r }).
                Where(r => r.unrr.user_notice_rechargeId == userNoticeRechargeId && r.unrr.status && r.r.status).Select(r => r.r);
            if(routes.Any())
                routesDto = Mapper.Map<List<RouteDto>>(routes);
            else
                throw new KeyNotFoundException("No se encontraron rutas para el userId=" + userNoticeRechargeId);

            return routesDto;
        }

        public List<RouteDto> GetByBranch(int branchId, int type)
        {
            List<RouteDto> routesDto = new List<RouteDto>();
            var routes = db.so_route.Join(db.so_user_route, r => r.routeId, ur => ur.routeId, (r, ur) => new { r, ur }).
                Where(x => x.r.branchId == branchId && x.r.status && x.ur.status).
                Join(db.so_user, x => x.ur.userId, u => u.userId, (x, u) => new { x.r, u }).
                Where(y => y.u.status && y.u.type == type).Select(y => y.r);

            if (routes.Any())
                routesDto = Mapper.Map<List<RouteDto>>(routes);
            else
                throw new KeyNotFoundException("No se encontraron rutas para el cedisId=" + branchId);

            return routesDto;
        }
    }
}