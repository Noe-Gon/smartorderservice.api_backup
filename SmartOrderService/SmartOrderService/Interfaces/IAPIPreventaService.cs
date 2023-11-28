using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Responses;

namespace SmartOrderService.Interfaces
{
    public interface IAPIPreventaService
    {
        ClosingPreclosingResponse SendPreSales(ClosingPreclosingDTO request);
    }
}