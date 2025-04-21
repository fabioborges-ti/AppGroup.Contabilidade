using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaConsistenciaCodigoHandler : Handler<EditarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public ChecaConsistenciaCodigoHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            if (request.Nivel is 1 or 2)
            {
                await VerificarNivel1e2(request);
            }
            else if (request.Nivel == 3)
            {
                if (!request.AceitaLancementos)
                    throw new ArgumentException("Tipo não pode ser alterado, porque tem registros filhos cadastrados");
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }

        await _successor!.Process(request);
    }

    private async Task VerificarNivel1e2(EditarContaContabilRequest request)
    {
        var conta = await _repository.BuscarContaPorId(request.Id);
        var filhos = await _repository.PesquisarFilhosPorId(conta.Id);

        if (filhos.Count > 0 && conta.Tipo != request.Tipo)
            throw new ArgumentException("Tipo não pode ser alterado, porque há registros filhos cadastrados na base");

        if (request.Nivel == 2)
        {
            var codigoPai = request.Codigo.Split('.')[0];
            var contaPai = await _repository.PesquisarContaPorCodigo(codigoPai);

            if (request.Tipo != contaPai.Tipo)
                throw new ArgumentException("Tipo não pode ser alterado porque está diferente da Conta-pai");
        }
    }
}
