using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using JobManagement.Application.Validators;

namespace JobManagement.Infrastructure.Configuration;

public static class ValidationConfiguration
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        // Ensure FluentValidation auto-validation runs before controller actions
        services.AddFluentValidationAutoValidation();

        // Register validators from the Application layer (ensure CreateJobRequestValidator exists there)
        services.AddValidatorsFromAssemblyContaining<CreateJobRequestValidator>();

        // Configure a consistent RFC-7807 style response for validation failures
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });
        return services;
    }
}
