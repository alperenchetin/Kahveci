using Kahveci_API.Data;
using Kahveci_API.Entities;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace Kahveci_API
{
    public static class Seed
    {
        public static async Task SeedData(AppDbContext db)
        {
            if (db.MenuItems.Any()) return;
            var menuItemList = new List<MenuItem>{
                new MenuItem
                {
                    //Id = 1,
                    Name = "Latte",
                    Description = "Latte Kahve",
                    Image = "https://kahveciimages.blob.core.windows.net/kahveci/Latte.jpg",
                    Price = 5.77,
                    Category = "Kahve",
                    SpecialTag = "Sıcak",
                },
                new MenuItem
                {
                    //Id = 2,
                    Name = "Filtre Kahve",
                    Description = "Filtre Kahve",
                    Image = "https://kahveciimages.blob.core.windows.net/kahveci/filtrekahve.jpeg",
                    Price = 7.77,
                    Category = "Kahve",
                    SpecialTag = "Sıcak",
                }
            };
            await db.AddRangeAsync(menuItemList);
            await db.SaveChangesAsync();
        }
    }
}
