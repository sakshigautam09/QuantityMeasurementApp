// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : IQuantityModelService.cs
//
// Purpose : Contract for arithmetic and conversion operations
//           on QuantityModel objects.
//           QuantityModel now carries IMeasurable, so no generic
//           type parameter is needed — dispatch goes through the interface.
// ============================================================

using QuantityMeasurementApp.Core.Interfaces;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.BusinessLayer
{
    public interface IQuantityModelService
    {
        QuantityModel ConvertTo(QuantityModel source, IMeasurable targetUnit);

        QuantityModel Add(QuantityModel first, QuantityModel second);
        QuantityModel AddWithTargetUnit(QuantityModel first, QuantityModel second, IMeasurable targetUnit);

        QuantityModel Subtract(QuantityModel first, QuantityModel second);
        QuantityModel SubtractWithTargetUnit(QuantityModel first, QuantityModel second, IMeasurable targetUnit);

        double Divide(QuantityModel first, QuantityModel second);

        bool AreEqual(QuantityModel first, QuantityModel second);
    }
}