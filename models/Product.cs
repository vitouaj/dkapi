namespace dkapi.Models;

public class Product
{
    public int Id { get; set; }
    public string Brand {get; set;} = string.Empty;
    public string Model {get; set;} = string.Empty;
    public float Price {get; set;}
    public int View {get; set;}
    public int CategoryId {get; set;}
    public Category Category {get; set;} = null!;
    public int DiscountId {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
    public string? CreatedBy {get; set;}
    public string? UpdatedBy {get; set;}

    public ICollection<Order> Orders {get; set;} = new List<Order>();

    public ICollection<ProductPicture> ProductPictures {get; set;} = new List<ProductPicture>();
}
