// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Menu.cs
//
// UC-15 : N-Tier Architecture
//
// Responsibilities:
//   • Implements IMenu
//   • Receives the fully-wired controller via constructor (DI)
//   • Delegates 100% to controller.ShowMenu()
//   • Zero business logic
//   • Zero user-input reading
//   • Zero service or repository access
// ============================================================

using QuantityMeasurementApp.BusinessLayer;
using QuantityMeasurementApp.Console.Interface;

namespace QuantityMeasurementApp.Console
{
    public class Menu : IMenu
    {
        private readonly QuantityMeasurementController _controller;

        public Menu(QuantityMeasurementController controller)
        {
            _controller = controller;
        }

        public void Show() => _controller.ShowMenu();
    }
}