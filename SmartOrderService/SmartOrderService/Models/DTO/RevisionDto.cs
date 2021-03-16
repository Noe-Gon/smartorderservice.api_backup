using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class RevisionDto
    {

        public int InventoryRevisionId { get; set; }
        public int RouteId { get; set;}
        public int InventoryId { get; set; }
        public int RevisionState { get; set; }
        public int RevisionType { get; set; }
        public string CreatedOn { get; set; }

    }
}