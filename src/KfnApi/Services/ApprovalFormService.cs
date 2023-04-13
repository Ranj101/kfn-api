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

public class ApprovalFormService : IApprovalFormService
{
    private static readonly Dictionary<SortFormBy, ISortBy> SortFunctions = new ()
    {
        { SortFormBy.DateCreated, new SortBy<ApprovalForm, DateTime>(x => x.CreatedAt) }
    };

    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public ApprovalFormService(DatabaseContext databaseContext, IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<ApprovalForm?> GetByIdAsync(Guid id)
    {
        return await _databaseContext.ApprovalForms
            .Include(u => u.User)
            .Include(u => u.Uploads)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<PaginatedList<ApprovalForm>> GetAllFormsAsync(GetAllFormsRequest request)
    {
        var stateFilter = request.FilterByState is null;
        var emailFilter = request.FilterByUserEmail is null;
        var idFilter = request.FilterByUserId is null;

        var forms = _databaseContext.ApprovalForms
            .Include(f => f.User)
            .Where(form => (emailFilter || form.User!.Email.ToLower().Contains(request.FilterByUserEmail!.Trim().ToLower())) &&
                           (stateFilter || form.State == request.FilterByState) &&
                           (idFilter || form.UserId == request.FilterByUserId))
            .AsQueryable();

        forms = request.SortDirection == SortDirection.Descending
            ? forms.SortByDescending(SortFunctions[request.SortBy])
            : forms.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<ApprovalForm>.CreateAsync(forms, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<Result<ApprovalForm>> CreateFormAsync(SubmitFormRequest request)
    {
        var user = await _databaseContext.Users
            .Include(u => u.ApprovalForms)
            .FirstOrDefaultAsync(u => u.Id == _authContext.GetUserId());

        if(user!.ApprovalForms?.Any(f => f.State is ApprovalFormState.Pending or ApprovalFormState.Approved) ?? false)
            return Result<ApprovalForm>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status422UnprocessableEntity,
                Title = "Submit Failed",
                Detail = "User already has a pending/approved form."
            });

        var uploads = new List<Upload>();

        foreach (var key in request.Uploads)
        {
            uploads = await TryAddUploadAsync(uploads, key);
        }

        if (uploads.Count != request.Uploads.Count)
            return Result<ApprovalForm>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Submit Failed",
                Detail = "List contains invalid upload."
            });

        var openingTime = new TimeOnly(request.OpeningTime.Hour, request.OpeningTime.Minute);
        var closingTime = new TimeOnly(request.ClosingTime.Hour, request.ClosingTime.Minute);

        var form = new ApprovalForm
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ProducerName = request.ProducerName,
            Locations = request.Locations,
            OpeningTime = openingTime,
            ClosingTime = closingTime,
            State = ApprovalFormState.Pending,
            CreatedBy = user.Id,
            User = user,
            Uploads = uploads
        };

        var entry = await _databaseContext.ApprovalForms.AddAsync(form);
        await _databaseContext.SaveChangesAsync();

        return Result<ApprovalForm>.SuccessResult(entry.Entity, StatusCodes.Status201Created);
    }

    public async Task<Result<ApprovalForm>> UpdateFormAsync(SubmitFormRequest request)
    {
        var uploads = new List<Upload>();

        foreach (var key in request.Uploads)
        {
            uploads = await TryAddUploadAsync(uploads, key);
        }

        if (uploads.Count != request.Uploads.Count)
            return Result<ApprovalForm>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Update Failed",
                Detail = "List contains invalid upload."
            });

        var form = await _databaseContext.ApprovalForms
            .Include(f => f.Uploads)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.UserId == _authContext.GetUserId() && f.State == ApprovalFormState.Pending);

        if(form is null)
            return Result<ApprovalForm>.NotFoundResult();

        var openingTime = new TimeOnly(request.OpeningTime.Hour, request.OpeningTime.Minute);
        var closingTime = new TimeOnly(request.ClosingTime.Hour, request.ClosingTime.Minute);

        form.ProducerName = request.ProducerName;
        form.Locations = request.Locations;
        form.OpeningTime = openingTime;
        form.ClosingTime = closingTime;
        form.Uploads = uploads;
        form.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();
        return Result<ApprovalForm>.SuccessResult(form, StatusCodes.Status200OK);
    }

    private async Task<List<Upload>> TryAddUploadAsync(List<Upload> uploads, Guid key)
    {
        var upload = await _databaseContext.Uploads.FirstOrDefaultAsync(u => u.Key == key);

        if(upload is not null)
            uploads.Add(upload);

        return uploads;
    }
}
