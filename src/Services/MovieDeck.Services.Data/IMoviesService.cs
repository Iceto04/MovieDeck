namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Movies;

    public interface IMoviesService
    {
        Task CreateAsync(CreateMovieInputModel input, string userId, string imagePath);

        IEnumerable<MovieViewModel> GetAllForHomePage();

        Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync();

        Task<IEnumerable<MovieViewModel>> GetUpcomingMoviesAsync();

        Task<SingleMovieViewModel> GetMovieByIdAsync(int id, string userId = null);

        T PopulateMovieInputModelDropdownCollections<T>(T viewModel)
            where T : MovieInputModelDropdownItems;
    }
}
