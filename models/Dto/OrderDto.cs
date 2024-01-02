using System.ComponentModel.DataAnnotations;

namespace dkapi.Models.Dto;

public class OrderDto
{
    public string UserId { get; set; } = string.Empty;
    public List<OrderRequest> ProductOrderList { get; set; } = [];
}

public class OrderRequest
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int Amount { get; set; }
}
