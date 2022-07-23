namespace MovieDeck.Web.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Actors;

    public class ActorsController : BaseController
    {
        private readonly IActorsService actorsService;
        private readonly IWebHostEnvironment environment;

        public ActorsController(
            IActorsService actorsService,
            IWebHostEnvironment environment)
        {
            this.actorsService = actorsService;
            this.environment = environment;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreateActorInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            string userId = null;
            if (this.User.Identity.IsAuthenticated)
            {
                userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            this.actorsService.CreateAsync(input, userId, $"{this.environment.WebRootPath}/images");

            return this.Redirect("/");
        }
    }
}
