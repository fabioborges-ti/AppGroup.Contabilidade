using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AppGroup.Contabilidade.Infrastructure.Database.Data.Repositories.Base;

public abstract class BaseRepository
{
    public readonly SqlConnection Connection;

    protected BaseRepository(IConfiguration configuration)
    {
        Connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task OpenConnectionAsync()
    {
        if (Connection.State == ConnectionState.Closed)
            await Connection.OpenAsync();
    }

    public async Task CloseConnectionAsync()
    {
        if (Connection.State != ConnectionState.Closed)
            await Connection.CloseAsync();
    }
}
