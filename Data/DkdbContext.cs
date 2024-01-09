using dkapi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using SQLitePCL;


namespace dkapi.Data;

class DkdbContext(DbContextOptions<DkdbContext> options) : IdentityDbContext<DkUser>(options)
{
    public DbSet<Computer> Computers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<ShippingStatus> ShippingStatuses { get; set; }
    public DbSet<ProductPicture> ProductPictures { get; set; }
    public DbSet<DkUser> DkUsers { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

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
            .HasForeignKey(e => e.CategoryId);


        builder.Entity<Product>()
            .HasOne(e => e.Discount)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.DiscountId);


        builder.Entity<Product>()
            .Property(e => e.CreatedDate)
            .HasDefaultValueSql("current_timestamp");
        builder.Entity<Product>()
            .Property(e => e.UpdatedDate)
            .HasDefaultValueSql("current_timestamp");
        builder.Entity<Product>()
            .Property(e => e.View)
            .HasDefaultValue(1);


        builder.Entity<Order>()
           .Property(e => e.CreatedDate)
           .HasDefaultValueSql("current_timestamp");
        builder.Entity<Order>()
            .Property(e => e.UpdatedDate)
            .HasDefaultValueSql("current_timestamp");
        builder.Entity<Order>()
            .Property(e => e.ShippingStatusId)
            .HasDefaultValue(1);


        builder.Entity<ProductPicture>()
            .HasOne(e => e.Product)
            .WithMany(e => e.ProductPictures)
            .HasForeignKey(e => e.ProductId);

        // order detail config
        builder.Entity<Product>()
            .HasMany(e => e.Orders)
            .WithMany(e => e.Products)
            .UsingEntity<OrderDetail>();


        builder.Entity<OrderDetail>()
            .Property(e => e.Amount)
            .HasDefaultValue(1);

        // end order detail config

        builder.Entity<ShippingStatus>()
            .HasMany(e => e.Orders)
            .WithOne(e => e.ShippingStatus)
            .HasForeignKey(e => e.ShippingStatusId);

        builder.Entity<DkUser>()
            .HasMany(e => e.Orders)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
    }
}