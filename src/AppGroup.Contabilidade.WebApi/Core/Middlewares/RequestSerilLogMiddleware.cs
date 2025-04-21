namespace AppGroup.Contabilidade.WebApi.Core.Middlewares;

public class RequestSerilLogMiddleware
{
    private readonly RequestDelegate _next;

    public RequestSerilLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        return _next.Invoke(context);
    }
}
