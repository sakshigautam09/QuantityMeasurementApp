// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : StorageMode.cs
// UC-17   : Reserved for future extensibility.
//           Routing in UC-17 is AUTOMATIC:
//             • DB reachable  → save directly to SQL Server
//             • DB unreachable → save to JSON cache file
//           This enum is not used at runtime in UC-17.
// ============================================================

namespace QuantityMeasurementApp.RepositoryLayer
{
    public enum StorageMode
    {
        Cache,
        Database
    }
}