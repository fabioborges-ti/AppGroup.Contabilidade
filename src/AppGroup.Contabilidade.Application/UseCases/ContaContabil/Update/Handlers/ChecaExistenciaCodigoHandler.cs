using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaExistenciaCodigoHandler : Handler<EditarContaContabilRequest>
{
    private readonly ILogger _logger;

    private readonly IContaContabilRepository _repository;

    public ChecaExistenciaCodigoHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<ChecaExistenciaCodigoHandler>();
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de existência do código.");

        try
        {
            var existe = await _repository.BuscarContaPorId(request.Id) ?? throw new ContaContabilValidationException("Registro de conta não encontrado");

            if (_successor != null)
                await _successor.Process(request);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar a existência do código: {Codigo}", request.Codigo);

            return;
        }
    }
}