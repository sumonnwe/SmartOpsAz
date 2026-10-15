//csharp src/InventoryService/InventoryService.Application/Inventory/UpdateInventory/UpdateInventoryCommandValidator.cs
using FluentValidation;

namespace InventoryService.Application.Inventory.UpdateInventory;

public sealed class UpdateInventoryCommandValidator : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DeviceName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeviceType).MaximumLength(100).When(x => x.DeviceType != null);
    }
}