using AppGroup.Contabilidade.Application.Common.Handlers;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaNivelCodigoHandler : Handler<EditarContaContabilRequest>
{
    private readonly ILogger _logger;

    public ChecaNivelCodigoHandler()
    {
        _logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger<ChecaNivelCodigoHandler>();
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de nível do código.");

        try
        {
            var codigoConta = request.Codigo;

            if (string.IsNullOrWhiteSpace(codigoConta))
                throw new ArgumentException("Codigo deve ser informado");

            var ehCodigoPai = codigoConta.Contains('.');

            if (!ehCodigoPai)
            {
                request.Nivel = 1;
            }
            else
            {
                request.Nivel = request.Codigo.Count(c => c == '.') + 1;
            }

            if (_successor != null)
                await _successor.Process(request);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar o nível do código: {Codigo}", request.Codigo);

            return;
        }
    }
}