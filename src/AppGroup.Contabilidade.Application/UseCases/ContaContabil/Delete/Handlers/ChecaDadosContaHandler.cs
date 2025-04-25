using AppGroup.Contabilidade.Application.Common.Exceptions;
using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete.Handlers;

public class ChecaDadosContaHandler : Handler<DeletarContaContabilRequest>
{
    private readonly ILogger _logger;

    private readonly IContaContabilRepository _repository;

    public ChecaDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                   .Create(builder => builder.AddConsole())
                   .CreateLogger<ChecaDadosContaHandler>();
    }

    public override async Task Process(DeletarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de dados da conta contábil.");

        try
        {
            var conta = await _repository.BuscarContaPorId(request.Id) ?? throw new NotFoundException("Conta contábil não encontrada");

            var contasFilhas = await _repository.PesquisarFilhosPorId(conta.Id);

            if (contasFilhas.Count > 0)
                throw new ContaContabilValidationException("Não é possível excluir uma conta que possui contas filhas associadas");

            if (_successor != null)
                await _successor.Process(request);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar os dados da conta contábil: {Id}", request.Id);

            return;
        }
    }
}
