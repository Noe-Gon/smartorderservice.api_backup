using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class CategoryRestriction
    {
        public int UserId;
        public int CustomerId;
        public int CategoryId;
        public bool Status;
    }
}