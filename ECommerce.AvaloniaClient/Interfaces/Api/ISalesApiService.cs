using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface ISalesApiService
{
    Task<SaleResponse?> CreateSaleAsync(CreateSaleRequest saleRequest);
    Task<bool> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest);
    Task<bool> RefundSaleAsync(int id);
    Task<bool> CancelSaleAsync(int id);
    Task<bool> UserCancelSaleAsync(int id);
    Task<PagedList<SaleResponse>?> GetAllSalesAsync(SaleQueryParams queryParams);
    Task<PagedList<SaleResponse>?> GetAllSalesForUserAsync(SaleQueryParams queryParams);
    Task<SaleResponse?> GetSaleForUserByIdAsync(int saleId);
    Task<SaleResponse?> GetSaleByIdAsync(int saleId);
}