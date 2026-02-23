using QuantityMeasurementApp.Console;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;

namespace QuantityMeasurementApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ILengthComparer service = new LengthComparerService();
            var menu = new Menu(service);
            menu.Show();
        }
    }
}