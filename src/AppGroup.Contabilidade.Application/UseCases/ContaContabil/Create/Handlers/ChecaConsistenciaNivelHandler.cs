using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaConsistenciaNivelHandler : Handler<CriarContaContabilRequest>
{
    private readonly ILogger _logger;

    public ChecaConsistenciaNivelHandler()
    {
        _logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<ChecaConsistenciaNivelHandler>();
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de consistência do nível.");

        try
        {
            if (request.Nivel == 3 && !request.AceitaLancamentos)
                throw new ContaContabilValidationException("Este nível de conta-contábil deve receber lançamentos");

            if (_successor is not null)
                await _successor.Process(request);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar a consistência do nível: {Nivel}", request.Nivel);

            return;
        }
    }
}
