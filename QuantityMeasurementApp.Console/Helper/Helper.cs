// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Helper.cs
// UC-16   : Database Integration
//
// Factory — wires all dependencies.
// Switches from CacheRepository (UC-15) to
// QuantityMeasurementDatabaseRepository (UC-16).
// Controller now also receives the repository directly
// for the history/stats menu options (7–11).
// ============================================================

using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.Console
{
    public static class Helper
    {
        public static QuantityMeasurementController CreateController()
        {
            // 1. Database repository (UC-16)
            IQuantityMeasurementRepository repository =
                new QuantityMeasurementDatabaseRepository();

            // 2. Model service + temperature service
            var modelService       = new QuantityModelServiceImpl();
            var temperatureService = new TemperatureService();

            // 3. Main service — still passes repository for auto-save on every operation
            IQuantityMeasurementService service = new QuantityMeasurementServiceImpl(
                modelService,
                temperatureService,
                repository);

            // 4. Controller receives BOTH service and repository
            //    service  → for operations (1–6)
            //    repository → for history/stats/clear (7–11)
            return new QuantityMeasurementController(service, repository);
        }
    }
}