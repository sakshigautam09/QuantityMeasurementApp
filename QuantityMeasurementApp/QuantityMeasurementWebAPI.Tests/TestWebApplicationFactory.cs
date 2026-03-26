using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurementRepository.Context;

namespace QuantityMeasurementWebAPI.Tests;

/// <summary>Integration test host: Testing environment, EF Core InMemory (no SQL Server).</summary>
public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var opts = services.Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)).ToList();
            foreach (var d in opts)
                services.Remove(d);

            var ctxRegs = services.Where(d => d.ServiceType == typeof(ApplicationDbContext)).ToList();
            foreach (var d in ctxRegs)
                services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("QuantityApiIntegrationTests_" + Guid.NewGuid().ToString("N")));
        });
    }
}
