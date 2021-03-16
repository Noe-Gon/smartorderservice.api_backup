using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class Promotion
    {
        public int PromotionId;
        public string Code;
        public string Name;
        public DateTime ValidityEnd;
        public int MaxAmountCustomer;
        public int MaxAmountUser;
        public int Type;
        public bool Global;
        public bool Status;
    }
}