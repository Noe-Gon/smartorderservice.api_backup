using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Message
{
    public class InventoryStatusResponse
    {
        public int InventoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OriginalOrder { get; set; }
        public int OpenOrder { get; set; }
    }
}