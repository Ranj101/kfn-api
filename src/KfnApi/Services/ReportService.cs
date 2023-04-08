using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class ReportService : IReportService
{
    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public ReportService(DatabaseContext databaseContext, IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<UserReport?> GetUserReportByIdAsync(Guid id)
    {
        return await _databaseContext.UserReports
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ProducerReport?> GetProducerReportByIdAsync(Guid id)
    {
        return await _databaseContext.ProducerReports
            .Include(r => r.Producer)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PaginatedList<UserReport>> GetAllUserReportsByIdAsync(GetAllReportsRequest request)
    {
        var reports = _databaseContext.UserReports
            .Where(u => u.UserId == request.AffiliatedEntityId);

        var paginated = await PaginatedList<UserReport>.CreateAsync(reports, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<PaginatedList<ProducerReport>> GetAllProducerReportsByIdAsync(GetAllReportsRequest request)
    {
        var reports = _databaseContext.ProducerReports
            .Where(p => p.ProducerId == request.AffiliatedEntityId);

        var paginated = await PaginatedList<ProducerReport>.CreateAsync(reports, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<PaginatedList<UserReport>> GetAllUserReportsAsync(GetAllReportsRequest request)
    {
        var reports = _databaseContext.UserReports
            .Include(r => r.User).AsQueryable();

        reports = request.SortDirection == SortDirection.Descending
            ? reports.OrderByDescending(r => r.CreatedAt)
            : reports.OrderBy(r => r.CreatedAt);

        var paginated = await PaginatedList<UserReport>.CreateAsync(reports, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<PaginatedList<ProducerReport>> GetAllProducerReportsAsync(GetAllReportsRequest request)
    {
        var reports = _databaseContext.ProducerReports
            .Include(r => r.Producer).AsQueryable();

        reports = request.SortDirection == SortDirection.Descending
            ? reports.OrderByDescending(r => r.CreatedAt)
            : reports.OrderBy(r => r.CreatedAt);

        var paginated = await PaginatedList<ProducerReport>.CreateAsync(reports, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<UserReport> CreateUserReportAsync(Guid id, SubmitReportRequest request)
    {
        var report = new UserReport
        {
            Id = Guid.NewGuid(),
            UserId = id,
            Title = request.Title,
            Summary = request.Summary,
            CreatedBy = _authContext.GetUserId()
        };

        var entry = await _databaseContext.UserReports.AddAsync(report);
        await _databaseContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<ProducerReport> CreateProducerReportAsync(Guid id, SubmitReportRequest request)
    {
        var report = new ProducerReport
        {
            Id = Guid.NewGuid(),
            ProducerId = id,
            Title = request.Title,
            Summary = request.Summary,
            CreatedBy = _authContext.GetUserId()
        };

        var entry = await _databaseContext.ProducerReports.AddAsync(report);
        await _databaseContext.SaveChangesAsync();

        return entry.Entity;
    }
}
