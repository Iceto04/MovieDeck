namespace MovieDeck.Services
{
    using System.Threading.Tasks;

    public interface IMovieScraperService
    {
        Task ImportMoviesAsync(int fromId = 1, int toId = 100);
    }
}
