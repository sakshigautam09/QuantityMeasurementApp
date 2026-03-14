// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Helper.cs
//
// Purpose : Factory — wires all dependencies and returns a
//           ready-to-use controller.
//           Only ITemperatureService is kept as a direct Core
//           injection because temperature needs special handling
//           in Compare/Convert.
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
            // 1. Repository (singleton)
            var repository = QuantityMeasurementCacheRepository.Instance;

            // 2. QuantityModelService — owns all arithmetic / conversion logic
            var modelService = new QuantityModelServiceImpl();

            // 3. TemperatureService — kept because temperature conversion formulas
            //    are non-linear and already implemented in Core.
            var temperatureService = new TemperatureService();

            // 4. Main service
            var service = new QuantityMeasurementServiceImpl(
                modelService,
                temperatureService,
                repository);

            // 5. Controller
            return new QuantityMeasurementController(service);
        }
    }
}