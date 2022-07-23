namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Actors;

    public class ActorsService : IActorsService
    {
        private readonly IDeletableEntityRepository<Actor> actorsRepository;
        private readonly IImagesService imagesService;

        public ActorsService(
            IDeletableEntityRepository<Actor> actorsRepository,
            IImagesService imagesService)
        {
            this.actorsRepository = actorsRepository;
            this.imagesService = imagesService;
        }

        public async Task CreateAsync(CreateActorInputModel input, string userId, string imagePath)
        {
            var actor = new Actor
            {
                FullName = input.FullName,
                Biography = input.Biography,
                BirthDate = input.BirthDate,
            };

            Directory.CreateDirectory($"{imagePath}/recipes/");

            Image photo = this.imagesService.CreateImage(input.Photo, userId);
            await this.imagesService.SaveImageToWebRootAsync(imagePath, photo, input.Photo);
            actor.PhotoPath = $"/{photo.Id}.{photo.Extension}";

            await this.actorsRepository.AddAsync(actor);
            await this.actorsRepository.SaveChangesAsync();
        }

        public Actor GetById(int id)
        {
            return this.actorsRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.actorsRepository.AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.FullName,
                })
                .OrderBy(x => x.FullName)
                .ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.FullName));
        }
    }
}
