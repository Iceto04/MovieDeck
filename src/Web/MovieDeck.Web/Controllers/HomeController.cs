namespace MovieDeck.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Services.Data;
    using MovieDeck.Services.TmdbApi;
    using MovieDeck.Web.ViewModels;
    using MovieDeck.Web.ViewModels.Home;
    using MovieDeck.Web.ViewModels.Movies;

    public class HomeController : BaseController
    {
        private readonly ITmdbService tmdbService;
        private readonly IMoviesService moviesService;

        public HomeController(ITmdbService tmdbService, IMoviesService moviesService)
        {
            this.tmdbService = tmdbService;
            this.moviesService = moviesService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = this.moviesService.GetAllForHomePage();
            var popularMovies = await this.moviesService.GetPopularMoviesAsync();
            var upcomingMovies = await this.moviesService.GetUpcomingMoviesAsync();

            var model = new IndexListViewModel
            {
                Movies = movies,
                PopularMovies = popularMovies,
                UpcomingMovies = upcomingMovies,
            };

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        public IActionResult Import()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(int from, int to)
        {
            await this.tmdbService.ImportMoviesInRangeAsync(from, to);
            return this.Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
