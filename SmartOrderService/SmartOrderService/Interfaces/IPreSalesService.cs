using SmartOrderService.Models.DTO;

namespace SmartOrderService.Interfaces

{
    public interface IPreSalesService
    {
        bool SendPreSales(SendPreSalesDTO WorkDayId);
    }
}