using Konteh.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Data.Common;

namespace Konteh.BackOfficeApi.Tests;
public abstract class IntegrationBase : IDisposable
{
    protected readonly WebApplicationFactory<Program> _factory;
    private readonly string ConnectionString = "Server=.;Database=KontehDBTests;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
    protected readonly HttpClient _httpClient;

    public IntegrationBase()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(cfg =>
        {
            cfg.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<KontehContext>));
                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<KontehContext>(Options =>
                Options.UseSqlServer(ConnectionString));

                services.AddScoped<IPolicyEvaluator, FakePolicyEvaluator>();
            });
        });

        _httpClient = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KontehContext>();
        dbContext.Database.Migrate();
    }
    [SetUp]
    public async Task SetUpAsync()
    {
        var _respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"],
        });
        await _respawner.ResetAsync(ConnectionString);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
