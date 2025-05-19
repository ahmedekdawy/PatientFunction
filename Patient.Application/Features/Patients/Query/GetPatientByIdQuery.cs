using MediatR;
using Patient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Patient.Application.Features.Patients.Command.AddPatient
{

    public record GetPatientByIdQuery : IRequest<object>
    {
        public int Id { get; set; }
    }

    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, object>
    {
        private readonly IPatientRepository _repo;

        public GetPatientByIdQueryHandler(IPatientRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> Handle(GetPatientByIdQuery query, CancellationToken cancellationToken)
        {
              return _repo.GetPatientById(query.Id);
           
        }
    }
}
