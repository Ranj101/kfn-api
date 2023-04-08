using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Services;

public class UserService : IUserService
{
    private static readonly Dictionary<SortBy, ISortBy> SortFunctions = new ()
    {
        { SortBy.DateCreated, new SortBy<User, DateTime>(x => x.CreatedAt) },
        { SortBy.FirstName, new SortBy<User, string>(x => x.FirstName) },
        { SortBy.LastName, new SortBy<User, string>(x => x.LastName) }
    };

    private readonly IFusionCache _cache;
    private readonly IRemoteUserService _users;
    private readonly IAuthContext _authContext;
    private readonly WorkflowContext _workflowContext;
    private readonly DatabaseContext _databaseContext;

    public UserService(IFusionCache cache, DatabaseContext databaseContext, IRemoteUserService users, WorkflowContext workflowContext,
        IAuthContext authContext)
    {
        _cache = cache;
        _users = users;
        _authContext = authContext;
        _workflowContext = workflowContext;
        _databaseContext = databaseContext;
    }

    public async Task<User?> GetByIdentityIdAsync(string id)
    {
        var cachedUser = await _cache.GetOrDefaultAsync<User>(GetKey(id));

        if (cachedUser is not null)
            return cachedUser;

        var dbUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.IdentityId == id);

        if (dbUser is null)
            return null;

        await UpsertCacheAsync(dbUser);
        return dbUser;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _databaseContext.Users
            .Include(u => u.Producer)
            .Include(u => u.AbuseReports)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request)
    {
        var users = _databaseContext.Users
            .Include(u => u.Producer)
            .Include(u => u.AbuseReports)
            .Where(user => request.FilterByEmail == null || user.Email.ToLower().Contains(request.FilterByEmail!.Trim().ToLower()))
            .AsQueryable();

        users = request.SortDirection == SortDirection.Descending
            ? users.SortByDescending(SortFunctions[request.SortBy])
            : users.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<User>.CreateAsync(users, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<User?> EnrollUserAsync(string id)
    {
        var authUser = await _users.GetByIdAsync(id);

        if (authUser is null)
            return null;

        var userRoles = new List<string> { Roles.Customer };

        if (authUser.Roles.Contains("kfn-admin"))
            userRoles.AddRange(new []{Roles.SuperAdmin, Roles.SystemAdmin});

        var newUserId = Guid.NewGuid();

        var newUser = new User
        {
            Roles = userRoles,
            Id = newUserId,
            IdentityId = authUser.UserId,
            Email = authUser.Email,
            FirstName = authUser.Name,
            LastName = authUser.Nickname,
            State = UserState.Active,
            CreatedBy = newUserId,
            CreatedAt = authUser.CreatedAt,
            Providers = authUser.Identities.Select(i => i.Provider).ToList()
        };

        var entry = await _databaseContext.Users.AddAsync(newUser);
        await _databaseContext.SaveChangesAsync();

        await UpsertCacheAsync(entry.Entity);
        return entry.Entity;
    }

    public async Task<Result<User>> UpdateUserState(Guid id, UpdateUserStateRequest request)
    {
        var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Result<User>.NotFoundResult();

        if (request.Trigger == UserTrigger.Deactivate)
        {
            //TODO: Block user at Auth0 level
            if (!_workflowContext.UserWorkflow.DeactivateUser(user))
                return Result<User>.StateErrorResult();

            user.UpdatedBy = _authContext.GetUserId();
            await _databaseContext.SaveChangesAsync();
            await RemoveCacheAsync(user);
            return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
        }

        if (!_workflowContext.UserWorkflow.ReactivateUser(user))
            return Result<User>.StateErrorResult();

        user.UpdatedBy = _authContext.GetUserId();
        await _databaseContext.SaveChangesAsync();
        await RemoveCacheAsync(user);
        return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
    }

    private async Task UpsertCacheAsync(User user)
    {
        await _cache.SetAsync(GetKey(user.IdentityId), user);
    }

    private async Task RemoveCacheAsync(User user)
    {
        await _cache.RemoveAsync(GetKey(user.IdentityId));
    }

    private static string GetKey(in string id)
    {
        return $"user:{id}";
    }
}
