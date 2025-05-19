using Moq;
using Patient.Application.Features.Patients.Command.AddPatient;
using Patient.Infrastructure;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Patient.Test
{
    public class AddPatientCommandTest
    {
        public Mock<IPatientRepository> mockRepo = new Mock<IPatientRepository>();

        AddPatientCommandHandler Service;
        public AddPatientCommandTest()
        {
            Service = new AddPatientCommandHandler(mockRepo.Object);
        }

        [Fact]
        async void AddPatientValid()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "ahmed",
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
          
                
            var result = await Service.Handle(model, CancellationToken.None);
            Assert.True(result);
        }

        [Fact]
        async void AddPatientValidationEmptyFirstName()
        {
            AddPatientCommand model = new AddPatientCommand
            {
               
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
            Assert.Contains("First Name is required", ex.Message);

        }
        [Fact]
        async void AddPatientValidationEmptyLastName()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName="Ahmed",
               
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
   
            Assert.Contains("Last Name is required", ex.Message);
   
        }
        [Fact]
        async void AddPatientValidationEmptyDateOfBirth()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "Ahmed",
                LastName="Elsayed",
              //  DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
            Assert.Contains("Birth date is required, must be greater than 1900", ex.Message);

        }
        [Fact]
        async void AddPatientValidationEmptyPhone()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "Ahmed",
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                //Phone = "01280514149",
                Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
            Assert.Contains("Phone is required", ex.Message);
   
        }
        [Fact]
        async void AddPatientValidationEmptyEmail()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "Ahmed",
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
               // Email = "ahmed@gmail.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
          
            Assert.Contains("Email is required", ex.Message);
            //Assert.Contains("Invalid Email", ex.Message);
            //
            //Assert.Contains("\"Email already register with another patient", ex.Message);
        }
        [Fact]
        async void AddPatientValidationInvalidEmail()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "Ahmed",
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "xxx"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));

            
            Assert.Contains("Invalid Email", ex.Message);
            //
            //Assert.Contains("\"Email already register with another patient", ex.Message);
        }
        [Fact]
        async void AddPatientValidationUsedEmail()
        {
            AddPatientCommand model = new AddPatientCommand
            {
                FirstName = "Ahmed",
                LastName = "Elsayed",
                DateOfBirth = DateTime.Now,
                Phone = "01280514149",
                Email = "a@b.com"
            };
            mockRepo.Setup(x => x.AddPatient(model)).Returns(true);
            mockRepo.Setup(x => x.CheckEmail(model.Email,0)).Returns(true);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await Service.Handle(model, CancellationToken.None));
            Assert.Contains("Email already register with another patient", ex.Message);
        }
    }
}