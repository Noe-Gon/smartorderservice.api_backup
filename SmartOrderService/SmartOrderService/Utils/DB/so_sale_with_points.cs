using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.Utils.DB
{
    public class so_sale_with_points
    {
        [Key]
        public int saleWithPointsId { get; set; }
        public int saleId { get; set; }
        public DateTime? date { get; set; }
        public int userId { get; set; }
        public int customerId { get; set; }
    }
}