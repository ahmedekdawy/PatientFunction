using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Patient.Application.Features.Patients.Command.AddPatient;
using Patient.Function.Middlewares;
using Patient.Functions.Middlewares.ExceptionHandlerMiddleware;
using Patient.Functions.Middlewares.JWT;
using Patient.Infrastructure;
using System.Text;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Patient.Function;
using FluentValidation;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);


// Register services for MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AddPatientCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorMiddleware<,>));
});



// Add JWT Authentication
var jwtSecretKey = Environment.GetEnvironmentVariable("JwtSecretKey");
var jwtIssuer = Environment.GetEnvironmentVariable("JwtIssuer");
var jwtAudience = Environment.GetEnvironmentVariable("JwtAudience");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IDocumentFilter, JwtAuthDocumentFilter>();


builder.Services.AddHttpClient();



// Configure OpenAPI documentation
builder.Services.Configure<OpenApiConfigurationOptions>(options =>
{
    options.Info = new OpenApiInfo
    {
        Title = "Patient API",
        Version = OpenApiVersionType.V3.ToString(),
        Description = "Patient API",

        TermsOfService = new Uri("https://aladwaa.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "ahmedekdawy@gmail.com",
            Url = new Uri("https://aladwaa.com/support")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("http://opensource.org/licenses/MIT")
        }
    };
 


});
builder.Services.AddScoped<IPatientRepository, PatientRepository>();



// Add JSON configuration for controllers
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new DefaultNamingStrategy()
    };
});

builder.UseMiddleware<ExceptionHandlerMiddleware>();

builder.UseMiddleware<JwtAuthenticationMiddleware>();





builder.Build().Run();
