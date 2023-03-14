﻿using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Requests;

namespace KfnApi.Abstractions;

public interface IUserService
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetSelfAsync();
    Task<PaginatedList<User>> GetAllUsersAsync(GetAllUsersRequest request);
    Task<User?> EnrollUserAsync(string id);
    Task UpsertCacheAsync(User user);
}
