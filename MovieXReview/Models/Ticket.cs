using System.ComponentModel.DataAnnotations;
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
}
