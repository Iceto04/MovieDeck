namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Actors;

    public interface IActorsService : IGetAllAsKeyValuePairs
    {
        Actor GetById(int id);

        Task CreateAsync(CreateActorInputModel input, string userId, string imagePath);

        Task RemoveAllMovieActorsForMovieAsync(int id);
    }
}
