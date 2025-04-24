using AppGroup.Contabilidade.Application.Common.Handlers;
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
            var codigoConta = request.Codigo;

            var existeCodigo = await _repository.ExisteCodigo(codigoConta);

            if (existeCodigo)
                throw new Exception("Código já cadastrado");
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar a existência do código.");
        }

        if (_successor is not null)
            await _successor!.Process(request);
    }
}
