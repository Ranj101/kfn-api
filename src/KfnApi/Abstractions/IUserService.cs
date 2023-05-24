using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdentityIdAsync(string id);
    Task<User?> GetByIdAsync(Guid id, bool activeOnly = false);
    Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request);
    Task<User> EnrollUserAsync(FirebaseUser identityUser);
    Task<Result<User>> UpdateUserRoleAsync(Guid id, string role, bool remove = false, bool allowInactiveUser = false);
}
