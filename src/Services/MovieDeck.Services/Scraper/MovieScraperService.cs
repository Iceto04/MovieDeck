namespace MovieDeck.Services.Scraper
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AngleSharp;
    using AngleSharp.Dom;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Models;

    /// <summary>
    /// Won't be used due to finding a good API that I can get data with.
    /// </summary>
    public class MovieScraperService : IMovieScraperService
    {
        private const string BaseUrl = "https://www.imdb.com/title/tt{0}/";
        private const string PosterUrlExtension = "._V1_.jpg";

        private readonly IBrowsingContext context;
        private readonly IDeletableEntityRepository<Movie> moviesRepository;
        private readonly IDeletableEntityRepository<Actor> actorsRepository;
        private readonly IDeletableEntityRepository<Director> directorsRepository;
        private readonly IRepository<MovieActor> movieActorsRepository;
        private readonly IRepository<MovieCompany> movieCompaniesRepository;
        private readonly IRepository<MovieDirector> movieDirectorsRepository;
        private readonly IRepository<MovieGenre> movieGenresRepository;
        private readonly IDeletableEntityRepository<Genre> genresRepository;
        private readonly IDeletableEntityRepository<ProductionCompany> companiesRepository;

        public MovieScraperService(
            IDeletableEntityRepository<Movie> moviesRepository,
            IDeletableEntityRepository<Actor> actorsRepository,
            IDeletableEntityRepository<Director> directorsRepository,
            IDeletableEntityRepository<ProductionCompany> companiesRepository,
            IDeletableEntityRepository<Genre> genresRepository,
            //// IDeletableEntityRepository<Image> imagesRepository,
            IRepository<MovieActor> movieActorsRepository,
            IRepository<MovieDirector> movieDirectorsRepository,
            IRepository<MovieCompany> movieCompaniesRepository,
            IRepository<MovieGenre> movieGenresRepository)
        {
            this.moviesRepository = moviesRepository;
            this.actorsRepository = actorsRepository;
            this.directorsRepository = directorsRepository;
            this.movieActorsRepository = movieActorsRepository;
            this.movieCompaniesRepository = movieCompaniesRepository;
            this.movieDirectorsRepository = movieDirectorsRepository;
            this.movieGenresRepository = movieGenresRepository;
            this.genresRepository = genresRepository;
            this.companiesRepository = companiesRepository;

            var config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(config);
        }

        public async Task ImportMoviesAsync(int fromId = 1, int toId = 100)
        {
            var concurrentBag = this.ScrapeMovies(fromId, toId);

            var added = 0;
            foreach (var movieDto in concurrentBag)
            {
                if (movieDto.Runtime.Days >= 1)
                {
                    movieDto.Runtime = new TimeSpan(23, 59, 59);
                }

                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Plot = movieDto.Plot,
                    ReleaseDate = movieDto.ReleaseDate,
                    Runtime = movieDto.Runtime,
                    ImdbRating = movieDto.ImdbRating,
                    PosterPath = movieDto.PosterPath,
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

                /*
                foreach (var imageUrl in movieDto.Images)
                {
                    var image = new Image
                    {
                        OriginalUrl = imageUrl,
                        Movie = movie,
                    };

                    await this.imagesRepository.AddAsync(image);
                }
                */

                added++;
            }

            await this.moviesRepository.SaveChangesAsync();
            Console.WriteLine($"Added {added}");
        }

        private static bool IsMovieDocumentIncorect(IDocument document)
        {
            return document.StatusCode == HttpStatusCode.NotFound ||
                    document.QuerySelector("div.sc-6592f214-2 > a")?
                    .TextContent.Contains("Go to the homepage") == true ||
                    document.QuerySelector("ul[data-testid=hero-title-block__metadata] > li")?
                    .TextContent.Contains("TV Series") == true ||
                    document.QuerySelector("a[data-testid=hero-subnav-bar-all-episodes-button] > div")?
                    .TextContent.Contains("All episodes") == true ||
                    document.QuerySelector("a[data-testid=hero-subnav-bar-series-episode-guide-link] > span")?
                    .TextContent.Contains("Episode guide") == true ||
                    document.QuerySelector("ul[data-testid=hero-title-block__metadata] > li")?
                    .TextContent.Contains("TV Mini Series") == true;
        }

        private ConcurrentBag<MovieDto> ScrapeMovies(int fromId, int toId)
        {
            var concurentBag = new ConcurrentBag<MovieDto>();
            Parallel.For(fromId, toId + 1, (i) =>
            {
                try
                {
                    var movie = this.GetMovie(i);
                    concurentBag.Add(movie);
                }
                catch
                {
                }
            });
            return concurentBag;
        }

        private MovieDto GetMovie(long id)
        {
            var url = string.Format(BaseUrl, id.ToString("D7"));

            var document = this.context
                .OpenAsync(url)
                .GetAwaiter()
                .GetResult();

            if (IsMovieDocumentIncorect(document))
            {
                throw new InvalidOperationException();
            }

            var movieDto = new MovieDto();

            movieDto.Title = document
                .QuerySelector("h1[data-testid=hero-title-block__title]")
                .TextContent;

            Console.WriteLine(movieDto.Title);

            var fullMoviePlotDoc = this.context
                    .OpenAsync($"{url}plotsummary")
                    .GetAwaiter()
                    .GetResult();

            movieDto.Plot = fullMoviePlotDoc.QuerySelectorAll(".ipl-zebra-list__item > p")[0].TextContent;

            var releaseDateAsString = document
                .QuerySelector("li[data-testid=title-details-releasedate] li.ipc-inline-list__item > a")
                .TextContent;

            var releaseDateNotSplited = releaseDateAsString
                .Substring(0, releaseDateAsString.IndexOf("("))
                .Trim()
                .Replace(",", string.Empty);

            var releaseDateElements = releaseDateNotSplited.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var month = releaseDateElements[0];
            var day = int.Parse(releaseDateElements[1]);
            var year = int.Parse(releaseDateElements[2]);

            int monthNumber = DateTimeFormatInfo.InvariantInfo.MonthNames.ToList().IndexOf(month) + 1;

            movieDto.ReleaseDate = new DateTime(year, monthNumber, day);

            var runtimeAsString = document.QuerySelector("li[data-testid=title-techspec_runtime] > div").TextContent;

            var runtimeHours = 0;
            var runtimeMinutes = 0;

            if (runtimeAsString.Contains("hours"))
            {
                var runtimeData = runtimeAsString
                    .ToLower()
                    .Replace("hours", string.Empty)
                    .Trim();

                if (runtimeAsString.Contains("minute"))
                {
                    var runtimeDataSplited = runtimeData.Replace("minute", string.Empty)
                        .Trim()
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Take(2)
                        .Select(int.Parse)
                        .ToArray();

                    runtimeHours = runtimeDataSplited[0];
                    runtimeMinutes = runtimeDataSplited[1];
                }

                if (runtimeMinutes == 0)
                {
                    runtimeHours = int.Parse(runtimeData);
                }
            }
            else if (runtimeAsString.Contains("hour"))
            {
                var runtimeDataSplited = runtimeAsString
                .ToLower()
                .Replace("hour", string.Empty)
                .Replace("minute", string.Empty)
                .Trim()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(int.Parse)
                .ToArray();

                runtimeHours = runtimeDataSplited[0];
                runtimeMinutes = runtimeDataSplited[1];
            }
            else
            {
                var runtimeDataSplited = runtimeAsString
                .ToLower()
                .Replace("minute", string.Empty)
                .Trim()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Take(1)
                .Select(int.Parse)
                .ToArray();

                runtimeMinutes = runtimeDataSplited[0];
            }

            movieDto.Runtime = new TimeSpan(runtimeHours, runtimeMinutes, 0);

            movieDto.ImdbRating = document.QuerySelector("div[data-testid=hero-rating-bar__aggregate-rating__score] > span")?.TextContent;

            var posterUrl = document
                .QuerySelector("div[data-testid=hero-media__poster] > div > img")
                .GetAttribute("src");

            movieDto.PosterPath = posterUrl.Substring(0, posterUrl.LastIndexOf("@") + 1) + PosterUrlExtension;

            var genres = document.QuerySelectorAll("div[data-testid=genres] a > ul > li")
                .Select(x => x.TextContent).ToList();

            movieDto.Genres.AddRange(genres);

            var directors = document
                .QuerySelector("section[data-testid=title-cast] > ul > li > div > ul")
                .QuerySelectorAll("li > a");

            foreach (var director in directors)
            {
                try
                {
                    var directorDto = new PersonDto();

                    directorDto.FullName = director.TextContent;

                    var directorFullInfoUrl = director.GetAttribute("href");
                    directorFullInfoUrl = directorFullInfoUrl.Substring(0, directorFullInfoUrl.IndexOf("?"));

                    var directorFullInfoDoc = this.context.OpenAsync($"https://www.imdb.com/{directorFullInfoUrl}")
                        .GetAwaiter()
                        .GetResult();

                    var directorBirthDateAsString = directorFullInfoDoc.QuerySelector("#name-born-info > time")?.GetAttribute("datetime");
                    //// {yyyy-MM-dd} date format

                    if (string.IsNullOrEmpty(directorBirthDateAsString))
                    {
                        directorDto.BirthDate = null;
                    }
                    else
                    {
                        directorDto.BirthDate = DateTime.ParseExact(directorBirthDateAsString, "yyyy-M-d", CultureInfo.InvariantCulture);
                    }

                    directorDto.Biography = directorFullInfoDoc.QuerySelector(".name-trivia-bio-text > div")?.TextContent
                        .Replace("See full bio »", string.Empty)
                        .Trim();

                    var photoUrl = directorFullInfoDoc
                        .QuerySelector("#name-poster")?.GetAttribute("src");

                    directorDto.PhotoPath = photoUrl
                            .Substring(0, photoUrl.LastIndexOf("._V1_")) + PosterUrlExtension;
                    /*
                    if (!photoUrl.Contains("@"))
                    {
                        directorDto.PhotoUrl = photoUrl
                            .Substring(0, photoUrl.LastIndexOf("._V1_")) + PosterUrlExtension;
                    }
                    else
                    {
                        directorDto.PhotoUrl = photoUrl
                            .Substring(0, photoUrl.LastIndexOf("@") + 1) + PosterUrlExtension;
                    }
                    */

                    movieDto.Directors.Add(directorDto);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            Console.WriteLine($"Directors Count:{movieDto.Directors.Count}");

            var companies = document
                .QuerySelector("li[data-testid=title-details-companies] > div > ul")
                .QuerySelectorAll("li > a")
                .Select(x => x.TextContent)
                .ToList();

            movieDto.Companies.AddRange(companies);

            var actorsDoc = this.context
                .OpenAsync($"{url}fullcredits")
                .GetAwaiter()
                .GetResult();

            var actors = actorsDoc.QuerySelectorAll("table.cast_list > tbody > tr.odd, tr.even");

            foreach (var actor in actors)
            {
                try
                {
                    var actorDto = new ActorDto();

                    actorDto.FullName = actor.QuerySelector("td:not([class]) > a").TextContent.Trim();

                    var characterInfo = actor.QuerySelector("td.character")
                        .TextContent
                        .Trim()
                        .Replace("\n", string.Empty);

                    if (characterInfo.Contains("/") && (characterInfo.Contains("(") || characterInfo.Contains(")")))
                    {
                        var character = characterInfo
                            .Split("/", StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();

                        var characterSplitted = string.Join(" / ", character);

                        character = characterSplitted.Split("  ", StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();

                        actorDto.Character = string.Join(" ", character);
                    }
                    else if (characterInfo.Contains("/"))
                    {
                        var characters = characterInfo
                            .Split("/", StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();

                        actorDto.Character = string.Join(" / ", characters);
                    }
                    else
                    {
                        var characters = characterInfo
                        .Split("  ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                        actorDto.Character = string.Join(" ", characters);
                    }

                    var actorFullInfoUrl = actor.QuerySelector("td.primary_photo > a")?.GetAttribute("href");
                    actorFullInfoUrl = actorFullInfoUrl.Substring(0, actorFullInfoUrl.LastIndexOf("/"));

                    var actorFullInfoDoc = this.context.OpenAsync($"https://www.imdb.com/{actorFullInfoUrl}")
                        .GetAwaiter()
                        .GetResult();

                    var actorBirthDateAsString = actorFullInfoDoc
                        .QuerySelector("#name-born-info > time")?.GetAttribute("datetime");

                    if (string.IsNullOrEmpty(actorBirthDateAsString))
                    {
                        actorDto.BirthDate = null;
                    }
                    else
                    {
                        actorDto.BirthDate = DateTime.ParseExact(actorBirthDateAsString, "yyyy-M-d", CultureInfo.InvariantCulture);
                    }

                    actorDto.Biography = actorFullInfoDoc.QuerySelector(".name-trivia-bio-text > div")?.TextContent
                        .Replace("See full bio »", string.Empty)
                        .Trim();

                    var photoUrl = actorFullInfoDoc.QuerySelector("#name-poster")?.GetAttribute("src");

                    actorDto.PhotoPath = photoUrl.Substring(0, photoUrl.LastIndexOf("._V1_")) + PosterUrlExtension;

                    /*
                    if (!photoUrl.Contains("@"))
                    {
                        actorDto.PhotoUrl = photoUrl.Substring(0, photoUrl.LastIndexOf("._V1_")) + PosterUrlExtension;
                    }
                    else
                    {
                        actorDto.PhotoUrl = photoUrl.Substring(0, photoUrl.LastIndexOf("@") + 1) + PosterUrlExtension;
                    }
                    */

                    movieDto.Actors.Add(actorDto);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            Console.WriteLine($"Actors Count:{movieDto.Actors.Count}");
            Console.WriteLine();

            return movieDto;
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
                PhotoPath = actorDto.PhotoPath,
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
                PhotoPath = directorDto.PhotoPath,
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
