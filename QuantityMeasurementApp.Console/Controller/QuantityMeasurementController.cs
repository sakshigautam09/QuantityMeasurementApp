// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Controller/QuantityMeasurementController.cs
// UC-17   : Complete controller inside Console project.
//           Combined from BusinessLayer/Controller into Console.
//           All menu logic, submenus, operations in one file.
// ============================================================

using System;
using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;
using QuantityMeasurementApp.RepositoryLayer.Interface;

namespace QuantityMeasurementApp.Console.Controller
{
    public class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repo;
        private readonly DualModeRepository?            _dualRepo;

        public QuantityMeasurementController(
            IQuantityMeasurementService    service,
            IQuantityMeasurementRepository repository)
        {
            _service  = service    ?? throw new ArgumentNullException(nameof(service));
            _repo     = repository ?? throw new ArgumentNullException(nameof(repository));
            _dualRepo = repository as DualModeRepository;
        }

        // ════════════════════════════════════════════════════════════════════════
        // MAIN MENU
        // ════════════════════════════════════════════════════════════════════════

        public void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                string modeLabel = GetModeLabel();
                int    pending   = _dualRepo?.CachePendingCount ?? 0;

                System.Console.WriteLine("\n╔══════════════════════════════════════════╗");
                System.Console.WriteLine("║       Quantity Measurement App           ║");
                System.Console.WriteLine($"║  Saving to : {modeLabel,-30}║");
                if (pending > 0)
                    System.Console.WriteLine(
                        $"║  Cache : {pending,-2} record(s) not yet synced to DB   ║");
                System.Console.WriteLine("╚══════════════════════════════════════════╝");
                System.Console.WriteLine("  1. Length");
                System.Console.WriteLine("  2. Weight");
                System.Console.WriteLine("  3. Volume");
                System.Console.WriteLine("  4. Temperature");
                System.Console.WriteLine("  ──────────────────────────────────────────");
                System.Console.WriteLine("  7. View All History");
                System.Console.WriteLine("  8. View By Operation Type");
                System.Console.WriteLine("  9. View By Measurement Type");
                System.Console.WriteLine(" 10. View Statistics");
                System.Console.WriteLine(" 11. Clear All Records");
                System.Console.WriteLine("  ──────────────────────────────────────────");
                System.Console.WriteLine(" 12. Sync Cache → Database");
                System.Console.WriteLine("  ──────────────────────────────────────────");
                System.Console.WriteLine("  0. Exit");
                System.Console.Write("\nSelect category: ");

