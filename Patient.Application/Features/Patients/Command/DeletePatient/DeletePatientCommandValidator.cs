using FluentValidation;
using Patient.Application.Features.Patients.Command.AddPatient;

namespace NahdetMisrDS.Aladwaa.Order.Application.Orders.Commands.CreateBarcodeOrder
{
        public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
        {
            public DeletePatientCommandValidator()
            {
            RuleFor(v => v.Id)
                  .NotEmpty().WithMessage("ID is required");





        }
        }
    }

