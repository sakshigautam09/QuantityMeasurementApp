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
            IWeightService weightService = new WeightService();
            IVolumeService volumeService = new VolumeService();
            var menu = new Menu(service, weightService, volumeService);
            menu.Show();
        }
    }
}