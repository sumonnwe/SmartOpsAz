using FluentValidation;

namespace InventoryService.Application.Inventory.AssignDepartment;

public sealed class AssignDepartmentValidator : AbstractValidator<AssignDepartmentCommand>
{
    public AssignDepartmentValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .Must(d => !string.IsNullOrWhiteSpace(d)).WithMessage("Department cannot be empty or whitespace.");
    }
}
