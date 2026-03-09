using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly ILengthService _lengthService;
        private readonly IWeightService _weightService;
        private readonly IVolumeService _volumeService;

        public Menu(ILengthService lengthService, IWeightService weightService, IVolumeService volumeService)
        {
            _lengthService = lengthService;
            _weightService = weightService;
            _volumeService = volumeService;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n===== Quantity Measurement App =====");

                System.Console.WriteLine("\n--- Length Operations ---");
                System.Console.WriteLine("1. Compare Lengths");
                System.Console.WriteLine("2. Convert Length");
                System.Console.WriteLine("3. Add Lengths");
                System.Console.WriteLine("4. Add Lengths With Target Unit");

                System.Console.WriteLine("\n--- Weight Operations ---");
                System.Console.WriteLine("5. Compare Weights");
                System.Console.WriteLine("6. Convert Weight");
                System.Console.WriteLine("7. Add Weights");
                System.Console.WriteLine("8. Add Weights With Target Unit");

                System.Console.WriteLine("\n--- Volume Operations ---");
                System.Console.WriteLine("9. Compare Volumes");
                System.Console.WriteLine("10. Convert Volume");
                System.Console.WriteLine("11. Add Volumes");
                System.Console.WriteLine("12. Add Volumes With Target Unit");

                System.Console.WriteLine("\n13. Exit");

                System.Console.Write("Select option: ");
                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1": CompareLength(); break;
                    case "2": ConvertLength(); break;
                    case "3": AddLength(); break;
                    case "4": AddLengthWithTarget(); break;

                    case "5": CompareWeight(); break;
                    case "6": ConvertWeight(); break;
                    case "7": AddWeight(); break;
                    case "8": AddWeightWithTarget(); break;

                    case "9": CompareVolume(); break;
                    case "10": ConvertVolume(); break;
                    case "11": AddVolume(); break;
                    case "12": AddVolumeWithTarget(); break;

                    case "13": running = false; break;
                }
            }
        }

        // ===== LENGTH OPERATIONS =====

        private void CompareLength()
        {
            var l1 = ReadLength("first");
            var l2 = ReadLength("second");

            bool result = _lengthService.AreEqual(l1, l2);

            System.Console.WriteLine($"Result: {result}");
        }

        private void ConvertLength()
        {
            double value = ReadValue("value");
            LengthUnit source = ReadLengthUnit("source unit");
            LengthUnit target = ReadLengthUnit("target unit");

            double result = _lengthService.Convert(value, source, target);

            System.Console.WriteLine($"Converted: {result} {target}");
        }

        private void AddLength()
        {
            var l1 = ReadLength("first");
            var l2 = ReadLength("second");

            Length result = _lengthService.Add(l1, l2);

            System.Console.WriteLine($"Sum: {result}");
        }

        private void AddLengthWithTarget()
        {
            var l1 = ReadLength("first");
            var l2 = ReadLength("second");

            LengthUnit target = ReadLengthUnit("target unit");

            Length result = _lengthService.Add(l1, l2, target);

            System.Console.WriteLine($"Sum: {result}");
        }

        // ===== WEIGHT OPERATIONS =====

        private void CompareWeight()
        {
            var w1 = ReadWeight("first");
            var w2 = ReadWeight("second");

            bool result = _weightService.AreEqual(w1, w2);

            System.Console.WriteLine($"Result: {result}");
        }

        private void ConvertWeight()
        {
            double value = ReadValue("value");
            WeightUnit source = ReadWeightUnit("source unit");
            WeightUnit target = ReadWeightUnit("target unit");

            double result = _weightService.Convert(value, source, target);

            System.Console.WriteLine($"Converted: {result} {target}");
        }

        private void AddWeight()
        {
            var w1 = ReadWeight("first");
            var w2 = ReadWeight("second");

            Weight result = _weightService.Add(w1, w2);

            System.Console.WriteLine($"Sum: {result}");
        }

        private void AddWeightWithTarget()
        {
            var w1 = ReadWeight("first");
            var w2 = ReadWeight("second");

            WeightUnit target = ReadWeightUnit("target unit");

            Weight result = _weightService.Add(w1, w2, target);

            System.Console.WriteLine($"Sum: {result}");
        }

        // ===== VOLUME OPERATIONS =====

        private void CompareVolume()
        {
            var v1 = ReadVolume("first");
            var v2 = ReadVolume("second");

            bool result = _volumeService.AreEqual(v1, v2);

            System.Console.WriteLine($"Result: {result}");
        }

        private void ConvertVolume()
        {
            double value = ReadValue("value");

            VolumeUnit source = ReadVolumeUnit("source unit");
            VolumeUnit target = ReadVolumeUnit("target unit");

            double result = _volumeService.Convert(value, source, target);

            System.Console.WriteLine($"Converted: {result} {target}");
        }

        private void AddVolume()
        {
            var v1 = ReadVolume("first");
            var v2 = ReadVolume("second");

            Volume result = _volumeService.Add(v1, v2);

            System.Console.WriteLine($"Sum: {result}");
        }

        private void AddVolumeWithTarget()
        {
            var v1 = ReadVolume("first");
            var v2 = ReadVolume("second");

            VolumeUnit target = ReadVolumeUnit("target unit");

            Volume result = _volumeService.Add(v1, v2, target);

            System.Console.WriteLine($"Sum: {result}");
        }

        // ===== COMMON INPUT METHODS =====

        private double ReadValue(string label)
        {
            System.Console.Write($"Enter {label}: ");
            string? input = System.Console.ReadLine();

            if (!double.TryParse(input, out double value))
                throw new ArgumentException("Invalid numeric input.");

            return value;
        }

        private Length ReadLength(string label)
        {
            double value = ReadValue($"{label} value");
            LengthUnit unit = ReadLengthUnit($"{label} unit");

            return _lengthService.Create(value, unit);
        }

        private Weight ReadWeight(string label)
        {
            double value = ReadValue($"{label} value");
            WeightUnit unit = ReadWeightUnit($"{label} unit");

            return _weightService.Create(value, unit);
        }

        private Volume ReadVolume(string label)
        {
            double value = ReadValue($"{label} value");
            VolumeUnit unit = ReadVolumeUnit($"{label} unit");

            return _volumeService.Create(value, unit);
        }

        private LengthUnit ReadLengthUnit(string label)
        {
            System.Console.Write($"Enter {label} (Feet/Inch/Yard/Centimeter): ");

            if (Enum.TryParse(System.Console.ReadLine(), true, out LengthUnit unit))
                return unit;

            throw new ArgumentException("Invalid length unit.");
        }

        private WeightUnit ReadWeightUnit(string label)
        {
            System.Console.Write($"Enter {label} (Gram/Kilogram/Pound): ");

            if (Enum.TryParse(System.Console.ReadLine(), true, out WeightUnit unit))
                return unit;

            throw new ArgumentException("Invalid weight unit.");
        }

        private VolumeUnit ReadVolumeUnit(string label)
        {
            System.Console.Write($"Enter {label} (Litre/Millilitre/Gallon): ");

            if (Enum.TryParse(System.Console.ReadLine(), true, out VolumeUnit unit))
                return unit;

            throw new ArgumentException("Invalid volume unit.");
        }
    }
}