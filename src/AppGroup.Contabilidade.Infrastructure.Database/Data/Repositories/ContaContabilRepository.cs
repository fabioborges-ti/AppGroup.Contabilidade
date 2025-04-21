using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories;

public class ContaContabilRepository : BaseRepository, IContaContabilRepository
{
    public ContaContabilRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> GerarCodigoPai()
    {
        await OpenConnectionAsync();

        var query = "select top 1 ( c.Codigo + 1 ) as sugestao from ContasContabeis as c where c.IdPai is null order by c.codigo desc ";

        var codigo = await Connection.QueryAsync<int>(query);

        await CloseConnectionAsync();

        return codigo.FirstOrDefault();
    }

    public async Task<bool> ExisteCodigo(string codigo)
    {
        await OpenConnectionAsync();

        var query = $"select 1 from ContasContabeis as c where c.Codigo = '{codigo}' ";

        var data = await Connection.QueryAsync<int>(query);

        await CloseConnectionAsync();

        return data.Any();
    }

    public async Task<List<(string, bool)>> PesquisarFilhosPorId(Guid? idContaPai)
    {
        await OpenConnectionAsync();

        var query = $"select c.Codigo, c.AceitaLancamentos from ContasContabeis as c where c.IdPai = '{idContaPai}' order by c.codigo desc ";

        var data = await Connection.QueryAsync<(string, bool)>(query);

        await CloseConnectionAsync();

        return [.. data];
    }

    public async Task<(string, bool)> PesquisarPaiPorId(Guid? idConta)
    {
        await OpenConnectionAsync();

        var query = $"select c.Codigo, c.AceitaLancamentos from ContasContabeis as c where c.Id = '{idConta}' ";

        var data = await Connection.QueryAsync<(string, bool)>(query);

        await CloseConnectionAsync();

        return data.FirstOrDefault()!;
    }

    public async Task<List<ContaContabilModel>> ListarContas()
    {
        await OpenConnectionAsync();

        var query = @$"WITH Hierarquia AS 
                    (
                     -- Primeiro nível (raiz)
                        SELECT Id, Codigo, Nome, Tipo, AceitaLancamentos, CAST(Codigo AS VARCHAR(MAX)) AS Caminho
                          FROM ContasContabeis 
	                     WHERE IdPai IS NULL
                    
                         UNION ALL
    
                     -- Níveis seguintes
                        SELECT f.Id, f.Codigo, f.Nome, f.Tipo, f.AceitaLancamentos, CAST(h.Caminho + '.' + f.Codigo AS VARCHAR(MAX))
                          FROM ContasContabeis f INNER JOIN Hierarquia h ON f.IdPai = h.Id
                    )

                    SELECT * FROM Hierarquia ORDER BY Caminho; ";

        var data = await Connection.QueryAsync<ContaContabilModel>(query);

        await CloseConnectionAsync();

        return [.. data];
    }

    public async Task<ContaContabilModel> BuscarContaPorId(Guid idConta)
    {
        await OpenConnectionAsync();

        var query = $"select c.Id, c.Codigo, c.Nome, c.Tipo, c.AceitaLancamentos from ContasContabeis as c where c.Id = '{idConta}' ";

        var data = await Connection.QueryAsync<ContaContabilModel>(query);

        await CloseConnectionAsync();

        return data.FirstOrDefault()!;
    }

    public async Task CriarContaContabil(CriarContaContabilModel model)
    {
        await OpenConnectionAsync();

        var query = @$"insert into ContasContabeis(Id, Codigo, Nome, Tipo, AceitaLancamentos, IdPai) 
                       values(NEWID(), '{model.Codigo}', '{model.Nome}', {model.Tipo}, {model.AceitaLancamentos}, '{model.IdPai}'); ";

        await Connection.ExecuteAsync(query);

        await CloseConnectionAsync();
    }

    public async Task<ContaContabilModel> PesquisarContaPorCodigo(string codigo)
    {
        await OpenConnectionAsync();

        var query = $"select * from ContasContabeis as c where c.Codigo = '{codigo}' ";

        var conta = await Connection.QueryAsync<ContaContabilModel>(query);

        await CloseConnectionAsync();

        return conta.FirstOrDefault()!;
    }

    public async Task EditarContaContabil(EditarContaContabilModel model)
    {
        await OpenConnectionAsync();

        var query = @$"update ContasContabeis set Nome = '{model.Nome}', Tipo = {model.Tipo}, AceitaLancamentos = {model.AceitaLancamentos} where Id = '{model.Id}' ";

        await Connection.ExecuteAsync(query);

        await CloseConnectionAsync();
    }

