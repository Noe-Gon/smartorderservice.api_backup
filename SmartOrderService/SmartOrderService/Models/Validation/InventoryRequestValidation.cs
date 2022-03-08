using SmartOrderService.Models.Requests;
using FluentValidation;

namespace SmartOrderService.Models.Validation
{
    public class InventoryRequestValidation : AbstractValidator<InventoryRequest>
    {

        public InventoryRequestValidation()
        {
            RuleFor(x => x.InventoryId).Must(x => x.HasValue).WithMessage("El id del inventario es obligatorio");
        }

    }
}