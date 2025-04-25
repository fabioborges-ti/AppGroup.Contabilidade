using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;

public class EditarContaContabilUseCase : IRequestHandler<EditarContaContabilRequest, EditarContaContabilResponse>
{
    private readonly IContaContabilRepository _repository;

    private readonly ILogger _logger;

    public EditarContaContabilUseCase(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger<EditarContaContabilUseCase>();
    }

    public async Task<EditarContaContabilResponse> Handle(EditarContaContabilRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando o processamento da edição da conta contábil.");

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