using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace MovieXReview.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public required string ReviewTitle { get; set; }
        public required string ReviewContent { get; set; }

        public float Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ImageTotal { get; set; }

        // A review belongs to one movie
        public virtual Movie Movie { get; set; }
        public int MovieId { get; set; }

        // A review belongs to a viewer
        public virtual Viewer Viewer { get; set; }
        //public IdentityUser Viewer { get; set; }
        public int ViewerId { get; set; }
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public required string ReviewTitle { get; set; }
        public required string ReviewContent { get; set; }
        public float Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ImageTotal { get; set; }


        public int MovieId { get; set; }
        public string? MovieName { get; set; }
        public int ViewerId { get; set; }

        public string? ViewerName { get; set; }
        public string? Identity { get; set; }

    }

}

    