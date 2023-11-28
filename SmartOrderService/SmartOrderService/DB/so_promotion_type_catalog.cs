using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_promotion_type_catalog
    {
        public int id { get; set; }
        public int code { get; set; }
        public string name { get; set; }
        public decimal createdby { get; set; }
        public decimal? modifiedby { get; set; }
        public DateTime createdon { get; set; }
        public DateTime? modifiedon { get; set; }
        public bool status { get; set; }
    }
}