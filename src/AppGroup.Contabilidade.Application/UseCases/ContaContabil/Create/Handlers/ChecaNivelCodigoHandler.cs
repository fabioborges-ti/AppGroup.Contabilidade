using AppGroup.Contabilidade.Application.Common.Handlers;
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
        if (request.HasError) return;

        try
        {
            if (request.IdPai is null || request.IdPai == Guid.Empty)
            {
                if (request.Codigo.Contains('.'))
                    throw new Exception("Para cadastro de Conta-pai não deve informar sub-niveis.");

                if (request.AceitaLancamentos)
                    throw new Exception("Conta-pai não deve aceitar lançamentos.");

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
        }

        if (_successor is not null)
            await _successor!.Process(request);
    }
}
