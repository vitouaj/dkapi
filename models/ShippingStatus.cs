using System.ComponentModel.DataAnnotations;

namespace dkapi.Models;

public class ShippingStatus
{
    [Key]
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public Order Order {get; set;} = null!;
}
