using dkapi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace dkapi.Data;

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

        builder.Entity<Category>()
            .HasMany(e => e.Products)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.CategoryId)
            .IsRequired();


        builder.Entity<Product>()
            .HasOne<Discount>()
            .WithOne();

        builder.Entity<ProductPicture>()
            .HasOne(e => e.Product)
            .WithMany(e => e.ProductPictures)
            .HasForeignKey(e => e.ProductId)
            .IsRequired();

        

        builder.Entity<Order>()
            .HasMany(e => e.Products)
            .WithMany(e => e.Orders)
            .UsingEntity<OrderDetail>();

        builder.Entity<ShippingStatus>()
            .HasOne(e => e.Order)
            .WithOne();


        builder.Entity<DkUser>()
            .HasMany(e => e.Orders)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);


    }
}
