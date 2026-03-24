// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Models/QuantityMeasurementDbEntity.cs
// UC-17   : Flat EF Core entity mapped to quantity_measurements table.
//
// WHY THIS FILE EXISTS:
//   The existing QuantityMeasurementEntity (ModelLayer) uses
//   QuantityDTO as nested objects — EF Core cannot map nested
//   complex objects directly. This flat entity stores all
//   fields as simple scalar columns that EF Core can map.
//
//   Service layer converts between:
//     QuantityMeasurementEntity (business) ↔ QuantityMeasurementDbEntity (DB)
// ============================================================

namespace QuantityMeasurementApp.RepositoryLayer
{
    public class QuantityMeasurementDbEntity
    {
        public Guid     Id              { get; set; }
        public DateTime Timestamp       { get; set; }
        public string   Operation       { get; set; } = "";    // enum as string
        public string   MeasurementType { get; set; } = "";    // Length/Weight/Volume/Temperature
        public double   FirstValue      { get; set; }
        public string   FirstUnit       { get; set; } = "";
        public double?  SecondValue     { get; set; }
        public string?  SecondUnit      { get; set; }
        public string?  TargetUnit      { get; set; }
        public string   ResultDisplay   { get; set; } = "";
        public bool     HasError        { get; set; }
        public string?  ErrorMessage    { get; set; }
    }
}