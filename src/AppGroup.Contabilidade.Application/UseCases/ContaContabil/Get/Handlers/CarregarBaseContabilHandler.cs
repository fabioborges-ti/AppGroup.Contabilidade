using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get.Handlers;

public class CarregarBaseContabilHandler : Handler<ListarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public CarregarBaseContabilHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(ListarContaContabilRequest request)
    {
        try
        {
            request.Contas = await _repository.ListarContas();
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }
}
