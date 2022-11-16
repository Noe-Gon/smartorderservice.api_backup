using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    #region Check BillPocket Sales
    public class CheckBillPocketSalesRequest
    {
        public int RouteId { get; set; }
        public int? UserId { get; set; }
        public Guid WorkDayId { get; set; }
    }

    public class CheckBillPocketSalesResponse
    {
        public int TotalSales { get; set; }
        public bool HasSales { get; set; }
    }
    #endregion

    #region Send BillPocket Report
    public class SendBillPocketReportRequest
    {
        public int? UserId { get; set; }
        public int RouteId { get; set; }
        public Guid WorkDayId { get; set; }
        public string Email { get; set; }
    }

    
    #endregion
}