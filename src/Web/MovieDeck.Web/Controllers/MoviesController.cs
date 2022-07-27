namespace MovieDeck.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Common;
    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesController : BaseController
    {
        private readonly IMoviesService moviesService;
        private readonly IRatingsService ratingsService;
        private readonly IWebHostEnvironment environment;

        public MoviesController(
            IMoviesService moviesService,
            IRatingsService ratingsService,
            IWebHostEnvironment environment)
        {
            this.moviesService = moviesService;
            this.ratingsService = ratingsService;
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

        public async Task<IActionResult> Edit(int id)
        {
            var input = await this.moviesService.GetMovieByIdAsync<EditMovieInputModel>(id);
            if (input == null)
            {
                return this.NotFound();
            }

            input = this.moviesService.PopulateMovieInputModelDropdownCollections(input);
            return this.View(input);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditMovieInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input = this.moviesService.PopulateMovieInputModelDropdownCollections(input);
                return this.View(input);
            }

            await this.moviesService.UpdateAsync(id, input);

            return this.RedirectToAction(nameof(this.ById), new { id });
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            await this.moviesService.DeleteAsync(id);
            return this.RedirectToAction("Index", "Home");
        }

        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> ById(int id)
        {
            var model = await this.moviesService.GetMovieByIdAsync<SingleMovieViewModel>(id);
            if (model == null)
            {
                return this.NotFound();
            }

            string userId = null;
            if (this.User.Identity.IsAuthenticated)
            {
                userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            model.UserRating = userId == null ? (byte)0 : this.ratingsService.GetUserRating(id, userId);
            model.Videos = this.moviesService.GetMovieVideosForSingleMoviePage(model.OriginalId);
            return this.View(model);
        }
    }
}
