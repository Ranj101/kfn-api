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
            AbuseReports = user.AbuseReports.Select(x => x.ToBasicReportResponse()).ToList()
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

    public static BasicReportResponse ToBasicReportResponse<T>(this T report) where T : IAbuseReport
    {
        return new BasicReportResponse
        {
            Id = report.Id,
            Title = report.Title,
            Summary = report.Summary,
            CreatedAt = report.CreatedAt
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
}
