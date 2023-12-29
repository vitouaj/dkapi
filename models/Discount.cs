using System.ComponentModel.DataAnnotations;

namespace dkapi.Models;

public class Discount
{
    [Key]
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public float Percentage {get; set;}
    
}
