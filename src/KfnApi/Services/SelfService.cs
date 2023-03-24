using KfnApi.Abstractions;
using KfnApi.Helpers.Database;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class SelfService : ISelfService
{
    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public SelfService(DatabaseContext databaseContext,IAuthContext authContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<User?> GetSelfAsync()
    {
        return await _databaseContext.Users
            .Include(u => u.Producer)
            .Include(u => u.AbuseReports)
            .FirstOrDefaultAsync(u => u.Id == _authContext.GetUserId());
    }
}
