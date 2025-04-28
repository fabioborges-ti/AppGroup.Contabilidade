using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories;

public class ContaContabilRepository : BaseRepository, IContaContabilRepository
{
    public ContaContabilRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> GerarCodigoPai()
    {
        await OpenConnectionAsync();

        const string query = @"
            SELECT TOP 1 (c.Codigo + 1) AS sugestao
            FROM ContasContabeis c
            WHERE c.IdPai IS NULL
            ORDER BY c.Codigo DESC";

        var result = await Connection.QueryFirstOrDefaultAsync<int?>(query);

        await CloseConnectionAsync();

        return result ?? 1;
    }

    public async Task<bool> ExisteCodigo(string codigo)
    {
        await OpenConnectionAsync();

        const string query = "SELECT 1 FROM ContasContabeis WHERE Codigo = @Codigo";

        var result = await Connection.QueryFirstOrDefaultAsync<int?>(query, new { Codigo = codigo });

        await CloseConnectionAsync();

        return result.HasValue;
    }

    public async Task<List<(string, bool)>> PesquisarFilhosPorId(Guid? idContaPai)
    {
        await OpenConnectionAsync();

        const string query = @"
            SELECT Codigo, AceitaLancamentos
            FROM ContasContabeis
            WHERE IdPai = @IdPai
            ORDER BY Codigo DESC";

        var result = await Connection.QueryAsync<(string, bool)>(query, new { IdPai = idContaPai });

        await CloseConnectionAsync();

        return [.. result];
    }

    public async Task<(string, bool)> PesquisarPaiPorId(Guid? idConta)
    {
        await OpenConnectionAsync();

        const string query = "SELECT Codigo, AceitaLancamentos FROM ContasContabeis WHERE Id = @Id";

        var result = await Connection.QueryFirstOrDefaultAsync<(string, bool)>(query, new { Id = idConta });

        await CloseConnectionAsync();

        return result;
    }

    public async Task<ContaContabilModel> BuscarContaPorId(Guid idConta)
    {
        await OpenConnectionAsync();

        const string query = @"
            SELECT Id, Codigo, Nome, Tipo, AceitaLancamentos
            FROM ContasContabeis
            WHERE Id = @Id";

        var result = await Connection.QueryFirstOrDefaultAsync<ContaContabilModel>(query, new { Id = idConta });

        await CloseConnectionAsync();

        return result!;
    }

    public async Task CriarContaContabil(CriarContaContabilModel model)
    {
        await OpenConnectionAsync();

        const string query = @"
            INSERT INTO ContasContabeis (Id, Codigo, Nome, Tipo, AceitaLancamentos, IdPai)
            VALUES (NEWID(), @Codigo, @Nome, @Tipo, @AceitaLancamentos, @IdPai)";

        await Connection.ExecuteAsync(query, model);

        await CloseConnectionAsync();
    }

    public async Task<ContaContabilModel> PesquisarContaPorCodigo(string codigo)
    {
        await OpenConnectionAsync();

        const string query = "SELECT * FROM ContasContabeis WHERE Codigo = @Codigo";

        var result = await Connection.QueryFirstOrDefaultAsync<ContaContabilModel>(query, new { Codigo = codigo });

        await CloseConnectionAsync();

        return result!;
    }

    public async Task EditarContaContabil(EditarContaContabilModel model)
    {
        await OpenConnectionAsync();

        const string query = @"
            UPDATE ContasContabeis
            SET Nome = @Nome, Tipo = @Tipo, AceitaLancamentos = @AceitaLancamentos
            WHERE Id = @Id";

        await Connection.ExecuteAsync(query, model);

        await CloseConnectionAsync();
    }

    public async Task DeletarContaContabil(Guid idConta)
    {
        await OpenConnectionAsync();

        const string query = "DELETE FROM ContasContabeis WHERE Id = @Id";

        await Connection.ExecuteAsync(query, new { Id = idConta });

        await CloseConnectionAsync();
    }

    public async Task<List<ContaContabilModel>> ListarContas()
    {
        await OpenConnectionAsync();

        var query = @$"WITH ContasHierarquia AS
                        (
                            SELECT 
                                Id,
                                Codigo,
                                Nome,
                                Tipo,
                                AceitaLancamentos,
                                IdPai,
                                0 AS Nivel,
                                CAST(Codigo AS VARCHAR(MAX)) AS CaminhoOrdenacao
                            FROM
                                ContasContabeis
                            WHERE
                                IdPai IS NULL -- Contas Pai

                            UNION ALL

                            SELECT
                                cc.Id,
                                cc.Codigo,
                                cc.Nome,
                                cc.Tipo,
                                cc.AceitaLancamentos,
                                cc.IdPai,
                                ch.Nivel + 1,
                                CAST(ch.CaminhoOrdenacao + '-' + cc.Codigo AS VARCHAR(MAX))
                            FROM
                                ContasContabeis cc
                                INNER JOIN ContasHierarquia ch ON cc.IdPai = ch.Id
                        )

                        SELECT Id, Codigo, Nome, Tipo, AceitaLancamentos, IdPai, Nivel FROM ContasHierarquia ORDER BY CaminhoOrdenacao; ";

        var data = await Connection.QueryAsync<ContaContabilModel>(query);

        await CloseConnectionAsync();

        return [.. data];
    }
}