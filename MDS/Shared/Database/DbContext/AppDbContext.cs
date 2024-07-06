using MDS.Model.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MDS.Shared.Database.DbContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<DiscountUser> DiscountsUser { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

        }
    }
}
