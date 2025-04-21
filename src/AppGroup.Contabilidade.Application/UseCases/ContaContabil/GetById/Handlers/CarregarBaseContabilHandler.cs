using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById.Handlers;

public class CarregarBaseContabilHandler : Handler<ListarPorIdContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public CarregarBaseContabilHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(ListarPorIdContaContabilRequest request)
    {
        try
        {
            request.Conta = await _repository.BuscarContaPorId(request.IdConta);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }
}
