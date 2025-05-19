using MediatR;
using Patient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Patient.Application.Features.Patients.Command.AddPatient
{

    public record GetPatientsQuery : IRequest<object>
    {

    }

    public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, object>
    {
        private readonly IPatientRepository _repo;

        public GetPatientsQueryHandler(IPatientRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> Handle(GetPatientsQuery command, CancellationToken cancellationToken)
        {
              return _repo.GetPatients();
           
        }
    }
}
