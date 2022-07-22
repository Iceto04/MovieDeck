namespace MovieDeck.Web.Controllers
{
using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesController : BaseController
    {
        private readonly IGenresService genresService;
        private readonly IMoviesService moviesService;
        private readonly IWebHostEnvironment environment;

        public MoviesController(
            IGenresService genresService,
            IMoviesService moviesService,
            IWebHostEnvironment environment)
        {
            this.genresService = genresService;
            this.moviesService = moviesService;
            this.environment = environment;
        }

        public IActionResult Create()
        {
            var viewModel = new CreateMovieInputModel();

            viewModel = this.moviesService.PopulateMovieInputModelDropdownCollections(viewModel);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieInputModel input)
        {
            string userId = null;
            if (this.User.Identity.IsAuthenticated)
            {
                userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            if (!this.ModelState.IsValid)
            {
                input = this.moviesService.PopulateMovieInputModelDropdownCollections(input);
                return this.View(input);
            }

            try
            {
                await this.moviesService.CreateAsync(input, userId, $"{this.environment.WebRootPath}/images");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                input = this.moviesService.PopulateMovieInputModelDropdownCollections(input);
                return this.View(input);
            }

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
