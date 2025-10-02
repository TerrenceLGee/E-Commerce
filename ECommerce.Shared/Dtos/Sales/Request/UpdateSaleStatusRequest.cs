using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Sales.Request;

public class UpdateSaleStatusRequest
{
    public SaleStatus UpdatedStatus { get; set; }
}