using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Identity;
using ECommerce.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public class DataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Customer" });
        }
    }

    public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        var adminEmail = "admin@example.com";
        var adminPassword = "Password123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                if (await roleManager.FindByNameAsync("Admin") is not null)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new()
            {
                Name = "Electronics", 
                Description = "Gadgets and devices."
            },
            new()
            {
                Name = "Books", 
                Description = "Paperback and hardcover books."
            },
            new()
            {
                Name = "Apparel", 
                Description = "Clothing and accessories"
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new()
            {
                Name = "Laptop Pro",
                Description = "High performance laptop, comes with 32GB memory and 2 TB SSD Drive",
                Price = 1200.99m,
                Discount = DiscountStatus.TenPercent,
                StockQuantity = 75,
                CategoryId = categories[0].Id,
                StockKeepingUnit = "ELEC-LP-PRO",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            },
            new()
            {
                Name = "Wireless Mouse",
                Description = "State of the art bluetooth wireless mouse",
                Price = 18.99m,
                Discount = DiscountStatus.FivePercent,
                StockQuantity = 200,
                CategoryId = categories[0].Id,
                StockKeepingUnit = "ELEC-MS-WL",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "The C# Players Guide",
                Description = "An engaging gamified introduction to programming in C#",
                Price = 21.99m,
                Discount = DiscountStatus.None,
                StockQuantity = 150,
                CategoryId = categories[1].Id,
                StockKeepingUnit = "BOOK-CS-JRN",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Web API Development with ASP.NET Core 8",
                Description = "Guide to building maintainable, scalable Wen APIS using ASP.NET Core 8",
                Price = 69.99m,
                Discount = DiscountStatus.FifteenPercent,
                StockQuantity = 100,
                CategoryId = categories[1].Id,
                StockKeepingUnit = "BOOK-API-DSG",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Star Wars Darth Vader T-Shirt",
                Description = "100% cotton, machine washable t-shirt",
                Price = 24.99m,
                Discount = DiscountStatus.FivePercent,
                StockQuantity = 300,
                CategoryId = categories[2].Id,
                StockKeepingUnit = "APP-TS-DVR"
            },
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}