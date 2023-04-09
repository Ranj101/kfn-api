using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IApprovalFormService
{
    Task<ApprovalForm?> GetByIdAsync(Guid id);
    Task<PaginatedList<ApprovalForm>> GetAllFormsAsync(GetAllFormsRequest request);
    Task<Result<ApprovalForm>> CreateFormAsync(SubmitFormRequest request);
    Task<Result<ApprovalForm>> UpdateFormStateAsync(Guid id, UpdateFormStateRequest request);
    Task<Result<ApprovalForm>> UpdateFormAsync(Guid id, SubmitFormRequest request);
}
