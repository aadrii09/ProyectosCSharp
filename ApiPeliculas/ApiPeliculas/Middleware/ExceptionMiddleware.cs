using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ApiPeliculas.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                var statusCode = HttpStatusCode.InternalServerError;
                var message = "Error interno del servidor";

                // Detectar errores relacionados con la base de datos
                if (ex is SqlException || ex.InnerException is SqlException ||
                    ex is DbUpdateException || ex.InnerException is DbUpdateException)
                {
                    context.Response.StatusCode = 666; // Código personalizado 666
                    statusCode = (HttpStatusCode)666; // Para mantener consistencia en el objeto response
                    message = "La base de datos esta malita. Por favor, inténtelo de nuevo más tarde.";
                }

                var response = new
                {
                    statusCode = (int)statusCode,
                    message = message,
                    details = _env.IsDevelopment() ? ex.StackTrace : null
                };

                context.Response.StatusCode = (int)statusCode;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }

    // Extension method para usar el middleware fácilmente
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
