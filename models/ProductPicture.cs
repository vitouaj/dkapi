using System.ComponentModel.DataAnnotations;

namespace dkapi.Models;

public class ProductPicture
{
    [Key]
    public int ProductId { get; set; }
    public Product Product {get; set;} = null!;
    public string? BinaryData { get; set; }
}
