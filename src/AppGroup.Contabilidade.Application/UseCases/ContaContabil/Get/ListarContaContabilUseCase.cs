using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get;

public class ListarContaContabilUseCase : IRequestHandler<ListarContaContabilRequest, ListarContaContabilResponse>
{
    private readonly IContaContabilRepository _repository;

    public ListarContaContabilUseCase(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListarContaContabilResponse> Handle(ListarContaContabilRequest request, CancellationToken cancellationToken)
    {
        var h1 = new CarregarBaseContabilHandler(_repository);

        await h1.Process(request);

        return new ListarContaContabilResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : request.Contas
        };
    }
}
