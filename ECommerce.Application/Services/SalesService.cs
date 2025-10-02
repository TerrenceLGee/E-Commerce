using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using FluentResults;

namespace ECommerce.Application.Services;

public class SalesService : ISalesService
{
    public async Task<Result<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, string customerId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> CancelSaleAsync(int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RefundSaleAsync(int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UserCancelSaleAsync(string userId, int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SaleResponse>> GetSaleByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId)
    {
        throw new NotImplementedException();
    }
}