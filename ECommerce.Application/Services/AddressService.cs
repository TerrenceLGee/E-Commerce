using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Interfaces.Services;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Extensions;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<AddressService> _logger;
    private readonly IMapper _mapper;

    public AddressService(
        IAddressRepository addressRepository,
        ILogger<AddressService> logger,
        IMapper mapper)
    {
        _addressRepository = addressRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<Result<AddressResponse>> AddAddressAsync(string customerId, CreateAddressRequest request)
    {
        try
        {
            var address = _mapper.Map<Address>(request);
            address.CustomerId = customerId;
            await _addressRepository.AddAsync(address);
            return Result.Ok(_mapper.Map<AddressResponse>(address));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error adding the address: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error adding the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> UpdateAddressAsync(string customerId, int addressId, UpdateAddressRequest request)
    {
        try
        {
            var addressToUpdate = await _addressRepository.GetByIdAsync(customerId, addressId);

            if (addressToUpdate is null)
            {
                _logger.LogError("Address with Id {id} not found", addressId);
                return Result.Fail($"Address with Id {addressId} not found");
            }

            _mapper.Map(request, addressToUpdate);
            await _addressRepository.UpdateAsync(addressToUpdate);
            var updatedAddress = await _addressRepository.GetByIdAsync(customerId, addressToUpdate.Id);

            return Result.Ok(_mapper.Map<AddressResponse>(updatedAddress));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error updating the address: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error updating the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> DeleteAddressAsync(string customerId, int addressId)
    {
        try
        {
            var addressToDelete = await _addressRepository.GetByIdAsync(customerId, addressId);

            if (addressToDelete is null)
            {
                _logger.LogError("Address with Id {id} not found", addressId);
                return Result.Fail($"Address with Id {addressId} not found");
            }

            await _addressRepository.DeleteAsync(addressToDelete);
            return Result.Ok(_mapper.Map<AddressResponse>(addressToDelete));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error deleting the address: {errorMessage}", ex.Message);
            return Result.Fail($"There was an deleting the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<AddressResponse>>> GetAllAddressesAsync(string customerId, AddressQueryParams queryParams)
    {
        try
        {
            var query = _addressRepository.GetAllQueryable(customerId);

            if (!string.IsNullOrEmpty(queryParams.StreetNumber))
            {
                query = query.Where(a => a.StreetNumber == queryParams.StreetNumber);
            }

            if (!string.IsNullOrEmpty(queryParams.StreetName))
            {
                query = query.Where(a => a.StreetName.ToLower().Contains(queryParams.StreetName.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParams.City))
            {
                query = query.Where(a => a.City.ToLower().Contains(queryParams.City.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParams.State))
            {
                query = query.Where(a => a.State.ToLower().Contains(queryParams.State.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParams.ZipCode))
            {
                query = query.Where(a => a.ZipCode == queryParams.ZipCode);
            }

            if (!string.IsNullOrEmpty(queryParams.Country))
            {
                query = query.Where(a => a.Country.ToLower().Contains(queryParams.Country.ToLower()));
            }

            if (queryParams.AddressType.HasValue)
            {
                query = query.Where(a => a.AddressType == queryParams.AddressType.Value);
            }

            query = queryParams.OrderBy switch
            {
                OrderByOptions.CustomerIdDesc => query.OrderByDescending(a => a.CustomerId),
                _ => query.OrderBy(a => a.CustomerId)
            };

            var projectedQuery = query.ProjectTo<AddressResponse>(_mapper.ConfigurationProvider);

            var pagedResponse = await projectedQuery.ToPagedListAsync(
                queryParams.PageNumber,
                queryParams.PageSize);

            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving all addresses: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error retrieving all addresses: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> GetAddressByIdAsync(string customerId, int addressId)
    {
        try
        {
            var address = await _addressRepository
                .GetByIdAsync(customerId, addressId);

            if (address is null)
            {
                _logger.LogError("Address with Id {id} not found", addressId);
                return Result.Fail($"Address with Id {addressId} not found");
            }

            return Result.Ok(_mapper.Map<AddressResponse>(address));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving the address with Id {id}: {errorMessage}", addressId,
                ex.Message);
            return Result.Fail($"There was an error retrieving the address with Id {addressId}: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                "An unexpected error occurred while retrieving the address with Id {id}: {errorMessage}", addressId,
                ex.Message);
            return Result.Fail($"An unexpected error occurred while retrieving the address with Id {addressId}: {ex.Message}");
        }
    }
}