using AutoMapper;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Identity;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAddressRequest, Address>();
        CreateMap<UpdateAddressRequest, Address>();
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>()
            .AfterMap((src, dest) => dest.UpdatedAt = DateTime.UtcNow);
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>()
            .AfterMap((src, dest) => dest.UpdatedAt = DateTime.UtcNow);
        CreateMap<CreateSaleRequest, Sale>();
        CreateMap<SaleItemRequest, SaleProduct>();
        CreateMap<Address, AddressResponse>();
        CreateMap<Category, CategoryResponse>();
        CreateMap<Product, ProductResponse>();
        CreateMap<Sale, SaleResponse>();
        CreateMap<SaleProduct, SaleProductResponse>();
        CreateMap<ApplicationUser, UserResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => GetCorrectAgeForUser(src.DateOfBirth)));
        CreateMap(typeof(PagedList<>), typeof(PagedList<>))
            .ConvertUsing(typeof(PagedListTypeConverter<,>));
    }

    private static int GetCorrectAgeForUser(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}