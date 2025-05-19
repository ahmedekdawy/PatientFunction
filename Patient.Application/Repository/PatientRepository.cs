using Dapper;
using Microsoft.Data.SqlClient;
using Patient.Application;
using Patient.Application.Features.Patients.Command.AddPatient;

namespace Patient.Infrastructure
{
    public class PatientRepository: IPatientRepository
    {
        private readonly string _connectionString=Environment.GetEnvironmentVariable("DefaultConnection");

        public PatientRepository()
        {
          
        }

        public bool AddPatient(AddPatientCommand patient)
        {
            var query = "INSERT INTO Patients (FirstName, LastName, DateOfBirth, Email, Phone) VALUES (@FirstName, @LastName, @DateOfBirth, @Email, @Phone)";
            int affectedRows = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                affectedRows = connection.Execute(query, new
                {
                    patient.FirstName,
                    patient.LastName,
                    patient.DateOfBirth,
                    patient.Email,
                    patient.Phone
                });
            }
            return affectedRows > 0;
        }
        public bool UpdatePatient(UpdatePatientCommand patient)
        {
            var query = @"update Patients set FirstName=@FirstName,LastName= @LastName, 
                DateOfBirth=@DateOfBirth,Email= @Email,Phone= @Phone where id=@id";
            int affectedRows = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                affectedRows = connection.Execute(query, new
                {
                    patient.FirstName,
                    patient.LastName,
                    patient.DateOfBirth,
                    patient.Email,
                    patient.Phone,
                    patient.Id
                });
            }
            return affectedRows > 0;
        }
        public bool DeletePatient(int id)
        {
            var query = "delete Patients where id=@id";
            int affectedRows = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                affectedRows =  connection.Execute(query, new
                {
                   id
                });
            }
            return affectedRows > 0;
        }
        public IEnumerable<PatientDto> GetPatients()
        {
            var query = "select * from  Patients ";
            IEnumerable<PatientDto> patients;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                 patients = connection.Query< PatientDto>(query);
            }
            return patients;
        }
        public PatientDto GetPatientById(int id)
        {
            var query = "select * from  Patients where id=@id";
            PatientDto patient;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                patient = connection.QuerySingle<PatientDto>(query, new
                {
                    id
                });
            }
            return patient;
        }
        public bool CheckEmail(string  email,int id)
        {
            var query = "select count(email) from  Patients where id!=@id and email =@email";
            int  patient;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                patient = connection.ExecuteScalar<int>(query, new
                {
                    id,
                    email
                });
            }
            return patient>0;
        }
    }
}
