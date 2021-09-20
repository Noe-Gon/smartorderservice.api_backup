using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    public class AuthenticateEmployeeCodeRequest
    {
        public string EmployeeCode { get; set; }
    }

    public class AuthenticateEmployeeCodeResponse
    {
        public DateTime Date { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}