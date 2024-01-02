using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dkapi.Models;

public class ShippingStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public ICollection<Order> Orders {get; set;} = new List<Order>();
}
