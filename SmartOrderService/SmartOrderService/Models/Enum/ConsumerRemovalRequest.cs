using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public class ConsumerRemovalRequest
    {
        public enum STATUS
        {
            UNKNOW = 0,
            PENDING,
            COMPLETED,
            DELETED
        }
    }
}