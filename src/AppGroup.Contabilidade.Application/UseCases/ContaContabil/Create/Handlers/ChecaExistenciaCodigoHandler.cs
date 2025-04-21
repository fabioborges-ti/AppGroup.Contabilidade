using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaExistenciaCodigoHandler : Handler<CriarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public ChecaExistenciaCodigoHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        try
        {
            var codigoConta = request.Codigo;

            var existeCodigo = await _repository.ExisteCodigo(codigoConta);

            if (existeCodigo)
                throw new Exception("Código já cadastrado");
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }

        await _successor!.Process(request);
    }
}
