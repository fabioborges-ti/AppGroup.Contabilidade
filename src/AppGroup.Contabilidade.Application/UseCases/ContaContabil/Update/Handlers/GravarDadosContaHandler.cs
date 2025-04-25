using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class GravarDadosContaHandler : Handler<EditarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    private readonly ILogger _logger;

    public GravarDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger<GravarDadosContaHandler>();
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a gravação dos dados da conta.");

        try
        {
            var conta = new EditarContaContabilModel
            {
                Id = request.Id,
                Codigo = request.Codigo,
                Nome = request.Nome,
                Tipo = (int)request.Tipo,
                AceitaLancamentos = request.AceitaLancamentos ? 1 : 0
            };

            await _repository.EditarContaContabil(conta);

            if (_successor != null)
                await _successor.Process(request);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao gravar os dados da conta: {Codigo}", request.Codigo);

            return;
        }
    }
}