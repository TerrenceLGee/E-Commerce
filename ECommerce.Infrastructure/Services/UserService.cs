using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Interfaces.Services;
using ECommerce.Infrastructure.Extensions;
using ECommerce.Infrastructure.Identity;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IAddressRepository addressRepository,
        ILogger<UserService> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _addressRepository = addressRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _userManager.Users.AsQueryable();

            query = paginationParams.OrderBy switch
            {
                OrderByOptions.BirthDateAsc => query.OrderBy(u => u.DateOfBirth),
                OrderByOptions.BirthDateDesc => query.OrderByDescending(u => u.DateOfBirth),
                OrderByOptions.FirstNameAsc => query.OrderBy(u => u.FirstName),
                OrderByOptions.FirstNameDesc => query.OrderByDescending(u => u.FirstName),
                OrderByOptions.LastNameAsc => query.OrderBy(u => u.LastName),
                OrderByOptions.LastNameDesc => query.OrderByDescending(u => u.LastName),
                OrderByOptions.UserIdDesc => query.OrderByDescending(u => u.Id),
                _ => query.OrderBy(u => u.Id)
            };

            var projectedQuery = query.ProjectTo<UserResponse>(_mapper.ConfigurationProvider);

            var pagedUsers =
                await projectedQuery.ToPagedListAsync(paginationParams.PageNumber, paginationParams.PageSize);

            return Result.Ok(pagedUsers);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("Unable to retrieve users from the database: {errorMessage}", ex.Message);
            return Result.Fail("Unable to retrieve users from the database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred retrieving users from the database: {errorMessage}", ex.Message);
            return Result.Fail($"An unexpected error occurred retrieving users from the database: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                _logger.LogError("User with Id {userId} not found", userId);
                return Result.Fail("User with Id {userId} not found");
            }

            var userResponse = _mapper.Map<UserResponse>(user);

            return Result.Ok(userResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("Unable to retrieve user with Id {id} from the database: {errorMessage}", userId, ex.Message);
            return Result.Fail($"Unable to retrieve user with Id {userId} from the database");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "There was an unexpected error retrieving user with Id {userId} from the database: {errorMessage}",
                userId, ex.Message);
            return Result.Fail(
                $"There was an unexpected error retrieving user with Id {userId} from the database: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<AddressResponse>>> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            var userExists = await _userManager.Users
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
            {
                _logger.LogError("User with Id {userId} not found.", userId);
                return Result.Fail($"User with Id {userId} not found");
            }

            var query = _addressRepository.GetAllQueryable(userId);

            query = paginationParams.OrderBy switch
            {
                OrderByOptions.CustomerIdDesc => query.OrderBy(a => a.CustomerId),
                _ => query.OrderBy(a => a.CustomerId)
            };

            var projectedQuery = query.ProjectTo<AddressResponse>(_mapper.ConfigurationProvider);

            var pagedResponse =
                await projectedQuery.ToPagedListAsync(paginationParams.PageNumber, paginationParams.PageSize);

            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("Unable to retrieve addresses from the database: {errorMessage}", ex.Message);
            return Result.Fail("Unable to retrieve addresses from the database");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "There was an unexpected error retrieving the addresses of user with id {userId} from the database: {errorMessage}",
                userId, ex.Message);
            return Result.Fail(
                $"There was an unexpected error retrieving the addresses os user with Id {userId} from the database: {ex.Message}");
        }
    }
}