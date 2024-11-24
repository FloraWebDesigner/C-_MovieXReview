using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
namespace MovieXReview.Models
{
    public class MovieImage
    {
        [Key]
        public int ImageId { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }

        public bool HasPic { get; set; } = false;

        // images stored in /wwwroot/images/movies/{ImageId}.{PicExtension}
        public string? PicExtension { get; set; }

        //An image belongs to one movie
        public required virtual Movie Movie { get; set; }
        public int MovieId { get; set; }
    }

    public class ImagesDto
    {
        public int ImageId { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }
        public int MovieId { get; set; }

        public string? MovieName { get; set; }
        public bool HasPic { get; set; }
        public string? PicExtension { get; set; }
    }
}
