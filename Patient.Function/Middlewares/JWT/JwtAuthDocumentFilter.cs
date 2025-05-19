using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.OpenApi.Models;

namespace Patient.Functions.Middlewares.JWT
{
    public class JwtAuthDocumentFilter : IDocumentFilter
    {
        public void Apply(IHttpRequestDataObject req, OpenApiDocument document)
        {
            // Initialize components if needed
            document.Components = document.Components ?? new OpenApiComponents();
            document.Components.SecuritySchemes = document.Components.SecuritySchemes ?? new Dictionary<string, OpenApiSecurityScheme>();

            // Add JWT Bearer security scheme
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey, // Use ApiKey instead of Http
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Scheme = "Bearer"
            };

            // Set global security requirement
            document.SecurityRequirements = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        };
        }
    }
}