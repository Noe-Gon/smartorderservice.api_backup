using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_branch_limit_time
    {
        [Key]
        public int branchLimitTimeId { get; set; }

        public int branchId { get; set; }

        public TimeSpan limit_time { get; set; }

        public bool status { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }
    }
}