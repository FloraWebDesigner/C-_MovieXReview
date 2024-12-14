using System.ComponentModel.DataAnnotations;

namespace MovieXReview.Models
{
    public class CustomerDto
    {
        public required string CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }
    }
}
