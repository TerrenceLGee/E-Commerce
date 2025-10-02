using ECommerce.Application.Validators.AddressValidators;
using ECommerce.Application.Validators.AuthValidators;
using ECommerce.Application.Validators.CategoryValidators;
using ECommerce.Application.Validators.ProductValidators;
using ECommerce.Application.Validators.SaleValidators;
using FluentValidation;
using AutoMapper;
using ECommerce.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAddressRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateAddressRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSaleRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SaleItemRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSaleStatusRequestValidator>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();