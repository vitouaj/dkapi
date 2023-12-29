#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace dkapi.Models;

public class DkUser : IdentityUser
{
    [Key]
    public int LockoutEnabled { get; set; }
    public int EmailConfirmed { get; set; }
    public ICollection<Order> Orders {get; set; } = new List<Order>();
}





#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
