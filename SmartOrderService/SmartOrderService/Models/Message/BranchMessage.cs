using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    public class GetLimitTimeResponse
    {
        public TimeSpan Time { get; set; }
        //public TimeSpan BaseUtcOffset { get; set; }
        public string TimeZone { get; set; }

    }
}