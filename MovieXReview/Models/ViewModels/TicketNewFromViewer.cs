namespace MovieXReview.Models.ViewModels
{
    public class TicketNewFromViewer
    {
        public IEnumerable<MovieDto> AllMovies { get; set; }

        public required ViewerDto ViewerDto { get; set; }

        public TicketDto Ticket { get; set; }
    }
}
