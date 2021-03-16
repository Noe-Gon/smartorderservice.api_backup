using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class UserDto
    {

        public int UserId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public int RouteId { get; set; }



    }
}