using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly ILengthService _lengthService;
        private readonly IWeightService _weightService;

        public Menu(ILengthService lengthService, IWeightService weightService)
        {
            _lengthService = lengthService;
            _weightService = weightService;
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

                System.Console.WriteLine("\n9. Exit");

                System.Console.Write("Select option: ");
                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CompareLength();
                        break;

                    case "2":
                        ConvertLength();
                        break;

                    case "3":
                        AddLength();
                        break;

                    case "4":
                        AddLengthWithTarget();
                        break;

                    case "5":
                        CompareWeight();
                        break;

                    case "6":
                        ConvertWeight();
                        break;

                    case "7":
                        AddWeight();
                        break;

                    case "8":
                        AddWeightWithTarget();
                        break;

                    case "9":
                        running = false;
                        break;
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

        // ===== COMMON INPUT METHODS =====

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

        private double ReadValue(string label)
        {
            System.Console.Write($"Enter {label}: ");

            string? input = System.Console.ReadLine();

            if (!double.TryParse(input, out double value))
                throw new ArgumentException("Invalid numeric input.");

            return value;
        }

        private LengthUnit ReadLengthUnit(string label)
        {
            System.Console.Write($"Enter {label} (Feet/Inch/Yard/Centimeter): ");

            string? input = System.Console.ReadLine();

            if (Enum.TryParse(input, true, out LengthUnit unit))
                return unit;

            throw new ArgumentException("Invalid length unit.");
        }

        private WeightUnit ReadWeightUnit(string label)
        {
            System.Console.Write($"Enter {label} (Gram/Kilogram/Pound): ");

            string? input = System.Console.ReadLine();

            if (Enum.TryParse(input, true, out WeightUnit unit))
                return unit;

            throw new ArgumentException("Invalid weight unit.");
        }
    }
}