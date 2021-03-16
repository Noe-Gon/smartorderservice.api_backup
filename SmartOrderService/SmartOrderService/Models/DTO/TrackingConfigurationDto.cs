using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class TrackingConfigurationDto
    {
        public int Id { set; get; }
        public int Distance { set; get; }
        public long Interval { set; get; }
        public bool HighPrecision { get; set; }
    }
}