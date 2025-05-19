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
    public class GetPatientByIdFunction
    {
        private readonly ILogger<AddPatientFunction> _logger;
        private readonly IMediator _mediator;


        public GetPatientByIdFunction(ILogger<AddPatientFunction> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        [Function("GetPatientByIdFunction")]
        [Authorize]
        [OpenApiOperation(operationId: "GetPatientByIdFunction", tags: new[] { "patient" }, Summary = "GetPatientByIdFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetPatientsQuery), Summary = "GetPatientByIdFunction.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Summary = "Bad request.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Summary = "Server error", Description = "Returns an error message")]
        public async Task<IActionResult> GetPatientById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPatientById/{id}")] HttpRequestData req,int id,
        ILogger log)
        {
          //  string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
          //  var command = JsonConvert.DeserializeObject<AddPatientCommand>(requestBody);

            //if (command == null)
            //{
            //    return new BadRequestObjectResult(new 
            //    {
            //        Code = 400,
            //        Error = "Invalid request payload."
            //    });
            //}
            var response = await _mediator.Send(new GetPatientByIdQuery { Id=id});
            return new OkObjectResult(new
            {
                Code = 200,
                Data = response
            });
        
        }

    }
}
