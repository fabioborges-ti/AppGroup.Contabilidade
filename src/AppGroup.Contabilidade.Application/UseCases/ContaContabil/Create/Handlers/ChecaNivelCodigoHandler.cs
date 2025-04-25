using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaNivelCodigoHandler : Handler<CriarContaContabilRequest>
{
    private readonly ILogger _logger;

    public ChecaNivelCodigoHandler()
    {
        _logger = LoggerFactory
                   .Create(builder => builder.AddConsole())
                   .CreateLogger<ChecaExistenciaCodigoHandler>();
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de nível do código.");

        try
        {
            if (request.IdPai is null || request.IdPai == Guid.Empty)
            {
                if (request.Codigo.Contains('.'))
                    throw new ContaContabilValidationException("Para cadastro de Conta-pai não deve informar sub-niveis.");

                if (request.AceitaLancamentos)
                    throw new ContaContabilValidationException("Conta-pai não deve aceitar lançamentos.");

                request.Nivel = 1;
            }
            else
            {
                request.Nivel = request.Codigo.Count(c => c == '.') + 1;
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar o nível do código: {Codigo}", request.Codigo);

            return;
        }

        if (_successor != null)
            await _successor.Process(request);
    }
}