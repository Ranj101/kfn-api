﻿using KfnApi.DTOs.Requests;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;

namespace KfnApi.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdentityIdAsync(string id);
    Task<User?> GetByIdAsync(Guid id);
    Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request);
    Task<User?> EnrollUserAsync(string id);
    Task<Result<User>> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request);
}
