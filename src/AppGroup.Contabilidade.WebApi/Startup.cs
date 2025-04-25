using AppGroup.Contabilidade.Application.Extensions;
using AppGroup.Contabilidade.Infrastructure.Database.Extensions;
using AppGroup.Contabilidade.WebApi.Extensions;
using AppGroup.Contabilidade.WebApi.Filters;
using AppGroup.Contabilidade.WebApi.Middlewares;
using AppGroup.Contabilidade.WebApi.Swagger;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AppGroup.Contabilidade.WebApi;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication()
                .AddInfrastructure(Configuration)
                .ConfigureHealthCheck(Configuration);

        services.AddHttpContextAccessor();

        services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine
                (
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("x-version")
                );
            })
            .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
            .AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

        services.Configure<KeyManagementOptions>(Configuration);

        services.AddFluentValidationAutoValidation();

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions.Select(p => p.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{description}/swagger.json", description.ToUpperInvariant());
            }

            options.DocExpansion(DocExpansion.List);
        });

        app.UseRequestLogging();
        app.UseRequestErrors();
        app.UseExceptionHandler("/error");

        app.UseHsts();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors(policy => policy
          .AllowAnyHeader()
          .AllowAnyMethod()
          .SetIsOriginAllowed(origin => true)
          .AllowCredentials());

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecksUI(options =>
            {
                options.UIPath = "/dashboard"; // URL do dashboard
                options.ApiPath = "/health-ui-api"; // endpoint da API do UI

            });
        });

        app.UseEndpoints(endpoints => endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}"));
    }
}