using Azure;
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
using Patient.Function.Middlewares;
using Patient.Functions.Middlewares.JWT;
using System.Net;

namespace Patient.Function.Functions
{
    public class AddPatientFunction
    {
        private readonly ILogger<AddPatientFunction> _logger;
        private readonly IMediator _mediator;


        public AddPatientFunction(ILogger<AddPatientFunction> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [Function("AddPatient")]
        [Authorize]
        [OpenApiOperation(operationId: "AddPatient", tags: new[] { "patient" }, Summary = "AddPatient.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AddPatientCommand), Summary = "AddPatient.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Summary = "Bad request.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Summary = "Server error", Description = "Returns an error message")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(AddPatientCommand),
            Required = true)]
        public async Task<IActionResult> AddPatient(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddPatient")] HttpRequestData req,
        ILogger log)
        {
            try
            {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<AddPatientCommand>(requestBody);

            if (command == null)
            {
                return new BadRequestObjectResult(new 
                {
                    Code = 400,
                    Error = "Invalid request payload."
                });
            }
            var response = await _mediator.Send(command);
            return new OkObjectResult(new
            {
                Code = 200,
                Data = response
            });

            }
            catch (CustomValidationException ex)
            {
                // Return a 400 Bad Request if a validation exception occurs
                return new BadRequestObjectResult(ex.ErrorResponse);
            }
            catch (Exception ex)
            {

                return new ObjectResult(new 
                {
                    Code = 500,
                    Error =  ex.Message
                })
                {
                    StatusCode = 500
                };
            }

        }

    }
}
