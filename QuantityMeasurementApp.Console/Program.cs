using QuantityMeasurementApp.Console;
using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.Core.Services;

class Program
{
    static void Main()
    {
        IFeetComparer comparerService = new FeetComparerService();

        var menu = new Menu(comparerService);

        menu.Show();
    }
}