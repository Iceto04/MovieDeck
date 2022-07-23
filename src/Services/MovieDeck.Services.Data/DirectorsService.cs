namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Directors;

    public class DirectorsService : IDirectorsService
    {
        private readonly IDeletableEntityRepository<Director> directorsRepository;
        private readonly IImagesService imagesService;

        public DirectorsService(
            IDeletableEntityRepository<Director> directorsRepository,
            IImagesService imagesService)
        {
            this.directorsRepository = directorsRepository;
            this.imagesService = imagesService;
        }

        public async Task CreateAsync(CreateDirectorInputModel input, string userId, string imagePath)
        {
            var director = new Director
            {
                FullName = input.FullName,
                Biography = input.Biography,
                BirthDate = input.BirthDate,
            };

            Directory.CreateDirectory($"{imagePath}/recipes/");

            Image photo = this.imagesService.CreateImage(input.Photo, userId);
            await this.imagesService.SaveImageToWebRootAsync(imagePath, photo, input.Photo);
            director.PhotoPath = $"/{photo.Id}.{photo.Extension}";

            await this.directorsRepository.AddAsync(director);
            await this.directorsRepository.SaveChangesAsync();
        }

        public Director GetById(int id)
        {
            return this.directorsRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllAsKeyValuePairs()
        {
            return this.directorsRepository.AllAsNoTracking()
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
