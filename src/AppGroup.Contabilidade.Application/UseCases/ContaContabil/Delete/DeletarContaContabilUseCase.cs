using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete;

public class DeletarContaContabilUseCase : IRequestHandler<DeletarContaContabilRequest, DeletarContaContabilResponse>
{
    private readonly IContaContabilRepository _repository;

    public DeletarContaContabilUseCase(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeletarContaContabilResponse> Handle(DeletarContaContabilRequest request, CancellationToken cancellationToken)
    {
        var h1 = new ChecaDadosContaHandler(_repository);
        var h2 = new DeletarContaContabilHandler(_repository);

        h1.SetSuccessor(h2);

        await h1.Process(request);

        return new DeletarContaContabilResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : true
        };
    }
}
