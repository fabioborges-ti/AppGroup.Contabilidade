using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaContaPaiHandler : Handler<CriarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    private readonly ILogger _logger;

    public ChecaContaPaiHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<ChecaContaPaiHandler>();
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de conta-pai.");

        if (request.HasError) return;

        try
        {
            var codigoConta = request.Codigo;
            var nivel = request.Nivel;
            var contaPai = string.Empty;

            var partes = codigoConta.Split('.');

            contaPai = nivel switch
            {
                1 => codigoConta,
                2 => partes[0],
                3 => $"{partes[0]}.{partes[1]}",
                _ => throw new Exception("Formato inválido.")
            };

            if (nivel > 1)
            {
                var existeContaPai = await _repository.ExisteCodigo(contaPai);

                if (!existeContaPai)
                    throw new Exception("Conta-pai não localizada");

                var dadosContaPai = await _repository.PesquisarContaPorCodigo(contaPai);

                if (dadosContaPai.Tipo != request.Tipo)
                    throw new Exception("Tipo da conta deve ser igual ao da Conta-pai");

                if (dadosContaPai.AceitaLancamentos)
                    throw new Exception("Conta-pai informada não aceita cadastro de contas-filhas");
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar a conta-pai: {Codigo}", request.Codigo);
        }

        if (_successor is not null)
            await _successor!.Process(request);
    }
}