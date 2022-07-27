namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Common;
    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Services.Mapping;
    using MovieDeck.Services.TmdbApi;
    using MovieDeck.Web.ViewModels.Actors;
    using MovieDeck.Web.ViewModels.Images;
    using MovieDeck.Web.ViewModels.Movies;

    public class ActorsService : IActorsService
    {
        private readonly IDeletableEntityRepository<Actor> actorsRepository;
        private readonly IRepository<MovieActor> movieActorRepository;
        private readonly IImagesService imagesService;
        private readonly ITmdbService tmdbService;

        public ActorsService(
            IDeletableEntityRepository<Actor> actorsRepository,
            IRepository<MovieActor> movieActorRepository,
            IImagesService imagesService,
            ITmdbService tmdbService)
        {
            this.actorsRepository = actorsRepository;
            this.movieActorRepository = movieActorRepository;
            this.imagesService = imagesService;
            this.tmdbService = tmdbService;
        }

        public async Task CreateAsync(CreateActorInputModel input, string userId, string imagePath)
        {
            var actor = new Actor
            {
                FullName = input.FullName,
                Biography = input.Biography,
                BirthDate = input.BirthDate,
            };

            Directory.CreateDirectory($"{imagePath}/{GlobalConstants.ActorsImagesFolder}/");

            Image photo = this.imagesService.CreateImage(input.Photo, userId);
            await this.imagesService
                .SaveImageToWebRootAsync(imagePath, photo, input.Photo, GlobalConstants.ActorsImagesFolder);
            actor.PhotoPath = $"/{photo.Id}.{photo.Extension}";

            await this.actorsRepository.AddAsync(actor);
            await this.actorsRepository.SaveChangesAsync();
        }

        public T GetById<T>(int id)
        {
            return this.actorsRepository.All()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();
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

        public async Task RemoveAllMovieActorsForMovieAsync(int id)
        {
            var movieActors = this.movieActorRepository.All().Where(x => x.MovieId == id);
            foreach (var movieActor in movieActors)
            {
                this.movieActorRepository.Delete(movieActor);
            }

            await this.movieActorRepository.SaveChangesAsync();
        }

        public List<ActorImageViewModel> GetActorPhotosForSingleActorPage(int originalId)
        {
            if (originalId == 0)
            {
                return new List<ActorImageViewModel>();
            }

            var actorImagesDtos = this.tmdbService.GetActorImages(originalId);

            var actorImages = new List<ActorImageViewModel>();

            foreach (var imageDto in actorImagesDtos)
            {
                var actorImage = new ActorImageViewModel
                {
                    PhotoUrl = string.Format(GlobalConstants.RemoteImagesUrl, imageDto.PhotoPath),
                };

                actorImages.Add(actorImage);
            }

            return actorImages;
        }

        public Actor GetActorById(int id)
        {
            return this.actorsRepository.All().FirstOrDefault(x => x.Id == id);
        }
    }
}
