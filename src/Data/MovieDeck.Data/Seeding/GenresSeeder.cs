namespace MovieDeck.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;

    public class GenresSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Genres.Any())
            {
                return;
            }

            await dbContext.Genres.AddRangeAsync(
                new Genre { Name = "Drama" },
                new Genre { Name = "Horror" });
        }
    }
}
