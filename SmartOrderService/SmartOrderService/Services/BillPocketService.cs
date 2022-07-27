using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class BillPocketService
    {
        SmartOrderModel db = new SmartOrderModel();

        public ResponseBase<BillPocketTokensResponse> GetTokensByUserId(int routeId)
        {
            int? branchId = db.so_route.Where(x => x.routeId == routeId).Select(x => x.branchId).FirstOrDefault();
            var token = db.Configuracion_WorkByCloud.Where(x => x.branchId == branchId).Select(x => x.BillPocket_TokenUsuario).FirstOrDefault();
            if (token != null)
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new BillPocketTokensResponse
                {
                    BillPocket_TokenUsuario = token
                });
            }
            else
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new List<string>
                {
                    "No se cuenta con una cuenta de BillPocket para esta ruta."
                });
            }
        }
    }
}
