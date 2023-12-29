using System.ComponentModel.DataAnnotations;

namespace dkapi.Models;

public class OrderDetail
{
    public int ProductId { get; set; }
    public string OrderId { get; set; } = null!;
    public int Amount { get; set; }
}
