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

        // Add these properties for relationships
        public int MovieId { get; set; } // To associate with a movie
        public int ViewerId { get; set; }
    }

}

    