    public async Task DeletarContaContabil(Guid idConta)
    {
        await OpenConnectionAsync();

        var query = $"delete from ContasContabeis where Id = '{idConta}' ";

        await Connection.ExecuteAsync(query);

        await CloseConnectionAsync();
    }

    //public async Task<Guid> Create(CreateMotorcyclesDto motocyle)
    //{
    //    await OpenConnectionAsync();

    //    var queryInsert = @"INSERT INTO public.tb_motorcycles(""Id"", ""Model"", ""PlateNumber"", ""Year"", ""Status"", ""CreatedAt"") VALUES(@Id, @Model, @PlateNumber, @Year, 0, @CreatedAt); ";

    //    await Connection.ExecuteAsync(queryInsert, motocyle);

    //    await CloseConnectionAsync();

    //    return motocyle.Id;
    //}

    //public async Task<GetMotorcyclesPagedDto> GetPaged(int page, int pagesize, int status)
    //{
    //    await OpenConnectionAsync();

    //    var result = new GetMotorcyclesPagedDto();

    //    var query = $@"select a.""Id"", a.""Model"", a.""PlateNumber"", a.""Year"", a.""Status"", a.""CreatedAt"" 
    //                    from public.tb_motorcycles as a 
    //                   where 1 = 1 
    //                     and a.""Status"" = {status} 
    //                   limit @Pagesize 
    //                  offset @Offset; 
    //                  select count(*) from public.tb_motorcycles as b where b.""Status"" = {status}; ";

    //    using var multi = await Connection.QueryMultipleAsync(query,
    //        new
    //        {
    //            Offset = (page - 1) * page,
    //            Pagesize = pagesize
    //        });

    //    var data = multi.Read<MotorcyclesDto>().ToList();

    //    result.Items = data;
    //    result.Total = multi.ReadFirst<int>();
    //    result.Page = page;
    //    result.PageSize = pagesize;
    //    result.TotalPages = Math.Ceiling((double)result.Total / pagesize);

    //    await CloseConnectionAsync();

    //    return result;
    //}

    //public async Task<MotorcyclesDto> GetByPlateNumber(string plateNumber)
    //{
    //    await OpenConnectionAsync();

    //    var queryGet = $@"select * from public.tb_motorcycles where ""PlateNumber"" = '{plateNumber}';";

    //    var result = await Connection.QueryFirstOrDefaultAsync<MotorcyclesDto>(queryGet);

    //    await CloseConnectionAsync();

    //    return result!;
    //}

    //public async Task Delete(string plateNumber)
    //{
    //    await OpenConnectionAsync();

    //    var queryDelete = $@"delete from public.tb_motorcycles where ""PlateNumber"" = '{plateNumber}' ";

    //    await Connection.ExecuteAsync(queryDelete);

    //    await CloseConnectionAsync();
    //}

    //public async Task Update(Guid id, string plateNumber)
    //{
    //    await OpenConnectionAsync();

    //    var queryUpdate = $@"update public.tb_motorcycles set ""PlateNumber"" = '{plateNumber}', ""LastModifiedAt"" = CURRENT_DATE where ""Id"" = '{id}' ";

    //    await Connection.ExecuteAsync(queryUpdate);

    //    await CloseConnectionAsync();
    //}

    //public async Task<MotorcyclesDto?> GetById(Guid id)
    //{
    //    await OpenConnectionAsync();

    //    var queryExists = $@"select * from public.tb_motorcycles where ""Id"" = '{id}' ";

    //    var result = await Connection.QueryFirstOrDefaultAsync<MotorcyclesDto>(queryExists);

    //    await CloseConnectionAsync();

    //    return result;
    //}

    ////public async Task<List<GetPricesDto>> GetPrices()
    ////{
    ////    await OpenConnectionAsync();

    ////    var queryGet = @"select * from public.tb_prices order by ""Days"" ";

    ////    var result = await Connection.QueryAsync<GetPricesDto>(queryGet);

    ////    await CloseConnectionAsync();

    ////    return result.ToList();
    ////}

    ////public async Task<GetPricesDto> GetPriceById(Guid id)
    ////{
    ////    await OpenConnectionAsync();

    ////    var queryGet = $@"select * from public.tb_prices where ""Id"" = '{id}' ";

    ////    var result = await Connection.QueryFirstOrDefaultAsync<GetPricesDto>(queryGet);

    ////    await CloseConnectionAsync();

    ////    return result!;
    ////}
}
