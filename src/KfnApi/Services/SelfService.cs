using KfnApi.Abstractions;
using KfnApi.Models.Entities;

namespace KfnApi.Services;

public class SelfService : ISelfService
{
    private readonly IAuthContext _authContext;

    public SelfService(IAuthContext authContext)
    {
        _authContext = authContext;
    }

    public async Task<User?> GetSelfAsync()
    {
        return _authContext.HasUser() ? _authContext.GetUser() : null;
    }
}
