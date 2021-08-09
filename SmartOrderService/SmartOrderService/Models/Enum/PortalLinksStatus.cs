using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public class PortalLinks
    {
        public enum STATUS
        {
            UNKNOW = 0,
            PENDING,
            ACTIVATED,
            EXPIRED,
            CANCELED
        }

        public enum TYPE
        {
            UNKNOW = 0,
            TERMSANDCONDITIONS_ACCEPT,
            EMAIL_ACTIVATION,
            EMAIL_DEACTIVATION,
            SMS_ACTIVATION,
            SMS_DEACTIVATION
        }
    }
}