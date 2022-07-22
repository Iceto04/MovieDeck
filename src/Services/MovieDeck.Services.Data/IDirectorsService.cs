namespace MovieDeck.Services.Data
{
    using MovieDeck.Data.Models;

    public interface IDirectorsService : IGetAllAsKeyValuePairs
    {
        Director GetById(int id);
    }
}
