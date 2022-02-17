using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.Utils.DB
{
    public class so_sale_with_points_details
    {
        [Key]
        public int saleWithPointDetailId { get; set; }
        public int saleWithPointsId { get; set; }
        public int productId { get; set; }
        public int Amount { get; set; }
        public int pointsPerUnit { get; set; }
        public int totalPoints { get; set; }
    }
}