using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Utils.Enums
{
    public class TeamTravelStatus
    {
        public enum CODES
        {
            DISPONIBLE = 0,
            ABIERTO_IMPULSOR,
            ABIERTO_AYUDANTE,
            CERRADO_AYUDANTE,
            CERRADO_IMULSOR
        }

        public static string TranslateCode(int code)
        {
            switch (code)
            {
                case (int)CODES.DISPONIBLE: return "Disponible";
                case (int)CODES.ABIERTO_IMPULSOR: return "Abierto por impulsor";
                case (int)CODES.ABIERTO_AYUDANTE: return "Abierto por ayudante";
                case (int)CODES.CERRADO_AYUDANTE: return "Cerrado por ayudante";
                case (int)CODES.CERRADO_IMULSOR: return "Cerrado por impulsor";
                default: return "Desconocido";
            }
        }
    }
}