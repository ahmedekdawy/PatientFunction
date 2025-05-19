using FluentValidation;
using Patient.Application.Features.Patients.Command.AddPatient;

namespace NahdetMisrDS.Aladwaa.Order.Application.Orders.Commands.CreateBarcodeOrder
{
        public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
        {
            public UpdatePatientCommandValidator()
            {
            RuleFor(v => v.Id)
                  .NotEmpty().WithMessage("ID is required");

            RuleFor(v => v.FirstName)
                    .NotEmpty().WithMessage("FirstName is required")
                    .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");

                RuleFor(v => v.LastName)
                     .NotEmpty().WithMessage("FirstName is required")
                    .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters");



        }
        }
    }

