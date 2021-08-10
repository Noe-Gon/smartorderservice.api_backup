using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public class Consumer
    {
        public enum STATUS
        {
            UNKNOW = 0,
            CONSUMER,
            DEACTIVATED
        }
    }
}