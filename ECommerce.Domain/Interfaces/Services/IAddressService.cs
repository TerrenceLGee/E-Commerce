using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Domain.Interfaces.Services;

public interface IAddressService
{
    Task<Result<AddressResponse>> AddAddressAsync(string customerId, CreateAddressRequest request);
    Task<Result<AddressResponse>> UpdateAddressAsync(string customerId, int addressId, UpdateAddressRequest request);
    Task<Result<AddressResponse>> DeleteAddressAsync(string customerId, int addressId);
    Task<Result<PagedList<AddressResponse>>> GetAllAddressesAsync(string customerId, AddressQueryParams queryParams);
    Task<Result<AddressResponse>> GetAddressByIdAsync(string customerId, int addressId);
}