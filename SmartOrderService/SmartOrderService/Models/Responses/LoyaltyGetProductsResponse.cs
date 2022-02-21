using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class LoyaltyGetProductsResponse
    {
        public List<ProductConfig> ProductConfig { get; set; }

    }

    public class Factor
    {
        public int points { get; set; }
        public double currency { get; set; }
    }

    public class ProductLoyalty
    {
        public string code { get; set; }
        public string name { get; set; }
        public int? points { get; set; }
    }

    public class ProductConfig
    {
        public Factor factor { get; set; }
        public List<ProductLoyalty> products { get; set; }
    }
}