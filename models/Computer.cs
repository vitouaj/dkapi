
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

public class Computer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string brandName { get; set; } = string.Empty;
    public string model { get; set; } = string.Empty;
    public float price { get; set; }
    public string GPU { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string screenSize { get; set; } = string.Empty;
}
