using SmartOrderService.Models.DTO;

namespace SmartOrderService.Interfaces

{
    public interface IPreSales
    {
        bool SendPreSales(SendPreSalesDTO WorkDayId);
    }
}