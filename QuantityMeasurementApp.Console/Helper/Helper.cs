// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Helper/Helper.cs
// UC-17   : Wires dependencies for Console app.
// ============================================================

using System;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.Console.Controller;
using QuantityMeasurementApp.Core.Services;
using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.Console
{
    public static class Helper
    {
        public static QuantityMeasurementController CreateController()
        {
            // 1. Both repos are singletons — use .Instance
            var jsonCache = QuantityMeasurementJsonCacheRepository.Instance;
            var cacheRepo = QuantityMeasurementCacheRepository.Instance;

            // 2. Ask ONCE where to save
            StorageMode mode = AskStorageMode();

            // 3. Wrap with DualModeRepository
            var dualRepo = new DualModeRepository(jsonCache, cacheRepo, mode);

            // 4. Offer startup sync if DB mode chosen
            if (mode == StorageMode.Database)
                OfferStartupSync(dualRepo);

            // 5. Wire services — 3-arg constructor (no encryption/Redis in Console)
            IQuantityMeasurementService service =
                new QuantityMeasurementServiceImpl(
                    new QuantityModelServiceImpl(),
                    new TemperatureService(),
                    dualRepo);

            // 6. Return controller
            return new QuantityMeasurementController(service, dualRepo);
        }

        private static StorageMode AskStorageMode()
        {
            while (true)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("╔══════════════════════════════════════════╗");
                System.Console.WriteLine("║    Where do you want to save data?       ║");
                System.Console.WriteLine("╠══════════════════════════════════════════╣");
                System.Console.WriteLine("║  1. Cache  →  quantity_cache.json        ║");
                System.Console.WriteLine("║  2. DB     →  SQL Server                 ║");
                System.Console.WriteLine("╚══════════════════════════════════════════╝");
                System.Console.Write("Your choice (1 or 2): ");

                switch (System.Console.ReadLine()?.Trim())
                {
                    case "1":
                        System.Console.WriteLine("\n  ✔ Saving to CACHE for this session.");
                        System.Console.WriteLine("  → Use menu option 12 to push data to DB anytime.\n");
                        return StorageMode.Cache;
                    case "2":
                        System.Console.WriteLine("\n  ✔ Saving to DATABASE for this session.\n");
                        return StorageMode.Database;
                    default:
                        System.Console.WriteLine("  Invalid. Please enter 1 or 2.");
                        break;
                }
            }
        }

        private static void OfferStartupSync(DualModeRepository dualRepo)
        {
            int pending = dualRepo.CachePendingCount;
            if (pending == 0) return;

            System.Console.WriteLine($"[Startup] Found {pending} record(s) in quantity_cache.json.");
            System.Console.Write("[Startup] Sync these to the database now? (yes/no): ");

            if (System.Console.ReadLine()?.Trim().ToLower() == "yes")
            {
                int synced = dualRepo.SyncCacheToDatabase();
                System.Console.WriteLine($"[Startup] Synced {synced} record(s) to the database.");
                int remaining = dualRepo.CachePendingCount;
                if (remaining > 0)
                    System.Console.WriteLine($"[Startup] {remaining} record(s) remain in cache.");
            }
            else
            {
                System.Console.WriteLine("[Startup] Skipped. Records stay in quantity_cache.json.\n");
            }
        }
    }
}