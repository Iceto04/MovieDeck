namespace MovieDeck.Services.TmdbApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;

    public interface ITmdbService
    {
        Task ImportMoviesAsync(int fromId, int toId);

        // Task ImportMovieAsync(Movie movie);

        // void GetMovieById(int id);

        void GetAll();
        Task<IEnumerable<Movie>> GetPopularMoviesAsync();

        // void GetMoviesInRange(int start, int end);
    }
}