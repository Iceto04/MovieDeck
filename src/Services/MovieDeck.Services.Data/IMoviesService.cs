namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using MovieDeck.Web.ViewModels.Movies;

    public interface IMoviesService
    {
        Task AddAsync(AddMovieInputModel input);
    }
}
