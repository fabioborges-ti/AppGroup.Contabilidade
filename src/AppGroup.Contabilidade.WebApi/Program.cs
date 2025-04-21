#nullable disable

using AppGroup.Contabilidade.Logging;
using AppGroup.Contabilidade.WebApi;
using Asp.Versioning.ApiExplorer;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(Serilogger.Configure);

    var startup = new Startup(builder.Configuration);

    startup.ConfigureServices(builder.Services);

    var app = builder.Build();

    #region MIGRATIONS

    //using (var scope = app.Services.CreateScope())
    //{
    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    //    db.Database.Migrate();
    //}

    #endregion

    app.UseSerilogRequestLogging();

    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    startup.Configure(app, provider);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}