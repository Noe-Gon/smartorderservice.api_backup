using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CustomerVisitDto
    {
        public int? UserId { get; set; }
        public int VisitId { get; set; }
        public int CustomerId { get; set; }
        public bool Scanned { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public double LatitudeIn { get; set; }
        public double LongitudeIn { get; set; }
        public double LatitudeOut { get; set; }
        public double LongitudeOut { get; set; }
        public int? ReasonFailedId { get; set; }
        public double? AccuracyIn { set; get; }
        public double? AccuracyOut { set; get; }

    }
}