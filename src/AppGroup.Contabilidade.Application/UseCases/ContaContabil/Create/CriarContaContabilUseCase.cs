using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;

public class CriarContaContabilUseCase : IRequestHandler<CriarContaContabilRequest, CriarContaContabilResponse>
{
    private readonly ILogger<CriarContaContabilUseCase> _logger;
    private readonly IContaContabilRepository _repository;

    public CriarContaContabilUseCase(ILogger<CriarContaContabilUseCase> logger, IContaContabilRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<CriarContaContabilResponse> Handle(CriarContaContabilRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o cadastro de conta contábil.");

        var h1 = new ChecaExistenciaCodigoHandler(_repository);
        var h2 = new ChecaNivelCodigoHandler();
        var h3 = new ChecaConsistenciaNivelHandler();
        var h4 = new ChecaContaPaiHandler(_repository);
        var h5 = new GravaDadosContaHandler(_repository);

        h1.SetSuccessor(h2);
        h2.SetSuccessor(h3);
        h3.SetSuccessor(h4);
        h4.SetSuccessor(h5);

        await h1.Process(request);

        return new CriarContaContabilResponse
        {
            Data = request.HasError
                    ? request.ErrorMessage
                    : request.Codigo
        };
    }
}