namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.TmdbApi;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Directors;
    using MovieDeck.Web.ViewModels.Genres;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesService : IMoviesService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly ITmdbService tmdbService;

        public MoviesService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Genre> genresRepository,
            ITmdbService tmdbService)
        {
            this.moviesRepository = moviesRepository;
            this.genresRepository = genresRepository;
            this.tmdbService = tmdbService;
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

        public IEnumerable<MovieViewModel> GetAllForHomePage()
        {
            return this.moviesRepository.AllAsNoTracking()
                .Select(x => new MovieViewModel
                {
                    Title = x.Title,
                    Plot = x.Plot,
                    ReleaseDate = x.ReleaseDate,
                    Runtime = x.Runtime,
                    ImdbRating = x.ImdbRating,
                    OriginalUrl = x.OriginalUrl,
                    PosterUrl = x.PosterUrl,
                    BannerUrl = x.Images.FirstOrDefault().OriginalUrl,
                }).ToList();
        }

        public async Task<SingleMovieViewModel> GetMovieByIdAsync(int id)
        {
            return await this.moviesRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SingleMovieViewModel
                {
                    Title = x.Title,
                    Plot = x.Plot,
                    ReleaseDate = x.ReleaseDate,
                    Runtime = x.Runtime,
                    OriginalUrl = x.OriginalUrl,
                    PosterUrl = x.PosterUrl,
                    ImdbRating = x.ImdbRating,
                    Genres = x.Genres.Select(g => new GenreViewModel
                    {
                        Name = g.Genre.Name,
                    }),
                    Directors = x.Directors.Select(d => new DirectorViewModel
                    {
                        Name = d.Director.FullName,
                        PhotoUrl = d.Director.PhotoUrl,
                    }),
                    Actors = x.Actors.Select(a => new ActorViewModel
                    {
                        FullName = a.Actor.FullName,
                        CharacterName = a.CharacterName,
                        PhotoUrl = a.Actor.PhotoUrl,
                    }),
                    Images = x.Images,
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync()
        {
            var movies = await this.tmdbService.GetPopularMoviesAsync();

            return movies.Select(x => new MovieViewModel
            {
                Title = x.Title,
                ImdbRating = x.ImdbRating,
                PosterUrl = x.PosterUrl,
            });
        }
    }
}
