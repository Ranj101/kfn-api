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

    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public ProducerService(DatabaseContext databaseContext, IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<Producer?> GetByIdAsync(Guid id, bool activeOnly = false)
    {
        return await _databaseContext.Producers
            .Include(p => p.User)
            .Include(p => p.Products)
            .Include(p => p.Uploads)
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

    public async Task CreateProducerAsync(ApprovalForm form)
    {
        var producer = new Producer
        {
            Id = Guid.NewGuid(),
            UserId = form.UserId,
            Name = form.ProducerName,
            Locations = form.Locations,
            OpeningTime = form.OpeningTime,
            ClosingTime = form.ClosingTime,
            State = ProducerState.Active,
            CreatedBy = form.UpdatedBy!.Value
        };

        await _databaseContext.Producers.AddAsync(producer);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<Result<Producer>> UpdateProducerAsync(UpdateProducerRequest request)
    {
        var uploads = new List<Upload>();

        foreach (var key in request.Uploads)
        {
            uploads = await TryAddUploadAsync(uploads, key);
        }

        if (uploads.Count != request.Uploads.Count)
            return Result<Producer>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Update Failed",
                Detail = "List contains invalid upload."
            });

        var producer = await _databaseContext.Producers
            .Include(p => p.Uploads)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == _authContext.GetUserId() && p.State == ProducerState.Active);

        if(producer is null)
            return Result<Producer>.NotFoundResult();

        var openingTime = new TimeOnly(request.OpeningTime.Hour, request.OpeningTime.Minute);
        var closingTime = new TimeOnly(request.ClosingTime.Hour, request.ClosingTime.Minute);

        producer.Locations = request.Locations;
        producer.OpeningTime = openingTime;
        producer.ClosingTime = closingTime;
        producer.Uploads = uploads;
        producer.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();
        return Result<Producer>.SuccessResult(producer, StatusCodes.Status200OK);
    }

    private async Task<List<Upload>> TryAddUploadAsync(List<Upload> uploads, Guid key)
    {
        var upload = await _databaseContext.Uploads.FirstOrDefaultAsync(u => u.Key == key);

        if(upload is not null)
            uploads.Add(upload);

        return uploads;
    }
}
