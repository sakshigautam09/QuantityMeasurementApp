using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    /// <summary>Async repository for quantity measurements (EF Core).</summary>
    public interface IQuantityMeasurementRepository
    {
        Task SaveAsync(QuantityMeasurementEFEntity entity);
        Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetAllAsync();
        Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetByOperationAsync(string operation);
        Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetByCategoryAsync(string category);
        Task<IReadOnlyList<QuantityMeasurementEFEntity>> GetErroredAsync();
        Task<int>  GetCountByOperationAsync(string operation);
        Task<int>  GetTotalCountAsync();
        Task ClearAllAsync();
    }
}
