using dkapi.Data;
using dkapi.Models;
using dkapi.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            CreateOrderResponse json;
            if (orderRequest == null || string.IsNullOrEmpty(orderRequest.UserId))
            {
                json = new CreateOrderResponse
                {
                    Success = false,
                    Error = true,
                    Message = $"Order created failed ;-;",
                };
                return JsonParser.Parse(json);
            }
            var user = await userManager.FindByIdAsync(orderRequest.UserId);
            if (user == null)
            {
                json = new CreateOrderResponse
                {
                    Success = false,
                    Error = true,
                    Message = $"Order created failed. can't find user id;-;",
                };
                return JsonParser.Parse(json);
            }

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
                    Product = db.Products.FirstOrDefault(i => i.Id == e.ProductId),
                    Amount = e.Amount,
                }).ToList();

            newOrder.OrderDetails = _OrderDetails;

            await db.Orders.AddAsync(newOrder);
            await db.SaveChangesAsync();

            var orderResponse = new OrderResponse
            {
                UserId = newOrder.UserId,
                OrderId = newOrder.Id,
                OrderDate = newOrder.CreatedDate,
            };

            var _orderDetailResponses = newOrder.OrderDetails
                .Select(e => new OrderDetailResponse
                {
                    ProductName = e.Product?.Brand,
                    UnitPrice = e.Product?.Price ?? 0, // Assuming a default value if Price is null
                    Amount = e.Amount,
                    TotalOfUnitPrice = (e.Product?.Price ?? 0) * e.Amount,
                }).ToList();

            orderResponse.OrderDetailResponses = _orderDetailResponses;
            double total = 0;
            foreach (var o in _orderDetailResponses)
            {
                total += o.TotalOfUnitPrice;
            }
            orderResponse.TotalCost = total;

            var cor = new CreateOrderResponse
            {
                Success = true,
                Error = false,
                Message = $"Order created successfully: {newOrder.Id}",
                Payload = orderResponse
            };

            return JsonParser.Parse(cor);
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
                    o.UserId,
                    OrderId = o.Id,
                    o.CreatedDate,
                    Products = o.OrderDetails.Select(od => new
                    {
                        od.Product.Brand,
                        od.Product.Model,
                        od.Product.Price,
                        Amount = o.OrderDetails
                            .Where(e => e.OrderId == od.OrderId)
                            .Select(e => e.Amount)
                            .FirstOrDefault()
                    }
                    ).ToList(),
                })
                .ToListAsync();

            var jsonOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(result, jsonOptions);


            return json;
        });
    }
}
