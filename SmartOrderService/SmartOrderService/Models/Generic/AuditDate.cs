using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Generic
{
    public class AuditDate
    {
        public DateTime? createdon { get; set; }

        public DateTime? modifiedon { get; set; }

        public AuditDate()
        {
            createdon = DateTime.Now;
        }
    }
}