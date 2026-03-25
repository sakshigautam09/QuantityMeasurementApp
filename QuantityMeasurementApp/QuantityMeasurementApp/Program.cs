using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// UC16: Entry point.
    /// The controller's default constructor calls RepositoryFactory which reads
    /// appsettings.json to decide between SQL Server (Database) or in-memory (Cache).
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            IQuantityMeasurementApp app = new QuantityMeasurementController();
            app.Run();
        }
    }
}
