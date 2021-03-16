using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class SalesData
    {

        public Dictionary<int, int> Products { get; set; }

        public Dictionary<int, int> Bottles { get; set; }

        public Dictionary<int, int> Replacements { get; set; }
    }
}