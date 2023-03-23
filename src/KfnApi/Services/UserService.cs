using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
using KfnApi.Models.Enums.Workflows;
using KfnApi.Models.Requests;
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
    private readonly WorkflowContext _workflowContext;
    private readonly DatabaseContext _databaseContext;

    public UserService(IFusionCache cache, DatabaseContext databaseContext, IRemoteUserService users, WorkflowContext workflowContext)
    {
        _cache = cache;
        _users = users;
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
        var cachedUser = await _cache.GetOrDefaultAsync<User>(GetKey(id.ToString()));

        if (cachedUser is not null)
            return cachedUser;

        var dbUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (dbUser is null)
            return null;

        await UpsertCacheAsync(dbUser);
        return dbUser;
    }

    public async Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request)
    {
        var users = _databaseContext.Users
            .Where(user => request.SearchByEmail == null || user.Email.ToLower().Contains(request.SearchByEmail!.Trim().ToLower()))
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
            return Result<User>.FailureResult(new Error
            {
                HttpCode = StatusCodes.Status404NotFound,
                Title = "User Not Found."
            });

        if (request.Trigger == UserTrigger.Deactivate)
        {
            //TODO: Block user at Auth0 level
            if (!_workflowContext.UserWorkflow.DeactivateUser(user))
                return Result<User>.FailureResult(new Error
                {
                    HttpCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "User state update not permitted."
                });

            await _databaseContext.SaveChangesAsync();
            await RemoveCacheAsync(user);
            return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
        }

        if (!_workflowContext.UserWorkflow.ReactivateUser(user))
            return Result<User>.FailureResult(new Error
            {
                HttpCode = StatusCodes.Status422UnprocessableEntity,
                Title = "User state update not permitted."
            });

        await _databaseContext.SaveChangesAsync();
        await RemoveCacheAsync(user);
        return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
    }

    private async Task UpsertCacheAsync(User user)
    {
        await _cache.SetAsync(GetKey(user.IdentityId), user);
        await _cache.SetAsync(GetKey(user.Id.ToString()), user);
    }

    private async Task RemoveCacheAsync(User user)
    {
        await _cache.RemoveAsync(GetKey(user.IdentityId));
        await _cache.RemoveAsync(GetKey(user.Id.ToString()));
    }

    private static string GetKey(in string id)
    {
        return $"user:{id}";
    }
}
