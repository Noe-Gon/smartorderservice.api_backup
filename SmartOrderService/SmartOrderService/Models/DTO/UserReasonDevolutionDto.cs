using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class UserReasonDevolutionDto
    {
        public int UserReasonDevolutionId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int Value { get; set; }
    }
}