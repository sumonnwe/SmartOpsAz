using FluentValidation;

namespace InventoryService.Application.Inventory.TransferLocation;

public sealed class TransferLocationValidator : AbstractValidator<TransferLocationCommand>
{
    public TransferLocationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .Must(l => !string.IsNullOrWhiteSpace(l)).WithMessage("Location cannot be empty or whitespace.");
    }
}
