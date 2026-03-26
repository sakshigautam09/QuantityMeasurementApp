using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementRepository;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementBusinessLayer.Extensions
{
    /// <summary>
    /// Dependency injection extensions for Business layer (Services).
    /// </summary>
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<QuantityMeasurementService>(sp =>
                new QuantityMeasurementService(
                    sp.GetRequiredService<IMeasurementHistoryRepository>(),
                    sp.GetRequiredService<QuantityMeasurementRepository.Interface.IQuantityMeasurementRepository>(),
                    sp.GetRequiredService<ILogger<QuantityMeasurementService>>()));
            services.AddScoped<IQuantityMeasurementService>(sp => sp.GetRequiredService<QuantityMeasurementService>());
            services.AddScoped<IQuantityMeasurementWebService>(sp => sp.GetRequiredService<QuantityMeasurementService>());

            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IEncryptionService, AesEncryptionService>();

            return services;
        }
    }
}
