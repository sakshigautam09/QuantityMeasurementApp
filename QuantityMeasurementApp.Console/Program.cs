// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Program.cs
//
// UC-15 : N-Tier Architecture
//
// Responsibilities:
//   1. Initialize dependencies  (Factory Pattern)
//   2. Wire layers together     (Dependency Injection)
//   3. Delegate to IMenu        (Façade Pattern + DIP)
//   4. No business logic here
//
// Program depends on IMenu, not on Menu directly → DIP
// ============================================================

using QuantityMeasurementApp.Console.Interface;
namespace QuantityMeasurementApp.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IMenu menu = new Menu(Helper.CreateController());
            menu.Show();
        }
    }
}