using System.Linq;
using System.Threading.Tasks;

using MovieDeck.Data.Common.Repositories;
using MovieDeck.Data.Models;

namespace MovieDeck.Services.Data
{
    public class RatingsService : IRatingsService
    {
        private readonly IRepository<Rating> ratingsRepository;
        private readonly IDeletableEntityRepository<Movie> moviesRepository;

        public RatingsService(
            IRepository<Rating> ratingsRepository,
            IDeletableEntityRepository<Movie> moviesRepository)
        {
            this.ratingsRepository = ratingsRepository;
            this.moviesRepository = moviesRepository;
        }

        public async Task SetRatingAsync(int movieId, string userId, byte value)
        {
            var rating = this.ratingsRepository.All()
                .FirstOrDefault(x => x.MovieId == movieId && x.UserId == userId);

            if (rating == null)
            {
                rating = new Rating
                {
                    MovieId = movieId,
                    UserId = userId,
                };

                await this.ratingsRepository.AddAsync(rating);
            }

            rating.Value = value;
            await this.ratingsRepository.SaveChangesAsync();
        }

        public double GetAverageRatings(int movieId)
        {
            var userRatings = this.ratingsRepository.All()
                .Where(x => x.MovieId == movieId)
                .Select(x => x.Value)
                .ToList();

            var movie = this.moviesRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == movieId);

            return ((movie.ImdbRating * movie.RatingsCount)
                + userRatings.Sum(x => x)) / (movie.RatingsCount + userRatings.Count);
        }
    }
}
