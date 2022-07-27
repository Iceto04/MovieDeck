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

        Task<IEnumerable<int>> GetPopularMoviesOriginalIdAsync();

        Task<IEnumerable<int>> GetUpcomingMoviesOriginalIdAsync();

        Task<MovieDto> GetMovieById(int id);

        List<MovieVideoDto> GetMovieVideos(int originaId);

        List<ActorImageDto> GetActorImages(int originalId);

        // void GetMoviesInRange(int start, int end);
    }
}