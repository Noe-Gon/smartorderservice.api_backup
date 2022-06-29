using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class OrderDTO
    {
        public List<OrderBodyDTO> Orders { get; set; }
    }

    public class OrderBodyDTO
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int UserId { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public double TotalCash { get; set; }

        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        public int productId { get; set; }

        public int amount { get; set; }

        public double price { get; set; }

        public double import { get; set; }
    }
}