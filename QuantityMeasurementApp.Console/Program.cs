using QuantityMeasurementApp.Console;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;

namespace QuantityMeasurementApp.Console
{
    class Program
    {
        static void Main()
        {
            ILengthService service = new LengthService();
            var menu = new Menu(service);
            menu.Show();
        }
    }
}