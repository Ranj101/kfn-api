using KfnApi.Abstractions;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Services;

public class UserService : IUserService
{
    private readonly IFusionCache _cache;
    private readonly IRemoteUserService _users;
    private readonly DatabaseContext _databaseContext;

    public UserService(IFusionCache cache, DatabaseContext databaseContext, IRemoteUserService users)
    {
        _cache = cache;
        _users = users;
        _databaseContext = databaseContext;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var cachedUser = await _cache.GetOrDefaultAsync<User>(GetKey(id));

        if (cachedUser is not null)
            return cachedUser;

        var dbUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (dbUser is null)
            return null;

        await UpsertCacheAsync(dbUser);
        return dbUser;
    }

    public async Task<User?> EnrollUserAsync(string id)
    {
        var authUser = await _users.GetByIdAsync(id);

        if (authUser is null)
            return null;

        var userRoles = new List<string> { Roles.Customer };

        if (authUser.AppMetadata.Roles.Contains("kfn-admin"))
            userRoles.Add(Roles.SuperAdmin);

        var newUser = new User
        {
            Roles = userRoles,
            Id = authUser.UserId,
            Email = authUser.Email,
            FirstName = authUser.Name,
            LastName = authUser.Nickname,
            Username = authUser.Username
        };

        try
        {
            var entry = await _databaseContext.Users.AddAsync(newUser);
            await _databaseContext.SaveChangesAsync();
            return entry.Entity;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task UpsertCacheAsync(User user)
    {
        var cachedUser = await _cache.TryGetAsync<User>(GetKey(user.Id));
        if (!cachedUser.HasValue)
            await _cache.SetAsync(GetKey(user.Id), user);
    }

    private static string GetKey(in string id)
    {
        return $"user:{id}";
    }
}
