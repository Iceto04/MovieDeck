namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Movies;

    public interface IMoviesService
    {
        Task CreateAsync(CreateMovieInputModel input, string userId, string imagePath);

        Task UpdateAsync(int id, EditMovieInputModel input);

        IEnumerable<T> GetAllForHomePage<T>();

        Task<IEnumerable<T>> GetPopularMoviesAsync<T>();

        Task<IEnumerable<T>> GetUpcomingMoviesAsync<T>();

        Task<T> GetMovieByIdAsync<T>(int id, string userId = null);

        T PopulateMovieInputModelDropdownCollections<T>(T viewModel)
            where T : MovieInputModelDropdownItems;
    }
}
