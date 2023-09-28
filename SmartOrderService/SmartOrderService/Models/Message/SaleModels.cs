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
}