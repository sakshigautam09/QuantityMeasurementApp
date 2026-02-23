using System;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly IMeasurementComparer _service;

        public Menu(IMeasurementComparer service)
        {
            _service = service;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n===== Quantity Measurement App =====");
                System.Console.WriteLine("1. Compare Feet (UC1)");
                System.Console.WriteLine("2. Compare Inches (UC2)");
                System.Console.WriteLine("3. Compare Feet and Inches");
                System.Console.WriteLine("4. Exit");
                System.Console.Write("Select option: ");

                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CompareFeet();
                        break;

                    case "2":
                        CompareInches();
                        break;

                    case "3":
                        CompareFeetAndInch();
                        break;

                    case "4":
                        running = false;
                        break;

                    default:
                        System.Console.WriteLine("Invalid selection.");
                        break;
                }

                if (running)
                {
                    System.Console.WriteLine("\nPress any key to continue...");
                    System.Console.ReadKey();
                    System.Console.Clear();
                }
            }
        }

        // ================== FEET ==================

        private void CompareFeet()
        {
            try
            {
                System.Console.WriteLine("\n--- UC1 : Feet Equality Check ---");

                double value1 = ReadNumericInput("first feet value");
                double value2 = ReadNumericInput("second feet value");

                FeetMeasurement f1 = _service.CreateFeet(value1);
                FeetMeasurement f2 = _service.CreateFeet(value2);

                bool result = _service.AreFeetEqual(f1, f2);

                DisplayResult(result, f1.Value, f2.Value, "ft");
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
                System.Console.WriteLine("Unexpected system error occurred.");
            }
        }

        // ================== INCHES ==================

        private void CompareInches()
        {
            try
            {
                System.Console.WriteLine("\n--- UC2 : Inch Equality Check ---");

                double value1 = ReadNumericInput("first inch value");
                double value2 = ReadNumericInput("second inch value");

                InchMeasurement i1 = _service.CreateInch(value1);
                InchMeasurement i2 = _service.CreateInch(value2);

                bool result = _service.AreInchesEqual(i1, i2);

                DisplayResult(result, i1.Value, i2.Value, "inch");
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
                System.Console.WriteLine("Unexpected system error occurred.");
            }
        }

        // ================== FEET + INCH ==================

        private void CompareFeetAndInch()
        {
            try
            {
                System.Console.WriteLine("\n--- UC2 : Feet & Inch Equality Check ---");

                double feetValue = ReadNumericInput("feet value");
                double inchValue = ReadNumericInput("inch value");

                FeetMeasurement f = _service.CreateFeet(feetValue);
                InchMeasurement i = _service.CreateInch(inchValue);

                bool result = _service.AreFeetAndInchEqual(f, i);

                System.Console.WriteLine("\n+--------------------------------------+");
                System.Console.WriteLine($"| Feet & Inch Equal : {result,-12}|");
                System.Console.WriteLine("+--------------------------------------+");

                double convertedInFeet = inchValue / 12.0;
                double difference = Math.Abs(feetValue - convertedInFeet);

                System.Console.WriteLine($"Converted Inch to Feet: {convertedInFeet} ft");
                System.Console.WriteLine($"Difference: {difference} ft");

                if (difference < 0.0001)
                    System.Console.WriteLine("Measurements are equivalent.");
                else
                    System.Console.WriteLine("Measurements are not equivalent.");
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
                System.Console.WriteLine("Unexpected system error occurred.");
            }
        }

        // ================== COMMON INPUT ==================

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

        // ================== COMMON RESULT DISPLAY ==================

        private void DisplayResult(bool result, double v1, double v2, string unit)
        {
            System.Console.WriteLine("\n+--------------------------------------+");
            System.Console.WriteLine($"| Equality Result : {result,-12}|");
            System.Console.WriteLine("+--------------------------------------+");

            double difference = Math.Abs(v1 - v2);

            System.Console.WriteLine($"Difference: {difference} {unit}");

            if (difference < 0.0001)
                System.Console.WriteLine("Measurements are identical.");
            else if (difference < 1)
                System.Console.WriteLine("Measurements are close.");
            else
                System.Console.WriteLine("Measurements differ significantly.");
        }
    }
}