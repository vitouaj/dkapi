using System.Net;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using dkapi.Data;
using dkapi.Models;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace dkapi;

public class ProductEnpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/products", async (DkdbContext db) =>
        {
            var p = await db.Products.ToListAsync();
            return Results.Ok(p);
        });

        app.MapGet("/product", async (DkdbContext db, string? keyword) =>
        {
            var prodKeyword = $"%{keyword}%";
            var ps = await db.Products
                .Include(e => e.Category)
                .Where(e => (
                    EF.Functions.Like(e.Brand.ToLower(), prodKeyword.ToLower()) ||
                    EF.Functions.Like(e.Model.ToLower(), prodKeyword.ToLower()) ||
                    EF.Functions.Like(e.Category.Name.ToLower(), prodKeyword.ToLower())
                    )
                )
                .Select(e =>
                    new
                    {
                        e.Brand,
                        e.Model,
                        e.Price,
                        e.Category.Name
                    })
                .ToListAsync();
            if (ps.Count == 0)
                return null;
            var jsonOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(ps, jsonOptions);
            return json;
        }).WithName("Search Product");



        app.MapPost("/product", async (DkdbContext db, ProductDto pd) =>
        {
            if (pd == null)
                return Results.BadRequest("record can't be empty");

            var hasProd = await db.Products
                .Where(e =>
                    e.Model.ToLower() == pd.Model.ToLower() &&
                    e.Brand.ToLower() == pd.Brand.ToLower() &&
                    e.Price == pd.Price
                ).AnyAsync();

            if (hasProd)
                return Results.BadRequest("record already exist");

            var newProduct = new Product
            {
                Brand = pd.Brand,
                Model = pd.Model,
                Price = pd.Price,
                CategoryId = pd.CategoryId,
                DiscountId = pd.DiscountId,
                CreatedBy = pd.CreatedBy,
                UpdatedBy = pd.CreatedBy,
            };
            await db.AddAsync(newProduct);
            await db.SaveChangesAsync();
            return Results.Created($"/newProduct/{newProduct.Id}", newProduct);
        });
        // app.MapGet("/get-uploaded-images", async (IAmazonS3 s3cleint) => {
        //     await s3cleint.GetAllObjectKeysAsync();
        // });



    }

}
