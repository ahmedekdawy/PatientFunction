using MediatR;
using Patient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Patient.Application.Features.Patients.Command.AddPatient
{

    public record DeletePatientCommand : IRequest<bool>
    {
        public int Id { get; set; }

    }

    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IPatientRepository _repo;

        public DeletePatientCommandHandler(IPatientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(DeletePatientCommand command, CancellationToken cancellationToken)
        {
              return _repo.DeletePatient(command.Id);
           
        }
    }
}
