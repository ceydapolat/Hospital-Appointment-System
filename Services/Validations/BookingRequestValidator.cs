using Entities.Models;
using FluentValidation;

namespace Services.Validations
{
	public class BookingRequestValidator : AbstractValidator<BookingRequest>
	{
		public BookingRequestValidator()
		{
            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required")
                .Must(BeValidDate)
                .WithMessage("Invalid date format or value");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start Time is required")
                .Matches(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("Start Time must be in HH:mm format");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("End Time is required")
                .Matches(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("End Time must be in HH:mm format");

            RuleFor(x => x.DoctorName)
                .NotEmpty()
                .WithMessage("Doctor Name is required");

            RuleFor(x => x.PatientName)
                .NotEmpty()
                .WithMessage("Patient Name is required");
        }

        private bool BeValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }
    }
}

