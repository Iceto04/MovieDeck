namespace MovieDeck.Web.Controllers
{
    using System.Linq;
using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesController : BaseController
    {
        private readonly IGenresService genresService;
        private readonly IMoviesService moviesService;

        public MoviesController(
            IGenresService genresService,
            IMoviesService moviesService)
        {
            this.genresService = genresService;
            this.moviesService = moviesService;
        }

        public IActionResult Add()
        {
            var viewModel = new AddMovieInputModel();
            viewModel.GenresItems = this.genresService.GetAllAsKeyValuePairs()
                .Select(x => new SelectListItem(x.Value, x.Key));

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMovieInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.GenresItems = this.genresService.GetAllAsKeyValuePairs()
                    .Select(x => new SelectListItem(x.Value, x.Key));
                return this.View();
            }

            await this.moviesService.AddAsync(input);

            return this.Redirect("/");
        }

        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> ById(int id)
        {
            string userId = null;
            if (this.User.Identity.IsAuthenticated)
            {
                userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            var model = await this.moviesService.GetMovieByIdAsync(id, userId);
            return this.View(model);
        }
    }
}
