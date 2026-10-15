using DeviceService.Application.DTOs;
using FluentValidation;

namespace DeviceService.Application.Validators;

public class RegisterDeviceRequestValidator : AbstractValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
        RuleFor(x => x.DeviceCode)
            .NotEmpty().WithMessage("DeviceCode is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        RuleFor(x => x.FirmwareVersion)
            .MaximumLength(50).WithMessage("FirmwareVersion must not exceed 50 characters.")
            .When(x => x.FirmwareVersion != null);
    }
}