using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class SendWellcomeEmailResponse
    {
        public string Msg { get; set; }
    }

    public class SendTicketDigitalEmailResponse
    {
        public string Msg { get; set; }
    }

    public class SendReactivationTicketDigitalResponse
    {
        public string Msg { get; set; }
    }

    public class SendRemovalRequestEmailResponse
    {
        public string Msg { get; set; }
    }
}