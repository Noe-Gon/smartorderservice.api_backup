using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class ProductState
    {
        public int ProductClassificationId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int TipoTran { get; set; }


    }
}