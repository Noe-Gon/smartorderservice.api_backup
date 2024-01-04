using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class MVPostCRMConsumer
    {
        public Guid CrmId { get; set; }
    }

    public class MVIdFromCRM
    {
        public Guid CrmId { get; set; }
    }

    public class MVIsInCRM
    {
        public bool IsInCRM { get; set; }
    }
}