using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CustomerDevolutionDto
    {
        public int? UserId { set; get; }
        public int DeliveryId { get; set; }
        public int ReasonDevolutionId { get; set; }
    }
}