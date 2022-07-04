namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesService : IMoviesService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;

        public MoviesService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Genre> genresRepository)
        {
            this.moviesRepository = moviesRepository;
            this.genresRepository = genresRepository;
        }

        public async Task AddAsync(AddMovieInputModel input)
        {
            var movie = new Movie
            {
                Title = input.Title,
                Plot = input.Plot,
                ReleaseDate = input.ReleaseDate,
                Runtime = input.Runtime,
            };

            foreach (var genreId in input.GenresIds)
            {
                var genre = this.genresRepository.All().FirstOrDefault(x => x.Id == genreId);
                if (genre == null)
                {
                    continue;
                }

                movie.Genres.Add(new MovieGenre
                {
                    Genre = genre,
                });
            }

            await this.moviesRepository.AddAsync(movie);
            await this.moviesRepository.SaveChangesAsync();
        }

        public IEnumerable<Movie> GetAll()
        {
            return this.moviesRepository.AllAsNoTracking().ToList();
        }
    }
}
