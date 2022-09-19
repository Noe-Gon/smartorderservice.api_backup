using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class LoyaltyGetRulesResponse
    {
        public List<RulesConfig> RulesConfig { get; set; }
    }

    public class RulesConfig
    {
        public string uuid { get; set; }
        public int? points { get; set; }
        public List<Customer> customers { get; set; }
    }

    public class Customer
    {
        public string code { get; set; }
        public int? points { get; set; }
        public List<Rule> rules { get; set; }
    }

    public class Rule
    {
        public FactorRule factor { get; set; }
        public List<ProductRule> products { get; set; }
    }

    public class FactorRule
    {
        public int points { get; set; }
        public double currency { get; set; }
        public int maximumPercentage { get; set; }
    }

    public class ProductRule
    {
        public string code { get; set; }
        public string name { get; set; }
        public object points { get; set; }
    }
}