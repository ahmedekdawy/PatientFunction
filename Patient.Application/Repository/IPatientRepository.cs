using Microsoft.Data.SqlClient;
using Patient.Application;
using Patient.Application.Features.Patients.Command.AddPatient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patient.Infrastructure
{
    public interface IPatientRepository
    {
         bool AddPatient(AddPatientCommand patient);
         bool UpdatePatient(UpdatePatientCommand patient);
         bool DeletePatient(int id);
         IEnumerable<PatientDto> GetPatients();
         PatientDto GetPatientById(int id);
        bool CheckEmail(string email, int id);
    }
}
