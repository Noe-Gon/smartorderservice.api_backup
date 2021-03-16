using OpeCDLib.Models;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class BinnacleVisitService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public List<Visita> getVisitByLastSync(string BranchCode, string UserCode, DateTime LastSync)
        {
            var user = db.so_user.Where(
                u => u.code.Equals(UserCode)
                && u.so_branch.code.Equals(BranchCode)
                && u.status
            ).FirstOrDefault();

            if (user == null)
                throw new NoUserFoundException();

            var visitsDB =
                db.so_binnacle_visit
                .Where(
                    bv => bv.userId.Equals(user.userId)
                    & bv.createdon > LastSync
                    && bv.status
                ).ToList();

            var route = user.so_user_route.
                        Join(db.so_route, ur => ur.routeId, r => r.routeId, (ur, r) => new { ur, r }).
                        Where(x => x.ur.status && x.r.status).
                        Select(y => y.r).FirstOrDefault();

            if (route == null)
                throw new Exception("El usuario no tiene ruta asignada");

            var OpeCDService = new OpeCDService();

            var Ventas = OpeCDService.CreateVisita(visitsDB, int.Parse(route.code));

            return Ventas;
        }
    }
}