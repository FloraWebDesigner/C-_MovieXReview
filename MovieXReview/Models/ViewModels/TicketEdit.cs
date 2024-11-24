namespace MovieXReview.Models.ViewModels
{
    public class TicketEdit
    {
        public required TicketDto Ticket { get; set; }

        //choose which movie the ticket refers
        public required IEnumerable<MovieDto> MovieOptions { get; set; }

        //choose which viewer the ticket refers
        public required IEnumerable<ViewerDto> ViewerOptions { get; set; }
    }
}
