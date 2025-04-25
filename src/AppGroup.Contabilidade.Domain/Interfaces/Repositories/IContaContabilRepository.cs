using AppGroup.Contabilidade.Domain.Models.ContaContabil;

namespace AppGroup.Contabilidade.Domain.Interfaces.Repositories;

public interface IContaContabilRepository
{
    Task<int> GerarCodigoPai();

    Task<List<(string, bool)>> PesquisarFilhosPorId(Guid? idContaPai);

    Task<(string, bool)> PesquisarPaiPorId(Guid? idConta);

    Task<bool> ExisteCodigo(string codigo);

    Task<List<ContaContabilModel>> ListarContas();

    Task<ContaContabilModel> BuscarContaPorId(Guid idConta);

    Task CriarContaContabil(CriarContaContabilModel model);

    Task<ContaContabilModel> PesquisarContaPorCodigo(string codigo);

    Task EditarContaContabil(EditarContaContabilModel model);

    Task DeletarContaContabil(Guid idConta);
}