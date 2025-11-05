using BancoAPI.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BancoAPI.API.Middleware
{
    /// <summary>
    /// Middleware global para manejo de excepciones 
    /// Excepciones de dominio en respuestas HTTP apropiadas
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                // Excepciones de negocio con mensajes específicos
                case SaldoInsuficienteException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                case CupoDiarioExcedidoException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                case EntidadNoEncontradaException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                case EntidadDuplicadaException:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                case OperacionInvalidaException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                case DomainException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;

                // Excepciones generales
                default:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Error interno del servidor",
                        detail = exception.Message
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Extensión para registrar el middleware
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
