using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class Article
    {
        public int ArticleId;
        public string Code;
        public string Name;
        public double Price;
        public bool Status;
    }
}