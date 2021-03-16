using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class TripDto
    {

        public int UserId { get; set; }
        public int Order { get; set; }
        public bool IsFinished { get; set; }
        public int RouteCode { get; set; }
        public string BranchCode { get; set; }
    }
}