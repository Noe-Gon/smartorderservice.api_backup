using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Generic
{
    public class AuditData
    {
        public int createdby { get; set; }
        public DateTime? createdon { get; set; }

        public int modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }

        public AuditData()
        {
            createdon = DateTime.Now;
        }
    }
}