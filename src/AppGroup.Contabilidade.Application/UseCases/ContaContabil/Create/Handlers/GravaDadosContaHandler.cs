using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create.Handlers;

public class GravaDadosContaHandler : Handler<CriarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public GravaDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(CriarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            var conta = new CriarContaContabilModel
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                Tipo = request.Tipo == TipoConta.Receitas ? 1 : 2,
                AceitaLancamentos = request.AceitaLancamentos ? 1 : 0,
                IdPai = request.IdPai
            };

            await _repository.CriarContaContabil(conta);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }
}
