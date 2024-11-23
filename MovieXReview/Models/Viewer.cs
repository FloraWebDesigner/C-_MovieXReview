using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieXReview.Models
{
    public class Viewer
    {
        [Key]
        public int ViewerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Identity { get; set; }
        public string? Membership { get; set; }
        public int? Age { get; set; }

        //A viewer has many tickets
        public ICollection<Ticket>? Tickets { get; set; }

        //A viewer can have many reviews
        public ICollection<Review>? Review { get; set; }
    }

    public class ViewerDto
    {
        public int ViewerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Identity { get; set; }
        public string? Membership { get; set; }
        public int? Age { get; set; }
    }


}
