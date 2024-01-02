using dkapi.Data;
using dkapi.Models;
using dkapi.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql.Replication;

namespace dkapi;

public class OrderEnpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/order", async (DkdbContext db, OrderDto orderRequest, UserManager<DkUser> userManager) =>
        {
            if (orderRequest == null || string.IsNullOrEmpty(orderRequest.UserId))
                return Results.BadRequest();

            var user = await userManager.FindByIdAsync(orderRequest.UserId);
            if (user == null)
                return Results.BadRequest();

            var newOrder = new Order
            {
                UserId = user?.Id,
                CreatedBy = user?.UserName,
                UpdatedBy = user?.UserName,
            };


            var _OrderDetails = orderRequest.ProductOrderList
                .Select(e => new OrderDetail
                {
                    Order = newOrder,
                    OrderId = newOrder.Id,
                    ProductId = e.ProductId,
                    Amount = e.Amount,
                }).ToList();

            newOrder.OrderDetails = _OrderDetails;

            await db.Orders.AddAsync(newOrder);
            await db.SaveChangesAsync();

            return Results.Created();
        });

        app.MapGet("/order/detail", async (DkdbContext db, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
            {
                return "user id not found";
            }

            // var result = db.Orders
            //     .Include(e => e.OrderDetails)
            //     .Where(e => e.UserId == userId)
            //     .Select(e => new
            //     {
            //         e.UserId,
            //         e.Products
            //             .Select(e => new
            //             {
            //                 e.Price
            //             })
            //     })
            //     .ToList();

            var result = await db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    UserId = o.UserId,
                    Products = o.OrderDetails.Select(od => new
                    {
                        od.Product.Brand,
                        od.Product.Model,
                        od.Product.Price,
                        Detial = o.OrderDetails
                            .Where(e => e.OrderId == od.OrderId)
                            .Select(e => new
                            {
                                e.OrderId,
                                e.Amount
                            })
                            .FirstOrDefault()
                    }
                    ).ToList(),
                })
                .ToListAsync();

            // var result = db.Orders
            //     .Where(e => e.UserId == userId)
            //     .Include(e => e.OrderDetails)
            //     .Select(e => new {
            //         e.UserId,
            //         e.OrderDetails
            //     }).ToList();


            var jsonOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(result, jsonOptions);


            return json;
        });


        static async Task<List<Product>> GetProductsByIds(List<int> ids, DkdbContext db)
        {
            List<Product> result = [];
            foreach (var id in ids)
            {
                var prod = await db.Products.Where(e => e.Id == id).FirstOrDefaultAsync();
                if (prod != null)
                    result.Add(prod);
            }
            return result;
        }
    }
}
