using Entities.Models;
using FluentValidation;

namespace Services;

public class CancellationRequestValidator : AbstractValidator<CancellationRequest>
{
    public CancellationRequestValidator()
    {
        RuleFor(request => request.BookingId)
            .GreaterThan(0).WithMessage("BookingId must be greater than 0.");
    }
}
