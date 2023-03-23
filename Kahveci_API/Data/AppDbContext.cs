using Kahveci_API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kahveci_API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts{ get; set; }
        public DbSet<CartItem> CartItems{ get; set; }
        public DbSet<OrderHeader> OrderHeaders{ get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<MenuItem>().HasData(
        //        new MenuItem
        //        {
        //            Id = 1,
        //            Name = "Latte",
        //            Description = "Sütlü Kahve",
        //            Image = String.Empty,
        //            Price = 5.77,
        //            Category = "Kahve",
        //            SpecialTag = "Sıcak",
        //        });
        //}
    }
}
