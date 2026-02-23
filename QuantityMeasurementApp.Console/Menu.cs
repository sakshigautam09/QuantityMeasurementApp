using System;
using QuantityMeasurementApp.Core.Entities;
using QuantityMeasurementApp.Core.Interfaces;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly ILengthService _service;

        public Menu(ILengthService service)
        {
            _service = service;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n===== Quantity Measurement App =====");
                System.Console.WriteLine("1. Compare Lengths");
                System.Console.WriteLine("2. Convert Length");
                System.Console.WriteLine("3. Add Lengths");
                System.Console.WriteLine("4. Exit");
                System.Console.Write("Select option: ");

                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Compare();
                        break;
                    case "2":
                        Convert();
                        break;
                    case "3":
                        Add();
                        break;
                    case "4":
                        running = false;
                        break;
                }
            }
        }

        private void Compare()
        {
            var l1 = ReadLength("first");
            var l2 = ReadLength("second");

            bool result = _service.AreEqual(l1, l2);
            System.Console.WriteLine($"Result: {result}");
        }

        private void Convert()
        {
            double value = ReadValue("value");
            LengthUnit source = ReadUnit("source unit");
            LengthUnit target = ReadUnit("target unit");

            double result = _service.Convert(value, source, target);
            System.Console.WriteLine($"Converted: {result} {target}");
        }

        private void Add()
        {
            var l1 = ReadLength("first");
            var l2 = ReadLength("second");

            Length result = _service.Add(l1, l2);
            System.Console.WriteLine($"Sum: {result}");
        }

        private Length ReadLength(string label)
        {
            double value = ReadValue($"{label} value");
            LengthUnit unit = ReadUnit($"{label} unit");
            return _service.Create(value, unit);
        }

        private double ReadValue(string label)
        {
            System.Console.Write($"Enter {label}: ");
            string? input = System.Console.ReadLine();

            if (!double.TryParse(input, out double value))
                throw new ArgumentException("Invalid numeric input.");

            return value;
        }

        private LengthUnit ReadUnit(string label)
        {
            System.Console.Write($"Enter {label} (Feet/Inch/Yard/Centimeter): ");
            string? input = System.Console.ReadLine();

            if (Enum.TryParse(input, true, out LengthUnit unit))
                return unit;

            throw new ArgumentException("Invalid unit type.");
        }
    }
}