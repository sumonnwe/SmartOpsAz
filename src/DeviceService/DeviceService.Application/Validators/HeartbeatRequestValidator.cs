using DeviceService.Application.DTOs;
using FluentValidation;

namespace DeviceService.Application.Validators;

public class HeartbeatRequestValidator : AbstractValidator<HeartbeatRequest>
{
    public HeartbeatRequestValidator()
    {
        RuleFor(x => x.RecordedAtUtc)
            .NotEmpty().WithMessage("RecordedAtUtc is required.");

        RuleFor(x => x.BatteryPercentage)
            .InclusiveBetween(0, 100).WithMessage("BatteryPercentage must be between 0 and 100.")
            .When(x => x.BatteryPercentage.HasValue);

        RuleFor(x => x.SignalStrengthRssi)
            .InclusiveBetween(-120, 0).WithMessage("SignalStrengthRssi must be between -120 and 0.")
            .When(x => x.SignalStrengthRssi.HasValue);

        RuleFor(x => x.CpuUsagePercentage)
            .InclusiveBetween(0, 100).WithMessage("CpuUsagePercentage must be between 0 and 100.")
            .When(x => x.CpuUsagePercentage.HasValue);

        RuleFor(x => x.MemoryUsagePercentage)
            .InclusiveBetween(0, 100).WithMessage("MemoryUsagePercentage must be between 0 and 100.")
            .When(x => x.MemoryUsagePercentage.HasValue);

        RuleFor(x => x.FirmwareVersion)
            .MaximumLength(50).WithMessage("FirmwareVersion must not exceed 50 characters.")
            .When(x => x.FirmwareVersion != null);
    }
}