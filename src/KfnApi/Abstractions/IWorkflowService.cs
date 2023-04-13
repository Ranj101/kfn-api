using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IWorkflowService
{
    Task<Result<User>> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request);
    Task<Result<Producer>> UpdateProducerStateAsync(Guid id, UpdateProducerStateRequest request);
    Task<Result<ApprovalForm>> UpdateFormStateAsync(Guid id, UpdateFormStateRequest request);
}
