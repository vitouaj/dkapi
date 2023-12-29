
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

public class Computer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Brandname { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public float Price { get; set; }
    public string GPU { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string Screensize { get; set; } = string.Empty;
}
