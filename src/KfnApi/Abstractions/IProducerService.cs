using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IProducerService
{
    Task<Producer?> GetByIdAsync(Guid id, bool activeOnly = false);
    Task<PaginatedList<Producer>> GetAllProducersAsync(GetAllProducersRequest request);
    Task CreateProducerAsync(ApprovalForm form);
    Task<Result<Producer>> UpdateProducerAsync(UpdateProducerRequest request);
    Task<Result<Producer>> UpdateProducerStateAsync(Guid id, UpdateProducerStateRequest request);
}
