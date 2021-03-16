using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class Device
    {

        public string BranchCode { get; set; }
        public string UserCode { get; set; }
        public string DeviceCode { get; set; }
        public Guid Token { get;  set; }
    }
}