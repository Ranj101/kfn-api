using KfnApi.Abstractions;
using KfnApi.DTOs.Responses;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Helpers.Extensions;

public static class Mappers
{
    public static PaginatedResponse<T, TMapped> ToPaginatedResponse<T, TMapped>(this PaginatedList<T> paginated, List<TMapped> data)
    {
        return new PaginatedResponse<T, TMapped>(paginated, data);
    }

    public static UserResponse ToUserResponse(this User user, ICloudStorageService service)
    {
        if(user.AbuseReports is null)
            throw new ArgumentException("null parameter mapping", nameof(user.AbuseReports));

        var coverUrl = GetPreSignedUrl(user.CoverPicture, service);
        var profileUrl = GetPreSignedUrl(user.ProfilePicture, service);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            State = user.State,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CoverPicture = coverUrl,
            ProfilePicture = profileUrl,
            Providers = user.Providers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IdentityId = user.IdentityId,
            ProducerPage = user.Producer?.ToProducerPageResponse(),
            AbuseReports = user.AbuseReports.Select(x => x.ToReportResponse()).ToList()
        };
    }

    public static UserListResponse ToUserListResponse(this User user, ICloudStorageService service)
    {
        var coverUrl = GetPreSignedUrl(user.CoverPicture, service);
        var profileUrl = GetPreSignedUrl(user.ProfilePicture, service);

        return new UserListResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            State = user.State,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CoverPicture = coverUrl,
            ProfilePicture = profileUrl,
            Providers = user.Providers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IdentityId = user.IdentityId
        };
    }

    public static ProfileResponse ToProfileResponse(this User user, ICloudStorageService service)
    {
        var coverUrl = GetPreSignedUrl(user.CoverPicture, service);
        var profileUrl = GetPreSignedUrl(user.ProfilePicture, service);

        return new ProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CoverPicture = coverUrl,
            ProfilePicture = profileUrl,
            CreatedAt = user.CreatedAt,
            Providers = user.Providers
        };
    }

    public static SelfResponse ToSelfResponse(this User user, ICloudStorageService service)
    {
        var coverUrl = GetPreSignedUrl(user.CoverPicture, service);
        var profileUrl = GetPreSignedUrl(user.ProfilePicture, service);

        return new SelfResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CoverPicture = coverUrl,
            ProfilePicture = profileUrl,
            Providers = user.Providers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static ProducerPageResponse ToProducerPageResponse(this Producer producer)
    {
        return new ProducerPageResponse
        {
            Id = producer.Id,
            Name = producer.Name,
            Reviews = producer.Reviews,
            Locations = producer.Locations,
            CreatedAt = producer.CreatedAt,
            OpeningTime = producer.OpeningTime,
            ClosingTime = producer.ClosingTime
        };
    }

    public static ReportResponse ToReportResponse<T>(this T report) where T : IReport
    {
        return new ReportResponse
        {
            Id = report.Id,
            Title = report.Title,
            Summary = report.Summary,
            CreatedBy = report.CreatedBy,
            CreatedAt = report.CreatedAt,
            UpdatedBy = report.UpdatedBy,
            UpdatedAt = report.UpdatedAt
        };
    }

    public static UserReportResponse ToUserReportResponse(this UserReport report, ICloudStorageService service)
    {
        if(report.User is null)
            throw new ArgumentException("null parameter mapping", nameof(report.User));

        return new UserReportResponse
        {
            Id = report.Id,
            Title = report.Title,
            Summary = report.Summary,
            CreatedBy = report.CreatedBy,
            CreatedAt = report.CreatedAt,
            UpdatedBy = report.UpdatedBy,
            UpdatedAt = report.UpdatedAt,
            User = report.User.ToProfileResponse(service)
        };
    }

    public static ProducerReportResponse ToProducerReportResponse(this ProducerReport report)
    {
        if(report.Producer is null)
            throw new ArgumentException("null parameter mapping", nameof(report.Producer));

        return new ProducerReportResponse
        {
            Id = report.Id,
            Title = report.Title,
            Summary = report.Summary,
            CreatedBy = report.CreatedBy,
            CreatedAt = report.CreatedAt,
            UpdatedBy = report.UpdatedBy,
            UpdatedAt = report.UpdatedAt,
            Producer = report.Producer.ToProducerPageResponse()
        };
    }

    public static UploadResponse ToUploadResponse(this Upload upload)
    {
        return new UploadResponse
        {
            Key = upload.Key,
            Size = upload.Size,
            ContentType = upload.ContentType,
            OriginalName = upload.OriginalName,
            DateUploaded = upload.DateUploaded
        };
    }

    public static FormResponse ToFormResponse(this ApprovalForm form, ICloudStorageService service)
    {
        if(form.Uploads is null)
            throw new ArgumentException("null parameter mapping", nameof(form.Uploads));

        if(form.User is null)
            throw new ArgumentException("null parameter mapping", nameof(form.User));

        return new FormResponse
        {
            Id = form.Id,
            State = form.State,
            ProducerName = form.ProducerName,
            Locations = form.Locations,
            OpeningTime = form.OpeningTime,
            ClosingTime = form.ClosingTime,
            CreatedBy = form.CreatedBy,
            UpdatedBy = form.UpdatedBy,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            User = form.User.ToProfileResponse(service),
            Uploads = form.Uploads.Select(x => GetPreSignedUrl(x.Key, service)!).ToList()
        };
    }

    public static FormListResponse ToFormListResponse(this ApprovalForm form)
    {
        return new FormListResponse
        {
            Id = form.Id,
            State = form.State,
            ProducerName = form.ProducerName,
            Locations = form.Locations,
            OpeningTime = form.OpeningTime,
            ClosingTime = form.ClosingTime,
            CreatedBy = form.CreatedBy,
            UpdatedBy = form.UpdatedBy,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }

    private static string? GetPreSignedUrl(Guid? key, ICloudStorageService service)
    {
        return key.HasValue ? service.GetPreSignedUrl(key.Value) : null;
    }
}
