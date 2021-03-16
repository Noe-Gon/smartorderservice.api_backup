using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class ReplacementDto
    {
        public int ReplacementId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Status { get; set; }
    }
}