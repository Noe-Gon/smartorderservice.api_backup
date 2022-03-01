using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() : base()
        {
        }

        public DuplicateEntityException(string message) : base(message)
        {
        }
    }
}