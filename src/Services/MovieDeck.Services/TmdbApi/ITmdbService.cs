namespace MovieDeck.Services.TmdbApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Services.Models;

    public interface ITmdbService
    {
        Task ImportMovieAsync(MovieDto movieDto);

        Task ImportMoviesInRangeAsync(int fromId, int toId);

        void GetAll();

        string GenereateImageUrl(string path);

        Task<IEnumerable<int>> GetPopularMoviesOriginalIdAsync();

        Task<IEnumerable<int>> GetUpcomingMoviesOriginalIdAsync();

        Task<MovieDto> GetMovieById(int id);

        List<MovieVideoDto> GetMovieVideos(int id);

        // void GetMoviesInRange(int start, int end);
    }
}