using FluentValidation;

namespace InventoryService.Application.Inventory.RegisterInventory;

public sealed class RegisterInventoryValidator : AbstractValidator<RegisterInventory>
{
    public RegisterInventoryValidator()
    {
        RuleFor(x => x.AssetTag).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DeviceName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DeviceType).MaximumLength(100).When(x => x.DeviceType != null);
    }
}