using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class ProductRequest : Request
    {
       public int? ProductId { get; set; }
    }
}