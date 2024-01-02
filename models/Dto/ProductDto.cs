using System.ComponentModel.DataAnnotations;

namespace dkapi;

public class ProductDto
{
    [Required]
    public string Brand {get; set;} = string.Empty;
    [Required]
    public string Model {get; set;} = string.Empty;
    [Required]
    public float Price {get; set;}
    public int CategoryId {get; set;}
    public int DiscountId {get; set;}
    public string? CreatedBy {get; set;}
}
