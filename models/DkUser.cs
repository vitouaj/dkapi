using Microsoft.AspNetCore.Identity;

namespace dkapi.Models;

public class DkUser : IdentityUser
{
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public int LockoutEnabled { get; set; }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public int EmailConfirmed { get; set; }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
}
