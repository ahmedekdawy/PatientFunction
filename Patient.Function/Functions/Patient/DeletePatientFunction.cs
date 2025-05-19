using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Patient.Application.Features.Patients.Command.AddPatient;
using Patient.Function.Helpers;
using System.Net;
using Patient.Functions.Middlewares.JWT;

namespace Patient.Function.Functions
{
    public class DeletePatientFunction
    {
        private readonly ILogger<AddPatientFunction> _logger;
        private readonly IMediator _mediator;


        public DeletePatientFunction(ILogger<AddPatientFunction> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [Function("DeletePatientFunction")]
        [Authorize]
        [OpenApiOperation(operationId: "DeletePatientFunction", tags: new[] { "patient" }, Summary = "DeletePatientFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UpdatePatientCommand), Summary = "UpdatePatientFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Summary = "Bad request.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Summary = "Server error", Description = "Returns an error message")]
        public async Task<IActionResult> DeletePatient(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeletePatient/{id}")] HttpRequestData req,int id,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = new DeletePatientCommand { Id=id};

            if (command == null)
            {
                return new BadRequestObjectResult(new 
                {
                    Code = 400,
                    Error = "Invalid request url."
                });
            }
            try
            {

           
            var response = await _mediator.Send(command);
            return new OkObjectResult(new
            {
                Code = 200,
                Data = response
            });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
