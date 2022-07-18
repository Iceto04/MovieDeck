namespace MovieDeck.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieDeck.Services.Data;
    using MovieDeck.Web.ViewModels.Ratings;

    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : BaseController
    {
        private readonly IRatingsService ratingsService;

        public RatingsController(IRatingsService ratingsService)
        {
            this.ratingsService = ratingsService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostRatingResponseModel>> Post(PostRatingInputModel input)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isMovieRated = this.ratingsService.IsMovieRated(input.MovieId, userId);
            await this.ratingsService.SetRatingAsync(input.MovieId, userId, input.Value);
            var averageRating = this.ratingsService.GetAverageRatings(input.MovieId);
            return new PostRatingResponseModel
            {
                AverageRating = averageRating,
                IsMovieRatedByCurrUser = isMovieRated,
            };
        }
    }
}
