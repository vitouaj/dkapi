using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dkapi;

class DkdbContext(DbContextOptions<DkdbContext> options) : IdentityDbContext<DkUser>(options)
{
    public DbSet<Computer> Computers { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Computer>()
            .HasKey(c => c.Id);

        builder.Entity<Computer>()
            .Property(c => c.Id)
            .HasColumnType("uuid");

    }
}
