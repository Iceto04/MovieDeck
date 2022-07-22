namespace MovieDeck.Services.Data
{
    using MovieDeck.Data.Models;

    public interface IActorsService : IGetAllAsKeyValuePairs
    {
        Actor GetById(int id);
    }
}
