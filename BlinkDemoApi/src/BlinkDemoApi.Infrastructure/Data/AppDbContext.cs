using BlinkDemoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Data;

/// <summary>
/// EF Core DbContext.
/// This is the single place where you map entities to database tables.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------- Users --------------------
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            b.Property(x => x.Mobile).HasColumnName("mobile").HasMaxLength(15).IsRequired();
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            b.Property(x => x.PasswordHash).HasColumnName("passwordHash").IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasIndex(x => x.Mobile).IsUnique();
            b.HasIndex(x => x.Email).IsUnique();
        });

        // -------------------- Addresses --------------------
        modelBuilder.Entity<UserAddress>(b =>
        {
            b.ToTable("user_addresses");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.UserId).HasColumnName("userId");
            b.Property(x => x.Label).HasColumnName("label").HasMaxLength(50);
            b.Property(x => x.AddressLine).HasColumnName("addressLine").HasMaxLength(500);
            b.Property(x => x.Pincode).HasColumnName("pincode").HasMaxLength(10);
            b.Property(x => x.City).HasColumnName("city").HasMaxLength(100);
            b.Property(x => x.Lat).HasColumnName("lat");
            b.Property(x => x.Lng).HasColumnName("lng");
            b.Property(x => x.IsDefault).HasColumnName("isDefault");
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------- Categories --------------------
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("categories");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            b.Property(x => x.IconUrl).HasColumnName("iconUrl");
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
        });

        // -------------------- SubCategories --------------------
        modelBuilder.Entity<SubCategory>(b =>
        {
            b.ToTable("subcategories");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.CategoryId).HasColumnName("categoryId").IsRequired();
            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.CategoryId, x.Name }).IsUnique();
        });

        // -------------------- Products --------------------
        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasColumnName("description");
            b.Property(x => x.Price).HasColumnName("price");
            b.Property(x => x.Mrp).HasColumnName("mrp");
            b.Property(x => x.ImageUrl).HasColumnName("imageUrl");

            b.Property(x => x.CategoryId).HasColumnName("categoryId").IsRequired();

            // ✅ NEW: subcategoryId
            b.Property(x => x.SubcategoryId).HasColumnName("subcategoryId").IsRequired();

            b.Property(x => x.StockQty).HasColumnName("stockQty");
            b.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Product -> SubCategory
            b.HasOne(x => x.Subcategory)
                .WithMany(sc => sc.Products)
                .HasForeignKey(x => x.SubcategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Helpful indexes
            b.HasIndex(x => x.CategoryId);
            b.HasIndex(x => x.SubcategoryId);
        });

        // -------------------- Cart items --------------------
        modelBuilder.Entity<CartItem>(b =>
        {
            b.ToTable("cart_items");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.UserId).HasColumnName("userId");
            b.Property(x => x.ProductId).HasColumnName("productId");
            b.Property(x => x.Quantity).HasColumnName("quantity");
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
        });

        // -------------------- Orders --------------------
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("orders");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.UserId).HasColumnName("userId");

            // ✅ NEW: addressId column mapping (nullable)

            b.Property(x => x.AddressId).HasColumnName("addressid");

            b.Property(x => x.TotalAmount).HasColumnName("totalAmount");

            b.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ NEW: FK orders.addressId -> user_addresses.id (ON DELETE SET NULL)
            // We do not expose a navigation on Order for UserAddress in Domain,
            // but we can still enforce FK from EF mapping.
            b.HasOne<UserAddress>()
                .WithMany()
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // -------------------- Order items --------------------
        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("order_items");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.OrderId).HasColumnName("orderId");
            b.Property(x => x.ProductId).HasColumnName("productId");
            b.Property(x => x.Quantity).HasColumnName("quantity");
            b.Property(x => x.PriceAtPurchase).HasColumnName("priceAtPurchase");
            b.Property(x => x.Subtotal).HasColumnName("subtotal");
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -------------------- Refresh tokens --------------------
        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("refresh_tokens");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.UserId).HasColumnName("userId");
            b.Property(x => x.TokenHash).HasColumnName("tokenHash").HasMaxLength(256).IsRequired();
            b.Property(x => x.ExpiresAt).HasColumnName("expiresAt");
            b.Property(x => x.RevokedAt).HasColumnName("revokedAt");
            b.Property(x => x.DeviceInfo).HasColumnName("deviceInfo");
            b.Property(x => x.CreatedAt).HasColumnName("createdAt");
            b.Property(x => x.UpdatedAt).HasColumnName("updatedAt");

            b.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}