namespace MovieDeck.Data.Models
{
    using System;

    using MovieDeck.Data.Common.Models;

    public class Image : BaseDeletableModel<string>
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public string RemoteImageUrl { get; set; }

        //// The contents of the image is in the file system
        public string Extension { get; set; }

        public string AddedByUserId { get; set; }

        public virtual ApplicationUser AddedByUser { get; set; }
    }
}
