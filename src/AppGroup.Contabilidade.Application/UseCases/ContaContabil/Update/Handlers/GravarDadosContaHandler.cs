using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class GravarDadosContaHandler : Handler<EditarContaContabilRequest>
{
    private readonly IContaContabilRepository _repository;

    public GravarDadosContaHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(EditarContaContabilRequest request)
    {
        if (request.HasError) return;

        try
        {
            var conta = new EditarContaContabilModel
            {
                Id = request.Id,
                Codigo = request.Codigo,
                Nome = request.Nome,
                Tipo = request.Tipo == TipoConta.Receitas ? 1 : 2,
                AceitaLancamentos = request.AceitaLancementos ? 1 : 0
            };

            await _repository.EditarContaContabil(conta);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }
}
