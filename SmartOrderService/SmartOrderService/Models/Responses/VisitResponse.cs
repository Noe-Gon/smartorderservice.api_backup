using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SmartOrderService.Models.DTO;

namespace SmartOrderService.Models.Responses
{
    public class VisitResponse : Response<VisitDto>
    {
    }

    public class GetTeamVisitResponse
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public bool Scanned { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public double? LatitudeIn { get; set; }
        public double? LatitudeOut { get; set; }
        public double? LongitudeIn { get; set; }
        public double? LongitudeOut { get; set; }
        public int? ReasonFailed { get; set; }
        public int? VisitId { get; set; }
    }
}