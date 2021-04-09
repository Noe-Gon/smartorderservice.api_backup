using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public enum EInventoryTeamStatus
    {
        InventarioCerrado = 0,
        InventarioAbiertoPorImpulsor = 1,
        InventarioAbiertoPorAyudante = 2,
        InventarioCerradoPorAsistente = 3,
        InventarioCerradoPorImpulsor = 4
    }
}