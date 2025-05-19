using FluentValidation;
using Patient.Application.Features.Patients.Command.AddPatient;

namespace NahdetMisrDS.Aladwaa.Order.Application.Orders.Commands.CreateBarcodeOrder
{
        public class AddPatientCommandValidator : AbstractValidator<AddPatientCommand>
        {
            public AddPatientCommandValidator()
            {

                RuleFor(v => v.FirstName)
                    .NotEmpty().WithMessage("FirstName is required")
                    .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");

                RuleFor(v => v.LastName)
                     .NotEmpty().WithMessage("FirstName is required")
                    .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");



        }
        }
    }

