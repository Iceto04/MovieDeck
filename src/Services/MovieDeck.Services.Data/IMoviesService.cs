namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Movies;

    public interface IMoviesService
    {
        Task AddAsync(AddMovieInputModel input);

        IEnumerable<MovieViewModel> GetAllForHomePage();

        Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync();

        Task<SingleMovieViewModel> GetMovieByIdAsync(int id);
    }
}
