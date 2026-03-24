// ============================================================
// PROJECT : QuantityMeasurementApp.Console
// FILE    : Services/Menu.cs
// UC-17   : Delegates to QuantityMeasurementController.
// ============================================================

using QuantityMeasurementApp.Console.Controller;
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