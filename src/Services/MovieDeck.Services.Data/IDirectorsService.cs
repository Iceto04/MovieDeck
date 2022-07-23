namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Directors;

    public interface IDirectorsService : IGetAllAsKeyValuePairs
    {
        Director GetById(int id);

        Task CreateAsync(CreateDirectorInputModel input, string userId, string imagePath);
    }
}
