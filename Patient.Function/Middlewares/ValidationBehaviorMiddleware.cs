using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Patient.Function.Middlewares
{
    public class ValidationBehaviorMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly HttpRequest _httpRequest;

        public ValidationBehaviorMiddleware(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor)
        {
            _validators = validators;
            _httpRequest = httpContextAccessor.HttpContext?.Request;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var validationContext = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

            var validationResult = validationResults.FirstOrDefault();

            if (validationResult != null && !validationResult.IsValid)
            {
                var errorResponse = ValidationHelper.CreateValidationErrorResponse(_httpRequest, validationResult);
                throw new CustomValidationException(errorResponse);
            }

            return await next();
        }
    }

    public static class ValidationHelper
    {
        public static dynamic CreateValidationErrorResponse(
      dynamic req,
      FluentValidation.Results.ValidationResult validationResult)
        {
            var apiResponse = new
            {
                status = "400",
                msg = "Validation failed",
                data = validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList()
            };
            return apiResponse;
        }
    }

    public class CustomValidationException : Exception
    {
        public dynamic ErrorResponse { get; }
        public CustomValidationException(dynamic errorResponse)
        {
            ErrorResponse = errorResponse;
        }
    }
}
