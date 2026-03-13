// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Helper.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Factory that initializes and wires all dependencies.
//           Program.cs calls this to get a ready controller.
//
// Design Pattern : Factory Pattern
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
            // 1. Repository
            var repository = QuantityMeasurementCacheRepository.Instance;

            // 2. Core Services (UC1-UC14 logic – unchanged)
            var lengthService      = new LengthService();
            var weightService      = new WeightService();
            var volumeService      = new VolumeService();
            var temperatureService = new TemperatureService();

            // 3. Service (UC15 – wraps Core, adds DTO mapping + persistence)
            var service = new QuantityMeasurementServiceImpl(
                lengthService,
                weightService,
                volumeService,
                temperatureService,
                repository);

            // 4. Controller
            return new QuantityMeasurementController(service);
        }
    }
}