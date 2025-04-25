using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete.Handlers;

public class DeletarContaContabilHandler : Handler<DeletarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public DeletarContaContabilHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(DeletarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            await _repository.DeletarContaContabil(request.Id);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }
}