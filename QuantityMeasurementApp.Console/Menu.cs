using System;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Entities;

namespace QuantityMeasurementApp.Console
{
    public class Menu
    {
        private readonly IFeetComparer _service;

        public Menu(IFeetComparer service)
        {
            _service = service;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                System.Console.WriteLine("\n===== Quantity Measurement App =====");
                System.Console.WriteLine("1. Compare Two Feet Values (UC1)");
                System.Console.WriteLine("2. Exit");
                System.Console.Write("Select option: ");

                string? choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ExecuteComparison();
                        break;

                    case "2":
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

        private void ExecuteComparison()
        {
            try
            {
                System.Console.WriteLine("\n--- UC1 : Feet Equality Check ---");
                System.Console.Write("Enter first value in feet: ");
                string? input1 = System.Console.ReadLine();

                if (!double.TryParse(input1, out double value1))
                    throw new ArgumentException("First value must be a valid numeric input.");
                    
                System.Console.Write("Enter second value in feet: ");
                string? input2 = System.Console.ReadLine();

                if (!double.TryParse(input2, out double value2))
                    throw new ArgumentException("Second value must be a valid numeric input.");


                FeetMeasurement f1 = _service.Create(value1);

                FeetMeasurement f2 = _service.Create(value2);

                bool result = _service.AreEqual(f1, f2);

                System.Console.WriteLine("\n+--------------------------------------+");
                System.Console.WriteLine($"| Equality Result : {result,-10}|");
                System.Console.WriteLine("+--------------------------------------+");

                double difference = Math.Abs(value1 - value2);
                System.Console.WriteLine($"Difference: {difference} ft");

                if (difference == 0)
                    System.Console.WriteLine("Measurements are identical.");
                else if (difference < 1)
                    System.Console.WriteLine("Measurements are close.");
                else
                    System.Console.WriteLine("Measurements differ significantly.");
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
    }
}