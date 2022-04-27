using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public class DeliveryStatus
    {
        public static string UNDEFINED = "UNDEFINED";
        public static string UNDELIVERED = "UNDELIVERED";
        public static string DELIVERED = "DELIVERED";
        public static string PARTIALLY_DELIVERED = "PARTIALLY_DELIVERED";
        public static string CANCELED = "CANCELED";
        
    }
}