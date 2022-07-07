namespace MovieDeck.Services.TmdbApi
{
    using System;
    using System.Collections.Generic;
using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
using AngleSharp.Media;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Models;

    using TMDbLib.Client;
    using TMDbLib.Objects.Configuration;
    using TMDbLib.Objects.Movies;

    public class TmdbService : ITmdbService
    {
        private const string TmdbApiKey = "ce275f68468813163db7ae1ad4adad0d";
        private const string BaseUrl = "https://www.themoviedb.org";

        private readonly IDeletableEntityRepository<Data.Models.Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Actor> actorsRepository;
        private readonly IDeletableEntityRepository<Director> directorsRepository;
        private readonly IDeletableEntityRepository<ProductionCompany> companiesRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly IDeletableEntityRepository<Image> imagesRepository;
        private readonly IRepository<MovieActor> movieActorsRepository;
        private readonly IRepository<MovieDirector> movieDirectorsRepository;
        private readonly IRepository<MovieCompany> movieCompaniesRepository;
        private readonly IRepository<MovieGenre> movieGenresRepository;

        private TMDbClient client;
        private APIConfiguration config;

        public TmdbService(
            IDeletableEntityRepository<Data.Models.Movie> moviesRepository,
            IDeletableEntityRepository<Actor> actorsRepository,
            IDeletableEntityRepository<Director> directorsRepository,
            IDeletableEntityRepository<ProductionCompany> companiesRepository,
            IDeletableEntityRepository<Genre> genresRepository,
            IDeletableEntityRepository<Image> imagesRepository,
            IRepository<MovieActor> movieActorsRepository,
            IRepository<MovieDirector> movieDirectorsRepository,
            IRepository<MovieCompany> movieCompaniesRepository,
            IRepository<MovieGenre> movieGenresRepository)
        {
            this.moviesRepository = moviesRepository;
            this.actorsRepository = actorsRepository;
            this.directorsRepository = directorsRepository;
            this.companiesRepository = companiesRepository;
            this.genresRepository = genresRepository;
            this.imagesRepository = imagesRepository;
            this.movieActorsRepository = movieActorsRepository;
            this.movieDirectorsRepository = movieDirectorsRepository;
            this.movieCompaniesRepository = movieCompaniesRepository;
            this.movieGenresRepository = movieGenresRepository;

            this.client = new TMDbClient(TmdbApiKey);
            this.config = this.client.GetAPIConfiguration().Result;
        }

        public async Task ImportMoviesAsync(int fromId, int toId)
        {
            var movies = await this.GetMoviesInRangeAsync(fromId, toId);

            foreach (var movieDto in movies)
            {
                if (this.moviesRepository.AllAsNoTracking().Any(x => x.Title == movieDto.Title && x.Plot == movieDto.Plot))
                {
                    continue;
                }

                var movie = new Data.Models.Movie
                {
                    Title = movieDto.Title,
                    Plot = movieDto.Plot,
                    ReleaseDate = movieDto.ReleaseDate,
                    Runtime = movieDto.Runtime,
                    ImdbRating = movieDto.ImdbRating,
                    OriginalUrl = movieDto.OriginalUrl,
                    PosterUrl = movieDto.PosterUrl,
                };

                await this.moviesRepository.AddAsync(movie);

                foreach (var actorDto in movieDto.Actors)
                {
                    var actorId = await this.GetOrCreateActorAsync(actorDto);
                    var characterName = actorDto.Character;

                    var movieActor = new MovieActor
                    {
                        ActorId = actorId,
                        Movie = movie,
                        CharacterName = characterName,
                    };
                    await this.movieActorsRepository.AddAsync(movieActor);
                }

                foreach (var directorDto in movieDto.Directors)
                {
                    var directorId = await this.GetOrCreateDirectorAsync(directorDto);

                    var movieDirector = new MovieDirector
                    {
                        DirectorId = directorId,
                        Movie = movie,
                    };
                    await this.movieDirectorsRepository.AddAsync(movieDirector);
                }

                foreach (var genreName in movieDto.Genres)
                {
                    var genreId = await this.GetOrCreateGenreAsync(genreName);

                    var movieGenre = new MovieGenre
                    {
                        GenreId = genreId,
                        Movie = movie,
                    };
                    await this.movieGenresRepository.AddAsync(movieGenre);
                }

                foreach (var companyName in movieDto.Companies)
                {
                    var companyId = await this.GetOrCreateCompanyAsync(companyName);

                    var movieCompany = new MovieCompany
                    {
                        CompanyId = companyId,
                        Movie = movie,
                    };
                    await this.movieCompaniesRepository.AddAsync(movieCompany);
                }

                foreach (var imageUrl in movieDto.Images)
                {
                    var image = new Image
                    {
                        OriginalUrl = imageUrl,
                        Movie = movie,
                    };

                    await this.imagesRepository.AddAsync(image);
                }

                await this.moviesRepository.SaveChangesAsync();
                Console.WriteLine(movie.Title + "is added");
            }
        }

        public async Task<IEnumerable<Data.Models.Movie>> GetPopularMoviesAsync()
        {
            var topPopularMovies = await this.client.GetMoviePopularListAsync();

            return topPopularMovies.Results.Select(x => new Data.Models.Movie
            {
                Title = x.Title,
                ImdbRating = x.VoteAverage.ToString("F1"),
                PosterUrl = this.config.Images.BaseUrl
                    + this.config.Images.PosterSizes.LastOrDefault() + x.PosterPath,
            });
        }

        public void GetAll()
        {
            throw new System.NotImplementedException();
        }

        private async Task<MovieDto> GetMovieById(int id)
        {
            TMDbLib.Objects.Movies.Movie movieInfo = this.client
                .GetMovieAsync(id, MovieMethods.Credits | MovieMethods.Images).Result;

            if (movieInfo == null)
            {
                return null;
            }

            var movie = new MovieDto
            {
                Title = movieInfo.Title,
                Plot = string.IsNullOrEmpty(movieInfo.Overview) ? null : movieInfo.Overview,
                ReleaseDate = movieInfo.ReleaseDate,
                Runtime = TimeSpan.FromMinutes((double)movieInfo.Runtime),
                ImdbRating = movieInfo.VoteAverage.ToString(),
                OriginalUrl = BaseUrl + $"/movie/{movieInfo.Id}",
                PosterUrl = movieInfo.PosterPath != null ? this.config.Images.BaseUrl
                    + this.config.Images.PosterSizes.LastOrDefault() + movieInfo.PosterPath : null,
                Genres = movieInfo.Genres.Select(x => x.Name).ToList(),
                Companies = movieInfo.ProductionCompanies.Select(x => x.Name).ToList(),
                Images = this.GetMovieImages(movieInfo),
                Directors = await this.GetMovieDirectorsAsync(movieInfo),
                Actors = await this.GetMovieActorsAsync(movieInfo),
            };

            Console.WriteLine(movie.Title + "is taken");

            return movie;
        }

        private async Task<List<ActorDto>> GetMovieActorsAsync(TMDbLib.Objects.Movies.Movie movieInfo)
        {
            var actors = new List<ActorDto>();

            foreach (var actor in movieInfo.Credits.Cast)
            {
                var actorInfo = await this.client.GetPersonAsync(actor.Id);

                actors.Add(new ActorDto
                {
                    FullName = actorInfo.Name,
                    Biography = string.IsNullOrEmpty(actorInfo.Biography) ? null : actorInfo.Biography,
                    BirthDate = actorInfo.Birthday,
                    PhotoUrl = actorInfo.ProfilePath != null ? this.config.Images.BaseUrl
                        + this.config.Images.ProfileSizes.LastOrDefault() + actorInfo.ProfilePath : null,
                    Character = string.IsNullOrEmpty(actor.Character) ? null : actor.Character,
                });
            }

            return actors;
        }

        private async Task<List<PersonDto>> GetMovieDirectorsAsync(TMDbLib.Objects.Movies.Movie movieInfo)
{
            var directors = new List<PersonDto>();

            foreach (var director in movieInfo.Credits.Crew.Where(x => x.Job == "Director"))
            {
                var directorInfo = await this.client.GetPersonAsync(director.Id);

                directors.Add(new PersonDto
                {
                    FullName = directorInfo.Name,
                    Biography = string.IsNullOrEmpty(directorInfo.Biography) ? null : directorInfo.Biography,
                    BirthDate = directorInfo.Birthday,
                    PhotoUrl = directorInfo.ProfilePath != null ? this.config.Images.BaseUrl
                        + this.config.Images.ProfileSizes.LastOrDefault() + directorInfo.ProfilePath : null,
                });
            }

            return directors;
        }

        private List<string> GetMovieImages(TMDbLib.Objects.Movies.Movie movieInfo)
        {
            return movieInfo.Images.Backdrops.Select(x => this.config.Images.BaseUrl
                        + this.config.Images.ProfileSizes.LastOrDefault() + x.FilePath).ToList();
        }

        private async Task<IEnumerable<MovieDto>> GetMoviesInRangeAsync(int fromId, int toId)
        {
            var movies = new List<MovieDto>();
            for (int i = fromId; i <= toId; i++)
            {
                var movie = await this.GetMovieById(i);

                if (movie != null)
                {
                    movies.Add(movie);
                }
            }

            return movies;
        }

        private async Task<int> GetOrCreateActorAsync(ActorDto actorDto)
        {
            var actor = this.actorsRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.FullName == actorDto.FullName && x.Biography == actorDto.Biography);

            if (actor != null)
            {
                return actor.Id;
            }

            actor = new Actor
            {
                FullName = actorDto.FullName,
                Biography = actorDto.Biography,
                BirthDate = actorDto.BirthDate,
                PhotoUrl = actorDto.PhotoUrl,
            };

            await this.actorsRepository.AddAsync(actor);
            await this.actorsRepository.SaveChangesAsync();

            return actor.Id;
        }

        private async Task<int> GetOrCreateDirectorAsync(PersonDto directorDto)
        {
            var director = this.directorsRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.FullName == directorDto.FullName && x.Biography == directorDto.Biography);

            if (director != null)
            {
                return director.Id;
            }

            director = new Director
            {
                FullName = directorDto.FullName,
                Biography = directorDto.Biography,
                BirthDate = directorDto.BirthDate,
                PhotoUrl = directorDto.PhotoUrl,
            };

            await this.directorsRepository.AddAsync(director);
            await this.directorsRepository.SaveChangesAsync();

            return director.Id;
        }

        private async Task<int> GetOrCreateGenreAsync(string name)
        {
            var genre = this.genresRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Name == name);

            if (genre != null)
            {
                return genre.Id;
            }

            genre = new Genre
            {
                Name = name,
            };

            await this.genresRepository.AddAsync(genre);
            await this.genresRepository.SaveChangesAsync();

            return genre.Id;
        }

        private async Task<int> GetOrCreateCompanyAsync(string name)
        {
            var company = this.companiesRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Name == name);

            if (company != null)
            {
                return company.Id;
            }

            company = new ProductionCompany
            {
                Name = name,
            };

            await this.companiesRepository.AddAsync(company);
            await this.companiesRepository.SaveChangesAsync();

            return company.Id;
        }
    }
}
