using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class TrackingDto
    {
        public TrackingDto() {
            Points = new List<PointDto>();
        }

        public string BranchCode { get; set; }
        public string UserCode { get; set; }
        public List<PointDto> Points { get; set; }
    }
}