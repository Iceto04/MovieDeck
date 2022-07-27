namespace MovieDeck.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MovieDeck.Common;
    using MovieDeck.Data.Common.Repositories;
    using MovieDeck.Data.Models;
    using MovieDeck.Web.ViewModels.Directors;

    public class DirectorsService : IDirectorsService
    {
        private readonly IDeletableEntityRepository<Director> directorsRepository;
        private readonly IRepository<MovieDirector> movieDirectorRepository;
        private readonly IImagesService imagesService;

        public DirectorsService(
            IDeletableEntityRepository<Director> directorsRepository,
            IRepository<MovieDirector> movieDirectorRepository,
            IImagesService imagesService)
        {
            this.directorsRepository = directorsRepository;
            this.movieDirectorRepository = movieDirectorRepository;
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



            Directory.CreateDirectory($"{imagePath}/{GlobalConstants.DirectorsImagesFolder}/");

            Image photo = this.imagesService.CreateImage(input.Photo, userId);
            await this.imagesService
                .SaveImageToWebRootAsync(imagePath, photo, input.Photo, GlobalConstants.DirectorsImagesFolder);
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

        public async Task RemoveAllMovieDirectorsForMovieAsync(int id)
        {
            var movieDirectors = this.movieDirectorRepository.All().Where(x => x.MovieId == id);
            foreach (var movieDirector in movieDirectors)
            {
                this.movieDirectorRepository.Delete(movieDirector);
            }

            await this.movieDirectorRepository.SaveChangesAsync();
        }
    }
}
