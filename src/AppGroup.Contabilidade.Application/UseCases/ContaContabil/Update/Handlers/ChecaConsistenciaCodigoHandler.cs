using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Application.Exceptions;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaConsistenciaCodigoHandler : Handler<EditarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    private readonly ILogger _logger;

    public ChecaConsistenciaCodigoHandler(IContaContabilRepository repository)
    {
        _repository = repository;

        _logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger<ChecaConsistenciaCodigoHandler>();
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        _logger.LogInformation("Iniciando a validação de consistência do código.");

        try
        {
            if (request.Nivel is 1 or 2)
            {
                await VerificarNivel1e2(request);
            }
            else if (request.Nivel == 3)
            {
                if (!request.AceitaLancamentos)
                    throw new ContaContabilValidationException("Tipo não pode ser alterado, porque tem registros filhos cadastrados");
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;

            _logger.LogError(ex, "Erro ao validar a consistência do código: {Codigo}", request.Codigo);

            return;
        }
    }

    private async Task VerificarNivel1e2(EditarContaContabilRequest request)
    {
        var conta = await _repository.BuscarContaPorId(request.Id);
        var filhos = await _repository.PesquisarFilhosPorId(conta.Id);

        if (filhos.Count > 0 && conta.Tipo != request.Tipo)
            throw new ContaContabilValidationException("Tipo não pode ser alterado, porque há registros filhos cadastrados na base");

        if (request.Nivel == 2)
        {
            var codigoPai = request.Codigo.Split('.')[0];
            var contaPai = await _repository.PesquisarContaPorCodigo(codigoPai);

            if (request.Tipo != contaPai.Tipo)
                throw new Exception("Tipo não pode ser alterado porque está diferente da Conta-pai");
        }

        if (_successor != null)
            await _successor.Process(request);
    }
}