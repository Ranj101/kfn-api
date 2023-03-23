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
            AbuseReports = user.AbuseReports
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
}
