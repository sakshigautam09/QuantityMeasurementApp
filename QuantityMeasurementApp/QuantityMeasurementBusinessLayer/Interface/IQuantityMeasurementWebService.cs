using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>UC17: Async service interface for Web API operations.</summary>
    public interface IQuantityMeasurementWebService
    {
        Task<QuantityMeasurementDto> CompareAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null);
        Task<QuantityMeasurementDto> ConvertAsync(QuantityRequestDto q1, QuantityRequestDto target, int? userId = null);
        Task<QuantityMeasurementDto> AddAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null);
        Task<QuantityMeasurementDto> SubtractAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null);
        Task<QuantityMeasurementDto> DivideAsync(QuantityRequestDto q1, QuantityRequestDto q2, int? userId = null);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationAsync(string operation);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByCategoryAsync(string category);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryAsync();
        Task<int> GetCountByOperationAsync(string operation);
    }
}
