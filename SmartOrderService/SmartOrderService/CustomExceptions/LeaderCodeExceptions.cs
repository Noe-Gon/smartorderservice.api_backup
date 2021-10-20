using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class LeaderCodeNotFoundException : Exception
    {
        public LeaderCodeNotFoundException() : base()
        {
        }

        public LeaderCodeNotFoundException(string message) : base(message)
        {
        }
    }

    public class LeaderCodeExpiredException : Exception
    {
        public LeaderCodeExpiredException() : base()
        {
        }

        public LeaderCodeExpiredException(string message) : base(message)
        {
        }
    }
}