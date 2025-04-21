using AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById;

public class ListarPorIdContaContabilUseCase : IRequestHandler<ListarPorIdContaContabilRequest, ListarPorIdContaContabilResponse>
{
    private readonly IContaContabilRepository _repository;

    public ListarPorIdContaContabilUseCase(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListarPorIdContaContabilResponse> Handle(ListarPorIdContaContabilRequest request, CancellationToken cancellationToken)
    {
        var h1 = new CarregarBaseContabilHandler(_repository);

        await h1.Process(request);

        return new ListarPorIdContaContabilResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : request.Conta
        };
    }
}
