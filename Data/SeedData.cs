using dkapi.Data;
using dkapi.Models;

namespace dkapi;

public class SeedData
{
    public static async void SeedCategories(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<DkdbContext>();
        var categories = new List<Category>()
        {
            new Category{Name="gaming"},
            new Category{Name="office"}
        };

        await db.Categories.AddRangeAsync(categories);
        await db.SaveChangesAsync();
    }

    public static async void SeedDiscount(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<DkdbContext>();
        var discounts = new List<Discount>()
        {
            new Discount{Name = "happy new year", Percentage=30},
            new Discount{Name="lunar new year", Percentage=50}
        };

        await db.Discounts.AddRangeAsync(discounts);
        await db.SaveChangesAsync();
    }

    public static async void SeedShippingStatus(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<DkdbContext>();
        var shippingStatus = new List<ShippingStatus>()
        {
            new ShippingStatus{Status = "ordering"},
            new ShippingStatus{Status="delivered"},
            new ShippingStatus{Status = "cancelled"}
        };

        await db.ShippingStatuses.AddRangeAsync(shippingStatus);
        await db.SaveChangesAsync();
    }

    public static async void SeedProduct(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<DkdbContext>();
        var products = new List<Product>()
        {
            new Product
            {
                Brand = "apple",
                Model = "macbook air m1",
                Price = 770,
                CategoryId = 1,
                DiscountId = 1,
                CreatedBy = "vitou",
                UpdatedBy = "vitou"
            },
            new Product
            {
                Brand = "asus",
                Model = "rog strix g15",
                Price = 1230,
                CategoryId = 2,
                DiscountId = 1,
                CreatedBy = "vitou",
                UpdatedBy = "vitou"
            },
            new Product
            {
                Brand = "msi",
                Model = "gf15",
                Price = 920,
                CategoryId = 2,
                DiscountId = 1,
                CreatedBy = "vitou",
                UpdatedBy = "vitou"
            },
            new Product
            {
                Brand = "apple",
                Model = "macbook pro m1",
                Price = 1200,
                CategoryId = 1,
                DiscountId = 1,
                CreatedBy = "vitou",
                UpdatedBy = "vitou"
            }
        };

        await db.Products.AddRangeAsync(products);
        await db.SaveChangesAsync();
    }
}
