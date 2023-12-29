using System.ComponentModel.DataAnnotations;
using dkapi.Models;

namespace dkapi;

public class Order
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public DkUser User { get; set; } = null!;
    public int ShippingStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
