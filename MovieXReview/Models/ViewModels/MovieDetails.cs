namespace MovieXReview.Models.ViewModels
{
    public class MovieDetails
    {
        // MovieList
        public required MovieDto Movie { get; set; }

        // All tickets for this movie
        public IEnumerable<TicketDto>? MovieTickets { get; set; }

        // All viewers for this movie
        public IEnumerable<ViewerDto>? MovieViewers { get; set; }

    }
}
