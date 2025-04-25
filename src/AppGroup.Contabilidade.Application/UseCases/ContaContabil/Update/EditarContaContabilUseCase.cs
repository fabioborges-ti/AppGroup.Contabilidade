using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;

public class EditarContaContabilUseCase : IRequestHandler<EditarContaContabilRequest, EditarContaContabilResponse>
{
    private readonly IContaContabilRepository _repository;

    public EditarContaContabilUseCase(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public async Task<EditarContaContabilResponse> Handle(EditarContaContabilRequest request, CancellationToken cancellationToken)
    {
        var h1 = new ChecaNivelCodigoHandler();
        var h2 = new ChecaExistenciaCodigoHandler(_repository);
        var h3 = new ChecaConsistenciaCodigoHandler(_repository);
        var h4 = new GravarDadosContaHandler(_repository);

        h1.SetSuccessor(h2);
        h2.SetSuccessor(h3);
        h3.SetSuccessor(h4);

        await h1.Process(request);

        return new EditarContaContabilResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : true
        };
    }
}