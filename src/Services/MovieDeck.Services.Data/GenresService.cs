namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;

    public class GenresService : IGenresService
    {
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly IRepository<MovieGenre> movieGenreRepository;

        public GenresService(
            IDeletableEntityRepository<Genre> genresRepository,
            IRepository<MovieGenre> movieGenreRepository)
        {
            this.genresRepository = genresRepository;
            this.movieGenreRepository = movieGenreRepository;
        }

        public Genre GetById(int id)
        {
            return this.genresRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.genresRepository.AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                })
                .OrderBy(x => x.Name)
                .ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name));
        }

        public async Task RemoveAllMovieGenresForMovieAsync(int id)
        {
            var movieGenres = this.movieGenreRepository.All().Where(x => x.MovieId == id);
            foreach (var movieGenre in movieGenres)
            {
                this.movieGenreRepository.Delete(movieGenre);
            }

            await this.movieGenreRepository.SaveChangesAsync();
        }
    }
}
