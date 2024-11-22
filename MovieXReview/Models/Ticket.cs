using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieXReview.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }
        public int TicketNo { get; set; }

        //Each ticket belongs to one movie
        public virtual Movie Movie { get; set; }

        //Each ticket belongs to one viewer
        public virtual Viewer Viewer { get; set; }
    }

    public class TicketDto
    {
        public int? TicketId { get; set; }
        public int TicketNo { get; set; }

        public int MovieId { get; set; }

        public int ViewerId { get; set; }

        //flattened from Ticket -> Movie
        public string? MovieName { get; set; }
        public int TicketQuantity { get; set; }

        //flattened from Ticket -> Viewer: first_name + last_name
        public string? ViewerName { get; set; }
        public string? Identity { get; set; }


    }
}
