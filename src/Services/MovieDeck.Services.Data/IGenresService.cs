namespace MovieDeck.Services.Data
{
    using MovieDeck.Data.Models;

    public interface IGenresService : IGetAllAsKeyValuePairs
    {
        Genre GetById(int id);
    }
}
