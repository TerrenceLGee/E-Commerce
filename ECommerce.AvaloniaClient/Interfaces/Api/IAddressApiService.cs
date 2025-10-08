using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface IAddressApiService
{
    Task<AddressResponse?> AddAddressAsync(CreateAddressRequest request);
    Task<AddressResponse?> UpdateAddressAsync(int addressId, UpdateAddressRequest request);
    Task<AddressResponse?> DeleteAddressAsync(int addressId);
    Task<PagedList<AddressResponse>?> GetAllAddressesAsync(AddressQueryParams queryParams);
    Task<AddressResponse?> GetAddressByIdAsync(int addressId);
}