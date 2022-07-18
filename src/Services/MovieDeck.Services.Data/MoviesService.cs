namespace MovieDeck.Services.Data
{
    using System;
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
    using MovieDeck.Web.ViewModels.Images;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesService : IMoviesService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly ITmdbService tmdbService;
        private readonly IRatingsService ratingsService;

        public MoviesService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Genre> genresRepository,
            ITmdbService tmdbService,
            IRatingsService ratingsService)
        {
            this.moviesRepository = moviesRepository;
            this.genresRepository = genresRepository;
            this.tmdbService = tmdbService;
            this.ratingsService = ratingsService;
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
                    Id = x.Id,
                    Title = x.Title,
                    Plot = x.Plot,
                    ReleaseDate = x.ReleaseDate,
                    Runtime = x.Runtime,
                    ImdbRating = x.ImdbRating.ToString("F1"),
                    PosterUrl = this.tmdbService.GenereateImageUrl(x.PosterPath),
                    BackdropUrl = this.tmdbService.GenereateImageUrl(x.BackdropPath),
                }).ToList();
        }

        public async Task<SingleMovieViewModel> GetMovieByIdAsync(int id)
        {
            return await this.moviesRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SingleMovieViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Plot = x.Plot,
                    ReleaseDate = x.ReleaseDate,
                    Runtime = x.Runtime,
                    PosterUrl = this.tmdbService.GenereateImageUrl(x.PosterPath),
                    AverageRating = this.ratingsService.GetAverageRatings(x.Id),
                    RatingsCount = this.ratingsService.GetRatingsCount(x.Id),
                    Genres = x.Genres.Select(g => new GenreViewModel
                    {
                        Name = g.Genre.Name,
                    }),
                    Directors = x.Directors.Select(d => new DirectorViewModel
                    {
                        Name = d.Director.FullName,
                        PhotoUrl = this.tmdbService.GenereateImageUrl(d.Director.PhotoPath),
                        PhotoPath = d.Director.PhotoPath,
                    }),
                    Actors = x.Actors.Select(a => new ActorViewModel
                    {
                        FullName = a.Actor.FullName,
                        CharacterName = a.CharacterName,
                        PhotoUrl = this.tmdbService.GenereateImageUrl(a.Actor.PhotoPath),
                        PhotoPath = a.Actor.PhotoPath,
                    }),
                    Images = x.Images.Select(i => new ImageViewModel
                    {
                        PhotoUrl = this.tmdbService.GenereateImageUrl(i.OriginalPath),
                    }),
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync()
        {
            var originalIds = await this.tmdbService.GetPopularMoviesOriginalIdAsync();

            await this.ImportMoviesIfNotExistAsync(originalIds);

            var popularMovies = new List<MovieViewModel>();
            foreach (var originalId in originalIds)
            {
                if (!this.moviesRepository.AllAsNoTracking().Any(x => x.OriginalId == originalId))
                {
                    continue;
                }

                var movie = await this.moviesRepository.AllAsNoTracking()
                    .Where(x => x.OriginalId == originalId)
                    .Select(x => new MovieViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Plot = x.Plot,
                        ReleaseDate = x.ReleaseDate,
                        Runtime = x.Runtime,
                        ImdbRating = x.ImdbRating.ToString("F1"),
                        PosterUrl = this.tmdbService.GenereateImageUrl(x.PosterPath),
                        BackdropUrl = this.tmdbService.GenereateImageUrl(x.BackdropPath),
                    }).FirstOrDefaultAsync();

                popularMovies.Add(movie);
            }

            return popularMovies;
        }

        private async Task ImportMoviesIfNotExistAsync(IEnumerable<int> originalIds)
        {
            foreach (var originalId in originalIds)
            {
                if (this.moviesRepository.AllAsNoTracking().Any(x => x.OriginalId == originalId))
                {
                    continue;
                }

                var movieDto = await this.tmdbService.GetMovieById(originalId);
                if (movieDto == null)
                {
                    continue;
                }

                await this.tmdbService.ImportMovieAsync(movieDto);
            }
        }
    }
}
