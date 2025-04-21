using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class ChecaContaPaiHandler : Handler<CriarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public ChecaContaPaiHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            var codigoConta = request.Codigo;
            var nivel = request.Nivel;
            var contaPai = string.Empty;

            var partes = codigoConta.Split('.');

            contaPai = nivel switch
            {
                1 => codigoConta,
                2 => partes[0],
                3 => $"{partes[0]}.{partes[1]}",
                _ => throw new Exception("Formato inválido.")
            };

            if (nivel > 1)
            {
                var existeContaPai = await _repository.ExisteCodigo(contaPai);

                if (!existeContaPai)
                    throw new Exception("Conta-pai não localizada");

                var dadosContaPai = await _repository.PesquisarContaPorCodigo(contaPai);

                if (dadosContaPai.Tipo != request.Tipo)
                    throw new Exception("Tipo da conta deve ser igual ao da Conta-pai");

                if (dadosContaPai.AceitaLancamentos)
                    throw new Exception("Conta-pai informada não aceita cadastro de contas-filhas");
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }

        await _successor!.Process(request);
    }
}
