using AppGroup.Contabilidade.Application.Common.Handlers;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;

namespace AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator.Handlers;

public class GerarSugestaoHandler : Handler<CriarSugestaoRequest>
{
    private readonly IContaContabilRepository _repository;

    public GerarSugestaoHandler(IContaContabilRepository repository)
    {
        _repository = repository;
    }

    public override async Task Process(CriarSugestaoRequest request)
    {
        try
        {
            request.Codigo = request.IdPai is null
                ? await GerarCodigoPaiAsync()
                : await GerarCodigoFilhoAsync(request.IdPai);
        }
        catch (Exception ex)
        {
            request.HasError = true;
            request.ErrorMessage = ex.Message;
        }
    }

    private async Task<string> GerarCodigoPaiAsync()
    {
        var codigoPai = await _repository.GerarCodigoPai();

        return codigoPai.ToString();
    }

    private async Task<string> GerarCodigoFilhoAsync(Guid? idPai)
    {
        var data = await _repository.PesquisarPaiPorId(idPai);

        var codigoPai = data.Item1;
        var aceitaLancamentos = data.Item2;

        if (string.IsNullOrEmpty(codigoPai))
            throw new InvalidOperationException("Código-pai não encontrado");

        if (aceitaLancamentos)
            throw new InvalidOperationException("Código-pai não permite criação de filhos");

        var codigosFilhos = await _repository.PesquisarFilhosPorId(idPai);

        if (codigosFilhos.Count == 0)
            return $"{codigoPai}.1";

        return await GerarCodigoFilhoComBaseNosFilhos(codigosFilhos, codigoPai);
    }

    private async Task<string> GerarCodigoFilhoComBaseNosFilhos(List<(string, bool)> codigosFilhos, string codigoPai)
    {
        var listaFilhos = new List<int>();
        int nivel = 0;

        foreach (var item in codigosFilhos)
        {
            var codigo = item.Item1;
            var aceitaLancamento = item.Item2;

            if (aceitaLancamento)
            {
                var partes = codigo.Split(".");
                nivel = partes.Length;

                switch (nivel)
                {
                    case 2:
                        if (int.TryParse(partes[1], out int nivel2))
                            listaFilhos.Add(nivel2);
                        break;

                    case 3:
                        if (int.TryParse(partes[2], out int nivel3))
                            listaFilhos.Add(nivel3);
                        break;
                }
            }
        }

        var proximo = listaFilhos.Count != 0 ? listaFilhos.Max() + 1 : 1;

        if (nivel == 2)
        {
            return $"{codigoPai}.{proximo}";
        }

        if (nivel == 3 && proximo == 1000)
        {
            var novoPai = await EncontrarProximoCodigoPaiDisponivelAsync(codigoPai);
            return novoPai;
        }

        return $"{codigoPai}.{proximo}";
    }

    private async Task<string> EncontrarProximoCodigoPaiDisponivelAsync(string codigoPai)
    {
        var partes = codigoPai.Split(".");
        var prefixo = partes[0];
        var numeroAtual = int.Parse(partes[1]);

        int tentativa = 1;

        while (true)
        {
            var proximoCodigoPai = $"{prefixo}.{numeroAtual + tentativa}";

            var existe = await _repository.ExisteCodigo(proximoCodigoPai);

            if (!existe)
                return proximoCodigoPai;

            tentativa++;
        }
    }
}