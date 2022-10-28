using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Locks
{
    public class WorkdayLock
    {
        public int LastUser { get; set; }
        public WorkdayService WorkdayService { get; set; }
    }
}