using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class BillPocketTokensResponse
    {
        public string BillPocket_TokenUsuario { get; set; }
        public string BillPocket_TokenDispositivo { get; set; }
    }
}