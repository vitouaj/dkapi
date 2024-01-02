using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace dkapi.Models;

public class OrderDetail
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int Amount { get; set; }
}
