namespace MovieDeck.Services.Data
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    using MovieDeck.Data.Models;

    public interface IImagesService
    {
        Task SaveImageToWebRootAsync(string imagePath, Image dbImage, IFormFile image, string folderPath);

        Image CreateImage(IFormFile image, string userId);
    }
}
