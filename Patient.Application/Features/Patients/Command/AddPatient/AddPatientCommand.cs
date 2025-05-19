using MediatR;
using Patient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Patient.Application.Features.Patients.Command.AddPatient
{

    public record AddPatientCommand : IRequest<bool>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, bool>
    {
        private readonly IPatientRepository _repo;

        public AddPatientCommandHandler(IPatientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(AddPatientCommand command, CancellationToken cancellationToken)
        {
            if (command.FirstName == null)
            {
                throw new Exception("First Name is required");
            }
            if (command.LastName == null)
            {
                throw new Exception("Last Name is required");
            }
            if (command.Phone == null)
            {
                throw new Exception("Phone is required");
            }
            if (command.Email == null)
            {
                throw new Exception("Email is required");
            }
            if (!IsValidEmail(command.Email))
            {
                throw new Exception("Invalid Email");
            }
            if (command.DateOfBirth.Year <1900)
            {
                throw new Exception("Birth date is required, must be greater than 1900");
            }
            var exists= _repo.CheckEmail(command.Email,0);
            if (exists)
            {
                throw new Exception("Email already register with another patient");
            }


            return _repo.AddPatient(command);
           
        }
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
