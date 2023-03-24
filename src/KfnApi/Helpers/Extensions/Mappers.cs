using KfnApi.Abstractions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Responses;

namespace KfnApi.Helpers.Extensions;

public static class Mappers
{
    public static PaginatedResponse<T, TMapped> ToPaginatedResponse<T, TMapped>(this PaginatedList<T> paginated, List<TMapped> data)
    {
        return new PaginatedResponse<T, TMapped>(paginated, data);
    }

    public static UserResponse ToUserResponse(this User user)
    {
        if(user.AbuseReports is null)
            throw new ArgumentException("null parameter mapping", nameof(user.AbuseReports));

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            State = user.State,
            LastName = user.LastName,
            FirstName = user.FirstName,
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

    public static UserListResponse ToUserListResponse(this User user)
    {
        return new UserListResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            State = user.State,
            LastName = user.LastName,
            FirstName = user.FirstName,
            Providers = user.Providers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IdentityId = user.IdentityId
        };
    }

    public static ProfileResponse ToProfileResponse(this User user)
    {
        return new ProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            Providers = user.Providers
        };
    }

    public static SelfResponse ToSelfResponse(this User user)
    {
        return new SelfResponse
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles,
            LastName = user.LastName,
            FirstName = user.FirstName,
            Providers = user.Providers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            ProducerPage = user.Producer?.ToProducerPageResponse()
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

    public static UserReportResponse ToUserReportResponse(this UserReport report)
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
            User = report.User.ToProfileResponse()
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
}
