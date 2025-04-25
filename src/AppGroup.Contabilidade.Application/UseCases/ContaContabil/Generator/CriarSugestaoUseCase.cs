using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;

public class CriarSugestaoUseCase : IRequestHandler<CriarSugestaoRequest, CriarSugestaoResponse>
{
    private readonly IContaContabilRepository _repository;

    public CriarSugestaoUseCase(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public async Task<CriarSugestaoResponse> Handle(CriarSugestaoRequest request, CancellationToken cancellationToken)
    {
        var h1 = new GerarSugestaoHandler(_repository);

        await h1.Process(request);

        return new CriarSugestaoResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : request.Codigo
        };
    }
}