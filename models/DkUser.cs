#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace dkapi.Models;

public class DkUser : IdentityUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public int LockoutEnabled { get; set; }
    public int EmailConfirmed { get; set; }
    public string? ProfileKey { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
