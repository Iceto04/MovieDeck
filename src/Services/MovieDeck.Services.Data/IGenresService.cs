namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;

    public interface IGenresService : IGetAllAsKeyValuePairs
    {
        Genre GetById(int id);

        Task RemoveAllMovieGenresForMovieAsync(int id);
    }
}
