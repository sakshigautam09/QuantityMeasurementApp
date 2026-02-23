using System;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly ILengthComparer _service;

        public Menu(ILengthComparer service)
        {
            _service = service;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n===== Quantity Measurement App (UC4) =====");
                System.Console.WriteLine("1. Compare Lengths");
                System.Console.WriteLine("2. Exit");
                System.Console.Write("Select option: ");

                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CompareLengths();
                        break;

                    case "2":
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
            try
            {
                double value1 = ReadNumericInput("first value");
                LengthUnit unit1 = ReadUnit("first unit (Feet/Inch/Yard/Centimeter)");
                double value2 = ReadNumericInput("second value");
                LengthUnit unit2 = ReadUnit("second unit (Feet/Inch/Yard/Centimeter)");

                var l1 = _service.Create(value1, unit1);
                var l2 = _service.Create(value2, unit2);

                bool result = _service.AreEqual(l1, l2);

                System.Console.WriteLine("\n+--------------------------------------+");
                System.Console.WriteLine($"| Equality Result : {result,-10}|");
                System.Console.WriteLine("+--------------------------------------+");

                System.Console.WriteLine($"Converted Value 1: {l1.ToFeet()} ft");
                System.Console.WriteLine($"Converted Value 2: {l2.ToFeet()} ft");
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Validation Error: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                System.Console.WriteLine($"Overflow Error: {ex.Message}");
            }
            catch (Exception)
            {
                System.Console.WriteLine("Unexpected error occurred.");
            }
        }

        private double ReadNumericInput(string label)
        {
            System.Console.Write($"Enter {label}: ");
            string? input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"{label} cannot be empty.");

            if (!double.TryParse(input, out double value))
                throw new ArgumentException($"{label} must be numeric.");

            return value;
        }

        private LengthUnit ReadUnit(string label)
        {
            System.Console.Write($"Enter {label}: ");
            string? input = System.Console.ReadLine();

            if (Enum.TryParse(input, true, out LengthUnit unit))
                return unit;

            throw new ArgumentException("Invalid unit type.");
        }
    }
}
