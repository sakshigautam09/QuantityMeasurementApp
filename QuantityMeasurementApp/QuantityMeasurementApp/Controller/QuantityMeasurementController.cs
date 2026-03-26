using Microsoft.Extensions.Logging.Abstractions;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementModel;
using QuantityMeasurementBusinessLayer;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementRepository;


namespace QuantityMeasurementApp.Controller
{
    public class QuantityMeasurementController : IQuantityMeasurementApp
    {
        private readonly IQuantityMeasurementService _service;
        private readonly IMeasurementHistoryRepository _repo;

        public QuantityMeasurementController()
        {
            _repo    = SelectRepository();
            _service = new QuantityMeasurementService(_repo, null, NullLogger<QuantityMeasurementService>.Instance);
        }

        private static IMeasurementHistoryRepository SelectRepository()
        {
            while (true)
            {
                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("|        Select Repository Type        |");
                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("| 1. Cache Repository                  |");
                Console.WriteLine("| 2. Database Repository               |");
                Console.WriteLine("+--------------------------------------+");
                Console.Write("\nEnter your choice: ");

                string? choice = Console.ReadLine()?.Trim();

                if (choice == "1")
                {
                    Console.WriteLine("\n[Controller] Cache Repository selected.");
                    return QuantityMeasurementCacheRepository.Instance;
                }
                else if (choice == "2")
                {
                    Console.WriteLine("\n[Controller] Database Repository selected.");
                    return new QuantityMeasurementDatabaseRepository();
                }
                else
                {
                    Console.WriteLine("\nInvalid choice. Please enter 1 or 2.\n");
                }
            }
        }

        public QuantityMeasurementController(IQuantityMeasurementService service, IMeasurementHistoryRepository repo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("|        Quantity Measurement App      |");
                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("| 1. Length Operations                 |");
                Console.WriteLine("| 2. Weight Operations                 |");
                Console.WriteLine("| 3. Volume Operations                 |");
                Console.WriteLine("| 4. Temperature Operations            |");
                Console.WriteLine("| 5. Operation History                 |");
                Console.WriteLine("| 6. Exit                              |");
                Console.WriteLine("+--------------------------------------+");

                Console.Write("\nEnter your choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RunLengthMenu();      break;
                    case "2": RunWeightMenu();      break;
                    case "3": RunVolumeMenu();      break;
                    case "4": RunTemperatureMenu(); break;
                    case "5": RunHistoryMenu();     break;

                    case "6":
                        running = false;
                        Console.WriteLine("\nThank you for using Quantity Measurement App.");
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
            }
        }

        public string PerformComparison(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Compare(q1, q2);
                bool equal = result.Value == 1;
                return $"Comparison Result: {(equal ? "true" : "false")}";
            }
            catch (QuantityMeasurementException ex) { return $"[ERROR] {ex.Message}"; }
        }

        public string PerformConversion(QuantityDTO q1, QuantityDTO targetUnit)
        {
            try
            {
                var result = _service.Convert(q1, targetUnit);
                return $"Conversion Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex) { return $"[ERROR] {ex.Message}"; }
        }

        public string PerformAddition(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Add(q1, q2);
                return $"Addition Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex) { return $"[ERROR] {ex.Message}"; }
        }

        public string PerformSubtraction(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Subtract(q1, q2);
                return $"Subtraction Result: {result.Value} {result.UnitName}";
            }
            catch (QuantityMeasurementException ex) { return $"[ERROR] {ex.Message}"; }
        }

        public string PerformDivision(QuantityDTO q1, QuantityDTO q2)
        {
            try
            {
                var result = _service.Divide(q1, q2);
                return $"Division Result: {result.Value} (scalar)";
            }
            catch (QuantityMeasurementException ex) { return $"[ERROR] {ex.Message}"; }
        }

        // ── History Menu ─────────────────────────────────────────────────

        private void RunHistoryMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("|          Operation History           |");
                Console.WriteLine("+--------------------------------------+");
                Console.WriteLine("| 1. Get All Measurements              |");
                Console.WriteLine("| 2. Get By Operation Type             |");
                Console.WriteLine("| 3. Get By Category                   |");
                Console.WriteLine("| 4. Get Total Count                   |");
                Console.WriteLine("| 5. Delete All Measurements           |");
                Console.WriteLine("| 6. Back to Main Menu                 |");
                Console.WriteLine("+--------------------------------------+");