                switch (System.Console.ReadLine()?.Trim())
                {
                    case "1":  ShowLengthMenu();        break;
                    case "2":  ShowWeightMenu();        break;
                    case "3":  ShowVolumeMenu();        break;
                    case "4":  ShowTemperatureMenu();   break;
                    case "7":  ShowAllHistory();        break;
                    case "8":  ShowByOperationType();   break;
                    case "9":  ShowByMeasurementType(); break;
                    case "10": ShowStatistics();        break;
                    case "11": ClearAllRecords();       break;
                    case "12": SyncCacheToDatabase();   break;
                    case "0":  running = false;         break;
                    default:   System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // MODE LABEL
        // ════════════════════════════════════════════════════════════════════════

        private string GetModeLabel()
        {
            if (_dualRepo is null) return "Database (SQL Server)";
            return _dualRepo.CurrentMode == StorageMode.Cache
                ? "Cache (quantity_cache.json)"
                : "Database (SQL Server)";
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 12 — Sync Cache → Database
        // ════════════════════════════════════════════════════════════════════════

        private void SyncCacheToDatabase()
        {
            if (_dualRepo is null)
            {
                System.Console.WriteLine("[Sync] Not available — dual-mode not active.");
                return;
            }

            int pending = _dualRepo.CachePendingCount;
            if (pending == 0)
            {
                System.Console.WriteLine("\n[Sync] Cache is empty — nothing to sync.");
                return;
            }

            System.Console.WriteLine($"\n[Sync] {pending} record(s) found in cache.");
            System.Console.Write("[Sync] Push all to database now? (yes/no): ");

            if (System.Console.ReadLine()?.Trim().ToLower() != "yes")
            {
                System.Console.WriteLine("[Sync] Cancelled.");
                return;
            }

            try
            {
                int synced = _dualRepo.SyncCacheToDatabase();
                System.Console.WriteLine($"[Sync] Done. {synced} record(s) moved to database.");

                int remaining = _dualRepo.CachePendingCount;
                if (remaining > 0)
                    System.Console.WriteLine(
                        $"[Sync] {remaining} record(s) remain in cache — DB may be unreachable.");
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[Sync] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 7 — View All History
        // ════════════════════════════════════════════════════════════════════════

        private void ShowAllHistory()
        {
            try
            {
                var records = _repo.FindAll();
                System.Console.WriteLine($"\n── All History ({records.Count} record(s)) ──");
                if (records.Count == 0) { System.Console.WriteLine("  No records found."); return; }
                foreach (var r in records) PrintRecord(r);
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[History] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 8 — View By Operation Type
        // ════════════════════════════════════════════════════════════════════════

        private void ShowByOperationType()
        {
            System.Console.WriteLine("\nOperation types: Compare, Convert, Add, Subtract, Divide");
            System.Console.Write("Enter operation type: ");

            if (!Enum.TryParse<QuantityMeasurementEntity.OperationType>(
                System.Console.ReadLine()?.Trim(), true, out var op))
            { System.Console.WriteLine("Invalid operation type."); return; }

            try
            {
                var records = _repo.FindByOperation(op);
                System.Console.WriteLine($"\n── {op} Records ({records.Count} record(s)) ──");
                if (records.Count == 0) { System.Console.WriteLine("  No records found."); return; }
                foreach (var r in records) PrintRecord(r);
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[ByOperation] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 9 — View By Measurement Type
        // ════════════════════════════════════════════════════════════════════════

        private void ShowByMeasurementType()
        {
            System.Console.WriteLine("\nMeasurement types: Length, Weight, Volume, Temperature");
            System.Console.Write("Enter measurement type: ");
            string? input = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            { System.Console.WriteLine("Invalid input."); return; }

            try
            {
                var records = _repo.FindByMeasurementType(input);
                System.Console.WriteLine($"\n── {input} Records ({records.Count} record(s)) ──");
                if (records.Count == 0) { System.Console.WriteLine("  No records found."); return; }
                foreach (var r in records) PrintRecord(r);
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[ByType] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 10 — View Statistics
        // ════════════════════════════════════════════════════════════════════════

        private void ShowStatistics()
        {
            try
            {
                int pending = _dualRepo?.CachePendingCount ?? 0;
                System.Console.WriteLine("\n── Statistics ──────────────────────────────");
                System.Console.WriteLine($"  Total records  : {_repo.GetTotalCount()}");
                System.Console.WriteLine($"  Errors         : {_repo.GetErrorCount()}");
                if (pending > 0)
                    System.Console.WriteLine($"  Pending cache  : {pending}  ← not yet synced");
                System.Console.WriteLine("  ── By operation ──");
                foreach (QuantityMeasurementEntity.OperationType op in
                    Enum.GetValues<QuantityMeasurementEntity.OperationType>())
                    System.Console.WriteLine($"    {op,-12}: {_repo.GetCountByOperation(op)}");
                System.Console.WriteLine("────────────────────────────────────────────");
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[Statistics] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // OPTION 11 — Clear All Records
        // ════════════════════════════════════════════════════════════════════════

        private void ClearAllRecords()
        {
            System.Console.Write("\nAre you sure you want to delete ALL records? (yes/no): ");
            if (System.Console.ReadLine()?.Trim().ToLower() != "yes")
            { System.Console.WriteLine("Cancelled."); return; }
            try
            {
                _repo.Clear();
                System.Console.WriteLine("All records cleared.");
            }
            catch (Exception ex)
            { System.Console.WriteLine($"[Clear] ERROR: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // SUBMENUS
        // ════════════════════════════════════════════════════════════════════════

        private void ShowLengthMenu()
        {
            bool running = true;
            while (running)
            {
                System.Console.WriteLine("\n--- Length Operations ---");
                System.Console.WriteLine("  1. Compare");
                System.Console.WriteLine("  2. Convert");
                System.Console.WriteLine("  3. Add");
                System.Console.WriteLine("  4. Add With Target Unit");
                System.Console.WriteLine("  5. Subtract");
                System.Console.WriteLine("  6. Subtract With Target Unit");
                System.Console.WriteLine("  7. Divide");
                System.Console.WriteLine("  0. Back");
                System.Console.Write("\nSelect operation: ");
                switch (System.Console.ReadLine())
                {
                    case "1": PerformCompare(ReadLengthDTO("first"),  ReadLengthDTO("second"));                                          break;
                    case "2": PerformConvert(ReadLengthDTO("source"), ReadLengthUnitHint("target unit"));                                break;
                    case "3": PerformAdd(ReadLengthDTO("first"),      ReadLengthDTO("second"));                                          break;
                    case "4": { var f = ReadLengthDTO("first"); var s = ReadLengthDTO("second"); PerformAddWithTargetUnit(f, s, ReadLengthUnitHint("target unit")); break; }
                    case "5": PerformSubtract(ReadLengthDTO("first"), ReadLengthDTO("second"));                                          break;
                    case "6": { var f = ReadLengthDTO("first"); var s = ReadLengthDTO("second"); PerformSubtractWithTargetUnit(f, s, ReadLengthUnitHint("target unit")); break; }
                    case "7": PerformDivide(ReadLengthDTO("first"),   ReadLengthDTO("second"));                                          break;
                    case "0": running = false;                                                                                            break;
                    default:  System.Console.WriteLine("Invalid choice!");                                                               break;
                }
            }
        }

        private void ShowWeightMenu()
        {
            bool running = true;
            while (running)
            {
                System.Console.WriteLine("\n--- Weight Operations ---");
                System.Console.WriteLine("  1. Compare");
                System.Console.WriteLine("  2. Convert");
                System.Console.WriteLine("  3. Add");
                System.Console.WriteLine("  4. Add With Target Unit");
                System.Console.WriteLine("  5. Subtract");
                System.Console.WriteLine("  6. Subtract With Target Unit");
                System.Console.WriteLine("  7. Divide");
                System.Console.WriteLine("  0. Back");
                System.Console.Write("\nSelect operation: ");
                switch (System.Console.ReadLine())
                {
                    case "1": PerformCompare(ReadWeightDTO("first"),  ReadWeightDTO("second"));                                          break;
                    case "2": PerformConvert(ReadWeightDTO("source"), ReadWeightUnitHint("target unit"));                                break;
                    case "3": PerformAdd(ReadWeightDTO("first"),      ReadWeightDTO("second"));                                          break;
                    case "4": { var f = ReadWeightDTO("first"); var s = ReadWeightDTO("second"); PerformAddWithTargetUnit(f, s, ReadWeightUnitHint("target unit")); break; }
                    case "5": PerformSubtract(ReadWeightDTO("first"), ReadWeightDTO("second"));                                          break;
                    case "6": { var f = ReadWeightDTO("first"); var s = ReadWeightDTO("second"); PerformSubtractWithTargetUnit(f, s, ReadWeightUnitHint("target unit")); break; }
                    case "7": PerformDivide(ReadWeightDTO("first"),   ReadWeightDTO("second"));                                          break;
                    case "0": running = false;                                                                                            break;
                    default:  System.Console.WriteLine("Invalid choice!");                                                               break;
                }
            }
        }

        private void ShowVolumeMenu()
        {
            bool running = true;
            while (running)
            {
                System.Console.WriteLine("\n--- Volume Operations ---");
                System.Console.WriteLine("  1. Compare");
                System.Console.WriteLine("  2. Convert");
                System.Console.WriteLine("  3. Add");
                System.Console.WriteLine("  4. Add With Target Unit");
                System.Console.WriteLine("  5. Subtract");
                System.Console.WriteLine("  6. Subtract With Target Unit");
                System.Console.WriteLine("  7. Divide");
                System.Console.WriteLine("  0. Back");
                System.Console.Write("\nSelect operation: ");
                switch (System.Console.ReadLine())
                {
                    case "1": PerformCompare(ReadVolumeDTO("first"),  ReadVolumeDTO("second"));                                          break;
                    case "2": PerformConvert(ReadVolumeDTO("source"), ReadVolumeUnitHint("target unit"));                                break;
                    case "3": PerformAdd(ReadVolumeDTO("first"),      ReadVolumeDTO("second"));                                          break;
                    case "4": { var f = ReadVolumeDTO("first"); var s = ReadVolumeDTO("second"); PerformAddWithTargetUnit(f, s, ReadVolumeUnitHint("target unit")); break; }
                    case "5": PerformSubtract(ReadVolumeDTO("first"), ReadVolumeDTO("second"));                                          break;
                    case "6": { var f = ReadVolumeDTO("first"); var s = ReadVolumeDTO("second"); PerformSubtractWithTargetUnit(f, s, ReadVolumeUnitHint("target unit")); break; }
                    case "7": PerformDivide(ReadVolumeDTO("first"),   ReadVolumeDTO("second"));                                          break;
                    case "0": running = false;                                                                                            break;
                    default:  System.Console.WriteLine("Invalid choice!");                                                               break;
                }
            }
        }

        private void ShowTemperatureMenu()
        {
            bool running = true;
            while (running)
            {
                System.Console.WriteLine("\n--- Temperature Operations ---");
                System.Console.WriteLine("  1. Compare");
                System.Console.WriteLine("  2. Convert");
                System.Console.WriteLine("  0. Back");
                System.Console.Write("\nSelect operation: ");
                switch (System.Console.ReadLine())
                {
                    case "1": PerformCompare(ReadTemperatureDTO("first"), ReadTemperatureDTO("second")); break;
                    case "2": PerformConvert(ReadTemperatureDTO("source"), ReadTemperatureUnitHint("target unit")); break;
                    case "0": running = false; break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // PERFORM — call service + display result
        // ════════════════════════════════════════════════════════════════════════

        public QuantityDTO PerformCompare(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Compare(first, second);
                System.Console.WriteLine($"Result: {result.Value == 1.0}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Compare", ex.Message, first); }
        }

        public QuantityDTO PerformConvert(QuantityDTO source, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.Convert(source, targetUnit);
                System.Console.WriteLine($"Converted: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Convert", ex.Message, source); }
        }

        public QuantityDTO PerformAdd(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Add(first, second);
                System.Console.WriteLine($"Sum: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Add", ex.Message, first); }
        }

        public QuantityDTO PerformAddWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.AddWithTargetUnit(first, second, targetUnit);
                System.Console.WriteLine($"Sum: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Add", ex.Message, first); }
        }

        public QuantityDTO PerformSubtract(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Subtract(first, second);
                System.Console.WriteLine($"Difference: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Subtract", ex.Message, first); }
        }

        public QuantityDTO PerformSubtractWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.SubtractWithTargetUnit(first, second, targetUnit);
                System.Console.WriteLine($"Difference: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Subtract", ex.Message, first); }
        }

        public QuantityDTO PerformDivide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Divide(first, second);
                System.Console.WriteLine($"Division Result: {result.Value:G6}");
                return result;
            }
            catch (QuantityMeasurementException ex) { return Error("Divide", ex.Message, first); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // READ INPUT HELPERS
        // ════════════════════════════════════════════════════════════════════════

        private static double ReadValue(string label)
        {
            System.Console.Write($"Enter {label}: ");
            if (double.TryParse(System.Console.ReadLine(), out double value)) return value;
            throw new ArgumentException("Invalid numeric input.");
        }

        private QuantityDTO ReadLengthDTO(string label)       => new(ReadValue($"{label} value"), ReadLengthUnit($"{label} unit"));
        private QuantityDTO ReadLengthUnitHint(string label)   => new(0.0, ReadLengthUnit(label));
        private static QuantityDTO.LengthUnit ReadLengthUnit(string label)
        {
            System.Console.Write($"Enter {label} (Feet/Inch/Yard/Centimeter): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.LengthUnit u)) return u;
            throw new ArgumentException("Invalid length unit.");
        }

        private QuantityDTO ReadWeightDTO(string label)       => new(ReadValue($"{label} value"), ReadWeightUnit($"{label} unit"));
        private QuantityDTO ReadWeightUnitHint(string label)   => new(0.0, ReadWeightUnit(label));
        private static QuantityDTO.WeightUnit ReadWeightUnit(string label)
        {
            System.Console.Write($"Enter {label} (Gram/Kilogram/Tonne): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.WeightUnit u)) return u;
            throw new ArgumentException("Invalid weight unit.");
        }

        private QuantityDTO ReadVolumeDTO(string label)       => new(ReadValue($"{label} value"), ReadVolumeUnit($"{label} unit"));
        private QuantityDTO ReadVolumeUnitHint(string label)   => new(0.0, ReadVolumeUnit(label));
        private static QuantityDTO.VolumeUnit ReadVolumeUnit(string label)
        {
            System.Console.Write($"Enter {label} (Litre/Millilitre/Gallon): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.VolumeUnit u)) return u;
            throw new ArgumentException("Invalid volume unit.");
        }

        private QuantityDTO ReadTemperatureDTO(string label)      => new(ReadValue($"{label} value"), ReadTemperatureUnit($"{label} unit"));
        private QuantityDTO ReadTemperatureUnitHint(string label)  => new(0.0, ReadTemperatureUnit(label));
        private static QuantityDTO.TemperatureUnit ReadTemperatureUnit(string label)
        {
            System.Console.Write($"Enter {label} (Celsius/Fahrenheit/Kelvin): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.TemperatureUnit u)) return u;
            throw new ArgumentException("Invalid temperature unit.");
        }

        // ════════════════════════════════════════════════════════════════════════
        // PRINT + ERROR
        // ════════════════════════════════════════════════════════════════════════

        private static void PrintRecord(QuantityMeasurementEntity r)
        {
            System.Console.WriteLine(
                $"  [{r.Timestamp:yyyy-MM-dd HH:mm:ss}] {r.Operation,-10} | " +
                $"{r.FirstOperand.Type,-12} | {r.FirstOperand}" +
                (r.SecondOperand != null ? $" , {r.SecondOperand}" : "") +
                (r.TargetUnit    != null ? $" → {r.TargetUnit.UnitLabel}" : "") +
                (r.HasError ? $" | ERROR: {r.ErrorMessage}" : $" | Result: {r.ResultDisplay}"));
        }

        private static QuantityDTO Error(string op, string message, QuantityDTO source)
        {
            System.Console.WriteLine($"[{op}] ERROR: {message}");
            return source.Type switch
            {
                QuantityDTO.MeasurementType.Length      => new(double.NaN, source.LengthUnitValue!.Value),
                QuantityDTO.MeasurementType.Weight      => new(double.NaN, source.WeightUnitValue!.Value),
                QuantityDTO.MeasurementType.Volume      => new(double.NaN, source.VolumeUnitValue!.Value),
                QuantityDTO.MeasurementType.Temperature => new(double.NaN, source.TemperatureUnitValue!.Value),
                _                                       => new(double.NaN, QuantityDTO.LengthUnit.Feet)
            };
        }
    }
}