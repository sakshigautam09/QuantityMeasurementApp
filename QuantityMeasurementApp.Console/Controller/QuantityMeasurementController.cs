// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : QuantityMeasurementController.cs
//
// UC-15 : N-Tier Architecture
//
// Responsibilities:
//   • Owns the interactive two-level menu loop (ShowMenu)
//   • Main menu  → select category (Length/Weight/Volume/Temperature)
//   • Sub  menu  → select operation within that category
//   • Reads ALL user console input
//   • Builds QuantityDTO objects from user input
//   • Delegates every operation to IQuantityMeasurementService
//   • Displays results / errors
//
// Design Patterns : Façade, Dependency Injection
// ============================================================

using System;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.BusinessLayer
{
    public class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // ════════════════════════════════════════════════════════════════════════════
        // MAIN MENU
        // ════════════════════════════════════════════════════════════════════════════

        public void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n╔══════════════════════════════════════╗");
                System.Console.WriteLine("║     Quantity Measurement App         ║");
                System.Console.WriteLine("╚══════════════════════════════════════╝");
                System.Console.WriteLine("  1. Length");
                System.Console.WriteLine("  2. Weight");
                System.Console.WriteLine("  3. Volume");
                System.Console.WriteLine("  4. Temperature");
                System.Console.WriteLine("  0. Exit");
                System.Console.Write("\nSelect category: ");

                switch (System.Console.ReadLine())
                {
                    case "1": ShowLengthMenu();      break;
                    case "2": ShowWeightMenu();      break;
                    case "3": ShowVolumeMenu();      break;
                    case "4": ShowTemperatureMenu(); break;
                    case "0": running = false;       break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // LENGTH SUBMENU
        // ════════════════════════════════════════════════════════════════════════════

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
                    case "1": CompareLengths();            break;
                    case "2": ConvertLength();             break;
                    case "3": AddLengths();                break;
                    case "4": AddLengthsWithTarget();      break;
                    case "5": SubtractLengths();           break;
                    case "6": SubtractLengthsWithTarget(); break;
                    case "7": DivideLengths();             break;
                    case "0": running = false;             break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // WEIGHT SUBMENU
        // ════════════════════════════════════════════════════════════════════════════

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
                    case "1": CompareWeights();            break;
                    case "2": ConvertWeight();             break;
                    case "3": AddWeights();                break;
                    case "4": AddWeightsWithTarget();      break;
                    case "5": SubtractWeights();           break;
                    case "6": SubtractWeightsWithTarget(); break;
                    case "7": DivideWeights();             break;
                    case "0": running = false;             break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // VOLUME SUBMENU
        // ════════════════════════════════════════════════════════════════════════════

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
                    case "1": CompareVolumes();            break;
                    case "2": ConvertVolume();             break;
                    case "3": AddVolumes();                break;
                    case "4": AddVolumesWithTarget();      break;
                    case "5": SubtractVolumes();           break;
                    case "6": SubtractVolumesWithTarget(); break;
                    case "7": DivideVolumes();             break;
                    case "0": running = false;             break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // TEMPERATURE SUBMENU  (Compare and Convert only)
        // ════════════════════════════════════════════════════════════════════════════

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
                    case "1": CompareTemperatures(); break;
                    case "2": ConvertTemperature();  break;
                    case "0": running = false;       break;
                    default:  System.Console.WriteLine("Invalid choice!"); break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // LENGTH – read input → build DTOs → call Perform
        // ════════════════════════════════════════════════════════════════════════════

        private void CompareLengths()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            PerformCompare(first, second);
        }

        private void ConvertLength()
        {
            var source = ReadLengthDTO("source");
            var target = ReadLengthUnitHint("target unit");
            PerformConvert(source, target);
        }

        private void AddLengths()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            PerformAdd(first, second);
        }

        private void AddLengthsWithTarget()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            var target = ReadLengthUnitHint("target unit");
            PerformAddWithTargetUnit(first, second, target);
        }

        private void SubtractLengths()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            PerformSubtract(first, second);
        }

        private void SubtractLengthsWithTarget()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            var target = ReadLengthUnitHint("target unit");
            PerformSubtractWithTargetUnit(first, second, target);
        }

        private void DivideLengths()
        {
            var first  = ReadLengthDTO("first");
            var second = ReadLengthDTO("second");
            PerformDivide(first, second);
        }

        // ════════════════════════════════════════════════════════════════════════════
        // WEIGHT – read input → build DTOs → call Perform
        // ════════════════════════════════════════════════════════════════════════════

        private void CompareWeights()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            PerformCompare(first, second);
        }

        private void ConvertWeight()
        {
            var source = ReadWeightDTO("source");
            var target = ReadWeightUnitHint("target unit");
            PerformConvert(source, target);
        }

        private void AddWeights()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            PerformAdd(first, second);
        }

        private void AddWeightsWithTarget()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            var target = ReadWeightUnitHint("target unit");
            PerformAddWithTargetUnit(first, second, target);
        }

        private void SubtractWeights()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            PerformSubtract(first, second);
        }

        private void SubtractWeightsWithTarget()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            var target = ReadWeightUnitHint("target unit");
            PerformSubtractWithTargetUnit(first, second, target);
        }

        private void DivideWeights()
        {
            var first  = ReadWeightDTO("first");
            var second = ReadWeightDTO("second");
            PerformDivide(first, second);
        }

        // ════════════════════════════════════════════════════════════════════════════
        // VOLUME – read input → build DTOs → call Perform
        // ════════════════════════════════════════════════════════════════════════════

        private void CompareVolumes()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            PerformCompare(first, second);
        }

        private void ConvertVolume()
        {
            var source = ReadVolumeDTO("source");
            var target = ReadVolumeUnitHint("target unit");
            PerformConvert(source, target);
        }

        private void AddVolumes()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            PerformAdd(first, second);
        }

        private void AddVolumesWithTarget()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            var target = ReadVolumeUnitHint("target unit");
            PerformAddWithTargetUnit(first, second, target);
        }

        private void SubtractVolumes()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            PerformSubtract(first, second);
        }

        private void SubtractVolumesWithTarget()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            var target = ReadVolumeUnitHint("target unit");
            PerformSubtractWithTargetUnit(first, second, target);
        }

        private void DivideVolumes()
        {
            var first  = ReadVolumeDTO("first");
            var second = ReadVolumeDTO("second");
            PerformDivide(first, second);
        }

        // ════════════════════════════════════════════════════════════════════════════
        // TEMPERATURE – read input → build DTOs → call Perform
        // ════════════════════════════════════════════════════════════════════════════

        private void CompareTemperatures()
        {
            var first  = ReadTemperatureDTO("first");
            var second = ReadTemperatureDTO("second");
            PerformCompare(first, second);
        }

        private void ConvertTemperature()
        {
            var source = ReadTemperatureDTO("source");
            var target = ReadTemperatureUnitHint("target unit");
            PerformConvert(source, target);
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PERFORM – delegate to service, display result
        // ════════════════════════════════════════════════════════════════════════════

        public QuantityDTO PerformCompare(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var  result = _service.Compare(first, second);
                bool equal  = result.Value == 1.0;
                System.Console.WriteLine($"Result: {equal}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Compare", ex.Message, first); }
        }

        public QuantityDTO PerformConvert(QuantityDTO source, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.Convert(source, targetUnit);
                System.Console.WriteLine($"Converted: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Convert", ex.Message, source); }
        }

        public QuantityDTO PerformAdd(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Add(first, second);
                System.Console.WriteLine($"Sum: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Add", ex.Message, first); }
        }

        public QuantityDTO PerformAddWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.AddWithTargetUnit(first, second, targetUnit);
                System.Console.WriteLine($"Sum: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Add", ex.Message, first); }
        }

        public QuantityDTO PerformSubtract(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Subtract(first, second);
                System.Console.WriteLine($"Difference: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Subtract", ex.Message, first); }
        }

        public QuantityDTO PerformSubtractWithTargetUnit(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.SubtractWithTargetUnit(first, second, targetUnit);
                System.Console.WriteLine($"Difference: {result}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Subtract", ex.Message, first); }
        }

        public QuantityDTO PerformDivide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                var result = _service.Divide(first, second);
                System.Console.WriteLine($"Division Result: {result.Value:G6}");
                return result;
            }
            catch (QuantityMeasurementException ex)
            { return Error("Divide", ex.Message, first); }
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PRIVATE – read console input + build QuantityDTO
        // ════════════════════════════════════════════════════════════════════════════

        private static double ReadValue(string label)
        {
            System.Console.Write($"Enter {label}: ");
            if (double.TryParse(System.Console.ReadLine(), out double value))
                return value;
            throw new ArgumentException("Invalid numeric input.");
        }

        // ── Length ────────────────────────────────────────────────────────────────────

        private QuantityDTO ReadLengthDTO(string label)
            => new(ReadValue($"{label} value"), ReadLengthUnit($"{label} unit"));

        private QuantityDTO ReadLengthUnitHint(string label)
            => new(0.0, ReadLengthUnit(label));

        private static QuantityDTO.LengthUnit ReadLengthUnit(string label)
        {
            System.Console.Write($"Enter {label} (Feet/Inch/Yard/Centimeter): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.LengthUnit unit))
                return unit;
            throw new ArgumentException("Invalid length unit.");
        }

        // ── Weight ────────────────────────────────────────────────────────────────────

        private QuantityDTO ReadWeightDTO(string label)
            => new(ReadValue($"{label} value"), ReadWeightUnit($"{label} unit"));

        private QuantityDTO ReadWeightUnitHint(string label)
            => new(0.0, ReadWeightUnit(label));

        private static QuantityDTO.WeightUnit ReadWeightUnit(string label)
        {
            System.Console.Write($"Enter {label} (Gram/Kilogram/Tonne): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.WeightUnit unit))
                return unit;
            throw new ArgumentException("Invalid weight unit.");
        }

        // ── Volume ────────────────────────────────────────────────────────────────────

        private QuantityDTO ReadVolumeDTO(string label)
            => new(ReadValue($"{label} value"), ReadVolumeUnit($"{label} unit"));

        private QuantityDTO ReadVolumeUnitHint(string label)
            => new(0.0, ReadVolumeUnit(label));

        private static QuantityDTO.VolumeUnit ReadVolumeUnit(string label)
        {
            System.Console.Write($"Enter {label} (Litre/Millilitre/Gallon): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.VolumeUnit unit))
                return unit;
            throw new ArgumentException("Invalid volume unit.");
        }

        // ── Temperature ───────────────────────────────────────────────────────────────

        private QuantityDTO ReadTemperatureDTO(string label)
            => new(ReadValue($"{label} value"), ReadTemperatureUnit($"{label} unit"));

        private QuantityDTO ReadTemperatureUnitHint(string label)
            => new(0.0, ReadTemperatureUnit(label));

        private static QuantityDTO.TemperatureUnit ReadTemperatureUnit(string label)
        {
            System.Console.Write($"Enter {label} (Celsius/Fahrenheit/Kelvin): ");
            if (Enum.TryParse(System.Console.ReadLine(), true, out QuantityDTO.TemperatureUnit unit))
                return unit;
            throw new ArgumentException("Invalid temperature unit.");
        }

        // ── error helper ──────────────────────────────────────────────────────────────

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