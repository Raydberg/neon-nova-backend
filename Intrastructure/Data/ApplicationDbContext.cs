using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<CartShop> CartShops { get; set; }
        public DbSet<CartShopDetail> CartShopDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Índice para Product.CategoryId.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            // Índice para Order.UserId.
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            // Índice compuesto en ProductComment (ProductId, UserId).
            modelBuilder.Entity<ProductComment>()
                .HasIndex(pc => new { pc.ProductId, pc.UserId })
                .IsUnique() //-> Si solo queremos que el usuario tenga un comentario por producto
                ;

            // Índice para Cart.UserId.
            modelBuilder.Entity<CartShop>()
                .HasIndex(c => c.UserId);

            // Índice para OrderDetail.OrderId.
            // modelBuilder.Entity<OrderDetail>()
            //     .HasIndex(od => od.OrderId);

            // Índice para CartDetail.CartId.
            modelBuilder.Entity<CartShopDetail>()
                .HasIndex(cd => cd.CartId);

            // Índice para Transaction.PaymentDate.
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.PaymentDate);

            // Índice para Order.Status.
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            // Índice para Product.Price.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Price);
        }
    }
}