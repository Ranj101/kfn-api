using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Database;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Services;

public class SelfService : ISelfService
{
    private readonly IFusionCache _cache;
    private readonly IAuthContext _authContext;
    private readonly DatabaseContext _databaseContext;

    public SelfService(IFusionCache cache, DatabaseContext databaseContext, IAuthContext authContext)
    {
        _cache = cache;
        _authContext = authContext;
        _databaseContext = databaseContext;
    }

    public async Task<User?> GetSelfAsync()
    {
        return await _databaseContext.Users
            .FirstOrDefaultAsync(u => u.Id == _authContext.GetUserId());
    }

    public async Task<Result<User>> UpdateSelfAsync(UpdateSelfRequest request)
    {
        var uploads = new List<Upload>();

        var hasCover = request.CoverPicture.HasValue;
        var hasProfile = request.ProfilePicture.HasValue;

        if (hasCover)
            uploads = await TryAddUploadAsync(uploads, request.CoverPicture!.Value);

        if (hasProfile)
            uploads = await TryAddUploadAsync(uploads, request.ProfilePicture!.Value);

        var user = await _databaseContext.Users
            .Include(u => u.Uploads)
            .FirstOrDefaultAsync(u => u.Id == _authContext.GetUserId());

        user!.CoverPicture = hasCover ? uploads.Find(u => u.Key == request.CoverPicture!.Value)?.Key : null;
        user.ProfilePicture = hasProfile ? uploads.Find(u => u.Key == request.ProfilePicture!.Value)?.Key : null;
        user.Uploads = uploads;
        user.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();
        await RemoveCacheAsync(user);
        return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
    }

    private async Task<List<Upload>> TryAddUploadAsync(List<Upload> uploads, Guid key)
    {
        var upload = await _databaseContext.Uploads.FirstOrDefaultAsync(u => u.Key == key);

        if(upload is not null)
            uploads.Add(upload);

        return uploads;
    }

    private async Task RemoveCacheAsync(User user)
    {
        await _cache.RemoveAsync(GetKey(user.IdentityId));

        string GetKey(in string id)
        {
            return $"user:{id}";
        }
    }
}
