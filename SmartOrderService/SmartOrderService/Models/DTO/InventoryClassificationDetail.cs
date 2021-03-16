using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InventoryClassificationDetail
    {

        public int ItemId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Code { get; set; }
        public List<ProductState> SubClassifications { get; set; }

        public InventoryClassificationDetail()
        {
            SubClassifications = new List<ProductState>();

        }

    }
}