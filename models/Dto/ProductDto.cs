namespace dkapi;

public class ProductDto
{
    public int Id { get; set; }
    public string Brand {get; set;} = string.Empty;
    public string Model {get; set;} = string.Empty;
    public float Price {get; set;}
    public int View {get; set;}
    public int CategoryId {get; set;}
    public int DiscountId {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
    public string? CreatedBy {get; set;}
    public string? UpdatedBy {get; set;}
}
