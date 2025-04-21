using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaExistenciaCodigoHandler : Handler<EditarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public ChecaExistenciaCodigoHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            var existe = await _repository.BuscarContaPorId(request.Id) ?? throw new ArgumentException("Registro de conta não encontrado");
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }

        await _successor!.Process(request);
    }
}
