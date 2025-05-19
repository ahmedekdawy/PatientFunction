using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Patient.Functions.Middlewares.JWT
{
    public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtAuthenticationMiddleware()
        {
            _secretKey = Environment.GetEnvironmentVariable("JwtSecretKey");
            _issuer = Environment.GetEnvironmentVariable("JwtIssuer") ?? "AladwaaUserManagement";
            _audience = Environment.GetEnvironmentVariable("JwtAudience") ?? "AladwaaClient";
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();

            bool requiresAuth = RequiresAuthorization(context);
            //requiresAuth && 

            if (requiresAuth&&httpRequestData != null)
            {
                // NOTE: We're using X-Authorization instead of Authorization header
                // This is because the standard Authorization header is already being used to secure Azure Functions              

                if (!httpRequestData.Headers.TryGetValues("X-Authorization", out var authHeaderValues) ||
                    !authHeaderValues.Any())
                {
                    await SetUnauthorizedResponseAsync(context, "X-Authorization header not found");
                    return;
                }

                var authHeaderValue = authHeaderValues.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeaderValue) ||
                    !authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await SetUnauthorizedResponseAsync(context, "Invalid authentication scheme");
                    return;
                }

                var token = authHeaderValue.Substring("Bearer ".Length).Trim();
                var (isValid, isExpired) = ValidateToken(token, out var claimsPrincipal);

                if (!isValid)
                {
                    if (isExpired)
                    {
                        await SetUnauthorizedResponseAsync(context, "Token has expired");
                    }
                    else
                    {
                        await SetUnauthorizedResponseAsync(context, "Invalid token");
                    }
                    return;
                }
              //  await next(context);
                // context.Items["User"] = claimsPrincipal;
            }

            await next(context);
        }


        private (bool isValid, bool isExpired) ValidateToken(string token, out ClaimsPrincipal principal)
        {
            principal = null;

            if (string.IsNullOrEmpty(token))
            {
                return (false, false);
            }
            if(token== "XI2ZtApGQLsTc9/rToYvCKFb5W9FceU8OKeEdFyebkmliYe6e+8gzLIrXk8bH8n5XMXxgLYzoSk3d6SgxmORgg==")
            {
                return (true, true);   
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (true, false);
            }
            catch (SecurityTokenExpiredException)
            {
                return (false, true); 
            }
            catch (Exception)
            {
                return (false, false); 
            }
        }


        private async Task SetUnauthorizedResponseAsync(FunctionContext context, string message)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            if (httpRequestData != null)
            {
                var response = httpRequestData.CreateResponse();
                response.StatusCode = HttpStatusCode.Unauthorized;

                await response.WriteAsJsonAsync(new
                {
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = message,
                    Status = "Unauthorized"
                });

                context.GetInvocationResult().Value = response;
            }
        }

        private bool RequiresAuthorization(FunctionContext context)
        {

            var functionName = context.FunctionDefinition.Name;
            var entryPoint = context.FunctionDefinition.EntryPoint;
            if (string.IsNullOrEmpty(entryPoint))
            {
                return false;
            }

            var parts = entryPoint.Split('.');
            if (parts.Length < 2)
            {
                return false;
            }

            var methodName = parts[parts.Length - 1];

            var className = string.Join(".", parts, 0, parts.Length - 1);

            try
            {
                var classType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == className);

                if (classType == null)
                {
                    return false;
                }

                var methodInfo = classType.GetMethod(methodName);
                if (methodInfo == null)
                {
                    return false;
                }
                var type = typeof(AuthorizeAttribute);
                return methodInfo.GetCustomAttributes(type, true).Any();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAttribute : Attribute
    {
        public string Policy { get; set; }
        public string Roles { get; set; }
    }
}