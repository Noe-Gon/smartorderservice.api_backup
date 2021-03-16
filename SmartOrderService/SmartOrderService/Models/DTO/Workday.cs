using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class Workday
    {
        public Guid WorkdayId { get; set;}
        public int UserId { get; set; }
        public bool IsOpen { get; set; }
        public string DateEnd { get; set; }
        public bool CloseByPortal { get; set; }
        public bool CloseByDevice { get; set; }
        public string NameUserClosedSession { get; set; }
    }
}