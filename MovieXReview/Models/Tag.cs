using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
namespace MovieXReview.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        public required string TagName { get; set; }

        public required string TagColor { get; set; }

        // Many-to-many relationship with Movie through TagMovie
        public ICollection<Movie>? Movie { get; set; }
    }
    public class TagDto
    {
        public int TagId { get; set; }

        public required string TagName { get; set; }

        public required string TagColor { get; set; }


    }
}
