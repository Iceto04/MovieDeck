namespace MovieDeck.Web.Controllers
{
    using System.Diagnostics;
using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Services;
    using MovieDeck.Web.ViewModels;

    public class HomeController : BaseController
    {
        private readonly IMovieScraperService movieScraperService;

        public HomeController(IMovieScraperService movieScraperService)
        {
            this.movieScraperService = movieScraperService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Import()
        {
            await this.movieScraperService.ImportMoviesAsync(4154795, 4154830);

            return this.Redirect("/");
        }
    }
}
