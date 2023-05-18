using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CustomerVisitDto
    {
        public static CustomerVisitDto CloneCustomerVisitDto(CustomerVisitDto dto)
        {
            var clone = new CustomerVisitDto();
            CultureInfo provider = CultureInfo.InvariantCulture;
            clone.UserId = dto.UserId;
            clone.VisitId = dto.VisitId;
            clone.CustomerId = dto.CustomerId;
            clone.Scanned = dto.Scanned;
            clone.CheckIn = DateTime.ParseExact(dto.CheckIn, "dd/MM/yyyy HH:mm:ss", provider).ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
            clone.CheckOut = DateTime.ParseExact(dto.CheckOut, "dd/MM/yyyy HH:mm:ss", provider).ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
            clone.LatitudeIn = dto.LatitudeIn;
            clone.LatitudeOut = dto.LatitudeOut;
            clone.LongitudeIn = dto.LongitudeIn;
            clone.LongitudeOut = dto.LongitudeOut;
            clone.ReasonFailedId = dto.ReasonFailedId;
            clone.AccuracyIn = dto.AccuracyIn;
            clone.AccuracyOut = dto.AccuracyOut;

            return clone;
        }

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