using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums;
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
    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public UserService(IFusionCache cache, DatabaseContext databaseContext, IRemoteUserService users, IAuthContext authContext)
    {
        _cache = cache;
        _users = users;
        _authContext = authContext;
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

    public async Task<User?> GetSelfAsync()
    {
        if (!_authContext.HasUser())
            return null;

        var currentUser = _authContext.GetUser();

        var cachedUser = await _cache.GetOrDefaultAsync<User>(GetKey(currentUser.IdentityId));

        if (cachedUser is not null)
            return cachedUser;

        var dbUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

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

        var newUser = new User
        {
            Roles = userRoles,
            Id = Guid.NewGuid(),
            IdentityId = authUser.UserId,
            Email = authUser.Email,
            FirstName = authUser.Name,
            LastName = authUser.Nickname,
            CreatedAt = authUser.CreatedAt,
            UpdatedAt = authUser.UpdatedAt,
            Providers = authUser.Identities.Select(i => i.Provider).ToList()
        };

        var entry = await _databaseContext.Users.AddAsync(newUser);
        await _databaseContext.SaveChangesAsync();

        await UpsertCacheAsync(entry.Entity);
        return entry.Entity;
    }

    public async Task UpsertCacheAsync(User user)
    {
        var cachedUser = await _cache.TryGetAsync<User>(GetKey(user.IdentityId));
        if (!cachedUser.HasValue)
            await _cache.SetAsync(GetKey(user.IdentityId), user);

        cachedUser = await _cache.TryGetAsync<User>(GetKey(user.Id.ToString()));
        if (!cachedUser.HasValue)
            await _cache.SetAsync(GetKey(user.Id.ToString()), user);
    }

    private static string GetKey(in string id)
    {
        return $"user:{id}";
    }
}
