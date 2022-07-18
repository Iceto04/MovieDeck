namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    public interface IRatingsService
    {
        Task SetRatingAsync(int movieId, string userId, byte value);

        double GetAverageRatings(int movieId);
    }
}
