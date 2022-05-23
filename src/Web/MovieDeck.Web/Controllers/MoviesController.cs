namespace MovieDeck.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MovieDeck.Web.ViewModels.Movies;

    public class MoviesController : Controller
    {
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Add(CreateMovieInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            return this.Redirect("/");
        }
    }
}
