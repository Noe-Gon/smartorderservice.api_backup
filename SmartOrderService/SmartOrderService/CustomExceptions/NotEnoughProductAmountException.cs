using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class NotEnoughProductAmountException : Exception
    {
        public NotEnoughProductAmountException()
        {

        }

        public NotEnoughProductAmountException(int productId): base("No existe suficiente monto para el producto " + productId)
        {

        }
    }
}