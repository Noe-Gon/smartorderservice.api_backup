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

        public ResponseBase<BillPocketTokensResponse> GetTokensByUserId(int userId)
        {
            var user = db.Configuracion_WorkByCloud.Where(x => x.userId == userId).FirstOrDefault();

            if (user != null)
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new BillPocketTokensResponse
                {
                    BillPocket_TokenDispositivo = user.BillPocket_TokenDispositivo,
                    BillPocket_TokenUsuario = user.BillPocket_TokenUsuario
                });
            }
            else
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new List<string>
                {
                    "No se logró encontrar al usuario."
                });
            }
        }
    }
}