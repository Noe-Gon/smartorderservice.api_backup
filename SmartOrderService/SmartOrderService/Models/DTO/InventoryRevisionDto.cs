using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InventoryRevisionDto
    {
        public int RouteId { get; set; }
        public int BranchId { get; set; }
        public int InventoryId { get; set; }
        public string Date { get; set; }
        public int Viaje { get; set; }
        public List<InventoryClassification> Classifications { get; set; }

    }
}