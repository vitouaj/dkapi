using System.Net;
using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using BrunoZell.ModelBinding;
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

        app.MapGet("/product/{productId}", async (DkdbContext db, int productId, IS3Service s3) => {
    
            var imageIds = await db.ProductPictures.Where(e => e.ProductId == productId).Select(e => e.ImageId).ToListAsync();
            List<string> imageUrls = [];
            foreach (var imageId in imageIds)
            {
                var stream = await s3.GetSingleImage(imageId);
                using var bytearray = new MemoryStream();
                await bytearray.CopyToAsync(stream);
                var base64url = Convert.ToBase64String(bytearray.ToArray());
                imageUrls.Add(base64url);
            }
            var product = await db.Products
            .Select(p => new {
                p.Id,
                p.Brand,
                p.Model,
                p.Category.Name,
                p.Price,
                ProductPictures = imageUrls
            })
            .FirstOrDefaultAsync(p => p.Id == productId);
            return product;
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

        app.MapPost("/product", async (DkdbContext db, string Brand, string Model, float Price, int CategoryId, int DiscountId, string? CreatedBy, string? UpdatedBy, IFormFileCollection files, IS3Service s3) =>
        {
            var hasProd = await db.Products
                .Where(e =>
                    e.Model.ToLower() == Model.ToLower() &&
                    e.Brand.ToLower() == Brand.ToLower() &&
                    e.Price == Price
                ).AnyAsync();

            if (hasProd)
                return Results.BadRequest("record already exist");

            var newProduct = new Product
            {
                Brand = Brand,
                Model = Model,
                Price = Price,
                CategoryId = CategoryId,
                DiscountId = DiscountId,
                CreatedBy = CreatedBy,
                UpdatedBy = CreatedBy,
            };

            foreach (var file in files)
            {
                var ImageKey = await s3.PutSingleImage(file);
                var prodPic = new ProductPicture { ImageId = ImageKey};
                newProduct.ProductPictures.Add(prodPic);
            }

            await db.AddAsync(newProduct);
            await db.SaveChangesAsync();
            return Results.Created();
        }).DisableAntiforgery();






    }

}
