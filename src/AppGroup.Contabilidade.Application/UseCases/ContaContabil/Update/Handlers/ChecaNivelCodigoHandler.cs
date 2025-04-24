using AppGroup.Contabilidade.Application.Common.Handlers;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

public class ChecaNivelCodigoHandler : Handler<EditarContaContabilRequest>
{
    public override async Task Process(EditarContaContabilRequest request)
    {
        var codigoConta = request.Codigo;

        if (string.IsNullOrWhiteSpace(codigoConta))
            throw new ArgumentException("Codigo deve ser informado");

        try
        {
            var ehCodigoPai = codigoConta.Contains('.');

            if (!ehCodigoPai)
            {
                request.Nivel = 1;
            }
            else
            {
                request.Nivel = request.Codigo.Count(c => c == '.') + 1;
            }
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }

        if (_successor is not null)
            await _successor!.Process(request);
    }
}
