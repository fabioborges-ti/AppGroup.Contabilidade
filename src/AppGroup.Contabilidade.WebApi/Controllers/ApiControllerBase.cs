using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppGroup.Contabilidade.WebApi.Controllers;

/// <summary>
/// Controlador base da API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : Controller
{
    private ISender _mediator = null!;

    /// <summary>
    /// Intermediador responsável por receber uma requisição e invocar o manipulador associado.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}