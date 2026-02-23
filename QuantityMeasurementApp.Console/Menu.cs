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
                System.Console.WriteLine("1. Compare Two Lengths");
                System.Console.WriteLine("2. Convert Length");
                System.Console.WriteLine("3. Exit");
                System.Console.Write("Select option: ");

                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CompareLengths();
                        break;

                    case "2":
                        ConvertLength();
                        break;

                    case "3":
                        running = false;
                        break;

                    default:
                        System.Console.WriteLine("Invalid selection.");
                        break;
                }
            }
        }

        private void CompareLengths()
        {
            double value1 = ReadValue("first value");
            LengthUnit unit1 = ReadUnit("first unit");

            double value2 = ReadValue("second value");
            LengthUnit unit2 = ReadUnit("second unit");

            var l1 = _service.Create(value1, unit1);
            var l2 = _service.Create(value2, unit2);

            bool result = _service.AreEqual(l1, l2);

            System.Console.WriteLine($"\nEquality Result: {result}");
        }

        private void ConvertLength()
        {
            double value = ReadValue("value");
            LengthUnit source = ReadUnit("source unit");
            LengthUnit target = ReadUnit("target unit");

            double result = _service.Convert(value, source, target);

            System.Console.WriteLine($"\nConverted Result: {result} {target}");
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