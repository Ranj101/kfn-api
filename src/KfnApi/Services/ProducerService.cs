using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class ProducerService : IProducerService
{
    private static readonly Dictionary<SortProducerBy, ISortBy> SortFunctions = new ()
    {
        { SortProducerBy.DateCreated, new SortBy<Producer, DateTime>(x => x.CreatedAt) },
        { SortProducerBy.OpeningTime, new SortBy<Producer, TimeOnly>(x => x.OpeningTime) },
        { SortProducerBy.ClosingTime, new SortBy<Producer, TimeOnly>(x => x.ClosingTime) }
    };

    private readonly DatabaseContext _databaseContext;

    public ProducerService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Producer?> GetByIdAsync(Guid id, bool activeOnly = false)
    {
        return await _databaseContext.Producers
            .Include(p => p.User)
            .Include(p => p.Products)
            .FirstOrDefaultAsync(p => p.Id == id && (!activeOnly || p.State == ProducerState.Active));
    }

    public async Task<PaginatedList<Producer>> GetAllProducersAsync(GetAllProducersRequest request)
    {
        var stateFilter = request.FilterByState is null;
        var emailFilter = request.FilterByUserEmail is null;
        var idFilter = request.FilterByUserId is null;

        var producers = _databaseContext.Producers
            .Include(f => f.User)
            .Where(producer => (emailFilter || producer.User!.Email.ToLower().Contains(request.FilterByUserEmail!.Trim().ToLower())) &&
                               (stateFilter || producer.State == request.FilterByState) &&
                               (idFilter || producer.UserId == request.FilterByUserId))
            .AsQueryable();

        producers = request.SortDirection == SortDirection.Descending
            ? producers.SortByDescending(SortFunctions[request.SortBy])
            : producers.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<Producer>.CreateAsync(producers, request.PageIndex, request.PageSize);

        return paginated;
    }
}
