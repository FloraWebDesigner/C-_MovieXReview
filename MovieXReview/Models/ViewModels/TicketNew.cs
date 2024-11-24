namespace MovieXReview.Models.ViewModels
{
    public class TicketNew
    {
        public TicketDto Ticket { get; set; }

        public IEnumerable<MovieDto> AllMovies { get; set; }

        public IEnumerable<ViewerDto> AllViewers { get; set; }
    }
}
