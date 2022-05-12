using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class CreateWorkdayInventoryNotFoundException : Exception
    {
        public CreateWorkdayInventoryNotFoundException() : base("Aún no se manda el inventario de Ope20/OpeCd a WBC. Comunicarse a Ope20/OpeCd")
        {

        }
    }
}