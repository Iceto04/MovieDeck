namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Images;

    public interface IActorsService : IGetAllAsKeyValuePairs
    {
        T GetById<T>(int id);

        Actor GetActorById(int id);

        Task CreateAsync(CreateActorInputModel input, string userId, string imagePath);

        Task RemoveAllMovieActorsForMovieAsync(int id);

        List<ActorImageViewModel> GetActorPhotosForSingleActorPage(int originalId);
    }
}
