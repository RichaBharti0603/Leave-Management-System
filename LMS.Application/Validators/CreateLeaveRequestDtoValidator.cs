using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.Application.Validators;

public class CreateLeaveRequestDtoValidator : AbstractValidator<CreateLeaveRequestDto>
{
    public CreateLeaveRequestDtoValidator()
    {
        RuleFor(x => x.LeaveTypeId).GreaterThan(0).WithMessage("Leave Type is required.");
        
        RuleFor(x => x.StartDateTime)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThan(DateTime.UtcNow.Date).WithMessage("Start date must be in the future.");

        RuleFor(x => x.EndDateTime)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDateTime).WithMessage("End date must be after or equal to start date.");
            
        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}
