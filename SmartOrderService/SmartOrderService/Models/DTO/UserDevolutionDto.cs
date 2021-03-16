using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class UserDevolutionDto
    {
        public int UserDevolutionId { get; set; }
        public int ProductId { get; set; }
        public int InventoryId { get; set; }
        public int Amount { get; set; }
        public int UserReasonDevolutionId { get; set; }
    }
}