using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class DeliveryReplacementDto
    {

        public int ReplacementId { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }

    }
}