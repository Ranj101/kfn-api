using KfnApi.Abstractions;
using KfnApi.Helpers.Database;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Services;

public class UploadService : IUploadService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ICloudStorageService _cloudService;

    public UploadService(DatabaseContext databaseContext,ICloudStorageService cloudService)
    {
        _cloudService = cloudService;
        _databaseContext = databaseContext;
    }

    public async Task<Upload> UploadAsync(IFormFile file, CancellationToken token)
    {
        var key = Guid.NewGuid();

        var upload = new Upload
        {
            Key = key,
            OriginalName = file.Name,
            ContentType = file.ContentType,
            Size = file.Length
        };

        var transaction = await _databaseContext.Database.BeginTransactionAsync(token);

        var entry = await _databaseContext.Uploads.AddAsync(upload, token);
        await _databaseContext.SaveChangesAsync(token);
        await _cloudService.PutObjectAsync(key.ToString(), file, token);

        await transaction.CommitAsync(token);

        return entry.Entity;
    }

    public async Task<Result<Upload>> DeleteAsync(Guid key, CancellationToken token)
    {
        var upload = await _databaseContext.Uploads
            .Include(u => u.Users)
            .Include(u => u.Producers)
            .Include(u => u.Products)
            .Include(u => u.ApprovalForms)
            .FirstOrDefaultAsync(u => u.Key == key, token);

        if (upload is null)
            return Result<Upload>.NotFoundResult();

        var result = EnsureUnlinked(upload);

        if (result is not null)
            return result;

        var transaction = await _databaseContext.Database.BeginTransactionAsync(token);

        _databaseContext.Uploads.Remove(upload);
        await _databaseContext.SaveChangesAsync(token);
        await _cloudService.DeleteObjectAsync(upload.Key.ToString(), token);

        await transaction.CommitAsync(token);

        return Result<Upload>.SuccessResult(upload, StatusCodes.Status200OK);
    }

    private Result<Upload>? EnsureUnlinked(Upload upload)
    {
        if (upload.Users?.Any() ?? false)
            return Result<Upload>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Deletion Failed",
                Detail = "upload has an association with one or more users. to delete any given upload, it must have no user associations."
            });

        if (upload.Producers?.Any() ?? false)
            return Result<Upload>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Deletion Failed",
                Detail = "upload has an association with one or more producers. to delete any given upload, it must have no producer associations."
            });

        if (upload.Products?.Any() ?? false)
            return Result<Upload>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Deletion Failed",
                Detail = "upload has an association with one or more products. to delete any given upload, it must have no product associations."
            });

        if (upload.ApprovalForms?.Any() ?? false)
            return Result<Upload>.ErrorResult(new Error
            {
                HttpCode = StatusCodes.Status400BadRequest,
                Title = "Deletion Failed",
                Detail = "upload has an association with one or more approval forms. to delete any given upload, it must have no approval form associations."
            });

        return null;
    }
}
