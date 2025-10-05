using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface IUserApiService
{
    Task<PagedList<UserResponse>?> GetAllUsersAsync(PaginationParams paginationParams);
    Task<UserResponse?> GetUserByIdAsync(string userId);
    Task<PagedList<AddressResponse>?> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams);
}