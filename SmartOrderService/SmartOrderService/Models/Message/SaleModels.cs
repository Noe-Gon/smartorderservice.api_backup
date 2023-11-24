using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    #region Send Ticket Digital

    public class SendTicketDigitalRequest
    {
        public int SaleId { get; set; }
        public string Email { get; set; }
    }

    #endregion

    #region Send Adjustment Email

    public class SendAdjustmentRequest
    {
        public int RouteId { get; set; }
        public string Email { get; set; }
        public DateTime? Date { get; set; }
    }

    #endregion
}