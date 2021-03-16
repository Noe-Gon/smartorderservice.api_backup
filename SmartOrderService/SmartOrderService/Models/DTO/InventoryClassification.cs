using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InventoryClassification
    {
        
        public int ClassificationId { get; set; }
        public string Name { get; set; }
        public List<InventoryClassificationDetail> Details { get; set; }


        public InventoryClassification() {
            Details = new List<InventoryClassificationDetail>();
        }

    }
}