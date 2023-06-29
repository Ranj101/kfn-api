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
    private static readonly Dictionary<SortUserBy, ISortBy> SortFunctions = new ()
    {
        { SortUserBy.DateCreated, new SortBy<User, DateTime>(x => x.CreatedAt) },
        { SortUserBy.FirstName, new SortBy<User, string>(x => x.FirstName) },
        { SortUserBy.LastName, new SortBy<User, string>(x => x.LastName) }
    };

    private readonly IFusionCache _cache;
    private readonly DatabaseContext _databaseContext;

    public UserService(IFusionCache cache, DatabaseContext databaseContext)
    {
        _cache = cache;
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

    public async Task<User?> GetByIdAsync(Guid id, bool activeOnly = false)
    {
        return await _databaseContext.Users
            .Include(u => u.Producer)
            .Include(u => u.AbuseReports)
            .FirstOrDefaultAsync(u => u.Id == id && (!activeOnly || u.State == UserState.Active));
    }

    public async Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request)
    {
        var stateFilter = request.FilterByState is null;
        var emailFilter = request.FilterByEmail is null;

        var users = _databaseContext.Users
            .Include(u => u.Producer)
            .Include(u => u.AbuseReports)
            .Where(user => (emailFilter || user.Email.ToLower().Contains(request.FilterByEmail!.Trim().ToLower())) &&
                           (stateFilter || user.State == request.FilterByState))
            .AsQueryable();

        users = request.SortDirection == SortDirection.Descending
            ? users.SortByDescending(SortFunctions[request.SortBy])
            : users.SortBy(SortFunctions[request.SortBy]);

        var paginated = await PaginatedList<User>.CreateAsync(users, request.PageIndex, request.PageSize);

        return paginated;
    }

    public async Task<User> EnrollUserAsync(FirebaseUser identityUser)
    {
        var newUserId = Guid.NewGuid();

        var newUser = new User
        {
            Roles = new List<string> { Roles.Customer },
            Id = newUserId,
            ProfilePicture = identityUser.ProfilePicture ?? Constants.DefaultUserProfilePicture,
            CoverPicture = Constants.DefaultUserCoverPicture,
            IdentityId = identityUser.Id,
            Email = identityUser.Email,
            FirstName = identityUser.FirstName,
            LastName = identityUser.LastName,
            State = UserState.Active,
            CreatedBy = newUserId,
            CreatedAt = DateTime.UtcNow
        };

        var entry = await _databaseContext.Users.AddAsync(newUser);
        await _databaseContext.SaveChangesAsync();

        await UpsertCacheAsync(entry.Entity);
        return entry.Entity;
    }

    public async Task<Result<User>> UpdateUserRoleAsync(Guid id, string role, bool remove = false, bool allowInactiveUser = false)
    {
        var user = await _databaseContext.Users.FirstOrDefaultAsync(u =>
            u.Id == id && (allowInactiveUser || u.State == UserState.Active));

        if (user is null)
            return Result<User>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status422UnprocessableEntity,
                Title = "Role Update Failed",
                Detail = "Associated user was not found or is inactive."
            });

        if (remove)
        {
            if (!user.Roles.Remove(role))
                return Result<User>.ErrorResult(new Error
                {
                    HttpCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Role Update Failed",
                    Detail = "Failed to remove role from list"
                });
        }
        else
        {
            if(user.Roles.Contains(role))
                return Result<User>.ErrorResult(new Error
                {
                    HttpCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Role Update Failed",
                    Detail = "Failed to add role to list"
                });

            user.Roles.Add(role);
        }

        await _databaseContext.SaveChangesAsync();
        return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
    }

    private async Task UpsertCacheAsync(User user)
    {
        await _cache.SetAsync(GetKey(user.IdentityId), user);
    }

    private static string GetKey(in string id)
    {
        return $"user:{id}";
    }
}
