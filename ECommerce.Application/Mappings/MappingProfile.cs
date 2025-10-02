using AutoMapper;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Identity;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Sales.Response;

namespace ECommerce.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
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