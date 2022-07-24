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
    using MovieDeck.Services.Mapping;
    using MovieDeck.Services.TmdbApi;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Directors;
    using MovieDeck.Web.ViewModels.Genres;
    using MovieDeck.Web.ViewModels.Images;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesService : IMoviesService
    {
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IActorsService actorsService;
        private readonly IGenresService genresService;
        private readonly IDirectorsService directorsService;
        private readonly ICompaniesService companiesService;
        private readonly IRatingsService ratingsService;
        private readonly IImagesService imagesService;
        private readonly ITmdbService tmdbService;

        public MoviesService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IActorsService actorsService,
            IGenresService genresService,
            IDirectorsService directorsService,
            ICompaniesService companiesService,
            IRatingsService ratingsService,
            IImagesService imagesService,
            ITmdbService tmdbService)
        {
            this.moviesRepository = moviesRepository;
            this.actorsService = actorsService;
            this.genresService = genresService;
            this.directorsService = directorsService;
            this.companiesService = companiesService;
            this.ratingsService = ratingsService;
            this.imagesService = imagesService;
            this.tmdbService = tmdbService;
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
            Image poster = this.imagesService.CreateImage(input.Poster, userId);
            await this.imagesService.SaveImageToWebRootAsync(imagePath, poster, input.Poster);

            movie.PosterPath = $"/{poster.Id}.{poster.Extension}";

            // Create and Save Backdrop Image
            Image backdrop = this.imagesService.CreateImage(input.Backdrop, userId);
            await this.imagesService.SaveImageToWebRootAsync(imagePath, backdrop, input.Backdrop);

            movie.BackdropPath = $"/{backdrop.Id}.{backdrop.Extension}";

            foreach (var image in input.Images)
            {
                Image dbImage = this.imagesService.CreateImage(image, userId);
                movie.Images.Add(dbImage);
                await this.imagesService.SaveImageToWebRootAsync(imagePath, dbImage, image);
            }

            foreach (var actorInput in input.Actors)
            {
                var actor = this.actorsService.GetById(actorInput.ActorId);
                if (actor == null)
                {
                    continue;
                }

                if (movie.Actors.Any(x => x.Actor == actor))
                {
                    throw new Exception($"{actor.FullName} is already set as an actor");
                }

                movie.Actors.Add(new MovieActor
                {
                    Actor = actor,
                    CharacterName = actorInput.CharacterName,
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

        public async Task UpdateAsync(int id, EditMovieInputModel input)
        {
            var movie = await this.moviesRepository.All().FirstOrDefaultAsync(x => x.Id == id);
            movie.Title = input.Title;
            movie.Plot = input.Plot;
            movie.ReleaseDate = input.ReleaseDate;
            movie.Runtime = input.Runtime;



            await this.genresService.RemoveAllMovieGenresForMovieAsync(id);
            movie.Genres.Clear();

            if (input.GenresIds != null)
            {
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
            }

            await this.actorsService.RemoveAllMovieActorsForMovieAsync(id);
            movie.Actors.Clear();

            if (input.Actors != null)
            {
                foreach (var actorInput in input.Actors)
                {
                    var actor = this.actorsService.GetById(actorInput.ActorId);
                    if (actor == null)
                    {
                        continue;
                    }

                    if (movie.Actors.Any(x => x.Actor == actor))
                    {
                        throw new Exception($"{actor.FullName} is already set as an actor");
                    }

                    movie.Actors.Add(new MovieActor
                    {
                        Actor = actor,
                        CharacterName = actorInput.CharacterName,
                    });
                }
            }


            await this.directorsService.RemoveAllMovieDirectorsForMovieAsync(id);
            movie.Directors.Clear();

            if (input.DirectorsIds != null)
            {
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
            }

            await this.companiesService.RemoveAllMovieCompaniesForMovieAsync(id);
            movie.Companies.Clear();

            if (input.CompaniesIds != null)
            {
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
            }
        }

        public IEnumerable<T> GetAllForHomePage<T>()
        {
            return this.moviesRepository.AllAsNoTracking()
                    .To<T>().ToList();
        }

        public async Task<T> GetMovieByIdAsync<T>(int id, string userId)
        {
            return await this.moviesRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>().FirstOrDefaultAsync();

                /* without AutoMapper
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
                    UserRating = (byte)(userId == null ? 0 : this.ratingsService.GetUserRating(x.Id, userId)),
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
                        PhotoUrl =
                            i.RemoteImageUrl != null ?
                            this.tmdbService.GenereateImageUrl(i.RemoteImageUrl) :
                            $"/images/recipes/{i.Id}.{i.Extension}",
                    }),
                */
        }

        public async Task<IEnumerable<T>> GetPopularMoviesAsync<T>()
        {
            var originalIds = await this.tmdbService.GetPopularMoviesOriginalIdAsync();

            await this.ImportMoviesIfNotExistAsync(originalIds);

            var popularMovies = new List<T>();
            foreach (var originalId in originalIds)
            {
                if (!this.moviesRepository.AllAsNoTracking().Any(x => x.OriginalId == originalId))
                {
                    continue;
                }

                var movie = await this.moviesRepository.AllAsNoTracking()
                    .Where(x => x.OriginalId == originalId)
                    .To<T>()
                    .FirstOrDefaultAsync();

                popularMovies.Add(movie);
            }

            return popularMovies;
        }

        public async Task<IEnumerable<T>> GetUpcomingMoviesAsync<T>()
        {
            var originalIds = await this.tmdbService.GetUpcomingMoviesOriginalIdAsync();

            await this.ImportMoviesIfNotExistAsync(originalIds);

            var upcomingMovies = new List<T>();
            foreach (var originalId in originalIds)
            {
                if (!this.moviesRepository.AllAsNoTracking().Any(x => x.OriginalId == originalId))
                {
                    continue;
                }

                var movie = await this.moviesRepository.AllAsNoTracking()
                    .Where(x => x.OriginalId == originalId)
                    .To<T>()
                    .FirstOrDefaultAsync();

                upcomingMovies.Add(movie);
            }

            return upcomingMovies;
        }

        public T PopulateMovieInputModelDropdownCollections<T>(T viewModel)
            where T : MovieInputModelDropdownItems
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
    }
}
