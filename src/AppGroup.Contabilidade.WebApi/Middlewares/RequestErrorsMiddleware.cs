using System.Net;
using System.Text.Json;

namespace AppGroup.Contabilidade.WebApi.Middlewares;

public class RequestErrorsMiddleware
{
    private readonly RequestDelegate _next;

    public RequestErrorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;

            response.ContentType = "application/json";

            switch (error)
            {
                case KeyNotFoundException e:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = error?.Message });

            await response.WriteAsync(result);
        }
    }
}

public static class ErrorHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestErrors(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestErrorsMiddleware>();
    }
}
