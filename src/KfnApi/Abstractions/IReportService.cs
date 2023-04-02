using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IReportService
{
    Task<UserReport?> GetUserReportByIdAsync(Guid id);
    Task<ProducerReport?> GetProducerReportByIdAsync(Guid id);
    Task<PaginatedList<UserReport>> GetAllUserReportsByIdAsync(GetAllReportsRequest request);
    Task<PaginatedList<ProducerReport>> GetAllProducerReportsByIdAsync(GetAllReportsRequest request);
    Task<PaginatedList<UserReport>> GetAllUserReportsAsync(GetAllReportsRequest request);
    Task<PaginatedList<ProducerReport>> GetAllProducerReportsAsync(GetAllReportsRequest request);
    Task<UserReport> CreateUserReportAsync(Guid id, SubmitReportRequest request);
    Task<ProducerReport> CreateProducerReportAsync(Guid id, SubmitReportRequest request);
}
