using AppGroup.Contabilidade.Application.Common.Exceptions;
using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete.Handlers;

public class ChecaDadosContaHandler : Handler<DeletarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public ChecaDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(DeletarContaContabilRequest request)
    {
        try
        {
            var idConta = request.Id;

            var conta = await _repository.BuscarContaPorId(idConta) ?? throw new NotFoundException("Conta contábil não encontrada");

            var contasFilhas = await _repository.PesquisarFilhosPorId(conta.Id);

            if (contasFilhas.Count > 0)
            {
                throw new Exception("Não é possível excluir uma conta que possui contas filhas associadas");
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