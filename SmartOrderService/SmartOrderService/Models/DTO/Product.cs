using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class Product
    {
        public String productName { get; set; }
        public int amount { get; set; }
        public int productId { get; set; }
    }
}