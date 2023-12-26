using Microsoft.AspNetCore.Identity;

namespace dkapi;

public class DkUser : IdentityUser
{
    public int LockoutEnabled { get; set; }
    public int EmailConfirmed { get; set; }
}
