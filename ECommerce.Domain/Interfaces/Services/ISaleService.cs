using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using FluentResults;

namespace ECommerce.Domain.Interfaces.Services;

public interface ISaleService
{
    Task<Result<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, string customerId);
    Task<Result> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus);
    Task<Result> CancelSaleAsync(int saleId);
    Task<Result> RefundSaleAsync(int saleId);
    Task<Result> UserCancelSaleAsync(string userId, int saleId);
    Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(SaleQueryParams queryParams);
    Task<Result<SaleResponse>> GetSaleByIdAsync(int id);
    Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, SaleQueryParams queryParams);
    Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId);
}