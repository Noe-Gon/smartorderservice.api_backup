using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.CustomExceptions
{
    public class RelatedDriverNotFoundException : Exception
    {
        public RelatedDriverNotFoundException(int idUser) : base("El usuario con id " + idUser + " no se encuentra relacionado con ningun equipo")
        {

        }

        public RelatedDriverNotFoundException(string msg) : base(msg)
        {

        }
    }
}