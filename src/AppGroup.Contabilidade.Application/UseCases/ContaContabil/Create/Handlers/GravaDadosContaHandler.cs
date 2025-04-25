using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class GravaDadosContaHandler : Handler<CriarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    private readonly ILogger _logger;

    public GravaDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<GravaDadosContaHandler>();
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a gravação de dados da conta contábil.");

        if (request.HasError) return;

        try
        {
            var conta = new CriarContaContabilModel
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                Tipo = (int)TipoConta.Receitas,
                AceitaLancamentos = request.AceitaLancamentos ? 1 : 0,
                IdPai = request.IdPai == Guid.Empty || request.IdPai == null
                            ? null
                            : request.IdPai,
            };

            await _repository.CriarContaContabil(conta);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao gravar os dados da conta contábil: {Codigo}", request.Codigo);
        }

        if (_successor is not null)
            await _successor!.Process(request);
    }
}