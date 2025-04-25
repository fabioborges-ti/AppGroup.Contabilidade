using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaExistenciaCodigoHandler : Handler<CriarContaContabilRequest>
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

    public override async Task Process(CriarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de existência de código.");

        try
        {
            if (await _repository.ExisteCodigo(request.Codigo))
            {
                request.HasError = true;
                request.ErrorMessage = "Código já cadastrado";

                _logger.LogWarning("Código de conta contábil já existente: {Codigo}", request.Codigo);

                return;
            }

            _logger.LogInformation("Validação de existência de código concluída com sucesso");

            if (_successor != null)
                await _successor.Process(request);
        }
        catch (Exception ex) when (ex is not ContaContabilValidationException)
        {
            request.HasError = true;
            request.ErrorMessage = "Erro ao validar a existência do código";

            _logger.LogError(ex, "Erro ao validar a existência do código para conta: {Codigo}", request.Codigo);

            return;
        }
    }
}
