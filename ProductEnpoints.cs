using dkapi.Data;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

public class ProductEnpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/product", async (DkdbContext db) =>
        {
            return await db.Products.ToListAsync();
        });

        // app.MapPost("/product", async (DkdbContext db) => {
        //     return await db.;
        // });
}

}
