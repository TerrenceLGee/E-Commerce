using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Application.Services;

public class AddressService : IAddressService
{
    public async Task<Result<AddressResponse>> AddAddressAsync(string customerId, CreateAddressRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<AddressResponse>> UpdateAddressAsync(string customerId, int addressId, UpdateAddressRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<AddressResponse>> DeleteAddressAsync(string customerId, int addressId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<AddressResponse>>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<AddressResponse>> GetAddressByIdAsync(string customerId, int addressId)
    {
        throw new NotImplementedException();
    }
}