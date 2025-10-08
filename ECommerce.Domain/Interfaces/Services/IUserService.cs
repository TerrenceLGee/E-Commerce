using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Domain.Interfaces.Services;

public interface IUserService
{
    Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(UserQueryParams queryParams);
    Task<Result<UserResponse>> GetUserByIdAsync(string userId);

    Task<Result<PagedList<AddressResponse>>>
        GetUserAddressesByIdAsync(string userId, AddressQueryParams queryParams);
}