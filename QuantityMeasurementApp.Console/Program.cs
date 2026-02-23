using QuantityMeasurementApp.Console;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;

namespace QuantityMeasurementApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IMeasurementComparer measurementService = new MeasurementComparerService();

            var menu = new Menu(measurementService);

            menu.Show();
        }
    }
}