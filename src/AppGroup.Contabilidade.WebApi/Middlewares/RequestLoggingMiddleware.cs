namespace AppGroup.Contabilidade.WebApi.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Log da requisição
        _logger.LogInformation("Request: {method} {path}", context.Request.Method, context.Request.Path);

        // Guarda o corpo da requisição para logar posteriormente
        Stream originalBody = context.Response.Body;
        try
        {
            using var memStream = new MemoryStream();

            context.Response.Body = memStream;

            await _next(context);

            // Loga o corpo da resposta
            memStream.Position = 0;

            string responseBody = await new StreamReader(memStream).ReadToEndAsync();

            _logger.LogInformation("Response: {body}", responseBody);

            memStream.Position = 0;

            await memStream.CopyToAsync(originalBody);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
