using AutoMapper;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Application.Mappings;

public class PagedListTypeConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>> where TDestination : class
{
    public PagedList<TDestination> Convert(
        PagedList<TSource> source,
        PagedList<TDestination> destination,
        ResolutionContext context)
    {
        var destinationItems = context.Mapper.Map<List<TDestination>>(source.Items);

        return new PagedList<TDestination>(
            destinationItems,
            source.TotalCount,
            source.CurrentPage,
            source.PageSize);
    }
}