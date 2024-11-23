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
        public required virtual Movie Movie { get; set; }
        public int MovieId { get; set; }
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public required string ReviewTitle { get; set; }
        public required string ReviewContent { get; set; }
        public float Rate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ImageTotal { get; set; }

    }

}

    