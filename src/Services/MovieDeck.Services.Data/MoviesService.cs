namespace MovieDeck.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly string[] allowedExtensions = new[] { "jpg", "jpeg", "png", "gif" };

        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IActorsService actorsService;
        private readonly IGenresService genresService;
        private readonly IDirectorsService directorsService;
        private readonly ICompaniesService companiesService;
        private readonly ITmdbService tmdbService;
        private readonly IRatingsService ratingsService;

        public MoviesService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IActorsService actorsService,
            IGenresService genresService,
            IDirectorsService directorsService,
            ICompaniesService companiesService,
            ITmdbService tmdbService,
            IRatingsService ratingsService)
        {
            this.moviesRepository = moviesRepository;
            this.actorsService = actorsService;
            this.genresService = genresService;
            this.directorsService = directorsService;
            this.companiesService = companiesService;
            this.tmdbService = tmdbService;
            this.ratingsService = ratingsService;
        }

        public async Task CreateAsync(CreateMovieInputModel input, string userId, string imagePath)
        {
            var movie = new Movie
            {
                Title = input.Title,
                Plot = input.Plot,
                ReleaseDate = input.ReleaseDate,
                Runtime = input.Runtime,
                AddedByUserId = userId,
            };

            foreach (var genreId in input.GenresIds)
            {
                var genre = this.genresService.GetById(genreId);
                if (genre == null)
                {
                    continue;
                }

                movie.Genres.Add(new MovieGenre
                {
                    Genre = genre,
                });
            }

            Directory.CreateDirectory($"{imagePath}/recipes/");

            // Create and Save Poster Image
            Image poster = this.CreateImage(input.Poster, userId);
            movie.Images.Add(poster);
            await this.SaveImageToWebRootAsync(imagePath, poster, input.Poster);

            movie.PosterPath = $"/{poster.Id}.{poster.Extension}";

            // Create and Save Backdrop Image
            Image backdrop = this.CreateImage(input.Backdrop, userId);
            movie.Images.Add(backdrop);
            await this.SaveImageToWebRootAsync(imagePath, backdrop, input.Backdrop);

            movie.BackdropPath = $"/{backdrop.Id}.{backdrop.Extension}";

            foreach (var image in input.Images)
            {
                Image dbImage = this.CreateImage(image, userId);
                movie.Images.Add(dbImage);
                await this.SaveImageToWebRootAsync(imagePath, dbImage, image);
            }

            foreach (var actorId in input.ActorsIds)
            {
                var actor = this.actorsService.GetById(actorId);
                if (actor == null)
                {
                    continue;
                }

                movie.Actors.Add(new MovieActor
                {
                    Actor = actor,
                });
            }

            foreach (var directorId in input.DirectorsIds)
            {
                var director = this.directorsService.GetById(directorId);
                if (director == null)
                {
                    continue;
                }

                movie.Directors.Add(new MovieDirector
                {
                    Director = director,
                });
            }

            foreach (var companyId in input.CompaniesIds)
            {
                var company = this.companiesService.GetById(companyId);
                if (company == null)
                {
                    continue;
                }

                movie.Companies.Add(new MovieCompany
                {
                    Company = company,
                });
            }

            await this.moviesRepository.AddAsync(movie);
            await this.moviesRepository.SaveChangesAsync();
        }

        private async Task SaveImageToWebRootAsync(string imagePath, Image dbImage, IFormFile image)
        {
            var posterPhysicalPath = $"{imagePath}/recipes/{dbImage.Id}.{dbImage.Extension}";
            using Stream fileStream = new FileStream(posterPhysicalPath, FileMode.Create);
            await image.CopyToAsync(fileStream);
        }

        private Image CreateImage(IFormFile image, string userId)
        {
            var extension = Path.GetExtension(image.FileName).TrimStart('.');
            if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
            {
                throw new Exception($"Invalid image extension {extension}");
            }

            var dbPoster = new Image
            {
                AddedByUserId = userId,
                Extension = extension,
            };

            return dbPoster;
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
                    PosterUrl =
                        x.PosterPath.Contains("-") ?
                        "/images/recipes/" + x.PosterPath :
                        this.tmdbService.GenereateImageUrl(x.PosterPath),
                    BackdropUrl =
                        x.BackdropPath.Contains("-") ?
                        "/images/recipes/" + x.BackdropPath :
                        this.tmdbService.GenereateImageUrl(x.BackdropPath),
                }).ToList();
        }

        public async Task<SingleMovieViewModel> GetMovieByIdAsync(int id, string userId)
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
                    PosterUrl =
                        x.PosterPath.Contains("-") ?
                        "/images/recipes/" + x.PosterPath :
                        this.tmdbService.GenereateImageUrl(x.PosterPath),
                    AverageRating = this.ratingsService.GetAverageRatings(x.Id),
                    RatingsCount = this.ratingsService.GetRatingsCount(x.Id),
                    UserRating = userId == null ? 0 : this.ratingsService.GetUserRating(x.Id, userId),
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
                    Images = x.Images.Where(i => !x.PosterPath.Contains(i.Id))
                    .Select(i => new ImageViewModel
                    {
                        PhotoUrl =
                            i.RemoteImageUrl != null ?
                            this.tmdbService.GenereateImageUrl(i.RemoteImageUrl) :
                            $"/images/recipes/{i.Id}.{i.Extension}",
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

        public async Task<IEnumerable<MovieViewModel>> GetUpcomingMoviesAsync()
        {
            var originalIds = await this.tmdbService.GetUpcomingMoviesOriginalIdAsync();

            await this.ImportMoviesIfNotExistAsync(originalIds);

            var upcomingMovies = new List<MovieViewModel>();
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

                upcomingMovies.Add(movie);
            }

            return upcomingMovies;
        }

        private async Task ImportMoviesIfNotExistAsync(IEnumerable<int> originalIds)
        {
            foreach (var originalId in originalIds)
            {
                if (this.moviesRepository.AllWithDeleted().Any(x => x.OriginalId == originalId))
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

        public CreateMovieInputModel PopulateMovieInputModelDropdownCollections(CreateMovieInputModel viewModel)
        {
            viewModel.GenresItems = this.genresService.GetAllAsKeyValuePairs()
                .Select(x => new SelectListItem(x.Value, x.Key));
            viewModel.ActorsItems = this.actorsService.GetAllAsKeyValuePairs()
                .Select(x => new SelectListItem(x.Value, x.Key));
            viewModel.DirectorsItems = this.directorsService.GetAllAsKeyValuePairs()
                .Select(x => new SelectListItem(x.Value, x.Key));
            viewModel.CompaniesItems = this.companiesService.GetAllAsKeyValuePairs()
                .Select(x => new SelectListItem(x.Value, x.Key));

            return viewModel;
        }
    }
}
