using System.ComponentModel.DataAnnotations;

namespace dkapi.Models;

public class OrderDetail
{
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public int Amount {get; set;}
}
