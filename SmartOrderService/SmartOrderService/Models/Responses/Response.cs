using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class Response<T>
    {

        public List<T> Items;
        public int TotalRecords;

        public Response() {
            Items = new List<T>();
            TotalRecords = 0;
        }
    }
}