                Console.Write("\nEnter your choice: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("|         All Measurements             |");
                        Console.WriteLine("+--------------------------------------+");
                        var all = _repo.GetAllMeasurements();
                        if (all.Count == 0)
                            Console.WriteLine("\n  No records found.");
                        else
                            foreach (var e in all)
                                Console.WriteLine(e.ToString());
                        Console.WriteLine($"\n  Total: {all.Count} record(s).");
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("|       Get By Operation Type          |");
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("| CONVERT | COMPARE | ADD             |");
                        Console.WriteLine("| SUBTRACT | DIVIDE                   |");
                        Console.WriteLine("+--------------------------------------+");
                        Console.Write("\nEnter operation type: ");
                        string? op = Console.ReadLine()?.Trim().ToUpper();
                        if (!string.IsNullOrWhiteSpace(op))
                        {
                            var result = _repo.GetByOperation(op);
                            if (result.Count == 0)
                                Console.WriteLine($"\n  No records found for operation: {op}");
                            else
                                foreach (var e in result)
                                    Console.WriteLine(e.ToString());
                            Console.WriteLine($"\n  Total: {result.Count} record(s).");
                        }
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("|          Get By Category             |");
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("| LENGTH | WEIGHT | VOLUME            |");
                        Console.WriteLine("| TEMPERATURE                         |");
                        Console.WriteLine("+--------------------------------------+");
                        Console.Write("\nEnter category: ");
                        string? cat = Console.ReadLine()?.Trim().ToUpper();
                        if (!string.IsNullOrWhiteSpace(cat))
                        {
                            var result = _repo.GetByCategory(cat);
                            if (result.Count == 0)
                                Console.WriteLine($"\n  No records found for category: {cat}");
                            else
                                foreach (var e in result)
                                    Console.WriteLine(e.ToString());
                            Console.WriteLine($"\n  Total: {result.Count} record(s).");
                        }
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("|          Get Total Count             |");
                        Console.WriteLine("+--------------------------------------+");
                        int count = _repo.GetTotalCount();
                        Console.WriteLine($"\n  Total measurements stored: {count}");
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("+--------------------------------------+");
                        Console.WriteLine("|       Delete All Measurements        |");
                        Console.WriteLine("+--------------------------------------+");
                        Console.Write("\n  Are you sure? (yes/no): ");
                        string? confirm = Console.ReadLine()?.Trim().ToLower();
                        if (confirm == "yes")
                        {
                            _repo.Clear();
                            Console.WriteLine("\n  All measurements deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("\n  Delete cancelled.");
                        }
                        break;

                    case "6":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // ── Length Menu ──────────────────────────────────────────────────

        private void RunLengthMenu()
        {
            Console.Clear();
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|         Length Operations            |");
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("| 1. Compare                           |");
            Console.WriteLine("| 2. Convert                           |");
            Console.WriteLine("| 3. Add                               |");
            Console.WriteLine("| 4. Subtract                          |");
            Console.WriteLine("| 5. Divide                            |");
            Console.WriteLine("+--------------------------------------+");
            Console.Write("\nEnter your choice: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("LENGTH");  break;
                case "2": RunConvert("LENGTH");  break;
                case "3": RunAdd("LENGTH");      break;
                case "4": RunSubtract("LENGTH"); break;
                case "5": RunDivide("LENGTH");   break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ── Weight Menu ──────────────────────────────────────────────────

        private void RunWeightMenu()
        {
            Console.Clear();
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|         Weight Operations            |");
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("| 1. Compare                           |");
            Console.WriteLine("| 2. Convert                           |");
            Console.WriteLine("| 3. Add                               |");
            Console.WriteLine("| 4. Subtract                          |");
            Console.WriteLine("| 5. Divide                            |");
            Console.WriteLine("+--------------------------------------+");
            Console.Write("\nEnter your choice: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("WEIGHT");  break;
                case "2": RunConvert("WEIGHT");  break;
                case "3": RunAdd("WEIGHT");      break;
                case "4": RunSubtract("WEIGHT"); break;
                case "5": RunDivide("WEIGHT");   break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ── Volume Menu ──────────────────────────────────────────────────

        private void RunVolumeMenu()
        {
            Console.Clear();
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|         Volume Operations            |");
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("| 1. Compare                           |");
            Console.WriteLine("| 2. Convert                           |");
            Console.WriteLine("| 3. Add                               |");
            Console.WriteLine("| 4. Subtract                          |");
            Console.WriteLine("| 5. Divide                            |");
            Console.WriteLine("+--------------------------------------+");
            Console.Write("\nEnter your choice: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("VOLUME");  break;
                case "2": RunConvert("VOLUME");  break;
                case "3": RunAdd("VOLUME");      break;
                case "4": RunSubtract("VOLUME"); break;
                case "5": RunDivide("VOLUME");   break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ── Temperature Menu ─────────────────────────────────────────────

        private void RunTemperatureMenu()
        {
            Console.Clear();
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|      Temperature Operations          |");
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("| 1. Compare                           |");
            Console.WriteLine("| 2. Convert                           |");
            Console.WriteLine("| 3. Add (not supported)               |");
            Console.WriteLine("| 4. Subtract (not supported)          |");
            Console.WriteLine("| 5. Divide (not supported)            |");
            Console.WriteLine("+--------------------------------------+");
            Console.Write("\nEnter your choice: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunCompare("TEMPERATURE");  break;
                case "2": RunConvert("TEMPERATURE");  break;
                case "3": RunAdd("TEMPERATURE");      break;
                case "4": RunSubtract("TEMPERATURE"); break;
                case "5": RunDivide("TEMPERATURE");   break;
                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ── Operations ───────────────────────────────────────────────────

        private void RunCompare(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Comparison ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformComparison(q1, q2));
            }
            catch (Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
        }

        private void RunConvert(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Conversion ---");
            try
            {
                Console.Write("Enter value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter source unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write($"Enter target unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1     = new QuantityDTO(v1, u1, category);
                var target = new QuantityDTO(0,  u2, category);

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"Target Unit   : {u2}");
                Console.WriteLine(PerformConversion(q1, target));
            }
            catch (Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
        }

        private void RunAdd(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Addition ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformAddition(q1, q2));
            }
            catch (Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
        }

        private void RunSubtract(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Subtraction ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformSubtraction(q1, q2));
            }
            catch (Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
        }

        private void RunDivide(string category)
        {
            Console.Clear();
            Console.WriteLine($"--- {category} Division ---");
            try
            {
                Console.Write("Enter first value: ");
                double v1 = ReadDouble();
                Console.Write($"Enter first unit {GetUnitHint(category)}: ");
                string u1 = ReadUnit();

                Console.Write("Enter second value: ");
                double v2 = ReadDouble();
                Console.Write($"Enter second unit {GetUnitHint(category)}: ");
                string u2 = ReadUnit();

                var q1 = new QuantityDTO(v1, u1, category);
                var q2 = new QuantityDTO(v2, u2, category);

                Console.WriteLine($"\nThis Quantity : {v1} {u1}");
                Console.WriteLine($"That Quantity : {v2} {u2}");
                Console.WriteLine(PerformDivision(q1, q2));
            }
            catch (Exception ex) { Console.WriteLine($"\n[ERROR] {ex.Message}"); }
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static double ReadDouble()
        {
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double val))
                throw new ArgumentException("Invalid input: value must be numeric.");
            if (val < 0)
                throw new ArgumentException("Value cannot be negative.");
            return val;
        }

        private static string ReadUnit()
        {
            string? input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");
            return input;
        }

        private static string GetUnitHint(string category)
            => category.ToUpperInvariant() switch
            {
                "LENGTH"      => "(feet/ft, inches/in, yards/yd, centimeters/cm)",
                "WEIGHT"      => "(kilogram/kg, gram/g, pound/lb)",
                "VOLUME"      => "(litre/l, millilitre/ml, gallon/gal)",
                "TEMPERATURE" => "(celsius/c, fahrenheit/f, kelvin/k)",
                _             => ""
            };
    }
}