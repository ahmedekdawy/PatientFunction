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
using Patient.Functions.Middlewares.JWT;
using System.Net;

namespace Patient.Function.Functions
{
    public class UpdatePatientFunction
    {
        private readonly ILogger<AddPatientFunction> _logger;
        private readonly IMediator _mediator;


        public UpdatePatientFunction(ILogger<AddPatientFunction> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [Function("UpdatePatientFunction")]
        [Authorize]
        [OpenApiOperation(operationId: "UpdatePatientFunction", tags: new[] { "patient" }, Summary = "UpdatePatientFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UpdatePatientCommand), Summary = "UpdatePatientFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Summary = "Bad request.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Summary = "Server error", Description = "Returns an error message")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(UpdatePatientCommand),
            Required = true)]
        public async Task<IActionResult> UpdatePatient(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdatePatient")] HttpRequestData req,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<UpdatePatientCommand>(requestBody);

            if (command == null)
            {
                return new BadRequestObjectResult(new 
                {
                    Code = 400,
                    Error = "Invalid request payload."
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
