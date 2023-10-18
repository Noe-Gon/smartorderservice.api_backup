using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Interfaces;
using SmartOrderService.Models.DTO;

namespace SmartOrderService.Services
{
    public class PreSalesService : IPreSales
    {
        private SmartOrderModel db = new SmartOrderModel();

        public bool SendPreSales(SendPreSalesDTO request)
        {
            throw new InternalServerException("No se ha podido conectar al servidor de API_Preventa");
        }
    }
}