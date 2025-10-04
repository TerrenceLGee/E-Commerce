using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Domain.Models;
using ECommerce.Shared.Enums;
using AutoMapper;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SaleService> _logger;
    private readonly IMapper _mapper;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        ILogger<SaleService> logger,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<Result<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, string customerId)
    {
        try
        {
            var productIds = request.Items
                .Select(i => i.ProductId)
                .Distinct();
            var products = await _productRepository.GetByIdsAsync(productIds);

            foreach (var item in request.Items)
            {
                var product = products
                    .FirstOrDefault(p => p.Id == item.ProductId);

                if (product is null)
                {
                    _logger.LogError("Product with Id {id} not found", item.ProductId);
                    return Result.Fail($"Product with Id {item.ProductId} not found");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    _logger.LogError("Insufficient stock for '{name}'. Available: {stockQuantity}, Request: {Quantity}",
                        product.Name, product.StockQuantity, item.Quantity);

                    return Result.Fail(
                        $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Request: {item.Quantity}");
                }
            }

            var sale = _mapper.Map<Sale>(request);
            sale.CustomerId = customerId;
            var totalPrice = 0.0m;

            foreach (var item in request.Items)
            {
                var product = products
                    .First(p => p.Id == item.ProductId);

                var saleProduct = new SaleProduct
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    DiscountPrice = product.Price - ((((int)product.Discount) / 100.0m) * product.Price),
                    Product = product
                };

                sale.SaleItems.Add(saleProduct);
                product.StockQuantity -= item.Quantity;
                totalPrice += saleProduct.FinalPrice;
            }

            sale.TotalPrice = totalPrice;

            await _saleRepository.AddAsync(sale);

            var saleResponse = _mapper.Map<SaleResponse>(sale);

            return Result.Ok(saleResponse);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error creating the Sale: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error creating the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error that occurred: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus)
    {
        try
        {
            var saleToUpdate = await _saleRepository.GetByIdAsync(saleId);

            if (saleToUpdate is null)
            {
                _logger.LogError("Sale with Id {id} not found", saleId);
                return Result.Fail($"Sale with Id {saleId} not found");
            }

            saleToUpdate.Status = updatedStatus;
            saleToUpdate.UpdatedAt = DateTime.UtcNow;
            await _saleRepository.UpdateAsync(saleToUpdate);

            return Result.Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error updating the Sale status: {errorMessage}", ex);
            return Result.Fail($"There was an error updating the Sale status: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result> CancelSaleAsync(int saleId)
    {
        try
        {
            var saleToCancel = await _saleRepository.GetByIdAsync(saleId);

            if (saleToCancel is null)
            {
                _logger.LogError("Sale with Id {id} not found", saleId);
                return Result.Fail($"Sale with Id {saleId} not found");
            }

            if (saleToCancel.Status != SaleStatus.Pending && saleToCancel.Status != SaleStatus.Pending)
            {
                _logger.LogError("Enable to cancel a Sale with status: {status}", saleToCancel.Status);
                return Result.Fail($"Unable to cancel a Sale with status: {saleToCancel.Status}");
            }

            foreach (var item in saleToCancel.SaleItems)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            saleToCancel.Status = SaleStatus.Canceled;
            saleToCancel.UpdatedAt = DateTime.UtcNow;
            await _saleRepository.UpdateAsync(saleToCancel);
            return Result.Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error canceling the Sale: {errorMessage}", ex);
            return Result.Fail($"There was an error canceling the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result> RefundSaleAsync(int saleId)
    {
        try
        {
            var saleToRefund = await _saleRepository.GetByIdAsync(saleId);

            if (saleToRefund is null)
            {
                _logger.LogError("Sale with Id {id} not found", saleId);
                return Result.Fail($"Sale with Id {saleId} not found");
            }

            if (saleToRefund.Status != SaleStatus.Completed)
            {
                _logger.LogError("Unable to refund a Sale with status: {saleToRefund}", saleToRefund.Status);
                return Result.Fail($"Unable to refund a Sale with status: {saleToRefund.Status}");
            }

            saleToRefund.Status = SaleStatus.Refunded;
            saleToRefund.UpdatedAt = DateTime.UtcNow;
            await _saleRepository.UpdateAsync(saleToRefund);

            return Result.Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error refunding the Sale: {errorMessage}", ex);
            return Result.Fail($"There was an error refunding the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result> UserCancelSaleAsync(string userId, int saleId)
    {
        try
        {
            var userSaleToCancel = await _saleRepository.GetUserSaleByIdAsync(userId, saleId);

            if (userSaleToCancel is null)
            {
                _logger.LogError("Sale with Id {id} not found", saleId);
                return Result.Fail($"Sale with Id {saleId} not found");
            }

            if (userSaleToCancel.Status != SaleStatus.Pending && userSaleToCancel.Status != SaleStatus.Processing)
            {
                _logger.LogError("Unable to cancel a Sale with status: {status}", userSaleToCancel.Status);
                return Result.Fail($"Unable to cancel a sale with status: {userSaleToCancel.Status}");
            }

            foreach (var item in userSaleToCancel.SaleItems)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            userSaleToCancel.Status = SaleStatus.Canceled;
            userSaleToCancel.UpdatedAt = DateTime.UtcNow;
            await _saleRepository.UpdateAsync(userSaleToCancel);
            return Result.Ok();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error canceling the Sale: {errorMessage}", ex);
            return Result.Fail($"There was an error canceling the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(PaginationParams paginationParams)
    {
        try
        {
            var pagedSales = await _saleRepository.GetAllAsync(paginationParams);

            var pagedResponse = _mapper.Map<PagedList<SaleResponse>>(pagedSales);

            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sales: {errorMessage}", ex);
            return Result.Fail($"There was an error retrieving sales: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse>> GetSaleByIdAsync(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id);

            if (sale is null)
            {
                _logger.LogError("Sale with Id {id} not found", id);
                return Result.Fail($"Sale with Id {id} not found");
            }

            return Result.Ok(_mapper.Map<SaleResponse>(sale));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sale: {errorMessage}", ex);
            return Result.Fail($"There was an error retrieving sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            var pagedSales = await _saleRepository.GetAllByUserIdAsync(userId, paginationParams);

            var pagedResponse = _mapper.Map<PagedList<SaleResponse>>(pagedSales);

            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sales: {errorMessage}", ex);
            return Result.Fail($"There was an error retrieving sales: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId)
    {
        try
        {
            var sale = await _saleRepository.GetUserSaleByIdAsync(userId, saleId);

            if (sale is null)
            {
                _logger.LogError("Sale with with Id {id} not found", saleId);
                return Result.Fail($"Sale with Id {saleId} not found");
            }

            return Result.Ok(_mapper.Map<SaleResponse>(sale));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sale: {errorMessage}", ex);
            return Result.Fail($"There was an error retrieving sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
        
    }
}