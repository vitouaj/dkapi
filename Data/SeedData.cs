using dkapi.Data;
using dkapi.Models;

namespace dkapi;

public class SeedData
{
    private static readonly DkdbContext db;
    
    public static async void SeedCategories()
    {
        // using var db = new DkdbContext();
        var categories = new List<Category>(){
        new Category{Name="gaming"},
        new Category{Name="office"}
        };

        await db.Categories.AddRangeAsync(categories);
        await db.SaveChangesAsync();
    }

    public static async void SeedDiscount()
    {
        var discounts = new List<Discount>(){
            new Discount{Name = "happy new year", Percentage=30},
            new Discount{Name="lunar new year", Percentage=50}
        };

        await db.Discounts.AddRangeAsync(discounts);
        await db.SaveChangesAsync();
    }
}
