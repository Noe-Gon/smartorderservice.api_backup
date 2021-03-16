using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class User
    {
        public int UserId;
        public String UserCode;
        public String Name;
        public int Type;
        public int RouteId;
        public int BranchId;
        public int ClosureTypeId;
    }
}