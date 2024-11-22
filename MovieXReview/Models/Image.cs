using System.ComponentModel.DataAnnotations;
namespace MovieXReview.Models
{
    public class Image
    {
        [Key]
        public int ImageID { get; set; }
        public DateTime UploadedAt { get; set; }

        public required string FileName { get; set; }

        public bool HasPic { get; set; } = false;

        // images stored in /wwwroot/images/projects/{ImageId}.{PicExtension}
        public string? PicExtension { get; set; }
    }
}
