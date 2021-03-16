using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class PromotionDto
    {

        public int PromotionId { get; set; }
        public string Name { get; set; }
        public string ValidityStart { get; set; }
        public string ValidityEnd { get; set; }
        public int Type { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }

    }
}