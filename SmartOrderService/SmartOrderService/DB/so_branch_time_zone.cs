using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_branch_time_zone
    {
        public int branchtimezoneId { get; set; }

        public int branchId { get; set; }
        public string time_zone { get; set; }
        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public string branchCode { get; set; }
    }
}